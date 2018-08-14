using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DBNetModel.DNA.Commands;

[assembly: InternalsVisibleTo("DBNetModel.Tests")]

namespace DBNetModel.DNA
{
    public static class Tokenizer
    {
        static Tokenizer()
        {
            SystemVariables = LoadSystemVariables();
            DnaMatrix = LoadDnaMatrix();
        }

        [ItemNotNull, NotNull]
        public static IReadOnlyList<SystemVariable> SystemVariables { get; }

        private static int[,] DnaMatrix { get; }

        [ItemNotNull, NotNull]
        public static IEnumerable<Block> ParseDna(string dnaText)
        {
            if (string.IsNullOrWhiteSpace(dnaText))
                return Enumerable.Empty<Block>();

            var result = new List<Block>();

            var lines = dnaText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("'"))
                    continue;

                if (trimmedLine.Contains("'"))
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

        [NotNull]
        internal static string BlockToCommand([NotNull] Block block)
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

        internal static int DnaToInt([NotNull] Block block)
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

        [NotNull]
        internal static Block ParseCommand(string command)
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

            tok = command.StartsWith("*")
                ? ParseSystemVariable(command.Substring(1), true)
                : ParseSystemVariable(command);

            return tok ?? new Block(BlockType.Variable, 0);
        }

        [NotNull]
        private static string AdvancedCommandToCommand([NotNull] Block block)
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

        [NotNull]
        private static string BasicCommandToCommand([NotNull] Block block)
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

        [NotNull]
        private static string BitwiseToCommand([NotNull] Block block)
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

        [NotNull]
        private static string ConditionToCommand([NotNull] Block block)
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

        [NotNull]
        private static string FlowToCommand([NotNull] Block block)
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

