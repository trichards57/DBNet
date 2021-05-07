using DBNet.Forms;
using static Buckets_Module;
using static Common;
using static DNAExecution;
using static DNATokenizing;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Strings;
using static Robots;
using static System.Math;
using static VBExtension;

internal static class DNAManipulations
{
    //All functions that manipulate DNA without actually mutating it should go here.
    //That is, anything that searches DNA, etc.
    // loads a dna file, inserting the robot in the simulation

    public static int CountGenes(block[] dna)
    {
        var CountGenes = 0;
        var counter = 1;
        var ingene = false;

        while (counter <= 32000 & counter <= UBound(dna))
        { //Botsareus 5/29/2012 Added upper bounds check
            if (dna[counter].tipo == 10 & dna[counter].value == 1)
                return CountGenes;

            // If a Start or Else
            if (dna[counter].tipo == 9 && (dna[counter].value == 2 || dna[counter].value == 3))
            {
                if (!ingene)
                    CountGenes++;

                ingene = false; // that follows a cond
            }
            // If a Cond
            if (dna[counter].tipo == 9 && (dna[counter].value == 1))
            {
                ingene = true;
                CountGenes++;
            }
            // If a stop
            if (dna[counter].tipo == 9 && dna[counter].value == 4)
                ingene = false;

            counter++;
        }
        return CountGenes;
    }

    public static int DnaLen(block[] dna)
    {
        var DnaLen = 1;
        while (!(dna[DnaLen].tipo == 10 & dna[DnaLen].value == 1) && DnaLen <= 32000 & DnaLen < UBound(dna))
            DnaLen++;

        return DnaLen;
    }

    public static void DupBoolStack()
    {
        if (Condst.Count == 0)
            return;

        Condst.Push(Condst.Peek());
    }

    public static void DupIntStack()
    {
        if (IntStack.Count == 0)
            return;

        IntStack.Push(IntStack.Peek());
    }

    public static void exechighlight(int n)
    {
        ActivForm.instance.DrawGrid(rob[n].ga); // EricL March 15, 2006 - This line uncommented
    }

    public static int GeneEnd(block[] dna, int position)
    {
        var condgene = false;

        var GeneEnd = position;
        if (dna[GeneEnd].tipo == 9 && dna[GeneEnd].value == 1)
            condgene = true;

        while (GeneEnd + 1 <= 32000)
        {
            if (dna[GeneEnd + 1].tipo == 10)
                break; // end of genome

            if (dna[GeneEnd + 1].tipo == 9 && ((dna[GeneEnd + 1].value == 1) || dna[GeneEnd + 1].value == 4))
            { // cond or stop
                if (dna[GeneEnd + 1].value == 4)
                    GeneEnd++; // Include the stop as part of the gene

                break;
            }

            if (dna[GeneEnd + 1].tipo == 9 && ((dna[GeneEnd + 1].value == 2) || dna[GeneEnd + 1].value == 3) && !condgene)
                break; // start or else

            if (dna[GeneEnd + 1].tipo == 9 && ((dna[GeneEnd + 1].value == 2) || dna[GeneEnd + 1].value == 3) && condgene)
                condgene = false; // start or else

            GeneEnd++;

            if ((GeneEnd + 1) > UBound(dna))
                break; //Botsareus 5/29/2012 Added upper bounds check
        }
        return GeneEnd;
    }

    public static int genepos(block[] dna, int n)
    {
        var k = 1;
        var genenum = 0;
        var ingene = false;
        var genepos = 0;

        if (n == 0)
            return 0;

        while (k > 0 & genepos == 0 & k <= 32000)
        {
            //A start or else
            if (dna[k].tipo == 9 && (dna[k].value == 2 || dna[k].value == 3))
            {
                if (!ingene)
                { // Does not follow a cond.  Make it a new gene
                    genenum++;
                    if (genenum == n)
                    {
                        genepos = k;
                        break;
                    }
                }
                else
                    ingene = false; // First Start or Else following a cond
            }

            // If a Cond
            if (dna[k].tipo == 9 && (dna[k].value == 1))
            {
                ingene = true;
                genenum++;
                if (genenum == n)
                {
                    genepos = k;
                    break;
                }
            }
            // If a stop
            if (dna[k].tipo == 9 && dna[k].value == 4)
                ingene = false;

            k++;
            if (dna[k].tipo == 10 & dna[k].value == 1)
                k = -1;
        }

        return genepos;
    }

    public static void insertsysvars(ref int n)
    {
        var t = 1;
        while (sysvar[t].Name != "")
        {
            rob[n].usedvars[t] = sysvar[t].value;
            t++;
        }
        rob[n].maxusedvars = t - 1;
    }

    public static void insertvar(int n, string a)
    {
        a = Right(a, Len(a) - 4);
        var pos = InStr(a, " ");
        var b = Left(a, pos - 1);
        var c = Right(a, Len(a) - pos);
        rob[n].vars[rob[n].vnum].Name = b;
        rob[n].vars[rob[n].vnum].value = Val(c);
        rob[n].vnum = rob[n].vnum + 1;
    }

    public static void interpretUSE(int n, string a)
    {
        a = Right(a, Len(a) - 4);

        if (a == "NewMove")
            rob[n].NewMove = true;
    }

