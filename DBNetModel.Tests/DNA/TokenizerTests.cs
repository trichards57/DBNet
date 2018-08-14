using System.Linq;
using DBNetModel.DNA;
using FluentAssertions;
using Xunit;

namespace DBNetModel.Tests.DNA
{
    public class TokenizerTests
    {
        [Theory,
             InlineData("end", BlockType.MasterFlow, 1),
             InlineData("cond", BlockType.Flow, 1),
             InlineData("start", BlockType.Flow, 2),
             InlineData("else", BlockType.Flow, 3),
             InlineData("stop", BlockType.Flow, 4),
             InlineData("store", BlockType.Stores, 1),
             InlineData("inc", BlockType.Stores, 2),
             InlineData("dec", BlockType.Stores, 3),
             InlineData("addstore", BlockType.Stores, 4),
             InlineData("substore", BlockType.Stores, 5),
             InlineData("multstore", BlockType.Stores, 6),
             InlineData("divstore", BlockType.Stores, 7),
             InlineData("ceilstore", BlockType.Stores, 8),
             InlineData("floorstore", BlockType.Stores, 9),
             InlineData("rndstore", BlockType.Stores, 10),
             InlineData("sgnstore", BlockType.Stores, 11),
             InlineData("absstore", BlockType.Stores, 12),
             InlineData("sqrstore", BlockType.Stores, 13),
             InlineData("negstore", BlockType.Stores, 14),
             InlineData("and", BlockType.Logic, 1),
             InlineData("or", BlockType.Logic, 2),
             InlineData("xor", BlockType.Logic, 3),
             InlineData("not", BlockType.Logic, 4),
             InlineData("true", BlockType.Logic, 5),
             InlineData("false", BlockType.Logic, 6),
             InlineData("dropbool", BlockType.Logic, 7),
             InlineData("clearbool", BlockType.Logic, 8),
             InlineData("dupbool", BlockType.Logic, 9),
             InlineData("swapbool", BlockType.Logic, 10),
             InlineData("overbool", BlockType.Logic, 11),
             InlineData("<", BlockType.Condition, 1),
             InlineData(">", BlockType.Condition, 2),
             InlineData("=", BlockType.Condition, 3),
             InlineData("!=", BlockType.Condition, 4),
             InlineData("%=", BlockType.Condition, 5),
             InlineData("!%=", BlockType.Condition, 6),
             InlineData("~=", BlockType.Condition, 7),
             InlineData("!~=", BlockType.Condition, 8),
             InlineData(">=", BlockType.Condition, 9),
             InlineData("<=", BlockType.Condition, 10),
             InlineData("~", BlockType.Bitwise, 1),
             InlineData("&", BlockType.Bitwise, 2),
             InlineData("|", BlockType.Bitwise, 3),
             InlineData("^", BlockType.Bitwise, 4),
             InlineData("++", BlockType.Bitwise, 5),
             InlineData("--", BlockType.Bitwise, 6),
             InlineData("-", BlockType.Bitwise, 7),
             InlineData("<<", BlockType.Bitwise, 8),
             InlineData(">>", BlockType.Bitwise, 9),
             InlineData("angle", BlockType.AdvancedCommand, 1),
             InlineData("dist", BlockType.AdvancedCommand, 2),
             InlineData("ceil", BlockType.AdvancedCommand, 3),
             InlineData("floor", BlockType.AdvancedCommand, 4),
             InlineData("sqr", BlockType.AdvancedCommand, 5),
             InlineData("pow", BlockType.AdvancedCommand, 6),
             InlineData("pyth", BlockType.AdvancedCommand, 7),
             InlineData("anglecmp", BlockType.AdvancedCommand, 8),
             InlineData("root", BlockType.AdvancedCommand, 9),
             InlineData("logx", BlockType.AdvancedCommand, 10),
             InlineData("sin", BlockType.AdvancedCommand, 11),
             InlineData("cos", BlockType.AdvancedCommand, 12),
             InlineData("debugint", BlockType.AdvancedCommand, 13),
             InlineData("debugbool", BlockType.AdvancedCommand, 14),
             InlineData("add", BlockType.BasicCommand, 1),
             InlineData("sub", BlockType.BasicCommand, 2),
             InlineData("mult", BlockType.BasicCommand, 3),
             InlineData("div", BlockType.BasicCommand, 4),
             InlineData("rnd", BlockType.BasicCommand, 5),
             InlineData("*", BlockType.BasicCommand, 6),
             InlineData("mod", BlockType.BasicCommand, 7),
             InlineData("sgn", BlockType.BasicCommand, 8),
             InlineData("abs", BlockType.BasicCommand, 9),
             InlineData("dup", BlockType.BasicCommand, 10),
             InlineData("drop", BlockType.BasicCommand, 11),
             InlineData("clear", BlockType.BasicCommand, 12),
             InlineData("swap", BlockType.BasicCommand, 13),
             InlineData("over", BlockType.BasicCommand, 14),
        ]
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

