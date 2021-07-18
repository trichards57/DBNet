using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DarwinBots.Modules
{
    internal class ShotsManager
    {
        private const double MinBotRadius = 0.2;
        private const int ShellEffectiveness = 20;
        private const int ShotDecay = 40;
        private const double SlimeEffectiveness = 1.0 / 20;
        private const int VenomEffectivenessVsShell = 25;

        public double MaxBotShotSeparation { get; set; }
        public List<Shot> Shots { get; } = new();

        public void Decay(robot rob)
        {
            rob.DecayTimer++;

            if (rob.DecayTimer < SimOpt.SimOpts.DecayDelay)
                return;

            rob.DecayTimer = 0;

            rob.aim = ThreadSafeRandom.Local.NextDouble() * 2 * Math.PI;
            rob.aimvector = new DoubleVector(Math.Cos(rob.aim), Math.Sin(rob.aim));

            var va = Math.Clamp(rob.body, 0, SimOpt.SimOpts.Decay / 10);

            if (va != 0)
            {
                switch (SimOpt.SimOpts.DecayType)
                {
                    case DecayType.Energy:
                        NewShot(rob, -4, va, 1);
                        break;

                    case DecayType.Waste:
                        NewShot(rob, -2, va, 1);
                        break;
                }
            }

            rob.body -= SimOpt.SimOpts.Decay / 10;
            rob.radius = Robots.FindRadius(rob);
        }

        public void Defecate(robot rob)
        {
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
            rob.nrg -= SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier / ((rob.Ties.Count < 0 ? 0 : rob.Ties.Count) + 1);
            NewShot(rob, -4, va, 1, true);
            rob.Pwaste += va / 1000;
        }

        public bool MakeVirus(robot rob, int gene)
        {
            rob.virusshot = NewShot(rob, -7, gene, 1);
            return rob.virusshot != null;
        }

        public Shot NewShot(robot rob, int shotType, double val, double rangeMultiplier, bool offset = false)
        {
            if (val > 32000)
                val = 32000;

            var shot = new Shot
            {
                Exist = true,
                Age = 0,
                Parent = rob,
                FromSpecie = rob.FName, //Which species fired the shot
                Color = rob.color,
                Value = (int)val,
                MemoryLocation = rob.mem[835],
                MemoryValue = rob.mem[836]
            };

            if (shotType is > 0 or -100)
                shot.ShotType = shotType;
            else
            {
                shot.ShotType = -(Math.Abs(shotType) % 8);

                if (shot.ShotType == 0)
                    shot.ShotType = -8; // want multiples of -8 to be -8
            }

            if (shotType == -2)
                shot.Color = Colors.White;

            double shotAngle;

            if (rob.mem[Robots.backshot] == 0)
                shotAngle = rob.aim; //forward shots
            else
            {
                shotAngle = Physics.NormaliseAngle(rob.aim - Math.PI); //backward shots
                rob.mem[Robots.backshot] = 0;
            }

            if (rob.mem[Robots.aimshoot] != 0)
            {
                shotAngle = rob.aim - rob.mem[Robots.aimshoot] % 1256 / 200.0;
                rob.mem[Robots.aimshoot] = 0;
            }

            shotAngle += (ThreadSafeRandom.Local.NextDouble() - .5) * 0.2;

            var angle = new DoubleVector(Math.Cos(shotAngle), -Math.Sin(shotAngle));

            shot.Position = rob.pos + angle * rob.radius;

            if (offset)
            {
                shot.Position -= rob.vel;
                shot.Position += rob.actvel;
            }

            shot.Velocity = rob.actvel + angle * 40;

            shot.OldPosition = shot.Position - shot.Velocity;

            if (rob.vbody > 10)
            {
                shot.Energy = Math.Log(Math.Abs(rob.vbody)) * 60 * rangeMultiplier;

                shot.Range = (shot.Energy + 40 + 1) / 40;
                shot.Energy += 40 + 1;
            }
            else
            {
                shot.Range = rangeMultiplier;
                shot.Energy = 40 * rangeMultiplier;
            }

            if (shotType == -7)
            {
                shot.Color = Colors.Cyan;
                shot.GeneNum = (int)val;
                shot.Stored = true;
                if (!CopyGene(shot, shot.GeneNum))
                {
                    return null;
                }
            }
            else
                shot.Stored = false;

            switch (shotType)
            {
                case -2:
                    shot.Energy = val;
                    break;

                case -8:
                    shot.Dna = rob.dna;
                    break;
            }

            Shots.Add(shot);

            return shot;
        }

        public void ShootVirus(robot rob, Shot shot)
        {
            if (!shot.Exist)
                return;

            if (!shot.Stored)
                return;

            if (rob.mem[Robots.VshootSys] < 0)
                rob.mem[Robots.VshootSys] = 1;

            var energy = Math.Clamp(rob.mem[Robots.VshootSys] * 20, 0, 32000); //.vshoot * 20

            shot.Energy = energy;
            rob.nrg -= energy / 20.0 - SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier;

            shot.Range = 11 + rob.mem[Robots.VshootSys] / 2;
            rob.nrg -= rob.mem[Robots.VshootSys] - SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier;

            var shotAngle = (double)ThreadSafeRandom.Local.Next(1, 1256) / 200;
            shot.Stored = false;
            shot.Position += new DoubleVector(Math.Cos(shotAngle) * rob.radius, -Math.Sin(shotAngle) * rob.radius);
            shot.Velocity = new DoubleVector(Robots.AbsX(shotAngle, Robots.RobSize / 3, 0, 0, 0), Robots.AbsY(shotAngle, Robots.RobSize / 3, 0, 0, 0)) + rob.actvel;

            shot.OldPosition = shot.Position - shot.Velocity;
        }

        public void UpdateShots()
        {
            foreach (var shot in Shots.ToArray())
            {
                if (shot.Flash)
                {
                    shot.Exist = false;
                    shot.Flash = false;
                    shot.Dna.Clear();
                    Shots.Remove(shot);
                    continue;
                }

                if (!shot.Exist)
                    continue;

                //Add the energy in the shot to the total sim energy if it is an energy shot
                if (shot.ShotType == -2)
                    Vegs.TotalSimEnergy[Vegs.CurrentEnergyCycle] += (int)shot.Energy;

                robot rob = null;
                if (shot.ShotType != -100 && !shot.Stored)
                    rob = NewShotCollision(shot); // go off and check for collisions with bots.

                //babies born into a stream of shots from its parent shouldn't die
                //from those shots.  I can't imagine this temporary imunity can be
                //exploited, so it should be safe
                if (rob != null && (shot.Parent != rob.parent || rob.age > 1))
                {
                    //this below is horribly complicated:  allow me to explain:
                    //nrg dissipates in a non-linear fashion.  Very little nrg disappears until you
                    //get near the last 10% of the journey or so.
                    //Don't dissipate nrg if nrg shots last forever.
                    if (!SimOpt.SimOpts.NoShotDecay || shot.ShotType != -2)
                    {
                        if (shot.ShotType != -4 || !SimOpt.SimOpts.NoWShotDecay)
                        {
                            var x = shot.Range == 0 ? shot.Age + 1 : shot.Age / shot.Range;
                            shot.Energy *= Math.Atan(x * ShotDecay - ShotDecay) / Math.Atan(-ShotDecay);
                        }
                    }

                    if (shot.ShotType > 0)
                    {
                        shot.ShotType = (shot.ShotType - 1) % 1000 + 1; // EricL 6/2006 Mod 1000 so as to increse probabiltiy that mutations do something interesting

                        if (shot.ShotType != Robots.DelgeneSys)
                        {
                            if (shot.Energy / 2 > rob.poison || rob.poison == 0)
                            {
                                rob.mem[shot.ShotType] = shot.Value;
                            }
                            else
                            {
                                CreateShot(shot.Position.X, shot.Position.Y, -shot.Velocity.X, -shot.Velocity.Y, -5, rob, shot.Energy / 2, shot.Range * 40, Colors.Yellow);
                                rob.poison -= shot.Energy / 2 * 0.9;
                                rob.Waste += shot.Energy / 2 * 0.1;
                                if (rob.poison < 0)
                                {
                                    rob.poison = 0;
                                }
                                rob.mem[Robots.poison] = (int)rob.poison;
                            }
                        }
                    }
                    else
                    {
                        switch (shot.ShotType)
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
                    Senses.Taste(rob, shot.OldPosition.X, shot.OldPosition.Y, shot.ShotType);
                    shot.Flash = true;
                }

                if (Globals.ObstacleManager.Obstacles.Count > 0)
                    Globals.ObstacleManager.DoShotObstacleCollisions(shot);

                shot.OldPosition = shot.Position;
                shot.Position += shot.Velocity; //Euler integration

                //Age shots unless we are not decaying them.  At some point, we may want to see how old shots are, so
                //this may need to be changed at some point but for now, it lets shots never die by never growing old.
                //Always age Poff shots
                if ((!SimOpt.SimOpts.NoShotDecay || shot.ShotType != -2) && !shot.Stored && (shot.ShotType != -4 || !SimOpt.SimOpts.NoWShotDecay))
                {
                    shot.Age++;
                }

                if (shot.Age > shot.Range && !shot.Flash)
                {
                    shot.Exist = false; // Kill shots once they reach maturity
                    shot.Dna.Clear();
                    Shots.Remove(shot);
                }
            }
        }

        private void AddGene(robot rob, Shot shot)
        {
            //Dead bodies and virus immune bots can't catch a virus
            if (rob.Corpse || rob.VirusImmune)
                return;

            var power = shot.Energy / (shot.Range * Robots.RobSize / 3) * shot.Value;

            if (power < rob.Slime * SlimeEffectiveness)
            {
                rob.Slime -= power / SlimeEffectiveness;
                return;
            }

            rob.Slime -= power / SlimeEffectiveness;
            if (rob.Slime < 0.5)
            {
                rob.Slime = 0;
            }

            var position = ThreadSafeRandom.Local.Next(0, rob.genenum);//gene position to insert the virus

            int insert;
            if (position == 0)
                insert = 0;
            else
            {
                insert = DnaManipulations.GeneEnd(rob.dna, DnaManipulations.GenePosition(rob.dna, position));
                if (insert == rob.dna.Count)
                {
                    insert = rob.dna.Count;
                }
            }

            rob.dna.InsertRange(insert, shot.Dna);

            Senses.MakeOccurrList(rob);
            rob.genenum = DnaManipulations.CountGenes(rob.dna);
            rob.mem[Robots.DnaLenSys] = rob.dna.Count;
            rob.mem[Robots.GenesSys] = rob.genenum;

            rob.SubSpecies = NeoMutations.NewSubSpecies(rob); // Infection with a virus counts as a new subspecies

            NeoMutations.LogMutation(rob, $"Infected with virus during cycle {SimOpt.SimOpts.TotRunCycle} at pos {insert}");
            rob.Mutations++;
            rob.LastMut++;
        }

        private bool CopyGene(Shot shot, int p)
        {
            if (p > shot.Parent.genenum || p < 1)
                return false;

            var geneStart = DnaManipulations.GenePosition(shot.Parent.dna, p);
            var geneEnding = DnaManipulations.GeneEnd(shot.Parent.dna, geneStart);
            var genelen = geneEnding - geneStart + 1;

            if (genelen < 1)
                return false;

            shot.Dna.Clear();
            shot.Dna.AddRange(shot.Parent.dna.Skip(geneStart).Take(genelen));

            return true;
        }

        private void CreateShot(double x, double y, double vx, double vy, int loc, robot par, double val, double range, Color col)
        {
            if (val > 32000)
                val = 32000; // Overflow protection

            var shot = new Shot
            {
                Parent = par,
                FromSpecie = par.FName,
                Position = new DoubleVector(x, y),
                Velocity = new DoubleVector(vx, vy),
                OldPosition = new DoubleVector(x + vx, y + vy),
                Age = 0,
                Color = col,
                Exist = true,
                Stored = false,
                Value = (int)val,
                Energy = loc == -2 ? val : range + 40 + 1,
                Range = (int)((range + 40 + 1) / 40),
                MemoryLocation = par.mem[834]
            };

            if (loc is > 0 or -100)
                shot.ShotType = loc;
            else
            {
                shot.ShotType = -(Math.Abs(loc) % 8);
                if (shot.ShotType == 0)
                    shot.ShotType = -8; // want multiples of -8 to be -8
            }

            if (shot.ShotType == -5)
                shot.MemoryValue = shot.Parent.mem[839];

            Shots.Add(shot);
        }

        private robot NewShotCollision(Shot shot)
        {
            // Check for collisions with the field edges
            if (SimOpt.SimOpts.UpDnConnected)
            {
                if (shot.Position.Y > SimOpt.SimOpts.FieldHeight)
                    shot.Position -= new DoubleVector(0, SimOpt.SimOpts.FieldHeight);
                else if (shot.Position.Y < 0)
                    shot.Position += new DoubleVector(0, SimOpt.SimOpts.FieldHeight);
            }
            else
            {
                if (shot.Position.Y > SimOpt.SimOpts.FieldHeight)
                {
                    shot.Position = shot.Position with { Y = SimOpt.SimOpts.FieldHeight };
                    shot.Velocity = shot.Velocity with { Y = -1 * Math.Abs(shot.Velocity.Y) };
                }
                else if (shot.Position.Y < 0)
                {
                    shot.Position = shot.Position with { Y = 0 };
                    shot.Velocity = shot.Velocity with { Y = Math.Abs(shot.Velocity.Y) };
                }
            }

            if (SimOpt.SimOpts.DxSxConnected)
            {
                if (shot.Position.X > SimOpt.SimOpts.FieldWidth)
                    shot.Position -= new DoubleVector(SimOpt.SimOpts.FieldWidth, 0);
                else if (shot.Position.X < 0)
                    shot.Position += new DoubleVector(SimOpt.SimOpts.FieldWidth, 0);
            }
            else
            {
                if (shot.Position.X > SimOpt.SimOpts.FieldWidth)
                {
                    shot.Position = shot.Position with { X = SimOpt.SimOpts.FieldWidth };
                    shot.Velocity = shot.Velocity with { X = -1 * Math.Abs(shot.Velocity.X) };
                }
                else if (shot.Position.X < 0)
                {
                    shot.Position = shot.Position with { X = 0 };
                    shot.Velocity = shot.Velocity with { X = Math.Abs(shot.Velocity.X) };
                }
            }

            robot newShotCollision = null;

            double earliestCollision = 100;//Used to find which bot was hit earliest in the cycle.

            foreach (var rob in Robots.rob)
            {
                if (!rob.exist || shot.Parent == rob || Math.Abs(shot.OldPosition.X - rob.pos.X) >= MaxBotShotSeparation || Math.Abs(shot.OldPosition.Y - rob.pos.Y) >= MaxBotShotSeparation)
                    continue;

                var b0 = rob.pos - rob.vel + rob.actvel;
                var p = shot.Position - b0;

                if (p.Magnitude() < rob.radius)
                {
                    // shot is inside the target at Time 0.  Did we miss the entry last cycle?  How?
                    earliestCollision = 0;
                    newShotCollision = rob;
                    break;
                }

                var d = shot.Velocity - rob.actvel;
                var p2 = p.MagnitudeSquare();
                var d2 = d.MagnitudeSquare();

                if (d2 == 0)
                    continue;

                var dDotP = DoubleVector.Dot(d, p);
                var x = -dDotP;
                var y = Math.Pow(dDotP, 2) - d2 * (p2 - Math.Pow(rob.radius, 2));

                if (y < 0)
                    continue; // No collision

                y = Math.Sqrt(y);

                //The time in the cycle at which the earliest collision with the shot occurred.
                var time0 = (x - y) / d2;
                var time1 = (x + y) / d2;

                var useTime0 = time0 is > 0 and < 1;
                var useTime1 = time1 is > 0 and < 1;

                if (!(useTime0 || useTime1))
                    continue;

                double hitTime;
                if (useTime0 & useTime1)
                    hitTime = Math.Min(time0, time1);
                else if (useTime0)
                    hitTime = time0;
                else
                    hitTime = time1;

                newShotCollision = rob;

                if (hitTime < earliestCollision)
                    earliestCollision = hitTime;

                if (earliestCollision <= MinBotRadius)
                    break;
            }

            if (earliestCollision <= 1)
                shot.Position = shot.Velocity * earliestCollision + shot.Position;

            return newShotCollision;
        }

        private void ReleaseBody(robot rob, Shot shot)
        {
            if (rob.body <= 0)
                return;

            var vel = rob.actvel - shot.Velocity + rob.actvel * 0.5;

            var power = SimOpt.SimOpts.EnergyExType == ShotMode.Proportional
                        ? shot.Range == 0 ? 0 : shot.Value * shot.Energy / (shot.Range * (Robots.RobSize / 3.0)) * SimOpt.SimOpts.EnergyProp
                        : SimOpt.SimOpts.EnergyFix;

            if (power > 32000)
                power = 32000;

            var shell = rob.shell * ShellEffectiveness;

            if (power > rob.body * 10 / 0.8 + shell)
                power = rob.body * 10 / 0.8 + shell;

            if (power < shell)
            {
                rob.shell -= power / ShellEffectiveness;

                if (rob.shell < 0)
                    rob.shell = 0;

                rob.mem[823] = (int)rob.shell;
                return;
            }

            rob.shell -= power / ShellEffectiveness;
            if (rob.shell < 0)
            {
                rob.shell = 0;
            }
            rob.mem[823] = (int)rob.shell;
            power -= shell;

            if (power <= 0)
                return;

            var range = shot.Range * 2;

            // create energy shot
            if (rob.Corpse)
            {
                power *= 4;
                if (power > rob.body * 10)
                    power = rob.body * 10;

                rob.body -= power / 10;
                rob.radius = Robots.FindRadius(rob);
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
                rob.radius = Robots.FindRadius(rob);
            }

            if (rob.body <= 0.5 || rob.nrg <= 0.5)
            {
                rob.Dead = true;
                shot.Parent.Kills = shot.Parent.Kills + 1;
                shot.Parent.mem[220] = shot.Parent.Kills;
            }

            CreateShot(shot.Position.X, shot.Position.Y, vel.X, vel.Y, -2, rob, power, range * (Robots.RobSize / 3.0), Colors.White);
        }

        private void ReleaseEnergy(robot rob, Shot shot)
        {
            if (rob.nrg <= 0.5)
                return;

            var vel = rob.actvel - shot.Velocity;
            vel += rob.actvel * 0.5;

            double power;
            if (SimOpt.SimOpts.EnergyExType == ShotMode.Proportional)
            {
                power = shot.Range == 0 ? 0 : shot.Value * shot.Energy / (shot.Range * (Robots.RobSize / 3.0)) * SimOpt.SimOpts.EnergyProp;
                if (shot.Energy < 0)
                    return;
            }
            else
                power = SimOpt.SimOpts.EnergyFix;

            if (rob.Corpse)
                power *= 0.5;

            var range = shot.Range * 2;

            if (rob.poison > power)
            {
                //create poison shot
                CreateShot(shot.Position.X, shot.Position.Y, vel.X, vel.Y, -5, rob, power, range * (Robots.RobSize / 3.0), Colors.Yellow);
                rob.poison -= power * 0.9;
                if (rob.poison < 0)
                {
                    rob.poison = 0;
                }
                rob.mem[Robots.poison] = (int)rob.poison;
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

                CreateShot(shot.Position.X, shot.Position.Y, vel.X, vel.Y, -2, rob, power, range * (Robots.RobSize / 3.0), Colors.White);
                rob.radius = Robots.FindRadius(rob);
            }

            if (!(rob.body <= 0.5) && !(rob.nrg <= 0.5)) return;

            rob.Dead = true;
            shot.Parent.Kills++;
            shot.Parent.mem[220] = shot.Parent.Kills;
        }

        private void TakeEnergy(robot rob, Shot shot)
        {
            double overflow = 0;

            if (rob.Corpse)
                return;

            var partial = shot.Range < 0.00001 ? 0 : shot.Energy;

            if (rob.nrg + partial * 0.95 > 32000)
            {
                overflow = rob.nrg + partial * 0.95 - 32000;
                rob.nrg = 32000;
            }
            else
                rob.nrg += partial * 0.95; // 95% of energy goes to nrg

            if (rob.body + partial * 0.004 + overflow * 0.1 > 32000)
                rob.body = 32000;
            else
                rob.body = rob.body + partial * 0.004 + overflow * 0.1; // 4% goes to body

            rob.Waste += partial * 0.01; // 1% goes to waste

            rob.radius = Robots.FindRadius(rob);
        }

        private void TakePoison(robot rob, Shot shot)
        {
            if (rob.Corpse)
                return;

            var power = shot.Energy / (shot.Range * (Robots.RobSize / 3.0)) * shot.Value;

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

                if (shot.MemoryLocation > 0)
                {
                    rob.Ploc = (shot.MemoryLocation - 1) % 1000 + 1;
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
                rob.Pval = shot.MemoryValue;
            }
        }

        private void TakeSperm(robot rob, Shot shot)
        {
            if (rob.fertilized < -10)
                return;//block sex repro when necessary

            if (shot.Dna.Count == 0)
                return;

            rob.fertilized = 10; // bots stay fertilized for 10 cycles currently
            rob.mem[Robots.SYSFERTILIZED] = 10;
            rob.spermDNA = shot.Dna;
        }

        private void TakeVenom(robot rob, Shot shot)
        {
            if (rob.Corpse)
                return;

            var power = shot.Energy / (shot.Range * (Robots.RobSize / 3.0)) * shot.Value;

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
                power *= VenomEffectivenessVsShell; //Botsareus 3/6/2013 max power for venum is capped at 100 so I multiply to get an average
                if (power < rob.shell * ShellEffectiveness)
                {
                    rob.shell -= power / ShellEffectiveness;
                    rob.mem[823] = (int)rob.shell;
                    return;
                }

                var temp = power;
                power -= rob.shell * ShellEffectiveness;
                rob.shell -= temp / ShellEffectiveness;
                if (rob.shell < 0)
                    rob.shell = 0;

                rob.mem[823] = (int)rob.shell;
                power /= VenomEffectivenessVsShell; // Botsareus 3/6/2013 after shell conversion divide

                if (power < 1)
                    return;

                rob.Paralyzed = true;

                if (rob.Paracount + power > 32000)
                    rob.Paracount = 32000;
                else
                    rob.Paracount += power;

                if (shot.MemoryLocation > 0)
                {
                    rob.Vloc = (shot.MemoryLocation - 1) % 1000 + 1;
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

                rob.Vval = shot.MemoryValue;
            }
        }

        private void TakeWaste(robot rob, Shot shot)
        {
            var power = shot.Energy / (shot.Range * (Robots.RobSize / 3.0)) * shot.Value;

            if (power >= 0)
                rob.Waste += power;
        }
    }
}
