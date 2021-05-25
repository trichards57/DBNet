using DBNet.Forms;
using System.Collections.Generic;
using static Common;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.VBMath;
using static Robots;
using static Senses;
using static SimOptModule;
using static System.Math;
using static VBConstants;
using static VBExtension;

internal static class Obstacles
{
    public const dynamic MAXOBSTACLES = 1000;

    public static decimal defaultHeight = 0;

    public static decimal defaultWidth = 0;

    public static int leftCompactor = 0;

    public static int mazeCorridorWidth = 0;

    public static int mazeWallThickness = 0;

    public static vector mousepos = null;

    public static int numObstacles = 0;

    public static int obstaclefocus = 0;

    public static List<Obstacle> Obstacles = new List<Obstacle>(new Obstacle[(MAXOBSTACLES + 1)]);

    // TODO: Confirm Array Size By Token
    public static int rightCompactor = 0;

    public static int AddRandomObstacles(int n)
    {
        int AddRandomObstacles = 0;
        int i = 0;

        decimal randomX = 0;

        decimal randomy = 0;

        decimal RandomWidth = 0;

        decimal RandomHeight = 0;

        if (n < 1)
        {
            AddRandomObstacles = -1;
            return AddRandomObstacles;
        }

        i = 0;
        While(i != -1 && n > 0);
        randomX = Rnd * SimOpts.FieldWidth;
        randomy = Rnd * SimOpts.FieldHeight;

        RandomWidth = Rnd * SimOpts.FieldWidth * defaultWidth;
        RandomHeight = Rnd * SimOpts.FieldHeight * defaultHeight;

        //Shift everything up and left by half the max dimensions then trim to more evenly distribute obstacles across the field
        randomX = randomX - SimOpts.FieldWidth * (defaultWidth / 2);
        randomy = randomy - SimOpts.FieldHeight * (defaultHeight / 2);

        if (randomX < 0)
        {
            randomX = 0;
        }
        if (randomy < 0)
        {
            randomy = 0;
        }

        if (randomX + RandomWidth > SimOpts.FieldWidth)
        {
            RandomWidth = SimOpts.FieldWidth - randomX;
        }
        if (randomy + RandomHeight > SimOpts.FieldHeight)
        {
            RandomHeight = SimOpts.FieldHeight - randomy;
        }
        i = NewObstacle(randomX, randomy, RandomWidth, RandomHeight);
        n = n - 1;
        Wend();

        if (i == -1 || n != 0)
        {
            AddRandomObstacles = -1;
        }
        else
        {
            AddRandomObstacles = 0;
        }

        return AddRandomObstacles;
    }

    public static dynamic ChangeAllObstacleColor(int color)
    {
        dynamic ChangeAllObstacleColor = null;
        int i = 0;

        for (i = 1; i < numObstacles; i++)
        {
            if (color < 0)
            {
                Obstacles(i).color = Rnd * 65536 + Rnd * 255 + Rnd; // Random Color
            }
            else
            {
                Obstacles(i).color = color;
            }
        }
        return ChangeAllObstacleColor;
    }

    public static dynamic DeleteAllObstacles()
    {
        dynamic DeleteAllObstacles = null;
        int i = 0;

        for (i = 1; i < numObstacles; i++)
        {
            Obstacles(i).exist = false;
        }
        numObstacles = 0;
        return DeleteAllObstacles;
    }

    public static dynamic DeleteObstacle(int i)
    {
        dynamic DeleteObstacle = null;
        int j = 0;

        if (i < 1 || i > numObstacles || numObstacles == 0)
        {
            return DeleteObstacle;
        }
        for (j = i; j < numObstacles; j++)
        {
            Obstacles(j) = Obstacles(j + 1);
        }
        Obstacles(numObstacles).exist = false;
        numObstacles = numObstacles - 1;

        return DeleteObstacle;
    }

    public static dynamic DeleteTenRandomObstacles()
    {
        dynamic DeleteTenRandomObstacles = null;
        int pos = 0;

        int i = 0;

        if (numObstacles > 0)
        {
            for (i = 1; i < 10; i++)
            {
                DeleteObstacle((Random(1, numObstacles)));
            }
        }

        return DeleteTenRandomObstacles;
    }

