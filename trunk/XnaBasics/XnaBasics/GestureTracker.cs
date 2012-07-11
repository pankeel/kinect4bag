﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    /// <summary>
    /// The delegate method for new certain Gesture Tracked
    /// if certain gesture is tracked, the call the function 
    /// OnGesturetracked(this,null)
    /// </summary>
    /// <param name="sender">should be "this" value</param>
    /// <param name="e">should be null at present</param>
    public delegate void GestureTracked(object sender, EventArgs e);
    public enum Trend{Neutral, Up, Down, Zoom};

    public class GestureTracker
    {
        public GestureTracked OnGestureTracked;
        public static int counter;
        public static Trend trend;
        public static SkeletonPoint last_hand_r;
        public static double last_ts;

        public GestureTracker(Game game, GestureTracked gestureTrackedFunction)
        {
            this.OnGestureTracked = gestureTrackedFunction;
        }

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

            //right hand moving upward
            if (hand_r.Y > last_hand_r.Y)
            {
                double velocity = Dist(hand_r, last_hand_r) / (ts - last_ts);
                //System.Diagnostics.Debug.WriteLine(ts+"~"+last_ts+":"+velocity);

                if (GestureTracker.trend == Trend.Up && velocity>0.003)
                {
                    GestureTracker.counter++;
                }
                else
                {
                    Reset();
                    GestureTracker.trend = Trend.Up;
                }
            }
            //right hand moving downward
            else if (hand_r.Y < last_hand_r.Y)
            {
                double velocity = Dist(hand_r, last_hand_r) / (ts - last_ts);
                //System.Diagnostics.Debug.WriteLine(dist);
                if (GestureTracker.trend == Trend.Down && velocity > 0.003)
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