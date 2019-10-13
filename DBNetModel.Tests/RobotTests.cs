using DBNetEngine;
using FluentAssertions;
using System;
using Xunit;

namespace DBNetModel.Tests
{
    public class RobotTests
    {
        private const float FloatTolerance = 0.001f;

        [Fact]
        public void GetAbsoluteAccelerationClipsResult()
        {
            var x1 = 1e6f;
            var y1 = -1e6f;
            var x2 = -1e6f;
            var y2 = 1e6f;
            var angle = 0;

            var res = Robot.GetAbsoluteAcceleration(angle, x1, x2, y1, y2);

            res.X.Should().BeApproximately(MathSupport.MaxValue, FloatTolerance);
            res.Y.Should().BeApproximately(-MathSupport.MaxValue, FloatTolerance);
        }

        [Theory, InlineData(0.0f, 1.0f, 0.0f), InlineData(Math.PI / 2, 0.0f, -1.0f),
                    InlineData(Math.PI, -1.0f, 0.0f), InlineData(Math.PI * 3 / 2, 0.0f, 1.0f),
            InlineData(Math.PI / 4, 0.707f, -0.707f)]
        public void GetAbsoluteAccelerationRotatesResult(float angle, float expectedX, float expectedY)
        {
            var res = Robot.GetAbsoluteAcceleration(angle, 1, 0, 0, 0);

            res.X.Should().BeApproximately(expectedX, FloatTolerance);
            res.Y.Should().BeApproximately(expectedY, FloatTolerance);
        }

        [Theory, InlineData(0.0f, -3.0f, -6.0f), InlineData(Math.PI / 2, -6.0f, 3.0f),
            InlineData(Math.PI, 3.0f, 6.0f), InlineData(Math.PI * 3 / 2, 6.0f, -3.0f)]
        public void GetAbsoluteAccelerationSumsComponentsAndRotates(float angle, float expectedX, float expectedY)
        {
            var x1 = 1.0f;
            var y1 = 2.0f;
            var x2 = 4.0f;
            var y2 = 8.0f;

            var res = Robot.GetAbsoluteAcceleration(angle, x1, x2, y1, y2);

            res.X.Should().BeApproximately(expectedX, FloatTolerance);
            res.Y.Should().BeApproximately(expectedY, FloatTolerance);
        }
    }
}