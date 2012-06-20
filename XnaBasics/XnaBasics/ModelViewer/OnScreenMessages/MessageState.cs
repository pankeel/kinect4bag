using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace ModelViewer.OnScreenMessages
{
    public class MessageState : State
    {
        protected SpriteFont _font;

        public MessageState(SpriteFont font, StateContext context)
            : base(context)
        {
            _font = font;
        }

        public virtual void UpdateAndDraw(SpriteBatch batch, Stopwatch sw)
        {

        }
    }
}
