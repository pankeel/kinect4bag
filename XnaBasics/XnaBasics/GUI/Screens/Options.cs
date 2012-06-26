//-----------------------------------------------
// XUI - Options.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

using Microsoft.Xna.Framework;

namespace UI
{

// class Screen_Options
public class Screen_Options : Screen
{
	// Screen_Options
	public Screen_Options()
		: base( "Options" )
	{
		WidgetGraphic logo = new WidgetGraphic();
		logo.Position = new Vector3( _UI.SXM, _UI.SYM - 65.0f, 0.0f );
		logo.Size = new Vector3( _UI.SY / 3.0f, _UI.SY / 3.0f, 0.0f );
		logo.Align = E_Align.BottomCentre;
		logo.ColorBase = new SpriteColors( Color.Orange, Color.Orange, Color.Black, Color.Black );
		logo.AddTexture( "null", 0.0f, 0.0f, 1.0f, 1.0f );
		Add( logo );

		WidgetMenuScroll menu = new WidgetMenuScroll( E_MenuType.Vertical );
		menu.Position = new Vector3( _UI.SXM, _UI.SYM + 25.0f + 37.5f, 0.0f );
		menu.Padding = 75.0f;
		menu.Alpha = 0.0f;

		Timeline menuT = new Timeline( "start", false, 0.25f, 0.25f, E_TimerType.Stop, E_RestType.None );
		menuT.AddEffect( new TimelineEffect_Alpha( 0.0f, 1.0f, E_LerpType.SmoothStep ) );
		menu.AddTimeline( menuT );

		Timeline menuT2 = new Timeline( "end", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.None );
		menuT2.AddEffect( new TimelineEffect_Alpha( 0.0f, -1.0f, E_LerpType.SmoothStep ) );
		menu.AddTimeline( menuT2 );

		Add( menu );

		// music volume
		WidgetMenuNode node0 = new WidgetMenuNode( 0 );
		Add( node0 );
		node0.Parent( menu );

		Timeline nodeT = new Timeline( "selected", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start );
		nodeT.AddEffect( new TimelineEffect_ScaleX( 0.0f, 0.125f, E_LerpType.SmoothStep ) );
		nodeT.AddEffect( new TimelineEffect_ScaleY( 0.0f, 0.125f, E_LerpType.SmoothStep ) );
			
		Timeline nodeT2 = new Timeline( "selected", false, 0.0f, 0.5f, E_TimerType.Bounce, E_RestType.Start );
		nodeT2.AddEffect( new TimelineEffect_Intensity( 0.0f, 0.75f, E_LerpType.SmoothStep ) );

		node0.AddTimeline( nodeT );
		node0.AddTimeline( nodeT2 );

		WidgetText text0 = new WidgetText();
		text0.Size = new Vector3( 0.0f, 50.0f, 0.0f );
		text0.Align = E_Align.MiddleCentre;
		text0.FontStyleName = "Default";
		text0.String = "MUSIC VOLUME";
		text0.Align = E_Align.MiddleRight;
		text0.Parent( node0 );
		text0.ParentAttach = E_Align.MiddleCentre;
		text0.ColorBase = Color.Orange;
		Add( text0 );

		WidgetMenuSwitch menuSwitch0 = new WidgetMenuSwitch( E_MenuType.Horizontal );
		menuSwitch0.Position = new Vector3( 80.0f, 0.0f, 0.0f );
		menuSwitch0.Parent( node0 );
		menuSwitch0.ParentAttach = E_Align.MiddleCentre;
		menuSwitch0.DeactivateArrows = false;
		Add( menuSwitch0 );

		Timeline timelineArrow_Selected = new Timeline( "selected", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start );
		timelineArrow_Selected.AddEffect( new TimelineEffect_Alpha( 0.0f, 1.0f, E_LerpType.SmoothStep ) );

		Timeline timelineArrow_Nudge = new Timeline( "nudge", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.None );
		timelineArrow_Nudge.AddEffect( new TimelineEffect_ScaleX( 0.0f, 0.25f, E_LerpType.BounceOnceSmooth ) );
		timelineArrow_Nudge.AddEffect( new TimelineEffect_ScaleY( 0.0f, 0.25f, E_LerpType.BounceOnceSmooth ) );

		WidgetGraphic arrow = new WidgetGraphic();
		arrow.Size = new Vector3( 15.0f, 30.0f, 0.0f );
		arrow.ColorBase = Color.Orange;
		arrow.Alpha = 0.0f;
		arrow.Align = E_Align.MiddleCentre;
		arrow.RenderState.Effect = (int)E_Effect.IntensityAsAlpha_PMA;
		arrow.FlagClear( E_WidgetFlag.InheritIntensity );
		arrow.AddTimeline( timelineArrow_Selected.Copy() );
		arrow.AddTimeline( timelineArrow_Nudge.Copy() );
		arrow.ParentAttach = E_Align.MiddleCentre;
		
		WidgetGraphic arrowLeft = (WidgetGraphic)arrow.Copy();
		arrowLeft.Name = "arrow_decrease";
		arrowLeft.Position = new Vector3( -50.0f, 0.0f, 0.0f );
		arrowLeft.Rotation.Z = 180.0f;
		arrowLeft.AddTexture( "null", 0.5f, 0.0f, 0.5f, 1.0f );
		arrowLeft.Parent( menuSwitch0 );
		Add( arrowLeft );

		WidgetGraphic arrowRight = (WidgetGraphic)arrow.Copy();
		arrowRight.Name = "arrow_increase";
		arrowRight.Position = new Vector3( 50.0f, 0.0f, 0.0f );
		arrowRight.AddTexture( "null", 0.5f, 0.0f, 0.5f, 1.0f );
		arrowRight.Parent( menuSwitch0 );
		Add( arrowRight );

		menuSwitch0.ArrowDecrease = arrowLeft;
		menuSwitch0.ArrowIncrease = arrowRight;

		for ( int i = 0; i < 11; ++i )
		{
			WidgetMenuNode node = new WidgetMenuNode( i );
			node.Parent( menuSwitch0 );
			Add( node );

			WidgetText text = new WidgetText();
			text.Size = new Vector3( 0.0f, 50.0f, 0.0f );
			text.String = "" + i;
			text.FontStyleName = "Default";
			text.Align = E_Align.MiddleCentre;
			text.ColorBase = Color.Orange;
			text.Parent( node );
			text.ParentAttach = E_Align.MiddleCentre;
			Add( text );
		}

		// sfx volume
		WidgetMenuNode node1 = (WidgetMenuNode)node0.Copy();
		node1.Value = 1;
		Add( node1 );
		node1.Parent( menu );

		WidgetText text1 = (WidgetText)text0.Copy();
		text1.String = "SFX VOLUME";
		text1.Parent( node1 );
		Add( text1 );

		WidgetMenuSwitch menuSwitch1 = (WidgetMenuSwitch)menuSwitch0.CopyAndAdd( this );
		menuSwitch1.ArrowDecrease = menuSwitch1.FindChild( "arrow_decrease" );
		menuSwitch1.ArrowIncrease = menuSwitch1.FindChild( "arrow_increase" );
		menuSwitch1.Parent( node1 );

		MenuSwitch0 = menuSwitch0;
		MenuSwitch1 = menuSwitch1;
	}

	// OnInit
	protected override void OnInit()
	{
		SetScreenTimers( 0.25f, 0.25f );
	}

	// OnPostInit
	protected override void OnPostInit()
	{
		MenuSwitch0.SetByValue( 9 );
		MenuSwitch1.SetByValue( 6 );
	}

	// OnProcessInput
	protected override void OnProcessInput( Input input )
	{
		if ( input.ButtonJustPressed( (int)E_UiButton.B ) )
			_UI.Screen.SetNextScreen( new Screen_MainMenu() );
	}

	//
	private WidgetMenuSwitch		MenuSwitch0;
	private WidgetMenuSwitch		MenuSwitch1;
	//
};

}; // namespace UI
