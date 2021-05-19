using DBNet.Forms;
using Iersera.Model;
using Iersera.Support;
using System;
using System.Linq;
using static Globals;
using static Robots;
using static SimOptModule;

internal static class DNAExecution
{
    public static var_[] sysvar = new var_[1001];
    public static var_[] sysvarIN = new var_[256];
    public static var_[] sysvarOUT = new var_[256];
    private const int MaxIntValue = 2000000000;
    private const bool NextBody = true;
    private const bool NextElse = false;
    private static readonly SafeStack<bool> BoolStack = new() { DefaultValue = true };
    private static readonly SafeStack<int> IntStack = new() { DefaultValue = 0 };
    private static bool CurrentCondFlag = false;
    private static FlowState CurrentFlow = FlowState.Clear;
    private static int CurrentGene = 0;
    private static bool InGene = false;

    public static void ExecRobs()
    {
        foreach (var rob in rob.Where(r => r.exist && !r.Corpse && !r.DisableDNA && !(r.FName == "Base.txt" && hidepred)))
            ExecuteDNA(rob);
    }

    private static bool AddupCond()
    {
        if (BoolStack.Count == 0)
            return true;

        var res = true;

        while (BoolStack.Count > 0)
            res &= BoolStack.Pop();

        return res;
    }

    private static void CheckTieAngTieLenAddress(robot rob, int address)
    {
        for (var k = 0; k < 4; k++)
        {
            if (address == 480 + k)
                rob.TieAngOverwrite[k] = true;
            if (address == 484 + k)
                rob.TieLenOverwrite[k] = true;
        }
    }

    private static int Clamp(int val, int max = MaxIntValue)
    {
        return Math.Clamp(val, max, -max);
    }

    private static void ExecuteAdvancedCommand(robot rob, int n)
    {
        if (n < 1 || n > 12)
            return;

        int a, b, e;
        double c, d;

        rob.nrg -= SimOpts.Costs[ADCMDCOST] * SimOpts.Costs[COSTMULTIPLIER];

        switch (n)
        {
            case 1: // findang
                b = IntStack.Pop();
                a = IntStack.Pop();
                c = rob.pos.X / Form1.instance.xDivisor;
                d = rob.pos.Y / Form1.instance.yDivisor;
                e = (int)Physics.angnorm(Physics.angle(c, d, a, b)) * 200;
                IntStack.Push(e);
                break;

            case 2: // finddist
                b = IntStack.Pop();
                a = IntStack.Pop();
                c = rob.pos.X;
                d = rob.pos.Y;
                e = Clamp((int)Math.Sqrt(Math.Pow(c - a * Form1.instance.xDivisor, 2) + Math.Pow(d - b * Form1.instance.yDivisor, 2)));
                IntStack.Push(e);
                break;

            case 3: // ceil
                b = IntStack.Pop() % MaxIntValue;
                a = IntStack.Pop() % MaxIntValue;
                IntStack.Push(Math.Min(a, b));
                break;

            case 4: // floor
                b = IntStack.Pop() % MaxIntValue;
                a = IntStack.Pop() % MaxIntValue;
                IntStack.Push(Math.Max(a, b));
                break;

            case 5: // sqr
                a = IntStack.Pop() % MaxIntValue;
                IntStack.Push(a > 0 ? (int)Math.Sqrt(a) : 0);
                break;

            case 6: // power
                b = IntStack.Pop() % MaxIntValue;
                a = IntStack.Pop() % MaxIntValue;
                IntStack.Push(Clamp((int)Math.Pow(a, b)));
                break;

            case 7: // pyth
                b = IntStack.Pop() % MaxIntValue;
                a = IntStack.Pop() % MaxIntValue;
                IntStack.Push(Clamp((int)Math.Sqrt(a * a + b * b)));
                break;

            case 8: // cmp
                b = IntStack.Pop();
                a = IntStack.Pop();

                b %= 1256;
                if (b < 0)
                    b += 1256;

                a %= 1256;
                if (a < 0)
                    a += 1256;

                c = Physics.AngDiff(a / 200, b / 200) * 200;

                IntStack.Push((int)c);
                break;

            case 9:// a ^ (1/b)
                b = Math.Abs(IntStack.Pop());
                a = Math.Abs(IntStack.Pop());
                IntStack.Push(b == 0 ? 0 : (int)Math.Pow(a, 1 / b));
                break;

            case 10: // log(a) / Log(b)
                b = Math.Abs(IntStack.Pop());
                a = Math.Abs(IntStack.Pop());
                c = Math.Log(a, b);

                if (double.IsNaN(c) && double.IsInfinity(c))
                    c = 0;

                IntStack.Push((int)c);
                break;

            case 11:
                IntStack.Push((int)(Math.Sin(IntStack.Pop() / 200) * 32000));
                break;

            case 12:
                IntStack.Push((int)(Math.Cos(IntStack.Pop() / 200) * 32000));
                break;
        }
    }

