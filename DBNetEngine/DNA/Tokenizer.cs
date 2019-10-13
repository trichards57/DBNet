using DBNetEngine.DNA.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("DBNetEngine.Tests")]

namespace DBNetEngine.DNA
{
    internal static class Tokenizer
    {
        static Tokenizer()
        {
            SystemVariables = LoadSystemVariables();
            DnaMatrix = LoadDnaMatrix();
        }

        public static IReadOnlyList<SystemVariable> SystemVariables { get; }

        private static int[,] DnaMatrix { get; }

        public static string BlockToCommand(Block block)
        {
            switch (block.Type)
            {
                case BlockType.MasterFlow:
                    return MasterFlowToCommand(block);

                case BlockType.Flow:
                    return FlowToCommand(block);

                case BlockType.Stores:
                    return StoreToCommand(block);

                case BlockType.Logic:
                    return LogicToCommand(block);

                case BlockType.Condition:
                    return ConditionToCommand(block);

                case BlockType.Bitwise:
                    return BitwiseToCommand(block);

                case BlockType.AdvancedCommand:
                    return AdvancedCommandToCommand(block);

                case BlockType.BasicCommand:
                    return BasicCommandToCommand(block);

                case BlockType.StarVariable:
                    return StarVariableToCommand(block);

                case BlockType.Variable:
                    return VariableToCommand(block);

                default:
                    return string.Empty;
            }
        }

        public static string DetokenizeDna(IList<Block> input)
        {
            var result = new StringBuilder();
            var geneCount = 0;
            var inGene = false;

            for (var i = 0; i < input.Count; i++)
            {
                var block = input[i];

                switch (block.Type)
                {
                    case BlockType.Flow:
                        switch (block.Value)
                        {
                            case Flow.Condition:
                                if (inGene)
                                    // A previous gene wasn't closed properly.
                                    result.AppendLine($"''' Gene {geneCount} ends at position {i - 1} '''");

                                geneCount++;

                                if (geneCount > 1)
                                    result.AppendLine();

                                result.AppendLine($"''' Gene {geneCount} begins at position {i} '''");
                                result.AppendLine("cond");
                                inGene = true;
                                break;

                            case Flow.Start:
                                result.AppendLine("start");
                                break;

                            case Flow.Else:
                                result.AppendLine("else");
                                break;

                            case Flow.Stop:
                                result.AppendLine("stop");
                                result.AppendLine($"''' Gene {geneCount} ends at position {i} '''");
                                inGene = false;
                                break;
                        }

                        break;

                    case BlockType.MasterFlow when block.Value == MasterFlow.End:
                        result.AppendLine();
                        result.AppendLine("end");
                        break;

                    default:
                        result.Append(" ");

                        if (input[i].Type != BlockType.Variable ||
                            (input[i].Type == BlockType.Variable && input[i + 1].Type == BlockType.Stores))
                            result.Append(BlockToCommand(block));
                        else
                            result.Append(block.Value);

                        if (block.Type != BlockType.StarVariable && block.Type != BlockType.Variable)
                            result.AppendLine();
                        break;
                }
            }

            return result.ToString();
        }

        public static int DnaToInt(Block block)
        {
            if (block.Type != BlockType.Variable && block.Type != BlockType.StarVariable)
                return DnaMatrix[(int)block.Type, block.Value] + 32691;

            var value = Math.Min(Math.Max(block.Value, -32000), 32000);

            var res = -16646;

            if (Math.Abs(value) > 999)
                value = 512 * Math.Sign(value) + (int)Math.Floor(value / 2.05);

            res += value;

            if (block.Type == BlockType.StarVariable)
                res += 32729;

            return res;
        }

        public static Block ParseCommand(string command)
        {
            var tok = TokenizeBasicCommand(command)
                   ?? TokenizeAdvancedCommand(command)
                   ?? TokenizeBitwise(command)
                   ?? TokenizeCondition(command)
                   ?? TokenizeLogic(command)
                   ?? TokenizeStore(command)
                   ?? TokenizeFlow(command)
                   ?? TokenizeMasterFlow(command);

            if (tok != null) return tok;

            tok = command.StartsWith("*", StringComparison.InvariantCultureIgnoreCase)
                ? ParseSystemVariable(command.Substring(1), true)
                : ParseSystemVariable(command);

            return tok ?? new Block(BlockType.Variable, 0);
        }

        public static IEnumerable<Block> ParseDna(string dnaText)
        {
            if (string.IsNullOrWhiteSpace(dnaText))
                return Enumerable.Empty<Block>();

            var result = new List<Block>();

            var lines = dnaText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("'", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (trimmedLine.Contains("'", StringComparison.InvariantCultureIgnoreCase))
                    trimmedLine = trimmedLine.Substring(0, trimmedLine.IndexOf("'", StringComparison.InvariantCulture)).Trim();

                var items = trimmedLine.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                result.AddRange(items.Select(ParseCommand));
            }

            if (!result.Any()) return result;

            var last = result.Last();

            if (last.Type != BlockType.MasterFlow || last.Value != 1)
            {
                result.Add(new Block(BlockType.MasterFlow, 1));
            }

            return result;
        }

