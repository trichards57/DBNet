using DBNet.Forms;
using Iersera.Model;
using System;
using System.Collections.Generic;
using static Common;
using static DNAManipulations;
using static Globals;
using static Microsoft.VisualBasic.Information;
using static Robots;
using static SimOptModule;
using static System.Math;
using static VBExtension;

internal static class DNAExecution
{
    public const byte body = 2;

    public const byte CLEAR = 0;

    public const byte COND = 1;

    public const byte ELSEBODY = 3;

    public const bool NEXTBODY = true;

    //both of these two are subsets of the clear flag technically
    public const bool NEXTELSE = false;

    public const int stacklim = 100;

    public static SafeStack<bool> Condst = new() { DefaultValue = true };

    public static bool DisplayActivations = false;

    //EricL - Toggle for displaying activations in the consol
    //Indicates whether the cycle was executed from a console
    public static bool ingene = false;

    public static SafeStack<int> IntStack = new() { DefaultValue = 0 };

    public static var_[] sysvar = new var_[1001];

    // array of system variables
    public static var_[] sysvarIN = new var_[256];

    // array of system variables informational
    public static var_[] sysvarOUT = new var_[256];

    //for the conditions stack
    private static readonly List<Queue> CommandQueue = new();

    private static int currbot = 0;

    private static bool CurrentCondFlag = false;

    private static byte CurrentFlow = 0;

    private static int currgene = 0;

    public static void ExecRobs()
    {
        for (var t = 1; t < MaxRobs; t++)
        {
            if (!rob[t].exist || rob[t].Corpse || rob[t].DisableDNA || rob[t].FName == "Base.txt" && hidepred)
                continue;

            ExecuteDNA(t);

            if (rob[t].console != null && DisplayActivations)
            {
                rob[t].console.textout("");
                rob[t].console.textout("***ROBOT GENES EXECUTION***"); //Botsareus 3/24/2012 looks a little better now
                for (var k = 1; k < rob[t].genenum; k++)
                {
                    if (rob[t].ga[k])
                        rob[t].console.textout(CStr(k) + " executed");
                    else
                        rob[t].console.textout(CStr(k) + " not executed"); //Botsareus 3/24/2012 looks a little better now
                }
            }

            if (t == robfocus && ActivForm.instance.Visibility == System.Windows.Visibility.Visible)
                exechighlight(t);
        }
    }

    private static bool AddupCond()
    {
        if (Condst.Count == 0)
            return true;

        var res = true;

        while (Condst.Count > 0)
            res &= Condst.Pop();

        return res;
    }

    private static void cdiff()
    {
        var b = IntStack.Pop();
        var a = IntStack.Pop();
        var c = a / 10;
        Condst.Push(a + c < b || a - c > b);
    }

    private static void cequa()
    {
        var b = IntStack.Pop();
        var a = IntStack.Pop();
        var c = a / 10;
        Condst.Push(!(a + c < b || a - c > b));
    }

    private static void CheckTieAngTieLenAddress(int b)
    {
        for (var k = 0; k < 4; k++)
        {
            if (b == 480 + k)
                rob[currbot].TieAngOverwrite[k] = true;
            if (b == 484 + k)
                rob[currbot].TieLenOverwrite[k] = true;
        }
    }

    private static int Clamp(int val, int max = 2000000000) => Clamp(val, max, -max);

    private static int Clamp(int val, int max, int min) => Math.Min(Max(val, min), max);

    private static void customcdiff()
    {
        var d = IntStack.Pop();
        var b = IntStack.Pop();
        var a = IntStack.Pop();

        var c = a * d / 100;

        Condst.Push(a + c < b || a - c > b);
    }

    private static void customcequa()
    {
        //usage: 10 20 30 ~= are 10 and 20 within 30 percent of each other?

        var d = IntStack.Pop();
        var b = IntStack.Pop();
        var a = IntStack.Pop();

        var c = a / 100 * d;

        Condst.Push(!(a + c < b || a - c > b));
    }

