using Iersera.Model;
using Iersera.Support;
using System;
using System.Linq;
using static Common;
using static Multibots;
using static Robots;
using static SimOpt;
using static Ties;

internal static class Physics
{
    public const double SmudgeFactor = 50;
    private const double AddedMassCoefficientForASphere = 0.5;
    public static double BouyancyScaling { get; set; }
    public static bool boylabldisp { get; set; }

    public static void AddedMass(robot rob)
    {
        rob.AddedMass = AddedMassCoefficientForASphere * SimOpts.Density * (Math.PI * 4 / 3) * Math.Pow(rob.radius, 3);
    }

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

    public static void BorderCollision(robot rob)
    {
        const double b = 0.05;

        if ((rob.pos.X > rob.radius) && (rob.pos.X < SimOpts.FieldWidth - rob.radius) && (rob.pos.Y > rob.radius) && (rob.pos.Y < SimOpts.FieldHeight - rob.radius))
            return;

        rob.mem[214] = 0;

        var smudge = rob.radius + SmudgeFactor;

        var dif = VectorMin(VectorMax(rob.pos, new vector(smudge, smudge)), new vector(SimOpts.FieldWidth - smudge, SimOpts.FieldHeight - smudge));
        var dist = dif - rob.pos;

        if (dist.X != 0)
        {
            if (SimOpts.Dxsxconnected == true)
            {
                if (dist.X < 0)
                    ReSpawn(rob, smudge, rob.pos.Y);
                else
                    ReSpawn(rob, SimOpts.FieldWidth - smudge, rob.pos.Y);
            }
            else
            {
                rob.mem[214] = 1;

                if (rob.pos.X - rob.radius < 0)
                    rob.pos.X = rob.radius;

                if (rob.pos.X + rob.radius > SimOpts.FieldWidth)
                    rob.pos.X = SimOpts.FieldWidth - rob.radius;

                rob.ImpulseRes.X += rob.vel.X * b;
            }
        }

        if (dist.Y != 0)
        {
            if (SimOpts.Updnconnected)
            {
                if (dist.Y < 0)
                    ReSpawn(rob, rob.pos.X, smudge);
                else
                    ReSpawn(rob, rob.pos.X, SimOpts.FieldHeight - smudge);
            }
            else
            {
                rob.mem[214] = 1;

                if (rob.pos.Y - rob.radius < 0)
                    rob.pos.Y = rob.radius;

                if (rob.pos.Y + rob.radius > SimOpts.FieldHeight)
                    rob.pos.Y = SimOpts.FieldHeight - rob.radius;

                rob.ImpulseRes.Y += rob.vel.Y * b;
            }
        }
    }

    public static void CalcMass(robot rob)
    {
        rob.mass = Math.Clamp((rob.body / 1000) + (rob.shell / 200) + (rob.chloroplasts / 32000) * 31680, 1, 32000);
    }

