using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using static DarwinBots.Modules.Robots;
using static DarwinBots.Modules.Senses;
using static DarwinBots.Modules.SimOpt;

namespace DarwinBots.Modules
{
    internal class ObstaclesManager
    {
        private const int MaxObstacles = 1000;
        public double DefaultHeight { get; set; }
        public double DefaultWidth { get; set; }
        public List<Obstacle> Obstacles { get; } = new();
        private Obstacle LeftCompactor { get; set; }
        private int MazeCorridorWidth { get; set; }
        private int MazeWallThickness { get; set; }
        private Obstacle RightCompactor { get; set; }

        public static bool ObstacleCollision(robot rob, Obstacle o)
        {
            var botrightedge = rob.pos.X + rob.radius;
            var botleftedge = rob.pos.X - rob.radius;
            var bottopedge = rob.pos.Y - rob.radius;
            var botbottomedge = rob.pos.Y + rob.radius;

            return botrightedge > o.pos.X && (botleftedge < o.pos.X + o.Width) && (botbottomedge > o.pos.Y) && (bottopedge < o.pos.Y + o.Height);
        }

        public void AddRandomObstacles(int n)
        {
            if (n < 1)
                return;

            for (var i = 0; i < n; i++)
            {
                var randomX = ThreadSafeRandom.Local.NextDouble() * SimOpts.FieldWidth;
                var randomy = ThreadSafeRandom.Local.NextDouble() * SimOpts.FieldHeight;

                var randomWidth = ThreadSafeRandom.Local.NextDouble() * SimOpts.FieldWidth * DefaultWidth;
                var randomHeight = ThreadSafeRandom.Local.NextDouble() * SimOpts.FieldHeight * DefaultHeight;

                //Shift everything up and left by half the max dimensions then trim to more evenly distribute obstacles across the field
                randomX -= SimOpts.FieldWidth * (DefaultWidth / 2);
                randomy -= SimOpts.FieldHeight * (DefaultHeight / 2);

                if (randomX < 0)
                    randomX = 0;
                if (randomy < 0)
                    randomy = 0;

                if (randomX + randomWidth > SimOpts.FieldWidth)
                    randomWidth = SimOpts.FieldWidth - randomX;
                if (randomy + randomHeight > SimOpts.FieldHeight)
                    randomHeight = SimOpts.FieldHeight - randomy;
                var res = NewObstacle(randomX, randomy, randomWidth, randomHeight);

                if (res == null)
                    break;
            }
        }

        public void ChangeAllObstacleColor(Color? color)
        {
            foreach (var o in Obstacles)
                o.color = color ?? Color.FromRgb((byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256));
        }

        public void DeleteAllObstacles()
        {
            foreach (var o in Obstacles.ToArray())
            {
                o.exist = false;
                Obstacles.Remove(o);
            }
        }

