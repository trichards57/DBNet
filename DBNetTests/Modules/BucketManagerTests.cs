using DarwinBots.Model;
using DarwinBots.Modules;
using FluentAssertions;
using System;
using Xunit;

namespace DBNetTests.Modules
{
    public class BucketManagerTests
    {
        private const int FieldHeight = (int)(BucketManager.BucketSize * 2.6);
        private const int FieldWidth = (int)(BucketManager.BucketSize * 2.8);

        [Theory,
            InlineData(0, 0, 0, 0),
            InlineData(10, BucketManager.BucketSize * 1.1, 0, 1),
            InlineData(BucketManager.BucketSize * 1.1, BucketManager.BucketSize * 1.1, 1, 1),
            InlineData(BucketManager.BucketSize * 2.3, BucketManager.BucketSize * 2.7, 2, 2),
            InlineData(BucketManager.BucketSize * 3, BucketManager.BucketSize * 3, 2, 2)]
        public void AssignsBucket(double x, double y, int bucketX, int bucketY)
        {
            var options = new SimOptions { FieldHeight = FieldHeight, FieldWidth = FieldWidth };

            var manager = new BucketManager(options);

            var bot = new Robot(manager) { Position = new DoubleVector(x, y) };

            bot.BucketPosition.Should().Be(new IntVector(-1, -1));

            manager.UpdateBotBucket(bot);

            bot.BucketPosition.Should().Be(new IntVector(bucketX, bucketY));
        }

        [Fact]
        public void CloseBotCrossesEyes()
        {
            SimOpt.SimOpts = new SimOptions { FieldHeight = FieldHeight, FieldWidth = FieldWidth, FixedBotRadii = true };

            var manager = new BucketManager(SimOpt.SimOpts);

            var position1 = new DoubleVector(0, 0);
            var position2 = new DoubleVector(500, 0);

            var bot1 = new Robot(manager) { Position = position1, AbsNum = 0 };
            var bot2 = new Robot(manager) { Position = position2, AbsNum = 1 };

            manager.UpdateBotBucket(bot1);
            manager.UpdateBotBucket(bot2);

            var b = manager.UpdateSight(bot1, true);

            bot1.Memory[MemoryAddresses.EyeStart + 3].Should().Be(0);
            bot1.Memory[MemoryAddresses.EyeStart + 4].Should().BeGreaterThan(0);
            bot1.Memory[MemoryAddresses.EyeStart + 5].Should().BeGreaterThan(0);
            bot1.Memory[MemoryAddresses.EyeStart + 6].Should().BeGreaterThan(0);
            bot1.Memory[MemoryAddresses.EyeStart + 7].Should().Be(0);

            bot1.LastSeenObject.Should().Be(b);
        }

        [Fact]
        public void CloserBotReported()
        {
            SimOpt.SimOpts = new SimOptions { FieldHeight = FieldHeight, FieldWidth = FieldWidth, FixedBotRadii = true };

            var manager = new BucketManager(SimOpt.SimOpts);

            var position1 = new DoubleVector(0, 0);
            var position2 = new DoubleVector(500, 0);
            var position3 = new DoubleVector(1000, 0);

            var bot1 = new Robot(manager) { Position = position1, AbsNum = 0 };
            var bot2 = new Robot(manager) { Position = position2, AbsNum = 2 };
            var bot3 = new Robot(manager) { Position = position3, AbsNum = 1 };

            manager.UpdateBotBucket(bot1);
            manager.UpdateBotBucket(bot3);

            manager.UpdateSight(bot1, true);

            bot1.Memory[MemoryAddresses.EyeStart + 3].Should().Be(0);
            bot1.Memory[MemoryAddresses.EyeStart + 4].Should().Be(0);
            bot1.Memory[MemoryAddresses.EyeStart + 5].Should().BeGreaterThan(0);
            bot1.Memory[MemoryAddresses.EyeStart + 6].Should().Be(0);
            bot1.Memory[MemoryAddresses.EyeStart + 7].Should().Be(0);

            var previous5 = bot1.Memory[MemoryAddresses.EyeStart + 5];

            manager.UpdateBotBucket(bot2);
            manager.UpdateSight(bot1, true);

            bot1.Memory[MemoryAddresses.EyeStart + 5].Should().BeGreaterThan(previous5);

            bot1.LastSeenObject.Should().Be(bot2);
        }

