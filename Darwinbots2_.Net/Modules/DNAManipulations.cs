using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DarwinBots.Modules
{
    internal static class DnaManipulations
    {
        public static int CountGenes(IEnumerable<DNABlock> dna)
        {
            var genesCount = 0;
            var inGene = false;

            foreach (var block in dna)
            {
                if (block.Type == 10 & block.Value == 1)
                    return genesCount;

                if (block.Type == 9)
                {
                    switch (block.Value)
                    {
                        case 1:
                            inGene = true;
                            genesCount++;
                            break;

                        case 2:
                        case 3:
                            if (!inGene)
                                genesCount++;

                            inGene = false; // that follows a cond
                            break;

                        case 4:
                            inGene = false;
                            break;
                    }
                }
            }

            return genesCount;
        }

        public static int DnaLen(IEnumerable<DNABlock> dna)
        {
            return dna.TakeWhile(d => !(d.Type == 10 & d.Value == 1)).Count();
        }

        public static int GeneEnd(IList<DNABlock> dna, int position)
        {
            var condGene = dna[position].Type == 9 && dna[position].Value == 1;

            for (var i = position + 1; i < dna.Count; i++)
            {
                if (dna[i].Type == 10)
                    break; // end of genome

                if (dna[i].Type == 9)
                {
                    switch (dna[i].Value)
                    {
                        case 1:
                            return i - 1;

                        case 2:
                        case 3:
                            if (!condGene)
                                return i - 1;
                            condGene = false;
                            break;

                        case 4:
                            return i;
                    }
                }
            }
            return dna.Count - 1;
        }

        public static int GenePosition(IList<DNABlock> dna, int n)
        {
            var k = 1;
            var geneNum = 0;
            var inGene = false;
            var genePos = 0;

            if (n == 0)
                return 0;

            while (k is > 0 and <= 32000)
            {
                //A start or else
                if (dna[k].Type == 9 && (dna[k].Value == 2 || dna[k].Value == 3))
                {
                    if (!inGene)
                    { // Does not follow a cond.  Make it a new gene
                        geneNum++;
                        if (geneNum == n)
                        {
                            genePos = k;
                            break;
                        }
                    }
                    else
                        inGene = false; // First Start or Else following a cond
                }

                // If a Cond
                if (dna[k].Type == 9 && (dna[k].Value == 1))
                {
                    inGene = true;
                    geneNum++;
                    if (geneNum == n)
                    {
                        genePos = k;
                        break;
                    }
                }
                // If a stop
                if (dna[k].Type == 9 && dna[k].Value == 4)
                    inGene = false;

                k++;
                if (dna[k].Type == 10 & dna[k].Value == 1)
                    k = -1;
            }

            return genePos;
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
            var rob = Robots.GetNewBot();
            PrepareRob(rob, path); // prepares structure
            if (await DnaTokenizing.LoadDna(path, rob))
            {
                // loads and parses dna
                Senses.MakeOccurrList(rob); // creates the ref* array
                rob.genenum = CountGenes(rob.dna);
                rob.mem[Robots.DnaLenSys] = rob.dna.Count;
                rob.mem[Robots.GenesSys] = rob.genenum;
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
