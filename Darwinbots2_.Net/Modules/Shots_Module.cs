using Iersera.Model;
using Iersera.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using static Common;
using static DNAManipulations;
using static F1Mode;
using static Globals;
using static NeoMutations;
using static Obstacles;
using static Physics;
using static Robots;
using static Senses;
using static SimOptModule;
using static System.Math;
using static Vegs;

internal static class Shots_Module
{
    private const double MinBotRadius = 0.2;
    private const int ShellEffectiveness = 20;
    private const int shotdecay = 40;
    private const double SlimeEffectiveness = 1 / 20;
    private const int VenumEffectivenessVSShell = 25;

    public static List<Color> FlashColor { get; set; } = new();
    public static double MaxBotShotSeperation { get; set; }
    public static int maxshotarray { get; set; }
    public static List<Shot> Shots { get; set; } = new();
    public static int ShotsThisCycle { get; set; }

    public static void CreateShot(double X, double Y, double vx, double vy, int loc, robot par, double val, double Range, Color col)
    {
        if (val > 32000)
            val = 32000; // Overflow protection

        var shot = new Shot
        {
            parent = par,
            FromSpecie = par.FName,
            fromveg = par.Veg,
            pos = new vector(X, Y),
            velocity = new vector(vx, vy),
            opos = new vector(X + vx, Y + vy),
            age = 0,
            color = col,
            exist = true,
            stored = false,
            value = (int)val,
            nrg = loc == -2 ? val : Range + 40 + 1,
            Range = (int)((Range + 40 + 1) / 40),
            memloc = par.mem[834]
        };

        if (loc > 0 || loc == -100)
            shot.shottype = loc;
        else
        {
            shot.shottype = -(Abs(loc) % 8);
            if (shot.shottype == 0)
                shot.shottype = -8; // want multiples of -8 to be -8
        }

        if (shot.shottype == -5)
            shot.Memval = shot.parent.mem[839];

        Shots.Add(shot);
    }

    public static Color DBrite(Color col)
    {
        var b = col.B;
        var g = col.G;
        var r = col.R;

        b += (byte)((255 - b) / 2);
        g += (byte)((255 - g) / 2);
        r += (byte)((255 - r) / 2);

        return Color.FromRgb(r, g, b);
    }

    public static void Decay(robot rob)
    {
        rob.DecayTimer++;

        if (rob.DecayTimer < SimOpts.Decaydelay)
            return;

        rob.DecayTimer = 0;

        rob.aim = ThreadSafeRandom.Local.NextDouble() * 2 * PI;
        rob.aimvector = new vector(Cos(rob.aim), Sin(rob.aim));

        var va = Clamp(rob.body, 0, SimOpts.Decay / 10);

        int SH;

        if (va != 0)
        {
            switch (SimOpts.DecayType)
            {
                case 2:
                    SH = -4;
                    NewShot(rob, SH, va, 1);
                    break;

                case 3:
                    SH = -2;
                    NewShot(rob, SH, va, 1);
                    break;
            }
        }

        rob.body -= SimOpts.Decay / 10;
        rob.radius = FindRadius(rob);
    }

    public static void Defacate(robot rob)
    {
        var SH = -4;
        var va = 200.0;

        if (va > rob.Waste)
        {
            va = rob.Waste;
        }
        if (rob.Waste > 32000)
        {
            rob.Waste = 31500;
            va = 500;
        }

        rob.Waste -= va;
        rob.nrg -= SimOpts.Costs[SHOTCOST] * SimOpts.Costs[COSTMULTIPLIER] / ((rob.Ties.Count < 0 ? 0 : rob.Ties.Count) + 1);
        NewShot(rob, SH, va, 1, true);
        rob.Pwaste += va / 1000;
    }

    public static bool MakeVirus(robot rob, int gene)
    {
        rob.virusshot = NewShot(rob, -7, gene, 1);
        return rob.virusshot != null;
    }

