//-----------------------------------------------
// XUI - LevelSelect.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Samples.Kinect.XnaBasics;

namespace UI
{

    // class Screen_LevelSelect
    public class SideBar : Screen
    {
        // Screen_LevelSelect
        public SideBar(XnaBasics game)
            : base("SideBar")
        {
            this.XnaGame = game;

            WidgetGraphic black = new WidgetGraphic();
            black.Size = new Vector3(_UI.SX, _UI.SY, 0.0f);
            black.AddTexture("null", 0.0f, 0.0f, 1.0f, 1.0f);
            black.ColorBase = Color.Black;
            Add(black);

            Timeline blackT = new Timeline("start", true, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start);
            blackT.AddEffect(new TimelineEffect_Alpha(0.0f, -1.0f, E_LerpType.Linear));
            black.AddTimeline(blackT);

            Random r = new Random();

            WidgetGraphic background0 = new WidgetGraphic();
            background0.RenderPass = 2;
            background0.Position = new Vector3(_UI.SXM, _UI.SYM, 0.0f);
            background0.Size = new Vector3(_UI.SX, _UI.SY, 0.0f);
            background0.Align = E_Align.MiddleCentre;
            background0.ColorBase = Color.Yellow;
            background0.Intensity = 0.25f;
            background0.Alpha = 1.0f;
            background0.AddTexture("null", 0.0f, 0.0f, 1.0f, 1.0f);
            background0.RenderState.Effect = (int)E_Effect.GrayScale;
            //Add(background0);

            Timeline back0T = new Timeline("selected", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start);
            back0T.AddEffect(new TimelineEffect_Alpha(0.0f, 1.0f, E_LerpType.Linear));
            background0.AddTimeline(back0T);

            WidgetGraphic background1 = (WidgetGraphic)background0.Copy();
            //Add(background1);

            Backgrounds[0] = background0;
            Backgrounds[1] = background1;

            RightBar = new WidgetMenuScroll(E_MenuType.Vertical);
            RightBar.RenderPass = 1;
            RightBar.Speed = 5.0f;
            RightBar.Padding = 2.0f;
            Add(RightBar);
            
            LeftBar = new WidgetMenuScroll(E_MenuType.Vertical);
            LeftBar.RenderPass = 1;
            LeftBar.Speed = 5.0f;
            LeftBar.Padding = 2.0f;
            Add(LeftBar);

            for (int i = 0; i < 4; ++i)
            {
                WidgetMenuNode node = new WidgetMenuNode(i);
                node.RenderPass = 1;
                node.Size = new Vector3(20.0f, 20.0f * (9.0f / 16.0f), 0.0f);
                node.Align = E_Align.MiddleLeft;
                node.Alpha = 0.5f;
                node.Parent(RightBar);
                Add(node);

                Timeline nodeT = new Timeline("selected", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start);
                nodeT.AddEffect(new TimelineEffect_Alpha(0.0f, 0.5f, E_LerpType.Linear));
                node.AddTimeline(nodeT);

                RightNodes[i] = node;

                WidgetGraphic back = new WidgetGraphic();
                back.RenderPass = 1;
                back.Size = new Vector3(node.Size.X + 1.0f, node.Size.Y + 1.0f, 0.0f);
                back.Align = E_Align.MiddleLeft;
                back.AddTexture("tex"+i, 0.0f, 0.0f, 1.0f, 1.0f);
                back.Parent(node);
                back.ParentAttach = E_Align.MiddleCentre;
                back.ColorBase = Color.AntiqueWhite;
                back.Intensity = 0.5f;
                back.Alpha = 0.0f;
                Add(back);

                Timeline backT = new Timeline("selected", false, 0.0f, 0.5f, E_TimerType.Bounce, E_RestType.Start);
                backT.AddEffect(new TimelineEffect_Alpha(0.5f, 0.8f, E_LerpType.SmoothStep));
                back.AddTimeline(backT);




                WidgetMenuNode node2 = new WidgetMenuNode(i);
                node2.RenderPass = 1;
                node2.Size = new Vector3(20.0f, 20.0f * (9.0f / 16.0f), 0.0f);
                node2.Align = E_Align.MiddleRight;
                node2.Alpha = 0.5f;
                node2.Parent(LeftBar);
                Add(node2);

                Timeline nodeT2 = new Timeline("selected", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start);
                nodeT2.AddEffect(new TimelineEffect_Alpha(0.0f, 0.5f, E_LerpType.Linear));
                node2.AddTimeline(nodeT2);

                LeftNodes[i] = node2;

                WidgetGraphic back2 = new WidgetGraphic();
                back2.RenderPass = 1;
                back2.Size = new Vector3(node2.Size.X + 1.0f, node2.Size.Y + 1.0f, 0.0f);
                back2.Align = E_Align.MiddleRight;
                back2.AddTexture("uvs", 0.0f, 0.0f, 1.0f, 1.0f);
                back2.Parent(node2);
                back2.ParentAttach = E_Align.MiddleCentre;
                back2.ColorBase = Color.Red;
                back2.Intensity = 0.5f;
                back2.Alpha = 0.0f;
                Add(back2);

                Timeline backT2 = new Timeline("selected", false, 0.0f, 0.5f, E_TimerType.Bounce, E_RestType.Start);
                backT2.AddEffect(new TimelineEffect_Alpha(0.5f, 0.8f, E_LerpType.SmoothStep));
                back2.AddTimeline(backT2);
            }

            CurrentSelectionRight = -1;
            CurrentSelectionLeft = -1;
            CurrentBackground = -1;
        }

