using DarwinBots.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using static DarwinBots.Modules.Common;
using static DarwinBots.Modules.Globals;
using static DarwinBots.Modules.Robots;
using static DarwinBots.Modules.SimOpt;

namespace DarwinBots.Modules
{
    internal static class BucketManager
    {
        //Using a bucket size of 4000.  3348 plus twice radius of the largest possible bot is the farthest possible a bot can see.  4000 is a
        //nice round number.
        private const int BucketSize = 4000;

        //This is the buckets Array
        private static Bucket[,] Buckets;

        private static int NumXBuckets;// Field Width divided by bucket size
        private static int NumYBuckets;// Field height divided by bucket size

        /// <summary>
        /// Checks all the bots in the same bucket and surrounding buckets for collisions.
        /// </summary>
        public static void BucketsCollision(robot rob)
        {
            // Check the bucket the bot is in
            CheckBotBucketForCollision(rob, rob.BucketPos);

            // Checks the abjacent buckets
            foreach (var adjBucket in Buckets[rob.BucketPos.X, rob.BucketPos.Y].AdjacentBuckets.Where(b => b.X != -1))
                CheckBotBucketForCollision(rob, adjBucket);
        }

        /// <summary>
        /// Checks all the bots in the same bucket and surrounding buckets for proximity.
        /// </summary>
        /// <returns>
        /// The index of the last viewed object
        /// </returns>
        public static object BucketsProximity(robot rob)
        {
            rob.lastopp = 0;
            rob.lastopptype = 0; // set the default type of object seen to a bot.
            rob.mem[EYEF] = 0;

            for (var x = EyeStart + 1; x < EyeEnd; x++)
                rob.mem[x] = 0;

            //Check the bucket the bot is in
            CheckBotBucketForVision(rob, rob.BucketPos);

            //Check all the adjacent buckets
            foreach (var adjBucket in Buckets[rob.BucketPos.X, rob.BucketPos.Y].AdjacentBuckets.Where(b => b.X != -1))
                CheckBotBucketForVision(rob, adjBucket);

            if (SimOpts.ShapesAreVisable && rob.exist)
                CompareShapes(rob);

            return rob.lastopp;
        }

        public static double EyeSightDistance(int width, robot rob)
        {
            return width == 35 ? 1440 * EyeStrength(rob) : 1440 * (1 - Math.Log(width / 35) / 4) * EyeStrength(rob);
        }

        public static void InitialiseBuckets()
        {
            // Determine the number of buckets.
            NumXBuckets = SimOpts.FieldWidth / BucketSize;
            NumYBuckets = SimOpts.FieldHeight / BucketSize;

            Buckets = new Bucket[NumXBuckets + 1, NumYBuckets + 1];

            // Buckets count along rows, top row, then next...
            for (var y = 0; y < NumYBuckets - 1; y++)
            {
                for (var x = 0; x < NumXBuckets - 1; x++)
                {
                    Buckets[x, y].Robots = new List<robot>();

                    // Set the list of adjacent buckets for this bucket
                    // We take the time to do this here to save the time it would take to compute these every cycle.
                    if (x > 0)
                        Buckets[x, y].AdjacentBuckets.Add(new IntVector(x - 1, y));

                    if (x < NumXBuckets - 1)
                        Buckets[x, y].AdjacentBuckets.Add(new IntVector(x + 1, y));

                    if (y > 0)
                        Buckets[x, y].AdjacentBuckets.Add(new IntVector(x, y - 1));

                    if (y < NumYBuckets - 1)
                        Buckets[x, y].AdjacentBuckets.Add(new IntVector(x, y + 1));

                    if (x > 0 & y > 0)
                        Buckets[x, y].AdjacentBuckets.Add(new IntVector(x - 1, y - 1));

                    if (x > 0 & y < NumYBuckets - 1)
                        Buckets[x, y].AdjacentBuckets.Add(new IntVector(x - 1, y + 1));

                    if (x < NumXBuckets - 1 && y > 0)
                        Buckets[x, y].AdjacentBuckets.Add(new IntVector(x + 1, y - 1));

                    if (x < NumXBuckets - 1 && y < NumYBuckets - 1)
                        Buckets[x, y].AdjacentBuckets.Add(new IntVector(x + 1, y + 1));
                }
            }

            foreach (var rob in rob.Where(r => r.exist))
            {
                rob.BucketPos.X = -2;
                rob.BucketPos.Y = -2;
                UpdateBotBucket(rob);
            }
        }