    public static dynamic DoObstacleCollisions(int n)
    {
        dynamic DoObstacleCollisions = null;
        int i = 0;

        decimal distleft = 0;

        decimal distright = 0;

        decimal distup = 0;

        decimal distdown = 0;

        int numofcollisions = 0;

        int LastPush = 0;

        decimal k = 0;

        decimal b = 0;

        numofcollisions = 0;
        LastPush = 0;

        k = 0.5m;
        b = 0.5m;

        dynamic _WithVar_5122;
        _WithVar_5122 = rob[n];
        for (i = 1; i < numObstacles; i++)
        {
            if (Obstacles(i).exist)
            {
                if (ObstacleCollision(n, i))
                {
                    numofcollisions = numofcollisions + 1;
                    if (numofcollisions >= 3)
                    {
                        // Prevents getting trapped
                        _WithVar_5122.pos.x = _WithVar_5122.pos.x + 200 * Sgn((SimOpts.TotRunCycle % 40) - 20);
                        _WithVar_5122.pos.y = _WithVar_5122.pos.y + 200 * Sgn((SimOpts.TotRunCycle % 50) - 25);
                        goto ;
                    }
                    //Push the bot the closest edge
                    distup = (rob[n].pos.Y + rob[n].radius) - Obstacles(i).pos.y; //- (rob[n].vel.y / 2)
                    distdown = Obstacles(i).pos.y + Obstacles(i).Height - (rob[n].pos.Y - rob[n].radius); //- (rob[n].vel.y / 2)
                    distleft = (rob[n].pos.X + rob[n].radius) - Obstacles(i).pos.x; //- (rob[n].vel.x / 2)
                    distright = Obstacles(i).pos.x + Obstacles(i).Width - (rob[n].pos.X - rob[n].radius); //- (rob[n].vel.x / 2)

                    if ((Min(distleft, distright) < Min(distup, distdown) && (LastPush != 1 && LastPush != 2)) || (LastPush == 3 || LastPush == 4))
                    {
                        //Push left or right
                        if (((distleft <= distright) || (Obstacles(i).pos.x + Obstacles(i).Width) >= SimOpts.FieldWidth) && (Obstacles(i).pos.x > 0))
                        {
                            if (rob[n].pos.X - rob[n].radius < Obstacles(i).pos.x)
                            {
                                _WithVar_5122.pos.x = Obstacles(i).pos.x - rob[n].radius;
                                _WithVar_5122.ImpulseRes.x = _WithVar_5122.ImpulseRes.x + _WithVar_5122.vel.x * b;
                                touch(n, _WithVar_5122.pos.x + _WithVar_5122.radius, _WithVar_5122.pos.y); // Update hit senses, right side
                            }
                            else
                            {
                                _WithVar_5122.ImpulseRes.x = _WithVar_5122.ImpulseRes.x + distleft * k;
                                //  If .Fixed Then .pos = VectorSub(.pos, .ImpulseRes) ' force .fixed guys to move without changing their fixedness
                                _WithVar_5122.pos.x = Obstacles(i).pos.x - rob[n].radius;
                            }
                            LastPush = 1;
                        }
                        else
                        {
                            if (rob[n].pos.X + rob[n].radius > Obstacles(i).pos.x + Obstacles(i).Width)
                            {
                                _WithVar_5122.pos.x = Obstacles(i).pos.x + Obstacles(i).Width + rob[n].radius;
                                _WithVar_5122.ImpulseRes.x = _WithVar_5122.ImpulseRes.x + _WithVar_5122.vel.x * b;
                                touch(n, _WithVar_5122.pos.x - _WithVar_5122.radius, _WithVar_5122.pos.y); // Update hit senses, left side
                            }
                            else
                            {
                                _WithVar_5122.ImpulseRes.x = _WithVar_5122.ImpulseRes.x - distright * k;
                                //   If .Fixed Then .pos = VectorSub(.pos, .ImpulseRes) ' force .fixed guys to move without changing their fixedness
                                _WithVar_5122.pos.x = Obstacles(i).pos.x + Obstacles(i).Width + rob[n].radius;
                            }
                            LastPush = 2;
                        }
                    }
                    else
                    {
                        //Push up or down
                        if (((distup <= distdown) || (Obstacles(i).pos.y + Obstacles(i).Height) >= SimOpts.FieldHeight) && (Obstacles(i).pos.y > 0))
                        {
                            if (rob[n].pos.Y - rob[n].radius < Obstacles(i).pos.y)
                            {
                                _WithVar_5122.pos.y = Obstacles(i).pos.y - rob[n].radius;
                                _WithVar_5122.ImpulseRes.y = _WithVar_5122.ImpulseRes.y + _WithVar_5122.vel.y * b;
                                touch(n, _WithVar_5122.pos.x, _WithVar_5122.pos.y + _WithVar_5122.radius); // Update hit senses, bottom
                            }
                            else
                            {
                                _WithVar_5122.ImpulseRes.y = _WithVar_5122.ImpulseRes.y + distup * k;
                                //    If .Fixed Then .pos = VectorSub(.pos, .ImpulseRes) ' force .fixed guys to move without changing their fixedness
                                _WithVar_5122.pos.y = Obstacles(i).pos.y - rob[n].radius;
                            }
                            LastPush = 3;
                        }
                        else
                        {
                            if (rob[n].pos.Y + rob[n].radius > Obstacles(i).pos.y + Obstacles(i).Height)
                            {
                                _WithVar_5122.pos.y = Obstacles(i).pos.y + Obstacles(i).Height + rob[n].radius;
                                _WithVar_5122.ImpulseRes.y = _WithVar_5122.ImpulseRes.y + _WithVar_5122.vel.y * b;
                                touch(n, _WithVar_5122.pos.x, _WithVar_5122.pos.y - _WithVar_5122.radius); // Update hit senses, bottom
                            }
                            else
                            {
                                _WithVar_5122.ImpulseRes.y = _WithVar_5122.ImpulseRes.y - distdown * k;
                                //  If .Fixed Then .pos = VectorSub(.pos, .ImpulseRes) ' force .fixed guys to move without changing their fixedness
                                _WithVar_5122.pos.y = Obstacles(i).pos.y + Obstacles(i).Height + rob[n].radius;
                            }

                            LastPush = 4;
                        }
                    }

                    //Botsareus 12/3/2013 If robot sees nothing and touch a shape update reftype
                    if (LastPush > 0 & _WithVar_5122.mem(EYEF) == 0)
                    {
                        _WithVar_5122.mem(REFTYPE) = 1;
                    }

                    // If VectorMagnitude(.ImpulseRes) > VectorMagnitude(.vel) Then
                    //   .ImpulseRes = VectorScalar(.ImpulseRes, (VectorMagnitude(.vel) / VectorMagnitude(.ImpulseRes)) * 0.99)
                    // End If
                }
            }
        }

    // If numofcollisions > 2 Then
    //Give up and just get them of there
    //   .pos.x = Rnd * SimOpts.FieldWidth
    //   .pos.y = Rnd * SimOpts.FieldHeight
    // End If
    //ImpulseRes.y = .ImpulseRes.y - SimOpts.MaxVelocity * (Rnd(1) * -2 + 1) * Rnd(1)
    //    .ImpulseRes.x = .ImpulseRes.x - SimOpts.MaxVelocity * (Rnd(1) * -2 + 1) * Rnd(1)
    //  End If
    getout:

        return DoObstacleCollisions;
    }

