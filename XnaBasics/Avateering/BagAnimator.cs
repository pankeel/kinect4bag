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


namespace Microsoft.Samples.Kinect.Avateering
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class BagAnimator : DrawableGameComponent
    {
        /// <summary>
        /// The 3D bag mesh.
        /// </summary>
        private Model currentModel;



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

        public BagAnimator(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            float axisHalfLength = 3.0f;
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
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="world"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        public void Draw(GameTime gameTime, Matrix world, Matrix view, Matrix projection)
        {
            // Render the 3D model bag mesh with Skinned Effect.
            /*
            foreach (ModelMesh mesh in this.currentModel.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
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
            */

            if (null == this.localCubeVertices )
            {
                return;
            }
            if (this.leftHandSkin != null && this.leftHandWorld != null)
            {
                //view = this.leftHandSkin * view;
                //Matrix temp = Matrix.CreateTranslation(this.leftHandWorld.Translation);

                world = this.leftHandWorld * world;
            }
            this.effect.World = world;
            this.effect.View = view;
            this.effect.Projection = projection;

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                // Draw grid vertices as line list
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, this.localCubeVertices,0,24,this.localCubeIndexes,0,12);
                
            }


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
