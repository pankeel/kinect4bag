//-----------------------------------------------
// XUI - Popup.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

using Microsoft.Xna.Framework;

namespace UI
{

// E_PopupType
public enum E_PopupType
{
	NewGame = 0,
	Quit,

	Count,
};

// class Screen_Popup
public class Screen_Popup : Screen
{
	// Screen_Popup
	public Screen_Popup( E_PopupType type )
		: base( "Popup" )
	{
		Type = type;

		int layer = _UI.Sprite.TopLayer - 1;

		WidgetGraphic black = new WidgetGraphic();
		black.Layer = layer;
		black.Size = new Vector3( _UI.SX, _UI.SY, 0.0f );
		black.AddTexture( "null", 0.0f, 0.0f, 1.0f, 1.0f );
		black.ColorBase = Color.Black;
		black.Alpha = 0.0f;
		Add( black );

		Timeline blackT = new Timeline( "start", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start );
		blackT.AddEffect( new TimelineEffect_Alpha( 0.0f, 0.85f, E_LerpType.SmoothStep ) );
		black.AddTimeline( blackT );

		WidgetText text = new WidgetText();
		text.Layer = layer;
		text.Size = new Vector3( 600.0f, 40.0f, 0.0f );
		text.Align = E_Align.MiddleCentre;
		text.FontStyle = _UI.Store_FontStyle.Get( "Default" );
		text.String = Messages[ (int)Type ];
		text.ColorBase = Color.Yellow;
		text.FlagClear( E_WidgetFlag.InheritAlpha );
		text.Parent( black );
		text.ParentAttach = E_Align.MiddleCentre;

		Timeline textT = new Timeline( "start", false, 0.0f, 0.25f, E_TimerType.Stop, E_RestType.Start );
		textT.AddEffect( new TimelineEffect_Alpha( -1.0f, 0.0f, E_LerpType.SmoothStep ) );
		text.AddTimeline( textT );

		Vector2 stringSize = _UI.Font.StringSize( text.String, text.FontStyle, text.Size.Y, text.Size.X );
		float border = 80.0f;

		WidgetBox back = new WidgetBox();
		back.Layer = layer;
		back.Size = new Vector3( stringSize.X + border, stringSize.Y + border, 0.0f );
		back.Align = E_Align.MiddleCentre;
		back.AddTexture( "box", 0.0f, 0.0f, 1.0f, 1.0f );
		back.ColorBase = Color.DarkBlue;
		back.Parent( text );
		back.ParentAttach = E_Align.MiddleCentre;
		back.CornerSize = new Vector2( border / 2.0f );
		back.CornerPuv01 = new Vector2( 0.0f );
		back.CornerSuv01 = new Vector2( 0.25f );

		Add( back );
		Add( text );

		WidgetText textButtons = new WidgetText();
		textButtons.Layer = layer;
		textButtons.Position = new Vector3( 0.0f, 20.0f, 0.0f );
		textButtons.Size = new Vector3( 0.0f, 30.0f, 0.0f );
		textButtons.Align = E_Align.MiddleCentre;
		textButtons.FontStyleName = "Default";
		textButtons.String = "[[A]] YES     [[B]] NO";
		textButtons.ColorBase = Color.White;
		textButtons.Parent( back );
		textButtons.ParentAttach = E_Align.BottomCentre;
		Add( textButtons );
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
			_UI.Screen.AddMessage( (int)E_UiMessageType.PopupConfirm, (int)Type );
			_UI.Screen.SetNextScreen( null );
		}
		else
		if ( input.ButtonJustPressed( (int)E_UiButton.B ) )
		{
			_UI.Screen.AddMessage( (int)E_UiMessageType.PopupCancel, (int)Type );
			_UI.Screen.SetNextScreen( null );
		}
	}

	//
	private E_PopupType		Type;

	private string[]		Messages = new string[ (int)E_PopupType.Count ]
	{
		"Are you sure you want to start a New Game? All current progress will be lost.",	// E_PopupType.NewGame
		"Are you sure you want to Quit?"													// E_PopupType.Quit
	};
	//
};

}; // namespace UI