        [NotNull, ItemNotNull]
        private static IReadOnlyList<SystemVariable> LoadSystemVariables()
        {
            var list = new List<SystemVariable>
            {
                new SystemVariable("up", 1, SystemVariableType.Functional),
                new SystemVariable("dn", 2, SystemVariableType.Functional),
                new SystemVariable("sx", 3, SystemVariableType.Functional),
                new SystemVariable("dx", 4, SystemVariableType.Functional),
                new SystemVariable("aimdx", 5, SystemVariableType.Functional),
                new SystemVariable("aimright", 5, SystemVariableType.Functional),
                new SystemVariable("aimsx", 6, SystemVariableType.Functional),
                new SystemVariable("aimleft", 6, SystemVariableType.Functional),
                new SystemVariable("shoot", 7, SystemVariableType.Functional),
                new SystemVariable("shootval", 8, SystemVariableType.Functional),
                new SystemVariable("robage", 9, SystemVariableType.Informational),
                new SystemVariable("mass", 10, SystemVariableType.Informational),
                new SystemVariable("maxvel", 11, SystemVariableType.Informational),
                new SystemVariable("timer", 12, SystemVariableType.Informational),
                new SystemVariable("aim", 18, SystemVariableType.Informational),
                new SystemVariable("setaim", 19, SystemVariableType.Functional),
                new SystemVariable("bodgain", 194, SystemVariableType.Informational),
                new SystemVariable("bodloss", 195, SystemVariableType.Informational),
                new SystemVariable("velscalar", 196, SystemVariableType.Informational),
                new SystemVariable("velsx", 197, SystemVariableType.Informational),
                new SystemVariable("veldx", 198, SystemVariableType.Informational),
                new SystemVariable("veldn", 199, SystemVariableType.Informational),
                new SystemVariable("vel", 200, SystemVariableType.Informational),
                new SystemVariable("velup", 200, SystemVariableType.Informational),
                new SystemVariable("hit", 201, SystemVariableType.Informational),
                new SystemVariable("shflav", 202, SystemVariableType.Informational),
                new SystemVariable("pain", 203, SystemVariableType.Informational),
                new SystemVariable("pleas", 204, SystemVariableType.Informational),
                new SystemVariable("hitup", 205, SystemVariableType.Informational),
                new SystemVariable("hitdn", 206, SystemVariableType.Informational),
                new SystemVariable("hitdx", 207, SystemVariableType.Informational),
                new SystemVariable("hitsx", 208, SystemVariableType.Informational),
                new SystemVariable("shang", 209, SystemVariableType.Informational),
                new SystemVariable("shup", 210, SystemVariableType.Informational),
                new SystemVariable("shdn", 211, SystemVariableType.Informational),
                new SystemVariable("shdx", 212, SystemVariableType.Informational),
                new SystemVariable("shsx", 213, SystemVariableType.Informational),
                new SystemVariable("edge", 214, SystemVariableType.Informational),
                new SystemVariable("fixed", 215, SystemVariableType.Informational),
                new SystemVariable("fixpos", 216, SystemVariableType.Functional),
                new SystemVariable("ypos", 217, SystemVariableType.Informational),
                new SystemVariable("depth", 217, SystemVariableType.Informational),
                new SystemVariable("daytime", 218, SystemVariableType.Informational),
                new SystemVariable("xpos", 219, SystemVariableType.Informational),
                new SystemVariable("kills", 220, SystemVariableType.Informational),
                new SystemVariable("hitang", 221, SystemVariableType.Informational),
                new SystemVariable("repro", 300, SystemVariableType.Functional),
                new SystemVariable("mrepro", 301, SystemVariableType.Functional),
                new SystemVariable("sexrepro", 302, SystemVariableType.Functional),
                new SystemVariable("nrg", 310, SystemVariableType.Informational),
                new SystemVariable("body", 311, SystemVariableType.Informational),
                new SystemVariable("fdbody", 312, SystemVariableType.Functional),
                new SystemVariable("strbody", 313, SystemVariableType.Functional),
                new SystemVariable("setboy", 314, SystemVariableType.Functional),
                new SystemVariable("rdboy", 315, SystemVariableType.Informational),
                new SystemVariable("tie", 330, SystemVariableType.Functional),
                new SystemVariable("stifftie", 331, SystemVariableType.Functional),
                new SystemVariable("mkvirus", 335, SystemVariableType.Functional),
                new SystemVariable("dnalen", 336, SystemVariableType.Informational),
                new SystemVariable("vtimer", 337, SystemVariableType.Informational),
                new SystemVariable("vshoot", 338, SystemVariableType.Functional),
                new SystemVariable("genes", 339, SystemVariableType.Informational),
                new SystemVariable("delgene", 340, SystemVariableType.Functional),
                new SystemVariable("thisgene", 341, SystemVariableType.Informational),
                new SystemVariable("sun", 400, SystemVariableType.Informational),
                new SystemVariable("trefbody", 437, SystemVariableType.Informational),
                new SystemVariable("trefxpos", 438, SystemVariableType.Informational),
                new SystemVariable("trefypos", 439, SystemVariableType.Informational),
                new SystemVariable("trefvelmysx", 440, SystemVariableType.Informational),
                new SystemVariable("trefvelmydx", 441, SystemVariableType.Informational),
                new SystemVariable("trefvelmydn", 442, SystemVariableType.Informational),
                new SystemVariable("trefvelmyup", 443, SystemVariableType.Informational),
                new SystemVariable("trefvelmyscalar", 444, SystemVariableType.Informational),
                new SystemVariable("trefvelyoursx", 445, SystemVariableType.Informational),
                new SystemVariable("trefvelyourdx", 446, SystemVariableType.Informational),
                new SystemVariable("trefvelyourdn", 447, SystemVariableType.Informational),
                new SystemVariable("trefvelyourup", 448, SystemVariableType.Informational),
                new SystemVariable("trefshell", 449, SystemVariableType.Informational),
                new SystemVariable("tieang", 450, SystemVariableType.Informational),
                new SystemVariable("tielen", 451, SystemVariableType.Informational),
                new SystemVariable("tieloc", 452, SystemVariableType.Functional),
                new SystemVariable("tieval", 453, SystemVariableType.Functional),
                new SystemVariable("tiepres", 454, SystemVariableType.Informational),
                new SystemVariable("tienum", 455, SystemVariableType.Functional),
                new SystemVariable("trefup", 456, SystemVariableType.Informational),
                new SystemVariable("trefdn", 457, SystemVariableType.Informational),
                new SystemVariable("trefsx", 458, SystemVariableType.Informational),
                new SystemVariable("trefdx", 459, SystemVariableType.Informational),
                new SystemVariable("trefaimdx", 460, SystemVariableType.Informational),
                new SystemVariable("trefaimsx", 461, SystemVariableType.Informational),
                new SystemVariable("trefshoot", 462, SystemVariableType.Informational),
                new SystemVariable("trefeye", 463, SystemVariableType.Informational),
                new SystemVariable("trefnrg", 464, SystemVariableType.Informational),
                new SystemVariable("trefage", 465, SystemVariableType.Informational),
                new SystemVariable("numties", 466, SystemVariableType.Informational),
                new SystemVariable("deltie", 467, SystemVariableType.Functional),
                new SystemVariable("fixang", 468, SystemVariableType.Functional),
                new SystemVariable("fixlen", 469, SystemVariableType.Functional),
                new SystemVariable("multi", 470, SystemVariableType.Informational),
                new SystemVariable("readtie", 471, SystemVariableType.Functional),
                new SystemVariable("fertilized", 303, SystemVariableType.Informational),
                new SystemVariable("memval", 473, SystemVariableType.Informational),
                new SystemVariable("memloc", 474, SystemVariableType.Functional),
                new SystemVariable("tmemval", 475, SystemVariableType.Informational),
                new SystemVariable("tmemloc", 476, SystemVariableType.Functional),
                new SystemVariable("reffixed", 477, SystemVariableType.Informational),
                new SystemVariable("treffixed", 478, SystemVariableType.Informational),
                new SystemVariable("trefaim", 479, SystemVariableType.Informational| SystemVariableType.Functional),
                new SystemVariable("tieang1", 480, SystemVariableType.Informational| SystemVariableType.Functional),
                new SystemVariable("tieang2", 481, SystemVariableType.Informational| SystemVariableType.Functional),
                new SystemVariable("tieang3", 482, SystemVariableType.Informational| SystemVariableType.Functional),
                new SystemVariable("tieang4", 483, SystemVariableType.Informational| SystemVariableType.Functional),
                new SystemVariable("tielen1", 484, SystemVariableType.Informational| SystemVariableType.Functional),
                new SystemVariable("tielen2", 485, SystemVariableType.Informational| SystemVariableType.Functional),
                new SystemVariable("tielen3", 486, SystemVariableType.Informational| SystemVariableType.Functional),
                new SystemVariable("tielen4", 487, SystemVariableType.Informational| SystemVariableType.Functional),
                new SystemVariable("eye1", 501, SystemVariableType.Informational),
                new SystemVariable("eye2", 502, SystemVariableType.Informational),
                new SystemVariable("eye3", 503, SystemVariableType.Informational),
                new SystemVariable("eye4", 504, SystemVariableType.Informational),
                new SystemVariable("eye5", 505, SystemVariableType.Informational),
                new SystemVariable("eye6", 506, SystemVariableType.Informational),
                new SystemVariable("eye7", 507, SystemVariableType.Informational),
                new SystemVariable("eye8", 508, SystemVariableType.Informational),
                new SystemVariable("eye9", 509, SystemVariableType.Informational),
                new SystemVariable("refmulti", 686, SystemVariableType.Informational),
                new SystemVariable("refshell", 687, SystemVariableType.Informational),
                new SystemVariable("refbody", 688, SystemVariableType.Informational),
                new SystemVariable("refxpos", 689, SystemVariableType.Informational),
                new SystemVariable("refypos", 690, SystemVariableType.Informational),
                new SystemVariable("refvelscalar", 695, SystemVariableType.Informational),
                new SystemVariable("refvelsx", 696, SystemVariableType.Informational),
                new SystemVariable("refveldx", 697, SystemVariableType.Informational),
                new SystemVariable("refveldn", 698, SystemVariableType.Informational),
                new SystemVariable("refvel", 699, SystemVariableType.Informational),
                new SystemVariable("refvelup", 699, SystemVariableType.Informational),
                new SystemVariable("refup", 701, SystemVariableType.Informational),
                new SystemVariable("refdn", 702, SystemVariableType.Informational),
                new SystemVariable("refsx", 703, SystemVariableType.Informational),
                new SystemVariable("refdx", 704, SystemVariableType.Informational),
                new SystemVariable("refaimdx", 705, SystemVariableType.Informational),
                new SystemVariable("refaimsx", 706, SystemVariableType.Informational),
                new SystemVariable("refshoot", 707, SystemVariableType.Informational),
                new SystemVariable("refeye", 708, SystemVariableType.Informational),
                new SystemVariable("refnrg", 709, SystemVariableType.Informational),
                new SystemVariable("refage", 710, SystemVariableType.Informational),
                new SystemVariable("refaim", 711, SystemVariableType.Informational),
                new SystemVariable("reftie", 712, SystemVariableType.Informational),
                new SystemVariable("refpoison", 713, SystemVariableType.Informational),
                new SystemVariable("refvenom", 714, SystemVariableType.Informational),
                new SystemVariable("refkills", 715, SystemVariableType.Informational),
                new SystemVariable("myup", 721, SystemVariableType.Informational),
                new SystemVariable("mydn", 722, SystemVariableType.Informational),
                new SystemVariable("mysx", 723, SystemVariableType.Informational),
                new SystemVariable("mydx", 724, SystemVariableType.Informational),
                new SystemVariable("myaimdx", 725, SystemVariableType.Informational),
                new SystemVariable("myaimsx", 726, SystemVariableType.Informational),
                new SystemVariable("myshoot", 727, SystemVariableType.Informational),
                new SystemVariable("myeye", 728, SystemVariableType.Informational),
                new SystemVariable("myties", 729, SystemVariableType.Informational),
                new SystemVariable("mypoison", 730, SystemVariableType.Informational),
                new SystemVariable("myvenom", 731, SystemVariableType.Informational),
                new SystemVariable("out1", 800, SystemVariableType.Functional),
                new SystemVariable("out2", 801, SystemVariableType.Functional),
                new SystemVariable("out3", 802, SystemVariableType.Functional),
                new SystemVariable("out4", 803, SystemVariableType.Functional),
                new SystemVariable("out5", 804, SystemVariableType.Functional),
                new SystemVariable("out6", 805, SystemVariableType.Functional),
                new SystemVariable("out7", 806, SystemVariableType.Functional),
                new SystemVariable("out8", 807, SystemVariableType.Functional),
                new SystemVariable("out9", 808, SystemVariableType.Functional),
                new SystemVariable("out10", 809, SystemVariableType.Functional),
                new SystemVariable("in1", 810, SystemVariableType.Informational),
                new SystemVariable("in2", 811, SystemVariableType.Informational),
                new SystemVariable("in3", 812, SystemVariableType.Informational),
                new SystemVariable("in4", 813, SystemVariableType.Informational),
                new SystemVariable("in5", 814, SystemVariableType.Informational),
                new SystemVariable("in6", 815, SystemVariableType.Informational),
                new SystemVariable("in7", 816, SystemVariableType.Informational),
                new SystemVariable("in8", 817, SystemVariableType.Informational),
                new SystemVariable("in9", 818, SystemVariableType.Informational),
                new SystemVariable("in10", 819, SystemVariableType.Informational),
                new SystemVariable("mkslime", 820, SystemVariableType.Functional),
                new SystemVariable("slime", 821, SystemVariableType.Informational),
                new SystemVariable("mkshell", 822, SystemVariableType.Functional),
                new SystemVariable("shell", 823, SystemVariableType.Informational),
                new SystemVariable("mkvenom", 824, SystemVariableType.Functional),
                new SystemVariable("strvenom", 824, SystemVariableType.Functional),
                new SystemVariable("venom", 825, SystemVariableType.Informational),
                new SystemVariable("mkpoison", 826, SystemVariableType.Functional),
                new SystemVariable("strpoison", 826, SystemVariableType.Functional),
                new SystemVariable("poison", 827, SystemVariableType.Informational),
                new SystemVariable("waste", 828, SystemVariableType.Informational),
                new SystemVariable("pwaste", 829, SystemVariableType.Informational),
                new SystemVariable("sharenrg", 830, SystemVariableType.Functional),
                new SystemVariable("sharewaste", 831, SystemVariableType.Functional),
                new SystemVariable("shareshell", 832, SystemVariableType.Functional),
                new SystemVariable("shareslime", 833, SystemVariableType.Functional),
                new SystemVariable("ploc", 834, SystemVariableType.Functional),
                new SystemVariable("vloc", 835, SystemVariableType.Functional),
                new SystemVariable("venval", 836, SystemVariableType.Functional),
                new SystemVariable("paralyzed", 837, SystemVariableType.Informational),
                new SystemVariable("poisoned", 838, SystemVariableType.Informational),
                new SystemVariable("backshot", 900, SystemVariableType.Functional),
                new SystemVariable("aimshoot", 901, SystemVariableType.Functional),
                new SystemVariable("eyef", 510, SystemVariableType.Informational),
                new SystemVariable("focuseye", 511, SystemVariableType.Functional),
                new SystemVariable("eye1dir", 521, SystemVariableType.Functional),
                new SystemVariable("eye2dir", 522, SystemVariableType.Functional),
                new SystemVariable("eye3dir", 523, SystemVariableType.Functional),
                new SystemVariable("eye4dir", 524, SystemVariableType.Functional),
                new SystemVariable("eye5dir", 525, SystemVariableType.Functional),
                new SystemVariable("eye6dir", 526, SystemVariableType.Functional),
                new SystemVariable("eye7dir", 527, SystemVariableType.Functional),
                new SystemVariable("eye8dir", 528, SystemVariableType.Functional),
                new SystemVariable("eye9dir", 529, SystemVariableType.Functional),
                new SystemVariable("eye1width", 531, SystemVariableType.Functional),
                new SystemVariable("eye2width", 532, SystemVariableType.Functional),
                new SystemVariable("eye3width", 533, SystemVariableType.Functional),
                new SystemVariable("eye4width", 534, SystemVariableType.Functional),
                new SystemVariable("eye5width", 535, SystemVariableType.Functional),
                new SystemVariable("eye6width", 536, SystemVariableType.Functional),
                new SystemVariable("eye7width", 537, SystemVariableType.Functional),
                new SystemVariable("eye8width", 538, SystemVariableType.Functional),
                new SystemVariable("eye9width", 539, SystemVariableType.Functional),
                new SystemVariable("reftype", 685, SystemVariableType.Informational),
                new SystemVariable("totalbots", 401, SystemVariableType.Informational),
                new SystemVariable("totalmyspecies", 402, SystemVariableType.Informational),
                new SystemVariable("tout1", 410, SystemVariableType.Functional),
                new SystemVariable("tout2", 411, SystemVariableType.Functional),
                new SystemVariable("tout3", 412, SystemVariableType.Functional),
                new SystemVariable("tout4", 413, SystemVariableType.Functional),
                new SystemVariable("tout5", 414, SystemVariableType.Functional),
                new SystemVariable("tout6", 415, SystemVariableType.Functional),
                new SystemVariable("tout7", 416, SystemVariableType.Functional),
                new SystemVariable("tout8", 417, SystemVariableType.Functional),
                new SystemVariable("tout9", 418, SystemVariableType.Functional),
                new SystemVariable("tout10", 419, SystemVariableType.Functional),
                new SystemVariable("tin1", 420, SystemVariableType.Informational),
                new SystemVariable("tin2", 421, SystemVariableType.Informational),
                new SystemVariable("tin3", 422, SystemVariableType.Informational),
                new SystemVariable("tin4", 423, SystemVariableType.Informational),
                new SystemVariable("tin5", 424, SystemVariableType.Informational),
                new SystemVariable("tin6", 425, SystemVariableType.Informational),
                new SystemVariable("tin7", 426, SystemVariableType.Informational),
                new SystemVariable("tin8", 427, SystemVariableType.Informational),
                new SystemVariable("tin9", 428, SystemVariableType.Informational),
                new SystemVariable("tin10", 429, SystemVariableType.Informational),
                new SystemVariable("pval", 839, SystemVariableType.Functional),
                new SystemVariable("chlr", 920, SystemVariableType.Informational),
                new SystemVariable("mkchlr", 921, SystemVariableType.Functional),
                new SystemVariable("rmchlr", 922, SystemVariableType.Functional),
                new SystemVariable("light", 923, SystemVariableType.Informational),
                new SystemVariable("availability", 923, SystemVariableType.Informational),
                new SystemVariable("sharechlr", 924, SystemVariableType.Functional)
            };

            return list.AsReadOnly();
        }

