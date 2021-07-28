using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DarwinBots.Modules
{
    internal interface IShotManager
    {
        List<Shot> Shots { get; }

        void Decay(Robot rob);

        void Defecate(Robot rob);

        bool MakeVirus(Robot rob, int gene);

        Shot NewShot(Robot rob, int shotType, double val, double rangeMultiplier, bool offset = false);

        void ShootVirus(Robot rob, Shot shot);
    }

    internal class ShotsManager : IShotManager
    {
        private const double MinBotRadius = 0.2;
        private const int ShellEffectiveness = 20;
        private const int ShotDecay = 40;
        private const double SlimeEffectiveness = 1.0 / 20;
        private const int VenomEffectivenessVsShell = 25;
        private readonly IObstacleManager _obstacleManager;

        public ShotsManager(IObstacleManager obstacleManager)
        {
            _obstacleManager = obstacleManager;
        }

        public double MaxBotShotSeparation { get; set; }
        public List<Shot> Shots { get; } = new();

        public void Decay(Robot rob)
        {
            rob.DecayTimer++;

            if (rob.DecayTimer < SimOpt.SimOpts.DecayDelay)
                return;

            rob.DecayTimer = 0;

            rob.Aim = ThreadSafeRandom.Local.NextDouble() * 2 * Math.PI;

            var va = Math.Clamp(rob.Body, 0, SimOpt.SimOpts.Decay / 10);

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

            rob.Body -= SimOpt.SimOpts.Decay / 10;
        }

        public void Defecate(Robot rob)
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
            rob.Energy -= SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier / ((rob.Ties.Count < 0 ? 0 : rob.Ties.Count) + 1);
            NewShot(rob, -4, va, 1, true);
            rob.PermanentWaste += va / 1000;
        }

        public bool MakeVirus(Robot rob, int gene)
        {
            rob.VirusShot = NewShot(rob, -7, gene, 1);
            return rob.VirusShot != null;
        }

        public Shot NewShot(Robot rob, int shotType, double val, double rangeMultiplier, bool offset = false)
        {
            if (val > 32000)
                val = 32000;

            var shot = new Shot
            {
                Age = 0,
                Parent = rob,
                FromSpecie = rob.FName, //Which species fired the shot
                Color = rob.Color,
                Value = (int)val,
                MemoryLocation = rob.Memory[835],
                MemoryValue = rob.Memory[836]
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

            if (rob.Memory[MemoryAddresses.backshot] == 0)
                shotAngle = rob.Aim; //forward shots
            else
            {
                shotAngle = Physics.NormaliseAngle(rob.Aim - Math.PI); //backward shots
                rob.Memory[MemoryAddresses.backshot] = 0;
            }

            if (rob.Memory[MemoryAddresses.aimshoot] != 0)
            {
                shotAngle = rob.Aim - rob.Memory[MemoryAddresses.aimshoot] % 1256 / 200.0;
                rob.Memory[MemoryAddresses.aimshoot] = 0;
            }

            shotAngle += (ThreadSafeRandom.Local.NextDouble() - .5) * 0.2;

            var angle = new DoubleVector(Math.Cos(shotAngle), -Math.Sin(shotAngle));

            shot.Position = rob.Position + angle * rob.GetRadius(SimOpt.SimOpts.FixedBotRadii);

            if (offset)
            {
                shot.Position -= rob.Velocity;
                shot.Position += rob.ActualVelocity;
            }

            shot.Velocity = rob.ActualVelocity + angle * 40;

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
                    shot.Dna = rob.Dna;
                    break;
            }

            Shots.Add(shot);

            return shot;
        }

        public void ShootVirus(Robot rob, Shot shot)
        {
            if (!shot.Exist)
                return;

            if (!shot.Stored)
                return;

            if (rob.Memory[MemoryAddresses.VshootSys] < 0)
                rob.Memory[MemoryAddresses.VshootSys] = 1;

            var energy = Math.Clamp(rob.Memory[MemoryAddresses.VshootSys] * 20, 0, 32000); //.vshoot * 20

            shot.Energy = energy;
            rob.Energy -= energy / 20.0 - SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier;

            shot.Range = 11 + rob.Memory[MemoryAddresses.VshootSys] / 2;
            rob.Energy -= rob.Memory[MemoryAddresses.VshootSys] - SimOpt.SimOpts.Costs.ShotFormationCost * SimOpt.SimOpts.Costs.CostMultiplier;

            var shotAngle = (double)ThreadSafeRandom.Local.Next(1, 1256) / 200;
            shot.Stored = false;
            shot.Position += new DoubleVector(Math.Cos(shotAngle) * rob.GetRadius(SimOpt.SimOpts.FixedBotRadii), -Math.Sin(shotAngle) * rob.GetRadius(SimOpt.SimOpts.FixedBotRadii));
            shot.Velocity = new DoubleVector(RobotsManager.AbsX(shotAngle, Robot.RobSize / 3, 0, 0, 0), RobotsManager.AbsY(shotAngle, Robot.RobSize / 3, 0, 0, 0)) + rob.ActualVelocity;

            shot.OldPosition = shot.Position - shot.Velocity;
        }

        public void UpdateShots(IRobotManager robotManager)
        {
            foreach (var shot in Shots.ToArray())
            {
                if (shot.Flash)
                {
                    shot.CleanUp();
                    Shots.Remove(shot);
                    continue;
                }

                if (!shot.Exist)
                    continue;

                //Add the energy in the shot to the total sim energy if it is an energy shot
                if (shot.ShotType == -2)
                    Vegs.TotalSimEnergy[Vegs.CurrentEnergyCycle] += (int)shot.Energy;

                Robot rob = null;
                if (shot.ShotType != -100 && !shot.Stored)
                    rob = NewShotCollision(robotManager, shot); // go off and check for collisions with bots.

                //babies born into a stream of shots from its parent shouldn't die
                //from those shots.  I can't imagine this temporary imunity can be
                //exploited, so it should be safe
                if (rob != null && (shot.Parent != rob.Parent || rob.Age > 1))
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

                        if (shot.ShotType != MemoryAddresses.DelgeneSys)
                        {
                            if (shot.Energy / 2 > rob.Poison || rob.Poison == 0)
                            {
                                rob.Memory[shot.ShotType] = shot.Value;
                            }
                            else
                            {
                                CreateShot(shot.Position.X, shot.Position.Y, -shot.Velocity.X, -shot.Velocity.Y, -5, rob, shot.Energy / 2, shot.Range * 40, Colors.Yellow);
                                rob.Poison -= shot.Energy / 2 * 0.9;
                                rob.Waste += shot.Energy / 2 * 0.1;
                                if (rob.Poison < 0)
                                {
                                    rob.Poison = 0;
                                }
                                rob.Memory[MemoryAddresses.poison] = (int)rob.Poison;
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
                                AddGene(robotManager, rob, shot);
                                break;

                            case -8:
                                TakeSperm(rob, shot);
                                break;
                        }
                    }
                    Senses.Taste(rob, shot.OldPosition.X, shot.OldPosition.Y, shot.ShotType);
                    shot.Flash = true;
                }

                if (_obstacleManager.Obstacles.Count > 0)
                    _obstacleManager.DoShotObstacleCollisions(this, shot);

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
                    shot.CleanUp();
                    Shots.Remove(shot);
                }
            }
        }

        private void AddGene(IRobotManager robotManager, Robot rob, Shot shot)
        {
            //Dead bodies and virus immune bots can't catch a virus
            if (rob.IsCorpse || rob.IsVirusImmune)
                return;

            var power = shot.Energy / (shot.Range * Robot.RobSize / 3) * shot.Value;

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

            var position = ThreadSafeRandom.Local.Next(0, rob.NumberOfGenes);//gene position to insert the virus

            int insert;
            if (position == 0)
                insert = 0;
            else
            {
                insert = DnaManipulations.GeneEnd(rob.Dna, DnaManipulations.GenePosition(rob.Dna, position));
                if (insert == rob.Dna.Count)
                {
                    insert = rob.Dna.Count;
                }
            }

            rob.Dna.InsertRange(insert, shot.Dna);

            Senses.MakeOccurrList(rob);
            rob.NumberOfGenes = DnaManipulations.CountGenes(rob.Dna);
            rob.Memory[MemoryAddresses.DnaLenSys] = rob.Dna.Count;
            rob.Memory[MemoryAddresses.GenesSys] = rob.NumberOfGenes;

            rob.SubSpecies = NeoMutations.NewSubSpecies(rob); // Infection with a virus counts as a new subspecies

            NeoMutations.LogMutation(rob, $"Infected with virus during cycle {SimOpt.SimOpts.TotRunCycle} at pos {insert}");
            rob.Mutations++;
            rob.LastMutation++;
        }

        private bool CopyGene(Shot shot, int p)
        {
            if (p > shot.Parent.NumberOfGenes || p < 1)
                return false;

            var geneStart = DnaManipulations.GenePosition(shot.Parent.Dna, p);
            var geneEnding = DnaManipulations.GeneEnd(shot.Parent.Dna, geneStart);
            var genelen = geneEnding - geneStart + 1;

            if (genelen < 1)
                return false;

            shot.Dna.Clear();
            shot.Dna.AddRange(shot.Parent.Dna.Skip(geneStart).Take(genelen));

            return true;
        }

        private void CreateShot(double x, double y, double vx, double vy, int loc, Robot par, double val, double range, Color col)
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
                Stored = false,
                Value = (int)val,
                Energy = loc == -2 ? val : range + 40 + 1,
                Range = (int)((range + 40 + 1) / 40),
                MemoryLocation = par.Memory[834]
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
                shot.MemoryValue = shot.Parent.Memory[839];

            Shots.Add(shot);
        }

        private Robot NewShotCollision(IRobotManager robotManager, Shot shot)
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
                    shot.Position = new DoubleVector(shot.Position.X, SimOpt.SimOpts.FieldHeight);
                    shot.Velocity = new DoubleVector(shot.Velocity.X, -1 * Math.Abs(shot.Velocity.Y));
                }
                else if (shot.Position.Y < 0)
                {
                    shot.Position = new DoubleVector(shot.Position.X, 0);
                    shot.Velocity = new DoubleVector(shot.Velocity.X, Math.Abs(shot.Velocity.Y));
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
                    shot.Position = new DoubleVector(SimOpt.SimOpts.FieldWidth, shot.Position.Y);
                    shot.Velocity = new DoubleVector(-1 * Math.Abs(shot.Velocity.X), shot.Velocity.Y);
                }
                else if (shot.Position.X < 0)
                {
                    shot.Position = new DoubleVector(0, shot.Position.Y);
                    shot.Velocity = new DoubleVector(Math.Abs(shot.Velocity.X), shot.Velocity.Y);
                }
            }

            Robot newShotCollision = null;

            double earliestCollision = 100;//Used to find which bot was hit earliest in the cycle.

            foreach (var rob in robotManager.Robots)
            {
                if (!rob.Exists || shot.Parent == rob || Math.Abs(shot.OldPosition.X - rob.Position.X) >= MaxBotShotSeparation || Math.Abs(shot.OldPosition.Y - rob.Position.Y) >= MaxBotShotSeparation)
                    continue;

                var b0 = rob.Position - rob.Velocity + rob.ActualVelocity;
                var p = shot.Position - b0;

                if (p.Magnitude() < rob.GetRadius(SimOpt.SimOpts.FixedBotRadii))
                {
                    // shot is inside the target at Time 0.  Did we miss the entry last cycle?  How?
                    earliestCollision = 0;
                    newShotCollision = rob;
                    break;
                }

                var d = shot.Velocity - rob.ActualVelocity;
                var p2 = p.MagnitudeSquare();
                var d2 = d.MagnitudeSquare();

                if (d2 == 0)
                    continue;

                var dDotP = DoubleVector.Dot(d, p);
                var x = -dDotP;
                var y = Math.Pow(dDotP, 2) - d2 * (p2 - Math.Pow(rob.GetRadius(SimOpt.SimOpts.FixedBotRadii), 2));

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

        private void ReleaseBody(Robot rob, Shot shot)
        {
            if (rob.Body <= 0)
                return;

            var vel = rob.ActualVelocity - shot.Velocity + rob.ActualVelocity * 0.5;

            var power = SimOpt.SimOpts.EnergyExType == ShotMode.Proportional
                        ? shot.Range == 0 ? 0 : shot.Value * shot.Energy / (shot.Range * (Robot.RobSize / 3.0)) * SimOpt.SimOpts.EnergyProp
                        : SimOpt.SimOpts.EnergyFix;

            if (power > 32000)
                power = 32000;

            var shell = rob.Shell * ShellEffectiveness;

            if (power > rob.Body * 10 / 0.8 + shell)
                power = rob.Body * 10 / 0.8 + shell;

            if (power < shell)
            {
                rob.Shell -= power / ShellEffectiveness;

                if (rob.Shell < 0)
                    rob.Shell = 0;

                rob.Memory[823] = (int)rob.Shell;
                return;
            }

            rob.Shell -= power / ShellEffectiveness;
            if (rob.Shell < 0)
            {
                rob.Shell = 0;
            }
            rob.Memory[823] = (int)rob.Shell;
            power -= shell;

            if (power <= 0)
                return;

            var range = shot.Range * 2;

            // create energy shot
            if (rob.IsCorpse)
            {
                power *= 4;
                if (power > rob.Body * 10)
                    power = rob.Body * 10;

                rob.Body -= power / 10;
            }
            else
            {
                var leftover = 0.0;
                var energyLost = power * 0.2;
                if (energyLost > rob.Energy)
                {
                    leftover = energyLost - rob.Energy;
                    rob.Energy = 0;
                }
                else
                    rob.Energy -= energyLost;

                energyLost = power * 0.08;
                if (energyLost > rob.Body)
                {
                    leftover = leftover + energyLost - rob.Body * 10;
                    rob.Body = 0;
                }
                else
                    rob.Body -= energyLost;

                if (leftover > 0)
                {
                    if (rob.Energy > 0 & rob.Energy > leftover)
                    {
                        rob.Energy -= leftover;
                        leftover = 0;
                    }
                    else if (rob.Energy > 0 & rob.Energy < leftover)
                    {
                        leftover -= rob.Energy;
                        rob.Energy = 0;
                    }

                    if (rob.Body > 0 & rob.Body * 10 > leftover)
                    {
                        rob.Body -= leftover * 0.1;
                    }
                    else if (rob.Body > 0 & rob.Body * 10 < leftover)
                        rob.Body = 0;
                }
            }

            if (rob.Body <= 0.5 || rob.Energy <= 0.5)
            {
                rob.IsDead = true;
                shot.Parent.Kills = shot.Parent.Kills + 1;
                shot.Parent.Memory[220] = shot.Parent.Kills;
            }

            CreateShot(shot.Position.X, shot.Position.Y, vel.X, vel.Y, -2, rob, power, range * (Robot.RobSize / 3.0), Colors.White);
        }

        private void ReleaseEnergy(Robot rob, Shot shot)
        {
            if (rob.Energy <= 0.5)
                return;

            var vel = rob.ActualVelocity - shot.Velocity;
            vel += rob.ActualVelocity * 0.5;

            double power;
            if (SimOpt.SimOpts.EnergyExType == ShotMode.Proportional)
            {
                power = shot.Range == 0 ? 0 : shot.Value * shot.Energy / (shot.Range * (Robot.RobSize / 3.0)) * SimOpt.SimOpts.EnergyProp;
                if (shot.Energy < 0)
                    return;
            }
            else
                power = SimOpt.SimOpts.EnergyFix;

            if (rob.IsCorpse)
                power *= 0.5;

            var range = shot.Range * 2;

            if (rob.Poison > power)
            {
                //create poison shot
                CreateShot(shot.Position.X, shot.Position.Y, vel.X, vel.Y, -5, rob, power, range * (Robot.RobSize / 3.0), Colors.Yellow);
                rob.Poison -= power * 0.9;
                if (rob.Poison < 0)
                {
                    rob.Poison = 0;
                }
                rob.Memory[MemoryAddresses.poison] = (int)rob.Poison;
            }
            else
            { // create energy shot
                var energyLost = power * 0.9;
                if (energyLost > rob.Energy)
                {
                    power = rob.Energy;
                    rob.Energy = 0;
                }
                else
                {
                    rob.Energy -= energyLost;
                }

                energyLost = power * 0.01;
                rob.Body = energyLost > rob.Body ? 0 : rob.Body - energyLost;

                CreateShot(shot.Position.X, shot.Position.Y, vel.X, vel.Y, -2, rob, power, range * (Robot.RobSize / 3.0), Colors.White);
            }

            if (!(rob.Body <= 0.5) && !(rob.Energy <= 0.5)) return;

            rob.IsDead = true;
            shot.Parent.Kills++;
            shot.Parent.Memory[220] = shot.Parent.Kills;
        }

        private void TakeEnergy(Robot rob, Shot shot)
        {
            double overflow = 0;

            if (rob.IsCorpse)
                return;

            var partial = shot.Range < 0.00001 ? 0 : shot.Energy;

            if (rob.Energy + partial * 0.95 > 32000)
            {
                overflow = rob.Energy + partial * 0.95 - 32000;
                rob.Energy = 32000;
            }
            else
                rob.Energy += partial * 0.95; // 95% of energy goes to nrg

            if (rob.Body + partial * 0.004 + overflow * 0.1 > 32000)
                rob.Body = 32000;
            else
                rob.Body = rob.Body + partial * 0.004 + overflow * 0.1; // 4% goes to body

            rob.Waste += partial * 0.01; // 1% goes to waste
        }

        private void TakePoison(Robot rob, Shot shot)
        {
            if (rob.IsCorpse)
                return;

            var power = shot.Energy / (shot.Range * (Robot.RobSize / 3.0)) * shot.Value;

            if (power < 1)
                return;

            if (shot.FromSpecie == rob.FName)
            {
                //Robot is immune to poison from his own species
                rob.Poison += power; //Robot absorbs poison fired by conspecs

                if (rob.Poison > 32000)
                    rob.Poison = 32000;

                rob.Memory[827] = (int)rob.Poison;
            }
            else
            {
                rob.IsPoisoned = true;
                rob.PoisonCountdown += (int)(power / 1.5);
                if (rob.PoisonCountdown > 32000)
                    rob.PoisonCountdown = 32000;

                if (shot.MemoryLocation > 0)
                {
                    rob.PoisonLocation = (shot.MemoryLocation - 1) % 1000 + 1;
                    if (rob.PoisonLocation == 340)
                        rob.PoisonLocation = 0;
                }
                else
                {
                    do
                    {
                        rob.PoisonLocation = ThreadSafeRandom.Local.Next(1, 1000);
                    } while (rob.PoisonLocation == 340);
                }
                rob.PoisonValue = shot.MemoryValue;
            }
        }

        private void TakeSperm(Robot rob, Shot shot)
        {
            if (rob.Fertilized < -10)
                return;//block sex repro when necessary

            if (shot.Dna.Count == 0)
                return;

            rob.Fertilized = 10; // bots stay fertilized for 10 cycles currently
            rob.Memory[MemoryAddresses.SYSFERTILIZED] = 10;
            rob.SpermDna = shot.Dna;
        }

        private void TakeVenom(Robot rob, Shot shot)
        {
            if (rob.IsCorpse)
                return;

            var power = shot.Energy / (shot.Range * (Robot.RobSize / 3.0)) * shot.Value;

            if (power < 1)
                return;

            if (shot.FromSpecie == rob.FName)
            {
                //Robot is immune to venom from his own species
                rob.Venom += power; //Robot absorbs venom fired by conspec

                if (rob.Venom > 32000)
                {
                    rob.Venom = 32000;
                }

                rob.Memory[825] = (int)rob.Venom;
            }
            else
            {
                power *= VenomEffectivenessVsShell; //Botsareus 3/6/2013 max power for venum is capped at 100 so I multiply to get an average
                if (power < rob.Shell * ShellEffectiveness)
                {
                    rob.Shell -= power / ShellEffectiveness;
                    rob.Memory[823] = (int)rob.Shell;
                    return;
                }

                var temp = power;
                power -= rob.Shell * ShellEffectiveness;
                rob.Shell -= temp / ShellEffectiveness;
                if (rob.Shell < 0)
                    rob.Shell = 0;

                rob.Memory[823] = (int)rob.Shell;
                power /= VenomEffectivenessVsShell; // Botsareus 3/6/2013 after shell conversion divide

                if (power < 1)
                    return;

                rob.IsParalyzed = true;

                if (rob.ParalyzedCountdown + power > 32000)
                    rob.ParalyzedCountdown = 32000;
                else
                    rob.ParalyzedCountdown += (int)power;

                if (shot.MemoryLocation > 0)
                {
                    rob.VirusLocation = (shot.MemoryLocation - 1) % 1000 + 1;
                    if (rob.VirusLocation == 340)
                        rob.VirusLocation = 0;
                }
                else
                {
                    do
                    {
                        rob.VirusLocation = ThreadSafeRandom.Local.Next(1, 1000);
                    } while (rob.VirusLocation == 340);
                }

                rob.VirusValue = shot.MemoryValue;
            }
        }

        private void TakeWaste(Robot rob, Shot shot)
        {
            var power = shot.Energy / (shot.Range * (Robot.RobSize / 3.0)) * shot.Value;

            if (power >= 0)
                rob.Waste += power;
        }
    }
}
