using DBNetEngine;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DBNetModel.Tests
{
    public class MathSupportTests
    {
        private const float AngleTolerance = 1.0f;

        [Fact]
        public void AngleDifferenceGivesDifference()
        {
            var angle1 = 2;
            var angle2 = 1;

            var result = MathSupport.AngleDifference(angle1, angle2);

            result.Should().BeApproximately(1.0f, AngleTolerance);
        }

        [Fact]
        public void AngleDifferenceGivesNormalisedDifference()
        {
            var angle1 = 2 + (float)Math.PI * 2;
            var angle2 = 1;

            var result = MathSupport.AngleDifference(angle1, angle2);

            result.Should().BeApproximately(1.0f, AngleTolerance);
        }

        [Fact]
        public void AngleNormaliseHandlesNormalisedValue()
        {
            var angle1 = 1;

            var result = MathSupport.AngleNormalise(angle1);

            result.Should().BeApproximately(1.0f, AngleTolerance);
        }

        [Fact]
        public void AngleNormaliseHandlesOverLargeValue()
        {
            var angle1 = 1 + (float)Math.PI * 6;

            var result = MathSupport.AngleNormalise(angle1);

            result.Should().BeApproximately(1.0f, AngleTolerance);
        }

        [Fact]
        public void AngleNormaliseHandlesOverSmallValue()
        {
            var angle1 = 1 - (float)Math.PI * 6;

            var result = MathSupport.AngleNormalise(angle1);

            result.Should().BeApproximately(1.0f, AngleTolerance);
        }
    }
}