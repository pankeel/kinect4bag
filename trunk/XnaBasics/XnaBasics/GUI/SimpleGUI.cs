using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    public class SimpleGUI : DrawableGameComponent
    {
        private List<SimpleButton> ButtonsLeft;
        private List<SimpleButton> ButtonsRight;
        private Texture2D PanelBG;
        private Texture2D ButtonBG;
        private Texture2D CursorTex;
        private RenderTarget2D renderTarget;

        public Rectangle BoundLeft { get; set; }
        public Rectangle BoundRight { get; set; }

        public Vector2 Size { get; set; }
        private Point CursorPos;
        public int margin_p { get; set; }
        public int margin_b { get; set; }
        public int width_top { get; set; }
        public int width_side { get; set; }
        public int height_side { get; set; }
        public int width_cursor { get; set; }
        public int height_cursor { get; set; }

        public int LeftOffset { get; set; }
        public int RightOffset { get; set; }

        public const int MOUSE_ON_NOTHING = 0;
        
        public const int MOUSE_ON_LEFT_TOP = 1;
        public const int MOUSE_ON_LEFT_NEUTRAL = 2;
        public const int MOUSE_ON_LEFT_BOTTOM = 3;

        public const int MOUSE_ON_RIGHT_TOP = 4;
        public const int MOUSE_ON_RIGHT_NEUTRAL = 5;
        public const int MOUSE_ON_RIGHT_BOTTOM = 6;

        private XnaBasics xnaGame;

        public Object3D ObjectRender { get; set; }

        public SimpleGUI(XnaBasics game)
            : base(game)
        {
            this.xnaGame = game;
        }

        public override void Initialize()
        {
            this.PanelBG = Game.Content.Load<Texture2D>("panel_tex");
            this.ButtonBG = Game.Content.Load<Texture2D>("button_tex");
            this.CursorTex = Game.Content.Load<Texture2D>("cursor");

            this.Size = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            this.CursorPos = new Point();
            this.CursorPos.X = this.CursorPos.Y = -1;

            this.margin_p = 10;
            this.margin_b = 10;
            
            this.width_side = 250;
            this.width_top = (int)this.Size.X - margin_p * 2 - width_side * 2;
            this.height_side = (int)this.Size.Y - margin_b * 2;
            this.width_cursor = 80;
            this.height_cursor = 80;

            this.LeftOffset = 0;
            this.RightOffset = 0;

            this.BoundLeft = new Rectangle(margin_p, 0, width_side, this.height_side);
            this.BoundRight = new Rectangle(width_side + margin_p * 4 + width_top, 0, width_side, height_side);

            this.ButtonsLeft = new List<SimpleButton>();
            this.ButtonsRight = new List<SimpleButton>();

            renderTarget = new RenderTarget2D(this.Game.GraphicsDevice,
                        (int)this.Size.X,
                        (int)this.Size.Y,
                        false,
                        SurfaceFormat.Color,
                        DepthFormat.None,
                        this.Game.GraphicsDevice.PresentationParameters.MultiSampleCount,
                        RenderTargetUsage.PreserveContents);
            base.Initialize();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            Texture2D t1 = Game.Content.Load<Texture2D>("cloth");
            SimpleButton b1 = new SimpleButton(t1, 0, 0, 200, 250);
            Texture2D t2 = Game.Content.Load<Texture2D>("header");
            SimpleButton b2 = new SimpleButton(t2, 0, 0, 200, 250);
            Texture2D t3 = Game.Content.Load<Texture2D>("uvs");
            SimpleButton b3 = new SimpleButton(t3, 0, 0, 200, 250);
            this.AddLeft(b1);
            this.AddLeft(b2);
            this.AddLeft(b3);

            Texture2D t4 = Game.Content.Load<Texture2D>("cursor");
            SimpleButton b4 = new SimpleButton(t4, 0, 0, 200, 250);
            Texture2D t5 = Game.Content.Load<Texture2D>("tiny_skin");
            SimpleButton b5 = new SimpleButton(t5, 0, 0, 200, 250);
            this.AddRight(b4);
            this.AddRight(b5);

            base.LoadContent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            this.Game.GraphicsDevice.SetRenderTargets(null);
            this.SharedSpriteBatch.Begin();

            #region DrawLeftPanel
            {
                this.SharedSpriteBatch.Draw(
                        PanelBG,
                        this.BoundLeft,
                        null,
                        Color.White * 0.5f);
                if (ButtonsLeft.Count <= 0)
                    goto BEGINDRAWRIGHT;

                int j;
                int current_y = BoundLeft.Y;
                int partial_height = LeftOffset;
                int remaining_height = BoundLeft.Height;
                Rectangle current_button_rect = new Rectangle();
                for (j = 0; j < ButtonsLeft.Count && partial_height >= 0; j++)
                {
                    partial_height -= ButtonsLeft[j].Bound.Height;
                }
                j--;
                partial_height = -partial_height;
                current_button_rect.X = BoundLeft.X;
                current_button_rect.Y = current_y;
                current_button_rect.Width = ButtonsLeft[j].Bound.Width;
                current_button_rect.Height = partial_height;
                if (current_button_rect.Contains(CursorPos))
                {
                    this.SharedSpriteBatch.Draw(
                            ButtonsLeft[j].tex,
                            current_button_rect,
                            new Rectangle(ButtonsLeft[j].Bound.X, ButtonsLeft[j].tex.Height * (ButtonsLeft[j].Bound.Height - partial_height) / ButtonsLeft[j].Bound.Height, ButtonsLeft[j].tex.Width, ButtonsLeft[j].tex.Height * partial_height / ButtonsLeft[j].Bound.Height),
                            Color.White * 1f);
                    this.ObjectRender.TargetTexture = ButtonsLeft[j].tex;
                }
                else
                {
                    this.SharedSpriteBatch.Draw(
                            ButtonsLeft[j].tex,
                            current_button_rect,
                            new Rectangle(ButtonsLeft[j].Bound.X, ButtonsLeft[j].tex.Height * (ButtonsLeft[j].Bound.Height - partial_height) / ButtonsLeft[j].Bound.Height, ButtonsLeft[j].tex.Width, ButtonsLeft[j].tex.Height * partial_height / ButtonsLeft[j].Bound.Height),
                            Color.White * 0.5f);
                }
                j++;
                remaining_height -= partial_height;
                current_y += partial_height;
                int current_height;
                for (; j < ButtonsLeft.Count && remaining_height >= 0; j++)
                {
                    current_height = remaining_height < ButtonsLeft[j].Bound.Height ? remaining_height : ButtonsLeft[j].Bound.Height;
                    current_button_rect.X = BoundLeft.X;
                    current_button_rect.Y = current_y;
                    current_button_rect.Width = ButtonsLeft[j].Bound.Width;
                    current_button_rect.Height = current_height;
                    if (current_button_rect.Contains(CursorPos))
                    {
                        this.SharedSpriteBatch.Draw(
                            ButtonsLeft[j].tex,
                            current_button_rect,
                            new Rectangle(ButtonsLeft[j].Bound.X, ButtonsLeft[j].Bound.Y, ButtonsLeft[j].tex.Width, ButtonsLeft[j].tex.Height * current_height / ButtonsLeft[j].Bound.Height),
                            Color.White * 1f);
                        this.ObjectRender.TargetTexture = ButtonsLeft[j].tex;
                    }
                    else
                        this.SharedSpriteBatch.Draw(
                            ButtonsLeft[j].tex,
                            current_button_rect,
                            new Rectangle(ButtonsLeft[j].Bound.X, ButtonsLeft[j].Bound.Y, ButtonsLeft[j].tex.Width, ButtonsLeft[j].tex.Height * current_height / ButtonsLeft[j].Bound.Height),
                            Color.White * 0.5f);

                    remaining_height -= ButtonsLeft[j].Bound.Height;
                    current_y += current_height;
                }
            }
            #endregion

            BEGINDRAWRIGHT:
            #region DrawRightPanel
            {
                this.SharedSpriteBatch.Draw(
                        PanelBG,
                        this.BoundRight,
                        null,
                        Color.White * 0.5f);
                if (ButtonsRight.Count <= 0)
                    goto BEGINDRAWCURSOR;

                int j;
                int current_y = BoundRight.Y;
                int partial_height = RightOffset;
                int remaining_height = BoundRight.Height;
                Rectangle current_button_rect = new Rectangle();
                for (j = 0; j < ButtonsRight.Count && partial_height >= 0; j++)
                {
                    partial_height -= ButtonsRight[j].Bound.Height;
                }
                j--;
                partial_height = -partial_height;
                current_button_rect.X = BoundRight.X;
                current_button_rect.Y = current_y;
                current_button_rect.Width = ButtonsRight[j].Bound.Width;
                current_button_rect.Height = partial_height;
                if (current_button_rect.Contains(CursorPos))
                {
                    this.SharedSpriteBatch.Draw(
                            ButtonsRight[j].tex,
                            current_button_rect,
                            new Rectangle(ButtonsRight[j].Bound.X, ButtonsRight[j].tex.Height * (ButtonsRight[j].Bound.Height - partial_height) / ButtonsRight[j].Bound.Height, ButtonsRight[j].tex.Width, ButtonsRight[j].tex.Height * partial_height / ButtonsRight[j].Bound.Height),
                            Color.White * 1f);
                    this.ObjectRender.TargetTexture = ButtonsRight[j].tex;

                }
                else
                {
                    this.SharedSpriteBatch.Draw(
                            ButtonsRight[j].tex,
                            current_button_rect,
                            new Rectangle(ButtonsRight[j].Bound.X, ButtonsRight[j].tex.Height * (ButtonsRight[j].Bound.Height - partial_height) / ButtonsRight[j].Bound.Height, ButtonsRight[j].tex.Width, ButtonsRight[j].tex.Height * partial_height / ButtonsRight[j].Bound.Height),
                            Color.White * 0.5f);
                }
                j++;
                remaining_height -= partial_height;
                current_y += partial_height;
                int current_height;
                for (; j < ButtonsRight.Count && remaining_height >= 0; j++)
                {
                    current_height = remaining_height < ButtonsRight[j].Bound.Height ? remaining_height : ButtonsRight[j].Bound.Height;
                    current_button_rect.X = BoundRight.X;
                    current_button_rect.Y = current_y;
                    current_button_rect.Width = ButtonsRight[j].Bound.Width;
                    current_button_rect.Height = current_height;
                    if (current_button_rect.Contains(CursorPos))
                    {
                        this.SharedSpriteBatch.Draw(
                            ButtonsRight[j].tex,
                            current_button_rect,
                            new Rectangle(ButtonsRight[j].Bound.X, ButtonsRight[j].Bound.Y, ButtonsRight[j].tex.Width, ButtonsRight[j].tex.Height * current_height / ButtonsRight[j].Bound.Height),
                            Color.White * 1f);
                        this.ObjectRender.TargetTexture = ButtonsRight[j].tex;

                    }
                    else
                        this.SharedSpriteBatch.Draw(
                            ButtonsRight[j].tex,
                            current_button_rect,
                            new Rectangle(ButtonsRight[j].Bound.X, ButtonsRight[j].Bound.Y, ButtonsRight[j].tex.Width, ButtonsRight[j].tex.Height * current_height / ButtonsRight[j].Bound.Height),
                            Color.White * 0.5f);

                    remaining_height -= ButtonsRight[j].Bound.Height;
                    current_y += current_height;
                }
            }
            #endregion

            BEGINDRAWCURSOR:
            //draw cursor
            if (this.CursorPos.X >= 0 && this.CursorPos.Y >= 0)
                this.SharedSpriteBatch.Draw(
                            CursorTex,
                            new Rectangle((int)this.CursorPos.X - width_cursor / 2, (int)this.CursorPos.Y - height_cursor / 2, width_cursor, height_cursor),
                            null,
                            Color.White * 1f);

            this.SharedSpriteBatch.End();
        }

        private int MouseHovering()
        {
            if (ButtonsLeft == null)
                return MOUSE_ON_NOTHING;
            foreach (SimpleButton sb in ButtonsLeft)
            {
                sb.hover = false;
            }

            Vector2 cursor_r = XnaBasics.RightHand;
            Vector2 cursor_l = XnaBasics.LeftHand;
            
            this.CursorPos.X = this.CursorPos.Y = -1;

            if (cursor_l.X > BoundLeft.X && cursor_l.X < BoundLeft.X + BoundLeft.Width
              && cursor_l.Y > BoundLeft.Y && cursor_l.Y < BoundLeft.Y + BoundLeft.Height)
            {
                this.CursorPos.X = (int)cursor_l.X;
                this.CursorPos.Y = (int)cursor_l.Y;

                float delta = cursor_l.Y-BoundLeft.Y;
                if (delta < BoundLeft.Height / 4)
                    return MOUSE_ON_LEFT_TOP;
                else if (delta > BoundLeft.Height * 1 / 2)
                    return MOUSE_ON_LEFT_BOTTOM;
                else
                    return MOUSE_ON_LEFT_NEUTRAL;
            }
            else if (cursor_r.X > BoundRight.X && cursor_r.X < BoundRight.X + BoundRight.Width
              && cursor_r.Y > BoundRight.Y && cursor_r.Y < BoundRight.Y + BoundRight.Height)
            {
                this.CursorPos.X = (int)cursor_r.X;
                this.CursorPos.Y = (int)cursor_r.Y;

                float delta = cursor_r.Y - BoundRight.Y;
                if (delta < BoundRight.Height / 4)
                    return MOUSE_ON_RIGHT_TOP;
                else if (delta > BoundRight.Height * 1 / 2)
                    return MOUSE_ON_RIGHT_BOTTOM;
                else
                    return MOUSE_ON_RIGHT_NEUTRAL;
            }

            return MOUSE_ON_NOTHING;
        }

        private bool DecreaseLeftOffset(int delta)
        {
            if (this.LeftOffset > 0)
            {
                this.LeftOffset -= delta;
                this.LeftOffset = this.LeftOffset < 0 ? 0 : this.LeftOffset;
                return true;
            }
            return false;
        }

        private bool IncreaseLeftOffset(int delta)
        {
            //int total_height = this.BoundLeft.Height;
            int total_height = 0;
            for (int i = 0; i < ButtonsLeft.Count; i++)
                total_height += ButtonsLeft[i].Bound.Height;

            total_height -= this.BoundLeft.Height;
            if (this.LeftOffset < total_height)
            {
                this.LeftOffset += delta;
                this.LeftOffset = this.LeftOffset > total_height ? total_height : this.LeftOffset;
                return true;
            }
            return false;
        }

        private bool DecreaseRightOffset(int delta)
        {
            if (this.RightOffset > 0)
            {
                this.RightOffset -= delta;
                this.RightOffset = this.RightOffset < 0 ? 0 : this.RightOffset;
                return true;
            }
            return false;
        }

        private bool IncreaseRightOffset(int delta)
        {
            int total_height = 0;
            for (int i = 0; i < ButtonsRight.Count; i++)
                total_height += ButtonsRight[i].Bound.Height;

            total_height -= this.BoundRight.Height;
            if (this.RightOffset < total_height)
            {
                this.RightOffset += delta;
                this.RightOffset = this.RightOffset > total_height ? total_height : this.RightOffset;
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            int mouse_pos = MouseHovering();
            switch (mouse_pos)
            {
                case MOUSE_ON_LEFT_TOP:
                    DecreaseLeftOffset(1);
                    break;
                case MOUSE_ON_LEFT_BOTTOM:
                    IncreaseLeftOffset(1);
                    break;
                case MOUSE_ON_RIGHT_TOP:
                    DecreaseRightOffset(1);
                    break;
                case MOUSE_ON_RIGHT_BOTTOM:
                    IncreaseRightOffset(1);
                    break;
            }
        }

        /// <summary>
        /// Gets the SpriteBatch from the services.
        /// </summary>
        public SpriteBatch SharedSpriteBatch
        {
            get
            {
                return (SpriteBatch)this.Game.Services.GetService(typeof(SpriteBatch));
            }
        }

        public void AddLeft(SimpleButton sb)
        {
            this.ButtonsLeft.Add(sb);
        }

        public void AddRight(SimpleButton sb)
        {
            this.ButtonsRight.Add(sb);
        }
    }
}
