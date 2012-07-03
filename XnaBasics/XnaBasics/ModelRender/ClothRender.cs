using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;
using System.Runtime.InteropServices;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    public class ClothRender : Object3D
    {
        //public Vector3 shapeSize;
        //public Vector3 shapePosition;
        //private VertexPositionNormalTexture[] shapeVertices;
        private VertexBuffer vertexBuffer;
        private int nVertices;
        private IndexBuffer indexBuffer;
        private VertexPositionColor[] vertexData;
        private int[] indexData;
        private int nIndices;
        //public Texture2D shapeTexture;
        CGePhysX mCloth;
        BasicEffect effect;

        /// <summary>
        /// Viewing Camera arc.
        /// </summary>
        private float cameraArc = 0;

        /// <summary>
        /// Viewing Camera current rotation.
        /// The virtual camera starts where Kinect is looking i.e. looking along the Z axis, with +X left, +Y up, +Z forward
        /// </summary>
        private float cameraRotation = 0;

        /// <summary>
        /// Viewing Camera distance from origin.
        /// The "Dude" model is defined in centimeters, hence all the units we use here are cm.
        /// </summary>
        private float cameraDistance = 40.0f;

        /// <summary>
        /// Viewing Camera projection matrix.
        /// </summary>
        private Matrix projection;

        /// <summary>
        /// Viewing Camera view matrix.
        /// </summary>
        private Matrix view;

        /// <summary>
        /// Camera starting Distance value.
        /// </summary>
        private const float CameraHeight = 10.0f;
        
        public ClothRender(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            this.scaleFactor = 1.0f;
        }
    
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            mCloth = new CGePhysX();
            mCloth.OnInit();
            string filePath = Path.GetFullPath("..\\..\\..\\Media\\yifu.obj");
            mCloth.createCloth(filePath);

            // Need Debug
            unsafe
            {
                
                nIndices = mCloth.getClothIndicesCount();
                indexData = new int[nIndices];
                mCloth.getClothIndicesContent(indexData);
                
                // If can't pass raw data, memcpy indexbuffer
                indexBuffer = new IndexBuffer(this.GraphicsDevice, typeof(int), nIndices, BufferUsage.WriteOnly);
                indexBuffer.SetData(indexData);

                nVertices = mCloth.getClothParticesCount();
                vertexBuffer = new VertexBuffer(this.GraphicsDevice, VertexPositionColor.VertexDeclaration
                    , nVertices, BufferUsage.WriteOnly);
                vertexData = new VertexPositionColor[nVertices];
            }

             this.effect = new BasicEffect(this.GraphicsDevice);

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
        /// Create the viewing camera.
        /// </summary>
        protected void UpdateViewingCamera()
        {
            GraphicsDevice device = this.Game.GraphicsDevice;

            // Compute camera matrices.
            this.view = Matrix.CreateTranslation(0, -CameraHeight, 0) *
                          Matrix.CreateRotationY(MathHelper.ToRadians(this.cameraRotation)) *
                          Matrix.CreateRotationX(MathHelper.ToRadians(this.cameraArc)) *
                          Matrix.CreateLookAt(
                                                new Vector3(0, 0, -this.cameraDistance),
                                                new Vector3(0, 0, 0),
                                                Vector3.Up);

            // Kinect vertical FOV in degrees
            float nominalVerticalFieldOfView = 91.2f;


            this.projection = Matrix.CreatePerspectiveFieldOfView(
                                                                (nominalVerticalFieldOfView * (float)Math.PI / 180.0f),
                                                                device.Viewport.AspectRatio,
                                                                1,
                                                                10000);
        }

        /// <summary>
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.Green);

            // Update
            mCloth.StepPhysX(1.0f / 60.0f);
            //mCloth.StepPhysX(gameTime.ElapsedGameTime.Seconds);

            //double[] buffer = new double[nVertices*3];
            //mCloth.getClothParticlesContent(buffer);
            //Vector3[] vertices = new Vector3[nVertices];
            float[] fVert = new float[nVertices * 3];
            unsafe
            {
                mCloth.getClothParticlesContent(fVert);
                // memcopy
                for (int i = 0; i < nVertices; ++i)
                {
                    vertexData[i] = new VertexPositionColor(new Vector3(fVert[3 * i], 
                        fVert[3 * i + 1], fVert[3 * i + 2]), Color.Red);
                    //vertexColor[i] = new VertexPositionColor( 
                    //new Vector3(vertices[i].X, vertices[i].Y, vertices[i].Z), Color.Red);
                }
            }
            vertexBuffer.SetData<VertexPositionColor>(vertexData);

            this.UpdateViewingCamera();

            // bind to graphics pipeline
            this.GraphicsDevice.Indices = indexBuffer;
            this.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default; 
            effect.World = Matrix.Identity;
            effect.View = view;
            effect.Projection = projection;
            effect.TextureEnabled = false;
            effect.DiffuseColor = Color.Green.ToVector3();
            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, nVertices, 0, nIndices / 3);
            }

        }

    }
}
