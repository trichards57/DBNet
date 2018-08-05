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
    }
}