using DarwinBots.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarwinBots.Modules
{
    internal class BucketManager
    {
        /// <summary>
        /// The width and height of each bucket.
        /// </summary>
        /// <remarks>
        /// 3348 plus twice radius of the largest possible bot is the farthest possible a bot can see.  4000 is a nice round number.
        /// </remarks>
        private const int BucketSize = 4000;

        private const int EyeEffectiveness = 3;

        /// <summary>
        /// The main buckets array.
        /// </summary>
        private readonly Bucket[,] _buckets;

        /// <summary>
        /// The number buckets in the x direction.
        /// </summary>
        /// <remarks>
        /// Field width divided by bucket size.
        /// </remarks>
        private readonly IntVector _numBuckets;

        /// <summary>
        /// A reference to the simulations options
        /// </summary>
        private readonly SimOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="BucketManager"/> class.
        /// </summary>
        /// <param name="opts">The simulation options.</param>
        public BucketManager(SimOptions opts)
        {
            _options = opts;

            _numBuckets = new IntVector(opts.FieldWidth / BucketSize, opts.FieldHeight / BucketSize);

            _buckets = new Bucket[_numBuckets.X + 1, _numBuckets.Y + 1];

            for (var y = 0; y < _numBuckets.Y - 1; y++)
            {
                for (var x = 0; x < _numBuckets.X - 1; x++)
                {
                    _buckets[x, y].Robots = new List<robot>();

                    if (x > 0)
                        _buckets[x, y].AdjacentBuckets.Add(new IntVector(x - 1, y));

                    if (x < _numBuckets.X - 1)
                        _buckets[x, y].AdjacentBuckets.Add(new IntVector(x + 1, y));

                    if (y > 0)
                        _buckets[x, y].AdjacentBuckets.Add(new IntVector(x, y - 1));

                    if (y < _numBuckets.Y - 1)
                        _buckets[x, y].AdjacentBuckets.Add(new IntVector(x, y + 1));

                    if (x > 0 & y > 0)
                        _buckets[x, y].AdjacentBuckets.Add(new IntVector(x - 1, y - 1));

                    if (x > 0 & y < _numBuckets.Y - 1)
                        _buckets[x, y].AdjacentBuckets.Add(new IntVector(x - 1, y + 1));

                    if (x < _numBuckets.X - 1 && y > 0)
                        _buckets[x, y].AdjacentBuckets.Add(new IntVector(x + 1, y - 1));

                    if (x < _numBuckets.X - 1 && y < _numBuckets.Y - 1)
                        _buckets[x, y].AdjacentBuckets.Add(new IntVector(x + 1, y + 1));
                }
            }

            foreach (var rob in Globals.RobotsManager.Robots.Where(r => r.exist))
            {
                rob.BucketPos = new IntVector(-2, -2);
                UpdateBotBucket(rob);
            }
        }

        /// <summary>
        /// Checks all the bots in the same bucket and surrounding buckets for collisions.
        /// </summary>
        /// <param name="rob">The robot to check.</param>
        public void BucketsCollision(robot rob)
        {
            CheckBotBucketForCollision(rob, rob.BucketPos);

            foreach (var adjBucket in _buckets[rob.BucketPos.X, rob.BucketPos.Y].AdjacentBuckets.Where(b => b.X != -1))
                CheckBotBucketForCollision(rob, adjBucket);
        }

        /// <summary>
        /// Checks all the bots in the same bucket and surrounding buckets for proximity.
        /// </summary>
        /// <returns>
        /// The last viewed object.
        /// </returns>
        /// <param name="rob">The robot to check.</param>
        public object BucketsProximity(robot rob)
        {
            rob.lastopp = 0;
            rob.mem[MemoryAddresses.EYEF] = 0;

            for (var x = MemoryAddresses.EyeStart + 1; x < MemoryAddresses.EyeEnd; x++)
                rob.mem[x] = 0;

            CheckBotBucketForVision(rob, rob.BucketPos);

            foreach (var adjBucket in _buckets[rob.BucketPos.X, rob.BucketPos.Y].AdjacentBuckets.Where(b => b.X != -1))
                CheckBotBucketForVision(rob, adjBucket);

            if (_options.ShapesAreVisable && rob.exist)
                CompareShapes(rob);

            return rob.lastopp;
        }

        /// <summary>
        /// Updates the bucket the bot is sat in.
        /// </summary>
        /// <param name="rob">The rob to place.</param>
        public void UpdateBotBucket(robot rob)
        {
            if (!rob.exist)
            {
                _buckets[rob.BucketPos.X, rob.BucketPos.Y].Robots.Remove(rob);
                return;
            }

            var current = rob.pos / BucketSize;
            current = DoubleVector.Floor(current);
            current = DoubleVector.Clamp(current, DoubleVector.Zero, _numBuckets);

            var newBucket = current.ToIntVector();

            if (rob.BucketPos == newBucket) return;

            _buckets[rob.BucketPos.X, rob.BucketPos.Y].Robots.Remove(rob);
            _buckets[newBucket.X, newBucket.Y].Robots.Add(rob);
            rob.BucketPos = newBucket;
        }

        private static double CheckDistance(DoubleVector botPosition, DoubleVector obstacleCorner, DoubleVector obstacleSide, DoubleVector eyeAimLeftVector, DoubleVector eyeAimRightVector, out double distRight, out double distLeft)
        {
            double dist = 32000;
            distLeft = 0;
            distRight = 0;

            var s = SegmentSegmentIntersect(botPosition, eyeAimLeftVector, obstacleCorner, obstacleSide); //Check intersection of left eye range and shape side
            if (s > 0)
                distLeft = s * eyeAimLeftVector.Magnitude(); //If the left eye range intersects then store the distance of the interesction

            var t = SegmentSegmentIntersect(botPosition, eyeAimRightVector, obstacleCorner, obstacleSide); //Check intersection of right eye range and shape side
            if (t > 0)
                distRight = t * eyeAimRightVector.Magnitude(); //If the right eye range intersects, then store the distance of the intersection

            if (distLeft > 0 & distRight > 0)
                dist = Math.Min(distLeft, distRight);
            else if (distLeft > 0)
                dist = distLeft;  //Only left side intersects
            else if (distRight > 0)
                dist = distRight;  //Only right side intersects

            return dist;
        }

        /// <summary>
        /// Calculates the position along p0 + sd0 where it intersects p1 + td1
        /// </summary>
        /// <returns>
        /// The percentage along p0 + sd0 where the intersection lies.  Returns 0 if there is no intersection.
        /// </returns>
        private static double SegmentSegmentIntersect(DoubleVector p0, DoubleVector d0, DoubleVector p1, DoubleVector d1)
        {
            var dotPerp = d0.X * d1.Y - d1.X * d0.Y; // Test for intersection

            if (dotPerp == 0) return 0.0;

            var delta = p1 - p0;

            var s = DoubleVector.Dot(delta, new DoubleVector(d1.Y, -d1.X)) / dotPerp;
            var t = DoubleVector.Dot(delta, new DoubleVector(d0.Y, -d0.X)) / dotPerp;

            if (s >= 0 & s <= 1 && t >= 0 & t <= 1)
                return s;

            return 0.0;
        }

        private static bool ShapeBlocksBot(robot rob1, robot rob2, Obstacle o)
        {
            var d1 = new DoubleVector[5];
            var p = new DoubleVector[5];

            if (o.pos.X > Math.Max(rob1.pos.X, rob2.pos.X) || o.pos.X + o.Width < Math.Min(rob1.pos.X, rob2.pos.X) || o.pos.Y > Math.Max(rob1.pos.Y, rob2.pos.Y) || o.pos.Y + o.Height < Math.Min(rob1.pos.Y, rob2.pos.Y))
                return false;

            d1[1] = new DoubleVector(0, o.Width); // top
            d1[2] = new DoubleVector(o.Height, 0); // left side
            d1[3] = d1[1]; // bottom
            d1[4] = d1[2]; // right side

            p[1] = o.pos;
            p[2] = p[1];
            p[3] = p[1] + d1[2];
            p[4] = p[1] + d1[1];

            var p0 = rob1.pos;
            var d0 = rob2.pos - rob1.pos;

            for (var i = 1; i < 4; i++)
            {
                var numerator = DoubleVector.Cross(d0, d1[i]);
                if (numerator == 0) continue;
                var delta = p[i] - p0;
                var s = DoubleVector.Cross(delta, d1[i]) / numerator;
                var t = DoubleVector.Cross(delta, d0) / numerator;

                if (t >= 0 & t <= 1)
                    return true;

                if (s >= 0 & s <= 1)
                    return true;
            }

            return false;
        }

        private void CheckBotBucketForCollision(robot rob, IntVector pos)
        {
            foreach (var r in _buckets[pos.X, pos.Y].Robots.Where(i => i != rob && i.AbsNum > rob.AbsNum))
            {
                var distvector = rob.pos - r.pos;
                var dist = rob.Radius + r.Radius;
                if (distvector.MagnitudeSquare() < dist * dist)
                    Physics.Repel(rob, r);
            }
        }

        private void CheckBotBucketForVision(robot rob, IntVector pos)
        {
            foreach (var r in _buckets[pos.X, pos.Y].Robots.Where(i => i != rob))
                CompareRobots(rob, r);
        }

        private void CompareRobots(robot rob1, robot rob2)
        {
            var ab = rob2.pos - rob1.pos;
            var edgeToEdgeDistance = ab.Magnitude() - rob1.Radius - rob2.Radius;

            var sightDistances = Enumerable.Range(0, 8)
                .Select(a => EyeSightDistance(rob1.mem[MemoryAddresses.EYE1WIDTH + a], rob1))
                .ToArray();

            var maxSightDistance = sightDistances.Max();

            // Now check the maximum possible distance bot N1 can see against how far away bot N2 is.
            if (edgeToEdgeDistance > maxSightDistance)
                return; // Bot too far away to see

            // If Shapes are see through, then there is no reason to check if a shape blocks a bot
            if (!_options.ShapesAreSeeThrough)
            {
                if (Globals.ObstacleManager.Obstacles.Where(o => o.exist).Any(o => ShapeBlocksBot(rob1, rob2, o)))
                    return;
            }

            // ac and ad are to either end of the bots, while ab is to the center

            var ac = ab.Unit();

            var ad = new DoubleVector(ac.Y, -ac.X);
            ad *= rob2.Radius;
            ad += ab;

            ac = new DoubleVector(-ac.Y, ac.X);
            ac *= rob2.Radius;
            ac += ab;

            // Coordinates are in the 4th quadrant, so make the y values negative so the math works
            ad = ad with { Y = -ad.Y };
            ac = ad with { Y = -ac.Y };

            var theta = Physics.NormaliseAngle(Math.Atan2(ad.Y, ad.X));
            var beta = Physics.NormaliseAngle(Math.Atan2(ac.Y, ac.X));

            var botspanszero = beta > theta;

            for (var a = 0; a < 8; a++)
            {
                var eyeDistance = sightDistances[a];

                if (!(edgeToEdgeDistance <= eyeDistance)) continue;

                var eyeAim = Physics.NormaliseAngle(Physics.IntToRadians(rob1.mem[MemoryAddresses.EYE1DIR + a]) - Math.PI / 18 * a + Math.PI / 18 * 4 + rob1.aim);

                var halfEyeWidth = Physics.IntToRadians(rob1.mem[MemoryAddresses.EYE1WIDTH + a]) / 2;

                while (halfEyeWidth > Math.PI - Math.PI / 36)
                    halfEyeWidth -= Math.PI;

                while (halfEyeWidth < -Math.PI / 36)
                    halfEyeWidth += Math.PI;

                var eyeAimLeft = eyeAim + halfEyeWidth + Math.PI / 36;
                var eyeAimRight = eyeAim - halfEyeWidth - Math.PI / 36;

                if (eyeAimRight < 0)
                    eyeAimRight = 2 * Math.PI + eyeAimRight;

                if (eyeAimLeft > 2 * Math.PI)
                    eyeAimLeft -= 2 * Math.PI;

                var eyeSpansZero = eyeAimLeft < eyeAimRight;

                if ((eyeAimLeft < theta || theta < eyeAimRight || eyeSpansZero) && (eyeAimLeft < theta || eyeSpansZero || !botspanszero) && (eyeAimRight < beta || eyeSpansZero || !botspanszero) && (eyeAimLeft > theta || eyeAimRight < beta || !eyeSpansZero || !botspanszero))
                    continue;

                double eyeValue;

                if (edgeToEdgeDistance <= 0)
                {
                    // bots overlap
                    eyeValue = 32000;
                }
                else
                {
                    var percentDist = (edgeToEdgeDistance + 10) / eyeDistance;
                    eyeValue = 1 / (percentDist * percentDist);
                    eyeValue = Math.Min(eyeValue, 32000);
                }

                // Check to see if it is closer than other bots we may have seen
                if (!(rob1.mem[MemoryAddresses.EyeStart + 1 + a] < eyeValue)) continue;

                // It is closer than other bots we may have seen.
                // Check to see if this eye has the focus
                if (a == Math.Abs(rob1.mem[MemoryAddresses.FOCUSEYE] + 4) % 9)
                {
                    // This eye does have the focus
                    // Set the EYEF value and also lastopp so the lookoccur list will get populated later
                    rob1.lastopp = rob2;
                    rob1.mem[MemoryAddresses.EYEF] = (int)eyeValue;
                }
                // Set the distance for the eye
                rob1.mem[MemoryAddresses.EyeStart + 1 + a] = (int)eyeValue;
            }
        }

        private void CompareShapes(robot rob)
        {
            var d1 = new DoubleVector[5];
            var p = new DoubleVector[5];

            DoubleVector lastopppos = null;

            var sightDistances = Enumerable.Range(0, 8)
                .Select(a => EyeSightDistance(rob.mem[MemoryAddresses.EYE1WIDTH + a], rob))
                .ToArray();

            var maxSightDistance = sightDistances.Max();

            foreach (var o in Globals.ObstacleManager.Obstacles.Where(o => o.exist))
            {
                // Check to see if shape is too far away to be seen
                if (o.pos.X > rob.pos.X + maxSightDistance || o.pos.X + o.Width < rob.pos.X - maxSightDistance || o.pos.Y > rob.pos.Y + maxSightDistance || o.pos.Y + o.Height < rob.pos.Y - maxSightDistance)
                    continue;

                if (o.pos.X < rob.pos.X && o.pos.X + o.Width > rob.pos.X && o.pos.Y < rob.pos.Y && o.pos.Y + o.Height > rob.pos.Y)
                {
                    // Bot is inside shape
                    for (var i = 0; i < 8; i++)
                        rob.mem[MemoryAddresses.EyeStart + 1 + i] = 32000;

                    rob.lastopp = o;
                    return;
                }

                // Here are the four sides of the shape
                d1[1] = new DoubleVector(o.Width, 0); // top
                d1[2] = new DoubleVector(0, o.Height); // left side
                d1[3] = d1[1]; // bottom
                d1[4] = d1[2]; // right side

                // Here are the four corners
                p[1] = o.pos; // NW corner
                p[2] = p[1] with { Y = p[1].Y + o.Height }; // SW Corner
                p[3] = p[1] + d1[1]; // NE Corner
                p[4] = p[2] + d1[1]; // SE Corner

                var p0 = rob.pos;

                Direction botLocation;
                // Bots can be in one of eight possible locations relative to a shape.
                // 1 North - Center is above top edge
                // 2 East - Center is to right of right edge
                // 3 South - Center is below bottom edge
                // 4 West - Center is left of left edge
                // 5 NE - Center is North of top and East of right edge
                // 6 SE - Center is South of bottom and East of right edge
                // 7 SW - Center is South of bottom and West of left edge
                // 8 NW - Center is North or top and West of left edge

                if (p0.X < p[1].X)
                {
                    //Must be NW, W or SW
                    botLocation = Direction.West;
                    if (p0.Y < p[1].Y)
                    {
                        botLocation = Direction.NorthWest;
                    }
                    else if (p0.Y > p[2].Y)
                    {
                        botLocation = Direction.SouthWest;
                    }
                }
                else if (p0.X > p[3].X)
                {
                    // Must be NE, E or SE
                    botLocation = Direction.East;
                    if (p0.Y < p[1].Y)
                    {
                        botLocation = Direction.NorthEast;
                    }
                    else if (p0.Y > p[2].Y)
                    {
                        botLocation = Direction.SouthEast;
                    }
                }
                else if (p0.Y < p[1].Y)
                {
                    botLocation = Direction.North;
                }
                else
                {
                    botLocation = Direction.South;
                }

                // If the bot is off one of the corners, we have to check two shape edges.
                // If it is off one of the sides, then we only have to check one.

                // For each eye
                for (var a = 0; a < 8; a++)
                {
                    //Now we check to see if the sight distance for this specific eye is far enough to see this specific shape
                    var sightDistance = sightDistances[a];

                    if (o.pos.X > rob.pos.X + sightDistance || o.pos.X + o.Width < rob.pos.X - sightDistance || o.pos.Y > rob.pos.Y + sightDistance || o.pos.Y + o.Height < rob.pos.Y - sightDistance)
                        continue;

                    // Check to see if the side is viewable in this eye
                    // First, figure out the direction in radians in which the eye is pointed relative to .aim
                    // We have to mod the value and divide by 200 to get radians
                    // then since the eyedir values are offsets from their defaults, eye 1 is off from .aim by 4 eye field widths,
                    // three for eye2, and so on.
                    var eyeAim = Physics.NormaliseAngle(Physics.IntToRadians(rob.mem[MemoryAddresses.EYE1DIR + a]) - Math.PI / 18 * a + Math.PI / 18 * 4 + rob.aim);

                    //These are the left and right sides of the field of view for the eye
                    var halfEyeWidth = Physics.NormaliseAngle(Physics.IntToRadians(rob.mem[MemoryAddresses.EYE1WIDTH + a] + 35)) / 2;

                    var eyeAimLeft = eyeAim + halfEyeWidth;
                    var eyeAimRight = eyeAim - halfEyeWidth;

                    //Check the case where the eye field of view spans 0
                    if (eyeAimRight < 0)
                        eyeAimRight += 2 * Math.PI;

                    if (eyeAimLeft > 2 * Math.PI)
                        eyeAimLeft -= 2 * Math.PI;

                    var eyeSpansZero = eyeAimLeft < eyeAimRight;

                    // Now we have the two sides of the eye.  We need to figure out if and where they intersect the shape.

                    // Change the angles to vectors and scale them by the sight distance
                    var eyeAimLeftVector = new DoubleVector(Math.Cos(eyeAimLeft), Math.Sin(eyeAimLeft));
                    eyeAimLeftVector = eyeAimLeftVector.Unit() * sightDistance;
                    var eyeAimRightVector = new DoubleVector(Math.Cos(eyeAimRight), Math.Sin(eyeAimRight));
                    eyeAimRightVector = eyeAimRightVector.Unit() * sightDistance;

                    eyeAimLeftVector = eyeAimLeftVector with { Y = -eyeAimLeftVector.Y };
                    eyeAimRightVector = eyeAimRightVector with { Y = -eyeAimRightVector.Y };

                    var lowestDist = 32000.0; // set to something impossibly big

                    DoubleVector closestPoint = null;
                    //First, check here for parts of the shape that may be in the eye and closer than either side of the eye width.
                    //There are two major cases here:  either the bot is off a corner and the eye spanes the corner or the bot is off a side
                    //and the bot eye spans the point on the shape closest to the bot.  For both these cases, we find out what is the closest point
                    //be it a corner or the point on the edge perpendicular to the bot and see if that point is inside the span of the eye.  If
                    //it is, it is closer then either eye edge.
                    //Perhaps do this before edges and not do edges if found?
                    switch (botLocation)
                    {
                        case Direction.North:
                            closestPoint = p0 with { Y = p[1].Y };
                            break;

                        case Direction.East:
                            closestPoint = p0 with { X = p[4].X };
                            break;

                        case Direction.South:
                            closestPoint = p0 with { Y = p[4].Y };
                            break;

                        case Direction.West:
                            closestPoint = p0 with { X = p[1].X };
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

                    var ab = closestPoint - p0;
                    //Coordinates are in the 4th quadrant, so make the y values negative so the math works
                    ab = ab with { Y = -ab.Y };

                    var theta = Physics.NormaliseAngle(Math.Atan2(ab.Y, ab.X));

                    if (eyeAimLeft >= theta && theta >= eyeAimRight && !eyeSpansZero || eyeAimLeft >= theta && eyeSpansZero || eyeAimRight <= theta && eyeSpansZero)
                    {
                        lowestDist = ab.Magnitude();
                        if (a == 4)
                            lastopppos = closestPoint;
                    }

                    if (lowestDist < 32000) continue;

                    if (botLocation.HasFlag(Direction.North))
                    {
                        var dist = CheckDistance(p0, p[1], d1[1], eyeAimLeftVector, eyeAimRightVector,
                            out var distRight, out var distLeft);

                        if (dist > 0 && dist < lowestDist)
                        {
                            lowestDist = dist;
                            if (a == 4)
                            {
                                lastopppos = distLeft < distRight && distLeft > 0
                                    ? rob.pos + eyeAimLeftVector.Unit() * dist
                                    : rob.pos + eyeAimRightVector.Unit() * dist;
                            }
                        }
                    }

                    if (botLocation.HasFlag(Direction.East))
                    {
                        var dist = CheckDistance(p0, p[3], d1[4], eyeAimLeftVector, eyeAimRightVector,
                            out var distRight, out var distLeft);

                        if (dist > 0 && dist < lowestDist)
                        {
                            lowestDist = dist;
                            if (a == 4)
                            {
                                lastopppos = distLeft < distRight && distLeft > 0
                                    ? rob.pos + eyeAimLeftVector.Unit() * dist
                                    : rob.pos + eyeAimRightVector.Unit() * dist;
                            }
                        }
                    }

                    if (botLocation.HasFlag(Direction.South))
                    {
                        var dist = CheckDistance(p0, p[2], d1[3], eyeAimLeftVector, eyeAimRightVector,
                            out var distRight, out var distLeft);

                        if (dist > 0 && dist < lowestDist)
                        {
                            lowestDist = dist;
                            if (a == 4)
                            {
                                lastopppos = distLeft < distRight && distLeft > 0
                                    ? rob.pos + eyeAimLeftVector.Unit() * dist
                                    : rob.pos + eyeAimRightVector.Unit() * dist;
                            }
                        }
                    }

                    if (botLocation.HasFlag(Direction.West))
                    {
                        var dist = CheckDistance(p0, p[1], d1[2], eyeAimLeftVector, eyeAimRightVector,
                            out var distRight, out var distLeft);

                        if (dist > 0 && dist < lowestDist)
                        {
                            lowestDist = dist;
                            if (a == 4)
                            {
                                lastopppos = distLeft < distRight && distLeft > 0
                                    ? rob.pos + eyeAimLeftVector.Unit() * dist
                                    : rob.pos + eyeAimRightVector.Unit() * dist;
                            }
                        }
                    }

                    if (lowestDist >= 32000) continue;

                    var percentDistance = (lowestDist - rob.Radius + 10) / sightDistance;
                    var eyeValue = percentDistance <= 0 ? 32000 : (int)(1 / (percentDistance * percentDistance));

                    if (eyeValue > 32000)
                        eyeValue = 32000;

                    if (rob.mem[MemoryAddresses.EyeStart + 1 + a] >= eyeValue) continue;

                    // It is closer than other bots we may have seen.
                    // Check to see if this eye has the focus
                    if (a == Math.Abs(rob.mem[MemoryAddresses.FOCUSEYE] + 4) % 9)
                    {
                        //This eye does have the focus
                        //Set the EYEF value and also lastopp so the lookoccur list will get populated later
                        rob.lastopp = o;
                        rob.mem[MemoryAddresses.EYEF] = eyeValue;
                        rob.lastopppos = lastopppos;
                    }

                    //Set the distance for the eye
                    rob.mem[MemoryAddresses.EyeStart + 1 + a] = eyeValue;
                }
            }
        }

        /// <summary>
        /// Calculates the distance <paramref name="rob"/> can see with an eye of width <paramref name="eyeWidth"/>.
        /// </summary>
        /// <param name="eyeWidth">Width of the eye.</param>
        /// <param name="rob">The rob.</param>
        private double EyeSightDistance(int eyeWidth, robot rob)
        {
            var width = eyeWidth % 1256 + 35;

            if (width <= 0)
                width += 1256;

            if (width == 35)
                return 1440 * EyeStrength(rob);

            return 1440 * (1 - Math.Log((double)width / 35) / 4) * EyeStrength(rob);
        }

        private double EyeStrength(robot rob)
        {
            double eyeStrength;

            if (_options.PondMode && rob.pos.Y > 1)
                eyeStrength = Math.Pow(Math.Pow(EyeEffectiveness / (rob.pos.Y / 2000), _options.Gradient), 6828.0 / _options.FieldHeight);
            else
                eyeStrength = 1;

            if (!_options.Daytime)
                eyeStrength *= 0.8f;

            if (eyeStrength > 1)
                eyeStrength = 1;

            return eyeStrength;
        }
    }
}
