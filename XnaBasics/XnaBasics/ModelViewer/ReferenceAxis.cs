using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ModelViewer
{
    public class ReferenceAxis
    {
        private static ReferenceAxis instance;
        public static ReferenceAxis Instance
        {
            get
            {
                if (instance == null) instance = new ReferenceAxis();
                return instance;
            }
        }

        Matrix view, projection, offset;
        BasicEffect effect;
        Vector3 originPos = new Vector3(-90, -80, -20);
        GraphicsDevice device;
        VertexPositionColor[][] vertices;

        private int size;
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                CreateLines();
            }
        }

        private ReferenceAxis()
        {
            vertices = new VertexPositionColor[3][];
            for (int x = 0; x < 3; x++)
            {
                vertices[x] = new VertexPositionColor[2];
                for (int y = 0; y < 2; y++)
                    vertices[x][y] = new VertexPositionColor();
            }

            vertices[0][0].Color = Color.Red;
            vertices[0][1].Color = Color.Red;
            vertices[1][0].Color = Color.Green;
            vertices[1][1].Color = Color.Green;
            vertices[2][0].Color = Color.Blue;
            vertices[2][1].Color = Color.Blue;

            // the first vertex is at the origin
            vertices[0][0].Position = Vector3.Zero;
            vertices[1][0].Position = Vector3.Zero;
            vertices[2][0].Position = Vector3.Zero;

            Size = 10;
            CreateLines();
        }

        private void CreateLines()
        {
            vertices[0][1].Position = new Vector3(Size, 0, 0);
            vertices[1][1].Position = new Vector3(0, Size, 0);
            vertices[2][1].Position = new Vector3(0, 0, Size);
        }

        public void Initialize(GraphicsDevice device)
        {
            this.device = device;
            offset = Matrix.CreateTranslation(originPos);
            
            projection = Matrix.CreateOrthographic(200, 200, 1, 500);
            view = Matrix.CreateLookAt(Vector3.UnitZ * 100, Vector3.Zero, Vector3.Up);

            effect = new BasicEffect(device);
            effect.VertexColorEnabled = true;
        }

        public void Draw(ArcBallCamera camera)
        {
            Vector3 scale;
            Vector3 translation;
            Quaternion rotation;
            camera.View.Decompose(out scale, out rotation, out translation);
            Matrix mRotation = Matrix.CreateFromQuaternion(rotation);

            Matrix mInv =  Matrix.Invert(camera.Projection)* 
                Matrix.Invert(Matrix.CreateTranslation(translation));

            effect.Projection = projection;
            effect.View = camera.View * Matrix.Invert(Matrix.CreateTranslation(translation)) * offset;

            effect.CurrentTechnique.Passes[0].Apply();
            for (int i = 0; i < 3; i++)
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices[i], 0, 1);

        }
    }
}
