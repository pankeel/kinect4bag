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
        //private VertexBuffer vertexBuffer;
        //private IndexBuffer indexBuffer;
        private int mNbVertices;
        private int mNbIndices;
        private int[] mIndexData;
        private float[] mVertexStream;
        private float[] mNormalStream;
        private float[] mTextureStream;
        private Texture2D mClothTexture;
        //private VertexPositionColor[] mVertexData;
        private VertexPositionNormalTexture[] mVertexData;
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
        private float cameraDistance = -40.0f;

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

        public Vector3[] gBoxPosition = new Vector3[31];

        public int[] gCollIndexPair = { 1, 0, 2, 1, 3, 2, 4, 3, 5, 4, 6, 0, 7, 
                                    6, 8, 7, 9, 8, 10, 9, 11, 0, 12, 11, 
                                    13, 12, 14, 13, 15, 14, 16, 15, 17, 13, 
                                    18, 17, 19, 18, 20, 19, 21, 20, 22, 21, 
                                    23, 20, 24, 13, 25, 24, 26, 25, 27, 26, 
                                    28, 27, 29, 28, 30, 27 };
        public float[] gCollRadius = { 0.200000f, 0.150000f, 0.150000f, 0.100000f, 0.100000f, 0.100000f, 0.150000f, 0.150000f,
                                        0.100000f, 0.100000f, 0.100000f, 0.250000f, 0.250000f, 0.200000f, 0.100000f, 0.100000f,
                                        0.200000f, 0.150000f, 0.150000f, 0.130000f, 0.150000f, 0.180000f, 0.050000f, 0.050000f,
                                        0.150000f, 0.150000f, 0.130000f, 0.150000f, 0.180000f, 0.050000f, 0.050000f
                                     };
        public float[] gCollPos = { 0.000000f, 0.000000f, 0.000000f,
                                    0.182560f, -0.166711f, 0.052266f,
                                    0.101902f, -0.907815f, 0.196846f,
                                    0.037522f, -1.569116f, -0.102427f,
                                    0.055697f, -1.646042f, 0.105217f,
                                    0.067290f, -1.648367f, 0.215836f,
                                    -0.141890f, -0.186983f, 0.085164f,
                                    -0.066574f, -0.867081f, 0.412992f,
                                    -0.151822f, -1.395693f, -0.070659f,
                                    -0.180680f, -1.573331f, 0.062266f,
                                    -0.186709f, -1.610105f, 0.167428f,
                                    -0.007249f, 0.205653f, 0.008180f,
                                    -0.010206f, 0.410989f, 0.030083f,
                                    -0.012232f, 0.617055f, 0.047453f,
                                    -0.004935f, 0.773784f, 0.034578f,
                                    -0.000600f, 0.922243f, -0.014431f,
                                    0.000222f, 1.079146f, -0.057277f,
                                    1.338583f, 0.714567f, 0.010548f,
                                    1.341517f, 0.232758f, -0.056876f,
                                    1.375151f, -0.039633f, 0.136173f,
                                    1.391969f, -0.175829f, 0.232698f,
                                    1.421101f, -0.234805f, 0.239378f,
                                    1.438532f, -0.284744f, 0.245988f,
                                    1.394281f, -0.245142f, 0.265072f,
                                    -1.364308f, 0.689457f, 0.047694f,
                                    -1.373673f, 0.191988f, -0.023668f,
                                    -1.365733f, -0.123844f, 0.091970f,
                                    -1.361764f, -0.281761f, 0.149789f,
                                    -1.390399f, -0.327233f, 0.199259f,
                                    -1.406978f, -0.367242f, 0.239160f,
                                    -1.351982f, -0.332152f, 0.216975f
                                  };
