using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using escape.Sprites;
using System.Collections.Generic;
using escape.Enemies;
using escape.Input;
using Microsoft.Xna.Framework.Input;

namespace escape;

// Runs the game and keeps the demo objects together
public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _blockAtlas = null!;

    private readonly List<ISprite> _sprites = new();
    private EnemyManager _enemies = null!;
    private EnemySpriteFactory _enemySpriteFactory = null!;
    private readonly KeyboardController _keyboard = new();

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
            var blockPosition = new Vector2(40 + (i % 5) * 64, 40 + (i / 5) * 64);
            _sprites.Add(SpriteFactory.CreateBlockSprite(_blockAtlas, blockTypes[i], blockPosition, 2f));
        }

        _enemySpriteFactory = new EnemySpriteFactory(GraphicsDevice);
        var position = new Vector2(450, 260);
        _enemies = new EnemyManager(new IEnemy[]
        {
            new EnemyA(_enemySpriteFactory.Create("Run", position), position),
            new EnemyB(_enemySpriteFactory.Create("DemonFlying", position), position),
            new EnemyC(_enemySpriteFactory.Create("Shield", position), position),
            new Boss(_enemySpriteFactory.Create("SorcererAttack", position), position)
        });
        _keyboard.Bind(Keys.O, new ActionCommand(() => _enemies.Cycle(-1)));
        _keyboard.Bind(Keys.P, new ActionCommand(() => _enemies.Cycle(1)));
        _keyboard.Bind(Keys.R, new ActionCommand(Reset));
        _keyboard.Bind(Keys.Q, new ActionCommand(Exit));
    }
    // Update every sprite once per frame
    protected override void Update(GameTime gameTime)
    {
        foreach (var sprite in _sprites)
        {
            sprite.Update(gameTime);
        }

        _keyboard.Update(Keyboard.GetState());
        _enemies.Update(gameTime);
        Window.Title = $"Escape | Enemy {_enemies.SelectedIndex + 1}/4: {_enemies.Current.GetType().Name} | O/P: cycle | R: reset | Q: quit";

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

        _enemies.Draw(_spriteBatch);

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

        _enemies.Reset();
    }
    protected override void UnloadContent()
    {
        _enemySpriteFactory.Dispose();
        _blockAtlas.Dispose();
        _spriteBatch.Dispose();
        base.UnloadContent();
    }
}
