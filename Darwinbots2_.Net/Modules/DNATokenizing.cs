using DarwinBots.Model;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static DarwinBots.Modules.DnaEngine;
using static DarwinBots.Modules.DNAManipulations;

namespace DarwinBots.Modules
{
    internal static class DNATokenizing
    {
        private static readonly int[,] dnamatrix = new int[9, 14];

        //make sure that when we are saving to file do not normalize custome sysvars

        public static void CalculateDnaMatrix()
        {
            //calculate dna matrix

            var count = 0;

            for (var y_tipo = 0; y_tipo < 9; y_tipo++)
            {
                for (var y_value = 0; y_value < 14; y_value++)
                {
                    var Y = new DNABlock
                    {
                        tipo = y_tipo + 2,
                        value = y_value + 1
                    };
                    var result = Parse(Y);
                    if (result == "") continue;
                    dnamatrix[y_tipo, y_value] = count;
                    count++;
                }
            }
        }

        public static string DetokenizeDNA(robot rob, int Position = 0)
        {
            var result = new StringBuilder();
            var geneEnd = false;
            var dna = rob.dna;
            var inGene = false;
            var coding = false;
            var t = 1;
            var gene = 0;
            var lastgene = 0;

            while (!(dna[t].tipo == 10 & dna[t].value == 1))
            {
                // If a Start or Else
                if (dna[t].tipo == 9 && (dna[t].value == 2 || dna[t].value == 3))
                {
                    if (coding && !inGene)
                        result.AppendLine($"''''''''''''''''''''''''  Gene: {gene} Ends at position {t - 1}  '''''''''''''''''''''''");

                    if (!inGene)
                        gene++;
                    else
                        inGene = false;

                    coding = true;
                }
                // If a Cond
                if (dna[t].tipo == 9 && (dna[t].value == 1))
                {
                    if (coding)
                    {
                        result.AppendLine($"''''''''''''''''''''''''  Gene: {gene} Ends at position {t - 1}  '''''''''''''''''''''''");
                        result.AppendLine();
                    }

                    inGene = true;
                    gene++;
                    coding = true;
                }
                // If a stop
                if (dna[t].tipo == 9 && dna[t].value == 4)
                {
                    if (coding)
                        geneEnd = true;

                    inGene = false;
                    coding = false;
                }

                if (gene != lastgene)
                {
                    if (gene > 1)
                    {
                        result.AppendLine($"''''''''''''''''''''''''  Gene: {gene} Begins at position {t}  '''''''''''''''''''''''");
                    }

                    result.AppendLine();

                    lastgene = gene;
                }

                var converttosysvar = dna[t + 1].tipo == 7;
                var temp = Parse(dna[t], rob, converttosysvar);
                if (temp == "")
                    temp = "VOID"; //alert user that there is an invalid DNA entry.

                result.Append(temp);
                //formatting
                if (dna[t].tipo == 5 || dna[t].tipo == 6 || dna[t].tipo == 7 || dna[t].tipo == 9)
                    result.AppendLine();

                if (geneEnd)
                { // Indicate gene ended via a stop.  Needs to come after base pair
                    result.AppendLine($"''''''''''''''''''''''''  Gene: {gene} Ends at position {t}  '''''''''''''''''''''''");
                    geneEnd = false;
                }

                if (Position > 0 & t == Position)
                    result.AppendLine(" '[<POSITION MARKER]"); //Botsareus 2/25/2013 Makes the program easy to debug

                t++;
            }
            if (!(dna[t - 1].tipo == 9 && dna[t - 1].value == 4) && coding)
            { // End of DNA without a stop.
                result.AppendLine($"''''''''''''''''''''''''  Gene: {gene} Ends at position {t - 1}  '''''''''''''''''''''''");
            }

            return result.ToString();
        }

