using Iersera.Model;
using Iersera.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using static Robots;
using static Senses;
using static SimOpt;

internal static class ObstaclesManager
{
    public const int MAXOBSTACLES = 1000;
    public static double defaultHeight { get; set; }
    public static double defaultWidth { get; set; }
    public static Obstacle leftCompactor { get; set; }
    public static int mazeCorridorWidth { get; set; }
    public static int mazeWallThickness { get; set; }
    public static vector mousepos { get; set; }
    public static int obstaclefocus { get; set; }
    public static List<Obstacle> Obstacles { get; set; } = new();
    public static Obstacle rightCompactor { get; set; }

    public static void AddRandomObstacles(int n)
    {
        if (n < 1)
            return;

        for (var i = 0; i < n; i++)
        {
            var randomX = ThreadSafeRandom.Local.NextDouble() * SimOpts.FieldWidth;
            var randomy = ThreadSafeRandom.Local.NextDouble() * SimOpts.FieldHeight;

            var RandomWidth = ThreadSafeRandom.Local.NextDouble() * SimOpts.FieldWidth * defaultWidth;
            var RandomHeight = ThreadSafeRandom.Local.NextDouble() * SimOpts.FieldHeight * defaultHeight;

            //Shift everything up and left by half the max dimensions then trim to more evenly distribute obstacles across the field
            randomX -= SimOpts.FieldWidth * (defaultWidth / 2);
            randomy -= SimOpts.FieldHeight * (defaultHeight / 2);

            if (randomX < 0)
                randomX = 0;
            if (randomy < 0)
                randomy = 0;

            if (randomX + RandomWidth > SimOpts.FieldWidth)
                RandomWidth = SimOpts.FieldWidth - randomX;
            if (randomy + RandomHeight > SimOpts.FieldHeight)
                RandomHeight = SimOpts.FieldHeight - randomy;
            var res = NewObstacle(randomX, randomy, RandomWidth, RandomHeight);

            if (res == null)
                break;
        }
    }

    public static void ChangeAllObstacleColor(Color? color)
    {
        foreach (var o in Obstacles)
            o.color = color ?? Color.FromRgb((byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256));
    }

    public static void DeleteAllObstacles()
    {
        foreach (var o in Obstacles.ToArray())
        {
            o.exist = false;
            Obstacles.Remove(o);
        }
    }

    public static void DeleteObstacle(Obstacle o)
    {
        o.exist = false;
        Obstacles.Remove(o);
    }

    public static void DeleteTenRandomObstacles()
    {
        var numToDelete = Math.Min(Obstacles.Count, 10);

        for (var i = 0; i > numToDelete; i++)
            DeleteObstacle(Obstacles[ThreadSafeRandom.Local.Next(0, Obstacles.Count)]);
    }

