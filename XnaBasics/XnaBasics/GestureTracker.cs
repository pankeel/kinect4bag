using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    enum Trend{Neutral, Up, Down};

    static class GestureTracker
    {
        public static int counter;
        public static Trend trend;
        public static SkeletonPoint last_hand_r;
        public static double last_ts;

        public static void Reset() {
            GestureTracker.counter = 0;
            GestureTracker.trend = Trend.Neutral;
        }

        public static void Update(Skeleton skel, double ts)
        {
            if (null == GestureTracker.last_hand_r)
            {
                GestureTracker.last_hand_r = skel.Joints[JointType.HandRight].Position;
                GestureTracker.last_ts = ts;
                Reset();
                return;
            }

            SkeletonPoint hand_r = skel.Joints[JointType.HandRight].Position;
            SkeletonPoint elbow_r = skel.Joints[JointType.ElbowRight].Position;

            //right hand above hip and moving upward
            //if (hand_r.Y > elbow_r.Y && hand_r.Y > last_hand_r.Y)
            if (hand_r.Y > last_hand_r.Y)
            {
                double dist = Dist(hand_r, last_hand_r);
                System.Diagnostics.Debug.WriteLine(dist);
                if (GestureTracker.trend == Trend.Up && dist>0.12)
                {
                    GestureTracker.counter++;
                }
                else
                {
                    Reset();
                    GestureTracker.trend = Trend.Up;
                }
            }
            //else if (hand_r.Y < elbow_r.Y && hand_r.Y < last_hand_r.Y) //right hand below hip and moving downward
            else if (hand_r.Y < last_hand_r.Y)
            {
                double dist = Dist(hand_r, last_hand_r);
                System.Diagnostics.Debug.WriteLine(dist);
                if (GestureTracker.trend == Trend.Down && dist > 0.12)
                {
                    GestureTracker.counter++;
                }
                else
                {
                    Reset();
                    GestureTracker.trend = Trend.Down;
                }
            }
            else
            {
                Reset();
            }

            GestureTracker.last_hand_r = hand_r;
            GestureTracker.last_ts = ts;
        }

        public static double Dist(SkeletonPoint p1, SkeletonPoint p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            double dz = p1.Z - p2.Z;

            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}
