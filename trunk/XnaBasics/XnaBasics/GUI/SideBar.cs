//-----------------------------------------------
// XUI - LevelSelect.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Samples.Kinect.XnaBasics;
using Microsoft.Xna.Framework.Graphics;

namespace UI
{

    // class Screen_LevelSelect
    public class SideBar : Screen
    {
        private WidgetMenuScroll CreateSideBar()
        {
            WidgetMenuScroll sideBar = new WidgetMenuScroll(E_MenuType.Vertical);
            sideBar.RenderPass = 1;
            sideBar.Speed = 5.0f;
            sideBar.Padding = 2.0f;
            Add(sideBar);
            //sideBar.ParentAttach = E_Align.MiddleLeft;
            return sideBar;
        }

        private void FillSideBar(WidgetMenuScroll sideBar, string[] texNameArray, Vector3 size)
        {
            for (int i = 0; i < texNameArray.Length; ++i)
            {
                WidgetMenuNode node = new WidgetMenuNode(i);
                node.RenderPass = 1;
                node.Size = size;
                node.Align = E_Align.MiddleCentre;
                node.Alpha = 0.5f;
                node.Parent(sideBar);
                Add(node);
                Timeline nodeT = new Timeline("selected", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start);
                nodeT.AddEffect(new TimelineEffect_Alpha(0.0f, 0.5f, E_LerpType.Linear));
                node.AddTimeline(nodeT);

                WidgetGraphic back = new WidgetGraphic();
                back.RenderPass = 1;
                back.Size = new Vector3(node.Size.X, node.Size.Y, 0.0f);
                back.Align = E_Align.MiddleCentre;
                back.AddTexture("null", 0.0f, 0.0f, 1.0f, 1.0f);
                back.Parent(node);
                back.ParentAttach = E_Align.MiddleRight;
                back.ColorBase = Color.Yellow;
                back.Intensity = 0.5f;
                back.Alpha = 0.0f;
                Add(back);
                Timeline backT = new Timeline("selected", false, 0.0f, 0.5f, E_TimerType.Bounce, E_RestType.Start);
                backT.AddEffect(new TimelineEffect_Alpha(0.2f, 0.9f, E_LerpType.SmoothStep));
                back.AddTimeline(backT);

                WidgetGraphic graphic = new WidgetGraphic();
                graphic.RenderPass = 1;
                graphic.Layer = 2;
                graphic.Size = new Vector3(node.Size.X - 1.0f, node.Size.Y - 1.0f, 0.0f);
                graphic.Align = E_Align.MiddleCentre;
                graphic.AddTexture(texNameArray[i], 0.0f, 0.0f, 1.0f, 1.0f);
                graphic.Name = texNameArray[i];
                graphic.Parent(node);
                graphic.ParentAttach = E_Align.MiddleRight;
                graphic.ColorBase = Color.White;

                Add(graphic);
            }
        }
        // Screen_LevelSelect
        public SideBar()
            : base("SideBar")
        {
            RightBar = CreateSideBar();
            RightBar.Position = new Vector3(21f, 0f, 0f);
            LeftBar = CreateSideBar();
            LeftBar.Position = new Vector3(-36f, 0f, 0f);
            
 
            _DictNodeToTextureIndex = new Dictionary<int,int>();
            string[] texNameArray = new string[4]
            {
                "ClothButtonTex0","ClothButtonTex1","ClothButtonTex2","ClothButtonTex3"
            };
            Vector3 size = new Vector3(15.0f, 15.0f, 0f);
            FillSideBar(RightBar, texNameArray, size);
            for (int i = 0; i < texNameArray.Length;i++ )
                _DictNodeToTextureIndex.Add(i, _UI.Texture.Get(texNameArray[i]));
            FillSideBar(LeftBar, texNameArray, size);

            CurrentSelectionRight = -1;
            CurrentSelectionLeft = -1;
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
                int texIndex = 0;
                _DictNodeToTextureIndex.TryGetValue(CurrentSelectionRight, out texIndex);
                ObjectRender.TargetTexture = _UI.Texture.Get(texIndex);
                    
            }
            if (LeftBar.ChildrenWidget.Count > 0)
            {
                menuSelected = LeftBar.GetByValue();
                if (menuSelected != CurrentSelectionLeft)
                {
                    CurrentSelectionLeft = menuSelected;
                }
            }

            if (GestureTracker.trend == Trend.Up)
            {
                if (GestureTracker.counter >= 3)
                {
                    RightBar.IncreaseCurrent();
                    GestureTracker.Reset();
                }
            }
            else if (GestureTracker.trend == Trend.Down)
            {
                if (GestureTracker.counter >= 2)
                {
                    RightBar.DecreaseCurrent();
                    GestureTracker.Reset();
                }
            }

        }

        // OnProcessInput
        protected override void OnProcessInput(Input input)
        {
            if (LeftBar.ChildrenWidget.Count > 0)
            {
                if (input.ButtonJustPressed((int)E_UiButton.Down))
                {
                    LeftBar.IncreaseCurrent();
                }
                else if (input.ButtonJustPressed((int)E_UiButton.Up))
                {
                    LeftBar.DecreaseCurrent();
                }

            }
            if (RightBar.ChildrenWidget.Count > 0)
            {
                if (input.ButtonJustPressed((int)E_UiButton.Back))
                {
                    RightBar.DecreaseCurrent();
                }
                else if (input.ButtonJustPressed((int)E_UiButton.Enter))
                {
                    RightBar.IncreaseCurrent();
                }
            }
        }

        private WidgetMenuScroll RightBar;
        private WidgetMenuScroll LeftBar;
        private int CurrentSelectionRight, CurrentSelectionLeft;

        private WidgetMenuNode[] LeftNodes = new WidgetMenuNode[4];

        public Object3D ObjectRender
        {
            get;
            set;
        }

        Dictionary<int, int> _DictNodeToTextureIndex;
    };

}; // namespace UI
