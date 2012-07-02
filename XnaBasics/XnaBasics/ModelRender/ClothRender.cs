using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.XnaBasics.ModelRender
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
            mCloth.StepPhysX(gameTime.ElapsedGameTime.Seconds);
            SWIGTYPE_p_physx__PxVec3 vertices;
            mCloth.getClothParticles(vertices, nVertices);
            // memcopy
            VertexPositionColor[] vertexColor = new VertexPositionColor[nVertices];
            for (int i = 0; i < nVertices; ++i)
            {
                vertexColor[i] = new VertexPositionColor(
                    Vector3(vertices[i].x, vertices[i].y, vertices[i].z), Color.Gray);
            }
            vertexBuffer = new VertexBuffer(this.GraphicsDevice, VertexPositionColor.VertexDeclaration
                , nVertices, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);

            // bind to graphics pipeline
            this.GraphicsDevice.Indices = indexBuffer;
            this.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 
                nVertices, 0, nIndices / 3);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            mCloth = new CGePhysX();
            mCloth.OnInit();
            string filePath = Path.GetFullPath("..\\..\\..\\Media\\yifu.obj");
            mCloth.createCloth(filePath);
            //SWIGTYPE_p_p_physx__PxU32 pIndices = 0;
            //SWIGTYPE_p_physx__PxU32 nIndices = 0;

            // Need Debug
            int* pIndices;
            mCloth.getClothIndices((SWIGTYPE_p_p_physx__PxU32)pIndices, (SWIGTYPE_p_physx__PxU32)nIndices);
            // If can't pass raw data, memcpy indexbuffer
            indexBuffer = new IndexBuffer(this.GraphicsDevice, typeof(int), nIndices, BufferUsage.WriteOnly);
            indexBuffer.SetData(pIndices);
        }

    }
}
