using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using escape.Blocks;
using escape.Enemies;
using escape.Interfaces;
using escape.Sprites;
using System.Collections.Generic;

namespace escape;

// Runs the game and keeps the demo objects together
public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _blockAtlas = null!;
    private readonly BlockManager _blockManager = new();
    private readonly GameResetCoordinator _resetCoordinator = new();
    private KeyboardState _previousKeyboardState;
    private readonly List<IEnemy> _enemies = new();

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 960;
        _graphics.PreferredBackBufferHeight = 540;
        _graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _resetCoordinator.Register(_blockManager);
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    // Load the textures and objects used by the starter demo
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load the PixelPack block tiles
        _blockAtlas = TextureLoader.Load(GraphicsDevice, "Content/Textures/Blocks/PixelPack_Block_Atlas.png");

        // Make ten blocks so the team can see how the factory chooses each block
        var blockTypes = new[]
        {
            "grass", "dirt", "stone", "brick", "water",
            "wood", "sand", "ice", "metal", "platform"
        };

        for (int i = 0; i < blockTypes.Length; i++)
        {
            var position = new Vector2(448, 238);
            _blockManager.Add(new Block(_blockAtlas, blockTypes[i], position, 2f));
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
            2f);

        var enemyA = new EnemyA(enemySprite, enemyPosition);
        _enemies.Add(enemyA);
        RegisterResettable(enemyA);

        _previousKeyboardState = Keyboard.GetState();
    }

    // Update every sprite once per frame
    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.T) && _previousKeyboardState.IsKeyUp(Keys.T))
        {
            _blockManager.SelectPrevious();
        }
        else if (keyboardState.IsKeyDown(Keys.Y) && _previousKeyboardState.IsKeyUp(Keys.Y))
        {
            _blockManager.SelectNext();
        }

        _blockManager.Update(gameTime);
        _previousKeyboardState = keyboardState;

        foreach (var enemy in _enemies)
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

        _blockManager.Draw(_spriteBatch);

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
        _resetCoordinator.Reset();
    }

    // Add a player or system so the game-wide reset can reach it
    public void RegisterResettable(IGameResettable system)
    {
        _resetCoordinator.Register(system);
    }
}
