//-----------------------------------------------
// XUI - LevelSelect.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Samples.Kinect.XnaBasics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Samples.Kinect.XnaBasics.GUI;

namespace UI
{

    // class Screen_LevelSelect
    public class SideBar : Screen
    {
        // Screen_LevelSelect
        public SideBar()
            : base("SideBar")
        {
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

            _DictNodeToTextureIndex = new Dictionary<int,int>();

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
                graphic.AddTexture("ClothButtonTex"+i, 0.0f, 0.0f, 1.0f, 1.0f);
                graphic.Name = "ClothButtonTex" + i;
                graphic.Parent(node);
                graphic.ParentAttach = E_Align.MiddleRight;
                graphic.ColorBase = Color.White;
                
                Add(graphic);

                _DictNodeToTextureIndex.Add( i,_UI.Texture.Get("ClothTexture" + i) );
                
                
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
                back2.Size = new Vector3(node2.Size.X, node2.Size.Y, 0.0f);
                back2.Align = E_Align.MiddleCentre;
                back2.AddTexture("null", 0.0f, 0.0f, 1.0f, 1.0f);
                back2.Parent(node2);
                back2.ParentAttach = E_Align.MiddleLeft;
                back2.ColorBase = Color.Tomato;
                back2.Intensity = 0.5f;
                back2.Alpha = 0.0f;
                Add(back2);
                Timeline backT2 = new Timeline("selected", false, 0.0f, 0.5f, E_TimerType.Bounce, E_RestType.Start);
                backT2.AddEffect(new TimelineEffect_Alpha(0.2f, 0.9f, E_LerpType.SmoothStep));
                back2.AddTimeline(backT2);

                WidgetGraphic graphic2 = new WidgetGraphic();
                graphic2.RenderPass = 1;
                graphic2.Layer = 2;
                graphic2.Size = new Vector3(node.Size.X - 1.0f, node.Size.Y - 1.0f, 0.0f);
                graphic2.Align = E_Align.MiddleCentre;
                graphic2.AddTexture("BagButtonTex" + i, 0.0f, 0.0f, 1.0f, 1.0f);
                graphic2.Parent(node2);
                graphic2.ParentAttach = E_Align.MiddleLeft;
                graphic2.ColorBase = Color.White;
                Add(graphic2);
            }

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
                //int slot = _UI.Texture.Get("ClothButtonTex" + CurrentSelectionRight);
                //ObjectRender.TargetTexture = _UI.Texture.Get(slot);
                    
            }
            menuSelected = LeftBar.GetByValue();
            if (menuSelected != CurrentSelectionLeft)
            {
                CurrentSelectionLeft = menuSelected;
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

        private WidgetMenuScroll RightBar;
        private WidgetMenuScroll LeftBar;
        private int CurrentSelectionRight, CurrentSelectionLeft;

        private WidgetMenuNode[] RightNodes = new WidgetMenuNode[4];
        private WidgetMenuNode[] LeftNodes = new WidgetMenuNode[4];

        public Object3D ObjectRender
        {
            get;
            set;
        }

        Dictionary<int, int> _DictNodeToTextureIndex;
    };

}; // namespace UI