        private static string AdvancedCommandToCommand(Block block)
        {
            switch (block.Value)
            {
                case AdvancedCommands.Angle:
                    return "angle";

                case AdvancedCommands.Distance:
                    return "dist";

                case AdvancedCommands.Ceiling:
                    return "ceil";

                case AdvancedCommands.Floor:
                    return "floor";

                case AdvancedCommands.Square:
                    return "sqr";

                case AdvancedCommands.Pow:
                    return "pow";

                case AdvancedCommands.Pythagorus:
                    return "pyth";

                case AdvancedCommands.AngleCompare:
                    return "anglecmp";

                case AdvancedCommands.Root:
                    return "root";

                case AdvancedCommands.LogX:
                    return "logx";

                case AdvancedCommands.Sin:
                    return "sin";

                case AdvancedCommands.Cos:
                    return "cos";

                case AdvancedCommands.DebugInt:
                    return "debugint";

                case AdvancedCommands.DebugBool:
                    return "debugbool";

                default:
                    return "";
            }
        }

        private static string BasicCommandToCommand(Block block)
        {
            switch (block.Value)
            {
                case BasicCommands.Add:
                    return "add";

                case BasicCommands.Subtract:
                    return "sub";

                case BasicCommands.Multiply:
                    return "mult";

                case BasicCommands.Divide:
                    return "div";

                case BasicCommands.Round:
                    return "rnd";

                case BasicCommands.Star:
                    return "*";

                case BasicCommands.Modulus:
                    return "mod";

                case BasicCommands.Sign:
                    return "sgn";

                case BasicCommands.Absolute:
                    return "abs";

                case BasicCommands.Duplicate:
                    return "dup";

                case BasicCommands.Drop:
                    return "drop";

                case BasicCommands.Clear:
                    return "clear";

                case BasicCommands.Swap:
                    return "swap";

                case BasicCommands.Over:
                    return "over";

                default:
                    return "";
            }
        }

        private static string BitwiseToCommand(Block block)
        {
            switch (block.Value)
            {
                case BitwiseCommands.Complement:
                    return "~";

                case BitwiseCommands.And:
                    return "&";

                case BitwiseCommands.Or:
                    return "|";

                case BitwiseCommands.XOr:
                    return "^";

                case BitwiseCommands.Increment:
                    return "++";

                case BitwiseCommands.Decrement:
                    return "--";

                case BitwiseCommands.Negate:
                    return "-";

                case BitwiseCommands.ShiftLeft:
                    return "<<";

                case BitwiseCommands.ShiftRight:
                    return ">>";

                default:
                    return "";
            }
        }

        private static string ConditionToCommand(Block block)
        {
            switch (block.Value)
            {
                case Condition.LessThan:
                    return "<";

                case Condition.GreaterThan:
                    return ">";

                case Condition.Equal:
                    return "=";

                case Condition.NotEqual:
                    return "!=";

                case Condition.CloselyEquals:
                    return "%=";

                case Condition.NotCloselyEquals:
                    return "!%=";

                case Condition.CustomCloselyEquals:
                    return "~=";

                case Condition.CustomNotCloselyEquals:
                    return "!~=";

                case Condition.GreaterThanOrEquals:
                    return ">=";

                case Condition.LessThanOrEquals:
                    return "<=";

                default:
                    return "";
            }
        }

        private static string FlowToCommand(Block block)
        {
            switch (block.Value)
            {
                case Flow.Condition:
                    return "cond";

                case Flow.Start:
                    return "start";

                case Flow.Else:
                    return "else";

                case Flow.Stop:
                    return "stop";

                default:
                    return "";
            }
        }

        private static int[,] LoadDnaMatrix()
        {
            var count = 1;

            var array = new int[11, 15];

            for (var type = 2; type < 11; type++)
            {
                for (var value = 0; value < 15; value++)
                {
                    var res = BlockToCommand(new Block((BlockType)type, value));
                    if (string.IsNullOrEmpty(res))
                        array[type, value] = 0;
                    else
                    {
                        array[type, value] = count;
                        count++;
                    }
                }
            }

            return array;
        }

