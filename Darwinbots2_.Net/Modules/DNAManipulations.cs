using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using static DarwinBots.Modules.DNATokenizing;
using static DarwinBots.Modules.Robots;

namespace DarwinBots.Modules
{
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
            var condgene = dna[position].tipo == 9 && dna[position].value == 1;

            for (var i = position + 1; i < dna.Count; i++)
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
            return dna.Count - 1;
        }

        public static int genepos(IList<DNABlock> dna, int n)
        {
            var k = 1;
            var genenum = 0;
            var ingene = false;
            var genepos = 0;

            if (n == 0)
                return 0;

            while (k is > 0 and <= 32000)
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

            rob.vars.Add(new Variable(parts[0], cValid ? cVal : 0));
        }

        public static async Task<robot> RobScriptLoad(string path)
        {
            var rob = GetNewBot();
            PrepareRob(rob, path); // prepares structure
            if (await LoadDNA(path, rob))
            {
                // loads and parses dna
                Senses.MakeOccurrList(rob); // creates the ref* array
                rob.genenum = CountGenes(rob.dna);
                rob.mem[DnaLenSys] = rob.dna.Count;
                rob.mem[GenesSys] = rob.genenum;
                return rob; // returns the index of the created rob
            }

            rob.exist = false;
            Robots.rob.Remove(rob);
            Globals.BucketManager.UpdateBotBucket(rob);
            return null;
        }

        private static void PrepareRob(robot rob, string path)
        {
            //rob.pos.X = ThreadSafeRandom.Local.Next(50, (int)Form1.instance.ScaleWidth());
            //rob.pos.Y = ThreadSafeRandom.Local.Next(50, (int)Form1.instance.ScaleHeight());
            rob.aim = (double)ThreadSafeRandom.Local.Next(0, 628) / 100;
            rob.aimvector = new DoubleVector(Math.Cos(rob.aim), Math.Sin(rob.aim));
            rob.exist = true;
            rob.BucketPos = new IntVector(-2, -2);
            Globals.BucketManager.UpdateBotBucket(rob);

            rob.color = Color.FromRgb((byte)ThreadSafeRandom.Local.Next(50, 255), (byte)ThreadSafeRandom.Local.Next(50, 255), (byte)ThreadSafeRandom.Local.Next(50, 255));
            rob.vars.Clear();
            rob.nrg = 20000;
            rob.Veg = false;
            rob.FName = System.IO.Path.GetFileName(path);
        }
    }
}
