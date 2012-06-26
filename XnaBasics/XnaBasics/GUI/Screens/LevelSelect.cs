//-----------------------------------------------
// XUI - LevelSelect.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace UI
{

    // class Screen_LevelSelect
    public class Screen_LevelSelect : Screen
    {
        // Screen_LevelSelect
        public Screen_LevelSelect()
            : base("LevelSelect")
        {
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

            Menu = new WidgetMenuScroll(E_MenuType.Horizontal);
            Menu.RenderPass = 1;
            Menu.Speed = 5.0f;
            Menu.Padding = 10.0f;
            Add(Menu);

            for (int i = 0; i < 1; ++i)
            {
                LevelNodes[i] = new List<WidgetBase>();

                WidgetMenuNode node = new WidgetMenuNode(i);
                node.RenderPass = 1;
                node.Size = new Vector3(40.0f, 40.0f * (9.0f / 16.0f), 0.0f);
                node.Align = E_Align.MiddleCentre;
                node.Alpha = 0.5f;
                node.Parent(Menu);
                Add(node);

                Timeline nodeT = new Timeline("selected", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start);
                nodeT.AddEffect(new TimelineEffect_Alpha(0.0f, 0.5f, E_LerpType.Linear));
                node.AddTimeline(nodeT);

                MenuNodes[i] = node;

                WidgetGraphic back = new WidgetGraphic();
                back.RenderPass = 1;
                back.Size = new Vector3(node.Size.X + 1.0f, node.Size.Y + 1.0f, 0.0f);
                back.Align = E_Align.MiddleCentre;
                back.AddTexture("null", 0.0f, 0.0f, 1.0f, 1.0f);
                back.Parent(node);
                back.ParentAttach = E_Align.MiddleCentre;
                back.ColorBase = Color.Red;
                back.Intensity = 0.5f;
                back.Alpha = 0.0f;
                Add(back);

                Timeline backT = new Timeline("selected", false, 0.0f, 0.5f, E_TimerType.Bounce, E_RestType.Start);
                backT.AddEffect(new TimelineEffect_Alpha(0.0f, 1.0f, E_LerpType.SmoothStep));
                back.AddTimeline(backT);

                int numRows = 4;
                int numCols = 4;

                for (int j = 0; j < numRows; ++j)
                {
                    for (int k = 0; k < numCols; ++k)
                    {
                        WidgetGraphic graphic = new WidgetGraphic();
                        graphic.RenderPass = 1;
                        graphic.Layer = 2;
                        graphic.Size = new Vector3(node.Size.X / numCols, node.Size.Y / numRows, 0.0f);

                        float x = node.Position.X - node.Size.X / 2.0f + graphic.Size.X / 2.0f;
                        float y = node.Position.Y + node.Size.Y / 2.0f - graphic.Size.Y / 2.0f;

                        graphic.Position = new Vector3(x + (graphic.Size.X * k), y - (graphic.Size.Y * j), 0.0f);
                        graphic.Align = E_Align.MiddleCentre;
                        graphic.AddTexture(TextureNames[i], (1.0f / numCols) * k, (1.0f / numRows) * j, 1.0f / numCols, 1.0f / numRows);
                        graphic.Parent(node);
                        graphic.ParentAttach = E_Align.MiddleCentre;
                        graphic.ColorBase = Color.White;

                        bool complete = ((i == 1) || (r.NextDouble() < 0.5f));

                        if (!complete)
                        {
                            graphic.RenderState.Effect = (int)E_Effect.GrayScale;
                            graphic.Intensity = 0.5f;
                        }

                        float gapX = 1.5f;
                        float gapY = 1.5f;

                        Vector3 posTo = new Vector3(x - gapX * (numCols - 1) / 2.0f + ((graphic.Size.X + gapX) * k) - graphic.Position.X, y + gapY * (numRows - 1) / 2.0f - ((graphic.Size.Y + gapY) * j) - graphic.Position.Y, 0.0f);

                        float halfX = (numCols * graphic.Size.X + (numCols - 1) * gapX) / 2.0f;
                        float rotY = (posTo.X / halfX) * -135.0f;

                        posTo.Z = graphic.Size.X * (float)Math.Tan(MathHelper.ToRadians(Math.Abs(rotY)));

                        Timeline graphicT = new Timeline("select_world", false, 0.0f, 0.5f, E_TimerType.Stop, E_RestType.Start);
                        graphicT.AddEffect(new TimelineEffect_PositionX(0.0f, posTo.X, E_LerpType.SmoothStep));
                        graphicT.AddEffect(new TimelineEffect_PositionY(0.0f, posTo.Y, E_LerpType.SmoothStep));
                        graphicT.AddEffect(new TimelineEffect_PositionZ(0.0f, posTo.Z, E_LerpType.SmoothStep));
                        graphicT.AddEffect(new TimelineEffect_Alpha(0.0f, 1.0f, E_LerpType.Linear));
                        graphicT.AddEffect(new TimelineEffect_RotationY(0.0f, rotY, E_LerpType.SmoothStep));
                        graphic.AddTimeline(graphicT);

                        Timeline graphicT2 = new Timeline("select_world", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start);
                        graphicT2.AddEffect(new TimelineEffect_Alpha(0.0f, 1.0f, E_LerpType.Linear));
                        graphic.AddTimeline(graphicT2);

                        WidgetGraphic backLevel = new WidgetGraphic();
                        backLevel.RenderPass = 1;
                        backLevel.Size = new Vector3(graphic.Size.X + 1.0f, graphic.Size.Y + 1.0f, 0.0f);
                        backLevel.Align = E_Align.MiddleCentre;
                        backLevel.AddTexture("null", 0.0f, 0.0f, 1.0f, 1.0f);
                        backLevel.Parent(graphic);
                        backLevel.ParentAttach = E_Align.MiddleCentre;
                        backLevel.ColorBase = Color.Red;
                        backLevel.Intensity = 0.5f;
                        backLevel.Alpha = 0.0f;
                        backLevel.FlagClear(E_WidgetFlag.InheritIntensity);
                        Add(backLevel);

                        Timeline backLevelT = new Timeline("select_level", false, 0.0f, 0.5f, E_TimerType.Bounce, E_RestType.Start);
                        backLevelT.AddEffect(new TimelineEffect_Alpha(0.0f, 1.0f, E_LerpType.SmoothStep));
                        backLevel.AddTimeline(backLevelT);

                        WidgetGraphic backG = (WidgetGraphic)graphic.Copy();
                        backG.Size += new Vector3(0.25f, 0.25f, 0.0f);
                        backG.ColorBase = Color.Black;
                        backG.Alpha = 0.0f;
                        backG.Layer = 1;
                        backG.Parent(node);
                        Add(backG);

                        Timeline backGT = new Timeline("select_world", false, 0.0f, 0.5f, E_TimerType.Stop, E_RestType.Start);
                        backGT.AddEffect(new TimelineEffect_Alpha(0.0f, 1.0f, E_LerpType.SmoothStep));
                        backG.AddTimeline(backGT);

                        Add(graphic);

                        if (complete)
                        {
                            WidgetGraphic tick = new WidgetGraphic();
                            tick.RenderPass = 1;
                            tick.Layer = 3;
                            tick.Position = new Vector3(1.0f, -1.0f, 0.0f);
                            tick.Size = new Vector3(3.0f, 3.0f, 0.0f);
                            tick.Align = E_Align.BottomRight;
                            tick.Parent(graphic);
                            tick.ParentAttach = E_Align.BottomRight;
                            tick.AddTexture("null", 0.0f, 0.0f, 1.0f, 1.0f);
                            tick.Alpha = 0.0f;
                            tick.ColorBase = Color.Green;
                            Add(tick);

                            Timeline tickT = new Timeline("select_world", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start);
                            tickT.AddEffect(new TimelineEffect_Alpha(0.0f, 1.0f, E_LerpType.SmoothStep));
                            tick.AddTimeline(tickT);
                        }

                        WidgetText text = new WidgetText();
                        text.RenderPass = 1;
                        text.Layer = 4;
                        text.Size = new Vector3(0.0f, 3.0f, 0.0f);
                        text.Align = E_Align.MiddleCentre;
                        text.FontStyleName = "Default3dDS";
                        text.String = "" + ((j * numCols) + k + 1);
                        text.Parent(graphic);
                        text.ParentAttach = E_Align.MiddleCentre;
                        text.ColorBase = Color.Yellow;
                        text.Alpha = 0.0f;
                        Add(text);

                        Timeline textT = new Timeline("select_world", false, 0.0f, 0.5f, E_TimerType.Stop, E_RestType.Start);
                        textT.AddEffect(new TimelineEffect_Alpha(0.0f, 1.0f, E_LerpType.SmoothStep));
                        text.AddTimeline(textT);

                        LevelNodes[i].Add(graphic);
                    }
                }

                if (i == 1)
                {
                    WidgetGraphic tickWorld = new WidgetGraphic();
                    tickWorld.RenderPass = 1;
                    tickWorld.Layer = 3;
                    tickWorld.Position = new Vector3(4.0f, -4.0f, 0.0f);
                    tickWorld.Size = new Vector3(12.0f, 12.0f, 0.0f);
                    tickWorld.Align = E_Align.BottomRight;
                    tickWorld.Parent(node);
                    tickWorld.ParentAttach = E_Align.BottomRight;
                    tickWorld.AddTexture("null", 0.0f, 0.0f, 1.0f, 1.0f);
                    tickWorld.ColorBase = Color.Green;
                    Add(tickWorld);

                    Timeline tickWorldT = new Timeline("select_world", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start);
                    tickWorldT.AddEffect(new TimelineEffect_Alpha(0.0f, -1.0f, E_LerpType.SmoothStep));
                    tickWorld.AddTimeline(tickWorldT);
                }

                // world name
                WidgetText textWorld = new WidgetText();
                textWorld.RenderPass = 1;
                textWorld.Layer = 4;
                textWorld.Position = new Vector3(0.0f, 1.5f, 0.0f);
                textWorld.Size = new Vector3(0.0f, 3.5f, 0.0f);
                textWorld.Align = E_Align.BottomCentre;
                textWorld.FontStyleName = "Default3dDS";
                textWorld.String = "WORLD " + (i + 1);
                textWorld.Parent(back);
                textWorld.ParentAttach = E_Align.TopCentre;
                textWorld.ColorBase = Color.Orange;
                textWorld.FlagClear(E_WidgetFlag.InheritAlpha);
                textWorld.FlagClear(E_WidgetFlag.InheritIntensity);
                Add(textWorld);

                // num levels complete
                WidgetText textLevelComplete = (WidgetText)textWorld.Copy();
                textLevelComplete.Position.Y *= -1.0f;
                textLevelComplete.Align = E_Align.TopCentre;
                textLevelComplete.Parent(back);
                textLevelComplete.ParentAttach = E_Align.BottomCentre;

                int count = 0;

                for (int j = 0; j < LevelNodes[i].Count; ++j)
                    if (LevelNodes[i][j].RenderState.Effect == (int)E_Effect.MultiTexture1)
                        ++count;

                textLevelComplete.String = count + " / " + LevelNodes[i].Count;
                Add(textLevelComplete);

                Timeline textWorldT = new Timeline("select_world", false, 0.0f, 0.5f, E_TimerType.Stop, E_RestType.Start);
                textWorldT.AddEffect(new TimelineEffect_PositionY(0.0f, 2.0f, E_LerpType.SmoothStep));
                textWorld.AddTimeline(textWorldT);

                Timeline textLevelCompleteT = new Timeline("select_world", false, 0.0f, 0.5f, E_TimerType.Stop, E_RestType.Start);
                textLevelCompleteT.AddEffect(new TimelineEffect_PositionY(0.0f, -2.0f, E_LerpType.SmoothStep));
                textLevelComplete.AddTimeline(textLevelCompleteT);
            }

            CurrentSelectionWorld = -1;
            CurrentSelectionLevel = 0;
            CurrentBackground = -1;

            Mode = 0; // world
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
            int menuSelected = Menu.GetByValue();

            if (menuSelected != CurrentSelectionWorld)
            {
                CurrentSelectionWorld = menuSelected;
                UpdateBackground();
            }
        }

        // UpdateBackground
        private void UpdateBackground()
        {
            if (CurrentBackground == -1)
            {
                CurrentBackground = 0;
                Backgrounds[CurrentBackground].ChangeTexture(0, TextureNames[CurrentSelectionWorld], 0.0f, 0.0f, 1.0f, 1.0f);
                Backgrounds[CurrentBackground].Selected(true, false, true);
            }
            else
            {
                Backgrounds[CurrentBackground].Selected(false, false, true);
                CurrentBackground = (CurrentBackground + 1) % 2;
                Backgrounds[CurrentBackground].ChangeTexture(0, TextureNames[CurrentSelectionWorld], 0.0f, 0.0f, 1.0f, 1.0f);
                Backgrounds[CurrentBackground].Selected(true, false, true);
            }
        }

        // OnProcessInput
        protected override void OnProcessInput(Input input)
        {
            if (Mode == 1)
            {
                float delay = _UI.AutoRepeatDelay;
                float repeat = _UI.AutoRepeatRepeat;

                int oldSelection = CurrentSelectionLevel;

                if (input.ButtonJustPressed((int)E_UiButton.A))
                {
                    _UI.Screen.SetNextScreen(new Screen_Loading());
                }
                else
                    if (input.ButtonAutoRepeat((int)E_UiButton.Up, delay, repeat))
                    {
                        if (CurrentSelectionLevel > 3)
                            CurrentSelectionLevel -= 4;
                    }
                    else
                        if (input.ButtonAutoRepeat((int)E_UiButton.Down, delay, repeat))
                        {
                            if (CurrentSelectionLevel < 12)
                                CurrentSelectionLevel += 4;
                        }
                        else
                            if (input.ButtonAutoRepeat((int)E_UiButton.Left, delay, repeat))
                            {
                                if ((CurrentSelectionLevel % 4) != 0)
                                    --CurrentSelectionLevel;
                            }
                            else
                                if (input.ButtonAutoRepeat((int)E_UiButton.Right, delay, repeat))
                                {
                                    if ((CurrentSelectionLevel % 4) != 3)
                                        ++CurrentSelectionLevel;
                                }

                if (oldSelection != CurrentSelectionLevel)
                {
                    LevelNodes[CurrentSelectionWorld][oldSelection].TimelineActive("select_level", false, true);
                    LevelNodes[CurrentSelectionWorld][CurrentSelectionLevel].TimelineActive("select_level", true, true);
                }
            }

            if (input.ButtonJustPressed((int)E_UiButton.A))
            {
                if (Mode == 0)
                {
                    Mode = 1;
                    Menu.Selected(false);
                    MenuNodes[CurrentSelectionWorld].TimelineActive("select_world", true, true);
                    LevelNodes[CurrentSelectionWorld][CurrentSelectionLevel].TimelineActive("select_level", true, true);
                }
            }
            else
                if (input.ButtonJustPressed((int)E_UiButton.B))
                {
                    if (Mode == 1)
                    {
                        Mode = 0;
                        Menu.Selected(true);
                        MenuNodes[CurrentSelectionWorld].TimelineActive("select_world", false, true);
                        LevelNodes[CurrentSelectionWorld][CurrentSelectionLevel].TimelineActive("select_level", false, true);
                    }
                    else
                    {
                        Backgrounds[CurrentBackground].Selected(false, false, true);
                        _G.UI.MM_FromLevelSelect = true;
                        _UI.Screen.SetNextScreen(new Screen_MainMenu());
                    }
                }
        }

        //
        private static string[] TextureNames = { "null", "null", "null", "null" };

        private WidgetGraphic[] Backgrounds = new WidgetGraphic[2];
        private int CurrentBackground;

        private WidgetMenuScroll Menu;
        private int CurrentSelectionWorld;
        private int CurrentSelectionLevel;

        private WidgetMenuNode[] MenuNodes = new WidgetMenuNode[4];
        private List<WidgetBase>[] LevelNodes = new List<WidgetBase>[4];

        private int Mode;
        //
    };

}; // namespace UI