        private static IReadOnlyList<SystemVariable> LoadSystemVariables()
        {
            var list = new List<SystemVariable>
            {
                new SystemVariable("up", 1, SystemVariableTypes.Functional),
                new SystemVariable("dn", 2, SystemVariableTypes.Functional),
                new SystemVariable("sx", 3, SystemVariableTypes.Functional),
                new SystemVariable("dx", 4, SystemVariableTypes.Functional),
                new SystemVariable("aimdx", 5, SystemVariableTypes.Functional),
                new SystemVariable("aimright", 5, SystemVariableTypes.Functional),
                new SystemVariable("aimsx", 6, SystemVariableTypes.Functional),
                new SystemVariable("aimleft", 6, SystemVariableTypes.Functional),
                new SystemVariable("shoot", 7, SystemVariableTypes.Functional),
                new SystemVariable("shootval", 8, SystemVariableTypes.Functional),
                new SystemVariable("robage", 9, SystemVariableTypes.Informational),
                new SystemVariable("mass", 10, SystemVariableTypes.Informational),
                new SystemVariable("maxvel", 11, SystemVariableTypes.Informational),
                new SystemVariable("timer", 12, SystemVariableTypes.Informational),
                new SystemVariable("aim", 18, SystemVariableTypes.Informational),
                new SystemVariable("setaim", 19, SystemVariableTypes.Functional),
                new SystemVariable("bodgain", 194, SystemVariableTypes.Informational),
                new SystemVariable("bodloss", 195, SystemVariableTypes.Informational),
                new SystemVariable("velscalar", 196, SystemVariableTypes.Informational),
                new SystemVariable("velsx", 197, SystemVariableTypes.Informational),
                new SystemVariable("veldx", 198, SystemVariableTypes.Informational),
                new SystemVariable("veldn", 199, SystemVariableTypes.Informational),
                new SystemVariable("vel", 200, SystemVariableTypes.Informational),
                new SystemVariable("velup", 200, SystemVariableTypes.Informational),
                new SystemVariable("hit", 201, SystemVariableTypes.Informational),
                new SystemVariable("shflav", 202, SystemVariableTypes.Informational),
                new SystemVariable("pain", 203, SystemVariableTypes.Informational),
                new SystemVariable("pleas", 204, SystemVariableTypes.Informational),
                new SystemVariable("hitup", 205, SystemVariableTypes.Informational),
                new SystemVariable("hitdn", 206, SystemVariableTypes.Informational),
                new SystemVariable("hitdx", 207, SystemVariableTypes.Informational),
                new SystemVariable("hitsx", 208, SystemVariableTypes.Informational),
                new SystemVariable("shang", 209, SystemVariableTypes.Informational),
                new SystemVariable("shup", 210, SystemVariableTypes.Informational),
                new SystemVariable("shdn", 211, SystemVariableTypes.Informational),
                new SystemVariable("shdx", 212, SystemVariableTypes.Informational),
                new SystemVariable("shsx", 213, SystemVariableTypes.Informational),
                new SystemVariable("edge", 214, SystemVariableTypes.Informational),
                new SystemVariable("fixed", 215, SystemVariableTypes.Informational),
                new SystemVariable("fixpos", 216, SystemVariableTypes.Functional),
                new SystemVariable("ypos", 217, SystemVariableTypes.Informational),
                new SystemVariable("depth", 217, SystemVariableTypes.Informational),
                new SystemVariable("daytime", 218, SystemVariableTypes.Informational),
                new SystemVariable("xpos", 219, SystemVariableTypes.Informational),
                new SystemVariable("kills", 220, SystemVariableTypes.Informational),
                new SystemVariable("hitang", 221, SystemVariableTypes.Informational),
                new SystemVariable("repro", 300, SystemVariableTypes.Functional),
                new SystemVariable("mrepro", 301, SystemVariableTypes.Functional),
                new SystemVariable("sexrepro", 302, SystemVariableTypes.Functional),
                new SystemVariable("nrg", 310, SystemVariableTypes.Informational),
                new SystemVariable("body", 311, SystemVariableTypes.Informational),
                new SystemVariable("fdbody", 312, SystemVariableTypes.Functional),
                new SystemVariable("strbody", 313, SystemVariableTypes.Functional),
                new SystemVariable("setboy", 314, SystemVariableTypes.Functional),
                new SystemVariable("rdboy", 315, SystemVariableTypes.Informational),
                new SystemVariable("tie", 330, SystemVariableTypes.Functional),
                new SystemVariable("stifftie", 331, SystemVariableTypes.Functional),
                new SystemVariable("mkvirus", 335, SystemVariableTypes.Functional),
                new SystemVariable("dnalen", 336, SystemVariableTypes.Informational),
                new SystemVariable("vtimer", 337, SystemVariableTypes.Informational),
                new SystemVariable("vshoot", 338, SystemVariableTypes.Functional),
                new SystemVariable("genes", 339, SystemVariableTypes.Informational),
                new SystemVariable("delgene", 340, SystemVariableTypes.Functional),
                new SystemVariable("thisgene", 341, SystemVariableTypes.Informational),
                new SystemVariable("sun", 400, SystemVariableTypes.Informational),
                new SystemVariable("trefbody", 437, SystemVariableTypes.Informational),
                new SystemVariable("trefxpos", 438, SystemVariableTypes.Informational),
                new SystemVariable("trefypos", 439, SystemVariableTypes.Informational),
                new SystemVariable("trefvelmysx", 440, SystemVariableTypes.Informational),
                new SystemVariable("trefvelmydx", 441, SystemVariableTypes.Informational),
                new SystemVariable("trefvelmydn", 442, SystemVariableTypes.Informational),
                new SystemVariable("trefvelmyup", 443, SystemVariableTypes.Informational),
                new SystemVariable("trefvelmyscalar", 444, SystemVariableTypes.Informational),
                new SystemVariable("trefvelyoursx", 445, SystemVariableTypes.Informational),
                new SystemVariable("trefvelyourdx", 446, SystemVariableTypes.Informational),
                new SystemVariable("trefvelyourdn", 447, SystemVariableTypes.Informational),
                new SystemVariable("trefvelyourup", 448, SystemVariableTypes.Informational),
                new SystemVariable("trefshell", 449, SystemVariableTypes.Informational),
                new SystemVariable("tieang", 450, SystemVariableTypes.Informational),
                new SystemVariable("tielen", 451, SystemVariableTypes.Informational),
                new SystemVariable("tieloc", 452, SystemVariableTypes.Functional),
                new SystemVariable("tieval", 453, SystemVariableTypes.Functional),
                new SystemVariable("tiepres", 454, SystemVariableTypes.Informational),
                new SystemVariable("tienum", 455, SystemVariableTypes.Functional),
                new SystemVariable("trefup", 456, SystemVariableTypes.Informational),
                new SystemVariable("trefdn", 457, SystemVariableTypes.Informational),
                new SystemVariable("trefsx", 458, SystemVariableTypes.Informational),
                new SystemVariable("trefdx", 459, SystemVariableTypes.Informational),
                new SystemVariable("trefaimdx", 460, SystemVariableTypes.Informational),
                new SystemVariable("trefaimsx", 461, SystemVariableTypes.Informational),
                new SystemVariable("trefshoot", 462, SystemVariableTypes.Informational),
                new SystemVariable("trefeye", 463, SystemVariableTypes.Informational),
                new SystemVariable("trefnrg", 464, SystemVariableTypes.Informational),
                new SystemVariable("trefage", 465, SystemVariableTypes.Informational),
                new SystemVariable("numties", 466, SystemVariableTypes.Informational),
                new SystemVariable("deltie", 467, SystemVariableTypes.Functional),
                new SystemVariable("fixang", 468, SystemVariableTypes.Functional),
                new SystemVariable("fixlen", 469, SystemVariableTypes.Functional),
                new SystemVariable("multi", 470, SystemVariableTypes.Informational),
                new SystemVariable("readtie", 471, SystemVariableTypes.Functional),
                new SystemVariable("fertilized", 303, SystemVariableTypes.Informational),
                new SystemVariable("memval", 473, SystemVariableTypes.Informational),
                new SystemVariable("memloc", 474, SystemVariableTypes.Functional),
                new SystemVariable("tmemval", 475, SystemVariableTypes.Informational),
                new SystemVariable("tmemloc", 476, SystemVariableTypes.Functional),
                new SystemVariable("reffixed", 477, SystemVariableTypes.Informational),
                new SystemVariable("treffixed", 478, SystemVariableTypes.Informational),
                new SystemVariable("trefaim", 479, SystemVariableTypes.Informational| SystemVariableTypes.Functional),
                new SystemVariable("tieang1", 480, SystemVariableTypes.Informational| SystemVariableTypes.Functional),
                new SystemVariable("tieang2", 481, SystemVariableTypes.Informational| SystemVariableTypes.Functional),
                new SystemVariable("tieang3", 482, SystemVariableTypes.Informational| SystemVariableTypes.Functional),
                new SystemVariable("tieang4", 483, SystemVariableTypes.Informational| SystemVariableTypes.Functional),
                new SystemVariable("tielen1", 484, SystemVariableTypes.Informational| SystemVariableTypes.Functional),
                new SystemVariable("tielen2", 485, SystemVariableTypes.Informational| SystemVariableTypes.Functional),
                new SystemVariable("tielen3", 486, SystemVariableTypes.Informational| SystemVariableTypes.Functional),
                new SystemVariable("tielen4", 487, SystemVariableTypes.Informational| SystemVariableTypes.Functional),
                new SystemVariable("eye1", 501, SystemVariableTypes.Informational),
                new SystemVariable("eye2", 502, SystemVariableTypes.Informational),
                new SystemVariable("eye3", 503, SystemVariableTypes.Informational),
                new SystemVariable("eye4", 504, SystemVariableTypes.Informational),
                new SystemVariable("eye5", 505, SystemVariableTypes.Informational),
                new SystemVariable("eye6", 506, SystemVariableTypes.Informational),
                new SystemVariable("eye7", 507, SystemVariableTypes.Informational),
                new SystemVariable("eye8", 508, SystemVariableTypes.Informational),
                new SystemVariable("eye9", 509, SystemVariableTypes.Informational),
                new SystemVariable("refmulti", 686, SystemVariableTypes.Informational),
                new SystemVariable("refshell", 687, SystemVariableTypes.Informational),
                new SystemVariable("refbody", 688, SystemVariableTypes.Informational),
                new SystemVariable("refxpos", 689, SystemVariableTypes.Informational),
                new SystemVariable("refypos", 690, SystemVariableTypes.Informational),
                new SystemVariable("refvelscalar", 695, SystemVariableTypes.Informational),
                new SystemVariable("refvelsx", 696, SystemVariableTypes.Informational),
                new SystemVariable("refveldx", 697, SystemVariableTypes.Informational),
                new SystemVariable("refveldn", 698, SystemVariableTypes.Informational),
                new SystemVariable("refvel", 699, SystemVariableTypes.Informational),
                new SystemVariable("refvelup", 699, SystemVariableTypes.Informational),
                new SystemVariable("refup", 701, SystemVariableTypes.Informational),
                new SystemVariable("refdn", 702, SystemVariableTypes.Informational),
                new SystemVariable("refsx", 703, SystemVariableTypes.Informational),
                new SystemVariable("refdx", 704, SystemVariableTypes.Informational),
                new SystemVariable("refaimdx", 705, SystemVariableTypes.Informational),
                new SystemVariable("refaimsx", 706, SystemVariableTypes.Informational),
                new SystemVariable("refshoot", 707, SystemVariableTypes.Informational),
                new SystemVariable("refeye", 708, SystemVariableTypes.Informational),
                new SystemVariable("refnrg", 709, SystemVariableTypes.Informational),
                new SystemVariable("refage", 710, SystemVariableTypes.Informational),
                new SystemVariable("refaim", 711, SystemVariableTypes.Informational),
                new SystemVariable("reftie", 712, SystemVariableTypes.Informational),
                new SystemVariable("refpoison", 713, SystemVariableTypes.Informational),
                new SystemVariable("refvenom", 714, SystemVariableTypes.Informational),
                new SystemVariable("refkills", 715, SystemVariableTypes.Informational),
                new SystemVariable("myup", 721, SystemVariableTypes.Informational),
                new SystemVariable("mydn", 722, SystemVariableTypes.Informational),
                new SystemVariable("mysx", 723, SystemVariableTypes.Informational),
                new SystemVariable("mydx", 724, SystemVariableTypes.Informational),
                new SystemVariable("myaimdx", 725, SystemVariableTypes.Informational),
                new SystemVariable("myaimsx", 726, SystemVariableTypes.Informational),
                new SystemVariable("myshoot", 727, SystemVariableTypes.Informational),
                new SystemVariable("myeye", 728, SystemVariableTypes.Informational),
                new SystemVariable("myties", 729, SystemVariableTypes.Informational),
                new SystemVariable("mypoison", 730, SystemVariableTypes.Informational),
                new SystemVariable("myvenom", 731, SystemVariableTypes.Informational),
                new SystemVariable("out1", 800, SystemVariableTypes.Functional),
                new SystemVariable("out2", 801, SystemVariableTypes.Functional),
                new SystemVariable("out3", 802, SystemVariableTypes.Functional),
                new SystemVariable("out4", 803, SystemVariableTypes.Functional),
                new SystemVariable("out5", 804, SystemVariableTypes.Functional),
                new SystemVariable("out6", 805, SystemVariableTypes.Functional),
                new SystemVariable("out7", 806, SystemVariableTypes.Functional),
                new SystemVariable("out8", 807, SystemVariableTypes.Functional),
                new SystemVariable("out9", 808, SystemVariableTypes.Functional),
                new SystemVariable("out10", 809, SystemVariableTypes.Functional),
                new SystemVariable("in1", 810, SystemVariableTypes.Informational),
                new SystemVariable("in2", 811, SystemVariableTypes.Informational),
                new SystemVariable("in3", 812, SystemVariableTypes.Informational),
                new SystemVariable("in4", 813, SystemVariableTypes.Informational),
                new SystemVariable("in5", 814, SystemVariableTypes.Informational),
                new SystemVariable("in6", 815, SystemVariableTypes.Informational),
                new SystemVariable("in7", 816, SystemVariableTypes.Informational),
                new SystemVariable("in8", 817, SystemVariableTypes.Informational),
                new SystemVariable("in9", 818, SystemVariableTypes.Informational),
                new SystemVariable("in10", 819, SystemVariableTypes.Informational),
                new SystemVariable("mkslime", 820, SystemVariableTypes.Functional),
                new SystemVariable("slime", 821, SystemVariableTypes.Informational),
                new SystemVariable("mkshell", 822, SystemVariableTypes.Functional),
                new SystemVariable("shell", 823, SystemVariableTypes.Informational),
                new SystemVariable("mkvenom", 824, SystemVariableTypes.Functional),
                new SystemVariable("strvenom", 824, SystemVariableTypes.Functional),
                new SystemVariable("venom", 825, SystemVariableTypes.Informational),
                new SystemVariable("mkpoison", 826, SystemVariableTypes.Functional),
                new SystemVariable("strpoison", 826, SystemVariableTypes.Functional),
                new SystemVariable("poison", 827, SystemVariableTypes.Informational),
                new SystemVariable("waste", 828, SystemVariableTypes.Informational),
                new SystemVariable("pwaste", 829, SystemVariableTypes.Informational),
                new SystemVariable("sharenrg", 830, SystemVariableTypes.Functional),
                new SystemVariable("sharewaste", 831, SystemVariableTypes.Functional),
                new SystemVariable("shareshell", 832, SystemVariableTypes.Functional),
                new SystemVariable("shareslime", 833, SystemVariableTypes.Functional),
                new SystemVariable("ploc", 834, SystemVariableTypes.Functional),
                new SystemVariable("vloc", 835, SystemVariableTypes.Functional),
                new SystemVariable("venval", 836, SystemVariableTypes.Functional),
                new SystemVariable("paralyzed", 837, SystemVariableTypes.Informational),
                new SystemVariable("poisoned", 838, SystemVariableTypes.Informational),
                new SystemVariable("backshot", 900, SystemVariableTypes.Functional),
                new SystemVariable("aimshoot", 901, SystemVariableTypes.Functional),
                new SystemVariable("eyef", 510, SystemVariableTypes.Informational),
                new SystemVariable("focuseye", 511, SystemVariableTypes.Functional),
                new SystemVariable("eye1dir", 521, SystemVariableTypes.Functional),
                new SystemVariable("eye2dir", 522, SystemVariableTypes.Functional),
                new SystemVariable("eye3dir", 523, SystemVariableTypes.Functional),
                new SystemVariable("eye4dir", 524, SystemVariableTypes.Functional),
                new SystemVariable("eye5dir", 525, SystemVariableTypes.Functional),
                new SystemVariable("eye6dir", 526, SystemVariableTypes.Functional),
                new SystemVariable("eye7dir", 527, SystemVariableTypes.Functional),
                new SystemVariable("eye8dir", 528, SystemVariableTypes.Functional),
                new SystemVariable("eye9dir", 529, SystemVariableTypes.Functional),
                new SystemVariable("eye1width", 531, SystemVariableTypes.Functional),
                new SystemVariable("eye2width", 532, SystemVariableTypes.Functional),
                new SystemVariable("eye3width", 533, SystemVariableTypes.Functional),
                new SystemVariable("eye4width", 534, SystemVariableTypes.Functional),
                new SystemVariable("eye5width", 535, SystemVariableTypes.Functional),
                new SystemVariable("eye6width", 536, SystemVariableTypes.Functional),
                new SystemVariable("eye7width", 537, SystemVariableTypes.Functional),
                new SystemVariable("eye8width", 538, SystemVariableTypes.Functional),
                new SystemVariable("eye9width", 539, SystemVariableTypes.Functional),
                new SystemVariable("reftype", 685, SystemVariableTypes.Informational),
                new SystemVariable("totalbots", 401, SystemVariableTypes.Informational),
                new SystemVariable("totalmyspecies", 402, SystemVariableTypes.Informational),
                new SystemVariable("tout1", 410, SystemVariableTypes.Functional),
                new SystemVariable("tout2", 411, SystemVariableTypes.Functional),
                new SystemVariable("tout3", 412, SystemVariableTypes.Functional),
                new SystemVariable("tout4", 413, SystemVariableTypes.Functional),
                new SystemVariable("tout5", 414, SystemVariableTypes.Functional),
                new SystemVariable("tout6", 415, SystemVariableTypes.Functional),
                new SystemVariable("tout7", 416, SystemVariableTypes.Functional),
                new SystemVariable("tout8", 417, SystemVariableTypes.Functional),
                new SystemVariable("tout9", 418, SystemVariableTypes.Functional),
                new SystemVariable("tout10", 419, SystemVariableTypes.Functional),
                new SystemVariable("tin1", 420, SystemVariableTypes.Informational),
                new SystemVariable("tin2", 421, SystemVariableTypes.Informational),
                new SystemVariable("tin3", 422, SystemVariableTypes.Informational),
                new SystemVariable("tin4", 423, SystemVariableTypes.Informational),
                new SystemVariable("tin5", 424, SystemVariableTypes.Informational),
                new SystemVariable("tin6", 425, SystemVariableTypes.Informational),
                new SystemVariable("tin7", 426, SystemVariableTypes.Informational),
                new SystemVariable("tin8", 427, SystemVariableTypes.Informational),
                new SystemVariable("tin9", 428, SystemVariableTypes.Informational),
                new SystemVariable("tin10", 429, SystemVariableTypes.Informational),
                new SystemVariable("pval", 839, SystemVariableTypes.Functional),
                new SystemVariable("chlr", 920, SystemVariableTypes.Informational),
                new SystemVariable("mkchlr", 921, SystemVariableTypes.Functional),
                new SystemVariable("rmchlr", 922, SystemVariableTypes.Functional),
                new SystemVariable("light", 923, SystemVariableTypes.Informational),
                new SystemVariable("availability", 923, SystemVariableTypes.Informational),
                new SystemVariable("sharechlr", 924, SystemVariableTypes.Functional)
            };

            return list.AsReadOnly();
        }