    public static Shot NewShot(robot rob, int shottype, double val, double rngmultiplier, bool offset = false)
    {
        if (val > 32000)
        {
            val = 32000; // EricL March 16, 2006 This line moved here from below to catch val before assignment
        }

        var shot = new Shot
        {
            exist = true,
            age = 0,
            parent = rob,
            FromSpecie = rob.FName, //Which species fired the shot
            fromveg = rob.Veg, //does shot come from a veg or not?
            color = rob.color,
            value = (int)val,
            memloc = rob.mem[835],
            Memval = rob.mem[836]
        };

        if (shottype > 0 || shottype == -100)
            shot.shottype = shottype;
        else
        {
            shot.shottype = -(Abs(shottype) % 8);

            if (shot.shottype == 0)
                shot.shottype = -8; // want multiples of -8 to be -8
        }

        if (shottype == -2)
            shot.color = Colors.White;

        double ShAngle;

        if (rob.mem[backshot] == 0)
            ShAngle = rob.aim; //forward shots
        else
        {
            ShAngle = angnorm(rob.aim - PI); //backward shots
            rob.mem[backshot] = 0;
        }

        if (rob.mem[aimshoot] != 0)
        {
            ShAngle = rob.aim - rob.mem[aimshoot] % 1256 / 200;
            rob.mem[aimshoot] = 0;
        }

        ShAngle += (ThreadSafeRandom.Local.NextDouble() - .5) * 0.2;

        var angle = new vector(Cos(ShAngle), -Sin(ShAngle));

        shot.pos = rob.pos + (angle * rob.radius);

        //Botsareus 6/23/2016 Takes care of shot position bug - so it matches the painted robot position
        if (offset)
        {
            shot.pos -= rob.vel;
            shot.pos += rob.actvel;
        }

        shot.velocity = rob.actvel + (angle * 40);

        shot.opos = shot.pos - shot.velocity;

        if (rob.vbody > 10)
        {
            shot.nrg = Log(Abs(rob.vbody)) * 60 * rngmultiplier;

            shot.Range = (shot.nrg + 40 + 1) / 40;
            shot.nrg += 40 + 1;
        }
        else
        {
            shot.Range = rngmultiplier;
            shot.nrg = 40 * rngmultiplier;
        }

        if (shottype == -7)
        {
            shot.color = Colors.Cyan;
            shot.genenum = (int)val;
            shot.stored = true;
            if (!CopyGene(shot, shot.genenum))
            {
                return null;
            }

            if ((SimOpts.F1 || x_restartmode == 1) && (Disqualify == 1 || Disqualify == 2))
                dreason(rob.FName, rob.tag, "using a virus");

            if (!SimOpts.F1 && rob.dq == 1 && (Disqualify == 1 || Disqualify == 2))
                rob.Dead = true;
        }
        else
            shot.stored = false;

        if (shottype == -2)
            shot.nrg = val;

        if (shottype == -8)
            shot.dna = rob.dna;

        Shots.Add(shot);

        return shot;
    }

    public static void ShootVirus(robot rob, Shot shot)
    {
        //here we shoot a virus

        if (!shot.exist)
            return;

        if (!shot.stored)
            return;

        if (rob.mem[VshootSys] < 0)
            rob.mem[VshootSys] = 1;

        var tempa = Clamp(rob.mem[VshootSys] * 20, 0, 32000); //.vshoot * 20

        shot.nrg = tempa;
        rob.nrg -= (tempa / 20) - (SimOpts.Costs[SHOTCOST] * SimOpts.Costs[COSTMULTIPLIER]);

        shot.Range = 11 + rob.mem[VshootSys] / 2;
        rob.nrg -= rob.mem[VshootSys] - (SimOpts.Costs[SHOTCOST] * SimOpts.Costs[COSTMULTIPLIER]);

        var ShAngle = (double)ThreadSafeRandom.Local.Next(1, 1256) / 200;
        shot.stored = false;
        shot.pos += new vector(Cos(ShAngle) * rob.radius, -Sin(ShAngle) * rob.radius);
        shot.velocity = new vector(absx(ShAngle, RobSize / 3, 0, 0, 0), absy(ShAngle, RobSize / 3, 0, 0, 0)) + rob.actvel;

        shot.opos = shot.pos - shot.velocity;
    }

