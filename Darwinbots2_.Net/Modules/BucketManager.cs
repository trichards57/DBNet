using DarwinBots.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarwinBots.Modules
{
    internal interface IBucketManager
    {
        IEnumerable<Robot> CheckForCollisions(Robot rob, bool botRadiiFixed);

        void UpdateBotBucket(Robot rob);

        Robot UpdateSight(Robot rob, bool botRadiiFixed);
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
                }
            }
            for (var y = 0; y < _numBuckets.Y; y++)
            {
                for (var x = 0; x < _numBuckets.X; x++)
                {
                    if (x > 0)
                        _buckets[x, y].Adjacent.Add(_buckets[x - 1, y]);

                    if (x < _numBuckets.X - 1)
                        _buckets[x, y].Adjacent.Add(_buckets[x + 1, y]);

                    if (y > 0)
                        _buckets[x, y].Adjacent.Add(_buckets[x, y - 1]);

                    if (y < _numBuckets.Y - 1)
                        _buckets[x, y].Adjacent.Add(_buckets[x, y + 1]);

                    if (x > 0 & y > 0)
                        _buckets[x, y].Adjacent.Add(_buckets[x - 1, y - 1]);

                    if (x > 0 & y < _numBuckets.Y - 1)
                        _buckets[x, y].Adjacent.Add(_buckets[x - 1, y + 1]);

                    if (x < _numBuckets.X - 1 && y > 0)
                        _buckets[x, y].Adjacent.Add(_buckets[x + 1, y - 1]);

                    if (x < _numBuckets.X - 1 && y < _numBuckets.Y - 1)
                        _buckets[x, y].Adjacent.Add(_buckets[x + 1, y + 1]);
                }
            }
        }

        /// <summary>
        /// Checks all the bots in the same bucket and surrounding buckets for collisions.
        /// </summary>
        /// <param name="rob">The robot to check.</param>
        public IEnumerable<Robot> CheckForCollisions(Robot rob, bool botRadiiFixed)
        {
            return GetAllBuckets(rob.BucketPosition).SelectMany(b => CheckBotBucketForCollision(rob, b, botRadiiFixed));
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

            var newBucket = DoubleVector.Clamp(DoubleVector.Floor(rob.Position / BucketSize), DoubleVector.Zero, _numBuckets - new DoubleVector(1, 1)).ToIntVector();

            if (rob.BucketPosition == newBucket) return;

            if (rob.BucketPosition.X >= 0 && rob.BucketPosition.Y >= 0)
                _buckets[rob.BucketPosition.X, rob.BucketPosition.Y].Robots.Remove(rob);

            _buckets[newBucket.X, newBucket.Y].Robots.Add(rob);
            rob.BucketPosition = newBucket;
        }

        /// <summary>
        /// Checks all the bots in the same bucket and surrounding buckets for proximity.
        /// </summary>
        /// <returns>
        /// The last viewed object.
        /// </returns>
        /// <param name="rob">The robot to check.</param>
        public Robot UpdateSight(Robot rob, bool botRadiiFixed)
        {
            rob.LastSeenObject = null;
            rob.Memory[MemoryAddresses.EYEF] = 0;

            for (var x = MemoryAddresses.EyeStart + 1; x < MemoryAddresses.EyeEnd; x++)
                rob.Memory[x] = 0;

            foreach (var r in GetAllBuckets(rob.BucketPosition).SelectMany(b => b.Robots.Where(i => i != rob)))
                CheckRobotSight(rob, r, botRadiiFixed);

            return rob.LastSeenObject;
        }

        private static IEnumerable<Robot> CheckBotBucketForCollision(Robot rob, Bucket bucket, bool botRadiiFixed)
        {
            foreach (var r in bucket.Robots.Where(i => i != rob && i.AbsNum > rob.AbsNum))
            {
                var distvector = rob.Position - r.Position;
                var dist = rob.GetRadius(botRadiiFixed) + r.GetRadius(botRadiiFixed);
                if (distvector.MagnitudeSquare() < dist * dist)
                    yield return r;
            }
        }

        private static void CheckRobotSight(Robot rob1, Robot rob2, bool botRadiiFixed)
        {
            var ab = rob2.Position - rob1.Position;
            var edgeToEdgeDistance = ab.Magnitude() - rob1.GetRadius(botRadiiFixed) - rob2.GetRadius(botRadiiFixed);

            const int sightDistance = 1440;

            if (edgeToEdgeDistance > sightDistance)
                return; // Bot too far away to see

            // ac and ad are to either end of the bots, while ab is to the center

            var ac = ab.Unit();

            var ad = new DoubleVector(ac.Y, -ac.X);
            ad *= rob2.GetRadius(botRadiiFixed);
            ad += ab;

            ac = new DoubleVector(-ac.Y, ac.X);
            ac *= rob2.GetRadius(botRadiiFixed);
            ac += ab;

            var theta = Physics.NormaliseAngle(Math.Atan2(ad.Y, ad.X) + rob1.Aim, false);
            var beta = Physics.NormaliseAngle(Math.Atan2(ac.Y, ac.X) + rob1.Aim, false);

            for (var a = 0; a < 9; a++)
            {
                if (edgeToEdgeDistance > sightDistance) continue;

                var eyeAim = Physics.NormaliseAngle(Math.PI / 18 * (4 - a), false);

                var eyeAimLeft = eyeAim + Math.PI / 36;
                var eyeAimRight = eyeAim - Math.PI / 36;

                if (!(eyeAimLeft >= theta && theta >= eyeAimRight || eyeAimLeft >= beta && beta >= eyeAimRight || theta >= eyeAimLeft && eyeAimRight >= beta || beta >= eyeAimLeft && eyeAimRight >= theta))
                    continue;

                double eyeValue;

                if (edgeToEdgeDistance <= 0)
                {
                    // bots overlap
                    eyeValue = 32000;
                }
                else
                {
                    var percentDist = (edgeToEdgeDistance + 10) / sightDistance;
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

        private static IEnumerable<Bucket> GetAllBuckets(Bucket bucket)
        {
            yield return bucket;

            foreach (var b in bucket.Adjacent)
                yield return b;
        }

        private IEnumerable<Bucket> GetAllBuckets(IntVector position)
        {
            return GetAllBuckets(_buckets[position.X, position.Y]);
        }
    }
}
