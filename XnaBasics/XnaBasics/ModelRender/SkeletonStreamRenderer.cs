//------------------------------------------------------------------------------
// <copyright file="SkeletonStreamRenderer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.XnaBasics
{
    using System;
    using Microsoft.Kinect;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A delegate method explaining how to map a SkeletonPoint from one space to another.
    /// </summary>
    /// <param name="point">The SkeletonPoint to map.</param>
    /// <returns>The Vector2 representing the target location.</returns>
    public delegate Vector2 SkeletonPointMap(SkeletonPoint point);

    /// <summary>
    /// This class is responsible for rendering a skeleton stream.
    /// </summary>
    public class SkeletonStreamRenderer : Object2D
    {
        /// <summary>
        /// This is the map method called when mapping from
        /// skeleton space to the target space.
        /// </summary>
        private readonly SkeletonPointMap mapMethod;

        /// <summary>
        /// The last frames skeleton data.
        /// </summary>
        public static Skeleton[] skeletonData;


        public static Tuple<float, float, float, float> FloorClipPlane;

        /// <summary>
        /// The origin (center) location of the joint texture.
        /// </summary>
        private Vector2 jointOrigin;

        /// <summary>
        /// The joint texture.
        /// </summary>
        private Texture2D jointTexture;

        /// <summary>
        /// The origin (center) location of the bone texture.
        /// </summary>
        private Vector2 boneOrigin;
        
        /// <summary>
        /// The bone texture.
        /// </summary>
        private Texture2D boneTexture;

        /// <summary>
        /// Whether the rendering has been initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// Initializes a new instance of the SkeletonStreamRenderer class.
        /// </summary>
        /// <param name="game">The related game object.</param>
        /// <param name="map">The method used to map the SkeletonPoint to the target space.</param>
        public SkeletonStreamRenderer(Game game, SkeletonPointMap map)
            : base(game)
        {
            this.mapMethod = map;
        }

        /// <summary>
        /// This method initializes necessary values.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.Size = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            this.initialized = true;
        }

        /// <summary>
        /// This method retrieves a new skeleton frame if necessary.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // If the sensor is not found, not running, or not connected, stop now
            if (null == this.Chooser.Sensor ||
                false == this.Chooser.Sensor.IsRunning ||
                KinectStatus.Connected != this.Chooser.Sensor.Status)
            {
                return;
            }
            // Update Skeleton Frame 
            using (var skeletonFrame = this.Chooser.Sensor.SkeletonStream.OpenNextFrame(0))
            {
                // Sometimes we get a null frame back if no data is ready
                if (null == skeletonFrame)
                {
                    return;
                }

                // Reallocate if necessary
                if (null == skeletonData || skeletonData.Length != skeletonFrame.SkeletonArrayLength)
                {
                    skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                }

                skeletonFrame.CopySkeletonDataTo(skeletonData);

            }
        }



        /// <summary>
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        //public override void Draw(GameTime gameTime)
        public void Draw(GameTime gameTime, Texture2D tex)
        {
            // If the joint texture isn't loaded, load it now
            if (null == this.jointTexture)
            {
                this.LoadContent();
            }

            // If we don't have data, lets leave
            if (null == skeletonData || null == this.mapMethod)
            {
                return;
            }

            if (false == this.initialized)
            {
                this.Initialize();
            }

            this.SharedSpriteBatch.Begin();

            Skeleton tracked_skel = null;

            foreach (var skeleton in skeletonData)
            {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    if (skeleton.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked)
                    {
                        tracked_skel = skeleton;
                        break;
                    }

            }

            if (tracked_skel != null)
            {
                Vector2 hand_r_v = mapMethod(tracked_skel.Joints[JointType.HandRight].Position);
                Vector2 hand_l_v = mapMethod(tracked_skel.Joints[JointType.HandLeft].Position);
                int actual_width = this.Game.GraphicsDevice.DisplayMode.Width;
                int actual_height = this.Game.GraphicsDevice.DisplayMode.Height;


                int original_width = (int)this.Size.X;
                int original_height = (int)this.Size.Y;
                Microsoft.Xna.Framework.Input.Mouse.SetPosition((int)hand_r_v.X * actual_width / original_width, (int)hand_r_v.Y * actual_height / original_height);

                XnaBasics.RightHand.X = hand_r_v.X * actual_width / original_width;
                XnaBasics.RightHand.Y = hand_r_v.Y * actual_height / original_height;
                XnaBasics.LeftHand.X = hand_l_v.X * actual_width / original_width;
                XnaBasics.LeftHand.Y = hand_l_v.Y * actual_height / original_height;

                if (tex != null)
                {
                    SkeletonPoint shoulder_l = tracked_skel.Joints[JointType.ShoulderLeft].Position;
                    SkeletonPoint shoulder_r = tracked_skel.Joints[JointType.ShoulderRight].Position;
                    SkeletonPoint shoulder_c = tracked_skel.Joints[JointType.ShoulderCenter].Position;
                    SkeletonPoint hip_c = tracked_skel.Joints[JointType.HipCenter].Position;

                    Vector2 shoulder_c_v = mapMethod(shoulder_c);
                    Vector2 shoulder_l_v = mapMethod(shoulder_l);
                    Vector2 shoulder_r_v = mapMethod(shoulder_r);
                    Vector2 hip_c_v = mapMethod(hip_c);

                    float skel_height = hip_c_v.Y - shoulder_c_v.Y;
                    float skel_width = shoulder_r_v.X - shoulder_l_v.X;

                    //this.SharedSpriteBatch.Draw(tex,
                    //            new Rectangle((int)(shoulder_l_v.X - 0.5 * skel_width), (int)shoulder_c_v.Y,
                    //                (int)(skel_width * 2), (int)(skel_height * 1.5)),
                    //            Color.White);
                }
            }

            this.SharedSpriteBatch.End();

            base.Draw(gameTime);
        }
        /// <summary>
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            // If the joint texture isn't loaded, load it now
            if (null == this.jointTexture)
            {
                this.LoadContent();
            }

            // If we don't have data, lets leave
            if (null == skeletonData || null == this.mapMethod)
            {
                return;
            }

            if (false == this.initialized)
            {
                this.Initialize();
            }

            this.SharedSpriteBatch.Begin();

            foreach (var skeleton in skeletonData)
            {
                switch (skeleton.TrackingState)
                {
                    case SkeletonTrackingState.Tracked:
                        // Draw Bones
                        /*this.DrawBone(skeleton.Joints, JointType.Head, JointType.ShoulderCenter);
                        this.DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderLeft);
                        this.DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight);
                        this.DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.Spine);
                        this.DrawBone(skeleton.Joints, JointType.Spine, JointType.HipCenter);
                        this.DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipLeft);
                        this.DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipRight);

                        this.DrawBone(skeleton.Joints, JointType.ShoulderLeft, JointType.ElbowLeft);
                        this.DrawBone(skeleton.Joints, JointType.ElbowLeft, JointType.WristLeft);
                        this.DrawBone(skeleton.Joints, JointType.WristLeft, JointType.HandLeft);

                        this.DrawBone(skeleton.Joints, JointType.ShoulderRight, JointType.ElbowRight);
                        this.DrawBone(skeleton.Joints, JointType.ElbowRight, JointType.WristRight);
                        this.DrawBone(skeleton.Joints, JointType.WristRight, JointType.HandRight);

                        this.DrawBone(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft);
                        this.DrawBone(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft);
                        this.DrawBone(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft);

                        this.DrawBone(skeleton.Joints, JointType.HipRight, JointType.KneeRight);
                        this.DrawBone(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight);
                        this.DrawBone(skeleton.Joints, JointType.AnkleRight, JointType.FootRight);
                        */
                        // Now draw the joints
                        foreach (Joint j in skeleton.Joints)
                        {
                            if (j.JointType != JointType.HandLeft)
                                continue;
                            Color jointColor = Color.LightYellow;
                            if (j.TrackingState != JointTrackingState.Tracked)
                            {
                                jointColor = Color.Yellow;
                            }
                            Vector2 posVector2 = this.mapMethod(j.Position);
                            this.SharedSpriteBatch.Draw(
                                this.jointTexture,
                                posVector2,
                                null,
                                jointColor,
                                0.0f,
                                this.jointOrigin,
                                1.0f,
                                SpriteEffects.None,
                                0.0f);
                        }

                        break;
                    case SkeletonTrackingState.PositionOnly:
                        // If we are only tracking position, draw a blue dot
                        this.SharedSpriteBatch.Draw(
                                this.jointTexture,
                                this.mapMethod(skeleton.Position),
                                null,
                                Color.Blue,
                                0.0f,
                                this.jointOrigin,
                                1.0f,
                                SpriteEffects.None,
                                0.0f);
                        break;
                }
            }

            this.SharedSpriteBatch.End();


            base.Draw(gameTime);
        }

        /// <summary>
        /// This method loads the textures and sets the origin values.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            this.jointTexture = Game.Content.Load<Texture2D>("Joint");
            this.jointOrigin = new Vector2(this.jointTexture.Width / 2, this.jointTexture.Height / 2);

            this.boneTexture = Game.Content.Load<Texture2D>("Bone");
            this.boneOrigin = new Vector2(0.5f, 0.0f);
        }

        /// <summary>
        /// This method draws a bone.
        /// </summary>
        /// <param name="joints">The joint data.</param>
        /// <param name="startJoint">The starting joint.</param>
        /// <param name="endJoint">The ending joint.</param>
        private void DrawBone(JointCollection joints, JointType startJoint, JointType endJoint)
        {
            Vector2 start = this.mapMethod(joints[startJoint].Position);
            Vector2 end = this.mapMethod(joints[endJoint].Position);
            Vector2 diff = end - start;
            Vector2 scale = new Vector2(1.0f, diff.Length() / this.boneTexture.Height);

            float angle = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;

            Color color = Color.LightGreen;
            if (joints[startJoint].TrackingState != JointTrackingState.Tracked ||
                joints[endJoint].TrackingState != JointTrackingState.Tracked)
            {
                color = Color.Gray;
            }

            this.SharedSpriteBatch.Draw(this.boneTexture, start, null, color, angle, this.boneOrigin, scale, SpriteEffects.None, 1.0f);
        }
    }
}