    private static void DNAabsstore()
    {
        var a = IntStack.Pop();

        if (a == 0)
            return;

        a = NormaliseMemoryAddress(a);

        rob[currbot].mem[a] = Abs(rob[currbot].mem[a]);
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 8;
    }

    private static void DNAadd()
    {
        var b = IntStack.Pop();
        var a = IntStack.Pop();

        a %= 2000000000;
        b %= 2000000000;

        IntStack.Push(Clamp(a + b));
    }

    private static void DNAaddstore()
    {
        var b = IntStack.Pop(); // Pop the stack and get the mem location to store to

        if (b == 0)
            return;

        b = NormaliseMemoryAddress(b);

        var a = IntStack.Pop() + rob[currbot].mem[b];

        //Botsareus 3/22/2013 handle tieang...tielen 1...4 overwrites

        CheckTieAngTieLenAddress(b);

        rob[currbot].mem[b] = mod32000(a);
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 5;
    }

    private static void DNAanglecmp()
    {
        var b = IntStack.Pop();
        var a = IntStack.Pop();

        //Botsareus 10/5/2015 Value normalization
        b %= 1256;
        if (b < 0)
            b += 1256;

        a %= 1256;
        if (a < 0)
            a += 1256;

        var c = AngDiff(a / 200, b / 200) * 200;

        IntStack.Push(c);
    }

    private static void DNAceilstore()
    {
        var b = IntStack.Pop(); // Pop the stack and get the mem location to store to
        var c = IntStack.Pop();

        if (b == 0)
            return;

        b = NormaliseMemoryAddress(b);
        CheckTieAngTieLenAddress(b);

        rob[currbot].mem[b] = mod32000(Math.Min(c, rob[currbot].mem[b]));
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 5;
    }

    private static void DNAdebugbool(int at_position)
    { 
        var a = Condst.Pop();

        rob[currbot].dbgstring = $"{rob[currbot].dbgstring}\n{a} at position {at_position}";

        Condst.Push(a);
    }

    private static void DNAdebugint(int at_position)
    { 
        var a = IntStack.Pop();

        rob[currbot].dbgstring = $"{rob[currbot].dbgstring}\n{a} at position {at_position}";

        IntStack.Push(a);
    }

    private static void DNAdec()
    {
        var a = IntStack.Pop();
        if (a == 0)
            return;

        a = NormaliseMemoryAddress(a);

        rob[currbot].mem[a] = mod32000(rob[currbot].mem[a] - 1);
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 10;
    }

    private static void DNAderef()
    {
        var a = IntStack.Pop();

        a = NormaliseMemoryAddress(a);

        IntStack.Push(rob[currbot].mem[a]);
    }

    private static void DNAdiv()
    {
        var b = IntStack.Pop();
        var a = IntStack.Pop();

        IntStack.Push(b == 0 ? 0 : a / b);
    }

    private static void DNAdivstore()
    {
        var b = IntStack.Pop(); // Pop the stack and get the mem location to store to
        var c = IntStack.Pop();

        if (b == 0)
            return;

        b = NormaliseMemoryAddress(b);
        CheckTieAngTieLenAddress(b);

        rob[currbot].mem[b] = c == 0 ? 0 : rob[currbot].mem[b] / c;
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 5;
    }

    private static void DNAfloorstore()
    {
        var b = IntStack.Pop();
        var c = IntStack.Pop();

        if (b == 0)
            return;

        b = NormaliseMemoryAddress(b);
        CheckTieAngTieLenAddress(b);

        rob[currbot].mem[b] = mod32000(Max(c, rob[currbot].mem[b]));
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 5;
    }

    private static void DNAinc()
    {
        var a = IntStack.Pop();

        if (a == 0)
            return;

        a = NormaliseMemoryAddress(a);

        rob[currbot].mem[a] = mod32000(rob[currbot].mem[a] + 1);
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 10;
    }

    private static void DNAlogx()
    {
        var b = Abs(IntStack.Pop());
        var a = Abs(IntStack.Pop());

        var c = Log(a, b);

        if (double.IsNaN(c) && double.IsInfinity(c))
            c = 0;

        IntStack.Push((int)c);
    }

