using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using escape.Sprites;
using System.Collections.Generic;
using escape.Enemies;

namespace escape;

// Runs the game and keeps the demo objects together
public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _blockAtlas = null!;

    private readonly List<ISprite> _sprites = new();
    private readonly List<IEnemy> _enemies = new();

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 960;
        _graphics.PreferredBackBufferHeight = 540;
        _graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    // Load the textures and objects used by the starter demo
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Use colored placeholder blocks until the real art is added
        _blockAtlas = DemoSpriteSheetBuilder.CreateBlockAtlas(GraphicsDevice);

        // Make ten blocks so the team can see how the factory chooses each block
        var blockTypes = new[]
        {
            "grass", "dirt", "stone", "brick", "water",
            "wood", "sand", "ice", "metal", "platform"
        };

        for (int i = 0; i < blockTypes.Length; i++)
        {
            var position = new Vector2(40 + (i % 5) * 64, 40 + (i / 5) * 64);
            _sprites.Add(SpriteFactory.CreateBlockSprite(_blockAtlas, blockTypes[i], position, 2f));
        }

        var enemyPosition = new Vector2(450, 200);

        var enemyFrames = new[]
        {
            new Rectangle(0, 0, 32, 32),
            new Rectangle(32, 0, 32, 32)
        };

        var enemySprite = SpriteFactory.CreateAnimatedSprite(
            _blockAtlas,
            enemyFrames,
            enemyPosition,
            0.12f,
            2f
        );

        var enemyA = new EnemyA(enemySprite, enemyPosition);

        _enemies.Add(enemyA);
        
    }

    // Update every sprite once per frame
    protected override void Update(GameTime gameTime)
    {
        foreach (var sprite in _sprites)
        {
            sprite.Update(gameTime);
        }

        foreach(var enemy in _enemies)
        {
            enemy.Update(gameTime);
        }

        base.Update(gameTime);
    }

    // Draw everything in the scene
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        foreach (var sprite in _sprites)
        {
            sprite.Draw(_spriteBatch);
        }

        foreach (var enemy in _enemies)
        {
            enemy.Draw(_spriteBatch);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    // Reset every sprite to its starting state
    public void Reset()
    {
        foreach (var sprite in _sprites)
        {
            sprite.Reset();
        }

        foreach (var enemy in _enemies)
        {
            enemy.Reset();
        }
    }
}