    public static bool IsRobDNABounded(block[] ArrayIn)
    {
        return UBound(ArrayIn) >= LBound(ArrayIn);
    }

    public static int NextStop(block[] dna, int inizio)
    {
        var NextStop = inizio;
        while (!((dna[NextStop].tipo == 9 && (dna[NextStop].value == 4)) || dna[NextStop].tipo == 10) && (NextStop <= 32000))
            NextStop++;

        return NextStop;
    }

    public static void OverBoolStack()
    {
        //a b -> a b a

        if (Condst.Count == 0)
            return;//Do nothing.  Nothing on stack.

        if (Condst.Count == 1)
        {
            //Only 1 thing on stack.
            Condst.Push(true);
            return;
        }

        var b = Condst.Pop();
        var a = Condst.Pop();
        Condst.Push(a);
        Condst.Push(b);
        Condst.Push(a);
    }

    public static void OverIntStack()
    {
        //a b -> a b a

        if (IntStack.Count == 0)
            return;

        if (IntStack.Count == 1)
        {
            // 1 value on the stack
            IntStack.Push(0);
            return;
        }

        var b = IntStack.Pop();
        var a = IntStack.Pop();
        IntStack.Push(a);
        IntStack.Push(b);
        IntStack.Push(a);
    }

    public static int PopBoolStack()
    {
        if (Condst.Count == 0)
            return -5;

        return Condst.Pop() ? 1 : 0;
    }

    public static int PopIntStack()
    {
        if (IntStack.Count == 0)
            return 0;

        return IntStack.Pop();
    }

    public static int PrevStop(block[] dna, int inizio)
    {
        var PrevStop = inizio;
        while (!((dna[PrevStop].tipo == 9 && dna[PrevStop].value != 4) || dna[PrevStop].tipo == 10))
        {
            PrevStop--;
            if (PrevStop < 1)
                break;
        }
        return PrevStop;
    }

    public static void PushBoolStack(bool value)
    {
        Condst.Push(value);
    }

    public static void PushIntStack(int value)
    {
        IntStack.Push(value);
    }

    public static int RobScriptLoad(string path)
    {
        var n = posto();
        preparerob(n, path); // prepares structure
        if (LoadDNA(path, n))
        { // loads and parses dna
            insertsysvars(n); // count system vars among used vars
            ScanUsedVars(n); // count other used locations
            makeoccurrlist(n); // creates the ref* array
            rob[n].DnaLen = DnaLen(rob[n].dna()); // measures dna length
            rob[n].genenum = CountGenes(rob[n].dna());
            rob[n].mem(DnaLenSys) = rob[n].DnaLen;
            rob[n].mem(GenesSys) = rob[n].genenum;
            return n; // returns the index of the created rob
        }

        rob[n].exist = false;
        UpdateBotBucket(n);
        return -1;
    }

    /*
    ' prepares with some values the struct of a new rob
    */

    public static void ScanUsedVars(ref int n)
    {
        var t = 0;
        var used = false;

        while (!(rob[n].dna[t].tipo == 10 & rob[n].dna[t].value == 1))
        {
            t++;
            if (UBound(rob[n].dna) < t)
                break;

            if (rob[n].dna[t].tipo == 1)
            {
                var a = rob[n].dna[t].value;
                for (var k = 1; k < rob[n].maxusedvars; k++)
                {
                    if (rob[n].usedvars[k] == a)
                        used = true;
                }
                if (!used)
                {
                    rob[n].maxusedvars = rob[n].maxusedvars + 1;
                    if (UBound(rob[n].usedvars) >= rob[n].maxusedvars)
                        rob[n].usedvars[rob[n].maxusedvars] = a;
                }
                used = false;
            }
        }
    }

    public static void SwapBoolStack()
    {
        if (Condst.Count <= 1)
            return;//Do nothing

        var a = Condst.Pop();
        var b = Condst.Pop();
        Condst.Push(a);
        Condst.Push(b);
    }

    public static void SwapIntStack()
    {
        if (IntStack.Count <= 1)
            return;//Do nothing

        var a = IntStack.Pop();
        var b = IntStack.Pop();
        IntStack.Push(a);
        IntStack.Push(b);
    }

    private static void preparerob(int t, string path)
    {
        rob[t].pos.x = Random(50, (int)Form1.instance.ScaleWidth());
        rob[t].pos.y = Random(50, (int)Form1.instance.ScaleHeight());
        rob[t].aim = Random(0, 628) / 100;
        rob[t].aimvector = VectorSet(Cos(rob[t].aim), Sin(rob[t].aim));
        rob[t].exist = true;
        rob[t].BucketPos.x = -2;
        rob[t].BucketPos.y = -2;
        UpdateBotBucket(t);

        var col1 = Random(50, 255);
        var col2 = Random(50, 255);
        var col3 = Random(50, 255);
        rob[t].color = col1 * 65536 + col2 * 256 + col3;

        rob[t].vnum = 1;
        //rob[t].st.pos = 1
        rob[t].nrg = 20000;
        rob[t].Veg = false;
        var k = 1;
        while (InStr(k, path, "\\") > 0)
            k++;

        rob[t].FName = Right(path, Len(path) - k + 1);
    }
}
