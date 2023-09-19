﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XML_Visualizer;

public class Window : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private Texture2D tex;

    public Window()
    {
        this.graphics = new GraphicsDeviceManager(this);
        base.Content.RootDirectory = "Content";
        base.IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        this.spriteBatch = new SpriteBatch(GraphicsDevice);
        
        this.tex = new Texture2D(base.GraphicsDevice, 1, 1);
        this.tex.SetData( new Color[] { Color.Green });

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.GraphicsDevice.Clear(Color.CornflowerBlue);

        this.spriteBatch.Begin();
        this.spriteBatch.Draw(tex, new Rectangle(0, 0, 100, 200), Color.White);
        this.spriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}