using DarwinBots.Model;
using DarwinBots.Support;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DarwinBots.Modules
{
    internal static class DnaManipulations
    {
        public static int CountGenes(IEnumerable<DnaBlock> dna)
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

        public static int DnaLen(IEnumerable<DnaBlock> dna)
        {
            return dna.TakeWhile(d => !(d.Type == 10 & d.Value == 1)).Count();
        }

        public static int GeneEnd(IList<DnaBlock> dna, int position)
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

        public static int GenePosition(IList<DnaBlock> dna, int n)
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

        public static void InsertVar(Robot rob, string a)
        {
            a = a[4..];
            var parts = a.Split(' ', 2);
            var cValid = int.TryParse(parts[1], out var cVal);

            rob.Variables.Add(new Variable(parts[0], cValid ? cVal : 0));
        }

        public static async Task<Robot> RobScriptLoad(IRobotManager robotManager, IBucketManager bucketManager, string path)
        {
            var rob = robotManager.GetNewBot();
            PrepareRob(bucketManager, rob, path); // prepares structure
            if (await DnaTokenizing.LoadDna(path, rob))
            {
                // loads and parses dna
                Senses.MakeOccurrList(rob); // creates the ref* array
                rob.NumberOfGenes = CountGenes(rob.Dna);
                rob.Memory[MemoryAddresses.DnaLenSys] = rob.Dna.Count;
                rob.Memory[MemoryAddresses.GenesSys] = rob.NumberOfGenes;
                return rob; // returns the index of the created rob
            }

            rob.Exists = false;
            robotManager.Robots.Remove(rob);
            bucketManager.UpdateBotBucket(rob);
            return null;
        }

        private static void PrepareRob(IBucketManager bucketManager, Robot rob, string path)
        {
            //rob.pos.X = ThreadSafeRandom.Local.Next(50, (int)Form1.instance.ScaleWidth());
            //rob.pos.Y = ThreadSafeRandom.Local.Next(50, (int)Form1.instance.ScaleHeight());
            rob.Aim = (double)ThreadSafeRandom.Local.Next(0, 628) / 100;
            rob.Exists = true;
            rob.BucketPosition = new IntVector(-2, -2);
            bucketManager.UpdateBotBucket(rob);

            rob.Color = Color.FromRgb((byte)ThreadSafeRandom.Local.Next(50, 255), (byte)ThreadSafeRandom.Local.Next(50, 255), (byte)ThreadSafeRandom.Local.Next(50, 255));
            rob.Variables.Clear();
            rob.Energy = 20000;
            rob.IsVegetable = false;
            rob.FName = System.IO.Path.GetFileName(path);
        }
    }
}
