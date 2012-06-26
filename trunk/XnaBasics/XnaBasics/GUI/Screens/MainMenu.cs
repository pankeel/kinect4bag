//-----------------------------------------------
// XUI - MainMenu.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

using Microsoft.Xna.Framework;

namespace UI
{

// class Screen_MainMenu
public class Screen_MainMenu : Screen
{
	// Screen_MainMenu
	public Screen_MainMenu()
		: base( "MainMenu" )
	{
		WidgetGraphic logo = new WidgetGraphic();
		logo.Position = new Vector3( _UI.SXM, _UI.SYM - 65.0f, 0.0f );
		logo.Size = new Vector3( _UI.SY / 3.0f, _UI.SY / 3.0f, 0.0f );
		logo.Align = E_Align.BottomCentre;
		logo.ColorBase = new SpriteColors( Color.Orange, Color.Orange, Color.Black, Color.Black );
		logo.AddTexture( "null", 0.0f, 0.0f, 1.0f, 1.0f );
		Add( logo );

		Timeline logoT = new Timeline( "end_fade", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start );
		logoT.AddEffect( new TimelineEffect_Alpha( 0.0f, -1.0f, E_LerpType.SmoothStep ) );
		logo.AddTimeline( logoT );

		Timeline logoT2 = new Timeline( "end_move", false, 0.25f, 0.25f, E_TimerType.Stop, E_RestType.None );
		logoT2.AddEffect( new TimelineEffect_PositionY( 0.0f, 65.0f, E_LerpType.SmoothStep ) );
		logo.AddTimeline( logoT2 );

		if ( _G.UI.MM_FromStartScreen )
		{
			logo.Position = new Vector3( _UI.SXM, _UI.SYM, 0.0f );

			Timeline logoT3 = new Timeline( "start", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.None );
			logoT3.AddEffect( new TimelineEffect_PositionY( 0.0f, -65.0f, E_LerpType.SmoothStep ) );
			logo.AddTimeline( logoT3 );
		}
		else
		if ( _G.UI.MM_FromLevelSelect )
		{
			Timeline logoT4 = new Timeline( "start", false, 0.25f, 0.25f, E_TimerType.Stop, E_RestType.None );
			logoT4.AddEffect( new TimelineEffect_Alpha( -1.0f, 0.0f, E_LerpType.SmoothStep ) );
			logo.AddTimeline( logoT4 );
		}

		_G.UI.MM_FromStartScreen = false;
		_G.UI.MM_FromLevelSelect = false;

		Logo = logo;

		WidgetMenuScroll menu = new WidgetMenuScroll( E_MenuType.Vertical );
		menu.Position = new Vector3( _UI.SXM, _UI.SYM + 25.0f, 0.0f );
		menu.Padding = 75.0f;
		menu.Alpha = 0.0f;
		Add( menu );

		Timeline menuT = new Timeline( "start", false, 0.25f, 0.25f, E_TimerType.Stop, E_RestType.None );
		menuT.AddEffect( new TimelineEffect_Alpha( 0.0f, 1.0f, E_LerpType.SmoothStep ) );
		menu.AddTimeline( menuT );

		Timeline menuT2 = new Timeline( "end", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.None );
		menuT2.AddEffect( new TimelineEffect_Alpha( 0.0f, -1.0f, E_LerpType.SmoothStep ) );
		menu.AddTimeline( menuT2 );

		Menu = menu;

		for ( int i = 0; i < Options.Length; ++i )
		{
			WidgetMenuNode node = new WidgetMenuNode( i );
			node.Parent( Menu );
			Add( node );
			
			Timeline nodeT = new Timeline( "selected", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start );
			nodeT.AddEffect( new TimelineEffect_ScaleX( 0.0f, 0.125f, E_LerpType.SmoothStep ) );
			nodeT.AddEffect( new TimelineEffect_ScaleY( 0.0f, 0.125f, E_LerpType.SmoothStep ) );
			
			Timeline nodeT2 = new Timeline( "selected", false, 0.0f, 0.5f, E_TimerType.Bounce, E_RestType.Start );
			nodeT2.AddEffect( new TimelineEffect_Intensity( 0.0f, 0.75f, E_LerpType.SmoothStep ) );

			node.AddTimeline( nodeT );
			node.AddTimeline( nodeT2 );

			WidgetText text = new WidgetText();
			text.Size = new Vector3( 0.0f, 50.0f, 0.0f );
			text.Align = E_Align.MiddleCentre;
			text.FontStyleName = "Default";
			text.String = Options[ i ];
			text.Parent( node );
			text.ParentAttach = E_Align.MiddleCentre;
			text.ColorBase = Color.Orange;
			Add( text );

			WidgetGraphic icon = new WidgetGraphic();
			icon.Layer = 1;
			icon.Position = new Vector3( -10.0f, 0.0f, 0.0f );
			icon.Size = new Vector3( 60.0f, 60.0f, 0.0f );
			icon.AddTexture( TextureNames[ i ], 0.0f, 0.0f, 1.0f, 1.0f );
			icon.Alpha = 0;
			icon.Parent( text );
			icon.ColorBase = Color.White;

			if ( ( i & 1 ) == 0 )
			{
				icon.Align = E_Align.MiddleRight;
				icon.ParentAttach = E_Align.MiddleLeft;
			}
			else
			{
				icon.Align = E_Align.MiddleLeft;
				icon.ParentAttach = E_Align.MiddleRight;
				icon.Position *= -1.0f;
			}
			
			Add( icon );

			Timeline iconT = new Timeline( "selected", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start );
			iconT.AddEffect( new TimelineEffect_Alpha( 0.0f, 1.0f, E_LerpType.SmoothStep ) );
			icon.AddTimeline( iconT );
		}
	}