        private static string LogicToCommand(Block block)
        {
            switch (block.Value)
            {
                case Logic.And:
                    return "and";

                case Logic.Or:
                    return "or";

                case Logic.XOr:
                    return "xor";

                case Logic.Not:
                    return "not";

                case Logic.True:
                    return "true";

                case Logic.False:
                    return "false";

                case Logic.DropBool:
                    return "dropbool";

                case Logic.ClearBool:
                    return "clearbool";

                case Logic.DuplicateBool:
                    return "dupbool";

                case Logic.SwapBool:
                    return "swapbool";

                case Logic.OverBool:
                    return "overbool";

                default:
                    return "";
            }
        }

        private static string MasterFlowToCommand(Block block)
        {
            switch (block.Value)
            {
                case MasterFlow.End:
                    return "end";

                default:
                    return "";
            }
        }

        private static Block ParseSystemVariable(string command, bool starReference = false)
        {
            if (command.StartsWith(".", StringComparison.InvariantCultureIgnoreCase))
            {
                command = command.Substring(1);

                var systemVariable = SystemVariables.FirstOrDefault(s =>
                    s.Name.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (systemVariable != null)
                {
                    return new Block(starReference ? BlockType.StarVariable : BlockType.Variable,
                        systemVariable.Address);
                }
            }
            else
            {
                var val = int.TryParse(command, out var address);

                if (val)
                    return new Block(starReference ? BlockType.StarVariable : BlockType.Variable, address);
            }

            return null;
        }

        private static string StarVariableToCommand(Block block)
        {
            return $"*{VariableToCommand(block)}";
        }

        private static string StoreToCommand(Block block)
        {
            switch (block.Value)
            {
                case Stores.Store:
                    return "store";

                case Stores.Increment:
                    return "inc";

                case Stores.Decrement:
                    return "dec";

                case Stores.AddStore:
                    return "addstore";

                case Stores.SubtractStore:
                    return "substore";

                case Stores.MultiplyStore:
                    return "multstore";

                case Stores.DivideStore:
                    return "divstore";

                case Stores.CeilingStore:
                    return "ceilstore";

                case Stores.FloorStore:
                    return "floorstore";

                case Stores.RoundStore:
                    return "rndstore";

                case Stores.SignStore:
                    return "sgnstore";

                case Stores.AbsoluteStore:
                    return "absstore";

                case Stores.SquareStore:
                    return "sqrstore";

                case Stores.NegateStore:
                    return "negstore";

                default:
                    return "";
            }
        }

        private static Block TokenizeAdvancedCommand(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "angle":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.Angle);

                case "dist":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.Distance);

