using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ModelViewer
{
    public class BoundingBoxRenderer
    {
        #region Fields
        private static BoundingBoxRenderer instance;
        public static BoundingBoxRenderer Instance
        {
            get
            {
                if (instance == null) instance = new BoundingBoxRenderer();
                return instance;
            }
        }

        VertexPositionColor[] verts = new VertexPositionColor[8];
        short[] indices = new short[]
        {
            0, 1,
            1, 2,
            2, 3,
            3, 0,
            0, 4,
            1, 5,
            2, 6,
            3, 7,
            4, 5,
            5, 6,
            6, 7,
            7, 4,
        };

        BasicEffect effect;
        GraphicsDevice device;
        bool initialized = false;
        public bool Enabled = false;
        #endregion

        private BoundingBoxRenderer()
        {
        }

        public void Initialize(GraphicsDevice device, BoundingBox box)
        {
            this.device = device;
            effect = new BasicEffect(device);
            effect.VertexColorEnabled = true;
            effect.LightingEnabled = false;

            Vector3[] corners = box.GetCorners();
            for (int i = 0; i < 8; i++)
            {
                verts[i].Position = corners[i];
                verts[i].Color = Color.White;
            }
            initialized = true;
        }

        public void Draw(ArcBallCamera camera)
        {
            if (!initialized || !Enabled) return;

            effect.Projection = camera.Projection;
            effect.View = camera.View;

            effect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserIndexedPrimitives(
                PrimitiveType.LineList,
                verts,
                0,
                8,
                indices,
                0,
                indices.Length / 2);

        }
    }
}
