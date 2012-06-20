using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ModelViewer
{
    public class Input
    {
        private KeyboardState kbState;
        private KeyboardState prevKbState;
        private MouseState mState;
        private MouseState prevMState;

        public void Update()
        {
            prevKbState = kbState;
            prevMState = mState;
            kbState = Keyboard.GetState();
            mState = Mouse.GetState();
        }

        public bool KeyDown(Keys key)
        {
            return kbState.IsKeyDown(key);
        }

        public bool KeyPress(Keys key)
        {
            return kbState.IsKeyDown(key) && prevKbState.IsKeyUp(key);
        }

        public Vector2 MousePosition
        {
            set { Mouse.SetPosition((int)value.X, (int)value.Y); prevMState = mState; mState = Mouse.GetState(); }
            get { return new Vector2(mState.X, mState.Y); }
        }

        public Vector2 MouseDelta { get { return new Vector2(mState.X - prevMState.X, mState.Y - prevMState.Y); } }

        public bool LeftDown { get { return mState.LeftButton == ButtonState.Pressed; } }

        public bool RightDown { get { return mState.RightButton == ButtonState.Pressed; } }

        public bool LeftClick
        {
            get
            {
                return mState.LeftButton == ButtonState.Pressed &&
                    prevMState.LeftButton == ButtonState.Released;
            }
        }

        public bool RightClick
        {
            get
            {
                return mState.RightButton == ButtonState.Pressed &&
                    prevMState.RightButton == ButtonState.Released;
            }
        }

        public int MouseWheelDelta
        {
            get { return mState.ScrollWheelValue - prevMState.ScrollWheelValue; }
        }

        public float X { get { return mState.X; } }
        public float Y { get { return mState.Y; } }
    }
}
