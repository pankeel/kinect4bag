using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace ModelViewer.OnScreenMessages
{
    public class LoadingMessageState : MessageState
    {
        const int DELAY = 500;
        string[] labels = { "Loading", "Loading.", "Loading..", "Loading..." };
        int curId = 0;

        public LoadingMessageState(SpriteFont font, StateContext context)
            : base(font, context)
        {
            _font = font;
        }

        public override void UpdateAndDraw(SpriteBatch batch, Stopwatch sw)
        {
            if (sw.ElapsedMilliseconds > DELAY)
            {
                sw.Restart();
                curId = (curId + 1) % labels.Length;
            }

            batch.Begin();
            batch.DrawString(_font, labels[curId],
                ModelViewControl.ScreenSize / 2 - _font.MeasureString(labels[curId]) / 2,
                Color.White);
            batch.End();

            base.UpdateAndDraw(batch, sw);
        }
    }
}
