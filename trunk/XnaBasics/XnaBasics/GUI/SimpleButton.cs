using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    public class SimpleButton
    {
        public Rectangle Bound { get; set; }
        public Texture2D tex { get; set; }

        public RenderTarget2D backBuffer;

        private GraphicsDevice graphics;

        public bool selected {get;set;}
        public bool hover { get; set; }

        public SimpleButton(Texture2D t2d, int x, int y, int width, int height)
        {
            this.tex = t2d;
            this.selected = false;
            this.hover = false;
            this.Bound = new Rectangle(x, y, width, height);


        }

        public void SetGraphicsDevice(GraphicsDevice graphics)
        {
            this.graphics = graphics;
            this.backBuffer = new RenderTarget2D(
                graphics,
                Bound.Width,
                Bound.Height);  
        }

        public void Draw()
        {

        }

    }
}
