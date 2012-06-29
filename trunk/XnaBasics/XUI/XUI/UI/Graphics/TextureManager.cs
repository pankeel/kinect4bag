//-----------------------------------------------
// XUI - TextureManager.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace UI
{

// class TextureBundle
public class TextureBundle
{
	// TextureBundle
	public TextureBundle()
	{
		Content = new ContentManager( _UI.Game.Services, "Content" );
		Textures = new List< Texture2D >();
		TextureNameMap = new Dictionary< string, int >();
	}

	// Destroy
	public void Destroy()
	{
		Content.Unload();
	}

	// Add
	public void Add( string path, string name )
	{
		Textures.Add( Content.Load< Texture2D >( path ) );
		TextureNameMap.Add( name, Textures.Count - 1 );
	}

    public void Add(Texture2D texture, string name)
    {
        Textures.Add(texture);
        TextureNameMap.Add(name, Textures.Count - 1);
    }

	// Get
	public int Get( string name )
	{
		int result;

		if ( TextureNameMap.TryGetValue( name, out result ) )
			return result;

		return -1;
	}

	public Texture2D Get( int index )
	{
		return Textures[ index ];
	}

	//
	private ContentManager					Content;
	private List< Texture2D >				Textures;
	private Dictionary< string, int >		TextureNameMap;
	//
};

// class TextureManager
public class TextureManager
{
	// TextureManager
	public TextureManager()
	{
		Bundles = new TextureBundle[ _UI.Settings.Texture_BundleCount ];

		// default bundle
		Bundles[ 0 ] = new TextureBundle();
		Bundles[ 0 ].Add( "Textures\\UI_Null", "null" );
	}

	// CreateBundle
	public int CreateBundle()
	{
		for ( int i = 0; i < Bundles.Length; ++i )
		{
			if ( Bundles[ i ] != null )
				continue;

			Bundles[ i ] = new TextureBundle();

			return i;
		}

		return -1;
	}

	// DestroyBundle
	public void DestroyBundle( int index )
	{
		if ( index == -1 )
		{
			// destroy all
			for ( int i = 0; i < Bundles.Length; ++i )
			{
				if ( Bundles[ i ] == null )
					continue;

				Bundles[ i ].Destroy();
				Bundles[ i ] = null;
			}
		}
		else
		{
			Bundles[ index ].Destroy();
			Bundles[ index ] = null;
		}
	}

	// Add
	public void Add( int bundleIndex, string path, string name )
	{
		Bundles[ bundleIndex ].Add( path, name );
	}

    public void AddDynamic( int bundleIndex, string modelPath, string texturePath, string name )
    {
        TextureBundle bundle = Bundles[bundleIndex];

        // Load Device Texture2D

        GraphicsDevice graphics = _UI.Game.GraphicsDevice;
        SpriteBatch spriteBatch = (SpriteBatch)_UI.Game.Services.GetService(typeof(SpriteBatch));

        
        RenderTarget2D backBuffer = new RenderTarget2D(graphics,
                        200,
                        200,
                        false,
                        SurfaceFormat.Color,
                        DepthFormat.None,
                        graphics.PresentationParameters.MultiSampleCount,
                        RenderTargetUsage.PreserveContents);
        // Set the backbuffer and clear
        graphics.SetRenderTarget(backBuffer);
        graphics.Clear(ClearOptions.Target, Color.Yellow, 1.0f, 0);

        // Draw Somethine
        graphics.DepthStencilState = DepthStencilState.Default;
        Model model = _UI.Game.Content.Load<Model>(modelPath);
        Texture2D modelSkin = _UI.Game.Content.Load<Texture2D>(texturePath);
        foreach (ModelMesh mesh in model.Meshes)
        {
            
            foreach (BasicEffect effect in mesh.Effects)
            {
                effect.Texture = modelSkin;
                effect.World = Matrix.Identity;
                effect.View = Matrix.CreateLookAt(
                    new Vector3(0.0f,0.0f,-10f),
                    new Vector3(0, 0, 0),
                    Vector3.Up);
                effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                    (45.6f * (float)Math.PI / 180.0f),
                    graphics.Viewport.AspectRatio,
                    1,
                    10000);
                effect.EnableDefaultLighting();

                effect.SpecularColor = new Vector3(0.25f);
                effect.SpecularPower = 16;
            }

            mesh.Draw();
        }
        // Reset the render target and prepare to draw scaled image
        graphics.SetRenderTargets(null);

        Bundles[bundleIndex].Add(backBuffer as Texture2D, name);

    }

	// Get
	public int Get( string name )
	{
		for ( int i = 0; i < Bundles.Length; ++i )
		{
			if ( Bundles[ i ] == null )
				continue;

			int result = Bundles[ i ].Get( name );

			if ( result != -1 )
				return ( ( i << 16 ) | result );
		}

		return 0; // null
	}

	public Texture2D Get( int textureIndex )
	{
		return Bundles[ textureIndex >> 16 ].Get( textureIndex & 0xffff );
	}

	//
	private TextureBundle[]		Bundles;
	//
};

// struct SpriteTexture
public struct SpriteTexture
{
	// SpriteTexture
	public SpriteTexture( string name, ref Vector2 puv, ref Vector2 suv )
	{
		TextureIndex = _UI.Texture.Get( name );
		PUV = puv;
		SUV = suv;
	}

	public SpriteTexture( string name, float pu, float pv, float su, float sv )
	{
		TextureIndex = _UI.Texture.Get( name );
		PUV = new Vector2( pu, pv );
		SUV = new Vector2( su, sv );
	}

	public Vector2			PUV;
	public Vector2			SUV;
	public int				TextureIndex;

	public static int		TextureCount = 3;
};

}; // namespace UI