        // OnInit
        protected override void OnInit()
        {
            SetScreenTimers(0.25f, 0.25f);
        }

        // OnStartLoop
        protected override void OnStartLoop(float frameTime)
        {
            OnUpdate(frameTime);
        }

        // OnUpdate
        protected override void OnUpdate(float frameTime)
        {
            int menuSelected = RightBar.GetByValue();
            if (menuSelected != CurrentSelectionRight)
            {
                CurrentSelectionRight = menuSelected;
                if (this.XnaGame != null) ;
                    
                UpdateBackground();
            }
            menuSelected = LeftBar.GetByValue();
            if (menuSelected != CurrentSelectionLeft)
            {
                CurrentSelectionLeft = menuSelected;
                UpdateBackground();
            }
        }

        // UpdateBackground
        private void UpdateBackground()
        {
            if (CurrentBackground == -1)
            {
                CurrentBackground = 0;
                Backgrounds[CurrentBackground].ChangeTexture(0, TextureNames[0], 0.0f, 0.0f, 1.0f, 1.0f);
                Backgrounds[CurrentBackground].Selected(true, false, true);
            }
            else
            {
                Backgrounds[CurrentBackground].Selected(false, false, true);
                CurrentBackground = (CurrentBackground + 1) % 2;
                Backgrounds[CurrentBackground].ChangeTexture(0, TextureNames[0], 0.0f, 0.0f, 1.0f, 1.0f);
                Backgrounds[CurrentBackground].Selected(true, false, true);
            }
        }

        // OnProcessInput
        protected override void OnProcessInput(Input input)
        {
            if (input.ButtonJustPressed((int)E_UiButton.Down))
            {
                LeftBar.IncreaseCurrent();
            }
            else if (input.ButtonJustPressed((int)E_UiButton.Up))
            {
                LeftBar.DecreaseCurrent();
            }
            else if (input.ButtonJustPressed((int)E_UiButton.B))
            {
                RightBar.DecreaseCurrent();
            }
            else if (input.ButtonJustPressed((int)E_UiButton.A))
            {
                RightBar.IncreaseCurrent();
            }

        }

        //
        private static string[] TextureNames = { "null", "null", "null", "null" };

        private WidgetGraphic[] Backgrounds = new WidgetGraphic[2];
        private int CurrentBackground;

        private WidgetMenuScroll RightBar;
        private WidgetMenuScroll LeftBar;
        private int CurrentSelectionRight, CurrentSelectionLeft;

        private WidgetMenuNode[] RightNodes = new WidgetMenuNode[4];
        private WidgetMenuNode[] LeftNodes = new WidgetMenuNode[4];
        private XnaBasics XnaGame;
    };

}; // namespace UI
