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
    public class BagAnimator : Object2D
    {
        /// <summary>
        /// The 3D bag mesh.
        /// </summary>
        public Model currentModel;


        /// <summary>
        /// Gets the KinectChooser from the services.
        /// </summary>
        public KinectChooser Chooser
        {
            get
            {
                return (KinectChooser)this.Game.Services.GetService(typeof(KinectChooser));
            }
        }

        /// <summary>
        /// This flag ensures only request a frame once per update call
        /// across the entire application.
        /// </summary>
        private static bool skeletonDrawn = true;

        /// <summary>
        /// The back buffer where the depth frame is scaled as requested by the Size.
        /// </summary>
        public RenderTarget2D backBuffer;

        /// <summary>
        /// Whether or not the back buffer needs updating.
        /// </summary>
        private bool needToRedrawBackBuffer = true;
        /// <summary>
        /// The last frames skeleton data.
        /// </summary>
        private static Skeleton[] skeletonData;
        /// <summary>
        /// This is the XNA BasicEffect we use to draw.
        /// </summary>
        private BasicEffect effect;

        /// <summary>
        /// This is the array of 3D vertices with associated colors.
        /// </summary>
        private VertexPositionColor[] localCubeVertices;

        private short[] localCubeIndexes;


        public Matrix leftHandSkin;

        public Matrix leftHandWorld;

        /// <summary>
        /// The depth frame as a texture.
        /// </summary>
        private Texture2D bagTexture;

        public BagAnimator(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            float axisHalfLength = 30.0f;
            if (0.0f == axisHalfLength)
            {
                return;
            }

            this.localCubeIndexes = new short[36] 
            {
                0,1,2,2,3,0,
                4,7,6,6,5,4,  
                8,11,10,10,9,8,
                12,13,14,14,15,12,
                16,17,18,18,19,16,
                20,23,22,22,21,20
            };

            this.localCubeVertices = new VertexPositionColor[24]
            {
                // Create Coordinate axes
                new VertexPositionColor(new Vector3(-axisHalfLength,-axisHalfLength,axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(-axisHalfLength, -axisHalfLength, -axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(axisHalfLength, -axisHalfLength, -axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(axisHalfLength, -axisHalfLength, axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(-axisHalfLength, axisHalfLength, axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(-axisHalfLength, axisHalfLength, -axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(axisHalfLength, axisHalfLength, -axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(axisHalfLength, axisHalfLength, axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(-axisHalfLength, -axisHalfLength, axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(-axisHalfLength, axisHalfLength, axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(axisHalfLength, axisHalfLength, axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(axisHalfLength, -axisHalfLength, axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(-axisHalfLength, -axisHalfLength, -axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(-axisHalfLength, axisHalfLength, -axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(axisHalfLength, axisHalfLength, -axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(axisHalfLength, -axisHalfLength, -axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(-axisHalfLength, -axisHalfLength, axisHalfLength),Color.Red),
                new VertexPositionColor( new Vector3(-axisHalfLength, axisHalfLength, axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(-axisHalfLength, axisHalfLength, -axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(-axisHalfLength, -axisHalfLength, -axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(axisHalfLength, -axisHalfLength, axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(axisHalfLength, axisHalfLength, axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(axisHalfLength, axisHalfLength, -axisHalfLength),Color.Red),
                new VertexPositionColor(new Vector3(axisHalfLength, -axisHalfLength, -axisHalfLength),Color.Red)
		    					                
                    
            };
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

            // If we have already drawn this skeleton, then we should retrieve a new frame
            // This prevents us from calling the next frame more than once per update
            if (skeletonDrawn)
            {
                skeletonData = SkeletonStreamRenderer.skeletonData;
                
                this.backBuffer = new RenderTarget2D(
                    this.Game.GraphicsDevice,
                    640,
                    480,
                    false,
                    SurfaceFormat.Color,
                                    //DepthFormat.None,
                    DepthFormat.Depth16,
                    this.Game.GraphicsDevice.PresentationParameters.MultiSampleCount,
                    RenderTargetUsage.PreserveContents);
                skeletonDrawn = false;

            }

            this.needToRedrawBackBuffer = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="world"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        private void DrawBagMeshModel(Matrix world, Matrix view, Matrix projection)
        {

            // Render the 3D model skinned mesh with Skinned Effect.
            if (this.currentModel != null)
            {
                foreach (ModelMesh mesh in this.currentModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.SetBoneTransforms(this.skinTransforms);
                        
                        effect.World = world;
                        effect.View = view;
                        effect.Projection = projection;
                        effect.EnableDefaultLighting();

                        effect.SpecularColor = new Vector3(0.25f);
                        effect.SpecularPower = 16;
                    }
                    
                    mesh.Draw();
                }
            }
        }
        /// <summary>
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        
        {

            // If the joint texture isn't loaded, load it now
            if (null == this.effect)
            {
                this.LoadContent();
            }

            if (skeletonData == null)
            {
                return;
            }

            if (null == this.localCubeVertices)
            {
                return;
            }
            foreach (var skeleton in skeletonData)
            {
                switch (skeleton.TrackingState)
                {
                    case SkeletonTrackingState.Tracked:
                        
                        // Now draw the bag at the left hand joint
                        if (skeleton.Joints[JointType.HandLeft] != null)
                        {
                            Joint j = skeleton.Joints[JointType.HandLeft];

                            
                            DepthImagePoint dpt = Chooser.Sensor.MapSkeletonPointToDepth(j.Position, Chooser.Sensor.DepthStream.Format);
                            float scaleFactor = (float)dpt.Depth / 600;
                            //Vector3 originalVector = new Vector3(dpt.X - 320, dpt.Y - 240, dpt.Depth);
                            //Vector3 certainVector = new Vector3(originalVector.X * scaleFactor, originalVector.Y * scaleFactor, originalVector.Z);


                            Matrix transformMatrix = Matrix.CreateTranslation(-320.0f, -240.0f, 0.0f)
                                * Matrix.CreateScale(scaleFactor, scaleFactor, 1.0f);

                            Vector3 originalVector = new Vector3(dpt.X , dpt.Y, dpt.Depth);
                            Vector3 certainVector = Vector3.Transform(originalVector, transformMatrix);
                            
                            Matrix world = Matrix.CreateScale(30.0f,30.0f,30.0f)
                                * Matrix.CreateWorld(
                                certainVector,
                                Vector3.Forward,
                                Vector3.Down
                            );
                            
                            
                            Matrix view;

                            view = Matrix.CreateLookAt(
                                new Vector3(0.0f, 0.0f, 0.0f),
                                new Vector3(0.0f,0.0f, 1.0f),
                                Vector3.Down);
                            float nominalVerticalFieldOfView = 45.6f;
                            
                            Matrix projection = Matrix.CreatePerspectiveFieldOfView(
                                (nominalVerticalFieldOfView * (float)Math.PI / 180.0f),
                                this.Game.GraphicsDevice.Viewport.AspectRatio,
                                1.0f,
                                20000.0f
                            );
                            this.effect.World = world;
                            this.effect.View = view;
                            this.effect.Projection = projection;

                            this.DrawBagMeshModel(world, view, projection);
                            //
                            // Draw the cube 3D object
                            //
                            /*foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
                            {
                                pass.Apply();
                                this.Game
                                    .GraphicsDevice
                                    .DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, this.localCubeVertices, 0, 24, this.localCubeIndexes, 0, 12);

                            }*/
                        }

                        break;
                }
            }


            skeletonDrawn = true;
            base.Draw(gameTime);
        }



        protected override void LoadContent()
        {

            
            this.effect = new BasicEffect(this.Game.GraphicsDevice);
            if (null == this.effect)
            {
                throw new InvalidOperationException("Error creating Basic Effect shader.");
            }

            this.effect.VertexColorEnabled = true;


            base.LoadContent();
        }
    }
}