    public static void NetForces(robot rob)
    {
        if (Math.Abs(rob.vel.X) < 0.0000001)
            rob.vel.X = 0;

        if (Math.Abs(rob.vel.Y) < 0.0000001)
            rob.vel.Y = 0;

        PlanetEaters(rob);
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

    public static void PlanetEaters(robot rob)
    {
        if (!SimOpts.PlanetEaters || rob.mass == 0)
            return;

        foreach (var r in Robots.rob.Where(r => r.mass > 0 && r.exist))
        {
            var PosDiff = r.pos - rob.pos;
            var mag = PosDiff.Magnitude();

            if (mag == 0)
                continue;

            var force = SimOpts.PlanetEatersG * (rob.mass > 192 ? 192 : rob.mass) * (r.mass > 192 ? 192 : r.mass) / (mag * mag);

            PosDiff *= 1 / mag;
            PosDiff *= force;

            rob.ImpulseInd += PosDiff;
        }
    }

    public static void Repel3(robot rob1, robot rob2)
    {
        double fixedSep;
        vector fixedSepVector;
        var e = SimOpts.CoefficientElasticity;

        var normal = rob2.pos - rob1.pos;
        var currdist = normal.Magnitude();

        //If both bots are fixed or not moving and they overlap, move their positions directly.  Fixed bots can overlap when shapes sweep them together
        //or when they teleport or materialize on top of each other.  We move them directly apart as they are assumed to have no velocity
        //by scaling the normal vector by the amount they need to be separated.  Each bot is moved half of the needed distance without taking into consideration
        //mass or size.
        if ((rob1.Fixed && rob2.Fixed) || (rob1.vel.Magnitude() < 0.0001 && rob2.vel.Magnitude() < 0.0001))
        {
            fixedSep = (rob1.radius + rob2.radius - currdist) / 2;
            fixedSepVector = normal.Unit() * fixedSep;
            rob1.pos -= fixedSepVector;
            rob2.pos += fixedSepVector;
        }
        else
        {
            //Botsareus 6/18/2016 Still slowly move robots appart to cancel compressive events
            var TotalMass = rob1.mass + rob2.mass;
            fixedSep = rob1.radius + rob2.radius - currdist;
            fixedSepVector = normal.Unit() * (fixedSep / (1 + Math.Pow(55, 0.3 - e)));
            rob1.pos -= fixedSepVector * (rob2.mass / TotalMass); //Botsareus 7/4/2016 Factor in mass of robots (apply inverted)
            rob2.pos -= fixedSepVector * (rob1.mass / TotalMass);
        }

        if (1 / normal.Magnitude() != -1)
        {
            //vectorinvmagnitude = inverse magnitude.  Returns -1# if divide by zero
            var M1 = rob1.mass;
            var M2 = rob2.mass;

            //If a bot is fixed, all the collision energy should be translated to the non-fixed bot so for
            //the purposes of calculating the force applied to the non-fixed bot, treat the fixed one as if it is very massive
            if (rob1.Fixed)
            {
                M1 = 32000;
            }
            if (rob2.Fixed)
            {
                M2 = 32000;
            }

            var unit = normal.Unit();
            var vel1 = rob1.vel;
            var vel2 = rob2.vel;

            //Project the bot's direction vector onto the unit vector and scale by velocity
            //These represent vectors we subtract from the bot's velocity to push the bot in a direction
            //appropriate to the collision.  This would be all we needed if the bots all massed the same.
            //It's possible the bots are already moving away from each other having "collided" last cycle.  If so,
            //we don't want to reverse them again and we don't want to add too much more further acceleration
            var projection = Dot(vel1, unit) * 0.99;

            if (projection <= 0)
            { // bots are already moving away from one another
                projection = 0.000001;
            }
            var V1 = unit * projection;

            projection = Dot(vel2, unit) * 0.99; // try damping things down a little

            if (projection >= 0)
            { // bots are already moving away from one another
                projection = -0.000001;
            }
            var V2 = unit * projection;

            //Now we need to factor in the mass of the bots.  These vectors represent the resistance to movement due
            //to the bot's mass
            var V1f = ((V2 * (e + 1) * M2) + (V1 * (M1 - e * M2))) * (1 / (M1 + M2));
            var V2f = ((V1 * (e + 1) * M1) + (V2 * (M2 - e * M1))) * (1 / (M1 + M2));

            //No reason to try to try to accelerate fixed bots
            if (!rob1.Fixed)
                rob1.vel -= V1 + V1f;

            if (!rob2.Fixed)
                rob2.vel -= V2 + V2f;

            //Update the touch senses
            Senses.Touch(rob1, rob2.pos.X, rob2.pos.Y);
            Senses.Touch(rob2, rob1.pos.X, rob1.pos.Y);

            //Update last touch variables
            rob1.lasttch = rob2;
            rob2.lasttch = rob1;

            //Update the refvars to reflect touching bots.
            Senses.LookOccurr(rob1, rob2);
            Senses.LookOccurr(rob2, rob1);
        }
    }

    public static void SphereDragForces(robot rob)
    {
        //No Drag if no velocity or no density
        if ((rob.vel.X == 0 && rob.vel.Y == 0) || SimOpts.Density == 0)
            return;

        //Here we calculate the reduction in angular momentum due to fluid density
        //I'm sure there there is a better calculation
        if (Math.Abs(rob.ma) > 0)
        {
            if (SimOpts.Density < 0.000001)
                rob.ma *= (1 - (SimOpts.Density * 1000000));
            else
                rob.ma = 0;

            if (Math.Abs(rob.ma) < 0.0000001)
                rob.ma = 0;
        }

        var mag = rob.vel.Magnitude();

        if (mag < 0.0000001)
            return;

        var Impulse = 0.5 * SphereCd(mag, rob.radius) * SimOpts.Density * mag * mag * (Math.PI * Math.Pow(rob.radius, 2));

        if (Impulse > mag)
            Impulse = mag * 0.99; // Prevents the resistance force from exceeding the velocity!

        rob.vel -= rob.vel.Unit() * Impulse;
    }

    public static void TieHooke(robot rob)
    {
        const double deformation = 20.0; // Tie can stretch or contract this much and no forces are applied.

        if (rob.Ties.Count == 0)
            return;

        foreach (var tie in rob.Ties.ToArray())
        {
            if (!tie.OtherBot.exist)
            {
                rob.Ties.Remove(tie);
                continue;
            }

            var uv = rob.pos - tie.OtherBot.pos;

            var Length = uv.Magnitude();

            //delete tie if length > 1000
            //remember length is inverse squareroot
            if (Length - rob.radius - tie.OtherBot.radius > 1000)
            {
                DeleteTie(rob, tie.OtherBot);
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
                    DeleteTie(rob, tie.OtherBot);
                    // k = k - 1 ' Have to do this since deltie slides all the ties down
                }
                else
                { // Stiffen the Tie, the bot is a multibot!
                    if (tie.Last == -1)
                        Ties.Regang(rob, tie);

                    if (Length != 0)
                    {
                        uv *= 1 / Length;

                        //first -kx
                        var displacement = tie.NaturalLength - Length;

                        if (Math.Abs(displacement) > deformation)
                        {
                            displacement = Math.Sign(displacement) * (Math.Abs(displacement) - deformation);
                            var Impulse = tie.k * displacement;
                            rob.ImpulseInd += uv * Impulse;

                            //next -bv
                            var vy = rob.vel - tie.OtherBot.vel;
                            Impulse = Dot(vy, uv) * -tie.b;
                            rob.ImpulseInd += uv * Impulse;
                        }
                    }
                }
            }
        }
    }

