//-----------------------------------------------
// XUI - Start.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

using Microsoft.Xna.Framework;

namespace UI
{

    // class Screen_Start
    public class Screen_Start : Screen
    {
        // Screen_Start
        public Screen_Start()
            : base("Start")
        {
            WidgetGraphic logo = new WidgetGraphic();
            logo.Position = new Vector3(_UI.SXM, _UI.SYM, 0.0f);
            logo.Size = new Vector3(_UI.SY / 3.0f, _UI.SY / 3.0f, 0.0f);
            logo.Align = E_Align.BottomCentre;
            logo.ColorBase = new SpriteColors(Color.Orange, Color.Orange, Color.Black, Color.Black);
            logo.AddTexture("null", 0.0f, 0.0f, 1.0f, 1.0f);
            Add(logo);

            if (!_G.UI.SS_FromMainMenu)
            {
                Timeline logoT = new Timeline("start", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.None);
                logoT.AddEffect(new TimelineEffect_Alpha(-1.0f, 0.0f, E_LerpType.SmoothStep));
                logo.AddTimeline(logoT);
            }

            _G.UI.SS_FromMainMenu = false;

            WidgetText text = new WidgetText();
            text.Position = new Vector3(_UI.SXM, _UI.SYM + 150.0f, 0.0f);
            text.Size = new Vector3(0.0f, 60.0f, 0.0f);
            text.Align = E_Align.MiddleCentre;
            text.FontStyle = _UI.Store_FontStyle.Get("Default").Copy();
            text.FontStyle.TrackingPercentage = 0.1875f;
            text.String = "PRESS START BUTTON";
            text.ColorBase = Color.Orange;
            text.AddFontEffect(new FontEffect_ColorLerp(0.03125f, 1.5f, 3.0f, Color.White, E_LerpType.BounceOnceSmooth));
            text.AddFontEffect(new FontEffect_Scale(0.03125f, 0.75f, 3.0f, 1.0f, 1.5f, 1.0f, 2.0f, E_LerpType.BounceOnceSmooth));
            Add(text);

            Timeline textT = new Timeline("start", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start);
            textT.AddEffect(new TimelineEffect_Alpha(-1.0f, 0.0f, E_LerpType.SmoothStep));
            text.AddTimeline(textT);
        }

        // OnInit
        protected override void OnInit()
        {
            SetScreenTimers(0.25f, 0.25f);
            _UI.PrimaryPad = -1;
        }

        // OnProcessInput
        protected override void OnProcessInput(Input input)
        {
            if (input.ButtonJustPressed((int)E_UiButton.Start) || input.ButtonJustPressed((int)E_UiButton.A))
            {
                _G.UI.MM_FromStartScreen = true;

                _UI.PrimaryPad = input.Controller.PadIndex;
                _UI.Screen.SetNextScreen(new Screen_MainMenu());
            }
        }
    };

}; // namespace UI