    public static dynamic DoShotObstacleCollisions(int n)
    {
        dynamic DoShotObstacleCollisions = null;
        int i = 0;

        dynamic _WithVar_2632;
        _WithVar_2632 = Shots(n);
        for (i = 1; i < numObstacles; i++)
        {
            if (Obstacles(i).exist)
            {
                if (_WithVar_2632.pos.x >= Obstacles(i).pos.x && _WithVar_2632.pos.x <= Obstacles(i).pos.x + Obstacles(i).Width && _WithVar_2632.pos.y >= Obstacles(i).pos.y && _WithVar_2632.pos.y <= Obstacles(i).pos.y + Obstacles(i).Height)
                {
                    if (SimOpts.ShapesAbsorbShots)
                    {
                        _WithVar_2632.exist = false;
                    }
                    if (_WithVar_2632.opos.x < Obstacles(i).pos.x || _WithVar_2632.opos.x > (Obstacles(i).pos.x + Obstacles(i).Width))
                    {
                        _WithVar_2632.velocity.x = -.velocity.x;
                    }
                    if (_WithVar_2632.opos.y < Obstacles(i).pos.y || _WithVar_2632.opos.y > (Obstacles(i).pos.y + Obstacles(i).Height))
                    {
                        _WithVar_2632.velocity.y = -.velocity.y;
                    }
                }
            }
        }
        return DoShotObstacleCollisions;
    }

