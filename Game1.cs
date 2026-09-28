using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using escape.Blocks;
using escape.Enemies;
using escape.Interfaces;
using escape.Sprites;
using escape.Inputs;
using System.Collections.Generic;

namespace escape;

// Runs the game and keeps the demo objects together
public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _blockAtlas = null!;
    private Texture2D _playerSheet = null!;
    private IController _keyboardController = null!;
    private readonly BlockManager _blockManager = new();
    private readonly GameResetCoordinator _resetCoordinator = new();
    private KeyboardState _previousKeyboardState;
    private IController keyboardController = null!;
    private Player _player = null!;

    private readonly List<ISprite> _sprites = new();

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

        _playerSheet = Texture2D.FromFile(GraphicsDevice, "Content/Textures/Player/link.png");
        _player = new Player(new Vector2(100, 100), _playerSheet);

        // Load the PixelPack block tiles
        _blockAtlas = TextureLoader.Load(GraphicsDevice, "Content/Textures/Blocks/PixelPack_Block_Atlas.png");

        _player = new Player(
            new Vector2(100, 100),
            _blockAtlas, 1,
            _blockAtlas, 1,
            _blockAtlas, 1,
            _spriteBatch);

        RegisterResettable(_player); 

        keyboardController = new KeyboardController(this, _player);
        keyboardController.Initialize();

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
        
        _keyboardController = new KeyboardController(this, _player);
        _keyboardController.Initialize();

        var enemyAPosition = new Vector2(450, 200);

        var enemyAFrames = new[]
        {
            new Rectangle(0, 0, 32, 32),
            new Rectangle(32, 0, 32, 32)
        };

        var enemyASprite = SpriteFactory.CreateAnimatedSprite(
            _blockAtlas,
            enemyAFrames,
            enemyAPosition,
            0.12f,
            2f);

        var enemyA = new EnemyA(enemyASprite, enemyAPosition);

        _enemies.Add(enemyA);

        var enemyBPosition = new Vector2(660, 300);

        var enemyBFrames = new[]
        {
            new Rectangle(64, 0, 32, 32),
            new Rectangle(96, 0, 32, 32)
        };

        var enemyBSprite = SpriteFactory.CreateAnimatedSprite(
            _blockAtlas,
            enemyBFrames,
            enemyBPosition,
            0.12f,
            2f
        );

        var enemyB = new EnemyB(enemyBSprite, enemyBPosition);

        _enemies.Add(enemyB);

        var enemyCPosition = new Vector2(450, 400);

        var enemyCFrames = new[]
        {
            new Rectangle(128, 0, 32, 32),
            new Rectangle(160, 0, 32, 32)
        };

        var enemyCSprite = SpriteFactory.CreateAnimatedSprite(
            _blockAtlas,
            enemyCFrames,
            enemyCPosition,
            0.12f,
            2f
        );

        var enemyC = new EnemyC(enemyCSprite, enemyCPosition);

        _enemies.Add(enemyC);

        var bossPosition = new Vector2(480, 300);

        var bossFrames = new[]
        {
            new Rectangle(192, 0, 32, 32),
            new Rectangle(224, 0, 32, 32)
        };

        var bossSprite = SpriteFactory.CreateAnimatedSprite(
            _blockAtlas,
            bossFrames,
            bossPosition,
            0.12f,
            2f
        );

        var boss = new Boss(bossSprite, bossPosition);
        _enemies.Add(boss);

        foreach (var enemy in _enemies)
        {
            RegisterResettable(enemy);
        }

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

        _player.Update(gameTime);

        _keyboardController.Update();
        
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
        _player.Draw(_spriteBatch);

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
        _player.Reset();
    }
}