    private static void DNAmod()
    {
        var b = IntStack.Pop();
        var a = IntStack.Pop();

        IntStack.Push(b == 0 ? 0 : a % b);
    }

    private static void DNAmultstore()
    {
        var b = IntStack.Pop(); // Pop the stack and get the mem location to store to

        if (b == 0)
            return;

        b = NormaliseMemoryAddress(b);
        CheckTieAngTieLenAddress(b);

        var c = mod32000(IntStack.Pop());

        rob[currbot].mem[b] = mod32000(rob[currbot].mem[b] * c);
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 5;
    }

    private static void DNAnegstore()
    {
        var a = IntStack.Pop();

        if (a == 0)
            return;

        a = NormaliseMemoryAddress(a);

        rob[currbot].mem[a] = -rob[currbot].mem[a];
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 8;
    }

    private static void DNApow()
    {
        var b = IntStack.Pop();
        var a = IntStack.Pop();

        b = Clamp(b, 10);

        IntStack.Push(Clamp((int)Pow(a, b)));
    }

    private static void DNApyth()
    {
        var b = IntStack.Pop();
        var a = IntStack.Pop();

        var c = Clamp((int)Sqrt(a * a + b * b));

        IntStack.Push(c);
    }

    private static void DNArndstore()
    {
        var a = IntStack.Pop();

        if (a == 0)
            return;

        a = NormaliseMemoryAddress(a);

        rob[currbot].mem[a] = Random(0, Abs(rob[currbot].mem[a])) * Sign(rob[currbot].mem[a]);
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 7;
    }

    private static void DNAroot()
    {
        var b = Abs(IntStack.Pop());
        var a = Abs(IntStack.Pop());

        IntStack.Push(b == 0 ? 0 : (int)Pow(a, 1 / b));
    }

    private static void DNAsgnstore()
    {
        var a = IntStack.Pop();

        if (a == 0)
            return;

        a = NormaliseMemoryAddress(a);

        rob[currbot].mem[a] = Sign(rob[currbot].mem[a]);
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 7;
    }

    private static void DNASqr()
    {
        var a = IntStack.Pop();

        IntStack.Push(a > 0 ? (int)Sqrt(a) : 0);
    }

    private static void DNAsqrstore()
    {
        var a = IntStack.Pop();

        if (a == 0)
            return;

        a = NormaliseMemoryAddress(a);

        rob[currbot].mem[a] = rob[currbot].mem[a] > 0 ? (int)Sqrt(rob[currbot].mem[a]) : 0;
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 7;
    }