        [Theory,
            InlineData("end", BlockType.MasterFlow, 1),
            InlineData("cond", BlockType.Flow, 1),
            InlineData("start", BlockType.Flow, 2),
            InlineData("else", BlockType.Flow, 3),
            InlineData("stop", BlockType.Flow, 4),
            InlineData("store", BlockType.Stores, 1),
            InlineData("inc", BlockType.Stores, 2),
            InlineData("dec", BlockType.Stores, 3),
            InlineData("addstore", BlockType.Stores, 4),
            InlineData("substore", BlockType.Stores, 5),
            InlineData("multstore", BlockType.Stores, 6),
            InlineData("divstore", BlockType.Stores, 7),
            InlineData("ceilstore", BlockType.Stores, 8),
            InlineData("floorstore", BlockType.Stores, 9),
            InlineData("rndstore", BlockType.Stores, 10),
            InlineData("sgnstore", BlockType.Stores, 11),
            InlineData("absstore", BlockType.Stores, 12),
            InlineData("sqrstore", BlockType.Stores, 13),
            InlineData("negstore", BlockType.Stores, 14),
            InlineData("and", BlockType.Logic, 1),
            InlineData("or", BlockType.Logic, 2),
            InlineData("xor", BlockType.Logic, 3),
            InlineData("not", BlockType.Logic, 4),
            InlineData("true", BlockType.Logic, 5),
            InlineData("false", BlockType.Logic, 6),
            InlineData("dropbool", BlockType.Logic, 7),
            InlineData("clearbool", BlockType.Logic, 8),
            InlineData("dupbool", BlockType.Logic, 9),
            InlineData("swapbool", BlockType.Logic, 10),
            InlineData("overbool", BlockType.Logic, 11),
            InlineData("<", BlockType.Condition, 1),
            InlineData(">", BlockType.Condition, 2),
            InlineData("=", BlockType.Condition, 3),
            InlineData("!=", BlockType.Condition, 4),
            InlineData("%=", BlockType.Condition, 5),
            InlineData("!%=", BlockType.Condition, 6),
            InlineData("~=", BlockType.Condition, 7),
            InlineData("!~=", BlockType.Condition, 8),
            InlineData(">=", BlockType.Condition, 9),
            InlineData("<=", BlockType.Condition, 10),
            InlineData("~", BlockType.Bitwise, 1),
            InlineData("&", BlockType.Bitwise, 2),
            InlineData("|", BlockType.Bitwise, 3),
            InlineData("^", BlockType.Bitwise, 4),
            InlineData("++", BlockType.Bitwise, 5),
            InlineData("--", BlockType.Bitwise, 6),
            InlineData("-", BlockType.Bitwise, 7),
            InlineData("<<", BlockType.Bitwise, 8),
            InlineData(">>", BlockType.Bitwise, 9),
            InlineData("angle", BlockType.AdvancedCommand, 1),
            InlineData("dist", BlockType.AdvancedCommand, 2),
            InlineData("ceil", BlockType.AdvancedCommand, 3),
            InlineData("floor", BlockType.AdvancedCommand, 4),
            InlineData("sqr", BlockType.AdvancedCommand, 5),
            InlineData("pow", BlockType.AdvancedCommand, 6),
            InlineData("pyth", BlockType.AdvancedCommand, 7),
            InlineData("anglecmp", BlockType.AdvancedCommand, 8),
            InlineData("root", BlockType.AdvancedCommand, 9),
            InlineData("logx", BlockType.AdvancedCommand, 10),
            InlineData("sin", BlockType.AdvancedCommand, 11),
            InlineData("cos", BlockType.AdvancedCommand, 12),
            InlineData("debugint", BlockType.AdvancedCommand, 13),
            InlineData("debugbool", BlockType.AdvancedCommand, 14),
            InlineData("add", BlockType.BasicCommand, 1),
            InlineData("sub", BlockType.BasicCommand, 2),
            InlineData("mult", BlockType.BasicCommand, 3),
            InlineData("div", BlockType.BasicCommand, 4),
            InlineData("rnd", BlockType.BasicCommand, 5),
            InlineData("*", BlockType.BasicCommand, 6),
            InlineData("mod", BlockType.BasicCommand, 7),
            InlineData("sgn", BlockType.BasicCommand, 8),
            InlineData("abs", BlockType.BasicCommand, 9),
            InlineData("dup", BlockType.BasicCommand, 10),
            InlineData("dupint", BlockType.BasicCommand, 10),
            InlineData("drop", BlockType.BasicCommand, 11),
            InlineData("dropint", BlockType.BasicCommand, 11),
            InlineData("clear", BlockType.BasicCommand, 12),
            InlineData("clearint", BlockType.BasicCommand, 12),
            InlineData("swap", BlockType.BasicCommand, 13),
            InlineData("swapint", BlockType.BasicCommand, 13),
            InlineData("over", BlockType.BasicCommand, 14),
            InlineData("overint", BlockType.BasicCommand, 14),
        ]
        public void ParseBlock(string input, BlockType expectedType, int expectedValue)
        {
            var expected = new Block(expectedType, expectedValue);
            var result = Tokenizer.ParseCommand(input);

            result.Should().BeEquivalentTo(expected);
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