    private static void ExecuteBasicCommand(robot rob, int n)
    {
        if (n < 1 || n > 14)
            return;

        int a, b;

        rob.nrg -= SimOpts.Costs[BCCMDCOST] * SimOpts.Costs[COSTMULTIPLIER];

        switch (n)
        {
            case 1: // add
                b = IntStack.Pop() % MaxIntValue;
                a = IntStack.Pop() % MaxIntValue;
                IntStack.Push(Clamp(a + b));
                break;

            case 2: // sub
                b = IntStack.Pop() % MaxIntValue;
                a = IntStack.Pop() % MaxIntValue;
                IntStack.Push(Clamp(a - b));
                break;

            case 3: // mult
                b = IntStack.Pop() % MaxIntValue;
                a = IntStack.Pop() % MaxIntValue;
                IntStack.Push(Clamp(a * b));
                break;

            case 4: // div
                b = IntStack.Pop() % MaxIntValue;
                a = IntStack.Pop() % MaxIntValue;
                IntStack.Push(b == 0 ? 0 : a / b);
                break;

            case 5: // rnd
                a = IntStack.Pop() % MaxIntValue;
                IntStack.Push(ThreadSafeRandom.Local.Next(0, a));
                break;

            case 6: // dereference AKA *
                a = NormaliseMemoryAddress(IntStack.Pop());
                IntStack.Push(rob.mem[a]);
                break;

            case 7: // mod
                b = IntStack.Pop() % MaxIntValue;
                a = IntStack.Pop() % MaxIntValue;
                IntStack.Push(b == 0 ? 0 : a % b);
                break;

            case 8: // sgn
                a = IntStack.Pop();
                IntStack.Push(Math.Sign(a));
                break;

            case 9: // absolute value
                IntStack.Push(Math.Abs(IntStack.Pop()));
                break;

            case 10: // dup or dupint
                IntStack.Push(IntStack.Peek());
                break;

            case 11: // dropint - Drops the top value on the Int stack
                IntStack.Pop();
                break;

            case 12: // clearint - Clears the Int stack
                IntStack.Clear();
                break;

            case 13: // swapint - Swaps the top two values on the Int stack
                IntStack.Swap();
                break;

            case 14: // overint - a b -> a b a  Dups the second value on the Int stack
                IntStack.Over();
                break;
        }
    }

    private static void ExecuteBitwiseCommand(robot rob, int n)
    {
        if (n < 1 || n > 9)
            return;

        int a, b;

        rob.nrg -= SimOpts.Costs[BTCMDCOST] * SimOpts.Costs[COSTMULTIPLIER];

        switch (n)
        {
            case 1: // Compliment ~ (tilde)
                a = IntStack.Pop();
                IntStack.Push(~a);
                break;

            case 2: // & And
                b = IntStack.Pop();
                a = IntStack.Pop();
                IntStack.Push(a & b);
                break;

            case 3: // | or
                b = IntStack.Pop();
                a = IntStack.Pop();
                IntStack.Push(a | b);
                break;

            case 4: // XOR ^
                b = IntStack.Pop();
                a = IntStack.Pop();
                IntStack.Push(a ^ b);
                break;

            case 5: // bitinc ++
                a = IntStack.Pop();
                IntStack.Push(a + 1);
                break;

            case 6: // bitdec --
                a = IntStack.Pop();
                IntStack.Push(a - 1);
                break;

            case 7: // negate
                a = IntStack.Pop();
                IntStack.Push(-a);
                break;

            case 8: // <<
                a = IntStack.Pop();
                IntStack.Push(a << 1);
                break;

            case 9: // >>
                a = IntStack.Pop();
                IntStack.Push(a >> 1);
                break;
        }
    }

