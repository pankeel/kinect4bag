//-----------------------------------------------
// XUI - Background.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

namespace UI
{

// class Screen_Background
public class Screen_Background : Screen
{
	// Screen_Background
	public Screen_Background()
		: base( "Background" )
	{
	#if !RELEASE
		Add( new WidgetFPS() );
	#endif
	}
};

}; // namespace UI
