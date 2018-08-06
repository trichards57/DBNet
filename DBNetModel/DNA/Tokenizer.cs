using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DBNetModel.DNA
{
    public static class Tokenizer
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

        public static Block ParseCommand([NotNull] string command)
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

            return command.StartsWith("*")
                ? ParseSystemVariable(command.Substring(1), true)
                : ParseSystemVariable(command);
        }

        private static string AdvancedCommandToCommand(Block block)
        {
            switch (block.Value)
            {
                case 1:
                    return "angle";

                case 2:
                    return "dist";

                case 3:
                    return "ceil";

                case 4:
                    return "floor";

                case 5:
                    return "sqr";

                case 6:
                    return "pow";

                case 7:
                    return "pyth";

                case 8:
                    return "anglecmp";

                case 9:
                    return "root";

                case 10:
                    return "logx";

                case 11:
                    return "sin";

                case 12:
                    return "cos";

                case 13:
                    return "debugint";

                case 14:
                    return "debugbool";

                default:
                    return "";
            }
        }

        private static string BasicCommandToCommand(Block block)
        {
            switch (block.Value)
            {
                case 1:
                    return "add";

                case 2:
                    return "sub";

                case 3:
                    return "mult";

                case 4:
                    return "div";

                case 5:
                    return "rnd";

                case 6:
                    return "*";

                case 7:
                    return "mod";

                case 8:
                    return "sgn";

                case 9:
                    return "abs";

                case 10:
                    return "dup";

                case 11:
                    return "drop";

                case 12:
                    return "clear";

                case 13:
                    return "swap";

                case 14:
                    return "over";

                default:
                    return "";
            }
        }

        private static string BitwiseToCommand(Block block)
        {
            switch (block.Value)
            {
                case 1:
                    return "~";

                case 2:
                    return "&";

                case 3:
                    return "|";

                case 4:
                    return "^";

                case 5:
                    return "++";

                case 6:
                    return "--";

                case 7:
                    return "-";

                case 8:
                    return "<<";

                case 9:
                    return ">>";

                default:
                    return "";
            }
        }

        private static string ConditionToCommand(Block block)
        {
            switch (block.Value)
            {
                case 1:
                    return "<";

                case 2:
                    return ">";

                case 3:
                    return "=";

                case 4:
                    return "!=";

                case 5:
                    return "%=";

                case 6:
                    return "!%=";

                case 7:
                    return "~=";

                case 8:
                    return "!~=";

                case 9:
                    return ">=";

                case 10:
                    return "<=";

                default:
                    return "";
            }
        }

        private static string FlowToCommand(Block block)
        {
            switch (block.Value)
            {
                case 1:
                    return "cond";

                case 2:
                    return "start";

                case 3:
                    return "else";

                case 4:
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

        private static string LogicToCommand(Block block)
        {
            switch (block.Value)
            {
                case 1:
                    return "and";

                case 2:
                    return "or";

                case 3:
                    return "xor";

                case 4:
                    return "not";

                case 5:
                    return "true";

                case 6:
                    return "false";

                case 7:
                    return "dropbool";

                case 8:
                    return "clearbool";

                case 9:
                    return "dupbool";

                case 10:
                    return "swapbool";

                case 11:
                    return "overbool";

                default:
                    return "";
            }
        }

        private static string MasterFlowToCommand(Block block)
        {
            switch (block.Value)
            {
                case 1:
                    return "end";

                default:
                    return "";
            }
        }

        private static Block ParseSystemVariable(string command, bool starReference = false)
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

        private static string StarVariableToCommand(Block block)
        {
            return $"*{VariableToCommand(block)}";
        }

        private static string StoreToCommand(Block block)
        {
            switch (block.Value)
            {
                case 1:
                    return "store";

                case 2:
                    return "inc";

                case 3:
                    return "dec";

                case 4:
                    return "addstore";

                case 5:
                    return "substore";

                case 6:
                    return "multstore";

                case 7:
                    return "divstore";

                case 8:
                    return "ceilstore";

                case 9:
                    return "floorstore";

                case 10:
                    return "rndstore";

                case 11:
                    return "sgnstore";

                case 12:
                    return "absstore";

                case 13:
                    return "sqrstore";

                case 14:
                    return "negstore";

                default:
                    return "";
            }
        }

        [CanBeNull]
        private static Block TokenizeAdvancedCommand(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "angle":
                    return new Block(BlockType.AdvancedCommand, 1);

                case "dist":
                    return new Block(BlockType.AdvancedCommand, 2);

                case "ceil":
                    return new Block(BlockType.AdvancedCommand, 3);

                case "floor":
                    return new Block(BlockType.AdvancedCommand, 4);

                case "sqr":
                    return new Block(BlockType.AdvancedCommand, 5);

                case "pow":
                    return new Block(BlockType.AdvancedCommand, 6);

                case "pyth":
                    return new Block(BlockType.AdvancedCommand, 7);

                case "anglecmp":
                    return new Block(BlockType.AdvancedCommand, 8);

                case "root":
                    return new Block(BlockType.AdvancedCommand, 9);

                case "logx":
                    return new Block(BlockType.AdvancedCommand, 10);

                case "sin":
                    return new Block(BlockType.AdvancedCommand, 11);

                case "cos":
                    return new Block(BlockType.AdvancedCommand, 12);

                case "debugint":
                    return new Block(BlockType.AdvancedCommand, 13);

                case "debugbool":
                    return new Block(BlockType.AdvancedCommand, 14);

                default:
                    return null;
            }
        }

        [CanBeNull]
        private static Block TokenizeBasicCommand(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "add":
                    return new Block(BlockType.BasicCommand, 1);

                case "sub":
                    return new Block(BlockType.BasicCommand, 2);

                case "mult":
                    return new Block(BlockType.BasicCommand, 3);

                case "div":
                    return new Block(BlockType.BasicCommand, 4);

                case "rnd":
                    return new Block(BlockType.BasicCommand, 5);

                case "*":
                    return new Block(BlockType.BasicCommand, 6);

                case "mod":
                    return new Block(BlockType.BasicCommand, 7);

                case "sgn":
                    return new Block(BlockType.BasicCommand, 8);

                case "abs":
                    return new Block(BlockType.BasicCommand, 9);

                case "dup":
                case "dupint":
                    return new Block(BlockType.BasicCommand, 10);

                case "drop":
                case "dropint":
                    return new Block(BlockType.BasicCommand, 11);

                case "clear":
                case "clearint":
                    return new Block(BlockType.BasicCommand, 12);

                case "swap":
                case "swapint":
                    return new Block(BlockType.BasicCommand, 13);

                case "over":
                case "overint":
                    return new Block(BlockType.BasicCommand, 14);

                default:
                    return null;
            }
        }

        [CanBeNull]
        private static Block TokenizeBitwise(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "~":
                    return new Block(BlockType.Bitwise, 1);

                case "&":
                    return new Block(BlockType.Bitwise, 2);

                case "|":
                    return new Block(BlockType.Bitwise, 3);

                case "^":
                    return new Block(BlockType.Bitwise, 4);

                case "++":
                    return new Block(BlockType.Bitwise, 5);

                case "--":
                    return new Block(BlockType.Bitwise, 6);

                case "-":
                    return new Block(BlockType.Bitwise, 7);

                case "<<":
                    return new Block(BlockType.Bitwise, 8);

                case ">>":
                    return new Block(BlockType.Bitwise, 9);

                default:
                    return null;
            }
        }

        [CanBeNull]
        private static Block TokenizeCondition(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "<":
                    return new Block(BlockType.Condition, 1);

                case ">":
                    return new Block(BlockType.Condition, 2);

                case "=":
                    return new Block(BlockType.Condition, 3);

                case "!=":
                    return new Block(BlockType.Condition, 4);

                case "%=":
                    return new Block(BlockType.Condition, 5);

                case "!%=":
                    return new Block(BlockType.Condition, 6);

                case "~=":
                    return new Block(BlockType.Condition, 7);

                case "!~=":
                    return new Block(BlockType.Condition, 8);

                case ">=":
                    return new Block(BlockType.Condition, 9);

                case "<=":
                    return new Block(BlockType.Condition, 10);

                default:
                    return null;
            }
        }

        [CanBeNull]
        private static Block TokenizeFlow(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "cond":
                    return new Block(BlockType.Flow, 1);

                case "start":
                    return new Block(BlockType.Flow, 2);

                case "else":
                    return new Block(BlockType.Flow, 3);

                case "stop":
                    return new Block(BlockType.Flow, 4);

                default:
                    return null;
            }
        }

        [CanBeNull]
        private static Block TokenizeLogic(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "and":
                    return new Block(BlockType.Logic, 1);

                case "or":
                    return new Block(BlockType.Logic, 2);

                case "xor":
                    return new Block(BlockType.Logic, 3);

                case "not":
                    return new Block(BlockType.Logic, 4);

                case "true":
                    return new Block(BlockType.Logic, 5);

                case "false":
                    return new Block(BlockType.Logic, 6);

                case "dropbool":
                    return new Block(BlockType.Logic, 7);

                case "clearbool":
                    return new Block(BlockType.Logic, 8);

                case "dupbool":
                    return new Block(BlockType.Logic, 9);

                case "swapbool":
                    return new Block(BlockType.Logic, 10);

                case "overbool":
                    return new Block(BlockType.Logic, 11);

                default:
                    return null;
            }
        }

        [CanBeNull]
        private static Block TokenizeMasterFlow(string command)
        {
            return command.Equals("end", StringComparison.InvariantCultureIgnoreCase) ? new Block(BlockType.MasterFlow, 1) : null;
        }

        [CanBeNull]
        private static Block TokenizeStore(string command)
        {
            command = command.ToLowerInvariant();

            switch (command)
            {
                case "store":
                    return new Block(BlockType.Stores, 1);

                case "inc":
                    return new Block(BlockType.Stores, 2);

                case "dec":
                    return new Block(BlockType.Stores, 3);

                case "addstore":
                    return new Block(BlockType.Stores, 4);

                case "substore":
                    return new Block(BlockType.Stores, 5);

                case "multstore":
                    return new Block(BlockType.Stores, 6);

                case "divstore":
                    return new Block(BlockType.Stores, 7);

                case "ceilstore":
                    return new Block(BlockType.Stores, 8);

                case "floorstore":
                    return new Block(BlockType.Stores, 9);

                case "rndstore":
                    return new Block(BlockType.Stores, 10);

                case "sgnstore":
                    return new Block(BlockType.Stores, 11);

                case "absstore":
                    return new Block(BlockType.Stores, 12);

                case "sqrstore":
                    return new Block(BlockType.Stores, 13);

                case "negstore":
                    return new Block(BlockType.Stores, 14);

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