    public static void UpdateShots()
    {
        var numshots = 0;

        foreach (var shot in Shots.ToArray())
        {
            if (shot.flash)
            {
                shot.exist = false;
                shot.flash = false;
                shot.dna.Clear();
                Shots.Remove(shot);
                continue;
            }

            if (!shot.exist)
                continue;

            numshots++; // Counts the number of existing shots each cycle for display purposes

            //Add the energy in the shot to the total sim energy if it is an energy shot
            if (shot.shottype == -2)
                TotalSimEnergy[CurrentEnergyCycle] += (int)shot.nrg;

            robot rob = null;
            if (shot.shottype != -100 && !shot.stored)
                rob = NewShotCollision(shot); // go off and check for collisions with bots.

            //babies born into a stream of shots from its parent shouldn't die
            //from those shots.  I can't imagine this temporary imunity can be
            //exploited, so it should be safe
            if (rob != null & (shot.parent != rob.parent || rob.age > 1))
            {
                //this below is horribly complicated:  allow me to explain:
                //nrg dissipates in a non-linear fashion.  Very little nrg disappears until you
                //get near the last 10% of the journey or so.
                //Don't dissipate nrg if nrg shots last forever.
                if (!SimOpts.NoShotDecay || shot.shottype != -2)
                {
                    if (shot.shottype != -4 || !SimOpts.NoWShotDecay)
                    {
                        var x = shot.Range == 0 ? shot.age + 1 : shot.age / shot.Range;
                        shot.nrg *= Atan(x * shotdecay - shotdecay) / Atan(-shotdecay);
                    }
                }

                if (shot.shottype > 0)
                {
                    //Botsareus 10/6/2015 Minor bug fixing and redundent code removal
                    shot.shottype = (shot.shottype - 1) % 1000 + 1; // EricL 6/2006 Mod 1000 so as to increse probabiltiy that mutations do something interesting

                    if (shot.shottype != DelgeneSys)
                    {
                        if ((shot.nrg / 2 > rob.poison) || (rob.poison == 0))
                        {
                            rob.mem[shot.shottype] = shot.value;
                        }
                        else
                        {
                            CreateShot(shot.pos.X, shot.pos.Y, -shot.velocity.X, -shot.velocity.Y, -5, rob, shot.nrg / 2, shot.Range * 40, Colors.Yellow);
                            rob.poison -= shot.nrg / 2 * 0.9;
                            rob.Waste += shot.nrg / 2 * 0.1;
                            if (rob.poison < 0)
                            {
                                rob.poison = 0;
                            }
                            rob.mem[poison] = (int)rob.poison;
                        }
                    }
                }
                else
                {
                    switch (shot.shottype)
                    {
                        case -1:
                            ReleaseEnergy(rob, shot);
                            break;

                        case -2:
                            TakeEnergy(rob, shot);
                            break;

                        case -3:
                            TakeVenom(rob, shot);
                            break;

                        case -4:
                            TakeWaste(rob, shot);
                            break;

                        case -5:
                            TakePoison(rob, shot);
                            break;

                        case -6:
                            ReleaseBody(rob, shot);
                            break;

                        case -7:
                            AddGene(rob, shot);
                            break;

                        case -8:
                            TakeSperm(rob, shot);
                            break;
                    }
                }
                taste(rob, shot.opos.X, shot.opos.Y, shot.shottype);
                shot.flash = true;
            }

            if (numObstacles > 0)
                DoShotObstacleCollisions(shot);

            shot.opos = shot.pos;
            shot.pos += shot.velocity; //Euler integration

            //Age shots unless we are not decaying them.  At some point, we may want to see how old shots are, so
            //this may need to be changed at some point but for now, it lets shots never die by never growing old.
            //Always age Poff shots
            if ((!SimOpts.NoShotDecay || shot.shottype != -2) && !shot.stored && (shot.shottype != -4 || !SimOpts.NoWShotDecay))
            {
                shot.age++;
            }

            if (shot.age > shot.Range && !shot.flash)
            {
                shot.exist = false; // Kill shots once they reach maturity
                shot.dna.Clear();
                Shots.Remove(shot);
            }
        }

        ShotsThisCycle = numshots;
    }

