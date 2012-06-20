using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ModelViewer
{
    
    public class ViewModeLabel
    {
        private static ViewModeLabel instance;
        public static ViewModeLabel Instance
        {
            get
            {
                if (instance == null) instance = new ViewModeLabel();
                return instance;
            }
        }

        ProjectionType projectionType = ProjectionType.Perspective;
        ShadingType shadingType = ShadingType.Solid;

        string label = "[ Perspective ] [ Solid ]";
        SpriteFont font;

        public ProjectionType ProjectionType
        {
            get { return projectionType; }
            set
            {
                projectionType = value;
                BuildLabel();
            }
        }

        public ShadingType ShadingType
        {
            get { return shadingType; }
            set
            {
                shadingType = value;
                BuildLabel();
            }
        }

        private ViewModeLabel()
        {
        }

        public void Initialize(ContentManager Content)
        {
            this.font = Content.Load<SpriteFont>(@"Fonts\Arial10Font");
        }

        private void BuildLabel()
        {
            label = string.Format("[ {0} ] [ {1} ]", projectionType.ToString(), shadingType.ToString());
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Begin();
            batch.DrawString(font, label, new Vector2(10, 10), Color.White);
            batch.End();
        }
    }
}
