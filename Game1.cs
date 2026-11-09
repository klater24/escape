using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using escape.Inputs;
using escape.Projectiles;
using escape.Levels;
using escape.Enemies;
using escape.Collisions;
using escape.Items;
using escape.Sprites;
using escape.UI;

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
    private ProjectileManager _projectiles = null!;
    private ProjectileFactory _projectileFactory = null!;
    private LevelManager _levelManager = null!;
    private Texture2D _exteriorTexture = null!;
    private readonly List<Boss> _roomBosses = new();
    public Inventory Inventory { get; private set; }
    public int Rupees { get; private set; } = 0;
    private SpriteFont _hudFont = null!;
    private HUD _hud = null!;
    private Texture2D _hudAtlas = null!;

    private EnemyManager _enemies = null!;

    public void AddRupees(int amount = 1)
    {
        if (amount > 0)
            Rupees += amount;
    }
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 768;
        _graphics.PreferredBackBufferHeight = 696;
        _graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Inventory = new Inventory();
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
        _exteriorTexture = Content.Load<Texture2D>("Textures/Blocks/Room-Exterior");

        _playerSheet = Content.Load<Texture2D>("Textures/Player/linkOrganized");
        _player = new Player(new Vector2(100, 100), _playerSheet, Content.Load<Texture2D>("Textures/Player/link"));

        _hudAtlas = Content.Load<Texture2D>("Textures/UI/NES - The Legend of Zelda - Miscellaneous - HUD & Pause Screen");
        _hudFont = Content.Load<SpriteFont>("HUDFont");
        _hud = new HUD(_player, Inventory, _hudFont, _itemAtlas, _hudAtlas);        

        RegisterResettable(_player);

        _projectileFactory = new ProjectileFactory(_projectileAtlas, _bossAtlas);
        _projectiles = new ProjectileManager(_projectileFactory);
        RegisterResettable(_projectiles);

        _levelManager = new LevelManager(_blockAtlas, Path.Combine(Content.RootDirectory, "Levels"),
                                            Content, _itemAtlas, _player, _exteriorTexture, Inventory, AddRupees);
        RegisterResettable(_levelManager);
        _levelManager.RoomChanged += ConnectRoomCombat;
        ConnectRoomCombat();

        _keyboardController = new KeyboardController(this, _player, _projectiles, _levelManager);
        _keyboardController.Initialize();
    }

    // Update every sprite once per frame
    protected override void Update(GameTime gameTime)
    {
        _levelManager.Update(gameTime);

        _projectiles.Obstacles = _levelManager.CurrentRoom.Blocks.Cast<ISolidCollider>()
            .Concat(_levelManager.CurrentRoom.ExteriorWalls).ToArray();
        _projectiles.Update(gameTime);

        _keyboardController.Update();
        _player.Update(gameTime);

        // Level Manager logic to check if player is in a door and should transition to another room
        _levelManager.CheckDoorTransitions();
        var currentRoomEnemies = _levelManager.CurrentRoom.Enemies;
        foreach (var enemy in currentRoomEnemies.OfType<IDamageable>())
        {
            CombatCollisionHandler.Resolve(_player, enemy);
        }

        _projectiles.ResolveCombat(_player, currentRoomEnemies.OfType<IDamageable>());
        var boss = currentRoomEnemies.OfType<Boss>().FirstOrDefault();

        string bossStatus = boss is null ? "" : $" | Boss HP: {boss.Health}/{Boss.MaxHealth}";
        Window.Title = $"Escape | HP: {_player.Health}/{_player.MaxHealth}{bossStatus}{(_player.IsDefeated ? " - Defeated: press R" : "")} | WASD/Arrows: move | Z/N: attack | E: damage | 1: arrow | 2: bomb | 3: boomerang | F1-F3: change room | R: reset | Q: quit";
        base.Update(gameTime);
    }

    // Draw everything in the scene
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateTranslation(0, 168, 0));

        _levelManager.Draw(_spriteBatch);

        _projectiles.Draw(_spriteBatch, gameTime);

        _player.Draw(_spriteBatch);

        _spriteBatch.End();

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _hud.Draw(_spriteBatch, Rupees);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    // Reset every sprite to its starting state
    public void Reset()
    {
        bool respawningAtDeathPosition = _player.IsDefeated;
        Vector2 deathPosition = _player.Position;

        Rupees = 0;
        Inventory.Reset();
        _resetCoordinator.Reset();

        if (respawningAtDeathPosition)
        {
            _player.Position = deathPosition;
        }
    }
    private void ConnectRoomCombat()
    {
        foreach (var boss in _roomBosses)
            boss.FireRequested -= SpawnBossProjectile;
        _roomBosses.Clear();
        _projectiles.Reset();
        _projectiles.Obstacles = _levelManager.CurrentRoom.Blocks.Cast<ISolidCollider>()
            .Concat(_levelManager.CurrentRoom.ExteriorWalls).ToArray();
        foreach (var boss in _levelManager.CurrentRoom.Enemies.OfType<Boss>())
        {
            boss.FireRequested += SpawnBossProjectile;
            _roomBosses.Add(boss);
        }
    }

    private void SpawnBossProjectile(Vector2 position)
    {
        if (_player.IsDefeated) return;
        Vector2 direction = _player.Bounds.Center.ToVector2() - (position + new Vector2(8, 16));
        if (direction.LengthSquared() < 0.001f) direction = Vector2.UnitX;
        _projectiles.SpawnBossProjectile(position, direction);
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