    public static void TieTorque(robot rob)
    {
        const double angleslack = 5 * 2 * Math.PI / 360; // 5 degrees

        var mt = 0.0;

        foreach (var tie in rob.Ties.Where(t => t.FixedAngle))
        {
            var anl = Angle(rob.pos.X, rob.pos.Y, tie.OtherBot.pos.X, tie.OtherBot.pos.Y);
            var dlo = AngDiff(anl, rob.aim); //difference of angle of tie and direction of robot
            var mm = AngDiff(dlo, tie.Angle + tie.Bend);

            tie.Bend = 0;

            if (Math.Abs(mm) > angleslack)
            {
                mm = (Math.Abs(mm) - angleslack) * Math.Sign(mm);
                var m = mm * 0.1;
                var dx = tie.OtherBot.pos.X - rob.pos.X;
                var dy = rob.pos.Y - tie.OtherBot.pos.Y;
                var dist = Math.Sqrt(dx * dx + dy * dy);
                //experimental limits to acceleration

                var nax = Math.Clamp(-Math.Sin(anl) * m * dist / 10, -100, 100);
                var nay = Math.Clamp(-Math.Cos(anl) * m * dist / 10, -100, 100);

                //EricL 4/24/2006 This is the torque vector on robot t from it's movement of the tie
                var TorqueVector = new vector(-Math.Sin(anl) * m * dist / 10, -Math.Cos(anl) * m * dist / 10);

                tie.OtherBot.ImpulseInd -= TorqueVector; //EricL Subtact the torque for bot n.
                rob.ImpulseInd += TorqueVector; //EricL Add the acceleration for bot t
                mt += mm;
            }
        }

        if (mt != 0)
            rob.ma = Math.Clamp(mt, -Math.PI / 4, Math.PI / 4);
    }