    private static void AddGene(robot rob, Shot shot)
    {
        //Dead bodies and virus immune bots can't catch a virus
        if (rob.Corpse || rob.VirusImmune)
            return;

        var power = shot.nrg / (shot.Range * RobSize / 3) * shot.value;

        if (power < rob.Slime * SlimeEffectiveness)
        {
            rob.Slime -= power / SlimeEffectiveness;
            return;
        }
        else
        {
            rob.Slime -= power / SlimeEffectiveness;
            if (rob.Slime < 0.5)
            {
                rob.Slime = 0;
            }
        }

        var Position = ThreadSafeRandom.Local.Next(0, rob.genenum);//gene position to insert the virus

        int Insert;
        if (Position == 0)
            Insert = 0;
        else
        {
            Insert = GeneEnd(rob.dna, genepos(rob.dna, Position));
            if (Insert == rob.DnaLen)
            {
                Insert = rob.DnaLen;
            }
        }

        rob.dna.InsertRange(Insert, shot.dna);

        makeoccurrlist(rob);
        rob.DnaLen = DnaLen(rob.dna);
        rob.genenum = CountGenes(rob.dna);
        rob.mem[DnaLenSys] = rob.DnaLen;
        rob.mem[GenesSys] = rob.genenum;

        rob.SubSpecies = NewSubSpecies(rob); // Infection with a virus counts as a new subspecies
        var vlen = 0;//length of the DNA code of the virus
        logmutation(rob, $"Infected with virus of length {vlen} during cycle {SimOpts.TotRunCycle} at pos {Insert}");
        rob.Mutations++;
        rob.LastMut++;
    }

    private static bool CopyGene(Shot shot, int p)
    {
        if ((p > shot.parent.genenum) || p < 1)
            return false;

        var GeneStart = genepos(shot.parent.dna, p);
        var GeneEnding = GeneEnd(shot.parent.dna, GeneStart);
        var genelen = GeneEnding - GeneStart + 1;

        if (genelen < 1)
            return false;

        shot.dna.Clear();
        shot.dna.AddRange(shot.parent.dna.Skip(GeneStart).Take(genelen));

        return true;
    }

