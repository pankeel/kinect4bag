using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace ModelViewer
{
    public class BoundingSphereRenderer
    {
        private static BoundingSphereRenderer instance;
        public static BoundingSphereRenderer Instance
        {
            get
            {
                if (instance == null) instance = new BoundingSphereRenderer();
                return instance;
            }
        }

        VertexBuffer vertBuffer;
        BasicEffect effect;
        int sphereResolution;
        GraphicsDevice graphicsDevice;
        BoundingSphere sphere;
        public bool Enabled = false;

        /// <summary>
        /// Initializes the graphics objects for rendering the spheres. If this method isn't
        /// run manually, it will be called the first time you render a sphere.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device to use when rendering.</param>
        /// <param name="sphereResolution">The number of line segments 
        ///     to use for each of the three circles.</param>
        public void Initialize(GraphicsDevice graphicsDevice, BoundingSphere sphere)
        {
            this.graphicsDevice = graphicsDevice;

            this.sphere = sphere;

            this.sphereResolution = 30;

            effect = new BasicEffect(graphicsDevice);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = false;

            VertexPositionColor[] verts = new VertexPositionColor[(sphereResolution + 1) * 3];

            int index = 0;

            float step = MathHelper.TwoPi / (float)sphereResolution;

            //create the loop on the XY plane first
            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
            {
                verts[index++] = new VertexPositionColor(
                    new Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f),
                    Color.White);
            }

            //next on the XZ plane
            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
            {
                verts[index++] = new VertexPositionColor(
                    new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a)),
                    Color.White);
            }

            //finally on the YZ plane
            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
            {
                verts[index++] = new VertexPositionColor(
                    new Vector3(0f, (float)Math.Cos(a), (float)Math.Sin(a)),
                    Color.White);
            }

            vertBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), verts.Length, BufferUsage.None);

            vertBuffer.SetData(verts);

        }

        public void Render(ArcBallCamera camera)
        {
            if (vertBuffer == null || !Enabled)
                return;

            graphicsDevice.SetVertexBuffer(vertBuffer);

            effect.World = Matrix.CreateScale(sphere.Radius) *
                            Matrix.CreateTranslation(sphere.Center);

            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.DiffuseColor = Color.Red.ToVector3();

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                //render each circle individually
                graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, sphereResolution);
                graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, sphereResolution + 1, sphereResolution);
                graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, (sphereResolution + 1) * 2, sphereResolution);

            }
        }
    }
}