        // used for exact distances to viewed objects for displaying the eye viewer for the focus bot
        // also erases array elements to retrieve memory
        public static void UpdateBotBucket(robot rob)
        {
            if (!rob.exist)
            {
                Buckets[rob.BucketPos.X, rob.BucketPos.Y].Robots.Remove(rob);
                return;
            }

            var newBucket = new IntVector(rob.BucketPos.X, rob.BucketPos.Y);

            var currentX = (int)Math.Floor(rob.pos.X / BucketSize);
            if (currentX < 0)
                currentX = 0; // Possible bot is off the field

            if (currentX >= NumXBuckets)
                currentX = NumXBuckets - 1; // Possible bot is off the field

            var changed = false;

            if (rob.BucketPos.X != currentX)
            {
                // we've moved off the bucket, update bucket
                newBucket.X = currentX;
                changed = true;
            }

            var currentY = (int)Math.Floor(rob.pos.Y / BucketSize);
            if (currentY < 0)
                currentY = 0; // Possible bot is off the field

            if (currentY >= NumYBuckets)
                currentY = NumYBuckets - 1; // Possible bot is off the field

            if (rob.BucketPos.Y != currentY)
            {
                newBucket.Y = currentY;
                changed = true;
            }

            if (changed)
            {
                Buckets[rob.BucketPos.X, rob.BucketPos.Y].Robots.Remove(rob);
                Buckets[newBucket.X, newBucket.Y].Robots.Add(rob);
                rob.BucketPos = newBucket;
            }
        }

        /// <summary>
        /// Returns the absolute width of an eye.
        /// </summary>
        private static int AbsoluteEyeWidth(int width)
        {
            if (width == 0)
                return 35;
            else
            {
                var val = (width % 1256) + 35;

                if (val <= 0)
                    val = 1256 + val;

                return val;
            }
        }

        private static void CheckBotBucketForCollision(robot rob, IntVector pos)
        {
            foreach (var r in Buckets[pos.X, pos.Y].Robots.Where(i => i != rob && !(i.FName == "Base.txt" && hidepred) && i.AbsNum > rob.AbsNum))
            {
                var distvector = rob.pos - r.pos;
                var dist = rob.radius + r.radius;
                if (distvector.MagnitudeSquare() < (dist * dist))
                    Physics.Repel3(rob, r);
            }
        }

        private static void CheckBotBucketForVision(robot rob, IntVector pos)
        {
            foreach (var r in Buckets[pos.X, pos.Y].Robots.Where(i => i != rob))
                CompareRobots(rob, r);
        }

        private static double CheckDistance(vector botPosition, vector obstacleCorner, vector obstacleSide, vector eyeaimleftvector, vector eyeaimrightvector, out double distright, out double distleft)
        {
            double dist = 32000;
            distleft = 0;
            distright = 0;

            var s = SegmentSegmentIntersect(botPosition, eyeaimleftvector, obstacleCorner, obstacleSide); //Check intersection of left eye range and shape side
            if (s > 0)
                distleft = s * eyeaimleftvector.Magnitude(); //If the left eye range intersects then store the distance of the interesction

            var t = SegmentSegmentIntersect(botPosition, eyeaimrightvector, obstacleCorner, obstacleSide); //Check intersection of right eye range and shape side
            if (t > 0)
                distright = t * eyeaimrightvector.Magnitude(); //If the right eye range intersects, then store the distance of the intersection

            if (distleft > 0 & distright > 0)
                dist = Math.Min(distleft, distright);
            else if (distleft > 0)
                dist = distleft;  //Only left side intersects
            else if (distright > 0)
                dist = distright;  //Only right side intersects

            return dist;
        }

