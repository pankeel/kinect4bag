//-----------------------------------------------
// XUI - FPS.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

using Microsoft.Xna.Framework;

namespace UI
{

// class WidgetFPS
public class WidgetFPS : WidgetBase
{
#if !RELEASE
	static Debug.BoolD d_ShowFPS = new Debug.BoolD( "Profile.ShowFPS", true );

	// WidgetFPS
	public WidgetFPS()
		: base()
	{
		RenderPass = 0;
		Layer = _UI.Sprite.TopLayer;

		Position = new Vector3( _UI.SXR, _UI.SYT, 0.0f );
		Size = new Vector3( 30.0f );

		AddTexture( "null", 0.0f, 0.0f, 1.0f, 1.0f );

		FontStyle = _UI.Store_FontStyle.Get( "Default" );

		StringUI = new StringUI( 8 );

		FrameRate = 0;
		FrameCounter = 0;
		ElapsedTime = 0.0f;
	}

	// CopyTo
	protected override void CopyTo( WidgetBase o )
	{
		base.CopyTo( o );

		WidgetFPS oo = (WidgetFPS)o;

		oo.FontStyle = FontStyle.Copy();
		oo.StringUI.Add( StringUI );

		oo.FrameRate = FrameRate;
		oo.FrameCounter = FrameCounter;
		oo.ElapsedTime = ElapsedTime;
	}

	// OnUpdate
	protected override void OnUpdate( float frameTime )
	{
		ElapsedTime += frameTime;

		if ( ElapsedTime > 1.0f )
		{
			ElapsedTime -= 1.0f;
			FrameRate = FrameCounter;
			FrameCounter = 0;
		}
	}

	// OnRender
	protected override void OnRender()
	{
		++FrameCounter;

		if ( !d_ShowFPS )
			return;
		
		UI.SpriteColors c = _G.Game.IsRunningSlowly ? Color.Red : Color.Green;
		c.A( 224 );

		_UI.Sprite.AddSprite( RenderPass, Layer, ref Position, Size.X, Size.Y, UI.E_Align.TopRight, ref c, ref RenderState );
		_UI.Sprite.AddTexture( 0, Textures[ 0 ].TextureIndex );

		Vector3 p = new Vector3( Position.X - ( Size.Y / 2.0f ), Position.Y + ( Size.Y / 2.0f ), Position.Z );
		UI.SpriteColors c2 = Color.Yellow;

		StringUI.Clear();
		StringUI.Add( FrameRate );

		_UI.Font.Draw( StringUI, FontStyle, RenderPass, Layer, ref p, Size.Y * 0.65f, UI.E_Align.MiddleCentre, ref c2, 0.0f );
	}

	//
	private FontStyle		FontStyle;
	private StringUI		StringUI;

	private int				FrameRate;
	private int				FrameCounter;
	private float			ElapsedTime;
	//
#endif
};

}; // namespace UI