    public static void DoObstacleCollisions(robot rob)
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
                rob.pos.X += 200 * Math.Sign((SimOpts.TotRunCycle % 40) - 20);
                rob.pos.Y += 200 * Math.Sign((SimOpts.TotRunCycle % 50) - 25);
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
                        rob.pos.X = o.pos.X - rob.radius;
                        rob.ImpulseRes.X += rob.vel.X * b;
                        Touch(rob, rob.pos.X + rob.radius, rob.pos.Y); // Update hit senses, right side
                    }
                    else
                    {
                        rob.ImpulseRes.X += distleft * k;
                        rob.pos.X = o.pos.X - rob.radius;
                    }
                    lastPush = 1;
                }
                else
                {
                    if (rob.pos.X + rob.radius > o.pos.X + o.Width)
                    {
                        rob.pos.X = o.pos.X + o.Width + rob.radius;
                        rob.ImpulseRes.X += rob.vel.X * b;
                        Touch(rob, rob.pos.X - rob.radius, rob.pos.Y); // Update hit senses, left side
                    }
                    else
                    {
                        rob.ImpulseRes.X -= distright * k;
                        rob.pos.X = o.pos.X + o.Width + rob.radius;
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
                        rob.pos.Y = o.pos.Y - rob.radius;
                        rob.ImpulseRes.Y += rob.vel.Y * b;
                        Touch(rob, rob.pos.X, rob.pos.Y + rob.radius); // Update hit senses, bottom
                    }
                    else
                    {
                        rob.ImpulseRes.Y += distup * k;
                        rob.pos.Y = o.pos.Y - rob.radius;
                    }
                    lastPush = 3;
                }
                else
                {
                    if (rob.pos.Y + rob.radius > o.pos.Y + o.Height)
                    {
                        rob.pos.Y = o.pos.Y + o.Height + rob.radius;
                        rob.ImpulseRes.Y += rob.vel.Y * b;
                        Touch(rob, rob.pos.X, rob.pos.Y - rob.radius); // Update hit senses, bottom
                    }
                    else
                    {
                        rob.ImpulseRes.Y -= distdown * k;
                        rob.pos.Y = o.pos.Y + o.Height + rob.radius;
                    }

                    lastPush = 4;
                }
            }

            //Botsareus 12/3/2013 If robot sees nothing and touch a shape update reftype
            if (lastPush > 0 & rob.mem[EYEF] == 0)
                rob.mem[REFTYPE] = 1;
        }
    }

    public static void DoShotObstacleCollisions(Shot shot)
    {
        foreach (var o in Obstacles.Where(o => o.exist).Where(o => shot.pos.X >= o.pos.X && shot.pos.X <= o.pos.X + o.Width && shot.pos.Y >= o.pos.Y && shot.pos.Y <= o.pos.Y + o.Height))
        {
            if (SimOpts.ShapesAbsorbShots)
            {
                shot.exist = false;
                ShotsManager.Shots.Remove(shot);
            }

            if (shot.opos.X < o.pos.X || shot.opos.X > (o.pos.X + o.Width))
                shot.velocity.X = -shot.velocity.X;

            if (shot.opos.Y < o.pos.Y || shot.opos.Y > (o.pos.Y + o.Height))
                shot.velocity.Y = -shot.velocity.Y;
        }
    }

    public static void DrawCheckerboardMaze()
    {
        var blockWidth = Math.Min(5000, (double)SimOpts.FieldWidth / 10);

        var numBlocksAcross = (int)(SimOpts.FieldWidth / (blockWidth + mazeCorridorWidth));
        var acrossGap = (numBlocksAcross * (blockWidth + mazeCorridorWidth) + mazeCorridorWidth - SimOpts.FieldWidth) / 2;
        var numBlocksDown = (int)(SimOpts.FieldHeight / (blockWidth + mazeCorridorWidth));
        var downGap = (numBlocksDown * (blockWidth + mazeCorridorWidth) + mazeCorridorWidth - SimOpts.FieldHeight) / 2;

        for (var i = 0; i < numBlocksAcross - 1; i++)
        {
            for (var j = 0; j < numBlocksDown - 1; j++)
            {
                var x = (i * blockWidth) + (i + 1) * mazeCorridorWidth - acrossGap;
                var y = (j * blockWidth) + (j + 1) * mazeCorridorWidth - downGap;
                NewObstacle(x, y, blockWidth, blockWidth);
            }
        }
    }

    public static void DrawHorizontalMaze()
    {
        var numOfLines = (int)((double)SimOpts.FieldWidth / (mazeCorridorWidth + mazeWallThickness)) - 1;
        for (var i = 1; i < numOfLines; i++)
        {
            var Opening = ThreadSafeRandom.Local.Next(0, SimOpts.FieldHeight - mazeCorridorWidth);
            NewObstacle(i * mazeCorridorWidth + mazeWallThickness, -100, mazeWallThickness, Opening);
            if ((Opening + mazeCorridorWidth) < SimOpts.FieldHeight + 100)
            {
                NewObstacle(i * (mazeCorridorWidth + mazeWallThickness), Opening + mazeCorridorWidth, mazeWallThickness, SimOpts.FieldHeight + 100 - Opening - mazeCorridorWidth);
            }
        }
    }

    public static void DrawObstacles()
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

    public static void DrawPolarIceMaze()
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

    public static void DrawSpiral()
    {
        var numOfHorzLines = (int)((double)SimOpts.FieldHeight / (mazeCorridorWidth + mazeWallThickness)) - 1;
        var numOfVertLines = (int)((double)SimOpts.FieldWidth / (mazeCorridorWidth + mazeWallThickness)) - 1;
        var numOfLines = Math.Min(numOfHorzLines, numOfVertLines);

        if ((numOfLines % 2) != 0)
            numOfLines--;

        for (var i = 1; i < (numOfLines / 2); i++)
        {
            NewObstacle((i - 1) * mazeCorridorWidth, i * mazeCorridorWidth, SimOpts.FieldWidth - (mazeCorridorWidth * (2 * (i - 1) + 1)), mazeWallThickness);
            NewObstacle(i * mazeCorridorWidth, SimOpts.FieldHeight - (mazeCorridorWidth * i), SimOpts.FieldWidth - (mazeCorridorWidth * 2 * i - mazeWallThickness), mazeWallThickness);
            NewObstacle(SimOpts.FieldWidth - (mazeCorridorWidth * i), i * mazeCorridorWidth, mazeWallThickness, SimOpts.FieldHeight - mazeCorridorWidth * 2 * i);
            NewObstacle(i * mazeCorridorWidth, (i + 1) * mazeCorridorWidth, mazeWallThickness, SimOpts.FieldHeight - (mazeCorridorWidth * (2 * i + 1)));
        }
    }

    public static void DrawVerticalMaze()
    {
        var numOfLines = (int)((double)SimOpts.FieldHeight / (mazeCorridorWidth + mazeWallThickness)) - 1;
        for (var i = 1; i < numOfLines; i++)
        {
            var Opening = ThreadSafeRandom.Local.Next(0, SimOpts.FieldWidth - mazeCorridorWidth);
            NewObstacle(-100, i * (mazeCorridorWidth + mazeWallThickness), Opening, mazeWallThickness);
            if ((Opening + mazeCorridorWidth) < SimOpts.FieldWidth + 100)
                NewObstacle(Opening + mazeCorridorWidth, i * (mazeCorridorWidth + mazeWallThickness), SimOpts.FieldWidth + 100 - Opening - mazeCorridorWidth, mazeWallThickness);
        }
    }

    public static void DriftObstacles()
    {
        foreach (var o in Obstacles.Where(o => o.exist && o != leftCompactor && o != rightCompactor))
        {
            if (SimOpts.AllowHorizontalShapeDrift)
                o.vel.X += ThreadSafeRandom.Local.Next(-SimOpts.ShapeDriftRate, SimOpts.ShapeDriftRate) * ThreadSafeRandom.Local.NextDouble() * 0.01;

            if (SimOpts.AllowVerticalShapeDrift)
                o.vel.Y += ThreadSafeRandom.Local.Next(-SimOpts.ShapeDriftRate, SimOpts.ShapeDriftRate) * ThreadSafeRandom.Local.NextDouble() * 0.01;

            if (o.vel.Magnitude() > SimOpts.MaxVelocity)
                o.vel *= o.vel.Magnitude() / SimOpts.MaxVelocity;
        }
    }

    public static void InitObstacles()
    {
        Obstacles.Clear();
    }

    public static void InitTrashCompactorMaze()
    {
        var blockWidth = 1000.0;
        var blockHeight = SimOpts.FieldHeight * 1.2;

        leftCompactor = NewObstacle(-blockWidth + 1, SimOpts.FieldHeight * -0.1, blockWidth, blockHeight);
        rightCompactor = NewObstacle(SimOpts.FieldWidth - 1, SimOpts.FieldHeight * -0.1, blockWidth, blockHeight);

        leftCompactor.vel.X = SimOpts.ShapeDriftRate * 0.1;
        rightCompactor.vel.X = -SimOpts.ShapeDriftRate * 0.1;
    }

    public static void MoveObstacles()
    {
        if (SimOpts.AllowHorizontalShapeDrift || SimOpts.AllowVerticalShapeDrift)
            DriftObstacles();

        if (leftCompactor != null || rightCompactor != null)
            TrashCompactorMove();

        foreach (var o in Obstacles.Where(o => o.exist))
        {
            o.pos += o.vel;

            if (o.pos.X < -o.Width)
            {
                o.pos.X = -o.Width;
                o.vel.X = SimOpts.ShapeDriftRate * 0.01;
            }
            if (o.pos.Y < -o.Height)
            {
                o.pos.Y = -o.Height;
                o.vel.Y = SimOpts.ShapeDriftRate * 0.01;
            }
            if (o.pos.X > SimOpts.FieldWidth)
            {
                o.pos.X = SimOpts.FieldWidth;
                o.vel.X = -SimOpts.ShapeDriftRate * 0.01;
            }
            if (o.pos.Y > SimOpts.FieldHeight)
            {
                o.pos.Y = SimOpts.FieldHeight;
                o.vel.Y = -SimOpts.ShapeDriftRate * 0.01;
            }
        }
    }

    public static Obstacle NewObstacle(double x, double y, double Width, double Height)
    {
        if (Obstacles.Count >= MAXOBSTACLES)
            return null;
        else
        {
            var obstacle = new Obstacle
            {
                exist = true,
                pos = new vector(x, y),
                Width = Width,
                Height = Height,
                vel = new vector(0, 0)
            };

            obstacle.color = SimOpts.MakeAllShapesBlack
                ? Colors.Black
                : Color.FromRgb((byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256), (byte)ThreadSafeRandom.Local.Next(0, 256));

            return obstacle;
        }
    }

    public static bool ObstacleCollision(robot rob, Obstacle o)
    {
        var botrightedge = rob.pos.X + rob.radius;
        var botleftedge = rob.pos.X - rob.radius;
        var bottopedge = rob.pos.Y - rob.radius;
        var botbottomedge = rob.pos.Y + rob.radius;

        return botrightedge > o.pos.X && (botleftedge < o.pos.X + o.Width) && (botbottomedge > o.pos.Y) && (bottopedge < o.pos.Y + o.Height);
    }

    public static void StopAllHorizontalObstacleMovement()
    {
        foreach (var o in Obstacles.Where(o => o.exist))
            o.vel.X = 0;
    }

    public static void StopAllVerticalObstacleMovement()
    {
        foreach (var o in Obstacles.Where(o => o.exist))
            o.vel.Y = 0;
    }

    public static void TrashCompactorMove()
    {
        if (leftCompactor.pos.X > rightCompactor.pos.X + 400)
        {
            leftCompactor.vel.X = -leftCompactor.vel.X;
            rightCompactor.vel.X = -rightCompactor.vel.X;
        }
        if (leftCompactor.pos.X <= -leftCompactor.Width)
        {
            leftCompactor.vel.X = SimOpts.ShapeDriftRate * 0.1;
            rightCompactor.vel.X = -SimOpts.ShapeDriftRate * 0.1;
        }
    }

    public static Obstacle WhichObstacle(double x, double y)
    {
        return Obstacles.FirstOrDefault(o => o.exist && x >= o.pos.X && x <= o.pos.X + o.Width && y >= o.pos.Y && y <= o.pos.Y + o.Height);
    }
}