                case "ceil":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.Ceiling);

                case "floor":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.Floor);

                case "sqr":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.Square);

                case "pow":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.Pow);

                case "pyth":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.Pythagorus);

                case "anglecmp":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.AngleCompare);

                case "root":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.Root);

                case "logx":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.LogX);

                case "sin":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.Sin);

                case "cos":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.Cos);

                case "debugint":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.DebugInt);

                case "debugbool":
                    return new Block(BlockType.AdvancedCommand, AdvancedCommands.DebugBool);

                default:
                    return null;
            }
        }

        private static Block TokenizeBasicCommand(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "add":
                    return new Block(BlockType.BasicCommand, BasicCommands.Add);

                case "sub":
                    return new Block(BlockType.BasicCommand, BasicCommands.Subtract);

                case "mult":
                    return new Block(BlockType.BasicCommand, BasicCommands.Multiply);

                case "div":
                    return new Block(BlockType.BasicCommand, BasicCommands.Divide);

                case "rnd":
                    return new Block(BlockType.BasicCommand, BasicCommands.Round);

                case "*":
                    return new Block(BlockType.BasicCommand, BasicCommands.Star);

                case "mod":
                    return new Block(BlockType.BasicCommand, BasicCommands.Modulus);

                case "sgn":
                    return new Block(BlockType.BasicCommand, BasicCommands.Sign);

                case "abs":
                    return new Block(BlockType.BasicCommand, BasicCommands.Absolute);

                case "dup":
                case "dupint":
                    return new Block(BlockType.BasicCommand, BasicCommands.Duplicate);

                case "drop":
                case "dropint":
                    return new Block(BlockType.BasicCommand, BasicCommands.Drop);

                case "clear":
                case "clearint":
                    return new Block(BlockType.BasicCommand, BasicCommands.Clear);

                case "swap":
                case "swapint":
                    return new Block(BlockType.BasicCommand, BasicCommands.Swap);

                case "over":
                case "overint":
                    return new Block(BlockType.BasicCommand, BasicCommands.Over);

                default:
                    return null;
            }
        }

        private static Block TokenizeBitwise(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "~":
                    return new Block(BlockType.Bitwise, BitwiseCommands.Complement);

                case "&":
                    return new Block(BlockType.Bitwise, BitwiseCommands.And);

                case "|":
                    return new Block(BlockType.Bitwise, BitwiseCommands.Or);

                case "^":
                    return new Block(BlockType.Bitwise, BitwiseCommands.XOr);

                case "++":
                    return new Block(BlockType.Bitwise, BitwiseCommands.Increment);

                case "--":
                    return new Block(BlockType.Bitwise, BitwiseCommands.Decrement);

                case "-":
                    return new Block(BlockType.Bitwise, BitwiseCommands.Negate);

                case "<<":
                    return new Block(BlockType.Bitwise, BitwiseCommands.ShiftLeft);

                case ">>":
                    return new Block(BlockType.Bitwise, BitwiseCommands.ShiftRight);

                default:
                    return null;
            }
        }

        private static Block TokenizeCondition(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "<":
                    return new Block(BlockType.Condition, Condition.LessThan);

                case ">":
                    return new Block(BlockType.Condition, Condition.GreaterThan);

                case "=":
                    return new Block(BlockType.Condition, Condition.Equal);

                case "!=":
                    return new Block(BlockType.Condition, Condition.NotEqual);

                case "%=":
                    return new Block(BlockType.Condition, Condition.CloselyEquals);

                case "!%=":
                    return new Block(BlockType.Condition, Condition.NotCloselyEquals);

                case "~=":
                    return new Block(BlockType.Condition, Condition.CustomCloselyEquals);

                case "!~=":
                    return new Block(BlockType.Condition, Condition.CustomNotCloselyEquals);

                case ">=":
                    return new Block(BlockType.Condition, Condition.GreaterThanOrEquals);

                case "<=":
                    return new Block(BlockType.Condition, Condition.LessThanOrEquals);

                default:
                    return null;
            }
        }

        private static Block TokenizeFlow(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "cond":
                    return new Block(BlockType.Flow, Flow.Condition);

                case "start":
                    return new Block(BlockType.Flow, Flow.Start);

                case "else":
                    return new Block(BlockType.Flow, Flow.Else);

                case "stop":
                    return new Block(BlockType.Flow, Flow.Stop);

                default:
                    return null;
            }
        }

        private static Block TokenizeLogic(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "and":
                    return new Block(BlockType.Logic, Logic.And);

                case "or":
                    return new Block(BlockType.Logic, Logic.Or);

                case "xor":
                    return new Block(BlockType.Logic, Logic.XOr);

                case "not":
                    return new Block(BlockType.Logic, Logic.Not);

                case "true":
                    return new Block(BlockType.Logic, Logic.True);

                case "false":
                    return new Block(BlockType.Logic, Logic.False);

                case "dropbool":
                    return new Block(BlockType.Logic, Logic.DropBool);

                case "clearbool":
                    return new Block(BlockType.Logic, Logic.ClearBool);

                case "dupbool":
                    return new Block(BlockType.Logic, Logic.DuplicateBool);

                case "swapbool":
                    return new Block(BlockType.Logic, Logic.SwapBool);

                case "overbool":
                    return new Block(BlockType.Logic, Logic.OverBool);

                default:
                    return null;
            }
        }

        private static Block TokenizeMasterFlow(string command)
        {
            return command.Equals("end", StringComparison.InvariantCultureIgnoreCase) ? new Block(BlockType.MasterFlow, 1) : null;
        }

        private static Block TokenizeStore(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "store":
                    return new Block(BlockType.Stores, Stores.Store);

                case "inc":
                    return new Block(BlockType.Stores, Stores.Increment);

                case "dec":
                    return new Block(BlockType.Stores, Stores.Decrement);

                case "addstore":
                    return new Block(BlockType.Stores, Stores.AddStore);

                case "substore":
                    return new Block(BlockType.Stores, Stores.SubtractStore);

                case "multstore":
                    return new Block(BlockType.Stores, Stores.MultiplyStore);

                case "divstore":
                    return new Block(BlockType.Stores, Stores.DivideStore);

                case "ceilstore":
                    return new Block(BlockType.Stores, Stores.CeilingStore);

                case "floorstore":
                    return new Block(BlockType.Stores, Stores.FloorStore);

                case "rndstore":
                    return new Block(BlockType.Stores, Stores.RoundStore);

                case "sgnstore":
                    return new Block(BlockType.Stores, Stores.SignStore);

                case "absstore":
                    return new Block(BlockType.Stores, Stores.AbsoluteStore);

                case "sqrstore":
                    return new Block(BlockType.Stores, Stores.SquareStore);

                case "negstore":
                    return new Block(BlockType.Stores, Stores.NegateStore);

                default:
                    return null;
            }
        }

        private static string VariableToCommand(Block block)
        {
            var variable = SystemVariables.Where(v => v.Address == block.Value).OrderBy(v => v.Name.Length).FirstOrDefault();

            return variable == null ? block.Value.ToString() : $".{variable.Name}";
        }
    }
}