using Iersera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using static Common;
using static Globals;
using static Microsoft.VisualBasic.Conversion;
using static Obstacles;
using static Robots;
using static SimOptModule;

internal static class Buckets_Module
{
    //Using a bucket size of 4000.  3348 plus twice radius of the largest possible bot is the farthest possible a bot can see.  4000 is a
    //nice round number.
    public const int BucketSize = 4000;

    public static double[] eyeDistance = new double[11];

    //This is the buckets Array
    private static BucketType[,] Buckets;

    private static int NumXBuckets = 0;// Field Width divided by bucket size
    private static int NumYBuckets = 0;// Field height divided by bucket size

    /// <summary>
    /// Returns the absolute width of an eye.
    /// </summary>
    public static int AbsoluteEyeWidth(int width)
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

    public static void Add_Bot(int n, vector pos)
    {
        Buckets[(int)pos.X, (int)pos.Y].arr.Add(n);
    }

    public static bool AnyShapeBlocksBot(int n1, int n2)
    {
        for (var i = 1; i < Obstacles.Obstacles.Count; i++)
        {
            if (Obstacles.Obstacles[i].exist && ShapeBlocksBot(n1, n2, i))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Checks all the bots in the same bucket and surrounding buckets for collisions.
    /// </summary>
    public static void BucketsCollision(int n)
    {
        // Check the bucket the bot is in
        CheckBotBucketForCollision(n, rob[n].BucketPos);

        // Checks the abjacent buckets
        foreach (var adjBucket in Buckets[(int)rob[n].BucketPos.X, (int)rob[n].BucketPos.Y].adjBucket.Where(b => b.X != -1))
            CheckBotBucketForCollision(n, adjBucket);
    }

    /// <summary>
    /// Checks all the bots in the same bucket and surrounding buckets for proximity.
    /// </summary>
    /// <returns>
    /// The index of the last viewed object
    /// </returns>
    public static int BucketsProximity(int n)
    {
        rob[n].lastopp = 0;
        rob[n].lastopptype = 0; // set the default type of object seen to a bot.
        rob[n].mem[EYEF] = 0;

        for (var x = EyeStart + 1; x < EyeEnd; x++)
            rob[n].mem[x] = 0;

        //Check the bucket the bot is in
        CheckBotBucketForVision(n, rob[n].BucketPos);

        //Check all the adjacent buckets
        foreach (var adjBucket in Buckets[(int)rob[n].BucketPos.X, (int)rob[n].BucketPos.Y].adjBucket.Where(b => b.X != -1))
            CheckBotBucketForVision(n, adjBucket);

        if (SimOpts.shapesAreVisable && rob[n].exist)
            CompareShapes(n);

        return rob[n].lastopp;
    }

    public static void CompareRobots3(int n1, int n2)
    {
        if (rob[n2].FName == "Base.txt" && hidepred)
            return;

        var ab = rob[n2].pos - rob[n1].pos;
        var edgetoedgedist = ab.Magnitude() - rob[n1].radius - rob[n2].radius;

        //Here we compute the maximum possible distance bot N1 can see.  Sight distance is a function of
        //eye width.  Narrower eyes can see farther, wider eyes not so much.  So, we find the narrowest eye
        //and use that to determine the max distance the bot can see.  But first we check the special case
        //where the bot has not changed any of it's eye widths.  Sims generally have lots of veggies which
        //don't bother to do this, so this is worth it.
        var eyesum = rob[n1].mem[531] + rob[n1].mem[532] + rob[n1].mem[533] + rob[n1].mem[534] + rob[n1].mem[535] + rob[n1].mem[536] + rob[n1].mem[537] + rob[n1].mem[538] + rob[n1].mem[539];
        var sightdist = eyesum == 0 ? 1440 * eyestrength(rob[n1]) : EyeSightDistance(NarrowestEye(n1), n1);

        //Now we check the maximum possible distance bot N1 can see against how far away bot N2 is.
        if (edgetoedgedist > sightdist)
            return; // Bot too far away to see

        //If Shapes are see through, then there is no reason to check if a shape blocks a bot
        if (!SimOpts.shapesAreSeeThrough)
        {
            if (AnyShapeBlocksBot(n1, n2))
                return;
        }

        var invdist = 1.0 / ab.Magnitude();

        //ac and ad are to either end of the bots, while ab is to the center

        var ac = ab * invdist;
        //ac is now unit vector

        var ad = new vector(ac.Y, -ac.X);
        ad *= rob[n2].radius;
        ad += ab;

        ac = new vector(-ac.Y, ac.X);
        ac *= rob[n2].radius;
        ac += ab;

        //Coordinates are in the 4th quadrant, so make the y values negative so the math works
        ad.Y = -ad.Y;
        ac.Y = -ac.Y;

        var theta = Math.Atan2(ad.Y, ad.X);
        var beta = Math.Atan2(ac.Y, ac.X);

        //lets be sure to just deal with postive angles
        if (theta < 0)
            theta += 2 * Math.PI;

        if (beta < 0)
            beta += 2 * Math.PI;

        var botspanszero = beta > theta;

        //For each eye
        for (var a = 0; a < 8; a++)
        {
            var eyedist = rob[n1].mem[EYE1WIDTH + a] == 0 ? 1440 * eyestrength(rob[n1]) : EyeSightDistance(AbsoluteEyeWidth(rob[n1].mem[EYE1WIDTH + a]), n1);
            //Now we check to see if the sight distance for this specific eye is far enough to see bot N2
            if (edgetoedgedist <= eyedist)
            {
                //Check to see if the bot is viewable in this eye
                //First, figure out the direction in radians in which the eye is pointed relative to .aim
                //We have to mod the value and divide by 200 to get radians
                //then since the eyedir values are offsets from their defaults, eye 1 is off from .aim by 4 eye field widths,
                //three for eye2, and so on.
                var eyeaim = rob[n1].mem[EYE1DIR + a] % 1256 / 200 - (Math.PI / 18 * a) + Math.PI / 18 * 4 + rob[n1].aim;

                //It's possible we wrapped 0 so check
                while (eyeaim > 2 * Math.PI)
                    eyeaim -= 2 * Math.PI;

                while (eyeaim < 0)
                    eyeaim += 2 * Math.PI;

                //These are the left and right sides of the field of view for the eye
                double halfeyewidth = rob[n1].mem[EYE1WIDTH + a] % 1256 / 400;
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
                if ((eyeaimleft >= theta && theta >= eyeaimright && !eyespanszero) || (eyeaimleft >= theta && !eyespanszero && botspanszero) || (eyeaimright >= beta && !eyespanszero && botspanszero) || (eyeaimleft <= theta && eyeaimright >= beta && eyespanszero && botspanszero))
                {
                    double eyevalue;
                    //The bot is viewable in this eye.

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
                    if (rob[n1].mem[EyeStart + 1 + a] < eyevalue)
                    {
                        //It is closer than other bots we may have seen.
                        //Check to see if this eye has the focus
                        if (a == Math.Abs(rob[n1].mem[FOCUSEYE] + 4) % 9)
                        {
                            //This eye does have the focus
                            //Set the EYEF value and also lastopp so the lookoccur list will get populated later
                            rob[n1].lastopp = n2;
                            rob[n1].mem[EYEF] = (int)eyevalue;
                        }
                        //Set the distance for the eye
                        rob[n1].mem[EyeStart + 1 + a] = (int)eyevalue;
                        // If n1 = robfocus Then eyeDistance(a + 1) = edgetoedgedist + rob[n1].radius
                    }
                }
            }
        }
    }

    public static void CompareShapes(int n)
    {
        var D1 = new vector[5];

        var p = new vector[5];
        vector lastopppos = null;
        var sightdist = EyeSightDistance(NarrowestEye(n), n) + rob[n].radius;

        for (var o = 1; o < numObstacles; o++)
        {
            if (Obstacles.Obstacles[o].exist)
            {
                //Cheap weed out check - check to see if shape is too far away to be seen
                if ((Obstacles.Obstacles[o].pos.X > rob[n].pos.X + sightdist) || (Obstacles.Obstacles[o].pos.X + Obstacles.Obstacles[o].Width < rob[n].pos.X - sightdist) || (Obstacles.Obstacles[o].pos.Y > rob[n].pos.Y + sightdist) || (Obstacles.Obstacles[o].pos.Y + Obstacles.Obstacles[o].Height < rob[n].pos.Y - sightdist))
                {
                    continue;
                }

                if ((Obstacles.Obstacles[o].pos.X < rob[n].pos.X) && (Obstacles.Obstacles[o].pos.X + Obstacles.Obstacles[o].Width > rob[n].pos.X) && (Obstacles.Obstacles[o].pos.Y < rob[n].pos.Y) && (Obstacles.Obstacles[o].pos.Y + Obstacles.Obstacles[o].Height > rob[n].pos.Y))
                {
                    //Bot is inside shape!
                    for (var i = 0; i < 8; i++)
                    {
                        rob[n].mem[EyeStart + 1 + i] = 32000;
                    }
                    rob[n].lastopp = o;
                    rob[n].lastopptype = 1;
                    return;
                }

                //Guess we have to actually do the hard work and check...

                //Here are the four sides of the shape
                D1[1] = new vector(Obstacles.Obstacles[o].Width, 0); // top
                D1[2] = new vector(0, Obstacles.Obstacles[o].Height); // left side
                D1[3] = D1[1]; // bottom
                D1[4] = D1[2]; // right side

                //Here are the four corners
                p[1] = Obstacles.Obstacles[o].pos; // NW corner
                p[2] = p[1]; // SW Corner
                p[2].Y = p[1].Y + Obstacles.Obstacles[o].Height;
                p[3] = p[1] + D1[1]; // NE Corner
                p[4] = p[2] + D1[1]; // SE Corner

                //Here is the bot.
                var P0 = rob[n].pos;

                int botLocation;
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
                { //Must be NW, W or SW
                    botLocation = 4; // Set to West for default
                    if (P0.Y < p[1].Y)
                    {
                        botLocation = 8; // Must be NW
                    }
                    else if (P0.Y > p[2].Y)
                    {
                        botLocation = 7; // Must be SW
                    }
                }
                else if (P0.X > p[3].X)
                { // Must be NE, E or SE
                    botLocation = 2; // Set to East for default
                    if (P0.Y < p[1].Y)
                    {
                        botLocation = 5; // Must be NE
                    }
                    else if (P0.Y > p[2].Y)
                    {
                        botLocation = 6; // Must be SE
                    }
                }
                else if (P0.Y < p[1].Y)
                {
                    botLocation = 1; // Must be North
                }
                else
                {
                    botLocation = 3; // Must be South
                }

                //If the bot is off one of the corners, we have to check two shape edges.
                //If it is off one of the sides, then we only have to check one.

                //For each eye
                for (var a = 0; a < 8; a++)
                {
                    //Now we check to see if the sight distance for this specific eye is far enough to see this specific shape
                    var eyedist = EyeSightDistance(AbsoluteEyeWidth(rob[n].mem[EYE1WIDTH + a]), n);

                    if ((Obstacles.Obstacles[o].pos.X > rob[n].pos.X + eyedist) || (Obstacles.Obstacles[o].pos.X + Obstacles.Obstacles[o].Width < rob[n].pos.X - eyedist) || (Obstacles.Obstacles[o].pos.Y > rob[n].pos.Y + eyedist) || (Obstacles.Obstacles[o].pos.Y + Obstacles.Obstacles[o].Height < rob[n].pos.Y - eyedist))
                    {
                        //  Do nothing - shape is too far away
                    }
                    else
                    {
                        //Check to see if the side is viewable in this eye
                        //First, figure out the direction in radians in which the eye is pointed relative to .aim
                        //We have to mod the value and divide by 200 to get radians
                        //then since the eyedir values are offsets from their defaults, eye 1 is off from .aim by 4 eye field widths,
                        //three for eye2, and so on.
                        var eyeaim = rob[n].mem[EYE1DIR + a] % 1256 / 200 - (Math.PI / 18 * a) + Math.PI / 18 * 4 + rob[n].aim;

                        //It's possible we wrapped 0 so check
                        while (eyeaim > 2 * Math.PI)
                            eyeaim -= 2 * Math.PI;

                        while (eyeaim < 0)
                            eyeaim += 2 * Math.PI;

                        //These are the left and right sides of the field of view for the eye
                        double halfeyewidth = (rob[n].mem[EYE1WIDTH + a] + 35) / 400;
                        while (halfeyewidth > Math.PI)
                            halfeyewidth -= Math.PI;

                        while (halfeyewidth < 0)
                            halfeyewidth += Math.PI;

                        var eyeaimleft = eyeaim + halfeyewidth;
                        var eyeaimright = eyeaim - halfeyewidth;

                        //Check the case where the eye field of view spans 0
                        if (eyeaimright < 0)
                            eyeaimright = 2 * Math.PI + eyeaimright;

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

                        var distleft = 0.0;
                        var distright = 0.0;
                        var dist = 32000.0; // set to something impossibly big
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
                            case 1:  // North side
                                closestPoint = P0;
                                closestPoint.Y = p[1].Y;
                                break;// East side
                            case 2:
                                closestPoint = P0;
                                closestPoint.X = p[4].X;
                                break;// South side
                            case 3:
                                closestPoint = P0;
                                closestPoint.Y = p[4].Y;
                                break;// West side
                            case 4:
                                closestPoint = P0;
                                closestPoint.X = p[1].X;
                                break;// NE Corner
                            case 5:
                                closestPoint = p[3];
                                break;// SE corner
                            case 6:
                                closestPoint = p[4];
                                break;// SW corner
                            case 7:
                                closestPoint = p[2];
                                break;// NW corner
                            case 8:
                                closestPoint = p[1];
                                break;
                        }

                        var ab = closestPoint - P0;
                        //Coordinates are in the 4th quadrant, so make the y values negative so the math works
                        ab.Y = -ab.Y;

                        double theta = angnorm(Math.Atan2(ab.Y, ab.X));

                        if ((eyeaimleft >= theta && theta >= eyeaimright && !eyespanszero) || (eyeaimleft >= theta && eyespanszero) || (eyeaimright <= theta && eyespanszero))
                        {
                            lowestDist = ab.Magnitude();
                            if (a == 4)
                            {
                                lastopppos = closestPoint;
                            }
                        }

                        if (lowestDist == 32000)
                        { // eye doesn't span corner or spot perpendicular to line from bot to shape side
                            if ((botLocation == 1) || (botLocation == 5) || (botLocation == 8))
                            {
                                // North - Bot is above shape, might be able to see top of shape
                                var s = SegmentSegmentIntersect(P0, eyeaimleftvector, p[1], D1[1]); //Check intersection of left eye range and shape side
                                if (s > 0)
                                    distleft = s * eyeaimleftvector.Magnitude(); //If the left eye range intersects then store the distance of the interesction

                                var t = SegmentSegmentIntersect(P0, eyeaimrightvector, p[1], D1[1]); //Check intersection of right eye range and shape side
                                if (t > 0)
                                    distright = t * eyeaimrightvector.Magnitude(); //If the right eye range intersects, then store the distance of the intersection

                                if (distleft > 0 & distright > 0)
                                    dist = Math.Min(distleft, distright);
                                else if (distleft > 0)
                                    dist = distleft;  //Only left side intersects
                                else if (distright > 0)
                                    dist = distright;  //Only right side intersects

                                if ((dist > 0) && (dist < lowestDist))
                                {
                                    lowestDist = dist;
                                    if (a == 4)
                                    {
                                        lastopppos = (distleft < distright) && (distleft > 0)
                                            ? rob[n].pos + (eyeaimleftvector.Unit() * dist)
                                            : rob[n].pos + (eyeaimrightvector.Unit() * dist);
                                    }
                                }
                            }

                            if ((botLocation == 2) || (botLocation == 5) || (botLocation == 6))
                            {
                                // East = Bot to right of shape, might be abel to see right side
                                var s = SegmentSegmentIntersect(P0, eyeaimleftvector, p[3], D1[4]); //Check intersection of left eye range and shape side

                                if (s > 0)
                                    distleft = s * eyeaimleftvector.Magnitude(); //If the left eye range intersects then store the distance of the interesction

                                var t = SegmentSegmentIntersect(P0, eyeaimrightvector, p[3], D1[4]); //Check intersection of right eye range and shape side

                                if (t > 0)
                                    distright = t * eyeaimrightvector.Magnitude(); //If the right eye range intersects, then store the distance of the intersection

                                if (distleft > 0 & distright > 0)
                                    dist = Math.Min(distleft, distright);
                                else if (distleft > 0)
                                    dist = distleft; //Only left side intersects
                                else if (distright > 0)
                                    dist = distright;  //Only right side intersects

                                if ((dist > 0) && (dist < lowestDist))
                                {
                                    lowestDist = dist;
                                    if (a == 4)
                                    {
                                        lastopppos = (distleft < distright) && (distleft > 0)
                                            ? rob[n].pos + (eyeaimleftvector.Unit() * dist)
                                            : rob[n].pos + (eyeaimrightvector.Unit() * dist);
                                    }
                                }
                            }

                            if ((botLocation == 3) || (botLocation == 6) || (botLocation == 7))
                            {
                                // South - Bot is below shape
                                var s = SegmentSegmentIntersect(P0, eyeaimleftvector, p[2], D1[3]); // Check intersection of left eye range and shape side
                                if (s > 0)
                                    distleft = s * eyeaimleftvector.Magnitude(); // If the left eye range intersects then store the distance of the interesction

                                var t = SegmentSegmentIntersect(P0, eyeaimrightvector, p[2], D1[3]); // Check intersection of right eye range and shape side
                                if (t > 0)
                                    distright = t * eyeaimrightvector.Magnitude(); // If the right eye range intersects, then store the distance of the intersection

                                if (distleft > 0 & distright > 0)
                                    dist = Math.Min(distleft, distright); // Both sides intersect, pick the closest
                                else if (distleft > 0)
                                    dist = distleft; // Only left side intersects
                                else if (distright > 0)
                                    dist = distright; // Only right side intersects

                                if ((dist > 0) && (dist < lowestDist))
                                {
                                    lowestDist = dist;
                                    if (a == 4)
                                    {
                                        lastopppos = (distleft < distright) && (distleft > 0)
                                            ? rob[n].pos + (eyeaimleftvector.Unit() * dist)
                                            : rob[n].pos + (eyeaimrightvector.Unit() * dist);
                                    }
                                }
                            }

                            if ((botLocation == 4) || (botLocation == 7) || (botLocation == 8))
                            {
                                // West - Bot is to left of shape
                                var s = SegmentSegmentIntersect(P0, eyeaimleftvector, p[1], D1[2]); //Check intersection of left eye range and shape side
                                if (s > 0)
                                    distleft = s * eyeaimleftvector.Magnitude(); //If the left eye range intersects then store the distance of the interesction

                                var t = SegmentSegmentIntersect(P0, eyeaimrightvector, p[1], D1[2]); //Check intersection of right eye range and shape side
                                if (t > 0)
                                    distright = t * eyeaimrightvector.Magnitude(); //If the right eye range intersects, then store the distance of the intersection

                                if (distleft > 0 & distright > 0)
                                    dist = Math.Min(distleft, distright);//bot eye sides intersect.  Pick the closest one.
                                else if (distleft > 0)
                                    dist = distleft; //Only left side intersects
                                else if (distright > 0)
                                    dist = distright; //Only right side intersects

                                if ((dist > 0) && (dist < lowestDist))
                                {
                                    lowestDist = dist;
                                    if (a == 4)
                                    {
                                        lastopppos = (distleft < distright) && (distleft > 0)
                                            ? rob[n].pos + (eyeaimleftvector.Unit() * dist)
                                            : rob[n].pos + (eyeaimrightvector.Unit() * dist);
                                    }
                                }
                            }

                            if (lowestDist < 32000)
                            {
                                var percentdist = (lowestDist - rob[n].radius + 10) / eyedist;
                                var eyevalue = percentdist <= 0 ? 32000 : (int)(1 / (percentdist * percentdist));
                                if (eyevalue > 32000)
                                {
                                    eyevalue = 32000;
                                }

                                if (rob[n].mem[EyeStart + 1 + a] < eyevalue)
                                {
                                    //It is closer than other bots we may have seen.
                                    //Check to see if this eye has the focus
                                    if (a == Math.Abs(rob[n].mem[FOCUSEYE] + 4) % 9)
                                    {
                                        //This eye does have the focus
                                        //Set the EYEF value and also lastopp so the lookoccur list will get populated later
                                        rob[n].lastopp = o;
                                        rob[n].lastopptype = 1;
                                        rob[n].mem[EYEF] = (int)eyevalue;
                                        rob[n].lastopppos = lastopppos;
                                    }
                                    //Set the distance for the eye
                                    rob[n].mem[EyeStart + 1 + a] = (int)eyevalue;
                                    if (n == robfocus)
                                    {
                                        eyeDistance[a + 1] = lowestDist; // + rob[n].radius
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static void Delete_Bot(int n, vector pos)
    {
        Buckets[(int)pos.X, (int)pos.Y].arr.RemoveAll(i => i == n);
    }

    public static double EyeSightDistance(int w, int n1)
    {
        return w == 35 ? 1440 * eyestrength(rob[n1]) : 1440 * (1 - (Math.Log(w / 35) / 4)) * eyestrength(rob[n1]);
    }

    public static void Init_Buckets()
    {
        // Determine the nubmer of buckets.
        NumXBuckets = Int(SimOpts.FieldWidth / BucketSize);
        NumYBuckets = Int(SimOpts.FieldHeight / BucketSize);

        Buckets = new BucketType[NumXBuckets + 1, NumYBuckets + 1];

        // Buckets count along rows, top row, then next...
        for (var y = 0; y < NumYBuckets - 1; y++)
        {
            for (var x = 0; x < NumXBuckets - 1; x++)
            {
                Buckets[x, y].arr = new List<int>();
                Buckets[x, y].size = 0;

                // Set the list of adjacent buckets for this bucket
                // We take the time to do this here to save the time it would take to compute these every cycle.
                if (x > 0)
                    Buckets[x, y].adjBucket.Add(new vector(x - 1, y));

                if (x < NumXBuckets - 1)
                    Buckets[x, y].adjBucket.Add(new vector(x + 1, y));

                if (y > 0)
                    Buckets[x, y].adjBucket.Add(new vector(x, y - 1));

                if (y < NumYBuckets - 1)
                    Buckets[x, y].adjBucket.Add(new vector(x, y + 1));

                if (x > 0 & y > 0)
                    Buckets[x, y].adjBucket.Add(new vector(x - 1, y - 1));

                if (x > 0 & y < NumYBuckets - 1)
                    Buckets[x, y].adjBucket.Add(new vector(x - 1, y + 1));

                if (x < NumXBuckets - 1 && y > 0)
                    Buckets[x, y].adjBucket.Add(new vector(x + 1, y - 1));

                if (x < NumXBuckets - 1 && y < NumYBuckets - 1)
                    Buckets[x, y].adjBucket.Add(new vector(x + 1, y + 1));
            }
        }

        for (var x = 1; x < MaxRobs; x++)
        {
            if (rob[x].exist)
            {
                rob[x].BucketPos.X = -2;
                rob[x].BucketPos.Y = -2;
                UpdateBotBucket(x);
            }
        }
    }

    public static int NarrowestEye(int n)
    {
        var NarrowestEye = 1221;
        for (var i = 0; i < 8; i++)
        {
            var Width = AbsoluteEyeWidth(rob[n].mem[EYE1WIDTH + i]);
            if (Width < NarrowestEye)
            {
                NarrowestEye = Width;
            }
        }
        return NarrowestEye;
    }

    public static double SegmentSegmentIntersect(vector P0, vector D0, vector P1, vector D1)
    {
        var dotPerp = D0.X * D1.Y - D1.X * D0.Y; // Test for intersection

        if (dotPerp != 0)
        {
            var Delta = P1 - P0;
            var s = Dot(Delta, new vector(D1.Y, -D1.X)) / dotPerp;
            var t = Dot(Delta, new vector(D0.Y, -D0.X)) / dotPerp;

            if (s >= 0 & s <= 1 && t >= 0 & t <= 1)
                return s;
        }

        return 0.0;
    }

    public static bool ShapeBlocksBot(int n1, int n2, int o)
    {
        var D1 = new vector[5];
        var p = new vector[5];

        //Cheap weed out check
        if ((Obstacles.Obstacles[o].pos.X > Math.Max(rob[n1].pos.X, rob[n2].pos.X)) || (Obstacles.Obstacles[o].pos.X + Obstacles.Obstacles[o].Width < Math.Min(rob[n1].pos.X, rob[n2].pos.X)) || (Obstacles.Obstacles[o].pos.Y > Math.Max(rob[n1].pos.Y, rob[n2].pos.Y)) || (Obstacles.Obstacles[o].pos.Y + Obstacles.Obstacles[o].Height < Math.Min(rob[n1].pos.Y, rob[n2].pos.Y)))
            return false;

        D1[1] = new vector(0, Obstacles.Obstacles[o].Width); // top
        D1[2] = new vector(Obstacles.Obstacles[o].Height, 0); // left side
        D1[3] = D1[1]; // bottom
        D1[4] = D1[2]; // right side

        p[1] = Obstacles.Obstacles[o].pos;
        p[2] = p[1];
        p[3] = p[1] + D1[2];
        p[4] = p[1] + D1[1];

        var P0 = rob[n1].pos;
        var D0 = rob[n2].pos - rob[n1].pos;

        for (var i = 1; i < 4; i++)
        {
            var numerator = Cross(D0, D1[i]);
            if (numerator != 0)
            {
                var Delta = p[i] - P0;
                var s = Cross(Delta, D1[i]) / numerator;
                var t = Cross(Delta, D0) / numerator;

                if (t >= 0 & t <= 1)
                    return true;

                if (s >= 0 & s <= 1)
                    return true;
            }
        }

        return false;
    }

    // used for exact distances to viewed objects for displaying the eye viewer for the focus bot
    // also erases array elements to retrieve memory
    public static void UpdateBotBucket(int n)
    {
        var changed = false;

        if (!rob[n].exist)
        {
            Delete_Bot(n, rob[n].BucketPos);
            return;
        }

        var newbucket = new vector(rob[n].BucketPos.X, rob[n].BucketPos.Y);

        var currbucket = (int)Math.Floor(rob[n].pos.X / BucketSize);
        if (currbucket < 0)
            currbucket = 0; // Possible bot is off the field

        if (currbucket >= NumXBuckets)
            currbucket = NumXBuckets - 1; // Possible bot is off the field

        if (rob[n].BucketPos.X != currbucket)
        {
            // we've moved off the bucket, update bucket
            newbucket.X = currbucket;
            changed = true;
        }

        currbucket = (int)Math.Floor(rob[n].pos.Y / BucketSize);
        if (currbucket < 0)
            currbucket = 0; // Possible bot is off the field

        if (currbucket >= NumYBuckets)
            currbucket = NumYBuckets - 1; // Possible bot is off the field

        if (rob[n].BucketPos.Y != currbucket)
        {
            newbucket.Y = currbucket;
            changed = true;
        }

        if (changed)
        {
            Delete_Bot(n, rob[n].BucketPos);
            Add_Bot(n, newbucket);
            rob[n].BucketPos = newbucket;
        }
    }

    private static void CheckBotBucketForCollision(int n, vector pos)
    {
        foreach (var robnumber in Buckets[(int)pos.X, (int)pos.Y].arr.Where(i => i > n))
        {
            // only have to check bots higher than n otherwise we do it twice for each bot pair
            if (!(rob[robnumber].FName == "Base.txt" && hidepred))
            {
                var distvector = (rob[n].pos - rob[robnumber].pos);
                var dist = rob[n].radius + rob[robnumber].radius;
                if (distvector.MagnitudeSquare() < (dist * dist))
                    Repel3(n, robnumber);
            }
        }
    }

    private static void CheckBotBucketForVision(int n, vector pos)
    {
        foreach (var robnumber in Buckets[(int)pos.X, (int)pos.Y].arr.Where(i => i != n))
            CompareRobots3(n, robnumber);
    }

    private static double eyestrength(robot rob)
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

    //number of bots in the bucket i.e. highest array element with a bot
    // List of buckets adjoining this one.  Interior buckets will have 8.  Edge buckets 5.  Corners 3.
    public class BucketType
    {
        public List<vector> adjBucket = new List<vector>();
        public List<int> arr;
        public int size = 0;
    }
}
