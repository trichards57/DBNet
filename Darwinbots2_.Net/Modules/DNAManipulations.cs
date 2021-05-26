using DBNet.Forms;
using Iersera.Model;
using Iersera.Support;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BucketManager;
using static DNATokenizing;
using static Microsoft.VisualBasic.Strings;
using static Robots;
using static System.Math;

internal static class DNAManipulations
{
    //All functions that manipulate DNA without actually mutating it should go here.
    //That is, anything that searches DNA, etc.
    // loads a dna file, inserting the robot in the simulation

    public static int CountGenes(IList<DNABlock> dna)
    {
        var genesCount = 0;
        var ingene = false;

        foreach (var block in dna)
        {
            if (block.tipo == 10 & block.value == 1)
                return genesCount;

            if (block.tipo == 9)
            {
                switch (block.value)
                {
                    case 1:
                        ingene = true;
                        genesCount++;
                        break;

                    case 2:
                    case 3:
                        if (!ingene)
                            genesCount++;

                        ingene = false; // that follows a cond
                        break;

                    case 4:
                        ingene = false;
                        break;
                }
            }
        }

        return genesCount;
    }

    public static int DnaLen(IList<DNABlock> dna)
    {
        return dna.TakeWhile(d => !(d.tipo == 10 & d.value == 1)).Count();
    }

    public static int GeneEnd(IList<DNABlock> dna, int position)
    {
        var condgene = false;
        if (dna[position].tipo == 9 && dna[position].value == 1)
            condgene = true;

        for (var i = position + 1; i < dna.Length; i++)
        {
            if (dna[i].tipo == 10)
                break; // end of genome

            if (dna[i].tipo == 9)
            {
                switch (dna[i].value)
                {
                    case 1:
                        return i - 1;

                    case 2:
                    case 3:
                        if (!condgene)
                            return i - 1;
                        condgene = false;
                        break;

                    case 4:
                        return i;
                }
            }
        }
        return dna.Length - 1;
    }

    public static int genepos(IList<DNABlock> dna, int n)
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

    public static void InsertVar(robot rob, string a)
    {
        a = a[4..];
        var parts = a.Split(' ', 2);
        var cValid = int.TryParse(parts[1], out var cVal);

        rob.vars.Add(new Globals.Variable(parts[0], cValid ? cVal : 0));
    }

    public static async Task<int> RobScriptLoad(string path)
    {
        var n = posto();
        PrepareRob(n, path); // prepares structure
        if (await LoadDNA(path, rob[n]))
        {
            // loads and parses dna
            Senses.makeoccurrlist(n); // creates the ref* array
            rob[n].DnaLen = DnaLen(rob[n].dna); // measures dna length
            rob[n].genenum = CountGenes(rob[n].dna);
            rob[n].mem[DnaLenSys] = rob[n].DnaLen;
            rob[n].mem[GenesSys] = rob[n].genenum;
            return n; // returns the index of the created rob
        }

        rob[n].exist = false;
        UpdateBotBucket(n);
        return -1;
    }

    private static void PrepareRob(int t, string path)
    {
        rob[t].pos.X = ThreadSafeRandom.Local.Next(50, (int)Form1.instance.ScaleWidth());
        rob[t].pos.Y = ThreadSafeRandom.Local.Next(50, (int)Form1.instance.ScaleHeight());
        rob[t].aim = ThreadSafeRandom.Local.Next(0, 628) / 100;
        rob[t].aimvector = new vector(Cos(rob[t].aim), Sin(rob[t].aim));
        rob[t].exist = true;
        rob[t].BucketPos.X = -2;
        rob[t].BucketPos.Y = -2;
        UpdateBotBucket(t);

        var col1 = ThreadSafeRandom.Local.Next(50, 255);
        var col2 = ThreadSafeRandom.Local.Next(50, 255);
        var col3 = ThreadSafeRandom.Local.Next(50, 255);
        rob[t].color = col1 * 65536 + col2 * 256 + col3;
        rob[t].vars.Clear();
        rob[t].nrg = 20000;
        rob[t].Veg = false;
        var k = 1;
        while (InStr(k, path, "\\") > 0)
            k++;

        rob[t].FName = Right(path, Len(path) - k + 1);
    }
}
