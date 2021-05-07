using DBNet.Forms;
using System.Collections.Generic;
using static Common;
using static DNAManipulations;
using static Globals;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
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

    public static boolstack Condst = null;

    public static bool DisplayActivations = false;

    //EricL - Toggle for displaying activations in the consol
    //Indicates whether the cycle was executed from a console
    public static bool ingene = false;

    public static Stack IntStack = null;

    public static var_[] sysvar = new var_[1001];

    // array of system variables
    public static var_[] sysvarIN = new var_[256];

    // array of system variables informational
    public static var_[] sysvarOUT = new var_[256];

    //for the conditions stack
    private static readonly List<Queue> CommandQueue = new List<Queue>();

    private static int currbot = 0;

    private static bool CurrentCondFlag = false;

    private static byte CurrentFlow = 0;

    private static int currgene = 0;

    public static void ExecRobs()
    {
        for (var t = 1; t < MaxRobs; t++)
        {
            if (t % 250 == 0)
            {
                DoEvents();
            }
            if (rob[t].exist && !rob[t].Corpse && !rob[t].DisableDNA && !(rob[t].FName == "Base.txt" && hidepred))
            {
                ExecuteDNA(t);
                if (!(rob[t].console == null) && DisplayActivations)
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
                if (t == robfocus && ActivForm.instance.Visible)
                {
                    exechighlight(t);
                }
            }
        }
    }

    private static bool AddupCond()
    {
        //AND together all conditions on the boolstack
        var AddupCond = true;

        var a = PopBoolStack();
        while (a != -5)
        {
            AddupCond = AddupCond && a;
            a = PopBoolStack();
        }
        return AddupCond;
    }

    private static void cdiff()
    {
        var b = PopIntStack();
        var a = PopIntStack();
        var c = a / 10;
        PushBoolStack((!((a + c >= b) && (a - c <= b))));
    }

    private static void cequa()
    {
        var b = PopIntStack();
        var a = PopIntStack();
        var c = a / 10;
        PushBoolStack(((a - c <= b) && (a + c >= b)));
    }

    private static bool CondStateIsTrue()
    {
        var a = PopBoolStack();
        if (a == -5)
            return true;

        PushBoolStack(CBool(a)); // If we popped something off the stack, push it back on

        return a;
    }

    private static void customcdiff()
    {
        var d = PopIntStack();
        var b = PopIntStack();
        var a = PopIntStack();
        var c = a / 100 * d;
        if (Abs(c) > 2000000000)
            c = Sign(c) * 2000000000;

        PushBoolStack((!((a + c >= b) && (a - c <= b))));
    }

    private static void customcequa()
    {
        //usage: 10 20 30 ~= are 10 and 20 within 30 percent of each other?

        var d = PopIntStack();
        var b = PopIntStack();
        var a = PopIntStack();
        var c = a / 100 * d;
        PushBoolStack(((a - c <= b) && (a + c >= b)));
    }

    private static void diff()
    {
        PushBoolStack((PopIntStack() != PopIntStack()));
    }

    private static void DNAabs()
    {
        PushIntStack(Abs(PopIntStack()));
    }

    private static void DNAabsstore()
    {
        var a = PopIntStack();
        if (a != 0)
        {
            a = Abs(a) % MaxMem;
            if (a == 0)
                a = 1000;

            var b = Abs(rob[currbot].mem[a]);
            rob[currbot].mem[a] = b;
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]) / 8;
        }
    }

    private static void DNAadd()
    {
        var b = PopIntStack();
        var a = PopIntStack();

        a %= 2000000000;
        b %= 2000000000;

        var c = a + b;

        if (Abs(c) > 2000000000)
            c -= Sign(c) * 2000000000;

        PushIntStack(c);
    }

    private static void DNAaddstore()
    {
        var b = PopIntStack(); // Pop the stack and get the mem location to store to
        if (b != 0)
        { // Stores to 0 are allowed, but do nothing and cost nothing
            b = Abs(b) % MaxMem; // Make sure the location hits the bot's memory to increase the chance of mutations hitting sysvars.
            if (b == 0)
                b = 1000; // Special case that multiples of 1000 should store to location 1000

            var a = PopIntStack() + rob[currbot].mem[b];

            //Botsareus 3/22/2013 handle tieang...tielen 1...4 overwrites
            byte k = 0;

            for (k = 0; k < 3; k++)
            {
                if (b == 480 + k)
                    rob[currbot].TieAngOverwrite[k] = true;
                if (b == 484 + k)
                    rob[currbot].TieLenOverwrite[k] = true;
            }

            rob[currbot].mem[b] = mod32000(a);
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]) / 5;
        }
    }

    private static void DNAanglecmp()
    { //Allowes a robot to quickly calculate the difference between two angles
        var b = PopIntStack();
        var a = PopIntStack();

        //Botsareus 10/5/2015 Value normalization
        b %= 1256;
        if (b < 0)
            b += 1256;

        a %= 1256;
        if (a < 0)
            a += 1256;

        var c = AngDiff(a / 200, b / 200) * 200;

        PushIntStack(c);
    }

    private static void DNABitwiseAND()
    {
        var valueB = PopIntStack();
        var valueA = PopIntStack();

        PushIntStack(valueA & valueB);
    }

    private static void DNABitwiseCompliment()
    {
        var value = PopIntStack();

        PushIntStack(!value);
    }

    private static void DNABitwiseDEC()
    {
        var value = PopIntStack();

        PushIntStack(value - 1);
    }

    private static void DNABitwiseINC()
    {
        var value = PopIntStack();

        PushIntStack(value + 1);
    }

    private static void DNABitwiseOR()
    {
        var valueB = PopIntStack();
        var valueA = PopIntStack();

        PushIntStack(valueA | valueB);
    }

    private static void DNABitwiseShiftLeft()
    {
        var value = PopIntStack();

        PushIntStack(value << 1);
    }

    private static void DNABitwiseShiftRight()
    {
        var value = PopIntStack();

        PushIntStack(value >> 1);
    }

    private static void DNABitwiseXOR()
    {
        var valueB = PopIntStack();
        var valueA = PopIntStack();

        PushIntStack(valueA ^ valueB);
    }

    private static void DNAceil()
    {
        double a = 0;

        double b = 0;

        b = PopIntStack();
        a = PopIntStack();

        PushIntStack(IIf(a > b, b, a));
    }

    private static void DNAceilstore()
    {
        var b = PopIntStack(); // Pop the stack and get the mem location to store to
        var c = PopIntStack();
        if (b != 0)
        { // Stores to 0 are allowed, but do nothing and cost nothing
            b = Abs(b) % MaxMem; // Make sure the location hits the bot's memory to increase the chance of mutations hitting sysvars.
            if (b == 0)
                b = 1000; // Special case that multiples of 1000 should store to location 1000

            var a = IIf(rob[currbot].mem[b] > c, c, rob[currbot].mem[b]);

            //Botsareus 3/22/2013 handle tieang...tielen 1...4 overwrites

            for (var k = 0; k < 3; k++)
            {
                if (b == 480 + k)
                    rob[currbot].TieAngOverwrite[k] = true;
                if (b == 484 + k)
                    rob[currbot].TieLenOverwrite[k] = true;
            }

            rob[currbot].mem[b] = mod32000(a);
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]) / 5;
        }
    }

    private static void DNAcos()
    {
        var a = PopIntStack();

        var b = Cos(a / 200) * 32000;

        PushIntStack(b);
    }

    private static void DNAdebugbool(ref int at_position)
    { //Botsareus 1/31/2013 The new debugbool command
        var a = PopBoolStack();

        rob[currbot].dbgstring = rob[currbot].dbgstring + vbCrLf + a + " at position " + at_position;

        PushBoolStack(a);
    }

    private static void DNAdebugint(ref int at_position)
    { //Botsareus 1/31/2013 The new debugint command 'Botsareus 4/5/2016 Cleaner architecture
        var a = PopIntStack();

        rob[currbot].dbgstring = rob[currbot].dbgstring + vbCrLf + a + " at position " + at_position;

        PushIntStack(a);
    }

    private static void DNAdec()
    {
        var a = PopIntStack();
        if (a != 0)
        {
            a = Abs(a) % MaxMem;
            if (a == 0)
                a = 1000;

            var b = rob[currbot].mem[a] - 1;
            rob[currbot].mem[a] = mod32000(b);
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]) / 10;
        }
    }

    private static void DNAderef()
    {
        var b = PopIntStack();

        b = Abs(b) % MaxMem;
        if (b == 0)
            b = 1000; // Special case that multiples of 1000 should store to location 1000

        PushIntStack(rob[currbot].mem[b]);
    }

    private static void DNAdiv()
    {
        var b = PopIntStack();
        var a = PopIntStack();
        if (b != 0)
            PushIntStack(a / b);
        else
            PushIntStack(0);
    }

    private static void DNAdivstore()
    {
        var b = PopIntStack(); // Pop the stack and get the mem location to store to
        var c = PopIntStack();
        if (b != 0)
        { // Stores to 0 are allowed, but do nothing and cost nothing
            b = Abs(b) % MaxMem; // Make sure the location hits the bot's memory to increase the chance of mutations hitting sysvars.
            int a;
            if (b == 0)
                b = 1000; // Special case that multiples of 1000 should store to location 1000

            if (c == 0)
                a = 0;
            else
                a = rob[currbot].mem[b] / c;

            //Botsareus 3/22/2013 handle tieang...tielen 1...4 overwrites

            for (var k = 0; k < 3; k++)
            {
                if (b == 480 + k)
                    rob[currbot].TieAngOverwrite[k] = true;

                if (b == 484 + k)
                    rob[currbot].TieLenOverwrite[k] = true;
            }

            rob[currbot].mem[b] = a;
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]) / 5;
        }
    }

    private static void DNAdup()
    {
        var b = PopIntStack();
        PushIntStack(b);
        PushIntStack(b);
    }

    private static void DNAfloor()
    {
        var b = PopIntStack();
        var a = PopIntStack();

        PushIntStack(IIf(a < b, b, a));
    }

    private static void DNAfloorstore()
    {
        var b = PopIntStack(); // Pop the stack and get the mem location to store to
        var c = PopIntStack();
        if (b != 0)
        { // Stores to 0 are allowed, but do nothing and cost nothing
            b = Abs(b) % MaxMem; // Make sure the location hits the bot's memory to increase the chance of mutations hitting sysvars.
            if (b == 0)
                b = 1000; // Special case that multiples of 1000 should store to location 1000

            var a = IIf(rob[currbot].mem[b] < c, c, rob[currbot].mem[b]);

            //Botsareus 3/22/2013 handle tieang...tielen 1...4 overwrites
            byte k = 0;

            for (k = 0; k < 3; k++)
            {
                if (b == 480 + k)
                    rob[currbot].TieAngOverwrite[k] = true;

                if (b == 484 + k)
                    rob[currbot].TieLenOverwrite[k] = true;
            }
            rob[currbot].mem[b] = mod32000(a);
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]) / 5;
        }
    }

    private static void DNAinc()
    {
        var a = PopIntStack();
        if (a != 0)
        {
            a = Abs(a) % MaxMem;
            if (a == 0)
                a = 1000;

            var b = rob[currbot].mem[a] + 1;
            rob[currbot].mem[a] = mod32000(b);
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]) / 10;
        }
    }

    private static void DNAlogx()
    {
        int c;

        var b = Abs(PopIntStack());
        var a = Abs(PopIntStack());

        if (b < 2 || a == 0)
            c = 0;
        else
            c = Log(a) / Log(b);
        PushIntStack(c);
    }

    private static void DNAmod()
    {
        var b = PopIntStack();
        if (b == 0)
        {
            PopIntStack();
            PushIntStack(0);
        }
        else
            PushIntStack(PopIntStack() % b);
    }

    private static void DNAmult()
    {
        var b = PopIntStack();
        var a = PopIntStack();
        var c = CDbl(a) * CDbl(b);
        if (Abs(c) > 2000000000)
            c = Sign(c) * 2000000000;

        PushIntStack(CLng(c));
    }

    private static void DNAmultstore()
    {
        var b = PopIntStack(); // Pop the stack and get the mem location to store to
        if (b != 0)
        { // Stores to 0 are allowed, but do nothing and cost nothing
            b = Abs(b) % MaxMem; // Make sure the location hits the bot's memory to increase the chance of mutations hitting sysvars.
            if (b == 0)
                b = 1000; // Special case that multiples of 1000 should store to location 1000

            //Botsareus 11/30/2013 Small bugfix to prevent overflow

            var c = PopIntStack();
            c = mod32000(c);

            var a = rob[currbot].mem[b] * c;

            //Botsareus 3/22/2013 handle tieang...tielen 1...4 overwrites

            for (var k = 0; k < 3; k++)
            {
                if (b == 480 + k)
                    rob[currbot].TieAngOverwrite[k] = true;
                if (b == 484 + k)
                    rob[currbot].TieLenOverwrite[k] = true;
            }

            rob[currbot].mem[b] = mod32000(a);
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]) / 5;
        }
    }

    private static void DNAnegstore()
    {
        var a = PopIntStack();
        if (a != 0)
        {
            a = Abs(a) % MaxMem;
            if (a == 0)
                a = 1000;

            var b = -rob[currbot].mem[a];
            rob[currbot].mem[a] = b;
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]) / 8;
        }
    }

    private static void DNApow()
    {
        int c;

        var b = PopIntStack();
        var a = PopIntStack();

        if (Abs(b) > 10)
            b = 10 * Sgn(b);

        if (a == 0)
            c = 0;
        else
            c = Pow(a, b);
        if (Abs(c) > 2000000000)
            c = Sign(c) * 2000000000;

        PushIntStack(c);
    }

    private static void DNApyth()
    {
        var b = PopIntStack();
        var a = PopIntStack();

        var c = Sqrt(a * a + b * b);
        if (Abs(c) > 2000000000)
            c = Sign(c) * 2000000000;

        PushIntStack(c);
    }

    private static void DNArnd()
    {
        PushIntStack(Random(0, PopIntStack()));
    }

    private static void DNArndstore()
    {
        var a = PopIntStack();
        if (a != 0)
        {
            a = Abs(a) % MaxMem;
            if (a == 0)
                a = 1000;

            var b = Random(0, Abs(rob[currbot].mem[a])) * Sign(rob[currbot].mem[a]);
            rob[currbot].mem[a] = b;
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]) / 7;
        }
    }

    private static void DNAroot()
    {
        int c;

        var b = Abs(PopIntStack());
        var a = Abs(PopIntStack());

        if (b == 0)
            c = 0;
        else
            c = Pow(a, (1 / b));

        PushIntStack(c);
    }

    private static void DNAsgn()
    {
        PushIntStack(Sgn(PopIntStack()));
    }

    private static void DNAsgnstore()
    {
        var a = PopIntStack();
        if (a != 0)
        {
            a = Abs(a) % MaxMem;
            if (a == 0)
                a = 1000;

            var b = Sign(rob[currbot].mem[a]);
            rob[currbot].mem[a] = b;
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]) / 7;
        }
    }

    private static void DNAsin()
    {
        var a = PopIntStack();

        var b = Sin(a / 200) * 32000;
        PushIntStack(b);
    }

    private static void DNASqr()
    {
        double a = 0;

        a = PopIntStack();
        double b = 0;

        if (a > 0)
            b = Sqrt(a);
        else
            b = 0;

        PushIntStack(b);
    }

    private static void DNAsqrstore()
    {
        int a = 0;
        int b = 0;

        a = PopIntStack();
        if (a != 0)
        {
            a = Abs(a) % MaxMem;
            if (a == 0)
                a = 1000;

            if (rob[currbot].mem[a] > 0)
                b = Sqrt(rob[currbot].mem[a]);
            else
                b = 0;

            rob[currbot].mem[a] = b;
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]) / 7;
        }
    }

    private static void DNAstore()
    {
        var b = PopIntStack(); // Pop the stack and get the mem location to store to
        if (b != 0)
        { // Stores to 0 are allowed, but do nothing and cost nothing
            b = Abs(b) % MaxMem; // Make sure the location hits the bot's memory to increase the chance of mutations hitting sysvars.
            if (b == 0)
            {
                b = 1000; // Special case that multiples of 1000 should store to location 1000
            }
            var a = PopIntStack();

            //Botsareus 3/22/2013 handle tieang...tielen 1...4 overwrites

            for (var k = 0; k < 3; k++)
            {
                if (b == 480 + k)
                {
                    rob[currbot].TieAngOverwrite[k] = true;
                }
                if (b == 484 + k)
                {
                    rob[currbot].TieLenOverwrite[k] = true;
                }
            }

            rob[currbot].mem[b] = mod32000(a);
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]);
        }
    }

    private static void DNASub()
    { //Botsareus 5/20/2012 new code to stop overflow
        var b = PopIntStack();
        var a = PopIntStack();

        a %= 2000000000;
        b %= 2000000000;

        var c = a - b;

        if (Abs(c) > 2000000000)
        {
            c -= Sign(c) * 2000000000;
        }
        PushIntStack(c);
    }

    private static void DNAsubstore()
    {
        var b = PopIntStack(); // Pop the stack and get the mem location to store to
        if (b != 0)
        { // Stores to 0 are allowed, but do nothing and cost nothing
            b = Abs(b) % MaxMem; // Make sure the location hits the bot's memory to increase the chance of mutations hitting sysvars.
            if (b == 0)
            {
                b = 1000; // Special case that multiples of 1000 should store to location 1000
            }
            var a = rob[currbot].mem[b] - PopIntStack();

            //Botsareus 3/22/2013 handle tieang...tielen 1...4 overwrites
            for (var k = 0; k < 3; k++)
            {
                if (b == 480 + k)
                {
                    rob[currbot].TieAngOverwrite[k] = true;
                }
                if (b == 484 + k)
                {
                    rob[currbot].TieLenOverwrite[k] = true;
                }
            }

            rob[currbot].mem[b] = mod32000(a);
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[COSTSTORE] * SimOpts.Costs[COSTMULTIPLIER]) / 5;
        }
    }

    private static void equa()
    {
        PushBoolStack((PopIntStack() == PopIntStack()));
    }

    private static void ExecuteAdvancedCommand(int n, int at_position)
    {
        if (n < 13)
        {
            rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[ADCMDCOST] * SimOpts.Costs[COSTMULTIPLIER]);
        }

        switch (n)
        {
            case 1:  //findang
                findang();
                break;//finddist
            case 2:
                finddist();
                break;//ceil
            case 3:
                DNAceil();
                break;//floor
            case 4:
                DNAfloor();
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
                DNAsin();
                break;

            case 12:
                DNAcos();
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

        rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[BCCMDCOST] * SimOpts.Costs[COSTMULTIPLIER]);

        switch (n)
        {
            case 1:  //add
                DNAadd();
                break;//sub (negative add) &
            case 2:
                DNASub();
                break;//mult
            case 3:
                DNAmult();
                break;//div
            case 4:
                DNAdiv();
                break;//rnd
            case 5:
                DNArnd();
                break;//dereference AKA *
            case 6:
                DNAderef();
                break;//mod
            case 7:
                DNAmod();
                break;//sgn
            case 8:
                DNAsgn();
                break;//absolute value &
            case 9:
                DNAabs();
                break;//dup or dupint
            case 10:
                DNAdup();
                break;//dropint - Drops the top value on the Int stack
            case 11:
                PopIntStack();
                break;//clearint - Clears the Int stack
            case 12:
                ClearIntStack();
                break;//swapint - Swaps the top two values on the Int stack
            case 13:
                SwapIntStack();
                break;//overint - a b -> a b a  Dups the second value on the Int stack
            case 14:
                OverIntStack();
                break;
        }
    }

    private static void ExecuteBitwiseCommand(ref int n)
    {
        rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[BTCMDCOST] * SimOpts.Costs[COSTMULTIPLIER]);

        switch (n)
        {
            case 1:  //Compliment ~ (tilde)
                DNABitwiseCompliment();
                break;//& And
            case 2:
                DNABitwiseAND();
                break;//| or
            case 3:
                DNABitwiseOR();
                break;// XOR, ^ (I need another representation)
            case 4:
                DNABitwiseXOR();
                break;//bitinc ++
            case 5:
                DNABitwiseINC();
                break;//bitdec --
            case 6:
                DNABitwiseDEC();
                break;//negate
            case 7:
                PushIntStack(-PopIntStack());
                break;// <<
            case 8:
                DNABitwiseShiftLeft();
                break;// >>
            case 9:
                DNABitwiseShiftRight();
                break;
        }
    }

    private static void ExecuteConditions(ref int n)
    {
        rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[CONDCOST] * SimOpts.Costs[COSTMULTIPLIER]);

        switch (n)
        {
            case 1:  //<
                Min();
                break;//>
            case 2:
                magg();
                break;//=
            case 3:
                equa();
                break;// <>, !=
            case 4:
                diff();
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
                maggequal();
                break;//<=
            case 10:
                minequal();
                break;
        }
    }

    private static void ExecuteDNA(int n)
    {
        currbot = n;
        currgene = 0;
        CurrentCondFlag = NEXTBODY; //execute body statements if no cond is found
        ingene = false;

        //New bot.  clear the stacks
        ClearIntStack();
        ClearBoolStack();

        //EricL - March 15, 2006 This section initializes the robot's ga() array to all False so that it can
        //be populated below for those genes that activate this cycle.  Used for displaying
        //Gene Activations.  Only initialized and populated for the robot with the focus or if the bot's console
        //is open.
        if ((n == robfocus) || !(Robots.rob[n].console == null))
        {
            Robots.rob[n].ga = new bool[Robots.rob[n].genenum];
            for (var i = 0; i < Robots.rob[n].genenum; i++)
            {
                Robots.rob[n].ga[i] = false;
            }
        }

        var rob = Robots.rob[n];
        var a = 1;
        Robots.rob[n].condnum = 0; // EricL 4/6/2006 reset the COND statement counter to 0
        Robots.rob[n].dbgstring = ""; //Botsareus 4/5/2016 Safer way to debug DNA
        while (!(rob.dna[a].tipo == 10 & rob.dna[a].value == 1) && a <= 32000 & a < UBound(rob.dna))
        { //Botsareus 6/16/2012 Added upper bounds check (This seems like overkill but I had situations where 'end' command did not exisit)
            var tipo = rob.dna[a].tipo;
            switch (tipo)
            {
                case 0:  //number[
                    if (CurrentFlow != CLEAR)
                    {
                        PushIntStack(rob.dna[a].value);
                        rob.nrg -= (SimOpts.Costs[NUMCOST] * SimOpts.Costs[COSTMULTIPLIER]);
                    }
                    break;//*number
                case 1:
                    if (CurrentFlow != CLEAR)
                    {
                        var b = rob.dna[a].value;
                        if (b > MaxMem || b < 1)
                        {
                            b = Abs(rob.dna[a].value) % MaxMem;
                            if (b == 0)
                            {
                                b = 1000; // Special case that multiples of 1000 should store to location 1000
                            }
                        }

                        PushIntStack(rob.mem[b]);
                        rob.nrg -= (SimOpts.Costs(DOTNUMCOST) * SimOpts.Costs[COSTMULTIPLIER]);
                    }
                    break;//commands (add, sub, etc.)
                case 2:
                    if (CurrentFlow != CLEAR)
                    {
                        ExecuteBasicCommand(rob.dna[a].value);
                    }
                    break;//advanced commands
                case 3:
                    if (CurrentFlow != CLEAR)
                    {
                        ExecuteAdvancedCommand(rob.dna[a].value, a);
                    }
                    break;//bitwise commands
                case 4:
                    if (CurrentFlow != CLEAR)
                    {
                        ExecuteBitwiseCommand(rob.dna[a].value);
                    }
                    break;//conditions
                case 5:
                    //EricL  11/2007 New execution paradym.  Conditions can now be executeed anywhere in the gene
                    if (CurrentFlow == COND || CurrentFlow == body || CurrentFlow == ELSEBODY)
                    {
                        ExecuteConditions(rob.dna[a].value);
                    }
                    break;//logic commands (and, or, etc.)
                case 6:
                    //EricL  11/2007 New execution paradym.  Conditions can now be executeed anywhere in the gene
                    if (CurrentFlow == COND || CurrentFlow == body || CurrentFlow == ELSEBODY)
                    {
                        ExecuteLogic(rob.dna[a].value);
                    }
                    break;//store, inc and dec
                case 7:
                    if (CurrentFlow == body || CurrentFlow == ELSEBODY)
                    {
                        if (CondStateIsTrue())
                        { // Check the Bool stack.  If empty or True on top, do the stores.  Don't if False.
                            ExecuteStores(rob.dna[a].value);
                            if (n == robfocus || !(Robots.rob[n].console == null))
                            {
                                Robots.rob[n].ga(currgene) = true; //EricL  This gene fired this cycle!  Populate ga()
                            }
                        }
                    }
                    break;//reserved for a future type
                case 8:
                    break;//flow commands
                case 9:
                    // EricL 4/6/2006 Added If statement.  This counts the number of COND statements in each bot.
                    if (!ExecuteFlowCommands(rob.dna[a].value, n))
                    {
                        Robots.rob[n].condnum = Robots.rob[n].condnum + 1;
                    }

                    //If .VirusArray(currgene) > 1 Then 'next gene is busy, so clear flag
                    //  CurrentFlow = CLEAR
                    //End If

                    rob.mem[thisgene] = currgene;
                    break;//Master flow, such as end, chromostart, etc.
                case 10:
                    //ExecuteMasterFlow .dna[a].value
                    break;
            }

            a++;
        }
        CurrentFlow = CLEAR; // EricL 4/15/2006 Do this so next bot doesn't inherit the flow control
    }

    private static bool ExecuteFlowCommands(int n, int bot)
    {
        bool ExecuteFlowCommands = false;

        rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[FLOWCOST] * SimOpts.Costs[COSTMULTIPLIER]);

        //returns true if a stop command was found (start, stop, or else)
        //returns false if cond was found
        ExecuteFlowCommands = false;
        switch (n)
        {
            case 1:  //cond
                CurrentFlow = COND;
                currgene++;
                ClearBoolStack();
                ingene = true;
                break;//assume a stop command, or it really is a stop command
            case 2:
                //this is supposed to come before case 2 and 3, since these commands
                //must be executed before start and else have a chance to go
                ExecuteFlowCommands = true;
                if (CurrentFlow == COND)
                {
                    CurrentCondFlag = AddupCond();
                }
                if (!ingene)
                {
                    CurrentCondFlag = NEXTBODY;
                }
                if (CurrentCondFlag && (CurrentFlow == ELSEBODY || CurrentFlow == body))
                { //Botsareus 3/24/2012 Fixed a bug where: any else gene was showing activation
                  // Need to check this for the case where the gene body doesn't have any stores to trigger the activation dialog
                    if (bot == robfocus || !(rob[bot].console == null))
                    {
                        rob[bot].ga[currgene] = true; //EricL  This gene fired this cycle!  Populate ga()
                    }
                }
                CurrentFlow = CLEAR;
                switch (n)
                {
                    case 2:  //start
                        if (!ingene)
                        { // the first start or else after a cond is not a new gene but the rest are
                            currgene++;
                        }
                        ingene = false;
                        if (CurrentCondFlag == NEXTBODY)
                        {
                            CurrentFlow = body;
                        }
                        break;//else
                    case 3:
                        if (CurrentCondFlag == NEXTELSE)
                        {
                            CurrentFlow = ELSEBODY;
                        }
                        if (!ingene)
                        {
                            currgene++;
                        }
                        ingene = false;
                        break;// stop
                    case 4:
                        ingene = false;
                        CurrentFlow = CLEAR;
                        break;
                }
                break;
        }
        return ExecuteFlowCommands;
    }

    private static void ExecuteLogic(ref int n)
    {
        rob[currbot].nrg = rob[currbot].nrg - (SimOpts.Costs[LOGICCOST] * SimOpts.Costs[COSTMULTIPLIER]);

        int a = 0;
        int b = 0;

        switch (n)
        {
            case 1:  //and
                b = PopBoolStack();
                if (b == -5)
                {
                    b = true;
                }
                a = PopBoolStack();
                if (a != -5)
                {
                    PushBoolStack(a && b);
                }
                else
                {
                    PushBoolStack(b);
                }
                break;//or
            case 2:
                b = PopBoolStack();
                if (b == -5)
                {
                    b = true;
                }
                a = PopBoolStack();
                if (a != -5)
                {
                    PushBoolStack(a || b);
                }
                else
                {
                    PushBoolStack(true);
                }
                break;//xor
            case 3:
                b = PopBoolStack();
                if (b == -5)
                {
                    b = true;
                }
                a = PopBoolStack();
                if (a != -5)
                {
                    PushBoolStack(a ^ b);
                }
                else
                {
                    PushBoolStack(!b);
                }
                break;//not
            case 4:
                b = PopBoolStack();
                if (b == -5)
                {
                    b = true;
                }
                PushBoolStack(!b);
                break;// true
            case 5:
                PushBoolStack(true);
                break;// false
            case 6:
                PushBoolStack(false);
                break;// dropbool
            case 7:
                b = PopBoolStack();
                break;// clearbool
            case 8:
                ClearBoolStack();
                break;// dupbool
            case 9:
                DupBoolStack();
                break;// swapbool
            case 10:
                SwapBoolStack();
                break;// overbool
            case 11:
                OverBoolStack();
                break;
        }
    }

    private static void ExecuteStores(ref int n)
    {
        switch (n)
        {
            case 1:  //store
                DNAstore();
                break;//inc
            case 2:
                DNAinc();
                break;//dec
            case 3:
                DNAdec();
                break;//+= 'Botsareus 9/7/2013 New commannds
            case 4:
                DNAaddstore();
                break;//-=
            case 5:
                DNAsubstore();
                break;//*=
            case 6:
                DNAmultstore();
                break;///=
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
        var b = PopIntStack(); // * Form1.yDivisor
        var a = PopIntStack(); // * Form1.xDivisor
        var c = rob[currbot].pos.x / Form1.instance.xDivisor;
        var d = rob[currbot].pos.y / Form1.instance.yDivisor;
        var e = angnorm(angle(c, d, a, b)) * 200;
        PushIntStack(e);
    }

    private static void finddist()
    {
        var b = PopIntStack() * Form1.yDivisor;
        var a = PopIntStack() * Form1.xDivisor;
        var c = rob[currbot].pos.x;
        var d = rob[currbot].pos.y;
        var e = Sqrt((Pow((c - a), 2) + Pow((d - b), 2)));
        if (Abs(e) > 2000000000)
        {
            e = Sign(e) * 2000000000;
        }
        PushIntStack(CLng(e));
    }

    private static void magg()
    {
        PushBoolStack((PopIntStack() < PopIntStack()));
    }

    private static void maggequal()
    {
        PushBoolStack((PopIntStack() <= PopIntStack()));
    }

    private static void Min()
    {
        PushBoolStack((PopIntStack() > PopIntStack()));
    }

    private static void minequal()
    {
        PushBoolStack((PopIntStack() >= PopIntStack()));
    }

    private static int mod32000(int a)
    {
        //Botsareus 10/6/2015 Fix for out of range

        if (a > 0)
        {
            a = a % 32000;
            if (a == 0)
            {
                a = 32000; // Special case 32000
            }
        }
        else if (a < 0)
        {
            a = a % 32000;
            if (a == 0)
            {
                a = -32000; // special case -32000
            }
        }

        return a;
    }

    public class block
    {
        public int tipo = 0;
        public int value = 0;
    }

    // Option Explicit
    //boolstack structure used for conditionals
    public class boolstack
    {
        public int pos = 0;
        public bool[] val = new bool[100];
    }

    public class Queue
    {
        public int memloc = 0;
        public int Memval = 0;
    }

    // array of system variables functional
    // stack structure, used for robots' stack
    public class Stack
    {
        public int pos = 0;
        public int[] val = new int[stacklim];
    }
}
