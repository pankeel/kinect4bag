//-----------------------------------------------
// XUI - Layer.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Samples.Kinect.XnaBasics;

// E_UiMessageType
public enum E_UiMessageType
{
	PopupConfirm = 0,
	PopupCancel,

	Count,
};

// class UiLayer
public class UiLayer : Layer
{
	// UiLayer
	public UiLayer(XnaBasics game)
		: base( (int)E_Layer.UI )
	{
        this.XnaGame = game;
	}

	// Startup
	public override void Startup( ContentManager content )
	{
		_UI.Startup( _G.Game, _G.GameInput );

		// load textures
		int bundleIndex = _UI.Texture.CreateBundle();

		_UI.Texture.Add( bundleIndex, "Textures\\UI_Box", "box" );
        for (int i = 0; i < 4; i++)
        {
            _UI.Texture.AddDynamic(bundleIndex, "bag", "Textures\\tex" + i, "BagButtonTex" + i);
            _UI.Texture.AddDynamic(bundleIndex, "yifu", "Textures\\tex" + i, "ClothButtonTex"+i);
        }

        //Add Dynamic Texture for Bag Model
        //_UI.Texture.AddDynamic(bundleIndex, "yifu", "Textures\\tex0", "BagDisplayUnit");
		// load fonts
		_UI.Font.Add( "Fonts\\", "SegoeUI" );

		// setup common font styles
		UI.FontStyle fontStyle = new UI.FontStyle( "SegoeUI" );
		fontStyle.AddRenderPass( new UI.FontStyleRenderPass() );
		_UI.Store_FontStyle.Add( "Default", fontStyle );

		UI.FontStyle fontStyleDS = new UI.FontStyle( "SegoeUI" );
		UI.FontStyleRenderPass renderPassDS = new UI.FontStyleRenderPass();
		renderPassDS.ColorOverride = Color.Black;
		renderPassDS.AlphaMult = 0.5f;
		renderPassDS.Offset = new Vector3( 0.05f, -0.05f, 0.0f );
		renderPassDS.OffsetProportional = true;
		fontStyleDS.AddRenderPass( renderPassDS );
		fontStyleDS.AddRenderPass( new UI.FontStyleRenderPass() );
		_UI.Store_FontStyle.Add( "Default3dDS", fontStyleDS );

		// setup font icons
		_UI.Store_FontIcon.Add( "A", new UI.FontIcon( _UI.Texture.Get( "null" ), 0.0f, 0.0f, 1.0f, 1.0f ) );
		_UI.Store_FontIcon.Add( "B", new UI.FontIcon( _UI.Texture.Get( "null" ), 0.0f, 0.0f, 1.0f, 1.0f ) );

		// add initial screens
		//_UI.Screen.AddScreen( new UI.Screen_Background() );
		//_UI.Screen.AddScreen( new UI.Screen_LevelSelect() );
        this.SideBars = new UI.SideBar(XnaGame);
        _UI.Screen.AddScreen(SideBars);
	}

	// Shutdown
	public override void Shutdown()
	{
		_UI.Shutdown();
	}

	// OnUpdate
	protected override void OnUpdate( float frameTime )
	{
	#if !RELEASE
		if ( _UI.DebugMenuActive )
			return;
	#endif

	#if !RELEASE
		_UI.Camera2D.DebugUpdate( frameTime, _UI.GameInput.GetInput( 0 ) );
		_UI.Camera3D.DebugUpdate( frameTime, _UI.GameInput.GetInput( 0 ) );
	#endif

		_UI.Sprite.RenderPassTransformMatrix[ 0 ] = _UI.Camera2D.TransformMatrix; // 2d
		_UI.Sprite.RenderPassTransformMatrix[ 1 ] = _UI.Camera3D.TransformMatrix; // 3d
		_UI.Sprite.RenderPassTransformMatrix[ 2 ] = _UI.Camera2D.TransformMatrix; // 2d background

		_UI.Sprite.BeginUpdate();
		_UI.Screen.Update( frameTime );
	}

	// OnRender
	protected override void OnRender( float frameTime )
	{
		_UI.Sprite.Render( 2 ); // 2d background
		_UI.Sprite.Render( 1 ); // 3d
		_UI.Sprite.Render( 0 ); // 2d
	}

	//
	public bool		MM_FromStartScreen;
	public bool		MM_FromLevelSelect;
	public bool		SS_FromMainMenu;
    public UI.SideBar SideBars;
    private XnaBasics XnaGame;
	//
};
