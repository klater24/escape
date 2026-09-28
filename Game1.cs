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
    private readonly BlockManager _blockManager = new();
    private readonly GameResetCoordinator _resetCoordinator = new();
    private KeyboardState _previousKeyboardState;
    private IController keyboardController = null!;
    private Player _player = null!;
    private Texture2D _playerSheet = null!;

    private EnemySpriteFactory _enemySpriteFactory = null!;
    private EnemyManager _enemies = null!;

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

        _playerSheet = TextureLoader.Load(GraphicsDevice, "Content/Textures/Player/link.png");
        _player = new Player(new Vector2(100, 100), _playerSheet);

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

        _enemySpriteFactory = new EnemySpriteFactory(GraphicsDevice);
        var enemyPosition = new Vector2(450, 260);
        _enemies = new EnemyManager(new IEnemy[]
        {
            new EnemyA(_enemySpriteFactory.Create("Run", enemyPosition), enemyPosition),
            new EnemyB(_enemySpriteFactory.Create("DemonFlying", enemyPosition), enemyPosition),
            new EnemyC(_enemySpriteFactory.Create("ManaSeed", enemyPosition), enemyPosition),
            new Boss(_enemySpriteFactory.Create("SorcererAttack", enemyPosition), enemyPosition)
        });
        RegisterResettable(_enemies);
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

        _enemies.Update(gameTime);

        keyboardController.Update();
        _player.Update(gameTime);
        Window.Title = $"Escape | Enemy {_enemies.SelectedIndex + 1}/4: {_enemies.Current.GetType().Name} | O/P: enemies | T/Y: blocks | R: reset | Q: quit";
        base.Update(gameTime);
    }

    // Draw everything in the scene
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _blockManager.Draw(_spriteBatch);

        _enemies.Draw(_spriteBatch);
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
    }
    public void CycleEnemy(int direction) => _enemies.Cycle(direction);

    protected override void UnloadContent()
    {
        _enemySpriteFactory.Dispose();
        _blockAtlas.Dispose();
        _playerSheet.Dispose();
        _spriteBatch.Dispose();
        base.UnloadContent();
    }}
