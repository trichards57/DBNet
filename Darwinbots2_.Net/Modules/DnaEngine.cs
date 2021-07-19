using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarwinBots.Modules
{
    internal class DnaEngine
    {
        public const int MaxIntValue = 2000000000;
        private const bool NextBody = true;
        private const bool NextElse = false;
        private readonly SafeStack<bool> _boolStack = new() { DefaultValue = true };
        private readonly Costs _costs;
        private readonly SafeStack<int> _intStack = new() { DefaultValue = 0 };
        private bool _currentCondFlag;
        private FlowState _currentFlow = FlowState.Clear;
        private int _currentGene;
        private bool _inGene;

        private DnaEngine(Costs costs)
        {
            _costs = costs;
        }

        public static IReadOnlyList<Variable> SystemVariables { get; private set; } = new List<Variable>();

        public static void ExecRobs(Costs costs, IEnumerable<robot> robs)
        {
            foreach (var rob in robs.Where(r => r.exist && !r.Corpse && !r.DisableDNA))
            {
                var engine = new DnaEngine(costs);
                engine.ExecuteDna(rob);
            }
        }

        public static void LoadSysVars()
        {
            SystemVariables = new List<Variable>
            {
                new("up", 1, functional: true),
                new("dn", 2, functional: true),
                new("sx", 3, functional: true),
                new("dx", 4, functional: true),
                new("aimright", 5, functional: true, synonym: "aimdx"),
                new("aimleft", 6, functional: true, synonym: "aimsx"),
                new("shoot", 7, functional: true),
                new("shootval", 8, functional: true),
                new("robage", 9, true),
                new("mass", 10, true),
                new("maxvel", 11, true),
                new("timer", 12, true),
                new("aim", 18, true),
                new("setaim", 19, functional: true),
                new("bodgain", 194, true),
                new("bodloss", 195, true),
                new("velscalar", 196, true),
                new("velsx", 197, true),
                new("veldx", 198, true),
                new("veldn", 199, true),
                new("velup", 200, true),
                new("vel", 200, true),
                new("hit", 201, true),
                new("shflav", 202, true),
                new("pain", 203, true),
                new("pleas", 204, true),
                new("hitup", 205, true),
                new("hitdn", 206, true),
                new("hitdx", 207, true),
                new("hitsx", 208, true),
                new("shang", 209, true),
                new("shup", 210, true),
                new("shdn", 211, true),
                new("shdx", 212, true),
                new("shsx", 213, true),
                new("edge", 214, true),
                new("fixed", 215, true),
                new("fixpos", 216, functional: true),
                new("ypos", 217, true, synonym: "depth"),
                new("daytime", 218, true),
                new("xpos", 219, true),
                new("kills", 220, true),
                new("hitang", 221),
                new("repro", 300, functional: true),
                new("mrepro", 301, functional: true),
                new("sexrepro", 302, functional: true),
                new("fertilized", 303, true),
                new("nrg", 310, true),
                new("body", 311, true),
                new("fdbody", 312, functional: true),
                new("strbody", 313, functional: true),
                new("setboy", 314, functional: true),
                new("rdboy", 315, true),
                new("tie", 330, functional: true),
                new("stifftie", 331, functional: true),
                new("mkvirus", 335, functional: true),
                new("dnalen", 336, true),
                new("vtimer", 337, true),
                new("vshoot", 338, functional: true),
                new("genes", 339, true),
                new("delgene", 340, functional: true),
                new("thisgene", 341, true),
                new("sun", 400, true),
                new("totalbots", 401, true),
                new("totalmyspecies", 402, true),
                new("tout1", 410, functional: true),
                new("tout2", 411, functional: true),
                new("tout3", 412, functional: true),
                new("tout4", 413, functional: true),
                new("tout5", 414, functional: true),
                new("tout6", 415, functional: true),
                new("tout7", 416, functional: true),
                new("tout8", 417, functional: true),
                new("tout9", 418, functional: true),
                new("tout10", 419, functional: true),
                new("tin1", 420, true),
                new("tin2", 421, true),
                new("tin3", 422, true),
                new("tin4", 423, true),
                new("tin5", 424, true),
                new("tin6", 425, true),
                new("tin7", 426, true),
                new("tin8", 427, true),
                new("tin9", 428, true),
                new("tin10", 429, true),
                new("trefbody", 437, true),
                new("trefxpos", 438, true),
                new("trefypos", 439, true),
                new("trefvelmysx", 440, true),
                new("trefvelmydx", 441, true),
                new("trefvelmydn", 442, true),
                new("trefvelmyup", 443, true),
                new("trefvelscalar", 444, true),
                new("trefvelyoursx", 445, true),
                new("trefvelyourdx", 446, true),
                new("trefvelyourdn", 447, true),
                new("trefvelyourup", 448, true),
                new("trefshell", 449, true),
                new("tieang", 450, true),
                new("tielen", 451, true),
                new("tieloc", 452, functional: true),
                new("tieval", 453, functional: true),
                new("tiepres", 454, true),
                new("tienum", 455, functional: true),
                new("trefup", 456, true),
                new("trefdn", 457, true),
                new("trefsx", 458, true),
                new("trefdx", 459, true),
                new("trefaimdx", 460, true),
                new("trefaimsx", 461, true),
                new("trefshoot", 462, true),
                new("trefeye", 463, true),
                new("trefnrg", 464, true),
                new("trefage", 465, true),
                new("numties", 466, true),
                new("deltie", 467, functional: true),
                new("fixang", 468, functional: true),
                new("fixlen", 469, functional: true),
                new("multi", 470, true),
                new("readtie", 471, functional: true),
                new("memval", 473, true),
                new("memloc", 474, functional: true),
                new("tmemval", 475, true),
                new("tmemloc", 476, functional: true),
                new("reffixed", 477, true),
                new("treffixed", 478, true),
                new("trefaim", 479, true),
                new("tieang1", 480, true, true),
                new("tieang2", 481, true, true),
                new("tieang3", 482, true, true),
                new("tieang4", 483, true, true),
                new("tielen1", 484, true, true),
                new("tielen2", 485, true, true),
                new("tielen3", 486, true, true),
                new("tielen4", 487, true, true),
                new("eye1", 501, true),
                new("eye2", 502, true),
                new("eye3", 503, true),
                new("eye4", 504, true),
                new("eye5", 505, true),
                new("eye6", 506, true),
                new("eye7", 507, true),
                new("eye8", 508, true),
                new("eye9", 509, true),
                new("eyef", 510, true),
                new("focuseye", 511, functional: true),
                new("eye1dir", 521, functional: true),
                new("eye2dir", 522, functional: true),
                new("eye3dir", 523, functional: true),
                new("eye4dir", 524, functional: true),
                new("eye5dir", 525, functional: true),
                new("eye6dir", 526, functional: true),
                new("eye7dir", 527, functional: true),
                new("eye8dir", 528, functional: true),
                new("eye9dir", 529, functional: true),
                new("eye1width", 531, functional: true),
                new("eye2width", 532, functional: true),
                new("eye3width", 533, functional: true),
                new("eye4width", 534, functional: true),
                new("eye5width", 535, functional: true),
                new("eye6width", 536, functional: true),
                new("eye7width", 537, functional: true),
                new("eye8width", 538, functional: true),
                new("eye9width", 539, functional: true),
                new("reftype", 685, true),
                new("refmulti", 686, true),
                new("refshell", 687, true),
                new("refbody", 688, true),
                new("refxpos", 689, true),
                new("refypos", 690, true),
                new("refvelscalar", 695, true),
                new("refvelsx", 696, true),
                new("refveldx", 697, true),
                new("refveldn", 698, true),
                new("refvelup", 699, true, synonym: "refvel"),
                new("refup", 701, true),
                new("refdn", 702, true),
                new("refsx", 703, true),
                new("refdx", 704, true),
                new("refaimdx", 705, true),
                new("refaimsx", 706, true),
                new("refshoot", 707, true),
                new("refeye", 708, true),
                new("refnrg", 709, true),
                new("refage", 710, true),
                new("refaim", 711, true),
                new("reftie", 712, true),
                new("refpoison", 713, true),
                new("refvenom", 714, true),
                new("refkills", 715, true),
                new("myup", 721, true),
                new("mydn", 722, true),
                new("mysx", 723, true),
                new("mydx", 724, true),
                new("myaimdx", 725, true),
                new("myaimsx", 726, true),
                new("myshoot", 727, true),
                new("myeye", 728, true),
                new("myties", 729, true),
                new("mypoison", 730, true),
                new("myvenom", 731, true),
                new("out1", 800, functional: true),
                new("out2", 801, functional: true),
                new("out3", 802, functional: true),
                new("out4", 803, functional: true),
                new("out5", 804, functional: true),
                new("out6", 805, functional: true),
                new("out7", 806, functional: true),
                new("out8", 807, functional: true),
                new("out9", 808, functional: true),
                new("out10", 809, functional: true),
                new("in1", 810, true),
                new("in2", 811, true),
                new("in3", 812, true),
                new("in4", 813, true),
                new("in5", 814, true),
                new("in6", 815, true),
                new("in7", 816, true),
                new("in8", 817, true),
                new("in9", 818, true),
                new("in10", 819, true),
                new("mkslime", 820, functional: true),
                new("slime", 821, true),
                new("mkshell", 822, functional: true),
                new("shell", 823, true),
                new("mkvenom", 824, functional: true, synonym: "strvenom"),
                new("venom", 825, true),
                new("mkpoison", 826, functional: true, synonym: "strpoison"),
                new("poison", 827, true),
                new("waste", 828, true),
                new("pwaste", 829, true),
                new("sharenrg", 830, functional: true),
                new("sharewaste", 831, functional: true),
                new("shareshell", 832, functional: true),
                new("shareslime", 833, functional: true),
                new("ploc", 834, functional: true),
                new("vloc", 835, functional: true),
                new("venval", 836, functional: true),
                new("paralyzed", 837, true),
                new("poisoned", 838, true),
                new("pval", 839, functional: true),
                new("backshot", 900, functional: true),
                new("aimshoot", 901, functional: true),
                new("chlr", 920, true),
                new("mkchlr", 921, functional: true),
                new("rmchlr", 922, functional: true),
                new("light", 923, true),
                new("availability", 923, true),
                new("sharechlr", 924, functional: true)
            };
        }

        private static void CheckTieAngTieLenAddress(robot rob, int address)
        {
            for (var k = 0; k < 4; k++)
            {
                if (address == 480 + k)
                    rob.TieAngOverwrite[k] = true;
                if (address == 484 + k)
                    rob.TieLenOverwrite[k] = true;
            }
        }

        private static int Clamp(int val, int max = MaxIntValue)
        {
            return Math.Clamp(val, max, -max);
        }

        private static int Mod32000(int a)
        {
            if (a > 0)
            {
                a %= 32000;
                if (a == 0)
                    a = 32000;
            }
            else if (a < 0)
            {
                a %= 32000;
                if (a == 0)
                    a = -32000;
            }

            return a;
        }

        private static int NormaliseMemoryAddress(int a)
        {
            a = Math.Abs(a) % RobotsManager.MaxMem;
            if (a == 0)
                a = 1000;

            return a;
        }

        private bool AddupCond()
        {
            if (_boolStack.Count == 0)
                return true;

            var res = true;

            while (_boolStack.Count > 0)
                res &= _boolStack.Pop();

            return res;
        }

        private void ExecuteAdvancedCommand(robot rob, int n)
        {
            if (n is < 1 or > 12)
                return;

            int a, b, e;
            double c, d;

            rob.nrg -= _costs.AdvancedCommandCost * _costs.CostMultiplier;

            switch (n)
            {
                case 1: // findang
                    b = _intStack.Pop();
                    a = _intStack.Pop();
                    c = rob.pos.X;
                    d = rob.pos.Y;
                    e = (int)Physics.NormaliseAngle(Physics.Angle(c, d, a, b)) * 200;
                    _intStack.Push(e);
                    break;

                case 2: // finddist
                    b = _intStack.Pop();
                    a = _intStack.Pop();
                    c = rob.pos.X;
                    d = rob.pos.Y;
                    e = Clamp((int)Math.Sqrt(Math.Pow(c - a, 2) + Math.Pow(d - b, 2)));
                    _intStack.Push(e);
                    break;

                case 3: // ceil
                    b = _intStack.Pop() % MaxIntValue;
                    a = _intStack.Pop() % MaxIntValue;
                    _intStack.Push(Math.Min(a, b));
                    break;

                case 4: // floor
                    b = _intStack.Pop() % MaxIntValue;
                    a = _intStack.Pop() % MaxIntValue;
                    _intStack.Push(Math.Max(a, b));
                    break;

                case 5: // sqr
                    a = _intStack.Pop() % MaxIntValue;
                    _intStack.Push(a > 0 ? (int)Math.Sqrt(a) : 0);
                    break;

                case 6: // power
                    b = _intStack.Pop() % MaxIntValue;
                    a = _intStack.Pop() % MaxIntValue;
                    _intStack.Push(Clamp((int)Math.Pow(a, b)));
                    break;

                case 7: // pyth
                    b = _intStack.Pop() % MaxIntValue;
                    a = _intStack.Pop() % MaxIntValue;
                    _intStack.Push(Clamp((int)Math.Sqrt(a * a + b * b)));
                    break;

                case 8: // cmp
                    b = _intStack.Pop();
                    a = _intStack.Pop();

                    b %= 1256;
                    if (b < 0)
                        b += 1256;

                    a %= 1256;
                    if (a < 0)
                        a += 1256;

                    c = Physics.RadiansToInt(Physics.AngDiff(Physics.IntToRadians(a), Physics.IntToRadians(b)));

                    _intStack.Push((int)c);
                    break;

                case 9:// a ^ (1/b)
                    b = Math.Abs(_intStack.Pop());
                    a = Math.Abs(_intStack.Pop());
                    _intStack.Push(b == 0 ? 0 : (int)Math.Pow(a, 1.0 / b));
                    break;

                case 10: // log(a) / Log(b)
                    b = Math.Abs(_intStack.Pop());
                    a = Math.Abs(_intStack.Pop());
                    c = Math.Log(a, b);

                    if (double.IsNaN(c) && double.IsInfinity(c))
                        c = 0;

                    _intStack.Push((int)c);
                    break;

                case 11:
                    _intStack.Push((int)(Math.Sin(Physics.IntToRadians(_intStack.Pop())) * 32000));
                    break;

                case 12:
                    _intStack.Push((int)(Math.Cos(Physics.IntToRadians(_intStack.Pop())) * 32000));
                    break;
            }
        }

        private void ExecuteBasicCommand(robot rob, int n)
        {
            if (n < 1 || n > 14)
                return;

            int a, b;

            rob.nrg -= _costs.BasicCommandCost * _costs.CostMultiplier;

            switch (n)
            {
                case 1: // add
                    b = _intStack.Pop() % MaxIntValue;
                    a = _intStack.Pop() % MaxIntValue;
                    _intStack.Push(Clamp(a + b));
                    break;

                case 2: // sub
                    b = _intStack.Pop() % MaxIntValue;
                    a = _intStack.Pop() % MaxIntValue;
                    _intStack.Push(Clamp(a - b));
                    break;

                case 3: // mult
                    b = _intStack.Pop() % MaxIntValue;
                    a = _intStack.Pop() % MaxIntValue;
                    _intStack.Push(Clamp(a * b));
                    break;

                case 4: // div
                    b = _intStack.Pop() % MaxIntValue;
                    a = _intStack.Pop() % MaxIntValue;
                    _intStack.Push(b == 0 ? 0 : a / b);
                    break;

                case 5: // rnd
                    a = _intStack.Pop() % MaxIntValue;
                    _intStack.Push(ThreadSafeRandom.Local.Next(0, a));
                    break;

                case 6: // dereference AKA *
                    a = NormaliseMemoryAddress(_intStack.Pop());
                    _intStack.Push(rob.mem[a]);
                    break;

                case 7: // mod
                    b = _intStack.Pop() % MaxIntValue;
                    a = _intStack.Pop() % MaxIntValue;
                    _intStack.Push(b == 0 ? 0 : a % b);
                    break;

                case 8: // sgn
                    a = _intStack.Pop();
                    _intStack.Push(Math.Sign(a));
                    break;

                case 9: // absolute value
                    _intStack.Push(Math.Abs(_intStack.Pop()));
                    break;

                case 10: // dup or dupint
                    _intStack.Push(_intStack.Peek());
                    break;

                case 11: // dropint - Drops the top value on the Int stack
                    _intStack.Pop();
                    break;

                case 12: // clearint - Clears the Int stack
                    _intStack.Clear();
                    break;

                case 13: // swapint - Swaps the top two values on the Int stack
                    _intStack.Swap();
                    break;

                case 14: // overint - a b -> a b a  Dups the second value on the Int stack
                    _intStack.Over();
                    break;
            }
        }

        private void ExecuteBitwiseCommand(robot rob, int n)
        {
            if (n < 1 || n > 9)
                return;

            int a, b;

            rob.nrg -= _costs.BitwiseCommandCost * _costs.CostMultiplier;

            switch (n)
            {
                case 1: // Compliment ~ (tilde)
                    a = _intStack.Pop();
                    _intStack.Push(~a);
                    break;

                case 2: // & And
                    b = _intStack.Pop();
                    a = _intStack.Pop();
                    _intStack.Push(a & b);
                    break;

                case 3: // | or
                    b = _intStack.Pop();
                    a = _intStack.Pop();
                    _intStack.Push(a | b);
                    break;

                case 4: // XOR ^
                    b = _intStack.Pop();
                    a = _intStack.Pop();
                    _intStack.Push(a ^ b);
                    break;

                case 5: // bitinc ++
                    a = _intStack.Pop();
                    _intStack.Push(a + 1);
                    break;

                case 6: // bitdec --
                    a = _intStack.Pop();
                    _intStack.Push(a - 1);
                    break;

                case 7: // negate
                    a = _intStack.Pop();
                    _intStack.Push(-a);
                    break;

                case 8: // <<
                    a = _intStack.Pop();
                    _intStack.Push(a << 1);
                    break;

                case 9: // >>
                    a = _intStack.Pop();
                    _intStack.Push(a >> 1);
                    break;
            }
        }

        private void ExecuteConditions(robot rob, int n)
        {
            if (n < 1 || n > 10)
                return;

            int a, b, c, d;

            rob.nrg -= _costs.ConditionCost * _costs.CostMultiplier;

            switch (n)
            {
                case 1:  // <
                    // ReSharper disable once EqualExpressionComparison
                    _boolStack.Push(_intStack.Pop() > _intStack.Pop());
                    break;

                case 2: // >
                    // ReSharper disable once EqualExpressionComparison
                    _boolStack.Push(_intStack.Pop() < _intStack.Pop());
                    break;

                case 3: // =
                    // ReSharper disable once EqualExpressionComparison
                    _boolStack.Push(_intStack.Pop() == _intStack.Pop());
                    break;

                case 4: // <>, !=
                    // ReSharper disable once EqualExpressionComparison
                    _boolStack.Push(_intStack.Pop() != _intStack.Pop());
                    break;

                case 5: // %=
                    b = _intStack.Pop();
                    a = _intStack.Pop();
                    c = a / 10;
                    _boolStack.Push(!(a + c < b || a - c > b));
                    break;

                case 6: // !%=
                    b = _intStack.Pop();
                    a = _intStack.Pop();
                    c = a / 10;
                    _boolStack.Push(a + c < b || a - c > b);
                    break;

                case 7: // ~=
                    d = _intStack.Pop();
                    b = _intStack.Pop();
                    a = _intStack.Pop();
                    c = a / 100 * d;
                    _boolStack.Push(!(a + c < b || a - c > b));
                    break;

                case 8: // !~=
                    d = _intStack.Pop();
                    b = _intStack.Pop();
                    a = _intStack.Pop();
                    c = a * d / 100;
                    _boolStack.Push(a + c < b || a - c > b);
                    break;

                case 9: // >=
                    // ReSharper disable once EqualExpressionComparison
                    _boolStack.Push(_intStack.Pop() <= _intStack.Pop());
                    break;

                case 10: // <=
                    // ReSharper disable once EqualExpressionComparison
                    _boolStack.Push(_intStack.Pop() >= _intStack.Pop());
                    break;
            }
        }

        private void ExecuteDna(robot rob)
        {
            _currentGene = 0;
            _currentCondFlag = NextBody;
            _inGene = false;
            _intStack.Clear();
            _boolStack.Clear();

            rob.condnum = 0;

            foreach (var block in rob.dna)
            {
                if (block.Type == 10 && block.Value == 1)
                    break;

                switch (block.Type)
                {
                    case 0:
                        if (_currentFlow == FlowState.Clear)
                            break;

                        _intStack.Push(block.Value);
                        rob.nrg -= _costs.NumberCost * _costs.CostMultiplier;
                        break;

                    case 1:
                        if (_currentFlow == FlowState.Clear)
                            break;

                        var b = NormaliseMemoryAddress(block.Value);
                        _intStack.Push(rob.mem[b]);
                        rob.nrg -= _costs.DotNumberCost * _costs.CostMultiplier;
                        break;

                    case 2:
                        if (_currentFlow != FlowState.Clear)
                            ExecuteBasicCommand(rob, block.Value);
                        break;

                    case 3:
                        if (_currentFlow != FlowState.Clear)
                            ExecuteAdvancedCommand(rob, block.Value);
                        break;

                    case 4:
                        if (_currentFlow != FlowState.Clear)
                            ExecuteBitwiseCommand(rob, block.Value);
                        break;

                    case 5:
                        if (_currentFlow == FlowState.Condition || _currentFlow == FlowState.Body || _currentFlow == FlowState.ElseBody)
                            ExecuteConditions(rob, block.Value);
                        break;

                    case 6:
                        if (_currentFlow == FlowState.Condition || _currentFlow == FlowState.Body || _currentFlow == FlowState.ElseBody)
                            ExecuteLogic(rob, block.Value);
                        break;

                    case 7:
                        if ((_currentFlow == FlowState.Body || _currentFlow == FlowState.ElseBody) && _boolStack.Peek())
                            ExecuteStores(rob, block.Value);
                        break;

                    case 9:
                        if (!ExecuteFlowCommands(rob, block.Value))
                            rob.condnum++;
                        rob.mem[MemoryAddresses.thisgene] = _currentGene;
                        break;
                }
            }

            _currentFlow = FlowState.Clear; // EricL 4/15/2006 Do this so next bot doesn't inherit the flow control
        }

        private bool ExecuteFlowCommands(robot rob, int n)
        {
            rob.nrg -= _costs.FlowCommandCost * _costs.CostMultiplier;

            var condFound = false;

            switch (n)
            {
                case 1:
                    _currentFlow = FlowState.Condition;
                    _currentGene++;
                    _boolStack.Clear();
                    _inGene = true;
                    break;

                case 2:
                case 3:
                case 4:
                    //this is supposed to come before case 2 and 3, since these commands
                    //must be executed before start and else have a chance to go
                    condFound = true;
                    if (_currentFlow == FlowState.Condition)
                        _currentCondFlag = AddupCond();

                    if (!_inGene)
                        _currentCondFlag = NextBody;

                    _currentFlow = FlowState.Clear;
                    switch (n)
                    {
                        case 2:
                            if (!_inGene)
                                _currentGene++;
                            _inGene = false;
                            if (_currentCondFlag == NextBody)
                                _currentFlow = FlowState.Body;
                            break;

                        case 3:
                            if (_currentCondFlag == NextElse)
                                _currentFlow = FlowState.ElseBody;
                            if (!_inGene)
                                _currentGene++;
                            _inGene = false;
                            break;

                        case 4:
                            _inGene = false;
                            _currentFlow = FlowState.Clear;
                            break;
                    }
                    break;
            }
            return condFound;
        }

        private void ExecuteLogic(robot rob, int n)
        {
            rob.nrg -= _costs.LogicCost * _costs.CostMultiplier;

            bool a, b;

            switch (n)
            {
                case 1:
                    b = _boolStack.Pop();
                    a = _boolStack.Pop();
                    _boolStack.Push(a && b);
                    break;

                case 2:
                    b = _boolStack.Pop();
                    a = _boolStack.Pop();
                    _boolStack.Push(a || b);
                    break;

                case 3:
                    b = _boolStack.Pop();
                    a = _boolStack.Pop();
                    _boolStack.Push(a ^ b);
                    break;

                case 4:
                    b = _boolStack.Pop();
                    _boolStack.Push(!b);
                    break;

                case 5:
                    _boolStack.Push(true);
                    break;

                case 6:
                    _boolStack.Push(false);
                    break;

                case 7:
                    _boolStack.Pop();
                    break;

                case 8:
                    _boolStack.Clear();
                    break;

                case 9:
                    if (_boolStack.Count == 0)
                        return;

                    _boolStack.Push(_boolStack.Peek());
                    break;

                case 10:
                    _boolStack.Swap();
                    break;

                case 11:
                    _boolStack.Over();
                    break;
            }
        }

        private void ExecuteStores(robot rob, int n)
        {
            if (n < 1 || n > 14)
                return;

            var b = _intStack.Pop();

            if (b == 0)
                return;

            b = NormaliseMemoryAddress(b);
            CheckTieAngTieLenAddress(rob, b);

            int a;

            switch (n)
            {
                case 1:
                    rob.mem[b] = Mod32000(_intStack.Pop());
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier;
                    break;

                case 2:
                    rob.mem[b] = Mod32000(rob.mem[b] + 1);
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier / 10;
                    break;

                case 3:
                    rob.mem[b] = Mod32000(rob.mem[b] - 1);
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier / 10;
                    break;

                case 4:
                    a = _intStack.Pop();
                    rob.mem[b] = Mod32000(rob.mem[b] + a);
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier / 5;
                    break;

                case 5:
                    a = _intStack.Pop();
                    rob.mem[b] = Mod32000(rob.mem[b] - a);
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier / 5;
                    break;

                case 6:
                    a = _intStack.Pop();
                    rob.mem[b] = Mod32000(rob.mem[b] * Mod32000(a));
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier / 5;
                    break;

                case 7:
                    a = _intStack.Pop();
                    rob.mem[b] = a == 0 ? 0 : rob.mem[b] / a;
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier / 5;
                    break;

                case 8:
                    a = _intStack.Pop();
                    rob.mem[b] = Mod32000(Math.Min(rob.mem[b], a));
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier / 5;
                    break;

                case 9:
                    a = _intStack.Pop();
                    rob.mem[b] = Mod32000(Math.Max(rob.mem[b], a));
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier / 5;
                    break;

                case 10:
                    rob.mem[b] = ThreadSafeRandom.Local.Next(0, Math.Abs(rob.mem[b])) * Math.Sign(rob.mem[b]);
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier / 7;
                    break;

                case 11:
                    rob.mem[b] = Math.Sign(rob.mem[b]);
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier / 7;
                    break;

                case 12:
                    rob.mem[b] = Math.Abs(rob.mem[b]);
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier / 8;
                    break;

                case 13:
                    rob.mem[b] = rob.mem[b] > 0 ? (int)Math.Sqrt(rob.mem[b]) : 0;
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier / 7;
                    break;

                case 14:
                    rob.mem[b] = -rob.mem[b];
                    rob.nrg -= _costs.StoresCost * _costs.CostMultiplier / 8;
                    break;
            }
        }
    }
}
