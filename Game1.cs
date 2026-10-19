using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using escape.Inputs;
using escape.Projectiles;
using escape.Levels;

namespace escape;

// Runs the game and keeps the demo objects together
public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _blockAtlas = null!;
    private readonly GameResetCoordinator _resetCoordinator = new();
    private IController _keyboardController = null!;
    private Player _player = null!;
    private Texture2D _playerSheet = null!;
    private Texture2D _itemAtlas = null!;
    private Texture2D _projectileAtlas = null!;
    private Texture2D _bossAtlas = null!;
     private readonly List<ISprite> _sprites = new();
    private ProjectileManager _projectiles = null!;
    private ProjectileFactory _projectileFactory = null!;
    private Room _currentRoom;
    private MouseState _previousMouseState;



    private EnemyManager _enemies = null!;
    private LevelManager _levelManager = null!;

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

        _itemAtlas = Content.Load<Texture2D>("zeldaitems");
        _projectileAtlas = Content.Load<Texture2D>("zelda");
        _bossAtlas = Content.Load<Texture2D>("boses");

        // Load the PixelPack block tiles
        _blockAtlas = Content.Load<Texture2D>("Textures/Blocks/PixelPack_Block_Atlas");

        _playerSheet = Content.Load<Texture2D>("Textures/Player/linkOrganized");
        _player = new Player(new Vector2(100, 100), _playerSheet);

        _levelManager = new LevelManager(_blockAtlas, Path.Combine(Content.RootDirectory, "Levels"), 
                                            Content, _itemAtlas, _player);

        RegisterResettable(_player);

        _projectileFactory = new ProjectileFactory(_projectileAtlas, _bossAtlas);
        _projectiles = new ProjectileManager(_projectileFactory);
        RegisterResettable(_projectiles);
        RegisterResettable(_levelManager);

        _keyboardController = new KeyboardController(this, _player, _projectiles, _levelManager);
        _keyboardController.Initialize();
    }

    // Update every sprite once per frame
    protected override void Update(GameTime gameTime)
    {
        _levelManager.Update(gameTime);

        _projectiles.Update(gameTime);
        _projectiles.RemoveOffscreenBossProjectiles(GraphicsDevice.Viewport.Width);

        // Update the player's obstacles based on the current block in the block manager
        _player.Obstacles = _blockManager.CurrentBlock is { } block
            ? new List<ISolidCollider> { block }
            : new List<ISolidCollider>();


        //probs need to fix this at some point 
        Vector2 prevPlayerPos = _player.getPosit();
        Vector2 currPlayerPos = _player.getPosit();
        MouseState currentMouseState = Mouse.GetState();
        if (prevPlayerPos != currPlayerPos)
        {
            Vector2 movement = currPlayerPos - prevPlayerPos;
            //need a thing of obstacles
            EnvironmentCollisionResolver.Move(_player, movement, null);
           _currentRoom.CheckDoorTransition(_player, _currentRoom, currentMouseState, _previousMouseState);
           _previousMouseState = currentMouseState; 
        }



        _keyboardController.Update();
        _player.Update(gameTime);
        Window.Title = "Escape | WASD/Arrows: move | Z/N: attack | E: damage | 1: arrow | 2: bomb | 3: boomerang | F1-F3: change room | R: reset | Q: quit";
        base.Update(gameTime);
    }

    // Draw everything in the scene
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _projectiles.Draw(_spriteBatch, gameTime);

        _levelManager.Draw(_spriteBatch);

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

    protected override void UnloadContent()
    {
        // Content owns and disposes the textures loaded through Content.Load.
        _spriteBatch.Dispose();
        base.UnloadContent();
    }}
