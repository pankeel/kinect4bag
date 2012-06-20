using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace ModelViewer.OnScreenMessages
{
    public class DragAndDropMessageState : MessageState
    {
        string helpLabel = "Drag-and-drop model file here to open.";

        public DragAndDropMessageState(SpriteFont font, StateContext context)
            : base(font, context)
        {
            _font = font;
        }

        public override void UpdateAndDraw(SpriteBatch batch, Stopwatch sw)
        {
            batch.Begin();
            batch.DrawString(_font, helpLabel,
                ModelViewControl.ScreenSize / 2 - _font.MeasureString(helpLabel) / 2,
                Color.White);
            batch.End();

            base.UpdateAndDraw(batch, sw);
        }
    }
}
