using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ModelViewer
{
    public class WorldAxes
    {
        private int size;

        private BasicEffect effect;
        private VertexPositionColor[][] vertices;

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

        public WorldAxes()
        {
            // load our vertices and there color
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

            Size = 5;
            CreateLines();
        }

        private void CreateLines()
        {
            vertices[0][1].Position = new Vector3(Size, 0, 0);
            vertices[1][1].Position = new Vector3(0, Size, 0);
            vertices[2][1].Position = new Vector3(0, 0, Size);
        }

        public void Draw(GraphicsDevice graphics, ArcBallCamera camera)
        {
            if(effect == null)
            {
                effect = new BasicEffect(graphics);
                effect.VertexColorEnabled = true;
            }

            effect.Projection = camera.Projection;
            effect.View = camera.View;

            effect.CurrentTechnique.Passes[0].Apply();
            for(int i = 0; i < 3; i++)
                graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices[i], 0, 1);
        }
    }
}
