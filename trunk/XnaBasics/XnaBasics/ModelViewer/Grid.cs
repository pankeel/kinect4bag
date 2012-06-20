using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using ModelViewer.

namespace ModelViewer
{
    public class Grid
    {

                int gridScale, numGridLines;

        public int GridScale
        {
            set
            {
                gridScale = value;
                CreateGrid();
            }
        }

        public int NumGridLines
        {
            set //must be an odd number. So if even, add a line
            {
                int num = value;
                //add modulus to check for even and change to odd
                numGridLines = num;
            }
        }

        

        Color lineColor, ordinalLineColor;


        BasicEffect bfx;
        VertexBuffer vb;
        int vertexCount;

        ModelViewControl parentControl;

        public Grid(ModelViewControl pc)
        {
            parentControl = pc;

            //default grid specs
            gridScale = 80;  
            numGridLines = 15; 

            bfx = new BasicEffect(parentControl.GraphicsDevice);
            bfx.VertexColorEnabled = true;
            bfx.LightingEnabled = false;

            CreateGrid();
            
        }

        private void CreateGrid()
        {
            lineColor = Color.Gray;
            ordinalLineColor = Color.Black;

            vertexCount = numGridLines * 4;

            VertexPositionColor[] vertices = new VertexPositionColor[vertexCount];

            vb = new VertexBuffer(parentControl.GraphicsDevice, typeof(VertexPositionColor), vertexCount, BufferUsage.WriteOnly);

            for (int i = 0; i < numGridLines ; i++)
            {
                float currentLinePosition = (gridScale * 0.5f) - (((float)gridScale / (numGridLines - 1)) * i);

                Color thisLinesColor = Math.Abs(currentLinePosition) < 0.1f ? ordinalLineColor : lineColor;

                //X line
                vertices[i * 4 + 0] = new VertexPositionColor(new Vector3(-gridScale / 2, 0, currentLinePosition), thisLinesColor);
                vertices[i * 4 + 1] = new VertexPositionColor(new Vector3(gridScale / 2, 0, currentLinePosition), thisLinesColor);
                //Z line
                vertices[i * 4 + 2] = new VertexPositionColor(new Vector3(currentLinePosition, 0, -gridScale/2), thisLinesColor);
                vertices[i * 4 + 3] = new VertexPositionColor(new Vector3(currentLinePosition, 0, gridScale / 2), thisLinesColor);


            }

            vb.SetData<VertexPositionColor>(vertices);
        }


        public void Draw()
        {
            bfx.World = Matrix.Identity;
            bfx.View = parentControl.camera.View;
            bfx.Projection = parentControl.camera.Projection;

            parentControl.GraphicsDevice.SetVertexBuffer(vb);

            foreach (EffectPass ep in bfx.CurrentTechnique.Passes)
            {
                ep.Apply();
                parentControl.GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, vertexCount * 2);
            }
        }
    }
}