	// OnInit
	protected override void OnInit()
	{
		SetScreenTimers( 0.25f, 0.25f );
	}

	// OnProcessInput
	protected override void OnProcessInput( Input input )
	{
		if ( input.ButtonJustPressed( (int)E_UiButton.A ) )
		{
			if ( Menu.GetByValue() == 0 )
			{
				_UI.Screen.AddScreen( new Screen_Popup( E_PopupType.NewGame ) );
			}
			else
			if ( Menu.GetByValue() == 1 )
			{
				Logo.TimelineActive( "end_fade", true, false );
				_UI.Screen.SetNextScreen( new Screen_LevelSelect() );
			}
			else
			if ( Menu.GetByValue() == 2 )
			{
				_UI.Screen.SetNextScreen( new Screen_Options() );
			}
			else
			if ( Menu.GetByValue() == 3 )
			{
				_UI.Screen.AddScreen( new Screen_Popup( E_PopupType.Quit ) );
			}
		}
		else
		if ( input.ButtonJustPressed( (int)E_UiButton.B ) )
		{
			SetScreenTimers( 0.0f, 0.5f );

			Logo.TimelineActive( "end_move", true, false );
			_G.UI.SS_FromMainMenu = true;
			
			_UI.Screen.SetNextScreen( new Screen_Start() );
		}
	}

	// OnProcessMessage
	protected override void OnProcessMessage( ref ScreenMessage message )
	{
		E_UiMessageType type = (E_UiMessageType)message.Type;

		if ( type == E_UiMessageType.PopupConfirm )
		{
			switch ( (E_PopupType)message.Data )
			{
				case E_PopupType.NewGame:	/* new game logic here */ 		break;
				case E_PopupType.Quit:		_UI.Game.Exit();				break;
			}
		}
	}

	//
	private static string[]			TextureNames = { "null", "null", "null", "null" };
	private static string[]			Options = { "NEW GAME", "LEVEL SELECT", "OPTIONS", "QUIT" };

	private WidgetGraphic			Logo;
	private WidgetMenuScroll		Menu;
	//
};

}; // namespace UI
