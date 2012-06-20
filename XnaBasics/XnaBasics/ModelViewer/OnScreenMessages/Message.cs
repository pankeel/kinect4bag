using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace ModelViewer.OnScreenMessages
{
    public class Message : StateContext
    {
        private static Message instance;
        public static Message Instance
        {
            get
            {
                if (instance == null) instance = new Message();
                return instance;
            }
        }

        Stopwatch sw;
        SpriteFont font;
        bool showHelper = true;
        bool toHide = false;
        FrameCounter fpsCounter;
        public bool ShowFPS = true;

        private Message() : base()
        {
            sw = new Stopwatch();
        }

        public void Initialize(ContentManager Content)
        {
            font = Content.Load<SpriteFont>(@"Fonts\Arial24Font");
            fpsCounter = new FrameCounter(font);
        }

        public void BeginLoading()
        {
            sw.Reset();
            sw.Start();
            SetState(new LoadingMessageState(font, this));
        }

        public void EndLoading()
        {
            sw.Reset();
            if (showHelper) SetState(new DragAndDropMessageState(font, this));
            else Hide();
        }

        public void Hide()
        {
            if (!toHide)
            {
                toHide = true;
                return;
            }
            SetState(null);
            showHelper = false;
        }

        public void Draw(SpriteBatch batch)
        {
            if (ShowFPS) fpsCounter.UpdateAndDraw(batch);
            if (CurrentState != null) (CurrentState as MessageState).UpdateAndDraw(batch, sw);            
        }
    }
}
