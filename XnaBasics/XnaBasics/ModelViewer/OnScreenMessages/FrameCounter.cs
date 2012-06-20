using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ModelViewer.OnScreenMessages
{
    public class FrameCounter
    {
        int frameRate = 0;
        int frameCounter = 0;
        SpriteFont _font;
        Stopwatch sw;

        public FrameCounter(SpriteFont font)
        {
            _font = font;
            sw = new Stopwatch();
            sw.Start();
        }

        public void UpdateAndDraw(SpriteBatch batch)
        {
            if (sw.Elapsed > TimeSpan.FromSeconds(1))
            {
                sw.Restart();
                frameRate = frameCounter;
                frameCounter = 0;
            }

            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);

            batch.Begin();
            batch.DrawString(_font, fps,
                ModelViewControl.ScreenSize - _font.MeasureString(fps),
                Color.White);
            batch.End();
        }
    }
}