        [Theory, InlineData(new[] { Robot.RobSize / 2.0, Robot.RobSize / 2 }, new[] { Robot.RobSize / 2.0, Robot.RobSize * 3 / 2 - 1 }, new[] { Robot.RobSize / 2.0, Robot.RobSize * 3 / 2 + 1 }, true, false),
            InlineData(new[] { Robot.RobSize / 2, Robot.RobSize / 2.0 }, new[] { Robot.RobSize * 3 / 2 - 1, Robot.RobSize / 2.0 }, new[] { Robot.RobSize * 3 / 2 + 1, Robot.RobSize / 2.0 }, true, false)]
        public void DetectsCollision(double[] position1, double[] position2, double[] position3, bool collides2, bool collides3)
        {
            SimOpt.SimOpts = new SimOptions { FieldHeight = FieldHeight, FieldWidth = FieldWidth, FixedBotRadii = true };

            var manager = new BucketManager(SimOpt.SimOpts);

            var bot1 = new Robot(manager) { Position = position1, AbsNum = 0 };
            var bot2 = new Robot(manager) { Position = position2, AbsNum = 1 };
            var bot3 = new Robot(manager) { Position = position3, AbsNum = 2 };

            manager.UpdateBotBucket(bot1);
            manager.UpdateBotBucket(bot2);
            manager.UpdateBotBucket(bot3);

            var collisionBots = manager.CheckForCollisions(bot1, true);

            if (collides2)
                collisionBots.Should().Contain(bot2);
            else
                collisionBots.Should().NotContain(bot2);

            if (collides3)
                collisionBots.Should().Contain(bot3);
            else
                collisionBots.Should().NotContain(bot3);
        }

        [Fact]
        public void DetectsCollisionInAdjacentBucket()
        {
            SimOpt.SimOpts = new SimOptions { FieldHeight = FieldHeight, FieldWidth = FieldWidth, FixedBotRadii = true };

            var manager = new BucketManager(SimOpt.SimOpts);

            var position1 = new DoubleVector(BucketManager.BucketSize - 1, BucketManager.BucketSize - 1);
            var position2 = new DoubleVector(BucketManager.BucketSize + 1, BucketManager.BucketSize + 1);

            var bot1 = new Robot(manager) { Position = position1, AbsNum = 0 };
            var bot2 = new Robot(manager) { Position = position2, AbsNum = 1 };

            manager.UpdateBotBucket(bot1);
            manager.UpdateBotBucket(bot2);

            var collisionBots = manager.CheckForCollisions(bot1, true);

            collisionBots.Should().Contain(bot2);
        }

        [Theory,
            InlineData(0, 5),
            InlineData(-Math.PI / 18, 6), InlineData(-2 * Math.PI / 18, 7),
            InlineData(-3 * Math.PI / 18, 8), InlineData(-4 * Math.PI / 18, 9),
            InlineData(1 * Math.PI / 18, 4), InlineData(2 * Math.PI / 18, 3),
            InlineData(3 * Math.PI / 18, 2), InlineData(4 * Math.PI / 18, 1)]
        public void DetectsVisibleBot(double aim, int eyeNumber)
        {
            SimOpt.SimOpts = new SimOptions { FieldHeight = FieldHeight, FieldWidth = FieldWidth, FixedBotRadii = true };

            var manager = new BucketManager(SimOpt.SimOpts);

            var position1 = new DoubleVector(0, 0);
            var position2 = new DoubleVector(1000, 0);

            var bot1 = new Robot(manager) { Position = position1, AbsNum = 0, Aim = aim };
            var bot2 = new Robot(manager) { Position = position2, AbsNum = 1 };

            bot1.Memory[MemoryAddresses.FOCUSEYE] = eyeNumber - 5;

            manager.UpdateBotBucket(bot1);
            manager.UpdateBotBucket(bot2);

            var b = manager.UpdateSight(bot1, true);

            if (eyeNumber > 1)
                bot1.Memory[MemoryAddresses.EyeStart + eyeNumber - 1].Should().Be(0);
            bot1.Memory[MemoryAddresses.EyeStart + eyeNumber].Should().BeGreaterThan(0);
            if (eyeNumber < 9)
                bot1.Memory[MemoryAddresses.EyeStart + eyeNumber + 1].Should().Be(0);

            b.Should().Be(bot2);

            bot1.LastSeenObject.Should().Be(b);
        }
    }
}