    public static dynamic DrawCheckerboardMaze()
    {
        dynamic DrawCheckerboardMaze = null;
        int i = 0;

        int j = 0;

        int k = 0;

        decimal x = 0;

        decimal y = 0;

        decimal numBlocksAcross = 0;

        decimal numBlocksDown = 0;

        decimal acrossGap = 0;

        decimal downGap = 0;

        decimal blockWidth = 0;

        blockWidth = Min(5000, SimOpts.FieldWidth / 10);

        numBlocksAcross = Int(SimOpts.FieldWidth / (blockWidth + mazeCorridorWidth));
        acrossGap = (numBlocksAcross * (blockWidth + mazeCorridorWidth) + mazeCorridorWidth - SimOpts.FieldWidth) / 2;
        numBlocksDown = Int(SimOpts.FieldHeight / (blockWidth + mazeCorridorWidth));
        downGap = (numBlocksDown * (blockWidth + mazeCorridorWidth) + mazeCorridorWidth - SimOpts.FieldHeight) / 2;

        for (i = 0; i < numBlocksAcross - 1; i++)
        {
            for (j = 0; j < numBlocksDown - 1; j++)
            {
                x = CSng(i * blockWidth) + CSng(i + 1) * CSng(mazeCorridorWidth) - acrossGap;
                y = CSng(j * blockWidth) + CSng(j + 1) * CSng(mazeCorridorWidth) - downGap;
                k = NewObstacle(x, y, blockWidth, blockWidth);
            }
        }
        //allowHorizontalShapeDrift = True
        // allowVerticalShapeDrift = True
        // obstacleDriftRate = 20

        return DrawCheckerboardMaze;
    }

    public static dynamic DrawHorizontalMaze()
    {
        dynamic DrawHorizontalMaze = null;
        int i = 0;

        int j = 0;

        int numOfLines = 0;

        int Opening = 0;

        numOfLines = CInt(SimOpts.FieldWidth / (mazeCorridorWidth + mazeWallThickness)) - 1;
        for (i = 1; i < numOfLines; i++)
        {
            Opening = Random(0, SimOpts.FieldHeight - mazeCorridorWidth);
            j = NewObstacle(CSng(CSng(i) * CSng(mazeCorridorWidth + mazeWallThickness)), -100, CSng(mazeWallThickness), CSng(Opening));
            if ((Opening + mazeCorridorWidth) < SimOpts.FieldHeight + 100)
            {
                NewObstacle(CSng(CSng(i) * CSng(mazeCorridorWidth + mazeWallThickness)), Opening + CSng(mazeCorridorWidth), CSng(mazeWallThickness), SimOpts.FieldHeight + 100 - Opening - CSng(mazeCorridorWidth));
            }
        }
        return DrawHorizontalMaze;
    }

    public static dynamic DrawObstacles()
    {
        dynamic DrawObstacles = null;
        int i = 0;

        Form1.FillStyle = 1;

        for (i = 1; i < numObstacles; i++)
        {
            if (Obstacles(i).exist)
            {
                if (SimOpts.MakeAllShapesTransparent)
                {
                    //Form1.Line(Obstacles(i).pos.x, Obstacles(i).pos.y) - (Obstacles(i).pos.x + Obstacles(i).Width, Obstacles(i).pos.y + Obstacles(i).Height), Obstacles(i).color, B);
                }
                else
                {
                    //Form1.Line(Obstacles(i).pos.x, Obstacles(i).pos.y)-(Obstacles(i).pos.x + Obstacles(i).Width, Obstacles(i).pos.y + Obstacles(i).Height), Obstacles(i).color, BF);
                }
                if (i == obstaclefocus)
                {
                    //Form1.Line(Obstacles(i).pos.x - 2, Obstacles(i).pos.y - 2) - (Obstacles(i).pos.x + Obstacles(i).Width + 2, Obstacles(i).pos.y + Obstacles(i).Height + 2), vbWhite, B);
                }
            }
        }

        Form1.FillStyle = 0;
        return DrawObstacles;
    }