    public static void VoluntaryForces(robot rob)
    {
        //corpses are dead, they don't move around of their own volition
        if (rob.Corpse || rob.DisableMovementSysvars || rob.DisableDNA || !rob.exist || ((rob.mem[dirup] == 0) && (rob.mem[dirdn] == 0) && (rob.mem[dirsx] == 0) && (rob.mem[dirdx] == 0)))
            return;

        var mult = rob.NewMove == false ? rob.mass : 1;

        //yes it's backwards, that's on purpose
        var dir = new vector(rob.mem[dirup] - rob.mem[dirdn], rob.mem[dirsx] - rob.mem[dirdx]) * mult;

        var NewAccel = new vector(Dot(rob.aimvector, dir), Cross(rob.aimvector, dir));

        //EricL 4/2/2006 Clip the magnitude of the acceleration vector to avoid an overflow crash
        //Its possible to get some really high accelerations here when altzheimers sets in or if a mutation
        //or venom or something writes some really high values into certain mem locations like .up, .dn. etc.
        //This keeps things sane down the road.
        if (NewAccel.Magnitude() > SimOpts.MaxVelocity)
        {
            NewAccel *= SimOpts.MaxVelocity / NewAccel.Magnitude();
        }

        //NewAccel is the impulse vector formed by the robot's internal "engine".
        //Impulse is the integral of Force over time.

        rob.ImpulseInd += NewAccel * SimOpts.PhysMoving;

        //calculates new acceleration and energy values from robot's
        //.up/.dn/.sx/.dx vars
        var EnergyCost = NewAccel.Magnitude() * SimOpts.Costs[MOVECOST] * SimOpts.Costs[COSTMULTIPLIER];

        //EricL 4/4/2006 Clip the energy loss due to voluntary forces.  The total energy loss per cycle could be
        //higher then this due to other nrg losses and this may be redundent with the magnitude clip above, but it
        //helps keep things sane down the road and avoid crashing problems when .nrg goes hugely negative.
        if (EnergyCost > rob.nrg)
            EnergyCost = rob.nrg;

        if (EnergyCost < -1000)
            EnergyCost = -1000;

        rob.nrg -= EnergyCost;
    }

    private static void BrownianForces(robot rob)
    {
        if (SimOpts.PhysBrown == 0)
            return;

        var Impulse = SimOpts.PhysBrown * 0.5 * ThreadSafeRandom.Local.NextDouble();
        var RandomAngle = ThreadSafeRandom.Local.NextDouble() * 2 * Math.PI;

        rob.ImpulseInd += new vector(Math.Cos(RandomAngle) * Impulse, Math.Sin(RandomAngle) * Impulse);
        rob.ma += Impulse / 100 * (ThreadSafeRandom.Local.NextDouble() - 0.5); // turning motion due to brownian motion
    }

    private static void FrictionForces(robot rob)
    {
        if (SimOpts.Zgravity == 0)
            return;

        var ZGrav = SimOpts.Zgravity;

        rob.ImpulseStatic = rob.mass * ZGrav * SimOpts.CoefficientStatic; // * 1 cycle (timestep = 1)

        var Impulse = rob.mass * ZGrav * SimOpts.CoefficientKinetic; // * 1 cycle (timestep = 1)

        //Here we calculate the reduction in angular momentum due to friction
        if (Math.Abs(rob.ma) > 0)
        {
            if (Impulse < 48)
            {
                rob.ma *= (48 - Impulse) / 48;
            }
            else
            {
                rob.ma = 0;
            }
            if (Math.Abs(rob.ma) < 0.0000001)
            {
                rob.ma = 0;
            }
        }

        if (Impulse > rob.vel.Magnitude())
        {
            Impulse = rob.vel.Magnitude(); // EricL 5/3/2006 Added to insure friction only counteracts
        }

        if (Impulse < 0.0000001)
        {
            return;
        }

        //EricL 5/7/2006 Changed to operate directly on velocity
        rob.vel -= rob.vel.Unit() * Impulse; //kinetic friction points in opposite direction of velocity
    }

