using DarwinBots.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarwinBots.Modules
{
    internal interface IBucketManager
    {
        IEnumerable<Robot> BucketsCollision(Robot rob, bool botRadiiFixed);

        object BucketsProximity(Robot rob, bool botRadiiFixed);

        void UpdateBotBucket(Robot rob);
    }

    internal class BucketManager : IBucketManager
    {
        /// <summary>
        /// The width and height of each bucket.
        /// </summary>
        /// <remarks>
        /// 3348 plus twice radius of the largest possible bot is the farthest possible a bot can see.  4000 is a nice round number.
        /// </remarks>
        public const int BucketSize = 4000;

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
        /// Initializes a new instance of the <see cref="BucketManager"/> class.
        /// </summary>
        /// <param name="opts">The simulation options.</param>
        public BucketManager(SimOptions opts)
        {
            _numBuckets = new IntVector((int)Math.Ceiling((double)opts.FieldWidth / BucketSize), (int)Math.Ceiling((double)opts.FieldHeight / BucketSize));

            _buckets = new Bucket[_numBuckets.X, _numBuckets.Y];

            for (var y = 0; y < _numBuckets.Y; y++)
            {
                for (var x = 0; x < _numBuckets.X; x++)
                {
                    _buckets[x, y] = new Bucket();

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
        }

        /// <summary>
        /// Checks all the bots in the same bucket and surrounding buckets for collisions.
        /// </summary>
        /// <param name="rob">The robot to check.</param>
        public IEnumerable<Robot> BucketsCollision(Robot rob, bool botRadiiFixed)
        {
            foreach (var r in CheckBotBucketForCollision(rob, rob.BucketPosition, botRadiiFixed))
                yield return r;

            foreach (var r in _buckets[rob.BucketPosition.X, rob.BucketPosition.Y].AdjacentBuckets.Where(b => b.X != -1).SelectMany(b => CheckBotBucketForCollision(rob, b, botRadiiFixed)))
                yield return r;
        }

        /// <summary>
        /// Checks all the bots in the same bucket and surrounding buckets for proximity.
        /// </summary>
        /// <returns>
        /// The last viewed object.
        /// </returns>
        /// <param name="rob">The robot to check.</param>
        public object BucketsProximity(Robot rob, bool botRadiiFixed)
        {
            rob.LastSeenObject = null;
            rob.Memory[MemoryAddresses.EYEF] = 0;

            for (var x = MemoryAddresses.EyeStart + 1; x < MemoryAddresses.EyeEnd; x++)
                rob.Memory[x] = 0;

            CheckBotBucketForVision(rob, rob.BucketPosition, botRadiiFixed);

            foreach (var adjBucket in _buckets[rob.BucketPosition.X, rob.BucketPosition.Y].AdjacentBuckets.Where(b => b.X != -1))
                CheckBotBucketForVision(rob, adjBucket, botRadiiFixed);

            return rob.LastSeenObject;
        }

        /// <summary>
        /// Updates the bucket the bot is sat in.
        /// </summary>
        /// <param name="rob">The rob to place.</param>
        public void UpdateBotBucket(Robot rob)
        {
            if (!rob.Exists)
            {
                _buckets[rob.BucketPosition.X, rob.BucketPosition.Y].Robots.Remove(rob);
                return;
            }

            var current = rob.Position / BucketSize;
            current = DoubleVector.Floor(current);
            current = DoubleVector.Clamp(current, DoubleVector.Zero, _numBuckets - new DoubleVector(1, 1));

            var newBucket = current.ToIntVector();

            if (rob.BucketPosition == newBucket) return;

            if (rob.BucketPosition.X >= 0 && rob.BucketPosition.Y >= 0)
                _buckets[rob.BucketPosition.X, rob.BucketPosition.Y].Robots.Remove(rob);

            _buckets[newBucket.X, newBucket.Y].Robots.Add(rob);
            rob.BucketPosition = newBucket;
        }

        private IEnumerable<Robot> CheckBotBucketForCollision(Robot rob, IntVector pos, bool botRadiiFixed)
        {
            foreach (var r in _buckets[pos.X, pos.Y].Robots.Where(i => i != rob && i.AbsNum > rob.AbsNum))
            {
                var distvector = rob.Position - r.Position;
                var dist = rob.GetRadius(botRadiiFixed) + r.GetRadius(botRadiiFixed);
                if (distvector.MagnitudeSquare() < dist * dist)
                    yield return r;
            }
        }

        private void CheckBotBucketForVision(Robot rob, IntVector pos, bool botRadiiFixed)
        {
            foreach (var r in _buckets[pos.X, pos.Y].Robots.Where(i => i != rob))
                CompareRobots(rob, r, botRadiiFixed);
        }

        private void CompareRobots(Robot rob1, Robot rob2, bool botRadiiFixed)
        {
            var ab = rob2.Position - rob1.Position;
            var edgeToEdgeDistance = ab.Magnitude() - rob1.GetRadius(botRadiiFixed) - rob2.GetRadius(botRadiiFixed);

            var sightDistances = Enumerable.Range(0, 8)
                .Select(a => 1440)
                .ToArray();

            var maxSightDistance = sightDistances.Max();

            // Now check the maximum possible distance bot N1 can see against how far away bot N2 is.
            if (edgeToEdgeDistance > maxSightDistance)
                return; // Bot too far away to see

            // ac and ad are to either end of the bots, while ab is to the center

            var ac = ab.Unit();

            var ad = new DoubleVector(ac.Y, -ac.X);
            ad *= rob2.GetRadius(botRadiiFixed);
            ad += ab;

            ac = new DoubleVector(-ac.Y, ac.X);
            ac *= rob2.GetRadius(botRadiiFixed);
            ac += ab;

            // Coordinates are in the 4th quadrant, so make the y values negative so the math works
            ad = ad.InvertY();
            ac = ac.InvertY();

            var theta = Physics.NormaliseAngle(Math.Atan2(ad.Y, ad.X));
            var beta = Physics.NormaliseAngle(Math.Atan2(ac.Y, ac.X));

            var botspanszero = beta > theta;

            for (var a = 0; a < 8; a++)
            {
                var eyeDistance = sightDistances[a];

                if (!(edgeToEdgeDistance <= eyeDistance)) continue;

                var eyeAim = Physics.NormaliseAngle(rob1.Aim + Math.PI / 18 * (4 - a));

                var eyeAimLeft = eyeAim + Math.PI / 36;
                var eyeAimRight = eyeAim - Math.PI / 36;

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
                if (!(rob1.Memory[MemoryAddresses.EyeStart + 1 + a] < eyeValue)) continue;

                // It is closer than other bots we may have seen.
                // Check to see if this eye has the focus
                if (a == Math.Abs(rob1.Memory[MemoryAddresses.FOCUSEYE] + 4) % 9)
                {
                    // This eye does have the focus
                    // Set the EYEF value and also lastopp so the lookoccur list will get populated later
                    rob1.LastSeenObject = rob2;
                    rob1.Memory[MemoryAddresses.EYEF] = (int)eyeValue;
                }
                // Set the distance for the eye
                rob1.Memory[MemoryAddresses.EyeStart + 1 + a] = (int)eyeValue;
            }
        }
    }
}
