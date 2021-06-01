using Iersera.Model;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static DNAExecution;
using static DNAManipulations;

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
                if (result != "")
                {
                    dnamatrix[y_tipo, y_value] = count;
                    count++;
                }
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
        var result = 0;

        value = Math.Clamp(value, -32000, 32000);

        //figure out conversion
        if (tipo < 2)
        {
            result = -16646;

            if (Math.Abs(value) > 999)
                value = (int)(512 * Math.Sign(value) + value / 2.05);

            result += value;

            if (tipo == 1)
                result += 32729;
        }
        else if (tipo > 1)
        {
            //other types
            result = 32691 + dnamatrix[tipo - 2, value - 1]; //dnamatrix adds max of 76 because we have 76 commands
        }
        return result;
    }

    /*
    ' loads the dna and parses it
    */

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

    public static void LoadSysVars()
    {
        sysvar.Clear();
        sysvar.Add(new("up", 1, functional: true));
        sysvar.Add(new("dn", 2, functional: true));
        sysvar.Add(new("sx", 3, functional: true));
        sysvar.Add(new("dx", 4, functional: true));
        sysvar.Add(new("aimright", 5, functional: true, synonym: "aimdx"));
        sysvar.Add(new("aimleft", 6, functional: true, synonym: "aimsx"));
        sysvar.Add(new("shoot", 7, functional: true));
        sysvar.Add(new("shootval", 8, functional: true));
        sysvar.Add(new("robage", 9, true));
        sysvar.Add(new("mass", 10, true));
        sysvar.Add(new("maxvel", 11, true));
        sysvar.Add(new("timer", 12, true));
        sysvar.Add(new("aim", 18, true));
        sysvar.Add(new("setaim", 19, functional: true));
        sysvar.Add(new("bodgain", 194, true));
        sysvar.Add(new("bodloss", 195, true));
        sysvar.Add(new("velscalar", 196, true));
        sysvar.Add(new("velsx", 197, true));
        sysvar.Add(new("veldx", 198, true));
        sysvar.Add(new("veldn", 199, true));
        sysvar.Add(new("velup", 200, true));
        sysvar.Add(new("vel", 200, true));
        sysvar.Add(new("hit", 201, true));
        sysvar.Add(new("shflav", 202, true));
        sysvar.Add(new("pain", 203, true));
        sysvar.Add(new("pleas", 204, true));
        sysvar.Add(new("hitup", 205, true));
        sysvar.Add(new("hitdn", 206, true));
        sysvar.Add(new("hitdx", 207, true));
        sysvar.Add(new("hitsx", 208, true));
        sysvar.Add(new("shang", 209, true));
        sysvar.Add(new("shup", 210, true));
        sysvar.Add(new("shdn", 211, true));
        sysvar.Add(new("shdx", 212, true));
        sysvar.Add(new("shsx", 213, true));
        sysvar.Add(new("edge", 214, true));
        sysvar.Add(new("fixed", 215, true));
        sysvar.Add(new("fixpos", 216, functional: true));
        sysvar.Add(new("ypos", 217, true, synonym: "depth"));
        sysvar.Add(new("daytime", 218, true));
        sysvar.Add(new("xpos", 219, true));
        sysvar.Add(new("kills", 220, true));
        sysvar.Add(new("hitang", 221));
        sysvar.Add(new("repro", 300, functional: true));
        sysvar.Add(new("mrepro", 301, functional: true));
        sysvar.Add(new("sexrepro", 302, functional: true));
        sysvar.Add(new("fertilized", 303, true));
        sysvar.Add(new("nrg", 310, true));
        sysvar.Add(new("body", 311, true));
        sysvar.Add(new("fdbody", 312, functional: true));
        sysvar.Add(new("strbody", 313, functional: true));
        sysvar.Add(new("setboy", 314, functional: true));
        sysvar.Add(new("rdboy", 315, true));
        sysvar.Add(new("tie", 330, functional: true));
        sysvar.Add(new("stifftie", 331, functional: true));
        sysvar.Add(new("mkvirus", 335, functional: true));
        sysvar.Add(new("dnalen", 336, true));
        sysvar.Add(new("vtimer", 337, true));
        sysvar.Add(new("vshoot", 338, functional: true));
        sysvar.Add(new("genes", 339, true));
        sysvar.Add(new("delgene", 340, functional: true));
        sysvar.Add(new("thisgene", 341, true));
        sysvar.Add(new("sun", 400, true));
        sysvar.Add(new("totalbots", 401, true));
        sysvar.Add(new("totalmyspecies", 402, true));
        sysvar.Add(new("tout1", 410, functional: true));
        sysvar.Add(new("tout2", 411, functional: true));
        sysvar.Add(new("tout3", 412, functional: true));
        sysvar.Add(new("tout4", 413, functional: true));
        sysvar.Add(new("tout5", 414, functional: true));
        sysvar.Add(new("tout6", 415, functional: true));
        sysvar.Add(new("tout7", 416, functional: true));
        sysvar.Add(new("tout8", 417, functional: true));
        sysvar.Add(new("tout9", 418, functional: true));
        sysvar.Add(new("tout10", 419, functional: true));
        sysvar.Add(new("tin1", 420, true));
        sysvar.Add(new("tin2", 421, true));
        sysvar.Add(new("tin3", 422, true));
        sysvar.Add(new("tin4", 423, true));
        sysvar.Add(new("tin5", 424, true));
        sysvar.Add(new("tin6", 425, true));
        sysvar.Add(new("tin7", 426, true));
        sysvar.Add(new("tin8", 427, true));
        sysvar.Add(new("tin9", 428, true));
        sysvar.Add(new("tin10", 429, true));
        sysvar.Add(new("trefbody", 437, true));
        sysvar.Add(new("trefxpos", 438, true));
        sysvar.Add(new("trefypos", 439, true));
        sysvar.Add(new("trefvelmysx", 440, true));
        sysvar.Add(new("trefvelmydx", 441, true));
        sysvar.Add(new("trefvelmydn", 442, true));
        sysvar.Add(new("trefvelmyup", 443, true));
        sysvar.Add(new("trefvelscalar", 444, true));
        sysvar.Add(new("trefvelyoursx", 445, true));
        sysvar.Add(new("trefvelyourdx", 446, true));
        sysvar.Add(new("trefvelyourdn", 447, true));
        sysvar.Add(new("trefvelyourup", 448, true));
        sysvar.Add(new("trefshell", 449, true));
        sysvar.Add(new("tieang", 450, true));
        sysvar.Add(new("tielen", 451, true));
        sysvar.Add(new("tieloc", 452, functional: true));
        sysvar.Add(new("tieval", 453, functional: true));
        sysvar.Add(new("tiepres", 454, true));
        sysvar.Add(new("tienum", 455, functional: true));
        sysvar.Add(new("trefup", 456, true));
        sysvar.Add(new("trefdn", 457, true));
        sysvar.Add(new("trefsx", 458, true));
        sysvar.Add(new("trefdx", 459, true));
        sysvar.Add(new("trefaimdx", 460, true));
        sysvar.Add(new("trefaimsx", 461, true));
        sysvar.Add(new("trefshoot", 462, true));
        sysvar.Add(new("trefeye", 463, true));
        sysvar.Add(new("trefnrg", 464, true));
        sysvar.Add(new("trefage", 465, true));
        sysvar.Add(new("numties", 466, true));
        sysvar.Add(new("deltie", 467, functional: true));
        sysvar.Add(new("fixang", 468, functional: true));
        sysvar.Add(new("fixlen", 469, functional: true));
        sysvar.Add(new("multi", 470, true));
        sysvar.Add(new("readtie", 471, functional: true));
        sysvar.Add(new("memval", 473, true));
        sysvar.Add(new("memloc", 474, functional: true));
        sysvar.Add(new("tmemval", 475, true));
        sysvar.Add(new("tmemloc", 476, functional: true));
        sysvar.Add(new("reffixed", 477, true));
        sysvar.Add(new("treffixed", 478, true));
        sysvar.Add(new("trefaim", 479, true));
        sysvar.Add(new("tieang1", 480, true, true));
        sysvar.Add(new("tieang2", 481, true, true));
        sysvar.Add(new("tieang3", 482, true, true));
        sysvar.Add(new("tieang4", 483, true, true));
        sysvar.Add(new("tielen1", 484, true, true));
        sysvar.Add(new("tielen2", 485, true, true));
        sysvar.Add(new("tielen3", 486, true, true));
        sysvar.Add(new("tielen4", 487, true, true));
        sysvar.Add(new("eye1", 501, true));
        sysvar.Add(new("eye2", 502, true));
        sysvar.Add(new("eye3", 503, true));
        sysvar.Add(new("eye4", 504, true));
        sysvar.Add(new("eye5", 505, true));
        sysvar.Add(new("eye6", 506, true));
        sysvar.Add(new("eye7", 507, true));
        sysvar.Add(new("eye8", 508, true));
        sysvar.Add(new("eye9", 509, true));
        sysvar.Add(new("eyef", 510, true));
        sysvar.Add(new("focuseye", 511, functional: true));
        sysvar.Add(new("eye1dir", 521, functional: true));
        sysvar.Add(new("eye2dir", 522, functional: true));
        sysvar.Add(new("eye3dir", 523, functional: true));
        sysvar.Add(new("eye4dir", 524, functional: true));
        sysvar.Add(new("eye5dir", 525, functional: true));
        sysvar.Add(new("eye6dir", 526, functional: true));
        sysvar.Add(new("eye7dir", 527, functional: true));
        sysvar.Add(new("eye8dir", 528, functional: true));
        sysvar.Add(new("eye9dir", 529, functional: true));
        sysvar.Add(new("eye1width", 531, functional: true));
        sysvar.Add(new("eye2width", 532, functional: true));
        sysvar.Add(new("eye3width", 533, functional: true));
        sysvar.Add(new("eye4width", 534, functional: true));
        sysvar.Add(new("eye5width", 535, functional: true));
        sysvar.Add(new("eye6width", 536, functional: true));
        sysvar.Add(new("eye7width", 537, functional: true));
        sysvar.Add(new("eye8width", 538, functional: true));
        sysvar.Add(new("eye9width", 539, functional: true));
        sysvar.Add(new("reftype", 685, true));
        sysvar.Add(new("refmulti", 686, true));
        sysvar.Add(new("refshell", 687, true));
        sysvar.Add(new("refbody", 688, true));
        sysvar.Add(new("refxpos", 689, true));
        sysvar.Add(new("refypos", 690, true));
        sysvar.Add(new("refvelscalar", 695, true));
        sysvar.Add(new("refvelsx", 696, true));
        sysvar.Add(new("refveldx", 697, true));
        sysvar.Add(new("refveldn", 698, true));
        sysvar.Add(new("refvelup", 699, true, synonym: "refvel"));
        sysvar.Add(new("refup", 701, true));
        sysvar.Add(new("refdn", 702, true));
        sysvar.Add(new("refsx", 703, true));
        sysvar.Add(new("refdx", 704, true));
        sysvar.Add(new("refaimdx", 705, true));
        sysvar.Add(new("refaimsx", 706, true));
        sysvar.Add(new("refshoot", 707, true));
        sysvar.Add(new("refeye", 708, true));
        sysvar.Add(new("refnrg", 709, true));
        sysvar.Add(new("refage", 710, true));
        sysvar.Add(new("refaim", 711, true));
        sysvar.Add(new("reftie", 712, true));
        sysvar.Add(new("refpoison", 713, true));
        sysvar.Add(new("refvenom", 714, true));
        sysvar.Add(new("refkills", 715, true));
        sysvar.Add(new("myup", 721, true));
        sysvar.Add(new("mydn", 722, true));
        sysvar.Add(new("mysx", 723, true));
        sysvar.Add(new("mydx", 724, true));
        sysvar.Add(new("myaimdx", 725, true));
        sysvar.Add(new("myaimsx", 726, true));
        sysvar.Add(new("myshoot", 727, true));
        sysvar.Add(new("myeye", 728, true));
        sysvar.Add(new("myties", 729, true));
        sysvar.Add(new("mypoison", 730, true));
        sysvar.Add(new("myvenom", 731, true));
        sysvar.Add(new("out1", 800, functional: true));
        sysvar.Add(new("out2", 801, functional: true));
        sysvar.Add(new("out3", 802, functional: true));
        sysvar.Add(new("out4", 803, functional: true));
        sysvar.Add(new("out5", 804, functional: true));
        sysvar.Add(new("out6", 805, functional: true));
        sysvar.Add(new("out7", 806, functional: true));
        sysvar.Add(new("out8", 807, functional: true));
        sysvar.Add(new("out9", 808, functional: true));
        sysvar.Add(new("out10", 809, functional: true));
        sysvar.Add(new("in1", 810, true));
        sysvar.Add(new("in2", 811, true));
        sysvar.Add(new("in3", 812, true));
        sysvar.Add(new("in4", 813, true));
        sysvar.Add(new("in5", 814, true));
        sysvar.Add(new("in6", 815, true));
        sysvar.Add(new("in7", 816, true));
        sysvar.Add(new("in8", 817, true));
        sysvar.Add(new("in9", 818, true));
        sysvar.Add(new("in10", 819, true));
        sysvar.Add(new("mkslime", 820, functional: true));
        sysvar.Add(new("slime", 821, true));
        sysvar.Add(new("mkshell", 822, functional: true));
        sysvar.Add(new("shell", 823, true));
        sysvar.Add(new("mkvenom", 824, functional: true, synonym: "strvenom"));
        sysvar.Add(new("venom", 825, true));
        sysvar.Add(new("mkpoison", 826, functional: true, synonym: "strpoison"));
        sysvar.Add(new("poison", 827, true));
        sysvar.Add(new("waste", 828, true));
        sysvar.Add(new("pwaste", 829, true));
        sysvar.Add(new("sharenrg", 830, functional: true));
        sysvar.Add(new("sharewaste", 831, functional: true));
        sysvar.Add(new("shareshell", 832, functional: true));
        sysvar.Add(new("shareslime", 833, functional: true));
        sysvar.Add(new("ploc", 834, functional: true));
        sysvar.Add(new("vloc", 835, functional: true));
        sysvar.Add(new("venval", 836, functional: true));
        sysvar.Add(new("paralyzed", 837, true));
        sysvar.Add(new("poisoned", 838, true));
        sysvar.Add(new("pval", 839, functional: true));
        sysvar.Add(new("backshot", 900, functional: true));
        sysvar.Add(new("aimshoot", 901, functional: true));
        sysvar.Add(new("chlr", 920, true));
        sysvar.Add(new("mkchlr", 921, functional: true));
        sysvar.Add(new("rmchlr", 922, functional: true));
        sysvar.Add(new("light", 923, true));
        sysvar.Add(new("availability", 923, true));
        sysvar.Add(new("sharechlr", 924, functional: true));
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
        var totmut = Math.Min(rob.Mutations + rob.OldMutations, DNAExecution.MaxIntValue);

        return $"'#generation: {rob.generation}\n'#mutations: {totmut}\n";
    }

    public static string SysvarDetok(int n, robot rob = null, bool savingToFile = false)
    {
        var s = sysvar.FirstOrDefault(s => s.Value == n);

        if (s != null)
            return $".{s.Name}";

        if (savingToFile)
            return n.ToString();

        if (rob != null & n != 0)
        {
            var v = rob.vars.FirstOrDefault(v => v.Value == n);

            if (v != null)
                return $".{v.Name}";
        }

        return n.ToString();
    }

    public static int SysvarTok(string a, robot rob)
    {
        if (a.StartsWith("."))
        {
            a = a[1..].ToLowerInvariant();

            var s = sysvar.FirstOrDefault(s => s.Name.Equals(a, StringComparison.InvariantCultureIgnoreCase));

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
        return new DNABlock
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
        return new DNABlock
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
        return new DNABlock
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
        return new DNABlock
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
        return new DNABlock
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
        return new DNABlock
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
        return new DNABlock
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
        return new DNABlock
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
