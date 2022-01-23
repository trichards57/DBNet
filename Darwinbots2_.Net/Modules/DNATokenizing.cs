using DarwinBots.Model;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DarwinBots.Modules
{
    internal static class DnaTokenizing
    {
        private static readonly int[,] DnaMatrix = new int[9, 14];

        static DnaTokenizing()
        {
            var count = 0;

            for (var type = 0; type < 9; type++)
            {
                for (var value = 0; value < 14; value++)
                {
                    var y = new DnaBlock
                    {
                        Type = type + 2,
                        Value = value + 1
                    };
                    var result = Parse(y);
                    if (result == "") continue;
                    DnaMatrix[type, value] = count;
                    count++;
                }
            }
        }

        public static string DetokenizeDna(Robot rob)
        {
            var result = new StringBuilder();
            var geneEnd = false;
            var dna = rob.Dna;
            var inGene = false;
            var coding = false;
            var t = 1;
            var gene = 0;
            var lastGene = 0;

            while (!(dna[t].Type == 10 & dna[t].Value == 1))
            {
                // If a Start or Else
                if (dna[t].Type == 9 && dna[t].Value is 2 or 3)
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
                if (dna[t].Type == 9 && (dna[t].Value == 1))
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
                if (dna[t].Type == 9 && dna[t].Value == 4)
                {
                    if (coding)
                        geneEnd = true;

                    inGene = false;
                    coding = false;
                }

                if (gene != lastGene)
                {
                    if (gene > 1)
                    {
                        result.AppendLine($"''''''''''''''''''''''''  Gene: {gene} Begins at position {t}  '''''''''''''''''''''''");
                    }

                    result.AppendLine();

                    lastGene = gene;
                }

                var convertToSysVar = dna[t + 1].Type == 7;
                var temp = Parse(dna[t], rob, convertToSysVar);
                if (temp == "")
                    temp = "VOID"; // alert user that there is an invalid DNA entry.

                result.Append(temp);
                // formatting
                if (dna[t].Type is 5 or 6 or 7 or 9)
                    result.AppendLine();

                if (geneEnd)
                {
                    // Indicate gene ended via a stop.  Needs to come after base pair
                    result.AppendLine($"''''''''''''''''''''''''  Gene: {gene} Ends at position {t}  '''''''''''''''''''''''");
                    geneEnd = false;
                }

                t++;
            }
            if (!(dna[t - 1].Type == 9 && dna[t - 1].Value == 4) && coding)
            {
                // End of DNA without a stop.
                result.AppendLine($"''''''''''''''''''''''''  Gene: {gene} Ends at position {t - 1}  '''''''''''''''''''''''");
            }

            return result.ToString();
        }

        public static int DnaToInt(int tipo, int value)
        {
            value = Math.Clamp(value, -32000, 32000);

            //figure out conversion
            if (tipo >= 2)
                return 32691 + DnaMatrix[tipo - 2, value - 1]; //dnaMatrix adds max of 76 because we have 76 commands

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

        public static async Task<bool> LoadDna(string path, Robot rob)
        {
            var hold = new StringBuilder();

            rob.Dna.Clear();

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
                        DnaManipulations.InsertVar(rob, a);
                    else
                    {
                        var parts = processedLine.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        foreach (var b in parts)
                            rob.Dna.Add(Parse(b, rob));
                    }
                }
                hold.AppendLine(a);
            }

            rob.Dna.Add(new DnaBlock { Type = 10, Value = 1 });

            return true;
        }

        public static string Parse(DnaBlock bp, Robot rob = null, bool convertToSystemVariable = true)
        {
            return bp.Type switch
            {
                //number
                0 => convertToSystemVariable ? SystemVariableDetokenize(bp.Value, rob) : bp.Value.ToString(),
                1 => "*" + SystemVariableDetokenize(bp.Value, rob),
                2 => BasicCommandDetok(bp.Value),
                3 => AdvancedCommandDetok(bp.Value),
                4 => BitwiseCommandDetok(bp.Value),
                5 => ConditionsDetok(bp.Value),
                6 => LogicDetok(bp.Value),
                7 => StoresDetok(bp.Value),
                8 => "",
                9 => FlowDetok(bp.Value),
                10 => MasterFlowDetok(bp.Value),
                _ => ""
            };
        }

        public static string SaveRobHeader(Robot rob)
        {
            var totalMutations = Math.Min(rob.Mutations + rob.OldMutations, DnaEngine.MaxIntValue);

            return $"'#generation: {rob.Generation}\n'#mutations: {totalMutations}\n";
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

        private static DnaBlock AdvancedCommandTok(string s)
        {
            return new()
            {
                Type = 3,
                Value = s switch
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

        private static DnaBlock BasicCommandTok(string s)
        {
            return new()
            {
                Type = 2,
                Value = s switch
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

        private static DnaBlock BitwiseCommandTok(string s)
        {
            return new()
            {
                Type = 4,
                Value = s switch
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

        private static DnaBlock ConditionsTok(string s)
        {
            return new()
            {
                Type = 5,
                Value = s switch
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

        private static DnaBlock FlowTok(string s)
        {
            return new()
            {
                Type = 9,
                Value = s switch
                {
                    "cond" => 1,
                    "start" => 2,
                    "else" => 3,
                    "stop" => 4,
                    _ => 0,
                },
            };
        }

        private static void GetVals(Robot rob, string a, string hold)
        {
            var parts = a.Split(":", 2);
            var name = parts[0].Trim()[3..];
            var value = parts[1].Trim();

            if (name == "generation")
            {
                var valValid = int.TryParse(value, out var val);
                rob.Generation = valValid ? val : 0;
            }

            if (name == "mutations")
            {
                var valValid = int.TryParse(value, out var val);
                rob.OldMutations = valValid ? val : 0;
            }

            if (name == "hash")
            {
                var value2 = Hash(hold);
                if (value2 != value)
                {
                    rob.Generation = 0;
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

        private static DnaBlock LogicTok(string s)
        {
            return new()
            {
                Type = 6,
                Value = s switch
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

        private static DnaBlock MasterFlowTok(string s)
        {
            return new()
            {
                Type = 10,
                Value = s switch
                {
                    "end" => 1,
                    _ => 0,
                }
            };
        }

        private static DnaBlock Parse(string command, Robot rob = null)
        {
            command = command.ToLowerInvariant();

            var bp = BasicCommandTok(command);
            if (bp.Value == 0)
                bp = AdvancedCommandTok(command);
            if (bp.Value == 0)
                bp = BitwiseCommandTok(command);
            if (bp.Value == 0)
                bp = ConditionsTok(command);
            if (bp.Value == 0)
                bp = LogicTok(command);
            if (bp.Value == 0)
                bp = StoresTok(command);
            if (bp.Value == 0)
                bp = FlowTok(command);
            if (bp.Value == 0)
                bp = MasterFlowTok(command);
            if (bp.Value == 0 & command.StartsWith('*'))
            {
                bp = new DnaBlock
                {
                    Type = 1,
                    Value = SystemVariableTokenize(command[1..], rob)
                };
            }
            else if (bp.Value == 0)
            {
                bp = new DnaBlock
                {
                    Type = 0,
                    Value = SystemVariableTokenize(command, rob)
                };
            }

            return bp;
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

        private static DnaBlock StoresTok(string s)
        {
            return new()
            {
                Type = 7,
                Value = s switch
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

        private static string SystemVariableDetokenize(int n, Robot rob = null, bool savingToFile = false)
        {
            var s = DnaEngine.SystemVariables.FirstOrDefault(t => t.Value == n);

            if (s != default)
                return $".{s.Name}";

            if (savingToFile)
                return n.ToString();

            if (rob == null || n == 0)
                return n.ToString();

            var v = rob.Variables.FirstOrDefault(u => u.Value == n);

            return v != default ? $".{v.Name}" : n.ToString();
        }

        private static int SystemVariableTokenize(string a, Robot rob)
        {
            if (a.StartsWith("."))
            {
                a = a[1..].ToLowerInvariant();

                var s = DnaEngine.SystemVariables.FirstOrDefault(t => t.Name.Equals(a, StringComparison.InvariantCultureIgnoreCase));

                if (s != default)
                    return s.Value;

                var v = rob.Variables.FirstOrDefault(t => t.Name.Equals(a, StringComparison.InvariantCultureIgnoreCase));

                if (v != default)
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
    }
}
