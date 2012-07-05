using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    public class LegRender : Object3D
    {
        public LegRender(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            this.scaleFactor = 80.0f;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            float axisHalfLength = 30.0f;
            if (0.0f == axisHalfLength)
            {
                return;
            }
        }

        /// <summary>
        /// This method retrieves a new skeleton frame if necessary.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        protected override Matrix CreateWorldMatrix(Skeleton skeleton)
        {
            Joint leftHip = skeleton.Joints[JointType.HipLeft],
                rightHip = skeleton.Joints[JointType.HipRight],
                centerHip = skeleton.Joints[JointType.HipCenter],
                trousersTarget = centerHip;

            //
            // Create World Matrix
            //
            DepthImagePoint trousersDepthPoint = Chooser.Sensor.MapSkeletonPointToDepth(trousersTarget.Position, Chooser.Sensor.DepthStream.Format);
            float scaleFactor = (float)trousersDepthPoint.Depth / 900;

            Matrix transformMatrix = Matrix.CreateTranslation(-320.0f, -240.0f, 0.0f)
                * Matrix.CreateScale(scaleFactor, scaleFactor, 1.0f);

            Vector3 originalVector = new Vector3(trousersDepthPoint.X, trousersDepthPoint.Y, trousersDepthPoint.Depth);
            Vector3 certainVector = Vector3.Transform(originalVector, transformMatrix);

            float rotationYaxi = 0.0f;

            Vector2 hipNormal = Vector2.Normalize(new Vector2(rightHip.Position.X, rightHip.Position.Z) - new Vector2(leftHip.Position.X, leftHip.Position.Z)),
                hipXYPorjNormal = Vector2.Normalize(new Vector2(1, 0));
            Vector3 hipV3 = new Vector3(hipNormal.X, hipNormal.Y, 0),
                hipXYProjV3 = new Vector3(hipXYPorjNormal.X, hipXYPorjNormal.Y, 0);

            rotationYaxi = (float)(Math.PI - Math.Acos(Vector2.Dot(hipXYPorjNormal, hipNormal)));
            Vector3 sign = Vector3.Cross(hipV3, hipXYProjV3);
            rotationYaxi = sign.Z < 0 ? -rotationYaxi : rotationYaxi;

            Matrix world = Matrix.CreateScale(this.scaleFactor, this.scaleFactor, this.scaleFactor)
                * Matrix.CreateRotationY(rotationYaxi)
                * Matrix.CreateWorld(
                certainVector,
                Vector3.Forward,
                Vector3.Down
            );

            return world;
        }

        /// <summary>
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            // If the joint texture isn't loaded, load it now
            
            if (this.TrackedSkeleton == null)
            {
                return;
            }

            // Now draw the bag at the left hand joint
            if (TrackedSkeleton.Joints[JointType.HandLeft] != null)
            {
                base.Draw(gameTime);
            }
            skeletonDrawn = true;
        }

        protected override void LoadContent()
        {

            base.LoadContent();
        }
    }
}
