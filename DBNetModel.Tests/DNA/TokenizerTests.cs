using DBNetModel.DNA;
using DBNetModel.DNA.Commands;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DBNetModel.Tests.DNA
{
    public class TokenizerTests
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly IEnumerable<object[]> Pairings = new[]
        {
             new object [] {"end", BlockType.MasterFlow, MasterFlow.End},
             new object [] {"cond", BlockType.Flow, Flow.Condition},
             new object [] {"start", BlockType.Flow, Flow.Start},
             new object [] {"else", BlockType.Flow, Flow.Else},
             new object [] {"stop", BlockType.Flow, Flow.Stop},
             new object [] {"store", BlockType.Stores, Stores.Store},
             new object [] {"inc", BlockType.Stores, Stores.Increment},
             new object [] {"dec", BlockType.Stores, Stores.Decrement},
             new object [] {"addstore", BlockType.Stores, Stores.AddStore},
             new object [] {"substore", BlockType.Stores, Stores.SubtractStore},
             new object [] {"multstore", BlockType.Stores, Stores.MultiplyStore},
             new object [] {"divstore", BlockType.Stores, Stores.DivideStore},
             new object [] {"ceilstore", BlockType.Stores, Stores.CeilingStore},
             new object [] {"floorstore", BlockType.Stores, Stores.FloorStore},
             new object [] {"rndstore", BlockType.Stores, Stores.RoundStore},
             new object [] {"sgnstore", BlockType.Stores, Stores.SignStore},
             new object [] {"absstore", BlockType.Stores, Stores.AbsoluteStore},
             new object [] {"sqrstore", BlockType.Stores, Stores.SquareStore},
             new object [] {"negstore", BlockType.Stores, Stores.NegateStore},
             new object [] {"and", BlockType.Logic, Logic.And},
             new object [] {"or", BlockType.Logic, Logic.Or},
             new object [] {"xor", BlockType.Logic, Logic.XOr},
             new object [] {"not", BlockType.Logic, Logic.Not},
             new object [] {"true", BlockType.Logic, Logic.True},
             new object [] {"false", BlockType.Logic, Logic.False},
             new object [] {"dropbool", BlockType.Logic, Logic.DropBool},
             new object [] {"clearbool", BlockType.Logic, Logic.ClearBool},
             new object [] {"dupbool", BlockType.Logic, Logic.DuplicateBool},
             new object [] {"swapbool", BlockType.Logic, Logic.SwapBool},
             new object [] {"overbool", BlockType.Logic, Logic.OverBool},
             new object [] {"<", BlockType.Condition, Condition.LessThan},
             new object [] {">", BlockType.Condition, Condition.GreaterThan},
             new object [] {"=", BlockType.Condition, Condition.Equal},
             new object [] {"!=", BlockType.Condition, Condition.NotEqual},
             new object [] {"%=", BlockType.Condition, Condition.CloselyEquals},
             new object [] {"!%=", BlockType.Condition, Condition.NotCloselyEquals},
             new object [] {"~=", BlockType.Condition, Condition.CustomCloselyEquals},
             new object [] {"!~=", BlockType.Condition, Condition.CustomNotCloselyEquals},
             new object [] {">=", BlockType.Condition, Condition.GreaterThanOrEquals},
             new object [] {"<=", BlockType.Condition, Condition.LessThanOrEquals},
             new object [] {"~", BlockType.Bitwise, BitwiseCommands.Complement},
             new object [] {"&", BlockType.Bitwise, BitwiseCommands.And},
             new object [] {"|", BlockType.Bitwise, BitwiseCommands.Or},
             new object [] {"^", BlockType.Bitwise, BitwiseCommands.XOr},
             new object [] {"++", BlockType.Bitwise, BitwiseCommands.Increment},
             new object [] {"--", BlockType.Bitwise, BitwiseCommands.Decrement},
             new object [] {"-", BlockType.Bitwise, BitwiseCommands.Negate},
             new object [] {"<<", BlockType.Bitwise, BitwiseCommands.ShiftLeft},
             new object [] {">>", BlockType.Bitwise, BitwiseCommands.ShiftRight},
             new object [] {"angle", BlockType.AdvancedCommand, AdvancedCommands.Angle},
             new object [] {"dist", BlockType.AdvancedCommand, AdvancedCommands.Distance},
             new object [] {"ceil", BlockType.AdvancedCommand, AdvancedCommands.Ceiling},
             new object [] {"floor", BlockType.AdvancedCommand, AdvancedCommands.Floor},
             new object [] {"sqr", BlockType.AdvancedCommand, AdvancedCommands.Square},
             new object [] {"pow", BlockType.AdvancedCommand, AdvancedCommands.Pow},
             new object [] {"pyth", BlockType.AdvancedCommand, AdvancedCommands.Pythagorus},
             new object [] {"anglecmp", BlockType.AdvancedCommand, AdvancedCommands.AngleCompare},
             new object [] {"root", BlockType.AdvancedCommand, AdvancedCommands.Root},
             new object [] {"logx", BlockType.AdvancedCommand, AdvancedCommands.LogX},
             new object [] {"sin", BlockType.AdvancedCommand, AdvancedCommands.Sin},
             new object [] {"cos", BlockType.AdvancedCommand, AdvancedCommands.Cos},
             new object [] {"debugint", BlockType.AdvancedCommand, AdvancedCommands.DebugInt},
             new object [] {"debugbool", BlockType.AdvancedCommand, AdvancedCommands.DebugBool},
             new object [] {"add", BlockType.BasicCommand, BasicCommands.Add},
             new object [] {"sub", BlockType.BasicCommand, BasicCommands.Subtract},
             new object [] {"mult", BlockType.BasicCommand, BasicCommands.Multiply},
             new object [] {"div", BlockType.BasicCommand, BasicCommands.Divide},
             new object [] {"rnd", BlockType.BasicCommand, BasicCommands.Round},
             new object [] {"*", BlockType.BasicCommand, BasicCommands.Star},
             new object [] {"mod", BlockType.BasicCommand, BasicCommands.Modulus},
             new object [] {"sgn", BlockType.BasicCommand, BasicCommands.Sign},
             new object [] {"abs", BlockType.BasicCommand, BasicCommands.Absolute},
             new object [] {"dup", BlockType.BasicCommand, BasicCommands.Duplicate},
             new object [] {"drop", BlockType.BasicCommand, BasicCommands.Drop},
             new object [] {"clear", BlockType.BasicCommand, BasicCommands.Clear},
             new object [] {"swap", BlockType.BasicCommand, BasicCommands.Swap},
             new object [] {"over", BlockType.BasicCommand, BasicCommands.Over}
        };

        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly IEnumerable<object[]> PairingsWithDuplicates = Pairings.Concat(new[] {
                new object [] {"dupint", BlockType.BasicCommand, BasicCommands.Duplicate},
                new object [] {"dropint", BlockType.BasicCommand, BasicCommands.Drop},
                new object [] {"clearint", BlockType.BasicCommand, BasicCommands.Clear},
                new object [] {"swapint", BlockType.BasicCommand, BasicCommands.Swap},
                new object [] {"overint", BlockType.BasicCommand, BasicCommands.Over}
            }
        );

        [Theory, MemberData(nameof(Pairings))]
        public void BlockToString(string expected, BlockType type, int value)
        {
            var block = new Block(type, value);

            var result = Tokenizer.BlockToCommand(block);

            result.Should().Be(expected);
        }

        [Theory,
             InlineData("add", 1 + 32691),
             InlineData("sub", 2 + 32691),
             InlineData("mult", 3 + 32691),
             InlineData("div", 4 + 32691),
             InlineData("rnd", 5 + 32691),
             InlineData("*", 6 + 32691),
             InlineData("mod", 7 + 32691),
             InlineData("sgn", 8 + 32691),
             InlineData("abs", 9 + 32691),
             InlineData("dup", 10 + 32691),
             InlineData("drop", 11 + 32691),
             InlineData("clear", 12 + 32691),
             InlineData("swap", 13 + 32691),
             InlineData("over", 14 + 32691),
             InlineData("angle", 15 + 32691),
             InlineData("dist", 16 + 32691),
             InlineData("ceil", 17 + 32691),
             InlineData("floor", 18 + 32691),
             InlineData("sqr", 19 + 32691),
             InlineData("pow", 20 + 32691),
             InlineData("pyth", 21 + 32691),
             InlineData("anglecmp", 22 + 32691),
             InlineData("root", 23 + 32691),
             InlineData("logx", 24 + 32691),
             InlineData("sin", 25 + 32691),
             InlineData("cos", 26 + 32691),
             InlineData("debugint", 27 + 32691),
             InlineData("debugbool", 28 + 32691),
             InlineData("~", 29 + 32691),
             InlineData("&", 30 + 32691),
             InlineData("|", 31 + 32691),
             InlineData("^", 32 + 32691),
             InlineData("++", 33 + 32691),
             InlineData("--", 34 + 32691),
             InlineData("-", 35 + 32691),
             InlineData("<<", 36 + 32691),
             InlineData(">>", 37 + 32691),
             InlineData("<", 38 + 32691),
             InlineData(">", 39 + 32691),
             InlineData("=", 40 + 32691),
             InlineData("!=", 41 + 32691),
             InlineData("%=", 42 + 32691),
             InlineData("!%=", 43 + 32691),
             InlineData("~=", 44 + 32691),
             InlineData("!~=", 45 + 32691),
             InlineData(">=", 46 + 32691),
             InlineData("<=", 47 + 32691),
             InlineData("and", 48 + 32691),
             InlineData("or", 49 + 32691),
             InlineData("xor", 50 + 32691),
             InlineData("not", 51 + 32691),
             InlineData("true", 52 + 32691),
             InlineData("false", 53 + 32691),
             InlineData("dropbool", 54 + 32691),
             InlineData("clearbool", 55 + 32691),
             InlineData("dupbool", 56 + 32691),
             InlineData("swapbool", 57 + 32691),
             InlineData("overbool", 58 + 32691),
             InlineData("store", 59 + 32691),
             InlineData("inc", 60 + 32691),
             InlineData("dec", 61 + 32691),
             InlineData("addstore", 62 + 32691),
             InlineData("substore", 63 + 32691),
             InlineData("multstore", 64 + 32691),
             InlineData("divstore", 65 + 32691),
             InlineData("ceilstore", 66 + 32691),
             InlineData("floorstore", 67 + 32691),
             InlineData("rndstore", 68 + 32691),
             InlineData("sgnstore", 69 + 32691),
             InlineData("absstore", 70 + 32691),
             InlineData("sqrstore", 71 + 32691),
             InlineData("negstore", 72 + 32691),
             InlineData("cond", 73 + 32691),
             InlineData("start", 74 + 32691),
             InlineData("else", 75 + 32691),
             InlineData("stop", 76 + 32691),
             InlineData("end", 77 + 32691)
        ]
        public void DnaToInt(string command, int expected)
        {
            var block = Tokenizer.ParseCommand(command);
            var actual = Tokenizer.DnaToInt(block);

            actual.Should().Be(expected);
        }

        [Fact]
        public void NumberToCommand()
        {
            const string expected = "1234";
            var actual = Tokenizer.BlockToCommand(new Block(BlockType.Variable, 1234));

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory, MemberData(nameof(PairingsWithDuplicates))]
        public void ParseBlock(string input, BlockType expectedType, int expectedValue)
        {
            var expected = new Block(expectedType, expectedValue);
            var result = Tokenizer.ParseCommand(input);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ParseDnaDoesNotAppendDuplicateMasterEnd()
        {
            var testInput = $"1 2 add{Environment.NewLine}3 4 sub{Environment.NewLine}end";
            var res = Tokenizer.ParseDna(testInput).ToList();

            res.Should().BeEquivalentTo(new Block(BlockType.Variable, 1), new Block(BlockType.Variable, 2), new Block(BlockType.BasicCommand, 1),
                new Block(BlockType.Variable, 3), new Block(BlockType.Variable, 4), new Block(BlockType.BasicCommand, 2), new Block(BlockType.MasterFlow, 1));
        }

        [Theory,
                    InlineData("' This is a comment"),
            InlineData("    ' This is a comment starting with a space"),
            InlineData("\t'This is a comment starting with a tab"),
            InlineData("'")]
        public void ParseDnaWithCommentOnlyLinesReturnsEmptyList(string input)
        {
            var res = Tokenizer.ParseDna(input);

            res.Should().BeEmpty();
        }

        [Fact]
        public void ParseDnaWithEmbeddedCommentsReturnsCorrectList()
        {
            var testInput = $"1 2 add{Environment.NewLine}' This is an embedded comment{Environment.NewLine}3 4 sub";
            var res = Tokenizer.ParseDna(testInput).ToList();

            res.Should().BeEquivalentTo(new Block(BlockType.Variable, 1), new Block(BlockType.Variable, 2), new Block(BlockType.BasicCommand, 1),
                new Block(BlockType.Variable, 3), new Block(BlockType.Variable, 4), new Block(BlockType.BasicCommand, 2), new Block(BlockType.MasterFlow, 1));
        }

        [Theory, InlineData(""), InlineData(null), InlineData(" "), InlineData("\t")]
        public void ParseDnaWithEmptyStringReturnsEmptyList(string input)
        {
            var res = Tokenizer.ParseDna(input);

            res.Should().BeEmpty();
        }

        [Fact]
        public void ParseDnaWithInlineCommentReturnsCorrectList()
        {
            var testInput = $"1 2 add ' This is an inline comment{Environment.NewLine}3 4 sub";
            var res = Tokenizer.ParseDna(testInput).ToList();

            res.Should().BeEquivalentTo(new Block(BlockType.Variable, 1), new Block(BlockType.Variable, 2), new Block(BlockType.BasicCommand, 1),
                new Block(BlockType.Variable, 3), new Block(BlockType.Variable, 4), new Block(BlockType.BasicCommand, 2), new Block(BlockType.MasterFlow, 1));
        }

        [Fact]
        public void ParseDnaWithIrregularSpacingReturnsCorrectList()
        {
            const string testInput = "1   2  add";
            var res = Tokenizer.ParseDna(testInput).ToList();

            res.Should().BeEquivalentTo(new Block(BlockType.Variable, 1), new Block(BlockType.Variable, 2), new Block(BlockType.BasicCommand, 1), new Block(BlockType.MasterFlow, 1));
        }

        [Fact]
        public void ParseDnaWithMultipleCommandsReturnsCorrectList()
        {
            const string testInput = "1 2 add";
            var res = Tokenizer.ParseDna(testInput).ToList();

            res.Should().BeEquivalentTo(new Block(BlockType.Variable, 1), new Block(BlockType.Variable, 2), new Block(BlockType.BasicCommand, 1), new Block(BlockType.MasterFlow, 1));
        }

        [Fact]
        public void ParseDnaWithMultipleLinesReturnsCorrectList()
        {
            var testInput = $"1 2 add{Environment.NewLine}3 4 sub";
            var res = Tokenizer.ParseDna(testInput).ToList();

            res.Should().BeEquivalentTo(new Block(BlockType.Variable, 1), new Block(BlockType.Variable, 2), new Block(BlockType.BasicCommand, 1),
                new Block(BlockType.Variable, 3), new Block(BlockType.Variable, 4), new Block(BlockType.BasicCommand, 2), new Block(BlockType.MasterFlow, 1));
        }

        [Fact]
        public void ParseDnaWithSingleCommandReturnsCorrectItem()
        {
            const string testInput = "start";
            var res = Tokenizer.ParseDna(testInput).ToList();

            res.Should().BeEquivalentTo(new Block(BlockType.Flow, 2), new Block(BlockType.MasterFlow, 1));
        }

        [Fact]
        public void ParseNumber()
        {
            var expected = new Block(BlockType.Variable, 1234);
            var result = Tokenizer.ParseCommand($"{expected.Value}");

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ParseStarNumber()
        {
            var expected = new Block(BlockType.StarVariable, 1234);
            var result = Tokenizer.ParseCommand($"*{expected.Value}");

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ParseStarSystemVariable()
        {
            foreach (var s in Tokenizer.SystemVariables)
            {
                var expected = new Block(BlockType.StarVariable, s.Address);
                var result = Tokenizer.ParseCommand($"*.{s.Name}");

                result.Should().BeEquivalentTo(expected);
            }
        }

        [Fact]
        public void ParseSystemVariable()
        {
            foreach (var s in Tokenizer.SystemVariables)
            {
                var expected = new Block(BlockType.Variable, s.Address);
                var result = Tokenizer.ParseCommand($".{s.Name}");

                result.Should().BeEquivalentTo(expected);
            }
        }

        [Fact]
        public void StarNumberToCommand()
        {
            const string expected = "*1234";
            var actual = Tokenizer.BlockToCommand(new Block(BlockType.StarVariable, 1234));

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void StarSystemVariableToCommand()
        {
            foreach (var address in Tokenizer.SystemVariables.Select(v => v.Address).Distinct())
            {
                var s = Tokenizer.SystemVariables.First(v => v.Address == address);
                var expected = $"*.{s.Name}";
                var result = Tokenizer.BlockToCommand(new Block(BlockType.StarVariable, s.Address));

                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory,
         InlineData(1, -16645 + 32729),
         InlineData(1000, -15647 + 32729),
         InlineData(10000, -11256 + 32729),
         InlineData(32000, -525 + 32729),
         InlineData(35000, -525 + 32729),
         InlineData(-1, -16647 + 32729),
         InlineData(-1000, -17646 + 32729),
         InlineData(-10000, -22037 + 32729),
         InlineData(-32000, -32768 + 32729),
         InlineData(-35000, -32768 + 32729)
        ]
        public void StarVariableToDnaInt(int number, int expected)
        {
            var block = new Block(BlockType.StarVariable, number);
            var actual = Tokenizer.DnaToInt(block);

            actual.Should().Be(expected);
        }

        [Fact]
        public void SystemVariableToCommand()
        {
            foreach (var address in Tokenizer.SystemVariables.Select(v => v.Address).Distinct())
            {
                var s = Tokenizer.SystemVariables.First(v => v.Address == address);
                var expected = $".{s.Name}";
                var result = Tokenizer.BlockToCommand(new Block(BlockType.Variable, s.Address));

                result.Should().BeEquivalentTo(expected);
            }
        }

        [Theory,
            InlineData(1, -16645),
            InlineData(1000, -15647),
            InlineData(10000, -11256),
            InlineData(32000, -525),
            InlineData(35000, -525),
            InlineData(-1, -16647),
            InlineData(-1000, -17646),
            InlineData(-10000, -22037),
            InlineData(-32000, -32768),
            InlineData(-35000, -32768)
        ]
        public void VariableToDnaInt(int number, int expected)
        {
            var block = new Block(BlockType.Variable, number);
            var actual = Tokenizer.DnaToInt(block);

            actual.Should().Be(expected);
        }
    }
}