        private static void CompareRobots(robot rob1, robot rob2)
        {
            if (rob2.FName == "Base.txt" && hidepred)
                return;

            var ab = rob2.pos - rob1.pos;
            var edgetoedgedist = ab.Magnitude() - rob1.radius - rob2.radius;

            //Here we compute the maximum possible distance bot N1 can see.  Sight distance is a function of
            //eye width.  Narrower eyes can see farther, wider eyes not so much.  So, we find the narrowest eye
            //and use that to determine the max distance the bot can see.  But first we check the special case
            //where the bot has not changed any of it's eye widths.  Sims generally have lots of veggies which
            //don't bother to do this, so this is worth it.
            var eyesum = rob1.mem[531] + rob1.mem[532] + rob1.mem[533] + rob1.mem[534] + rob1.mem[535] + rob1.mem[536] + rob1.mem[537] + rob1.mem[538] + rob1.mem[539];
            var sightdist = eyesum == 0 ? 1440 * EyeStrength(rob1) : EyeSightDistance(NarrowestEye(rob1), rob1);

            //Now we check the maximum possible distance bot N1 can see against how far away bot N2 is.
            if (edgetoedgedist > sightdist)
                return; // Bot too far away to see

            //If Shapes are see through, then there is no reason to check if a shape blocks a bot
            if (!SimOpts.ShapesAreSeeThrough)
            {
                if (ObstaclesManager.Obstacles.Where(o => o.exist).Any(o => ShapeBlocksBot(rob1, rob2, o)))
                    return;
            }

            //ac and ad are to either end of the bots, while ab is to the center

            var ac = ab.Unit();
            //ac is now unit vector

            var ad = new vector(ac.Y, -ac.X);
            ad *= rob2.radius;
            ad += ab;

            ac = new vector(-ac.Y, ac.X);
            ac *= rob2.radius;
            ac += ab;

            //Coordinates are in the 4th quadrant, so make the y values negative so the math works
            ad.Y = -ad.Y;
            ac.Y = -ac.Y;

            var theta = Physics.NormaliseAngle(Math.Atan2(ad.Y, ad.X));
            var beta = Physics.NormaliseAngle(Math.Atan2(ac.Y, ac.X));

            //lets be sure to just deal with positive angles
            var botspanszero = beta > theta;

            //For each eye
            for (var a = 0; a < 8; a++)
            {
                var eyedist = rob1.mem[EYE1WIDTH + a] == 0 ? 1440 * EyeStrength(rob1) : EyeSightDistance(AbsoluteEyeWidth(rob1.mem[EYE1WIDTH + a]), rob1);
                //Now we check to see if the sight distance for this specific eye is far enough to see bot N2
                if (!(edgetoedgedist <= eyedist)) continue;
                //Check to see if the bot is viewable in this eye
                //First, figure out the direction in radians in which the eye is pointed relative to .aim
                //We have to mod the value and divide by 200 to get radians
                //then since the eyedir values are offsets from their defaults, eye 1 is off from .aim by 4 eye field widths,
                //three for eye2, and so on.
                var eyeaim = rob1.mem[EYE1DIR + a] % 1256 / 200 - Math.PI / 18 * a + Math.PI / 18 * 4 + rob1.aim;

                //It's possible we wrapped 0 so check
                while (eyeaim > 2 * Math.PI)
                    eyeaim -= 2 * Math.PI;

                while (eyeaim < 0)
                    eyeaim += 2 * Math.PI;

                //These are the left and right sides of the field of view for the eye
                double halfeyewidth = rob1.mem[EYE1WIDTH + a] % 1256 / 400;
                while (halfeyewidth > Math.PI - Math.PI / 36)
                    halfeyewidth -= Math.PI;

                while (halfeyewidth < -Math.PI / 36)
                    halfeyewidth += Math.PI;

                var eyeaimleft = eyeaim + halfeyewidth + Math.PI / 36;
                var eyeaimright = eyeaim - halfeyewidth - Math.PI / 36;

                //Check the case where the eye field of view spans 0
                if (eyeaimright < 0)
                    eyeaimright = 2 * Math.PI + eyeaimright;

                if (eyeaimleft > 2 * Math.PI)
                    eyeaimleft -= 2 * Math.PI;

                var eyespanszero = eyeaimleft < eyeaimright;

                // Bot is visiable if either left edge is in eye or right edge is in eye or whole bot spans eye
                //If leftside of bot is in eye or
                //   rightside of bot is in eye or
                //   bot spans eye
                if ((eyeaimleft < theta || theta < eyeaimright || eyespanszero) && (eyeaimleft < theta || eyespanszero || !botspanszero) && (eyeaimright < beta || eyespanszero || !botspanszero) && (eyeaimleft > theta || eyeaimright < beta || !eyespanszero || !botspanszero))
                    continue;

                //The bot is viewable in this eye.
                double eyevalue;

                //Calculate the eyevalue
                if (edgetoedgedist <= 0)
                { // bots overlap
                    eyevalue = 32000;
                }
                else
                {
                    var percentdist = (edgetoedgedist + 10) / eyedist;
                    eyevalue = 1 / (percentdist * percentdist);
                    if (eyevalue > 32000)
                    {
                        eyevalue = 32000;
                    }
                }

                //Check to see if it is closer than other bots we may have seen
                if (rob1.mem[EyeStart + 1 + a] < eyevalue)
                {
                    //It is closer than other bots we may have seen.
                    //Check to see if this eye has the focus
                    if (a == Math.Abs(rob1.mem[FOCUSEYE] + 4) % 9)
                    {
                        //This eye does have the focus
                        //Set the EYEF value and also lastopp so the lookoccur list will get populated later
                        rob1.lastopp = rob.IndexOf(rob2);
                        rob1.mem[EYEF] = (int)eyevalue;
                    }
                    //Set the distance for the eye
                    rob1.mem[EyeStart + 1 + a] = (int)eyevalue;
                }
            }
        }