    private static void ExecuteConditions(robot rob, int n)
    {
        if (n < 1 || n > 10)
            return;

        int a, b, c, d;

        rob.nrg -= SimOpts.Costs[CONDCOST] * SimOpts.Costs[COSTMULTIPLIER];

        switch (n)
        {
            case 1:  // <
                BoolStack.Push(IntStack.Pop() > IntStack.Pop());
                break;

            case 2: // >
                BoolStack.Push(IntStack.Pop() < IntStack.Pop());
                break;

            case 3: // =
                BoolStack.Push(IntStack.Pop() == IntStack.Pop());
                break;

            case 4: // <>, !=
                BoolStack.Push(IntStack.Pop() != IntStack.Pop());
                break;

            case 5: // %=
                b = IntStack.Pop();
                a = IntStack.Pop();
                c = a / 10;
                BoolStack.Push(!(a + c < b || a - c > b));
                break;

            case 6: // !%=
                b = IntStack.Pop();
                a = IntStack.Pop();
                c = a / 10;
                BoolStack.Push(a + c < b || a - c > b);
                break;

            case 7: // ~=
                d = IntStack.Pop();
                b = IntStack.Pop();
                a = IntStack.Pop();
                c = a / 100 * d;
                BoolStack.Push(!(a + c < b || a - c > b));
                break;

            case 8: // !~=
                d = IntStack.Pop();
                b = IntStack.Pop();
                a = IntStack.Pop();
                c = a * d / 100;
                BoolStack.Push(a + c < b || a - c > b);
                break;

            case 9: // >=
                BoolStack.Push(IntStack.Pop() <= IntStack.Pop());
                break;

            case 10: // <=
                BoolStack.Push(IntStack.Pop() >= IntStack.Pop());
                break;
        }
    }

    private static void ExecuteDNA(robot rob)
    {
        CurrentGene = 0;
        CurrentCondFlag = NextBody;
        InGene = false;
        IntStack.Clear();
        BoolStack.Clear();

        rob.condnum = 0;

        foreach (var block in rob.dna)
        {
            if (block.tipo == 10 && block.value == 1)
                break;

            switch (block.tipo)
            {
                case 0:
                    if (CurrentFlow == FlowState.Clear)
                        break;

                    IntStack.Push(block.value);
                    rob.nrg -= SimOpts.Costs[NUMCOST] * SimOpts.Costs[COSTMULTIPLIER];
                    break;

                case 1:
                    if (CurrentFlow == FlowState.Clear)
                        break;

                    var b = NormaliseMemoryAddress(block.value);
                    IntStack.Push(rob.mem[b]);
                    rob.nrg -= SimOpts.Costs[DOTNUMCOST] * SimOpts.Costs[COSTMULTIPLIER];
                    break;

                case 2:
                    if (CurrentFlow != FlowState.Clear)
                        ExecuteBasicCommand(rob, block.value);
                    break;

                case 3:
                    if (CurrentFlow != FlowState.Clear)
                        ExecuteAdvancedCommand(rob, block.value);
                    break;

                case 4:
                    if (CurrentFlow != FlowState.Clear)
                        ExecuteBitwiseCommand(rob, block.value);
                    break;

                case 5:
                    if (CurrentFlow == FlowState.Condition || CurrentFlow == FlowState.Body || CurrentFlow == FlowState.ElseBody)
                        ExecuteConditions(rob, block.value);
                    break;

                case 6:
                    if (CurrentFlow == FlowState.Condition || CurrentFlow == FlowState.Body || CurrentFlow == FlowState.ElseBody)
                        ExecuteLogic(rob, block.value);
                    break;

                case 7:
                    if ((CurrentFlow == FlowState.Body || CurrentFlow == FlowState.ElseBody) && BoolStack.Peek())
                        ExecuteStores(rob, block.value);
                    break;

                case 9:
                    if (!ExecuteFlowCommands(rob, block.value))
                        rob.condnum++;
                    rob.mem[thisgene] = CurrentGene;
                    break;
            }
        }

        CurrentFlow = FlowState.Clear; // EricL 4/15/2006 Do this so next bot doesn't inherit the flow control
    }

    private static bool ExecuteFlowCommands(robot rob, int n)
    {
        rob.nrg -= SimOpts.Costs[FLOWCOST] * SimOpts.Costs[COSTMULTIPLIER];

        var condFound = false;

        switch (n)
        {
            case 1:
                CurrentFlow = FlowState.Condition;
                CurrentGene++;
                BoolStack.Clear();
                InGene = true;
                break;

            case 2:
                //this is supposed to come before case 2 and 3, since these commands
                //must be executed before start and else have a chance to go
                condFound = true;
                if (CurrentFlow == FlowState.Condition)
                    CurrentCondFlag = AddupCond();

                if (!InGene)
                    CurrentCondFlag = NextBody;

                CurrentFlow = FlowState.Clear;
                switch (n)
                {
                    case 2:
                        if (!InGene)
                            CurrentGene++;
                        InGene = false;
                        if (CurrentCondFlag == NextBody)
                            CurrentFlow = FlowState.Body;
                        break;

                    case 3:
                        if (CurrentCondFlag == NextElse)
                            CurrentFlow = FlowState.ElseBody;
                        if (!InGene)
                            CurrentGene++;
                        InGene = false;
                        break;

                    case 4:
                        InGene = false;
                        CurrentFlow = FlowState.Clear;
                        break;
                }
                break;
        }
        return condFound;
    }