    private static robot NewShotCollision(Shot shot)
    {
        // Check for collisions with the field edges
        if (SimOpts.Updnconnected == true)
        {
            if (shot.pos.Y > SimOpts.FieldHeight)
                shot.pos.Y -= SimOpts.FieldHeight;
            else if (shot.pos.Y < 0)
                shot.pos.Y += SimOpts.FieldHeight;
        }
        else
        {
            if (shot.pos.Y > SimOpts.FieldHeight)
            {
                shot.pos.Y = SimOpts.FieldHeight;
                shot.velocity.Y = -1 * Abs(shot.velocity.Y);
            }
            else if (shot.pos.Y < 0)
            {
                shot.pos.Y = 0;
                shot.velocity.Y = Abs(shot.velocity.Y);
            }
        }

        if (SimOpts.Dxsxconnected == true)
        {
            if (shot.pos.X > SimOpts.FieldWidth)
                shot.pos.X -= SimOpts.FieldWidth;
            else if (shot.pos.X < 0)
                shot.pos.X += SimOpts.FieldWidth;
        }
        else
        {
            if (shot.pos.X > SimOpts.FieldWidth)
            {
                shot.pos.X = SimOpts.FieldWidth;
                shot.velocity.X = -1 * Abs(shot.velocity.X);
            }
            else if (shot.pos.X < 0)
            {
                shot.pos.X = 0;
                shot.velocity.X = Abs(shot.velocity.X);
            }
        }

        robot NewShotCollision = null;

        double earliestCollision = 100;//Used to find which bot was hit earliest in the cycle.

        foreach (var rob in rob)
        {
            if (!rob.exist || shot.parent == rob || rob.FName == "Base.txt" && hidepred || Abs(shot.opos.X - rob.pos.X) >= MaxBotShotSeperation || Abs(shot.opos.Y - rob.pos.Y) >= MaxBotShotSeperation)
                continue;

            var B0 = rob.pos - rob.vel + rob.actvel;
            var p = shot.pos - B0;
            var hitTime = 0.0;

            if (p.Magnitude() < rob.radius)
            {
                // shot is inside the target at Time 0.  Did we miss the entry last cycle?  How?
                hitTime = 0;
                earliestCollision = 0;
                NewShotCollision = rob;
                break;
            }

            var d = shot.velocity - rob.actvel;
            var P2 = p.MagnitudeSquare();
            var D2 = d.MagnitudeSquare();

            if (D2 == 0)
                continue;

            var DdotP = Dot(d, p);
            var X = -DdotP;
            var Y = Math.Pow(DdotP, 2) - D2 * (P2 - Math.Pow(rob.radius, 2));

            if (Y < 0)
                continue; // No collision

            Y = Sqrt(Y);

            //The time in the cycle at which the earliest collision with the shot occurred.
            var time0 = (X - Y) / D2;
            var time1 = (X + Y) / D2;

            var usetime0 = time0 > 0 && time0 < 1;
            var usetime1 = time1 > 0 && time1 < 1;

            if (!(usetime0 || usetime1))
                continue;

            if (usetime0 & usetime1)
                hitTime = Min(time0, time1);
            else if (usetime0)
                hitTime = time0;
            else
                hitTime = time1;

            NewShotCollision = rob;

            if (hitTime < earliestCollision)
                earliestCollision = hitTime;

            if (earliestCollision <= MinBotRadius)
                break;
            else
                continue;
        }

        if (earliestCollision <= 1)
            shot.pos = (shot.velocity * earliestCollision) + shot.pos;

        return NewShotCollision;
    }