        public void DeleteObstacle(Obstacle o)
        {
            o.exist = false;
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

            foreach (var o in Obstacles.Where(o => o.exist && ObstacleCollision(rob, o)))
            {
                numofcollisions++;
                if (numofcollisions >= 3)
                {
                    // Prevents getting trapped
                    rob.pos = new DoubleVector(200 * Math.Sign((SimOpts.TotRunCycle % 40) - 20), 200 * Math.Sign((SimOpts.TotRunCycle % 50) - 25));
                    return;
                }

                //Push the bot the closest edge
                var distup = rob.pos.Y + rob.radius - o.pos.Y;
                var distdown = o.pos.Y + o.Height - (rob.pos.Y - rob.radius);
                var distleft = rob.pos.X + rob.radius - o.pos.X;
                var distright = o.pos.X + o.Width - (rob.pos.X - rob.radius);

                if ((Math.Min(distleft, distright) < Math.Min(distup, distdown) && lastPush != 1 && lastPush != 2) || lastPush == 3 || lastPush == 4)
                {
                    //Push left or right
                    if (((distleft <= distright) || (o.pos.X + o.Width) >= SimOpts.FieldWidth) && (o.pos.X > 0))
                    {
                        if (rob.pos.X - rob.radius < o.pos.X)
                        {
                            rob.pos = rob.pos with { X = o.pos.X - rob.radius };
                            rob.ImpulseRes += new DoubleVector(rob.vel.X * b, 0);
                            Touch(rob, rob.pos.X + rob.radius, rob.pos.Y); // Update hit senses, right side
                        }
                        else
                        {
                            rob.ImpulseRes += new DoubleVector(distleft * k, 0);
                            rob.pos = rob.pos with { X = o.pos.X - rob.radius };
                        }
                        lastPush = 1;
                    }
                    else
                    {
                        if (rob.pos.X + rob.radius > o.pos.X + o.Width)
                        {
                            rob.pos = rob.pos with { X = o.pos.X + o.Width + rob.radius };
                            rob.ImpulseRes += new DoubleVector(rob.vel.X * b, 0);
                            Touch(rob, rob.pos.X - rob.radius, rob.pos.Y); // Update hit senses, left side
                        }
                        else
                        {
                            rob.ImpulseRes -= new DoubleVector(distright * k, 0);
                            rob.pos = rob.pos with { X = o.pos.X + o.Width + rob.radius };
                        }
                        lastPush = 2;
                    }
                }
                else
                {
                    //Push up or down
                    if (((distup <= distdown) || (o.pos.Y + o.Height) >= SimOpts.FieldHeight) && (o.pos.Y > 0))
                    {
                        if (rob.pos.Y - rob.radius < o.pos.Y)
                        {
                            rob.pos = rob.pos with { Y = o.pos.Y - rob.radius };
                            rob.ImpulseRes += new DoubleVector(0, rob.vel.Y * b);
                            Touch(rob, rob.pos.X, rob.pos.Y + rob.radius); // Update hit senses, bottom
                        }
                        else
                        {
                            rob.ImpulseRes += new DoubleVector(0, distup * k);
                            rob.pos = rob.pos with { Y = o.pos.Y - rob.radius };
                        }
                        lastPush = 3;
                    }
                    else
                    {
                        if (rob.pos.Y + rob.radius > o.pos.Y + o.Height)
                        {
                            rob.pos = rob.pos with { Y = o.pos.Y + o.Height + rob.radius };
                            rob.ImpulseRes += new DoubleVector(0, rob.vel.Y * b);
                            Touch(rob, rob.pos.X, rob.pos.Y - rob.radius); // Update hit senses, bottom
                        }
                        else
                        {
                            rob.ImpulseRes -= new DoubleVector(0, distdown * k);
                            rob.pos = rob.pos with { Y = o.pos.Y + o.Height + rob.radius };
                        }

                        lastPush = 4;
                    }
                }

                if (rob.mem[EYEF] == 0)
                    rob.mem[REFTYPE] = 1;
            }
        }

        public void DoShotObstacleCollisions(Shot shot)
        {
            foreach (var o in Obstacles.Where(o => o.exist).Where(o => shot.pos.X >= o.pos.X && shot.pos.X <= o.pos.X + o.Width && shot.pos.Y >= o.pos.Y && shot.pos.Y <= o.pos.Y + o.Height))
            {
                if (SimOpts.ShapesAbsorbShots)
                {
                    shot.exist = false;
                    ShotsManager.Shots.Remove(shot);
                }

                if (shot.opos.X < o.pos.X || shot.opos.X > (o.pos.X + o.Width))
                    shot.velocity = shot.velocity with { X = -shot.velocity.X };

                if (shot.opos.Y < o.pos.Y || shot.opos.Y > (o.pos.Y + o.Height))
                    shot.velocity = shot.velocity with { Y = -shot.velocity.Y };
            }
        }

        public void DrawCheckerboardMaze()
        {
            var blockWidth = Math.Min(5000, (double)SimOpts.FieldWidth / 10);

            var numBlocksAcross = (int)(SimOpts.FieldWidth / (blockWidth + MazeCorridorWidth));
            var acrossGap = (numBlocksAcross * (blockWidth + MazeCorridorWidth) + MazeCorridorWidth - SimOpts.FieldWidth) / 2;
            var numBlocksDown = (int)(SimOpts.FieldHeight / (blockWidth + MazeCorridorWidth));
            var downGap = (numBlocksDown * (blockWidth + MazeCorridorWidth) + MazeCorridorWidth - SimOpts.FieldHeight) / 2;

            for (var i = 0; i < numBlocksAcross - 1; i++)
            {
                for (var j = 0; j < numBlocksDown - 1; j++)
                {
                    var x = (i * blockWidth) + (i + 1) * MazeCorridorWidth - acrossGap;
                    var y = (j * blockWidth) + (j + 1) * MazeCorridorWidth - downGap;
                    NewObstacle(x, y, blockWidth, blockWidth);
                }
            }
        }

