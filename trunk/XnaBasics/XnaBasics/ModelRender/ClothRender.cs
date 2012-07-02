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
                
                int nIndices = mCloth.getClothIndicesCount();
                int[] pIndices = new int[nIndices];
                mCloth.getClothIndicesContent(pIndices);
                
                // If can't pass raw data, memcpy indexbuffer
                indexBuffer = new IndexBuffer(this.GraphicsDevice, typeof(int), nIndices, BufferUsage.WriteOnly);
                indexBuffer.SetData(pIndices);
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
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            // If the joint texture isn't loaded, load it now
            
            //if (this.trackedSkeleton == null)
            //{
            //    return;
            //}

            //// Now draw the bag at the left hand joint
            //if (trackedSkeleton.Joints[JointType.HandLeft] != null)
            //{
            //    base.Draw(gameTime);
            //}
            //skeletonDrawn = true;

            // Update
            unsafe
            {
                mCloth.StepPhysX(gameTime.ElapsedGameTime.Seconds);
                
                nVertices = mCloth.getClothParticesCount();
                int[] buffer = new int[nVertices];
                mCloth.getClothParticlesContent(buffer);
                Vector3[] vertices = new Vector3[nVertices];
                fixed (int* bytePtr = &buffer[0])
                {
                    Vector3 *vec = (Vector3*)bytePtr;
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
                        new Vector3(vertices[i].X, vertices[i].Y, vertices[i].Z), Color.Gray);
                }
                vertexBuffer = new VertexBuffer(this.GraphicsDevice, VertexPositionColor.VertexDeclaration
                    , nVertices, BufferUsage.WriteOnly);
                vertexBuffer.SetData<Vector3>(vertices);
            }
            // bind to graphics pipeline
            this.GraphicsDevice.Indices = indexBuffer;
            this.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 
                nVertices, 0, nIndices / 3);
        }


    }
}
