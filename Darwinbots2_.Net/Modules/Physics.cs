using DarwinBots.Model;
using DarwinBots.Support;
using System;
using System.Linq;

namespace DarwinBots.Modules
{
    internal static class Physics
    {
        public const double SmudgeFactor = 50;
        public static double BouyancyScaling { get; set; }

        public static double AngDiff(double a1, double a2)
        {
            var r = a1 - a2;

            if (r > Math.PI)
                r -= 2 * Math.PI;

            if (r < -Math.PI)
                r += 2 * Math.PI;

            return r;
        }

        public static double Angle(double x1, double y1, double x2, double y2)
        {
            return Math.Atan2(y2 - y1, x2 - x1);
        }

        public static void BorderCollision(IBucketManager bucketManager, Robot rob)
        {
            const double b = 0.05;

            if (rob.Position.X > rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) && rob.Position.X < SimOpt.SimOpts.FieldWidth - rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) && rob.Position.Y > rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) && rob.Position.Y < SimOpt.SimOpts.FieldHeight - rob.GetRadius(SimOpt.SimOpts.FixedBotRadii))
                return;

            rob.Memory[214] = 0;

            var smudge = rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) + SmudgeFactor;

            var dif = DoubleVector.Min(DoubleVector.Max(rob.Position, new DoubleVector(smudge, smudge)), new DoubleVector(SimOpt.SimOpts.FieldWidth - smudge, SimOpt.SimOpts.FieldHeight - smudge));
            var dist = dif - rob.Position;

            if (dist.X != 0)
            {
                if (SimOpt.SimOpts.DxSxConnected)
                {
                    if (dist.X < 0)
                        Multibots.ReSpawn(bucketManager, rob, smudge, rob.Position.Y);
                    else
                        Multibots.ReSpawn(bucketManager, rob, SimOpt.SimOpts.FieldWidth - smudge, rob.Position.Y);
                }
                else
                {
                    rob.Memory[214] = 1;

                    if (rob.Position.X - rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) < 0)
                        rob.Position = new DoubleVector(rob.GetRadius(SimOpt.SimOpts.FixedBotRadii), rob.Position.Y);

                    if (rob.Position.X + rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) > SimOpt.SimOpts.FieldWidth)
                        rob.Position = new DoubleVector(SimOpt.SimOpts.FieldWidth - rob.GetRadius(SimOpt.SimOpts.FixedBotRadii), rob.Position.Y);

                    rob.ResistiveImpulse += new DoubleVector(rob.Velocity.X * b, 0);
                }
            }

            if (dist.Y != 0)
            {
                if (SimOpt.SimOpts.UpDnConnected)
                {
                    if (dist.Y < 0)
                        Multibots.ReSpawn(bucketManager, rob, rob.Position.X, smudge);
                    else
                        Multibots.ReSpawn(bucketManager, rob, rob.Position.X, SimOpt.SimOpts.FieldHeight - smudge);
                }
                else
                {
                    rob.Memory[214] = 1;

                    if (rob.Position.Y - rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) < 0)
                        rob.Position = new DoubleVector(rob.Position.X, rob.GetRadius(SimOpt.SimOpts.FixedBotRadii));

                    if (rob.Position.Y + rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) > SimOpt.SimOpts.FieldHeight)
                        rob.Position = new DoubleVector(rob.Position.X, SimOpt.SimOpts.FieldHeight - rob.GetRadius(SimOpt.SimOpts.FixedBotRadii));

                    rob.ResistiveImpulse += new DoubleVector(0, rob.Velocity.Y * b);
                }
            }
        }

        public static double IntToRadians(int angle)
        {
            return NormaliseAngle(angle / 200.0);
        }

        public static void NetForces(IRobotManager robotManager, Robot rob)
        {
            if (Math.Abs(rob.Velocity.X) < 0.0000001)
                rob.Velocity = new DoubleVector(0, rob.Velocity.Y);

            if (Math.Abs(rob.Velocity.Y) < 0.0000001)
                rob.Velocity = new DoubleVector(rob.Velocity.X, 0);

            PlanetEaters(robotManager, rob);
            FrictionForces(rob);
            SphereDragForces(rob);
            BrownianForces(rob);
            GravityForces(rob);
            VoluntaryForces(rob);
        }

        public static double NormaliseAngle(double an)
        {
            while (an < 0)
                an += 2 * Math.PI;

            while (an > 2 * Math.PI)
                an -= 2 * Math.PI;

            return an;
        }

        public static int RadiansToInt(double angle)
        {
            return (int)(NormaliseAngle(angle) * 200);
        }

        public static void Repel(Robot rob1, Robot rob2)
        {
            double fixedSep;
            DoubleVector fixedSepVector;
            var e = SimOpt.SimOpts.CoefficientElasticity;

            var normal = rob2.Position - rob1.Position;
            var currDist = normal.Magnitude();

            //If both bots are fixed or not moving and they overlap, move their positions directly.  Fixed bots can overlap when shapes sweep them together
            //or when they teleport or materialize on top of each other.  We move them directly apart as they are assumed to have no velocity
            //by scaling the normal vector by the amount they need to be separated.  Each bot is moved half of the needed distance without taking into consideration
            //mass or size.
            if (rob1.IsFixed && rob2.IsFixed || rob1.Velocity.Magnitude() < 0.0001 && rob2.Velocity.Magnitude() < 0.0001)
            {
                fixedSep = (rob1.GetRadius(SimOpt.SimOpts.FixedBotRadii) + rob2.GetRadius(SimOpt.SimOpts.FixedBotRadii) - currDist) / 2;
                fixedSepVector = normal.Unit() * fixedSep;
                rob1.Position -= fixedSepVector;
                rob2.Position += fixedSepVector;
            }
            else
            {
                var totalMass = rob1.Mass + rob2.Mass;
                fixedSep = rob1.GetRadius(SimOpt.SimOpts.FixedBotRadii) + rob2.GetRadius(SimOpt.SimOpts.FixedBotRadii) - currDist;
                fixedSepVector = normal.Unit() * (fixedSep / (1 + Math.Pow(55, 0.3 - e)));
                rob1.Position -= fixedSepVector * (rob2.Mass / totalMass);
                rob2.Position += fixedSepVector * (rob1.Mass / totalMass);
            }

            if (!double.IsFinite(1.0 / normal.Magnitude()))
                return;

            var m1 = rob1.Mass;
            var m2 = rob2.Mass;

            //If a bot is fixed, all the collision energy should be translated to the non-fixed bot so for
            //the purposes of calculating the force applied to the non-fixed bot, treat the fixed one as if it is very massive
            if (rob1.IsFixed)
                m1 = 32000;

            if (rob2.IsFixed)
                m2 = 32000;

            var unit = normal.Unit();
            var vel1 = rob1.Velocity;
            var vel2 = rob2.Velocity;

            //Project the bot's direction vector onto the unit vector and scale by velocity
            //These represent vectors we subtract from the bot's velocity to push the bot in a direction
            //appropriate to the collision.  This would be all we needed if the bots all massed the same.
            //It's possible the bots are already moving away from each other having "collided" last cycle.  If so,
            //we don't want to reverse them again and we don't want to add too much more further acceleration
            var projection = DoubleVector.Dot(vel1, unit) * 0.99;

            if (projection <= 0)
            { // bots are already moving away from one another
                projection = 0.000001;
            }
            var v1 = unit * projection;

            projection = DoubleVector.Dot(vel2, unit) * 0.99; // try damping things down a little

            if (projection >= 0)
            { // bots are already moving away from one another
                projection = -0.000001;
            }
            var v2 = unit * projection;

            //Now we need to factor in the mass of the bots.  These vectors represent the resistance to movement due
            //to the bot's mass
            var v1F = (v2 * (e + 1) * m2 + v1 * (m1 - e * m2)) * (1 / (m1 + m2));
            var v2F = (v1 * (e + 1) * m1 + v2 * (m2 - e * m1)) * (1 / (m1 + m2));

            //No reason to try to try to accelerate fixed bots
            if (!rob1.IsFixed)
                rob1.Velocity -= v1 + v1F;

            if (!rob2.IsFixed)
                rob2.Velocity -= v2 + v2F;

            //Update the touch senses
            Senses.Touch(rob1, rob2.Position.X, rob2.Position.Y);
            Senses.Touch(rob2, rob1.Position.X, rob1.Position.Y);

            //Update last touch variables
            rob1.LastTouched = rob2;
            rob2.LastTouched = rob1;

            //Update the refvars to reflect touching bots.
            Senses.LookOccurr(rob1, rob2);
            Senses.LookOccurr(rob2, rob1);
        }

        public static void TieHooke(Robot rob)
        {
            const double deformation = 20.0; // Tie can stretch or contract this much and no forces are applied.

            if (rob.Ties.Count == 0)
                return;

            foreach (var tie in rob.Ties.ToArray())
            {
                if (!tie.OtherBot.Exists)
                {
                    rob.Ties.Remove(tie);
                    continue;
                }

                var uv = rob.Position - tie.OtherBot.Position;

                var length = uv.Magnitude();

                //delete tie if length > 1000
                //remember length is inverse squareroot
                if (length - rob.GetRadius(SimOpt.SimOpts.FixedBotRadii) - tie.OtherBot.GetRadius(SimOpt.SimOpts.FixedBotRadii) > 1000)
                {
                    Ties.DeleteTie(rob, tie.OtherBot);
                }
                else
                {
                    if (tie.Last > 1)
                        tie.Last--; // Countdown to deleting tie

                    if (tie.Last < 0)
                        tie.Last++; // Countup to hardening tie

                    //EricL 5/7/2006 Following section stiffens ties after 20 cycles
                    if (tie.Last == 1)
                    {
                        Ties.DeleteTie(rob, tie.OtherBot);
                        // k = k - 1 ' Have to do this since deltie slides all the ties down
                    }
                    else
                    { // Stiffen the Tie, the bot is a multibot!
                        if (tie.Last == -1)
                            Ties.Regang(rob, tie);

                        if (length == 0)
                            continue;

                        uv *= 1 / length;

                        //first -kx
                        var displacement = tie.NaturalLength - length;

                        if (!(Math.Abs(displacement) > deformation))
                            continue;

                        displacement = Math.Sign(displacement) * (Math.Abs(displacement) - deformation);
                        var impulse = tie.k * displacement;
                        rob.IndependentImpulse += uv * impulse;

                        //next -bv
                        var vy = rob.Velocity - tie.OtherBot.Velocity;
                        impulse = DoubleVector.Dot(vy, uv) * -tie.b;
                        rob.IndependentImpulse += uv * impulse;
                    }
                }
            }
        }

        public static void TieTorque(Robot rob)
        {
            const double angleslack = 5 * 2 * Math.PI / 360; // 5 degrees

            var mt = 0.0;

            foreach (var tie in rob.Ties.Where(t => t.FixedAngle))
            {
                var anl = Angle(rob.Position.X, rob.Position.Y, tie.OtherBot.Position.X, tie.OtherBot.Position.Y);
                var dlo = AngDiff(anl, rob.Aim); //difference of angle of tie and direction of robot
                var mm = AngDiff(dlo, tie.Angle + tie.Bend);

                tie.Bend = 0;

                if (!(Math.Abs(mm) > angleslack))
                    continue;

                mm = (Math.Abs(mm) - angleslack) * Math.Sign(mm);
                var m = mm * 0.1;
                var dx = tie.OtherBot.Position.X - rob.Position.X;
                var dy = rob.Position.Y - tie.OtherBot.Position.Y;
                var dist = Math.Sqrt(dx * dx + dy * dy);

                var torqueVector = new DoubleVector(-Math.Sin(anl) * m * dist / 10, -Math.Cos(anl) * m * dist / 10);

                tie.OtherBot.IndependentImpulse -= torqueVector;
                rob.IndependentImpulse += torqueVector;
                mt += mm;
            }

            if (mt != 0)
                rob.AngularMomentum = Math.Clamp(mt, -Math.PI / 4, Math.PI / 4);
        }

        private static void BrownianForces(Robot rob)
        {
            if (SimOpt.SimOpts.PhysBrown == 0)
                return;

            var impulse = SimOpt.SimOpts.PhysBrown * 0.5 * ThreadSafeRandom.Local.NextDouble();
            var randomAngle = ThreadSafeRandom.Local.NextDouble() * 2 * Math.PI;

            rob.IndependentImpulse += new DoubleVector(Math.Cos(randomAngle) * impulse, Math.Sin(randomAngle) * impulse);
            rob.AngularMomentum += impulse / 100 * (ThreadSafeRandom.Local.NextDouble() - 0.5); // turning motion due to brownian motion
        }

        private static void FrictionForces(Robot rob)
        {
            if (SimOpt.SimOpts.ZGravity == 0)
                return;

            rob.StaticImpulse = rob.Mass * SimOpt.SimOpts.ZGravity * SimOpt.SimOpts.CoefficientStatic; // * 1 cycle (timestep = 1)

            var impulse = rob.Mass * SimOpt.SimOpts.ZGravity * SimOpt.SimOpts.CoefficientKinetic; // * 1 cycle (timestep = 1)

            //Here we calculate the reduction in angular momentum due to friction
            if (Math.Abs(rob.AngularMomentum) > 0)
            {
                if (impulse < 48)
                    rob.AngularMomentum *= (48 - impulse) / 48;
                else
                    rob.AngularMomentum = 0;

                if (Math.Abs(rob.AngularMomentum) < 0.0000001)
                    rob.AngularMomentum = 0;
            }

            if (impulse > rob.Velocity.Magnitude())
                impulse = rob.Velocity.Magnitude(); // EricL 5/3/2006 Added to insure friction only counteracts

            if (impulse < 0.0000001)
                return;

            //EricL 5/7/2006 Changed to operate directly on velocity
            rob.Velocity -= rob.Velocity.Unit() * impulse; //kinetic friction points in opposite direction of velocity
        }

        private static void GravityForces(Robot rob)
        {
            rob.IndependentImpulse += new DoubleVector(0, SimOpt.SimOpts.YGravity * rob.Mass);
        }

        private static void PlanetEaters(IRobotManager robotManager, Robot rob)
        {
            if (!SimOpt.SimOpts.PlanetEaters || rob.Mass == 0)
                return;

            foreach (var r in robotManager.Robots.Where(r => r.Mass > 0 && r.Exists))
            {
                var posDiff = r.Position - rob.Position;
                var mag = posDiff.Magnitude();

                if (mag == 0)
                    continue;

                var force = SimOpt.SimOpts.PlanetEatersG * (rob.Mass > 192 ? 192 : rob.Mass) * (r.Mass > 192 ? 192 : r.Mass) / (mag * mag);

                posDiff *= 1 / mag;
                posDiff *= force;

                rob.IndependentImpulse += posDiff;
            }
        }

        private static double SphereCd(double velocitymagnitude, double radius)
        {
            if (SimOpt.SimOpts.Viscosity == 0)
                return 0;

            if (velocitymagnitude < 0.00001)
                velocitymagnitude = 0.00001; // Overflow protection

            var reynolds = radius * 2 * velocitymagnitude * SimOpt.SimOpts.Density / SimOpt.SimOpts.Viscosity;

            var y11 = 24 / 300000;
            var y12 = 6 / (1 + Math.Sqrt(300000));
            var y13 = 0.4;

            var y1 = y11 + y12 + y13;
            var y2 = 0.09;

            var alpha = (y2 - y1) * Math.Pow(50000, -2);

            return reynolds switch
            {
                0 => 0,
                < 300000 => 24 / reynolds + 6 / (1 + Math.Sqrt(reynolds)) + 0.4,
                < 350000 => alpha * Math.Pow(reynolds - 300000, 2) + y1,
                < 600000 => 0.09,
                < 4000000 => Math.Pow(reynolds / 600000, 0.55 * y2),
                _ => 0.255,
            };
        }

        private static void SphereDragForces(Robot rob)
        {
            //No Drag if no velocity or no density
            if (rob.Velocity.X == 0 && rob.Velocity.Y == 0 || SimOpt.SimOpts.Density == 0)
                return;

            //Here we calculate the reduction in angular momentum due to fluid density
            //I'm sure there there is a better calculation
            if (Math.Abs(rob.AngularMomentum) > 0)
            {
                if (SimOpt.SimOpts.Density < 0.000001)
                    rob.AngularMomentum *= 1 - SimOpt.SimOpts.Density * 1000000;
                else
                    rob.AngularMomentum = 0;

                if (Math.Abs(rob.AngularMomentum) < 0.0000001)
                    rob.AngularMomentum = 0;
            }

            var mag = rob.Velocity.Magnitude();

            if (mag < 0.0000001)
                return;

            var impulse = 0.5 * SphereCd(mag, rob.GetRadius(SimOpt.SimOpts.FixedBotRadii)) * SimOpt.SimOpts.Density * mag * mag * (Math.PI * Math.Pow(rob.GetRadius(SimOpt.SimOpts.FixedBotRadii), 2));

            if (impulse > mag)
                impulse = mag * 0.99; // Prevents the resistance force from exceeding the velocity!

            rob.Velocity -= rob.Velocity.Unit() * impulse;
        }

        private static void VoluntaryForces(Robot rob)
        {
            //corpses are dead, they don't move around of their own volition
            if (rob.IsCorpse || rob.MovementSysvarsDisabled || rob.DnaDisabled || !rob.Exists || rob.Memory[MemoryAddresses.dirup] == 0 && rob.Memory[MemoryAddresses.dirdn] == 0 && rob.Memory[MemoryAddresses.dirsx] == 0 && rob.Memory[MemoryAddresses.dirdx] == 0)
                return;

            var mult = rob.Mass;

            //yes it's backwards, that's on purpose
            var dir = new DoubleVector(rob.Memory[MemoryAddresses.dirup] - rob.Memory[MemoryAddresses.dirdn], rob.Memory[MemoryAddresses.dirsx] - rob.Memory[MemoryAddresses.dirdx]) * mult;

            var newAccel = new DoubleVector(DoubleVector.Dot(rob.AimVector, dir), DoubleVector.Cross(rob.AimVector, dir));

            //EricL 4/2/2006 Clip the magnitude of the acceleration vector to avoid an overflow crash
            //Its possible to get some really high accelerations here when altzheimers sets in or if a mutation
            //or venom or something writes some really high values into certain mem locations like .up, .dn. etc.
            //This keeps things sane down the road.
            if (newAccel.Magnitude() > SimOpt.SimOpts.MaxVelocity)
            {
                newAccel *= SimOpt.SimOpts.MaxVelocity / newAccel.Magnitude();
            }

            //NewAccel is the impulse vector formed by the robot's internal "engine".
            //Impulse is the integral of Force over time.

            rob.IndependentImpulse += newAccel * SimOpt.SimOpts.PhysMoving;

            //calculates new acceleration and energy values from robot's
            //.up/.dn/.sx/.dx vars
            var energyCost = newAccel.Magnitude() * SimOpt.SimOpts.Costs.VoluntaryMovementCost * SimOpt.SimOpts.Costs.CostMultiplier;

            //EricL 4/4/2006 Clip the energy loss due to voluntary forces.  The total energy loss per cycle could be
            //higher then this due to other nrg losses and this may be redundent with the magnitude clip above, but it
            //helps keep things sane down the road and avoid crashing problems when .nrg goes hugely negative.
            if (energyCost > rob.Energy)
                energyCost = rob.Energy;

            if (energyCost < -1000)
                energyCost = -1000;

            rob.Energy -= energyCost;
        }
    }
}
