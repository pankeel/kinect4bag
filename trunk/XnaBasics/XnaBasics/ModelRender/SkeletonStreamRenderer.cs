using JiggleGame;
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
        /// Whether the rendering has been initialized.
        /// </summary>
        private bool initialized;

        private Skeleton primarySkeleton = null;

        private bool isDrawSkeleton = true;

        protected Camera camera
        {
            get
            {
                return (Camera)this.Game.Services.GetService(typeof(Camera));
            }
        }

        private Model JointModel;

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
                if (skeletonData != null)
                {
                    float zPosition = 3.0f;
                    primarySkeleton = null;
                    foreach (Skeleton skeleton in skeletonData)
                    {
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            if (zPosition > skeleton.Position.Z)
                            {
                                zPosition = skeleton.Position.Z;
                                primarySkeleton = skeleton;
                            }
                        }
                    }
                }
                else
                {
                    primarySkeleton = null;
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
            this.JointModel = Game.Content.Load<Model>("box");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transVec"></param>
        private void DrawJoint(Vector3 transVec)
        {
            foreach (ModelMesh mesh in this.JointModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateWorld(transVec, Vector3.Forward, Vector3.Up);
                    //effect.World = Matrix.Identity;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }
        }
        /// <summary>
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            // If we don't have data, lets leave
            if (null == skeletonData )
            {
                return;
            }

            if (false == this.initialized)
            {
                this.Initialize();
            }
            if (this.primarySkeleton != null)
            {
                JointCollection JointCollection = primarySkeleton.Joints;
                foreach (Joint joint in JointCollection)
                {
                    if (joint.TrackingState != JointTrackingState.Tracked)
                        continue;
                    DepthImagePoint depthPtr = Chooser.Sensor.MapSkeletonPointToDepth(joint.Position, Chooser.Sensor.DepthStream.Format);
                    Vector3 transVect = new Vector3(joint.Position.X*10, joint.Position.Y*10, joint.Position.Z*10);
                    
                    DrawJoint(transVect);
                }

                
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
            //SpriteFont spriteFont = Game.Content.Load<SpriteFont>("Segoe16");
            //this.SharedSpriteBatch.DrawString(spriteFont,"SXsss", new Vector2(10,10),Color.Red);

            this.SharedSpriteBatch.End();


            base.Draw(gameTime);
        }



    }
}