//        public float[] gCollPos = {
//        0.000000f,	0.000000f,	0.000000f,
//0.052266f,	-0.166711f,	0,
//0.196846f,	-0.907815f,	0,
//-0.102427f,	-1.569116f,	0,
//0.105217f,	-1.646042f,	0,
//0.215836f,	-1.648367f,	0,
//0.085164f,	-0.186983f,	0,
//0.412992f,	-0.867081f,	0,
//-0.070659f,	-1.395693f,	0,
//0.062266f,	-1.573331f,	0,
//0.167428f,	-1.610105f,	0,
//0.008180f,	0.205653f,	0,
//0.030083f,	0.410989f,	0,
//0.047453f,	0.617055f,	0,
//0.034578f,	0.773784f,	0,
//-0.014431f,	0.922243f,	0,
//-0.057277f,	1.079146f,	0,
//0.010548f,	0.714567f,	0,
//-0.056876f,	0.232758f,	0,
//0.136173f,	-0.039633f,	0,
//0.232698f,	-0.175829f,	0,
//0.239378f,	-0.234805f,	0,
//0.245988f,	-0.284744f,	0,
//0.265072f,	-0.245142f,	0,
//0.047694f,	0.689457f,	0,
//-0.023668f,	0.191988f,	0,
//0.091970f,	-0.123844f,	0,
//0.149789f,	-0.281761f,	0,
//0.199259f,	-0.327233f,	0,
//0.239160f,	-0.367242f,	0,
//0.216975f,	-0.332152f,	0
//                                  };

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            mCloth = new CGePhysX();
            mCloth.OnInit();
            //string filePath = Path.GetFullPath("..\\..\\..\\Media\\yifu.obj");
            string filePath = Path.GetFullPath("..\\..\\..\\Media\\skirt2.obj");
            //mClothTexture = this.Game.Content.Load<Texture2D>("..\\..\\..\\Media\\uvs.jpg");
            mClothTexture = this.Game.Content.Load<Texture2D>("uvs");

            // collision spheres
            unsafe
            {
                //gBoxPosition = new Vector3(-0.2216031f, 0.4518394f, 1.912647f);
                //gBoxPosition = new Vector3(0, 0, 3.5f);
                //gBoxRadius = 5.0f;
                //float[] collRadius = { gBoxRadius };
                //float[] collPos = { gBoxPosition.X, gBoxPosition.Y, gBoxPosition.Z };
                //int[] collIdxPair = { };
                // scale
                for (int i = 0; i < 31; ++i)
                {
                    gCollPos[3 * i] *= 10.0f;
                    gCollPos[3 * i + 1] *= 10.0f;
                    gCollPos[3 * i + 2] *= 10.0f;
                    gCollPos[3 * i + 2] = 1.00f;
                    gCollRadius[i] *= 12.0f;
                    //gCollRadius[i] = 0.2f;
                }
                mCloth.addCollisionSpheres(31, gCollPos, gCollRadius, 60, gCollIndexPair);
            }
            float[] offset = {0, -14.0f, 0};
            mCloth.createCloth(filePath, 10.0f, offset);

            // box
            for (int i = 0; i < 31; ++i)
            {
                gBoxPosition[i] = new Vector3(gCollPos[3 * i], gCollPos[3 * i + 1], gCollPos[3 * i + 2]);
            }

            // cloth details
            unsafe
            {
                mNbIndices = mCloth.getClothIndicesCount();
                mIndexData = new int[mNbIndices];
                mCloth.getClothIndicesContent(mIndexData);

                mNbVertices = mCloth.getClothParticesCount();
                mVertexData = new VertexPositionNormalTexture[mNbVertices];
                mVertexStream = new float[mNbVertices * 3];
                mNormalStream = new float[mNbVertices * 3];

                mTextureStream = new float[mNbVertices * 2];
                mCloth.getClothTextureStream(mTextureStream);
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

        public Vector3[] gBoxVertices = {
                                        // Z+
		new Vector3(-1,-1, 1), new Vector3(-1, 1, 1), new Vector3( 1, 1, 1), new Vector3( 1,-1, 1),
	// X+
		new Vector3( 1,-1, 1), new Vector3( 1, 1, 1), new Vector3( 1, 1,-1), new Vector3( 1,-1,-1),
	// Z-
		new Vector3( 1,-1,-1), new Vector3( 1, 1,-1), new Vector3(-1, 1,-1), new Vector3(-1,-1,-1),
	// X-
		new Vector3(-1,-1,-1), new Vector3(-1, 1,-1), new Vector3(-1, 1, 1), new Vector3(-1,-1, 1),
	// Y+
		new Vector3(-1, 1, 1), new Vector3(-1, 1,-1), new Vector3( 1, 1,-1), new Vector3( 1, 1, 1),
	// Y-
        new Vector3(-1,-1,-1), new Vector3(-1,-1, 1), new Vector3( 1,-1, 1), new Vector3( 1,-1,-1)
                                   };

        public int[] gBoxIndices = { 0, 1, 2, 0, 2, 3 };

        public void drawBox(Vector3 position, float radius)
        {
            VertexPositionColor[] boxVertexData = new VertexPositionColor[24];
            Vector3[] boxVertex = new Vector3[24];
            for(int i = 0; i < 6; ++i)
                for (int j = 0; j < 4; ++j)
                {
                    boxVertex[4 * i + j] = gBoxVertices[4 * i + j] * radius + position;
                    boxVertexData[4 * i + j] = new VertexPositionColor(
                        boxVertex[4 * i + j], Color.Green);
                }
            int[] boxIndex = new int[36];
            for ( int i = 0; i < 6; ++i)
            {
                for ( int j = 0; j < 6; ++j)
                    boxIndex[i * 6 + j] = i * 4 + gBoxIndices[j];
            }

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            effect.World = Matrix.Identity;
            effect.View = view;
            effect.TextureEnabled = false;
            effect.LightingEnabled = false;
            effect.Projection = projection;
            effect.DiffuseColor = Color.Yellow.ToVector3();
            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, nVertices, 0, nIndices / 3);
                this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                    boxVertexData, 0, 24, boxIndex, 0, 12);
            }
        }

        /// <summary>
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            // Update
            mCloth.StepPhysX(1.0f / 60.0f);
            //mCloth.StepPhysX(gameTime.ElapsedGameTime.Seconds);

            unsafe
            {
                // vertex
                mCloth.getClothParticlesContent(mVertexStream);
                // normal
                mCloth.getClothNormalStream(mNormalStream);
                // memcopy
                for (int i = 0; i < mNbVertices; ++i)
                {
                    //mVertexData[i] = new VertexPositionColor(new Vector3(mVertexStream[3 * i],
                    //    mVertexStream[3 * i + 1], mVertexStream[3 * i + 2]), Color.Red);
                    mVertexData[i] = new VertexPositionNormalTexture(
                        new Vector3(mVertexStream[3 * i], mVertexStream[3 * i + 1], mVertexStream[3 * i + 2]),
                        new Vector3(mNormalStream[3 * i], mNormalStream[3 * i + 1], mNormalStream[3 * i + 2]),
                        new Vector2(mTextureStream[2 * i], mTextureStream[2 * i + 1])
                        );
                }
            }
            //vertexBuffer.SetData<VertexPositionColor>(vertexData);

            this.UpdateViewingCamera();

            // bind to graphics pipeline
            //this.GraphicsDevice.Indices = indexBuffer;
            //this.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default; 
            effect.World = Matrix.Identity;
            effect.View = view;
            effect.Projection = projection;
            effect.TextureEnabled = true;
            //effect.DiffuseColor = Color.Green.ToVector3();
            effect.EnableDefaultLighting();
            effect.Texture = mClothTexture;
            effect.SpecularColor = new Vector3(0.25f);
            effect.SpecularPower = 16;
            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, nVertices, 0, nIndices / 3);
                this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                    mVertexData, 0, mNbVertices, mIndexData, 0, mNbIndices / 3);
            }

            for (int i = 0; i < 31; ++i )
            {
                drawBox(gBoxPosition[i], gCollRadius[i]);
            }
        }

    }
}
