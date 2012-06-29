using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    public class BodyRender : Object3D
    {
        public BodyRender(Game game)
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
            Joint leftShoulder = skeleton.Joints[JointType.ShoulderLeft],
                rightShoulder = skeleton.Joints[JointType.ShoulderRight],
                centerShoulder = skeleton.Joints[JointType.ShoulderCenter],
                clothTarget = centerShoulder;

            //
            // Create World Matrix
            //
            DepthImagePoint clothDepthPoint = Chooser.Sensor.MapSkeletonPointToDepth(clothTarget.Position, Chooser.Sensor.DepthStream.Format);
            float scaleFactor = (float)clothDepthPoint.Depth / 600;

            Matrix transformMatrix = Matrix.CreateTranslation(-320.0f, -240.0f, 0.0f)
                * Matrix.CreateScale(scaleFactor, scaleFactor, 1.0f);

            Vector3 originalVector = new Vector3(clothDepthPoint.X, clothDepthPoint.Y, clothDepthPoint.Depth);
            Vector3 certainVector = Vector3.Transform(originalVector, transformMatrix);

            float rotationYaxi = 0.0f;

            Vector2 shoulderNormal = Vector2.Normalize(new Vector2(rightShoulder.Position.X, rightShoulder.Position.Z) - new Vector2(leftShoulder.Position.X, leftShoulder.Position.Z)),
                shoulderXYPorjNormal = Vector2.Normalize(new Vector2(1, 0));
            Vector3 shoulderV3 = new Vector3(shoulderNormal.X, shoulderNormal.Y, 0),
                shoulderXYProjV3 = new Vector3(shoulderXYPorjNormal.X, shoulderXYPorjNormal.Y, 0);

            rotationYaxi = (float)(Math.PI - Math.Acos(Vector2.Dot(shoulderXYPorjNormal, shoulderNormal)));
            Vector3 sign = Vector3.Cross(shoulderV3, shoulderXYProjV3);
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
            if (trackedSkeleton.Joints[JointType.ShoulderCenter].TrackingState == JointTrackingState.Tracked)
            {

                // Render the 3D model skinned mesh with Skinned Effect.
                Matrix world = this.CreateWorldMatrix(trackedSkeleton),
                    view = this.CreateViewMatrix(trackedSkeleton),
                    projection = this.CreateProjectionMatrix(trackedSkeleton);
                if (this.Model3DAvatar != null)
                {
                    bool isSkinnedEffect = true;
                    foreach (ModelMesh mesh in this.Model3DAvatar.Meshes)
                    {
                        if (mesh.Effects.Count > 0 && mesh.Effects[0].GetType()!=typeof(SkinnedEffect))
                        {
                            isSkinnedEffect = false;

                            break;
                        }
                        foreach (SkinnedEffect effect in mesh.Effects)
                        {
                            
                            //effect.SetBoneTransforms(this.skinTransforms);
                            if (TargetTexture != null && effect.Texture != TargetTexture)
                                effect.Texture = TargetTexture;
                            effect.World = world;
                            effect.View = view;
                            effect.Projection = projection;
                            effect.EnableDefaultLighting();

                            effect.SpecularColor = new Vector3(0.25f);
                            effect.SpecularPower = 16;
                        }

                        mesh.Draw();
                    }

                    if (!isSkinnedEffect)
                        base.Draw(gameTime);
                }
            }
            
            skeletonDrawn = true;
        }



        protected override void LoadContent()
        {

            base.LoadContent();
        }
    }
}