        public void DrawHorizontalMaze()
        {
            var numOfLines = (int)((double)SimOpts.FieldWidth / (MazeCorridorWidth + MazeWallThickness)) - 1;
            for (var i = 1; i < numOfLines; i++)
            {
                var Opening = ThreadSafeRandom.Local.Next(0, SimOpts.FieldHeight - MazeCorridorWidth);
                NewObstacle(i * MazeCorridorWidth + MazeWallThickness, -100, MazeWallThickness, Opening);
                if ((Opening + MazeCorridorWidth) < SimOpts.FieldHeight + 100)
                {
                    NewObstacle(i * (MazeCorridorWidth + MazeWallThickness), Opening + MazeCorridorWidth, MazeWallThickness, SimOpts.FieldHeight + 100 - Opening - MazeCorridorWidth);
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
            var blockWidth = (double)SimOpts.FieldWidth / 2;
            var blockHeight = (double)SimOpts.FieldHeight / 2;

            for (var i = 0; i < 8; i++)
            {
                NewObstacle(blockWidth / 2, blockHeight / 2, blockWidth, blockHeight);
            }

            SimOpts.AllowHorizontalShapeDrift = true;
            SimOpts.AllowVerticalShapeDrift = true;
            SimOpts.ShapeDriftRate = 20;
        }

        public void DrawSpiral()
        {
            var numOfHorzLines = (int)((double)SimOpts.FieldHeight / (MazeCorridorWidth + MazeWallThickness)) - 1;
            var numOfVertLines = (int)((double)SimOpts.FieldWidth / (MazeCorridorWidth + MazeWallThickness)) - 1;
            var numOfLines = Math.Min(numOfHorzLines, numOfVertLines);

            if ((numOfLines % 2) != 0)
                numOfLines--;

            for (var i = 1; i < (numOfLines / 2); i++)
            {
                NewObstacle((i - 1) * MazeCorridorWidth, i * MazeCorridorWidth, SimOpts.FieldWidth - (MazeCorridorWidth * (2 * (i - 1) + 1)), MazeWallThickness);
                NewObstacle(i * MazeCorridorWidth, SimOpts.FieldHeight - (MazeCorridorWidth * i), SimOpts.FieldWidth - (MazeCorridorWidth * 2 * i - MazeWallThickness), MazeWallThickness);
                NewObstacle(SimOpts.FieldWidth - (MazeCorridorWidth * i), i * MazeCorridorWidth, MazeWallThickness, SimOpts.FieldHeight - MazeCorridorWidth * 2 * i);
                NewObstacle(i * MazeCorridorWidth, (i + 1) * MazeCorridorWidth, MazeWallThickness, SimOpts.FieldHeight - (MazeCorridorWidth * (2 * i + 1)));
            }
        }

        public void DrawVerticalMaze()
        {
            var numOfLines = (int)((double)SimOpts.FieldHeight / (MazeCorridorWidth + MazeWallThickness)) - 1;
            for (var i = 1; i < numOfLines; i++)
            {
                var Opening = ThreadSafeRandom.Local.Next(0, SimOpts.FieldWidth - MazeCorridorWidth);
                NewObstacle(-100, i * (MazeCorridorWidth + MazeWallThickness), Opening, MazeWallThickness);
                if ((Opening + MazeCorridorWidth) < SimOpts.FieldWidth + 100)
                    NewObstacle(Opening + MazeCorridorWidth, i * (MazeCorridorWidth + MazeWallThickness), SimOpts.FieldWidth + 100 - Opening - MazeCorridorWidth, MazeWallThickness);
            }
        }

        public void DriftObstacles()
        {
            foreach (var o in Obstacles.Where(o => o.exist && o != LeftCompactor && o != RightCompactor))
            {
                if (SimOpts.AllowHorizontalShapeDrift)
                    o.vel += new DoubleVector(ThreadSafeRandom.Local.Next(-SimOpts.ShapeDriftRate, SimOpts.ShapeDriftRate) * ThreadSafeRandom.Local.NextDouble() * 0.01, 0);

                if (SimOpts.AllowVerticalShapeDrift)
                    o.vel += new DoubleVector(0, ThreadSafeRandom.Local.Next(-SimOpts.ShapeDriftRate, SimOpts.ShapeDriftRate) * ThreadSafeRandom.Local.NextDouble() * 0.01);

                if (o.vel.Magnitude() > SimOpts.MaxVelocity)
                    o.vel *= o.vel.Magnitude() / SimOpts.MaxVelocity;
            }
        }

        public void InitObstacles()
        {
            Obstacles.Clear();
        }

        public void InitTrashCompactorMaze()
        {
            var blockWidth = 1000.0;
            var blockHeight = SimOpts.FieldHeight * 1.2;

            LeftCompactor = NewObstacle(-blockWidth + 1, SimOpts.FieldHeight * -0.1, blockWidth, blockHeight);
            RightCompactor = NewObstacle(SimOpts.FieldWidth - 1, SimOpts.FieldHeight * -0.1, blockWidth, blockHeight);

            LeftCompactor.vel = LeftCompactor.vel with { X = SimOpts.ShapeDriftRate * 0.1 };
            RightCompactor.vel = RightCompactor.vel with { X = -SimOpts.ShapeDriftRate * 0.1 };
        }

        public void MoveObstacles()
        {
            if (SimOpts.AllowHorizontalShapeDrift || SimOpts.AllowVerticalShapeDrift)
                DriftObstacles();

            if (LeftCompactor != null || RightCompactor != null)
                TrashCompactorMove();

            foreach (var o in Obstacles.Where(o => o.exist))
            {
                o.pos += o.vel;

                if (o.pos.X < -o.Width)
                {
                    o.pos = o.pos with { X = -o.Width };
                    o.vel = o.vel with { X = SimOpts.ShapeDriftRate * 0.01 };
                }
                if (o.pos.Y < -o.Height)
                {
                    o.pos = o.pos with { Y = -o.Height };
                    o.vel = o.vel with { Y = SimOpts.ShapeDriftRate * 0.01 };
                }
                if (o.pos.X > SimOpts.FieldWidth)
                {
                    o.pos = o.pos with { X = SimOpts.FieldWidth };
                    o.vel = o.vel with { X = -SimOpts.ShapeDriftRate * 0.01 };
                }
                if (o.pos.Y > SimOpts.FieldHeight)
                {
                    o.pos = o.pos with { Y = SimOpts.FieldHeight };
                    o.vel = o.vel with { Y = -SimOpts.ShapeDriftRate * 0.01 };
                }
            }
        }

        public Obstacle NewObstacle(double x, double y, double Width, double Height)
        {
            if (Obstacles.Count >= MaxObstacles)
                return null;
            else
            {
                var obstacle = new Obstacle
                {
                    exist = true,
                    pos = new DoubleVector(x, y),
                    Width = Width,
                    Height = Height,
                    vel = new DoubleVector(0, 0)
                };

                obstacle.color = SimOpts.MakeAllShapesBlack
                    ? Colors.Black
                    : Color.FromRgb((byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256));

                return obstacle;
            }
        }

        public void StopAllHorizontalObstacleMovement()
        {
            foreach (var o in Obstacles.Where(o => o.exist))
                o.vel = o.vel with { X = 0 };
        }

        public void StopAllVerticalObstacleMovement()
        {
            foreach (var o in Obstacles.Where(o => o.exist))
                o.vel = o.vel with { Y = 0 };
        }

        public void TrashCompactorMove()
        {
            if (LeftCompactor.pos.X > RightCompactor.pos.X + 400)
            {
                LeftCompactor.vel = LeftCompactor.vel.InvertX();
                RightCompactor.vel = RightCompactor.vel.InvertX();
            }
            if (LeftCompactor.pos.X <= -LeftCompactor.Width)
            {
                LeftCompactor.vel = LeftCompactor.vel with { X = SimOpts.ShapeDriftRate * 0.1 };
                RightCompactor.vel = RightCompactor.vel with { X = -SimOpts.ShapeDriftRate * 0.1 };
            }
        }

        public Obstacle WhichObstacle(double x, double y)
        {
            return Obstacles.FirstOrDefault(o => o.exist && x >= o.pos.X && x <= o.pos.X + o.Width && y >= o.pos.Y && y <= o.pos.Y + o.Height);
        }
    }
}