        private static void CompareShapes(robot rob)
        {
            var D1 = new vector[5];

            var p = new vector[5];
            vector lastopppos = null;
            var sightdist = EyeSightDistance(NarrowestEye(rob), rob) + rob.radius;

            foreach (var o in ObstaclesManager.Obstacles.Where(o => o.exist))
            {
                //Cheap weed out check - check to see if shape is too far away to be seen
                if ((o.pos.X > rob.pos.X + sightdist) || (o.pos.X + o.Width < rob.pos.X - sightdist) || (o.pos.Y > rob.pos.Y + sightdist) || (o.pos.Y + o.Height < rob.pos.Y - sightdist))
                {
                    continue;
                }

                if ((o.pos.X < rob.pos.X) && (o.pos.X + o.Width > rob.pos.X) && (o.pos.Y < rob.pos.Y) && (o.pos.Y + o.Height > rob.pos.Y))
                {
                    //Bot is inside shape!
                    for (var i = 0; i < 8; i++)
                    {
                        rob.mem[EyeStart + 1 + i] = 32000;
                    }
                    rob.lastopp = ObstaclesManager.Obstacles.IndexOf(o);
                    rob.lastopptype = 1;
                    return;
                }

                //Guess we have to actually do the hard work and check...

                //Here are the four sides of the shape
                D1[1] = new vector(o.Width, 0); // top
                D1[2] = new vector(0, o.Height); // left side
                D1[3] = D1[1]; // bottom
                D1[4] = D1[2]; // right side

                //Here are the four corners
                p[1] = o.pos; // NW corner
                p[2] = p[1]; // SW Corner
                p[2].Y = p[1].Y + o.Height;
                p[3] = p[1] + D1[1]; // NE Corner
                p[4] = p[2] + D1[1]; // SE Corner

                //Here is the bot.
                var P0 = rob.pos;

                Direction botLocation;
                //Bots can be in one of eight possible locations relative to a shape.
                // 1 North - Center is above top edge
                // 2 East - Center is to right of right edge
                // 3 South - Center is below bottom edge
                // 4 West - Center is left of left edge
                // 5 NE - Center is North of top and East of right edge
                // 6 SE - Center is South of bottom and East of right edge
                // 7 SW - Center is South of bottom and West of left edge
                // 8 NW - Center is North or top and West of left edge
                // We first need to figure out which the bot is in.

                if (P0.X < p[1].X)
                {
                    //Must be NW, W or SW
                    botLocation = Direction.West;
                    if (P0.Y < p[1].Y)
                    {
                        botLocation = Direction.NorthWest;
                    }
                    else if (P0.Y > p[2].Y)
                    {
                        botLocation = Direction.SouthWest;
                    }
                }
                else if (P0.X > p[3].X)
                {
                    // Must be NE, E or SE
                    botLocation = Direction.East;
                    if (P0.Y < p[1].Y)
                    {
                        botLocation = Direction.NorthEast;
                    }
                    else if (P0.Y > p[2].Y)
                    {
                        botLocation = Direction.SouthEast;
                    }
                }
                else if (P0.Y < p[1].Y)
                {
                    botLocation = Direction.North;
                }
                else
                {
                    botLocation = Direction.South;
                }

                //If the bot is off one of the corners, we have to check two shape edges.
                //If it is off one of the sides, then we only have to check one.

                //For each eye
                for (var a = 0; a < 8; a++)
                {
                    //Now we check to see if the sight distance for this specific eye is far enough to see this specific shape
                    var eyedist = EyeSightDistance(AbsoluteEyeWidth(rob.mem[EYE1WIDTH + a]), rob);

                    if (o.pos.X > rob.pos.X + eyedist || o.pos.X + o.Width < rob.pos.X - eyedist || o.pos.Y > rob.pos.Y + eyedist || o.pos.Y + o.Height < rob.pos.Y - eyedist)
                        continue;

                    //Check to see if the side is viewable in this eye
                    //First, figure out the direction in radians in which the eye is pointed relative to .aim
                    //We have to mod the value and divide by 200 to get radians
                    //then since the eyedir values are offsets from their defaults, eye 1 is off from .aim by 4 eye field widths,
                    //three for eye2, and so on.
                    var eyeaim = rob.mem[EYE1DIR + a] % 1256 / 200 - (Math.PI / 18 * a) + Math.PI / 18 * 4 + rob.aim;

                    eyeaim = Physics.NormaliseAngle(eyeaim);

                    //These are the left and right sides of the field of view for the eye
                    double halfeyewidth = (rob.mem[EYE1WIDTH + a] + 35) / 400;

                    while (halfeyewidth > Math.PI)
                        halfeyewidth -= Math.PI;

                    while (halfeyewidth < 0)
                        halfeyewidth += Math.PI;

                    var eyeaimleft = eyeaim + halfeyewidth;
                    var eyeaimright = eyeaim - halfeyewidth;

                    //Check the case where the eye field of view spans 0
                    if (eyeaimright < 0)
                        eyeaimright += 2 * Math.PI;

                    if (eyeaimleft > 2 * Math.PI)
                        eyeaimleft -= 2 * Math.PI;

                    var eyespanszero = eyeaimleft < eyeaimright;

                    //Now we have the two sides of the eye.  We need to figure out if and where they intersect the shape.

                    //Change the angles to vectors and scale them by the sight distance
                    var eyeaimleftvector = new vector(Math.Cos(eyeaimleft), Math.Sin(eyeaimleft));
                    eyeaimleftvector = eyeaimleftvector.Unit() * eyedist;
                    var eyeaimrightvector = new vector(Math.Cos(eyeaimright), Math.Sin(eyeaimright));
                    eyeaimrightvector = eyeaimrightvector.Unit() * eyedist;

                    eyeaimleftvector.Y = -eyeaimleftvector.Y;
                    eyeaimrightvector.Y = -eyeaimrightvector.Y;

                    var lowestDist = 32000.0; // set to something impossibly big

                    vector closestPoint = null;
                    //First, check here for parts of the shape that may be in the eye and closer than either side of the eye width.
                    //There are two major cases here:  either the bot is off a corner and the eye spanes the corner or the bot is off a side
                    //and the bot eye spans the point on the shape closest to the bot.  For both these cases, we find out what is the closest point
                    //be it a corner or the point on the edge perpendicular to the bot and see if that point is inside the span of the eye.  If
                    //it is, it is closer then either eye edge.
                    //Perhaps do this before edges and not do edges if found?
                    switch (botLocation)
                    {
                        case Direction.North:
                            closestPoint = P0;
                            closestPoint.Y = p[1].Y;
                            break;

                        case Direction.East:
                            closestPoint = P0;
                            closestPoint.X = p[4].X;
                            break;

                        case Direction.South:
                            closestPoint = P0;
                            closestPoint.Y = p[4].Y;
                            break;

                        case Direction.West:
                            closestPoint = P0;
                            closestPoint.X = p[1].X;
                            break;

                        case Direction.NorthEast:
                            closestPoint = p[3];
                            break;

                        case Direction.SouthEast:
                            closestPoint = p[4];
                            break;

                        case Direction.SouthWest:
                            closestPoint = p[2];
                            break;

                        case Direction.NorthWest:
                            closestPoint = p[1];
                            break;
                    }

                    var ab = closestPoint - P0;
                    //Coordinates are in the 4th quadrant, so make the y values negative so the math works
                    ab.Y = -ab.Y;

                    var theta = Physics.NormaliseAngle(Math.Atan2(ab.Y, ab.X));

                    if ((eyeaimleft >= theta && theta >= eyeaimright && !eyespanszero) || (eyeaimleft >= theta && eyespanszero) || (eyeaimright <= theta && eyespanszero))
                    {
                        lowestDist = ab.Magnitude();
                        if (a == 4)
                        {
                            lastopppos = closestPoint;
                        }
                    }

                    if (lowestDist != 32000) continue;
                    // eye doesn't span corner or spot perpendicular to line from bot to shape side
                    if (botLocation.HasFlag(Direction.North))
                    {
                        var dist = CheckDistance(P0, p[1], D1[1], eyeaimleftvector, eyeaimrightvector, out var distright, out var distleft);

                        if (dist > 0 && dist < lowestDist)
                        {
                            lowestDist = dist;
                            if (a == 4)
                            {
                                lastopppos = (distleft < distright) && (distleft > 0)
                                    ? rob.pos + (eyeaimleftvector.Unit() * dist)
                                    : rob.pos + (eyeaimrightvector.Unit() * dist);
                            }
                        }
                    }

                    if (botLocation.HasFlag(Direction.East))
                    {
                        var dist = CheckDistance(P0, p[3], D1[4], eyeaimleftvector, eyeaimrightvector, out var distright, out var distleft);

                        if ((dist > 0) && (dist < lowestDist))
                        {
                            lowestDist = dist;
                            if (a == 4)
                            {
                                lastopppos = (distleft < distright) && (distleft > 0)
                                    ? rob.pos + (eyeaimleftvector.Unit() * dist)
                                    : rob.pos + (eyeaimrightvector.Unit() * dist);
                            }
                        }
                    }

                    if (botLocation.HasFlag(Direction.South))
                    {
                        var dist = CheckDistance(P0, p[2], D1[3], eyeaimleftvector, eyeaimrightvector, out var distright, out var distleft);

                        if ((dist > 0) && (dist < lowestDist))
                        {
                            lowestDist = dist;
                            if (a == 4)
                            {
                                lastopppos = (distleft < distright) && (distleft > 0)
                                    ? rob.pos + (eyeaimleftvector.Unit() * dist)
                                    : rob.pos + (eyeaimrightvector.Unit() * dist);
                            }
                        }
                    }

                    if (botLocation.HasFlag(Direction.West))
                    {
                        var dist = CheckDistance(P0, p[1], D1[2], eyeaimleftvector, eyeaimrightvector, out var distright, out var distleft);

                        if ((dist > 0) && (dist < lowestDist))
                        {
                            lowestDist = dist;
                            if (a == 4)
                            {
                                lastopppos = (distleft < distright) && (distleft > 0)
                                    ? rob.pos + (eyeaimleftvector.Unit() * dist)
                                    : rob.pos + (eyeaimrightvector.Unit() * dist);
                            }
                        }
                    }

                    if (lowestDist < 32000)
                    {
                        var percentdist = (lowestDist - rob.radius + 10) / eyedist;
                        var eyevalue = percentdist <= 0 ? 32000 : (int)(1 / (percentdist * percentdist));
                        if (eyevalue > 32000)
                        {
                            eyevalue = 32000;
                        }

                        if (rob.mem[EyeStart + 1 + a] < eyevalue)
                        {
                            //It is closer than other bots we may have seen.
                            //Check to see if this eye has the focus
                            if (a == Math.Abs(rob.mem[FOCUSEYE] + 4) % 9)
                            {
                                //This eye does have the focus
                                //Set the EYEF value and also lastopp so the lookoccur list will get populated later
                                rob.lastopp = ObstaclesManager.Obstacles.IndexOf(o);
                                rob.lastopptype = 1;
                                rob.mem[EYEF] = eyevalue;
                                rob.lastopppos = lastopppos;
                            }
                            //Set the distance for the eye
                            rob.mem[EyeStart + 1 + a] = eyevalue;
                        }
                    }
                }
            }
        }