    private static void ReleaseBody(robot rob, Shot shot)
    {
        if (rob.body <= 0)
            return;

        var vel = rob.actvel - shot.velocity + (rob.actvel * 0.5);

        var power = SimOpts.EnergyExType
                    ? shot.Range == 0 ? 0 : shot.value * shot.nrg / (shot.Range * (RobSize / 3)) * SimOpts.EnergyProp
                    : SimOpts.EnergyFix;

        if (power > 32000)
            power = 32000;

        var shell = rob.shell * ShellEffectiveness;

        if (power > (rob.body * 10 / 0.8 + shell))
            power = rob.body * 10 / 0.8 + shell;

        if (power < shell)
        {
            rob.shell -= power / ShellEffectiveness;

            if (rob.shell < 0)
                rob.shell = 0;

            rob.mem[823] = (int)rob.shell;
            return;
        }
        else
        {
            rob.shell -= power / ShellEffectiveness;
            if (rob.shell < 0)
            {
                rob.shell = 0;
            }
            rob.mem[823] = (int)rob.shell;
            power -= shell;
        }

        if (power <= 0)
            return;

        var range = shot.Range * 2;

        // create energy shot
        if (rob.Corpse == true)
        {
            power *= 4;
            if (power > rob.body * 10)
                power = rob.body * 10;

            rob.body -= power / 10;
            rob.radius = FindRadius(rob);
        }
        else
        {
            var leftover = 0.0;
            var energyLost = power * 0.2;
            if (energyLost > rob.nrg)
            {
                leftover = energyLost - rob.nrg;
                rob.nrg = 0;
            }
            else
                rob.nrg -= energyLost;

            energyLost = power * 0.08;
            if (energyLost > rob.body)
            {
                leftover = leftover + energyLost - rob.body * 10;
                rob.body = 0;
            }
            else
                rob.body -= energyLost;

            if (leftover > 0)
            {
                if (rob.nrg > 0 & rob.nrg > leftover)
                {
                    rob.nrg -= leftover;
                    leftover = 0;
                }
                else if (rob.nrg > 0 & rob.nrg < leftover)
                {
                    leftover -= rob.nrg;
                    rob.nrg = 0;
                }

                if (rob.body > 0 & rob.body * 10 > leftover)
                {
                    rob.body -= leftover * 0.1;
                }
                else if (rob.body > 0 & rob.body * 10 < leftover)
                    rob.body = 0;
            }
            rob.radius = FindRadius(rob);
        }

        if (rob.body <= 0.5 || rob.nrg <= 0.5)
        {
            rob.Dead = true;
            shot.parent.Kills = shot.parent.Kills + 1;
            shot.parent.mem[220] = shot.parent.Kills;
        }

        CreateShot(shot.pos.X, shot.pos.Y, vel.X, vel.Y, -2, rob, power, range * (RobSize / 3), Colors.White);
    }

    private static void ReleaseEnergy(robot rob, Shot shot)
    {
        if (rob.nrg <= 0.5)
            return;

        var vel = rob.actvel - shot.velocity;
        vel += rob.actvel * 0.5;

        double power;
        if (SimOpts.EnergyExType)
        {
            power = shot.Range == 0 ? 0 : shot.value * shot.nrg / (shot.Range * (RobSize / 3)) * SimOpts.EnergyProp;
            if (shot.nrg < 0)
                return;
        }
        else
            power = SimOpts.EnergyFix;

        if (rob.Corpse)
            power *= 0.5;

        var range = shot.Range * 2;

        if (rob.poison > power)
        {
            //create poison shot
            CreateShot(shot.pos.X, shot.pos.Y, vel.X, vel.Y, -5, rob, power, range * (RobSize / 3), Colors.Yellow);
            rob.poison -= (power * 0.9);
            if (rob.poison < 0)
            {
                rob.poison = 0;
            }
            rob.mem[poison] = (int)rob.poison;
        }
        else
        { // create energy shot
            var energyLost = power * 0.9;
            if (energyLost > rob.nrg)
            {
                power = rob.nrg;
                rob.nrg = 0;
            }
            else
            {
                rob.nrg -= energyLost;
            }

            energyLost = power * 0.01;
            rob.body = energyLost > rob.body ? 0 : rob.body - energyLost;

            CreateShot(shot.pos.X, shot.pos.Y, vel.X, vel.Y, -2, rob, power, range * (RobSize / 3), Colors.White);
            rob.radius = FindRadius(rob);
        }

        if (rob.body <= 0.5 || rob.nrg <= 0.5)
        {
            rob.Dead = true;
            shot.parent.Kills++;
            shot.parent.mem[220] = shot.parent.Kills;
        }
    }

    private static void TakeEnergy(robot rob, Shot shot)
    {
        double overflow = 0;

        if (rob.Corpse)
            return;

        var partial = shot.Range < 0.00001 ? 0 : shot.nrg;

        if ((rob.nrg + partial * 0.95) > 32000)
        {
            overflow = rob.nrg + (partial * 0.95) - 32000;
            rob.nrg = 32000;
        }
        else
            rob.nrg += partial * 0.95; //95% of energy goes to nrg

        if (rob.body + partial * 0.004 + (overflow * 0.1) > 32000)
            rob.body = 32000;
        else
            rob.body = rob.body + (partial * 0.004) + (overflow * 0.1); //4% goes to body

        rob.Waste += partial * 0.01; //1% goes to waste

        rob.radius = FindRadius(rob);
    }