    public static dynamic DrawPolarIceMaze()
    {
        dynamic DrawPolarIceMaze = null;
        int i = 0;

        int k = 0;

        decimal blockWidth = 0;

        decimal blockHeight = 0;

        blockWidth = SimOpts.FieldWidth / 2;
        blockHeight = SimOpts.FieldHeight / 2;

        for (i = 0; i < 8; i++)
        {
            k = NewObstacle(blockWidth / 2, blockHeight / 2, blockWidth, blockHeight);
        }

        SimOpts.AllowHorizontalShapeDrift = true;
        SimOpts.AllowVerticalShapeDrift = true;
        SimOpts.ShapeDriftRate = 20;

        return DrawPolarIceMaze;
    }

    public static dynamic DrawSpiral()
    {
        dynamic DrawSpiral = null;
        int numOfHorzLines = 0;

        int numOfVertLines = 0;

        int numOfLines = 0;

        int i = 0;

        int j = 0;

        numOfHorzLines = CInt(SimOpts.FieldHeight / (mazeCorridorWidth + mazeWallThickness)) - 1;
        numOfVertLines = CInt(SimOpts.FieldWidth / (mazeCorridorWidth + mazeWallThickness)) - 1;
        numOfLines = Min(numOfHorzLines, numOfVertLines);
        if ((numOfLines % 2) != 0)
        {
            numOfLines = numOfLines - 1;
        }
        for (i = 1; i < (numOfLines / 2); i++)
        {
            j = NewObstacle(CSng(CSng(i - 1) * CSng(mazeCorridorWidth)), CSng(CSng(i) * CSng(mazeCorridorWidth)), CSng(SimOpts.FieldWidth - (CSng(mazeCorridorWidth) * (2 * (i - 1) + 1))), CSng(mazeWallThickness));
            j = NewObstacle(CSng(CSng(i) * CSng(mazeCorridorWidth)), CSng(SimOpts.FieldHeight - CSng(CSng(mazeCorridorWidth) * CSng(i))), CSng(SimOpts.FieldWidth - CSng(mazeCorridorWidth * 2 * CSng(i) - CSng(mazeWallThickness))), CSng(mazeWallThickness));
            j = NewObstacle(CSng(SimOpts.FieldWidth - (CSng(mazeCorridorWidth) * CSng(i))), CSng(CSng(i) * CSng(mazeCorridorWidth)), CSng(mazeWallThickness), CSng(SimOpts.FieldHeight - (CSng(CSng(mazeCorridorWidth) * CSng(2 * i)))));
            j = NewObstacle(CSng(CSng(i) * CSng(mazeCorridorWidth)), CSng(CSng(i + 1) * CSng(mazeCorridorWidth)), CSng(mazeWallThickness), CSng(SimOpts.FieldHeight - CSng(mazeCorridorWidth * CSng(2 * i + 1))));
        }
        return DrawSpiral;
    }

    public static dynamic DrawVerticalMaze()
    {
        dynamic DrawVerticalMaze = null;
        int i = 0;

        int j = 0;

        int numOfLines = 0;

        int Opening = 0;

        numOfLines = CInt(SimOpts.FieldHeight / (mazeCorridorWidth + mazeWallThickness)) - 1;
        for (i = 1; i < numOfLines; i++)
        {
            Opening = Random(0, SimOpts.FieldWidth - mazeCorridorWidth);
            j = NewObstacle(-100, CSng(CSng(i) * CSng(mazeCorridorWidth + mazeWallThickness)), CSng(Opening), CSng(mazeWallThickness));
            if ((Opening + mazeCorridorWidth) < SimOpts.FieldWidth + 100)
            {
                j = NewObstacle(CSng(Opening + CSng(mazeCorridorWidth)), CSng(i) * CSng(mazeCorridorWidth + mazeWallThickness), SimOpts.FieldWidth + 100 - Opening - CSng(mazeCorridorWidth), CSng(mazeWallThickness));
            }
        }
        return DrawVerticalMaze;
    }