        private static double EyeStrength(robot rob)
        {
            const byte EyeEffectiveness = 3; //Botsareus 3/26/2013 For eye strength formula

            double eyestrength;

            if (SimOpts.Pondmode && rob.pos.Y > 1)
                eyestrength = Math.Pow(Math.Pow(EyeEffectiveness / (rob.pos.Y / 2000), SimOpts.Gradient), 6828 / SimOpts.FieldHeight); //Botsareus 3/26/2013 Robots only effected by density, not light intensity
            else
                eyestrength = 1;

            if (!SimOpts.Daytime)
                eyestrength *= 0.8f;

            if (eyestrength > 1)
                eyestrength = 1;

            return eyestrength;
        }

        private static int NarrowestEye(robot rob)
        {
            var NarrowestEye = 1221;
            for (var i = 0; i < 8; i++)
            {
                var Width = AbsoluteEyeWidth(rob.mem[EYE1WIDTH + i]);
                if (Width < NarrowestEye)
                {
                    NarrowestEye = Width;
                }
            }
            return NarrowestEye;
        }

        private static double SegmentSegmentIntersect(vector P0, vector D0, vector P1, vector D1)
        {
            var dotPerp = D0.X * D1.Y - D1.X * D0.Y; // Test for intersection

            if (dotPerp == 0) return 0.0;

            var Delta = P1 - P0;
            var s = Dot(Delta, new vector(D1.Y, -D1.X)) / dotPerp;
            var t = Dot(Delta, new vector(D0.Y, -D0.X)) / dotPerp;

            if (s >= 0 & s <= 1 && t >= 0 & t <= 1)
                return s;

            return 0.0;
        }