    private static void DNAstore()
    {
        var b = IntStack.Pop(); // Pop the stack and get the mem location to store to

        if (b == 0)
            return;

        b = NormaliseMemoryAddress(b);
        CheckTieAngTieLenAddress(b);

        rob[currbot].mem[b] = mod32000(IntStack.Pop());
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER];
    }

    private static void DNASub()
    {
        var b = IntStack.Pop();
        var a = IntStack.Pop();

        a %= 2000000000;
        b %= 2000000000;

        IntStack.Push(Clamp(a - b));
    }

    private static void DNAsubstore()
    {
        var b = IntStack.Pop(); // Pop the stack and get the mem location to store to

        if (b == 0)
            return;

        b = NormaliseMemoryAddress(b);
        CheckTieAngTieLenAddress(b);

        rob[currbot].mem[b] = mod32000(rob[currbot].mem[b] - IntStack.Pop());
        rob[currbot].nrg -= SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER] / 5;
    }

    private static void ExecuteAdvancedCommand(int n, int at_position)
    {
        if (n < 13)
            rob[currbot].nrg -= SimOpts.Costs[ADCMDCOST] * SimOpts.Costs[COSTMULTIPLIER];

        switch (n)
        {
            case 1:  //findang
                findang();
                break;//finddist
            case 2:
                finddist();
                break;//ceil
            case 3:
                IntStack.Push(Math.Min(IntStack.Pop(), IntStack.Pop()));
                break;//floor
            case 4:
                IntStack.Push(Max(IntStack.Pop(), IntStack.Pop()));
                break;// sqr
            case 5:
                DNASqr();
                break;// power
            case 6:
                DNApow();
                break;// pyth
            case 7:
                DNApyth();
                break;

            case 8:
                DNAanglecmp();
                break;

            case 9:
                DNAroot(); // a ^ (1/b)
                break;

            case 10:
                DNAlogx(); // log(a) / Log(b)
                break;

            case 11:
                IntStack.Push((int)(Sin(IntStack.Pop() / 200) * 32000));
                break;

            case 12:
                IntStack.Push((int)(Cos(IntStack.Pop() / 200) * 32000));
                break;

            case 13:
                DNAdebugint(at_position); //Botsareus 1/31/2013 the new debugint command
                break;

            case 14:
                DNAdebugbool(at_position); //Botsareus 1/31/2013 the new debugbool command
                break;
        }
    }

    private static void ExecuteBasicCommand(int n)
    {
        //& denotes commands that can be constructed from other commands, but
        //are still basic enough to be listed here

        rob[currbot].nrg -= SimOpts.Costs[BCCMDCOST] * SimOpts.Costs[COSTMULTIPLIER];

        switch (n)
        {
            case 1:  //add
                DNAadd();
                break;//sub (negative add) &
            case 2:
                DNASub();
                break;//mult
            case 3:
                IntStack.Push(Clamp(IntStack.Pop() * IntStack.Pop()));
                break;//div
            case 4:
                DNAdiv();
                break;//rnd
            case 5:
                IntStack.Push(Random(0, IntStack.Pop()));
                break;//dereference AKA *
            case 6:
                DNAderef();
                break;//mod
            case 7:
                DNAmod();
                break;//sgn
            case 8:
                IntStack.Push(Sign(IntStack.Pop()));
                break;//absolute value &
            case 9:
                IntStack.Push(Abs(IntStack.Pop()));
                break;//dup or dupint
            case 10:
                IntStack.Push(IntStack.Peek());
                break;//dropint - Drops the top value on the Int stack
            case 11:
                IntStack.Pop();
                break;//clearint - Clears the Int stack
            case 12:
                IntStack.Clear();
                break;//swapint - Swaps the top two values on the Int stack
            case 13:
                SwapIntStack();
                break;//overint - a b -> a b a  Dups the second value on the Int stack
            case 14:
                OverIntStack();
                break;
        }
    }

    private static void ExecuteBitwiseCommand(int n)
    {
        rob[currbot].nrg -= SimOpts.Costs[BTCMDCOST] * SimOpts.Costs[COSTMULTIPLIER];

        switch (n)
        {
            case 1:  //Compliment ~ (tilde)
                IntStack.Push(~IntStack.Pop());
                break;//& And
            case 2:
                IntStack.Push(IntStack.Pop() & IntStack.Pop());
                break;//| or
            case 3:
                IntStack.Push(IntStack.Pop() | IntStack.Pop());
                break;// XOR, ^ (I need another representation)
            case 4:
                IntStack.Push(IntStack.Pop() ^ IntStack.Pop());
                break;//bitinc ++
            case 5:
                IntStack.Push(IntStack.Pop() + 1);
                break;//bitdec --
            case 6:
                IntStack.Push(IntStack.Pop() - 1);
                break;//negate
            case 7:
                IntStack.Push(-IntStack.Pop());
                break;// <<
            case 8:
                IntStack.Push(IntStack.Pop() << 1);
                break;// >>
            case 9:
                IntStack.Push(IntStack.Pop() >> 1);
                break;
        }
    }

    private static void ExecuteConditions(int n)
    {
        rob[currbot].nrg -= SimOpts.Costs[CONDCOST] * SimOpts.Costs[COSTMULTIPLIER];

        switch (n)
        {
            case 1:  //<
                Condst.Push(IntStack.Pop() > IntStack.Pop());
                break;//>
            case 2:
                Condst.Push(IntStack.Pop() < IntStack.Pop());
                break;//=
            case 3:
                Condst.Push(IntStack.Pop() == IntStack.Pop());
                break;// <>, !=
            case 4:
                Condst.Push(IntStack.Pop() != IntStack.Pop());
                break;// %=
            case 5:
                cequa();
                break;//!%=
            case 6:
                cdiff();
                break;//~=
            case 7:
                customcequa();
                break;//!~=
            case 8:
                customcdiff();
                break;//>=
            case 9:
                Condst.Push(IntStack.Pop() <= IntStack.Pop());
                break;//<=
            case 10:
                Condst.Push(IntStack.Pop() >= IntStack.Pop());
                break;
        }
    }

    private static void ExecuteDNA(int n)
    {
        currbot = n;
        currgene = 0;
        CurrentCondFlag = NEXTBODY;
        ingene = false;
        IntStack.Clear();
        Condst.Clear();

        if (n == robfocus || rob[n].console != null)
        {
            rob[n].ga = new bool[rob[n].genenum];
            for (var i = 0; i < rob[n].genenum; i++)
                rob[n].ga[i] = false;
        }

        var a = 1;
        rob[n].condnum = 0;
        rob[n].dbgstring = "";

        while (!(rob[n].dna[a].tipo == 10 & rob[n].dna[a].value == 1) && a <= 32000 && a < UBound(rob[n].dna))
        {
            var tipo = rob[n].dna[a].tipo;
            switch (tipo)
            {
                case 0:
                    if (CurrentFlow != CLEAR)
                    {
                        IntStack.Push(rob[n].dna[a].value);
                        rob[n].nrg -= SimOpts.Costs[NUMCOST] * SimOpts.Costs[COSTMULTIPLIER];
                    }
                    break;

                case 1:
                    if (CurrentFlow != CLEAR)
                    {
                        var b = NormaliseMemoryAddress(rob[n].dna[a].value);
                        IntStack.Push(rob[n].mem[b]);
                        rob[n].nrg -= SimOpts.Costs[DOTNUMCOST] * SimOpts.Costs[COSTMULTIPLIER];
                    }
                    break;

                case 2:
                    if (CurrentFlow != CLEAR)
                        ExecuteBasicCommand(rob[n].dna[a].value);
                    break;

                case 3:
                    if (CurrentFlow != CLEAR)
                        ExecuteAdvancedCommand(rob[n].dna[a].value, a);
                    break;

                case 4:
                    if (CurrentFlow != CLEAR)
                        ExecuteBitwiseCommand(rob[n].dna[a].value);
                    break;

                case 5:
                    if (CurrentFlow == COND || CurrentFlow == body || CurrentFlow == ELSEBODY)
                        ExecuteConditions(rob[n].dna[a].value);
                    break;

                case 6:
                    if (CurrentFlow == COND || CurrentFlow == body || CurrentFlow == ELSEBODY)
                        ExecuteLogic(rob[n].dna[a].value);
                    break;

                case 7:
                    if (CurrentFlow == body || CurrentFlow == ELSEBODY)
                    {
                        if (Condst.Peek())
                        {
                            ExecuteStores(rob[n].dna[a].value);
                            if (n == robfocus || !(rob[n].console == null))
                                rob[n].ga[currgene] = true;
                        }
                    }
                    break;

                case 8:
                    break;

                case 9:
                    if (!ExecuteFlowCommands(rob[n].dna[a].value, n))
                        rob[n].condnum++;
                    rob[n].mem[thisgene] = currgene;
                    break;

                case 10:
                    break;
            }

            a++;
        }
        CurrentFlow = CLEAR; // EricL 4/15/2006 Do this so next bot doesn't inherit the flow control
    }

    private static bool ExecuteFlowCommands(int n, int bot)
    {
        rob[currbot].nrg -= SimOpts.Costs[FLOWCOST] * SimOpts.Costs[COSTMULTIPLIER];

        //returns true if a stop command was found (start, stop, or else)
        //returns false if cond was found
        var ExecuteFlowCommands = false;

        switch (n)
        {
            case 1:
                CurrentFlow = COND;
                currgene++;
                Condst.Clear();
                ingene = true;
                break;

            case 2:
                //this is supposed to come before case 2 and 3, since these commands
                //must be executed before start and else have a chance to go
                ExecuteFlowCommands = true;
                if (CurrentFlow == COND)
                    CurrentCondFlag = AddupCond();

                if (!ingene)
                    CurrentCondFlag = NEXTBODY;

                if (CurrentCondFlag && (CurrentFlow == ELSEBODY || CurrentFlow == body))
                {
                    // Need to check this for the case where the gene body doesn't have any stores to trigger the activation dialog
                    if (bot == robfocus || !(rob[bot].console == null))
                        rob[bot].ga[currgene] = true;
                }

                CurrentFlow = CLEAR;
                switch (n)
                {
                    case 2:
                        if (!ingene)
                            currgene++;
                        ingene = false;
                        if (CurrentCondFlag == NEXTBODY)
                            CurrentFlow = body;
                        break;

                    case 3:
                        if (CurrentCondFlag == NEXTELSE)
                            CurrentFlow = ELSEBODY;
                        if (!ingene)
                            currgene++;
                        ingene = false;
                        break;

                    case 4:
                        ingene = false;
                        CurrentFlow = CLEAR;
                        break;
                }
                break;
        }
        return ExecuteFlowCommands;
    }

    private static void ExecuteLogic(int n)
    {
        rob[currbot].nrg -= SimOpts.Costs[LOGICCOST] * SimOpts.Costs[COSTMULTIPLIER];

        bool a, b;

        switch (n)
        {
            case 1:
                b = Condst.Pop();
                a = Condst.Pop();
                Condst.Push(a && b);
                break;

            case 2:
                b = Condst.Pop();
                a = Condst.Pop();
                Condst.Push(a || b);
                break;

            case 3:
                b = Condst.Pop();
                a = Condst.Pop();
                Condst.Push(a ^ b);
                break;

            case 4:
                b = Condst.Pop();
                Condst.Push(!b);
                break;

            case 5:
                Condst.Push(true);
                break;

            case 6:
                Condst.Push(false);
                break;

            case 7:
                Condst.Pop();
                break;

            case 8:
                Condst.Clear();
                break;

            case 9:
                if (Condst.Count == 0)
                    return;

                Condst.Push(Condst.Peek());
                break;

            case 10:
                SwapBoolStack();
                break;

            case 11:
                OverBoolStack();
                break;
        }
    }

    private static void ExecuteStores(int n)
    {
        switch (n)
        {
            case 1:
                DNAstore();
                break;

            case 2:
                DNAinc();
                break;

            case 3:
                DNAdec();
                break;

            case 4:
                DNAaddstore();
                break;

            case 5:
                DNAsubstore();
                break;

            case 6:
                DNAmultstore();
                break;

            case 7:
                DNAdivstore();
                break;

            case 8:
                DNAceilstore();
                break;

            case 9:
                DNAfloorstore();
                break;

            case 10:
                DNArndstore();
                break;

            case 11:
                DNAsgnstore();
                break;

            case 12:
                DNAabsstore();
                break;

            case 13:
                DNAsqrstore();
                break;

            case 14:
                DNAnegstore();
                break;
        }
    }

    private static void findang()
    {
        var b = IntStack.Pop();
        var a = IntStack.Pop();
        var c = rob[currbot].pos.X / Form1.instance.xDivisor;
        var d = rob[currbot].pos.Y / Form1.instance.yDivisor;
        var e = angnorm(angle(c, d, a, b)) * 200;
        IntStack.Push(e);
    }

    private static void finddist()
    {
        var b = IntStack.Pop() * Form1.instance.yDivisor;
        var a = IntStack.Pop() * Form1.instance.xDivisor;
        var c = rob[currbot].pos.X;
        var d = rob[currbot].pos.Y;
        var e = Clamp(Sqrt(Pow(c - a, 2) + Pow(d - b, 2)));
        IntStack.Push(e);
    }

    private static int mod32000(int a)
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
        a = Abs(a) % MaxMem;
        if (a == 0)
            a = 1000;

        return a;
    }

    public class block
    {
        public int tipo = 0;
        public int value = 0;
    }

    public class Queue
    {
        public int memloc = 0;
        public int Memval = 0;
    }
}