    private static void GravityForces(robot rob)
    {
        if ((SimOpts.Ygravity == 0 || !SimOpts.Pondmode || SimOpts.Updnconnected))
        {
            rob.ImpulseInd += new vector(0, SimOpts.Ygravity * rob.mass);
        }
        else
        {
            if (rob.Bouyancy > 0)
                rob.nrg -= SimOpts.Ygravity / SimOpts.PhysMoving * (rob.mass > 192 ? 192 : rob.mass) * SimOpts.Costs[MOVECOST] * SimOpts.Costs[COSTMULTIPLIER] * rob.Bouyancy;

            if (((1 / BouyancyScaling) - rob.pos.Y / SimOpts.FieldHeight) > rob.Bouyancy)
                rob.ImpulseInd += new vector(0, SimOpts.Ygravity * rob.mass);
            else
                rob.ImpulseInd += new vector(0, -SimOpts.Ygravity * rob.mass);
        }
    }

    private static double SphereCd(double velocitymagnitude, double radius)
    {
        if (SimOpts.Viscosity == 0)
            return 0;

        if (velocitymagnitude < 0.00001)
            velocitymagnitude = 0.00001; // Overflow protection

        var Reynolds = radius * 2 * velocitymagnitude * SimOpts.Density / SimOpts.Viscosity;

        var y11 = 24 / 300000;
        var y12 = 6 / (1 + Math.Sqrt(300000));
        var y13 = 0.4;

        var y1 = y11 + y12 + y13;
        var y2 = 0.09;

        var alpha = (y2 - y1) * Math.Pow(50000, -2);

        return Reynolds switch
        {
            0 => 0,
            < 300000 => 24 / Reynolds + 6 / (1 + Math.Sqrt(Reynolds)) + 0.4,
            < 350000 => alpha * Math.Pow(Reynolds - 300000, 2) + y1,
            < 600000 => 0.09,
            < 4000000 => Math.Pow(Reynolds / 600000, 0.55 * y2),
            _ => 0.255,
        };
    }

    /*
    'Botsareus 9/30/2014 Returns true if robot does not exsist
    */
    /*
    ' calculates angle between (x1,y1) and (x2,y2)
    */
    /*
    ' normalizes angle in 0,2pi
    */
    /*
    ' calculates difference between two angles
    */
    /*
    '' calculates torque generated by all ties on robots
    */
    /*
    '' calculates acceleration due to the medium action on the links
    '' (used for swimming)
    'Public Sub Swimming()
    '  Dim vxle As Long
    '  Dim vyle As Long
    '  Dim nd As node
    '  Dim t As Integer, p As Integer
    '  Dim j As Byte
    '  Dim anle As Single, anve As Single, ancm As Single
    '  Dim vea As Long, lle As Long
    '  Dim cnorm As Single
    '  Dim Fx As Long, Fy As Long
    '  Set nd = rlist.firstnode
    '  While Not (nd Is rlist.last)
    '    t = nd.robn
    '    With rob(t)
    '      If .Corpse = False And .Numties > 0 Then  'new conditions to prevent parsing corpses and robots without ties.
    '        j = 1
    '        While .Ties(j).pnt > 0
    '          p = .Ties(j).pnt
    '          vxle = (.vx + rob(p).vx) / 2  'average x velocity
    '          vyle = (.vy + rob(p).vy) / 2  'average y velocity
    '          anle = angle(.x, .y, rob(p).x, rob(p).y)  'Angle between robots
    '          anve = angle(0, 0, vxle, vyle)            'Angle of vector velocity
    '          ancm = anve - anle                        'Combined angle
    '          vea = Sqr(vxle ^ 2 + vyle ^ 2)            'velocity along vector
    '          lle = Sqr((.x - rob(p).x) ^ 2 + (.y - rob(p).y) ^ 2)  'distance between robots
    '          cnorm = Sin(ancm) * vea * lle * SimOpts.PhysSwim  'Swim force
    '          'cnorm = Average Cross velocity * distance between bots * PhysSwim
    '          Fx = cnorm * Sin(anle) / 800
    '          Fy = cnorm * Cos(anle) / 800
    '          .ax = .ax + Fx
    '          .ay = .ay + Fy
    '          rob(p).ax = rob(p).ax + Fx
    '          rob(p).ay = rob(p).ay + Fy
    '          j = j + 1
    '        Wend
    '      End If
    '    End With
    '    Set nd = nd.pn
    '  Wend
    'End Sub
    */
    /*
    'EricL - My attempt to back port 2.5 physics to address collision detection
    'with a bunch of extra tweaks figurred via trial and error.
    */
}
