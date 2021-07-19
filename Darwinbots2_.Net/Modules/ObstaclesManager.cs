using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DarwinBots.Modules
{
    internal class ObstaclesManager
    {
        private const int MaxObstacles = 1000;
        public List<Obstacle> Obstacles { get; } = new();
        private static double DefaultHeight => 0.2;
        private static double DefaultWidth => 0.2;
        private static int MazeCorridorWidth => 500;
        private static int MazeWallThickness => 50;
        private Obstacle LeftCompactor { get; set; }
        private Obstacle RightCompactor { get; set; }

        public void AddRandomObstacles(int n)
        {
            if (n < 1)
                return;

            for (var i = 0; i < n; i++)
            {
                var randomX = ThreadSafeRandom.Local.NextDouble() * SimOpt.SimOpts.FieldWidth;
                var randomy = ThreadSafeRandom.Local.NextDouble() * SimOpt.SimOpts.FieldHeight;

                var randomWidth = ThreadSafeRandom.Local.NextDouble() * SimOpt.SimOpts.FieldWidth * DefaultWidth;
                var randomHeight = ThreadSafeRandom.Local.NextDouble() * SimOpt.SimOpts.FieldHeight * DefaultHeight;

                //Shift everything up and left by half the max dimensions then trim to more evenly distribute obstacles across the field
                randomX -= SimOpt.SimOpts.FieldWidth * (DefaultWidth / 2);
                randomy -= SimOpt.SimOpts.FieldHeight * (DefaultHeight / 2);

                if (randomX < 0)
                    randomX = 0;
                if (randomy < 0)
                    randomy = 0;

                if (randomX + randomWidth > SimOpt.SimOpts.FieldWidth)
                    randomWidth = SimOpt.SimOpts.FieldWidth - randomX;
                if (randomy + randomHeight > SimOpt.SimOpts.FieldHeight)
                    randomHeight = SimOpt.SimOpts.FieldHeight - randomy;
                var res = NewObstacle(randomX, randomy, randomWidth, randomHeight);

                if (res == null)
                    break;
            }
        }

        public void ChangeAllObstacleColor(Color? color)
        {
            foreach (var o in Obstacles)
                o.Color = color ?? Color.FromRgb((byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256));
        }

        public void DeleteAllObstacles()
        {
            foreach (var o in Obstacles.ToArray())
            {
                o.Exist = false;
                Obstacles.Remove(o);
            }
        }

        public void DeleteObstacle(Obstacle o)
        {
            o.Exist = false;
            Obstacles.Remove(o);
        }

        public void DeleteTenRandomObstacles()
        {
            var numToDelete = Math.Min(Obstacles.Count, 10);

            for (var i = 0; i > numToDelete; i++)
                DeleteObstacle(Obstacles[ThreadSafeRandom.Local.Next(0, Obstacles.Count)]);
        }

        public void DoObstacleCollisions(robot rob)
        {
            const double k = 0.5;
            const double b = 0.5;
            var numofcollisions = 0;
            var lastPush = 0;

            foreach (var o in Obstacles.Where(o => o.Exist && ObstacleCollision(rob, o)))
            {
                numofcollisions++;
                if (numofcollisions >= 3)
                {
                    // Prevents getting trapped
                    rob.pos = new DoubleVector(200 * Math.Sign(SimOpt.SimOpts.TotRunCycle % 40 - 20), 200 * Math.Sign(SimOpt.SimOpts.TotRunCycle % 50 - 25));
                    return;
                }

                //Push the bot the closest edge
                var distUp = rob.pos.Y + rob.Radius - o.Position.Y;
                var distDown = o.Position.Y + o.Height - (rob.pos.Y - rob.Radius);
                var distLeft = rob.pos.X + rob.Radius - o.Position.X;
                var distRight = o.Position.X + o.Width - (rob.pos.X - rob.Radius);

                if (Math.Min(distLeft, distRight) < Math.Min(distUp, distDown) && lastPush != 1 && lastPush != 2 || lastPush is 3 or 4)
                {
                    //Push left or right
                    if ((distLeft <= distRight || o.Position.X + o.Width >= SimOpt.SimOpts.FieldWidth) && o.Position.X > 0)
                    {
                        if (rob.pos.X - rob.Radius < o.Position.X)
                        {
                            rob.pos = new DoubleVector(o.Position.X - rob.Radius, rob.pos.Y);
                            rob.ImpulseRes += new DoubleVector(rob.vel.X * b, 0);
                            Senses.Touch(rob, rob.pos.X + rob.Radius, rob.pos.Y); // Update hit senses, right side
                        }
                        else
                        {
                            rob.ImpulseRes += new DoubleVector(distLeft * k, 0);
                            rob.pos = new DoubleVector(o.Position.X - rob.Radius, rob.pos.Y);
                        }
                        lastPush = 1;
                    }
                    else
                    {
                        if (rob.pos.X + rob.Radius > o.Position.X + o.Width)
                        {
                            rob.pos = new DoubleVector(o.Position.X + o.Width + rob.Radius, rob.pos.Y);
                            rob.ImpulseRes += new DoubleVector(rob.vel.X * b, 0);
                            Senses.Touch(rob, rob.pos.X - rob.Radius, rob.pos.Y); // Update hit senses, left side
                        }
                        else
                        {
                            rob.ImpulseRes -= new DoubleVector(distRight * k, 0);
                            rob.pos = new DoubleVector(o.Position.X + o.Width + rob.Radius, rob.pos.Y);
                        }
                        lastPush = 2;
                    }
                }
                else
                {
                    //Push up or down
                    if ((distUp <= distDown || o.Position.Y + o.Height >= SimOpt.SimOpts.FieldHeight) && o.Position.Y > 0)
                    {
                        if (rob.pos.Y - rob.Radius < o.Position.Y)
                        {
                            rob.pos = new DoubleVector(rob.pos.X, o.Position.Y - rob.Radius);
                            rob.ImpulseRes += new DoubleVector(0, rob.vel.Y * b);
                            Senses.Touch(rob, rob.pos.X, rob.pos.Y + rob.Radius); // Update hit senses, bottom
                        }
                        else
                        {
                            rob.ImpulseRes += new DoubleVector(0, distUp * k);
                            rob.pos = new DoubleVector(rob.pos.X, o.Position.Y - rob.Radius);
                        }
                        lastPush = 3;
                    }
                    else
                    {
                        if (rob.pos.Y + rob.Radius > o.Position.Y + o.Height)
                        {
                            rob.pos = new DoubleVector(rob.pos.X, o.Position.Y + o.Height + rob.Radius);
                            rob.ImpulseRes += new DoubleVector(0, rob.vel.Y * b);
                            Senses.Touch(rob, rob.pos.X, rob.pos.Y - rob.Radius); // Update hit senses, bottom
                        }
                        else
                        {
                            rob.ImpulseRes -= new DoubleVector(0, distDown * k);
                            rob.pos = new DoubleVector(rob.pos.X, o.Position.Y + o.Height + rob.Radius);
                        }

                        lastPush = 4;
                    }
                }

                if (rob.mem[MemoryAddresses.EYEF] == 0)
                    rob.mem[MemoryAddresses.REFTYPE] = 1;
            }
        }

        public void DoShotObstacleCollisions(Shot shot)
        {
            foreach (var o in Obstacles.Where(o => o.Exist).Where(o => shot.Position.X >= o.Position.X && shot.Position.X <= o.Position.X + o.Width && shot.Position.Y >= o.Position.Y && shot.Position.Y <= o.Position.Y + o.Height))
            {
                if (SimOpt.SimOpts.ShapesAbsorbShots)
                {
                    shot.Exist = false;
                    Globals.ShotsManager.Shots.Remove(shot);
                }

                if (shot.OldPosition.X < o.Position.X || shot.OldPosition.X > o.Position.X + o.Width)
                    shot.Velocity = shot.Velocity.InvertX();

                if (shot.OldPosition.Y < o.Position.Y || shot.OldPosition.Y > o.Position.Y + o.Height)
                    shot.Velocity = shot.Velocity.InvertY();
            }
        }

        public void DrawCheckerboardMaze()
        {
            var blockWidth = Math.Min(5000, (double)SimOpt.SimOpts.FieldWidth / 10);

            var numBlocksAcross = (int)(SimOpt.SimOpts.FieldWidth / (blockWidth + MazeCorridorWidth));
            var acrossGap = (numBlocksAcross * (blockWidth + MazeCorridorWidth) + MazeCorridorWidth - SimOpt.SimOpts.FieldWidth) / 2;
            var numBlocksDown = (int)(SimOpt.SimOpts.FieldHeight / (blockWidth + MazeCorridorWidth));
            var downGap = (numBlocksDown * (blockWidth + MazeCorridorWidth) + MazeCorridorWidth - SimOpt.SimOpts.FieldHeight) / 2;

            for (var i = 0; i < numBlocksAcross - 1; i++)
            {
                for (var j = 0; j < numBlocksDown - 1; j++)
                {
                    var x = i * blockWidth + (i + 1) * MazeCorridorWidth - acrossGap;
                    var y = j * blockWidth + (j + 1) * MazeCorridorWidth - downGap;
                    NewObstacle(x, y, blockWidth, blockWidth);
                }
            }
        }

        public void DrawHorizontalMaze()
        {
            var numOfLines = (int)((double)SimOpt.SimOpts.FieldWidth / (MazeCorridorWidth + MazeWallThickness)) - 1;
            for (var i = 1; i < numOfLines; i++)
            {
                var opening = ThreadSafeRandom.Local.Next(0, SimOpt.SimOpts.FieldHeight - MazeCorridorWidth);
                NewObstacle(i * MazeCorridorWidth + MazeWallThickness, -100, MazeWallThickness, opening);
                if (opening + MazeCorridorWidth < SimOpt.SimOpts.FieldHeight + 100)
                {
                    NewObstacle(i * (MazeCorridorWidth + MazeWallThickness), opening + MazeCorridorWidth, MazeWallThickness, SimOpt.SimOpts.FieldHeight + 100 - opening - MazeCorridorWidth);
                }
            }
        }

        public void DrawObstacles()
        {
            //int i = 0;

            //for (i = 1; i < numObstacles; i++)
            //{
            //    if (Obstacles(i).exist)
            //    {
            //        if (SimOpts.MakeAllShapesTransparent)
            //        {
            //            //Form1.Line(Obstacles(i).pos.x, Obstacles(i).pos.y) - (Obstacles(i).pos.x + Obstacles(i).Width, Obstacles(i).pos.y + Obstacles(i).Height), Obstacles(i).color, B);
            //        }
            //        else
            //        {
            //            //Form1.Line(Obstacles(i).pos.x, Obstacles(i).pos.y)-(Obstacles(i).pos.x + Obstacles(i).Width, Obstacles(i).pos.y + Obstacles(i).Height), Obstacles(i).color, BF);
            //        }
            //        if (i == obstaclefocus)
            //        {
            //            //Form1.Line(Obstacles(i).pos.x - 2, Obstacles(i).pos.y - 2) - (Obstacles(i).pos.x + Obstacles(i).Width + 2, Obstacles(i).pos.y + Obstacles(i).Height + 2), vbWhite, B);
            //        }
            //    }
            //}
        }

        public void DrawPolarIceMaze()
        {
            var blockWidth = (double)SimOpt.SimOpts.FieldWidth / 2;
            var blockHeight = (double)SimOpt.SimOpts.FieldHeight / 2;

            for (var i = 0; i < 8; i++)
            {
                NewObstacle(blockWidth / 2, blockHeight / 2, blockWidth, blockHeight);
            }

            SimOpt.SimOpts.AllowHorizontalShapeDrift = true;
            SimOpt.SimOpts.AllowVerticalShapeDrift = true;
            SimOpt.SimOpts.ShapeDriftRate = 20;
        }

        public void DrawSpiral()
        {
            var numOfHorizontalLines = (int)((double)SimOpt.SimOpts.FieldHeight / (MazeCorridorWidth + MazeWallThickness)) - 1;
            var numOfVerticalLines = (int)((double)SimOpt.SimOpts.FieldWidth / (MazeCorridorWidth + MazeWallThickness)) - 1;
            var numOfLines = Math.Min(numOfHorizontalLines, numOfVerticalLines);

            if (numOfLines % 2 != 0)
                numOfLines--;

            for (var i = 1; i < numOfLines / 2; i++)
            {
                NewObstacle((i - 1) * MazeCorridorWidth, i * MazeCorridorWidth, SimOpt.SimOpts.FieldWidth - MazeCorridorWidth * (2 * (i - 1) + 1), MazeWallThickness);
                NewObstacle(i * MazeCorridorWidth, SimOpt.SimOpts.FieldHeight - MazeCorridorWidth * i, SimOpt.SimOpts.FieldWidth - (MazeCorridorWidth * 2 * i - MazeWallThickness), MazeWallThickness);
                NewObstacle(SimOpt.SimOpts.FieldWidth - MazeCorridorWidth * i, i * MazeCorridorWidth, MazeWallThickness, SimOpt.SimOpts.FieldHeight - MazeCorridorWidth * 2 * i);
                NewObstacle(i * MazeCorridorWidth, (i + 1) * MazeCorridorWidth, MazeWallThickness, SimOpt.SimOpts.FieldHeight - MazeCorridorWidth * (2 * i + 1));
            }
        }

        public void DrawVerticalMaze()
        {
            var numOfLines = (int)((double)SimOpt.SimOpts.FieldHeight / (MazeCorridorWidth + MazeWallThickness)) - 1;
            for (var i = 1; i < numOfLines; i++)
            {
                var opening = ThreadSafeRandom.Local.Next(0, SimOpt.SimOpts.FieldWidth - MazeCorridorWidth);
                NewObstacle(-100, i * (MazeCorridorWidth + MazeWallThickness), opening, MazeWallThickness);
                if (opening + MazeCorridorWidth < SimOpt.SimOpts.FieldWidth + 100)
                    NewObstacle(opening + MazeCorridorWidth, i * (MazeCorridorWidth + MazeWallThickness), SimOpt.SimOpts.FieldWidth + 100 - opening - MazeCorridorWidth, MazeWallThickness);
            }
        }

        public void InitObstacles()
        {
            Obstacles.Clear();
        }

        public void InitTrashCompactorMaze()
        {
            const double blockWidth = 1000.0;
            var blockHeight = SimOpt.SimOpts.FieldHeight * 1.2;

            LeftCompactor = NewObstacle(-blockWidth + 1, SimOpt.SimOpts.FieldHeight * -0.1, blockWidth, blockHeight);
            RightCompactor = NewObstacle(SimOpt.SimOpts.FieldWidth - 1, SimOpt.SimOpts.FieldHeight * -0.1, blockWidth, blockHeight);

            LeftCompactor.Velocity = new DoubleVector(SimOpt.SimOpts.ShapeDriftRate * 0.1, 0);
            RightCompactor.Velocity = LeftCompactor.Velocity.InvertX();
        }

        public void MoveObstacles()
        {
            if (SimOpt.SimOpts.AllowHorizontalShapeDrift || SimOpt.SimOpts.AllowVerticalShapeDrift)
                DriftObstacles();

            if (LeftCompactor != null || RightCompactor != null)
                TrashCompactorMove();

            foreach (var o in Obstacles.Where(o => o.Exist))
            {
                o.Position += o.Velocity;

                if (o.Position.X < -o.Width)
                {
                    o.Position = new DoubleVector(-o.Width, o.Position.Y);
                    o.Velocity = new DoubleVector(SimOpt.SimOpts.ShapeDriftRate * 0.01, o.Velocity.Y);
                }
                if (o.Position.Y < -o.Height)
                {
                    o.Position = new DoubleVector(o.Position.X, -o.Height);
                    o.Velocity = new DoubleVector(o.Velocity.X, SimOpt.SimOpts.ShapeDriftRate * 0.01);
                }
                if (o.Position.X > SimOpt.SimOpts.FieldWidth)
                {
                    o.Position = new DoubleVector(SimOpt.SimOpts.FieldWidth, o.Position.Y);
                    o.Velocity = new DoubleVector(-SimOpt.SimOpts.ShapeDriftRate * 0.01, o.Velocity.Y);
                }
                if (o.Position.Y > SimOpt.SimOpts.FieldHeight)
                {
                    o.Position = new DoubleVector(o.Position.X, SimOpt.SimOpts.FieldHeight);
                    o.Velocity = new DoubleVector(o.Velocity.X, -SimOpt.SimOpts.ShapeDriftRate * 0.01);
                }
            }
        }

        public Obstacle NewObstacle(double x, double y, double width, double height)
        {
            if (Obstacles.Count >= MaxObstacles)
                return null;

            var obstacle = new Obstacle
            {
                Exist = true,
                Position = new DoubleVector(x, y),
                Width = width,
                Height = height,
                Velocity = new DoubleVector(0, 0),
                Color = SimOpt.SimOpts.MakeAllShapesBlack
                    ? Colors.Black
                    : Color.FromRgb((byte)ThreadSafeRandom.Local.Next(0, 256),
                        (byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256))
            };

            return obstacle;
        }

        public void StopAllHorizontalObstacleMovement()
        {
            foreach (var o in Obstacles.Where(o => o.Exist))
                o.Velocity = new DoubleVector(0, o.Velocity.Y);
        }

        public void StopAllVerticalObstacleMovement()
        {
            foreach (var o in Obstacles.Where(o => o.Exist))
                o.Velocity = new DoubleVector(o.Velocity.X, 0);
        }

        public Obstacle WhichObstacle(double x, double y)
        {
            return Obstacles.FirstOrDefault(o => o.Exist && x >= o.Position.X && x <= o.Position.X + o.Width && y >= o.Position.Y && y <= o.Position.Y + o.Height);
        }

        private static bool ObstacleCollision(robot rob, Obstacle o)
        {
            var botRightEdge = rob.pos.X + rob.Radius;
            var botLeftEdge = rob.pos.X - rob.Radius;
            var botTopEdge = rob.pos.Y - rob.Radius;
            var botBottomEdge = rob.pos.Y + rob.Radius;

            return botRightEdge > o.Position.X && botLeftEdge < o.Position.X + o.Width && botBottomEdge > o.Position.Y && botTopEdge < o.Position.Y + o.Height;
        }

        private void DriftObstacles()
        {
            foreach (var o in Obstacles.Where(o => o.Exist && o != LeftCompactor && o != RightCompactor))
            {
                if (SimOpt.SimOpts.AllowHorizontalShapeDrift)
                    o.Velocity += new DoubleVector(ThreadSafeRandom.Local.Next(-SimOpt.SimOpts.ShapeDriftRate, SimOpt.SimOpts.ShapeDriftRate) * ThreadSafeRandom.Local.NextDouble() * 0.01, 0);

                if (SimOpt.SimOpts.AllowVerticalShapeDrift)
                    o.Velocity += new DoubleVector(0, ThreadSafeRandom.Local.Next(-SimOpt.SimOpts.ShapeDriftRate, SimOpt.SimOpts.ShapeDriftRate) * ThreadSafeRandom.Local.NextDouble() * 0.01);

                if (o.Velocity.Magnitude() > SimOpt.SimOpts.MaxVelocity)
                    o.Velocity *= o.Velocity.Magnitude() / SimOpt.SimOpts.MaxVelocity;
            }
        }

        private void TrashCompactorMove()
        {
            if (LeftCompactor.Position.X > RightCompactor.Position.X + 400)
            {
                LeftCompactor.Velocity = LeftCompactor.Velocity.InvertX();
                RightCompactor.Velocity = RightCompactor.Velocity.InvertX();
            }
            if (LeftCompactor.Position.X <= -LeftCompactor.Width)
            {
                LeftCompactor.Velocity = new DoubleVector(SimOpt.SimOpts.ShapeDriftRate * 0.1, 0);
                RightCompactor.Velocity = LeftCompactor.Velocity.InvertX();
            }
        }
    }
}
