using System.Text;
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
    using System.IO;

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
            if (this.Chooser.LastStatus == KinectStatus.Connected)
            {
                this.Chooser.Sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(Sensor_SkeletonFrameReady);
            }
            
        }

        public void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                // Reallocate if necessary
                if (skeletonFrame != null)
                {
                    if (null == skeletonData || skeletonData.Length != skeletonFrame.SkeletonArrayLength)
                    {
                        skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                }


            }
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

            //// If the sensor is not found, not running, or not connected, stop now
            //if (null == this.Chooser.Sensor ||
            //    false == this.Chooser.Sensor.IsRunning ||
            //    KinectStatus.Connected != this.Chooser.Sensor.Status)
            //{
            //    return;
            //}
            //// Update Skeleton Frame 
            //using (var skeletonFrame = this.Chooser.Sensor.SkeletonStream.OpenNextFrame(0))
            //{
            //    // Sometimes we get a null frame back if no data is ready
            //    if (null == skeletonFrame)
            //    {
            //        return;
            //    }

            //    // Reallocate if necessary
            //    if (null == skeletonData || skeletonData.Length != skeletonFrame.SkeletonArrayLength)
            //    {
            //        skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
            //    }

            //    skeletonFrame.CopySkeletonDataTo(skeletonData);

            //}
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

                        // Now draw the joints
                        FileStream fs = new FileStream("Skeleton.txt", FileMode.Append, FileAccess.Write, FileShare.None);
                        StreamWriter mStreamWriter = new StreamWriter(fs);
                        String content = "";
                        Joint leftShoulder = skeleton.Joints[JointType.ShoulderLeft],
                            rightShoulder = skeleton.Joints[JointType.ShoulderRight];
                        content += String.Format("{0} {1} {2} {3} {4} {5}",
                            leftShoulder.Position.X,
                            leftShoulder.Position.Y,
                            leftShoulder.Position.Z,
                            rightShoulder.Position.X,
                            rightShoulder.Position.Y,
                            rightShoulder.Position.Z);

                        
                        mStreamWriter.WriteLine(content);
                        mStreamWriter.Flush();
                        mStreamWriter.Close();
                        fs.Close();
                        fs.Dispose();
                        break;

                }
            }

            this.SharedSpriteBatch.End();


            base.Draw(gameTime);
        }



    }
}