        public static int DNAtoInt(int tipo, int value)
        {
            value = Math.Clamp(value, -32000, 32000);

            //figure out conversion
            if (tipo >= 2)
                return 32691 + dnamatrix[tipo - 2, value - 1]; //dnamatrix adds max of 76 because we have 76 commands

            var result = -16646;

            if (Math.Abs(value) > 999)
                value = (int)(512 * Math.Sign(value) + value / 2.05);

            result += value;

            if (tipo == 1)
                result += 32729;

            return result;
        }

        public static string Hash(string s)
        {
            return Convert.ToHexString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(s)));
        }

        public static async Task<bool> LoadDNA(string path, robot rob)
        {
            var hold = new StringBuilder();

            rob.dna.Clear();

            if (path == "")
                return false;

            var lines = await File.ReadAllLinesAsync(path);
            foreach (var a in lines)
            {
                var processedLine = a;

                // eliminate comments at the end of a line
                // but preserves comments-only lines
                if (a.IndexOf('\'') > 0)
                    processedLine = processedLine.Split("'")[0];

                processedLine = processedLine.Replace('\t', ' ').Trim();

                if (processedLine.StartsWith("'") || processedLine.StartsWith("/") || string.IsNullOrEmpty(processedLine))
                {
                    if (processedLine.StartsWith("'#") || processedLine.StartsWith("/#"))
                        GetVals(rob, a, hold.ToString());
                }
                else
                {
                    if (processedLine.StartsWith("def"))
                        InsertVar(rob, a);
                    else
                    {
                        var parts = processedLine.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        foreach (var b in parts)
                            rob.dna.Add(Parse(b, rob));
                    }
                }
                hold.AppendLine(a);
            }

            rob.dna.Add(new DNABlock { tipo = 10, value = 1 });

            return true;
        }

        public static DNABlock Parse(string Command, robot rob = null)
        {
            Command = Command.ToLowerInvariant();

            var bp = BasicCommandTok(Command);
            if (bp.value == 0)
                bp = AdvancedCommandTok(Command);
            if (bp.value == 0)
                bp = BitwiseCommandTok(Command);
            if (bp.value == 0)
                bp = ConditionsTok(Command);
            if (bp.value == 0)
                bp = LogicTok(Command);
            if (bp.value == 0)
                bp = StoresTok(Command);
            if (bp.value == 0)
                bp = FlowTok(Command);
            if (bp.value == 0)
                bp = MasterFlowTok(Command);
            if (bp.value == 0 & Command.StartsWith('*'))
            {
                bp.tipo = 1;
                bp.value = SysvarTok(Command[1..], rob);
            }
            else if (bp.value == 0)
            {
                bp.tipo = 0;
                bp.value = SysvarTok(Command, rob);
            }

            return bp;
        }

        public static string Parse(DNABlock bp, robot rob = null, bool converttosysvar = true)
        {
            return bp.tipo switch
            {
                //number
                0 => converttosysvar ? SysvarDetok(bp.value, rob) : bp.value.ToString(),
                1 => "*" + SysvarDetok(bp.value, rob),
                2 => BasicCommandDetok(bp.value),
                3 => AdvancedCommandDetok(bp.value),
                4 => BitwiseCommandDetok(bp.value),
                5 => ConditionsDetok(bp.value),
                6 => LogicDetok(bp.value),
                7 => StoresDetok(bp.value),
                8 => "",
                9 => FlowDetok(bp.value),
                10 => MasterFlowDetok(bp.value),
                _ => "",
            };
        }

        public static string SaveRobHeader(robot rob)
        {
            var totmut = Math.Min(rob.Mutations + rob.OldMutations, MaxIntValue);

            return $"'#generation: {rob.generation}\n'#mutations: {totmut}\n";
        }

        public static string SysvarDetok(int n, robot rob = null, bool savingToFile = false)
        {
            var s = SystemVariables.FirstOrDefault(s => s.Value == n);

            if (s != null)
                return $".{s.Name}";

            if (savingToFile)
                return n.ToString();

            if (!(rob != null & n != 0)) return n.ToString();

            var v = rob.vars.FirstOrDefault(v => v.Value == n);

            return v != null ? $".{v.Name}" : n.ToString();
        }

        public static int SysvarTok(string a, robot rob)
        {
            if (a.StartsWith("."))
            {
                a = a[1..].ToLowerInvariant();

                var s = SystemVariables.FirstOrDefault(s => s.Name.Equals(a, StringComparison.InvariantCultureIgnoreCase));

                if (s != null)
                    return s.Value;

                var v = rob.vars.FirstOrDefault(s => s.Name.Equals(a, StringComparison.InvariantCultureIgnoreCase));

                if (v != null)
                    return v.Value;
            }
            else
            {
                var intValid = int.TryParse(a, out var val);

                if (intValid)
                    return val;
            }

            return 0;
        }

        public static string TipoDetok(int tipo)
        {
            return tipo switch
            {
                0 => "number",
                1 => "*number",
                2 => "basic command",
                3 => "advanced command",
                4 => "bit command",
                5 => "condition",
                6 => "logic operator",
                7 => "store command",
                9 => "flow command",
                _ => "",
            };
        }

        private static string AdvancedCommandDetok(int n)
        {
            return n switch
            {
                1 => "angle",
                2 => "dist",
                3 => "ceil",
                4 => "floor",
                5 => "sqr",
                6 => "pow",
                7 => "pyth",
                8 => "anglecmp",
                9 => "root",
                10 => "logx",
                11 => "sin",
                12 => "cos",
                _ => "",
            };
        }

        private static DNABlock AdvancedCommandTok(string s)
        {
            return new()
            {
                tipo = 3,
                value = s switch
                {
                    "angle" => 1,
                    "dist" => 2,
                    "ceil" => 3,
                    "floor" => 4,
                    "sqr" => 5,
                    "pow" => 6,
                    "pyth" => 7,
                    "anglecmp" => 8,
                    "root" => 9,
                    "logx" => 10,
                    "sin" => 11,
                    "cos" => 12,
                    _ => 0,
                },
            };
        }

        private static string BasicCommandDetok(int n)
        {
            return n switch
            {
                1 => "add",
                2 => "sub",
                3 => "mult",
                4 => "div",
                5 => "rnd",
                6 => "*",
                7 => "mod",
                8 => "sgn",
                9 => "abs",
                10 => "dup",
                11 => "drop",
                12 => "clear",
                13 => "swap",
                14 => "over",
                _ => "",
            };
        }

        private static DNABlock BasicCommandTok(string s)
        {
            return new()
            {
                tipo = 2,
                value = s switch
                {
                    "add" => 1,
                    "sub" => 2,
                    "mult" => 3,
                    "div" => 4,
                    "rnd" => 5,
                    "*" => 6,
                    "mod" => 7,
                    "sgn" => 8,
                    "abs" => 9,
                    "dup" => 10,
                    "dupint" => 10,
                    "drop" => 11,
                    "dropint" => 11,
                    "clear" => 12,
                    "clearint" => 12,
                    "swap" => 13,
                    "swapint" => 13,
                    "over" => 14,
                    "overint" => 14,
                    _ => 0,
                },
            };
        }

        private static string BitwiseCommandDetok(int n)
        {
            return n switch
            {
                1 => "~", // bitwise compliment
                2 => "&", // bitwise AND
                3 => "|", // bitwise OR
                4 => "^", // bitwise XOR
                5 => "++",
                6 => "--",
                7 => "-",
                8 => "<<", // bit shift left
                9 => ">>", // bit shift right
                _ => "",
            };
        }

        private static DNABlock BitwiseCommandTok(string s)
        {
            return new()
            {
                tipo = 4,
                value = s switch
                {
                    "~" => 1,
                    "&" => 2,
                    "|" => 3,
                    "^" => 4,
                    "++" => 5,
                    "--" => 6,
                    "-" => 7,
                    "<<" => 8,
                    ">>" => 9,
                    _ => 0,
                },
            };
        }

        private static string ConditionsDetok(int n)
        {
            return n switch
            {
                1 => "<",
                2 => ">",
                3 => "=",
                4 => "!=",
                5 => "%=",
                6 => "!%=",
                7 => "~=",
                8 => "!~=",
                9 => ">=",
                10 => "<=",
                _ => "",
            };
        }

        private static DNABlock ConditionsTok(string s)
        {
            return new()
            {
                tipo = 5,
                value = s switch
                {
                    "<" => 1,
                    ">" => 2,
                    "=" => 3,
                    "!=" => 4,
                    "%=" => 5,
                    "!%=" => 6,
                    "~=" => 7,
                    "!~=" => 8,
                    ">=" => 9,
                    "<=" => 10,
                    _ => 0,
                },
            };
        }

        private static string FlowDetok(int n)
        {
            return n switch
            {
                1 => "cond",
                2 => "start",
                3 => "else",
                4 => "stop",
                _ => "",
            };
        }

        private static DNABlock FlowTok(string s)
        {
            return new()
            {
                tipo = 9,
                value = s switch
                {
                    "cond" => 1,
                    "start" => 2,
                    "else" => 3,
                    "stop" => 4,
                    _ => 0,
                },
            };
        }

        private static void GetVals(robot rob, string a, string hold)
        {
            var parts = a.Split(":", 2);
            var name = parts[0].Trim()[3..];
            var value = parts[1].Trim();

            if (name == "generation")
            {
                var valValid = int.TryParse(value, out var val);
                rob.generation = valValid ? val : 0;
            }

            if (name == "mutations")
            {
                var valValid = int.TryParse(value, out var val);
                rob.OldMutations = valValid ? val : 0;
            }

            if (name == "tag")
                rob.tag = value.Substring(0, 45);

            if (name == "hash")
            {
                var value2 = Hash(hold);
                if (value2 != value)
                {
                    rob.generation = 0;
                    rob.OldMutations = 0;
                }
            }
        }

        private static string LogicDetok(int n)
        {
            return n switch
            {
                1 => "and",
                2 => "or",
                3 => "xor",
                4 => "not",
                5 => "true",
                6 => "false",
                7 => "dropbool",
                8 => "clearbool",
                9 => "dupbool",
                10 => "swapbool",
                11 => "overbool",
                _ => "",
            };
        }

        private static DNABlock LogicTok(string s)
        {
            return new()
            {
                tipo = 6,
                value = s switch
                {
                    "and" => 1,
                    "or" => 2,
                    "xor" => 3,
                    "not" => 4,
                    "true" => 5,
                    "false" => 6,
                    "dropbool" => 7,
                    "clearbool" => 8,
                    "dupbool" => 9,
                    "swapbool" => 10,
                    "overbool" => 11,
                    _ => 0,
                },
            };
        }

        private static string MasterFlowDetok(int n)
        {
            return n switch
            {
                1 => "end",
                _ => "",
            };
        }

        private static DNABlock MasterFlowTok(string s)
        {
            return new()
            {
                tipo = 10,
                value = s switch
                {
                    "end" => 1,
                    _ => 0,
                }
            };
        }

        private static string StoresDetok(int n)
        {
            return n switch
            {
                1 => "store",
                2 => "inc",
                3 => "dec",
                4 => "addstore",
                5 => "substore",
                6 => "multstore",
                7 => "divstore",
                8 => "ceilstore",
                9 => "floorstore",
                10 => "rndstore",
                11 => "sgnstore",
                12 => "absstore",
                13 => "sqrstore",
                14 => "negstore",
                _ => "",
            };
        }

        private static DNABlock StoresTok(string s)
        {
            return new()
            {
                tipo = 7,
                value = s switch
                {
                    "store" => 1,
                    "inc" => 2,
                    "dec" => 3,
                    "addstore" => 4,
                    "substore" => 5,
                    "multstore" => 6,
                    "divstore" => 7,
                    "ceilstore" => 8,
                    "floorstore" => 9,
                    "rndstore" => 10,
                    "sgnstore" => 11,
                    "absstore" => 12,
                    "sqrstore" => 13,
                    "negstore" => 14,
                    _ => 0,
                },
            };
        }
    }
}