        [NotNull]
        private static string LogicToCommand([NotNull] Block block)
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

        [NotNull]
        private static string MasterFlowToCommand([NotNull] Block block)
        {
            switch (block.Value)
            {
                case MasterFlow.End:
                    return "end";

                default:
                    return "";
            }
        }

        [CanBeNull]
        private static Block ParseSystemVariable([NotNull] string command, bool starReference = false)
        {
            if (command.StartsWith("."))
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

        [NotNull]
        private static string StarVariableToCommand([NotNull] Block block)
        {
            return $"*{VariableToCommand(block)}";
        }

        [NotNull]
        private static string StoreToCommand([NotNull] Block block)
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

        [CanBeNull]
        private static Block TokenizeAdvancedCommand([NotNull] string command)
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

        [CanBeNull]
        private static Block TokenizeBasicCommand([NotNull] string command)
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

        [CanBeNull]
        private static Block TokenizeBitwise([NotNull] string command)
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

        [CanBeNull]
        private static Block TokenizeCondition([NotNull] string command)
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

        [CanBeNull]
        private static Block TokenizeFlow([NotNull] string command)
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

        [CanBeNull]
        private static Block TokenizeLogic([NotNull] string command)
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

        [CanBeNull]
        private static Block TokenizeMasterFlow([NotNull] string command)
        {
            return command.Equals("end", StringComparison.InvariantCultureIgnoreCase) ? new Block(BlockType.MasterFlow, 1) : null;
        }

        [CanBeNull]
        private static Block TokenizeStore([NotNull] string command)
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

        [NotNull]
        private static string VariableToCommand([NotNull] Block block)
        {
            var variable = SystemVariables.Where(v => v.Address == block.Value).OrderBy(v => v.Name.Length).FirstOrDefault();

            return variable == null ? block.Value.ToString() : $".{variable.Name}";
        }
    }
}