    public static dynamic DriftObstacles()
    {
        dynamic DriftObstacles = null;
        int i = 0;

        for (i = 1; i < numObstacles; i++)
        {
            if (Obstacles(i).exist && (i != leftCompactor && i != rightCompactor))
            {
                if (SimOpts.AllowHorizontalShapeDrift)
                {
                    Obstacles(i).vel.x = Obstacles(i).vel.x + Random(-SimOpts.ShapeDriftRate, SimOpts.ShapeDriftRate) * Rndy() * 0.01m;
                }
                if (SimOpts.AllowVerticalShapeDrift)
                {
                    Obstacles(i).vel.y = Obstacles(i).vel.y + Random(-SimOpts.ShapeDriftRate, SimOpts.ShapeDriftRate) * Rndy() * 0.01m;
                }
                if (VectorMagnitude(Obstacles(i).vel) > SimOpts.MaxVelocity)
                {
                    Obstacles(i).vel = VectorScalar(Obstacles(i).vel, VectorMagnitude(Obstacles(i).vel) / SimOpts.MaxVelocity);
                }
            }
        }
        return DriftObstacles;
    }

    public static dynamic InitObstacles()
    {
        dynamic InitObstacles = null;
        int i = 0;

        for (i = 1; i < MAXOBSTACLES; i++)
        {
            Obstacles(i).exist = false;
        }
        numObstacles = 0;
        return InitObstacles;
    }

    public static dynamic InitTrashCompactorMaze()
    {
        dynamic InitTrashCompactorMaze = null;
        int i = 0;

        int k = 0;

        decimal blockWidth = 0;

        decimal blockHeight = 0;

        blockWidth = 1000;
        blockHeight = SimOpts.FieldHeight * 1.2m;

        leftCompactor = NewObstacle(-blockWidth + 1, SimOpts.FieldHeight * -0.1m, blockWidth, blockHeight);
        rightCompactor = NewObstacle(SimOpts.FieldWidth - 1, SimOpts.FieldHeight * -0.1m, blockWidth, blockHeight);
        //SimOpts.shapeDriftRate = 100
        Obstacles(leftCompactor).vel.x = SimOpts.ShapeDriftRate * 0.1m;
        Obstacles(rightCompactor).vel.x = -SimOpts.ShapeDriftRate * 0.1m;

        return InitTrashCompactorMaze;
    }

    public static dynamic MoveObstacles()
    {
        dynamic MoveObstacles = null;
        int i = 0;

        if (SimOpts.AllowHorizontalShapeDrift || SimOpts.AllowVerticalShapeDrift)
        {
            DriftObstacles();
        }
        if (leftCompactor > 0 || rightCompactor > 0)
        {
            TrashCompactorMove();
        }

        for (i = 1; i < numObstacles; i++)
        {
            if (Obstacles(i).exist)
            {
                Obstacles(i).pos = VectorAdd(Obstacles(i).pos, Obstacles(i).vel);
                //Keep obstalces from drifting off into space.
                if (Obstacles(i).pos.x < -Obstacles(i).Width)
                {
                    Obstacles(i).pos.x = -Obstacles(i).Width;
                    Obstacles(i).vel.x = SimOpts.ShapeDriftRate * 0.01m;
                }
                if (Obstacles(i).pos.y < -Obstacles(i).Height)
                {
                    Obstacles(i).pos.y = -Obstacles(i).Height;
                    Obstacles(i).vel.y = SimOpts.ShapeDriftRate * 0.01m;
                }
                if (Obstacles(i).pos.x > SimOpts.FieldWidth)
                {
                    Obstacles(i).pos.x = SimOpts.FieldWidth;
                    Obstacles(i).vel.x = -SimOpts.ShapeDriftRate * 0.01m;
                }
                if (Obstacles(i).pos.y > SimOpts.FieldHeight)
                {
                    Obstacles(i).pos.y = SimOpts.FieldHeight;
                    Obstacles(i).vel.y = -SimOpts.ShapeDriftRate * 0.01m;
                }
            }
        }
        return MoveObstacles;
    }

    public static int NewObstacle(decimal x, decimal y, decimal Width, decimal Height)
    {
        int NewObstacle = 0;
        int i = 0;

        if (numObstacles + 1 > MAXOBSTACLES)
        {
            NewObstacle = -1;
        }
        else
        {
            numObstacles = numObstacles + 1;
            NewObstacle = numObstacles;
            Obstacles(numObstacles).exist = true;
            Obstacles(numObstacles).pos.x = x;
            Obstacles(numObstacles).pos.y = y;
            Obstacles(numObstacles).Width = Width;
            Obstacles(numObstacles).Height = Height;
            Obstacles(numObstacles).vel.x = 0;
            Obstacles(numObstacles).vel.y = 0;
            if (SimOpts.MakeAllShapesBlack)
            {
                Obstacles(numObstacles).color = vbBlack;
            }
            else
            {
                Obstacles(numObstacles).color = Rnd * 65536 + Rnd * 255 + Rnd; // Random Color
            }
        }

        return NewObstacle;
    }

