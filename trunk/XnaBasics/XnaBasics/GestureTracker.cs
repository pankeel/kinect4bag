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

        public static void Reset() {
            counter = 0;
            trend = Trend.Neutral;
        }

        public static void Update(Skeleton skel)
        {
            if (null == GestureTracker.last_hand_r)
            {
                GestureTracker.last_hand_r = skel.Joints[JointType.HandRight].Position;
                Reset();
                return;
            }

            SkeletonPoint hand_r = skel.Joints[JointType.HandRight].Position;
            SkeletonPoint elbow_r = skel.Joints[JointType.ElbowRight].Position;

            //right hand above hip and moving upward
            if (hand_r.Y < elbow_r.Y && hand_r.Y < last_hand_r.Y)
            {
                if (trend == Trend.Up)
                {
                    counter++;
                }
                else
                {
                    Reset();
                    trend = Trend.Up;
                }
            }
            else if (hand_r.Y > elbow_r.Y && hand_r.Y > last_hand_r.Y) //right hand below hip and moving downward
            {
                if (trend == Trend.Down)
                {
                    counter++;
                }
                else
                {
                    Reset();
                    trend = Trend.Down;
                }
            }
            else
            {
                Reset();
            }

            GestureTracker.last_hand_r = hand_r;
        }
    }
}
