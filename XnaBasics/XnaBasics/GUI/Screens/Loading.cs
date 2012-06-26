//-----------------------------------------------
// XUI - Loading.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

using Microsoft.Xna.Framework;

namespace UI
{

// class Screen_Loading
public class Screen_Loading : Screen
{
	// Screen_Loading
	public Screen_Loading()
		: base( "Loading" )
	{
		int numIcons = 10;
		float gap = -40.0f;

		for ( int i = 0; i < numIcons; ++i )
		{
			float rot = ( MathHelper.TwoPi / numIcons ) * i;
			Matrix m = Matrix.CreateRotationZ( rot );

			WidgetGraphic icon = new WidgetGraphic();
			icon.Position = new Vector3( 0.0f, gap, 0.0f );
			Vector3.Transform( ref icon.Position, ref m, out icon.Position );
			icon.Position += new Vector3( _UI.SXM, _UI.SYM - 50.0f, 0.0f );
			icon.Size = new Vector3( 12.0f, 12.0f, 0.0f );
			icon.Align = E_Align.MiddleCentre;
			icon.ColorBase = Color.Orange;
			icon.AddTexture( "null", 0.0f, 0.0f, 1.0f, 1.0f );
			icon.Rotation.Z = MathHelper.ToDegrees( rot );
			Add( icon );

			Timeline iconT = new Timeline( "", true, ( 1.0f / numIcons ) * i, 0.5f, E_TimerType.Bounce, E_RestType.None );
			iconT.AddEffect( new TimelineEffect_Alpha( 0.0f, -1.0f, E_LerpType.SmoothStep ) );
			iconT.AddEffect( new TimelineEffect_ScaleX( 0.0f, 0.5f, E_LerpType.SmoothStep ) );
			iconT.AddEffect( new TimelineEffect_ScaleY( 0.0f, 0.5f, E_LerpType.SmoothStep ) );
			icon.AddTimeline( iconT );

			Timeline iconT2 = new Timeline( "start", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start );
			iconT2.AddEffect( new TimelineEffect_Alpha( -1.0f, 0.0f, E_LerpType.SmoothStep ) );
			icon.AddTimeline( iconT2 );
		}

		WidgetText text = new WidgetText();
		text.Position = new Vector3( _UI.SXM, _UI.SYM + 25.0f, 0.0f );
		text.Size = new Vector3( 0.0f, 60.0f, 0.0f );
		text.Align = E_Align.TopCentre;
		text.FontStyle = _UI.Store_FontStyle.Get( "Default" ).Copy();
		text.FontStyle.TrackingPercentage = 0.1875f;
		text.String = "LOADING";
		text.ColorBase = Color.Orange;
		text.AddFontEffect( new FontEffect_ColorLerp( 0.03125f, 1.5f, 3.0f, Color.White, E_LerpType.BounceOnceSmooth ) );
		text.AddFontEffect( new FontEffect_Scale( 0.03125f, 0.75f, 3.0f, 1.0f, 1.5f, 1.0f, 2.0f, E_LerpType.BounceOnceSmooth ) );
		Add( text );

		Timeline textT2 = new Timeline( "start", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start );
		textT2.AddEffect( new TimelineEffect_Alpha( -1.0f, 0.0f, E_LerpType.SmoothStep ) );
		text.AddTimeline( textT2 );
	}

	// OnInit
	protected override void OnInit()
	{
		SetScreenTimers( 0.25f, 0.25f );
	}

	// OnUpdate
	protected override void OnUpdate( float frameTime )
	{
		// wait until load is finished etc ...
	}
};

}; // namespace UI