    public static bool ObstacleCollision(int n, int o)
    {
        bool ObstacleCollision = false;
        decimal botrightedge = 0;

        decimal botleftedge = 0;

        decimal bottopedge = 0;

        decimal botbottomedge = 0;

        ObstacleCollision = false;

        botrightedge = rob[n].pos.X + rob[n].radius;
        botleftedge = rob[n].pos.X - rob[n].radius;
        bottopedge = rob[n].pos.Y - rob[n].radius;
        botbottomedge = rob[n].pos.Y + rob[n].radius;

        if ((botrightedge > Obstacles(o).pos.x) And(botleftedge < Obstacles(o).pos.x + Obstacles(o).Width) And(botbottomedge > Obstacles(o).pos.y) And(bottopedge < Obstacles(o).pos.y + Obstacles(o).Height)) {
            ObstacleCollision = true;
        }
        return ObstacleCollision;
    }

    public static dynamic StopAllHorizontalObstacleMovement()
    {
        dynamic StopAllHorizontalObstacleMovement = null;
        int i = 0;

        for (i = 1; i < numObstacles; i++)
        {
            if (Obstacles(i).exist)
            {
                Obstacles(i).vel.x = 0;
            }
        }
        return StopAllHorizontalObstacleMovement;
    }

    public static dynamic StopAllVerticalObstacleMovement()
    {
        dynamic StopAllVerticalObstacleMovement = null;
        int i = 0;

        for (i = 1; i < numObstacles; i++)
        {
            if (Obstacles(i).exist)
            {
                Obstacles(i).vel.y = 0;
            }
        }
        return StopAllVerticalObstacleMovement;
    }

    public static dynamic TrashCompactorMove()
    {
        dynamic TrashCompactorMove = null;
        if (Obstacles(leftCompactor).pos.x > Obstacles(rightCompactor).pos.x + 400)
        {
            Obstacles(leftCompactor).vel.x = -Obstacles(leftCompactor).vel.x;
            Obstacles(rightCompactor).vel.x = -Obstacles(rightCompactor).vel.x;
        }
        if (Obstacles(leftCompactor).pos.x <= -Obstacles(leftCompactor).Width)
        {
            Obstacles(leftCompactor).vel.x = SimOpts.ShapeDriftRate * 0.1m;
            Obstacles(rightCompactor).vel.x = -SimOpts.ShapeDriftRate * 0.1m;
        }
        return TrashCompactorMove;
    }

    public static int whichobstacle(decimal x, decimal y)
    {
        int whichobstacle = 0;
        int t = 0;

        whichobstacle = 0;
        for (t = numObstacles; t < 1 Step - 1; t++) {
            if (Obstacles(t).exist)
            {
                if (x >= Obstacles(t).pos.x && x <= Obstacles(t).pos.x + Obstacles(t).Width && y >= Obstacles(t).pos.y && y <= Obstacles(t).pos.y + Obstacles(t).Height)
                {
                    whichobstacle = t;
                    return whichobstacle;
                }
            }
        }
        return whichobstacle;
    }

    // Copyright (c) 2006 Eric Lockard
    // eric@sulaadventures.com
    // All rights reserved.
    //Redistribution and use in source and binary forms, with or without
    //modification, are permitted provided that:
    //(1) source code distributions retain the above copyright notice and this
    //    paragraph in its entirety,
    //(2) distributions including binary code include the above copyright notice and
    //    this paragraph in its entirety in the documentation or other materials
    //    provided with the distribution, and
    //(3) Without the agreement of the author redistribution of this product is only allowed
    //    in non commercial terms and non profit distributions.
    //THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR IMPLIED
    //WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED WARRANTIES OF
    //MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
    // Option Explicit
    public class Obstacle
    {
        public int color = 0;
        public bool exist = false;
        public double Height = 0;
        public vector pos = null;
        public vector vel = null;
        public double Width = 0;
    }
}
