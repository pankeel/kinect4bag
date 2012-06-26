//-----------------------------------------------
// XUI - GameControls.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

// E_GameButton
public enum E_GameButton
{
	SomeGameButton = E_UiButton.Count,
	SomeOtherGameButton,

	// etc ...

	Count,
};

// E_GameAxis
public enum E_GameAxis
{
	SameGameAxis = E_UiAxis.Count,
	SameOtherGameAxis,

	// etc ...

	Count,
};

// class GameControls
public static class GameControls
{
	// Setup
	public static void Setup()
	{
		for ( int i = 0; i < GameInput.NumPads; ++i )
		{
			Input input = _G.GameInput.GetInput( i );

			_UI.SetupControls( input );

			// add custom mappings here ...
		}
	}
};