    private static void TakePoison(robot rob, Shot shot)
    {
        if (rob.Corpse)
            return;

        var power = shot.nrg / (shot.Range * (RobSize / 3)) * shot.value;

        if (power < 1)
            return;

        if (shot.FromSpecie == rob.FName)
        {
            //Robot is immune to poison from his own species
            rob.poison += power; //Robot absorbs poison fired by conspecs

            if (rob.poison > 32000)
                rob.poison = 32000;

            rob.mem[827] = (int)rob.poison;
        }
        else
        {
            rob.Poisoned = true;
            rob.Poisoncount += power / 1.5;
            if (rob.Poisoncount > 32000)
                rob.Poisoncount = 32000;

            if (shot.memloc > 0)
            {
                rob.Ploc = (shot.memloc - 1) % 1000 + 1;
                if (rob.Ploc == 340)
                    rob.Ploc = 0;
            }
            else
            {
                do
                {
                    rob.Ploc = ThreadSafeRandom.Local.Next(1, 1000);
                } while (rob.Ploc == 340);
            }
            rob.Pval = shot.Memval;
        }
    }

    private static void TakeSperm(robot rob, Shot shot)
    {
        if (rob.fertilized < -10)
            return;//block sex repro when necessary

        if (shot.dna.Count == 0)
            return;

        rob.fertilized = 10; // bots stay fertilized for 10 cycles currently
        rob.mem[SYSFERTILIZED] = 10;
        rob.spermDNA = shot.dna;
    }

    private static void TakeVenom(robot rob, Shot shot)
    {
        if (rob.Corpse)
            return;

        var power = shot.nrg / (shot.Range * (RobSize / 3)) * shot.value;

        if (power < 1)
            return;

        if (shot.FromSpecie == rob.FName)
        {
            //Robot is immune to venom from his own species
            rob.venom += power; //Robot absorbs venom fired by conspec

            if (rob.venom > 32000)
            {
                rob.venom = 32000;
            }

            rob.mem[825] = (int)rob.venom;
        }
        else
        {
            power *= VenumEffectivenessVSShell; //Botsareus 3/6/2013 max power for venum is capped at 100 so I multiply to get an average
            if (power < rob.shell * ShellEffectiveness)
            {
                rob.shell -= power / ShellEffectiveness;
                rob.mem[823] = (int)rob.shell;
                return;
            }
            else
            {
                var temp = power;
                power -= rob.shell * ShellEffectiveness;
                rob.shell -= temp / ShellEffectiveness;
                if (rob.shell < 0)
                    rob.shell = 0;

                rob.mem[823] = (int)rob.shell;
            }
            power /= VenumEffectivenessVSShell; //Botsareus 3/6/2013 after shell conversion devide

            if (power < 1)
                return;

            rob.Paralyzed = true;

            if ((rob.Paracount + power) > 32000)
                rob.Paracount = 32000;
            else
                rob.Paracount += power;

            if (shot.memloc > 0)
            {
                rob.Vloc = (shot.memloc - 1) % 1000 + 1;
                if (rob.Vloc == 340)
                    rob.Vloc = 0;
            }
            else
            {
                do
                {
                    rob.Vloc = ThreadSafeRandom.Local.Next(1, 1000);
                } while (rob.Vloc == 340);
            }

            rob.Vval = shot.Memval;
        }
    }

    private static void TakeWaste(robot rob, Shot shot)
    {
        var power = shot.nrg / (shot.Range * (RobSize / 3)) * shot.value;

        if (power >= 0)
            rob.Waste += power;
    }
}
