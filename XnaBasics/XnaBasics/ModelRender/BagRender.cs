using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;
using System.IO;



namespace Microsoft.Samples.Kinect.XnaBasics
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class BagRender : Object3D
    {

        private bool isOnLeftHand = true;
        /// <summary>
        /// This is the array of 3D vertices with associated colors.
        /// </summary>
        private VertexPositionColor[] localCubeVertices;

        private short[] localCubeIndexes;


        public Matrix leftHandSkin;

        public Matrix leftHandWorld;


        private void Initialize_ToDelete()
        {
            float axisHalfLength = 30.0f;
            if (0.0f == axisHalfLength)
            {
                return;
            }

        }

        public BagRender(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            this.scaleFactor = 80.0f;
            this.Initialize_ToDelete();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
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
            //
            // Left Hand Joint
            //
            Joint leftHandJoint = skeleton.Joints[JointType.HandLeft],
                leftElbowJoint = skeleton.Joints[JointType.ElbowLeft],
                leftShoulderJoint = skeleton.Joints[JointType.ShoulderLeft],
                rightHandJoint = skeleton.Joints[JointType.HandRight],
                rightElbowJoint = skeleton.Joints[JointType.ElbowRight],
                bagHangTargetJoint = skeleton.Joints[JointType.HipCenter];

            float distanceTwoHands = Vector3.Distance(new Vector3(leftHandJoint.Position.X, leftHandJoint.Position.Y, leftHandJoint.Position.Z), new Vector3(rightHandJoint.Position.X, rightHandJoint.Position.Y, rightHandJoint.Position.Z));

            //
            // Check the last time change the bag between two hands
            //
            DateTime checkTime = DateTime.Now;

            if (distanceTwoHands < 0.2f && checkTime.Subtract(lastChangeTicks).TotalSeconds > 2)
            {
                this.isOnLeftHand = !this.isOnLeftHand;
                lastChangeTicks = checkTime;
            }

            if (!this.isOnLeftHand)
            {
                leftHandJoint = rightHandJoint;
                leftElbowJoint = rightElbowJoint;
            }
            //
            // Create World Matrix
            //
            if (leftElbowJoint.Position.Y < leftHandJoint.Position.Y)
            {
                //bagHangTargetJoint = leftElbowJoint;
                bagHangTargetJoint = leftHandJoint;
            }
            else
            {
                bagHangTargetJoint = leftHandJoint;
            }

            DepthImagePoint bagHangTargetDepthPoint = Chooser.Sensor
                .MapSkeletonPointToDepth(bagHangTargetJoint.Position, Chooser.Sensor.DepthStream.Format);

            float scaleFactor = (float)bagHangTargetDepthPoint.Depth / 600;
            

            Matrix transformMatrix = Matrix.CreateTranslation(-320.0f, -240.0f, 0.0f)
                * Matrix.CreateScale(scaleFactor, scaleFactor, 1.0f);

            Vector3 originalVector = new Vector3(bagHangTargetDepthPoint.X, bagHangTargetDepthPoint.Y, bagHangTargetDepthPoint.Depth);
            Vector3 certainVector = Vector3.Transform(originalVector, transformMatrix);

            float rotationYaxi = 0.0f;
            Vector2 leftBoneNormal = Vector2.Normalize( 
                new Vector2(leftHandJoint.Position.X,leftHandJoint.Position.Z ) 
                -
                new Vector2(leftElbowJoint.Position.X,leftElbowJoint.Position.Z) 
            ),
                leftBoneXYProjectionNormal = Vector2.Normalize( 
                    new Vector2(1,0)
                );
            Vector3 boneV3 = new Vector3(leftBoneNormal.X, leftBoneNormal.Y, 0),
                boneXYProjV3 = new Vector3(leftBoneXYProjectionNormal.X, leftBoneXYProjectionNormal.Y, 0);
            rotationYaxi = (float)( Math.PI - Math.Acos(Vector2.Dot(leftBoneXYProjectionNormal, leftBoneNormal)) );
            Vector3 sign = Vector3.Cross(boneV3, boneXYProjV3);
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
        /// 
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        protected override Matrix CreateViewMatrix(Skeleton skeleton)
        {
            return Matrix.CreateLookAt(
                   new Vector3(0.0f, 0.0f, 0.0f),
                   new Vector3(0.0f, 0.0f, 1.0f),
                   Vector3.Down);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        protected override Matrix CreateProjectionMatrix(Skeleton skeleton)
        {
            float nominalVerticalFieldOfView = 45.6f;

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(
                (nominalVerticalFieldOfView * (float)Math.PI / 180.0f),
                this.Game.GraphicsDevice.Viewport.AspectRatio,
                1.0f,
                20000.0f
            );
            return projection;
        }
        /// <summary>
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        
        {
            // If the joint texture isn't loaded, load it now
            
            if (this.trackedSkeleton == null)
            {
                return;
            }

            // Now draw the bag at the left hand joint
            if (trackedSkeleton.Joints[JointType.HandLeft] != null)
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