        private static bool ShapeBlocksBot(robot rob1, robot rob2, Obstacle o)
        {
            var D1 = new vector[5];
            var p = new vector[5];

            //Cheap weed out check
            if (o.pos.X > Math.Max(rob1.pos.X, rob2.pos.X) || o.pos.X + o.Width < Math.Min(rob1.pos.X, rob2.pos.X) || o.pos.Y > Math.Max(rob1.pos.Y, rob2.pos.Y) || o.pos.Y + o.Height < Math.Min(rob1.pos.Y, rob2.pos.Y))
                return false;

            D1[1] = new vector(0, o.Width); // top
            D1[2] = new vector(o.Height, 0); // left side
            D1[3] = D1[1]; // bottom
            D1[4] = D1[2]; // right side

            p[1] = o.pos;
            p[2] = p[1];
            p[3] = p[1] + D1[2];
            p[4] = p[1] + D1[1];

            var P0 = rob1.pos;
            var D0 = rob2.pos - rob1.pos;

            for (var i = 1; i < 4; i++)
            {
                var numerator = Cross(D0, D1[i]);
                if (numerator == 0) continue;
                var delta = p[i] - P0;
                var s = Cross(delta, D1[i]) / numerator;
                var t = Cross(delta, D0) / numerator;

                if (t >= 0 & t <= 1)
                    return true;

                if (s >= 0 & s <= 1)
                    return true;
            }

            return false;
        }
    }
}