    private static void ExecuteLogic(robot rob, int n)
    {
        rob.nrg -= SimOpts.Costs[LOGICCOST] * SimOpts.Costs[COSTMULTIPLIER];

        bool a, b;

        switch (n)
        {
            case 1:
                b = BoolStack.Pop();
                a = BoolStack.Pop();
                BoolStack.Push(a && b);
                break;

            case 2:
                b = BoolStack.Pop();
                a = BoolStack.Pop();
                BoolStack.Push(a || b);
                break;

            case 3:
                b = BoolStack.Pop();
                a = BoolStack.Pop();
                BoolStack.Push(a ^ b);
                break;

            case 4:
                b = BoolStack.Pop();
                BoolStack.Push(!b);
                break;

            case 5:
                BoolStack.Push(true);
                break;

            case 6:
                BoolStack.Push(false);
                break;

            case 7:
                BoolStack.Pop();
                break;

            case 8:
                BoolStack.Clear();
                break;

            case 9:
                if (BoolStack.Count == 0)
                    return;

                BoolStack.Push(BoolStack.Peek());
                break;

            case 10:
                BoolStack.Swap();
                break;

            case 11:
                BoolStack.Over();
                break;
        }
    }

    private static void ExecuteStores(robot rob, int n)
    {
        if (n < 1 || n > 14)
            return;

        var b = IntStack.Pop();

        if (b == 0)
            return;

        b = NormaliseMemoryAddress(b);
        CheckTieAngTieLenAddress(rob, b);

        int a;

        switch (n)
        {
            case 1:
                rob.mem[b] = Mod32000(IntStack.Pop());
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER];
                break;

            case 2:
                rob.mem[b] = Mod32000(rob.mem[b] + 1);
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 10;
                break;

            case 3:
                rob.mem[b] = Mod32000(rob.mem[b] - 1);
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 10;
                break;

            case 4:
                a = IntStack.Pop();
                rob.mem[b] = Mod32000(rob.mem[b] + a);
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 5;
                break;

            case 5:
                a = IntStack.Pop();
                rob.mem[b] = Mod32000(rob.mem[b] - a);
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 5;
                break;

            case 6:
                a = IntStack.Pop();
                rob.mem[b] = Mod32000(rob.mem[b] * Mod32000(a));
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 5;
                break;

            case 7:
                a = IntStack.Pop();
                rob.mem[b] = a == 0 ? 0 : rob.mem[b] / a;
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 5;
                break;

            case 8:
                a = IntStack.Pop();
                rob.mem[b] = Mod32000(Math.Min(rob.mem[b], a));
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 5;
                break;

            case 9:
                a = IntStack.Pop();
                rob.mem[b] = Mod32000(Math.Max(rob.mem[b], a));
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 5;
                break;

            case 10:
                rob.mem[b] = ThreadSafeRandom.Local.Next(0, Math.Abs(rob.mem[b])) * Math.Sign(rob.mem[b]);
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 7;
                break;

            case 11:
                rob.mem[b] = Math.Sign(rob.mem[b]);
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 7;
                break;

            case 12:
                rob.mem[b] = Math.Abs(rob.mem[b]);
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 8;
                break;

            case 13:
                rob.mem[b] = rob.mem[b] > 0 ? (int)Math.Sqrt(rob.mem[b]) : 0;
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 7;
                break;

            case 14:
                rob.mem[b] = -rob.mem[b];
                rob.nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 8;
                break;
        }
    }

    private static int Mod32000(int a)
    {
        if (a > 0)
        {
            a %= 32000;
            if (a == 0)
                a = 32000;
        }
        else if (a < 0)
        {
            a %= 32000;
            if (a == 0)
                a = -32000;
        }

        return a;
    }

    private static int NormaliseMemoryAddress(int a)
    {
        a = Math.Abs(a) % MaxMem;
        if (a == 0)
            a = 1000;

        return a;
    }
}
