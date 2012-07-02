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
        private int nIndices;
        //public Texture2D shapeTexture;
        CGePhysX mCloth;
        BasicEffect effect;
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
                int[] pIndices = new int[nIndices];
                mCloth.getClothIndicesContent(pIndices);
                
                // If can't pass raw data, memcpy indexbuffer
                indexBuffer = new IndexBuffer(this.GraphicsDevice, typeof(int), nIndices, BufferUsage.WriteOnly);
                indexBuffer.SetData(pIndices);
                
            }


             this.effect = new BasicEffect(this.GraphicsDevice);
             this.effect.World = Matrix.Identity;
             this.effect.View = Matrix.CreateLookAt(
                 new Vector3(0, 0, -3000000000.0f),
                 new Vector3(0, 0, 0),
                 Vector3.Up);
             this.effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                 (45.6f * (float)Math.PI / 180.0f),
                 this.GraphicsDevice.Viewport.AspectRatio,
                 1.0f,
                 20000.0f
             );

             this.effect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
             this.effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
             this.effect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
             this.effect.SpecularPower = 5.0f;
             this.effect.Alpha = 1.0f;
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
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);
            // Update
            unsafe
            {
                //mCloth.StepPhysX(gameTime.ElapsedGameTime.Seconds);
                mCloth.StepPhysX(1.0f / 60.0f);

                nVertices = mCloth.getClothParticesCount();
                int[] buffer = new int[nVertices];
                mCloth.getClothParticlesContent(buffer);
                Vector3[] vertices = new Vector3[nVertices];
                fixed (int* bytePtr = &buffer[0])
                {
                    Vector3* vec = (Vector3*)bytePtr;
                    for (int i = 0; i < nVertices; i++, vec++)
                    {
                        vertices[i].X = vec->X;
                        vertices[i].Y = vec->Y;
                        vertices[i].Z = vec->Z;
                    }
                }

                // memcopy
                VertexPositionColor[] vertexColor = new VertexPositionColor[nVertices];
                for (int i = 0; i < nVertices; ++i)
                {
                    vertexColor[i] = new VertexPositionColor(
                        new Vector3(vertices[i].X, vertices[i].Y, vertices[i].Z), Color.Red);
                }
                vertexBuffer = new VertexBuffer(this.GraphicsDevice, VertexPositionColor.VertexDeclaration
                    , nVertices, BufferUsage.WriteOnly);
                vertexBuffer.SetData<VertexPositionColor>(vertexColor);

                // bind to graphics pipeline
                this.GraphicsDevice.Indices = indexBuffer;
                this.GraphicsDevice.SetVertexBuffer(vertexBuffer);

                foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, nVertices, 0, nIndices / 3);
                }
                
            }

        }

    }
}
