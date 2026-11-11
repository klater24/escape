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
using escape.GameStates;
using escape.UI;
using escape.View;
using escape.Sounds;

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
    public Inventory Inventory { get; private set; } = null!;
    public GameStateManager States { get; } = new();
    public IGameTransition Transitions { get; set; } = new ImmediateGameTransition();
    public IInventoryScreen InventoryScreen { get; set; } = null!;
    private StateScreen _stateScreen = null!;

    public int Rupees { get; private set; } = 0;
    private SpriteFont _hudFont = null!;
    private HUD _hud = null!;
    private Texture2D _hudAtlas = null!;
    private Audio _audio = null!;
    private RenderTarget2D _outgoingScene = null!;
    private RenderTarget2D _incomingScene = null!;
    private const int HudHeight = 168;
    private readonly RasterizerState _transitionRasterizer = new() { ScissorTestEnable = true };



    public void AddRupees(int amount = 1)
    {
        if (amount > 0)
            Rupees += amount;
    }

    public bool TrySpendRupees(int amount)
    {
        if (amount <= 0 || Rupees < amount) return false;
        Rupees -= amount;
        return true;
    }

    public void ToggleMusicMute() => _audio.ToggleMusicMute();
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
        RegisterResettable(Inventory);
        base.Initialize();
    }

    // Load the textures and objects used by the starter demo
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _outgoingScene = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        _incomingScene = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        _audio = new Audio();
        _audio.LoadContent(Content);
        _audio.PlayMusic("MainTheme");
        Inventory.ItemAdded += OnItemAdded;
        Inventory.ItemRemoved += OnItemRemoved;
        var stateFont = Content.Load<SpriteFont>("Fonts/StateFont");
        _stateScreen = new StateScreen(GraphicsDevice, stateFont);
        InventoryScreen = new BasicInventoryScreen(Inventory, stateFont);

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
        Transitions = new ScreenTransition(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
            HudHeight, _levelManager.GetTransitionDirection);
        _levelManager.RoomChanged += ConnectRoomCombat;
        _levelManager.RoomChanging += BeginRoomTransition;
        ConnectRoomCombat();

        _keyboardController = new KeyboardController(this, _player, _projectiles, _levelManager);
        _keyboardController.Initialize();
    }

    // Update every sprite once per frame
    protected override void Update(GameTime gameTime)
    {
        EvaluateOutcome();
        var stateBeforeInput = States.Current;
        bool wasAttacking = _player.IsAttacking;
        float previousHealth = _player.Health;
        _keyboardController.Update();
        if (!wasAttacking && _player.IsAttacking) _audio.PlaySound("attack");
        EvaluateOutcome();
        if (States.Current == GameState.Transition)
        {
            Transitions.Update(gameTime);
            if (!Transitions.IsTransitioning) States.CompleteTransition();
        }
        else if (States.Current == GameState.Inventory && stateBeforeInput == GameState.Inventory)
        {
            InventoryScreen.Update(gameTime);
        }
        else if (States.CanUpdateGameplay && stateBeforeInput == GameState.Gameplay)
        {
            _player.Update(gameTime);
            _levelManager.Update(gameTime);
            EvaluateOutcome();
            if (States.CanUpdateGameplay)
            {
                _projectiles.Obstacles = _levelManager.CurrentRoom.Blocks.Cast<ISolidCollider>()
                    .Concat(_levelManager.CurrentRoom.ExteriorWalls).ToArray();
                _projectiles.Update(gameTime);
                foreach (var enemy in _levelManager.CurrentRoom.Enemies.OfType<IDamageable>())
                    CombatCollisionHandler.Resolve(_player, enemy);
                _projectiles.ResolveCombat(_player, _levelManager.CurrentRoom.Enemies.OfType<IDamageable>());
                EvaluateOutcome();
                if (States.CanUpdateGameplay) _levelManager.CheckDoorTransitions();
            }
        }
        var boss = _levelManager.CurrentRoom.Enemies.OfType<Boss>().FirstOrDefault();
        if (_player.Health < previousHealth) _audio.PlaySound("damage");
        string bossStatus = boss is null ? "" : $" | Boss HP: {boss.Health}/{Boss.MaxHealth}";
        Window.Title = $"Escape | {States.Current} | HP: {_player.Health}/{_player.MaxHealth}{bossStatus} | M: music {(_audio.IsMusicMuted ? "off" : "on")} | P: pause | Enter: inventory | X: equipped item | R: restart | Q: quit";
        base.Update(gameTime);
    }

    // Draw everything in the scene
    protected override void Draw(GameTime gameTime)
    {
        if (States.Current == GameState.Transition && Transitions is ScreenTransition transition)
        {
            CaptureScene(_incomingScene, transition.To, gameTime);
            DrawTransition(transition);
        }
        else DrawScene(States.Current, gameTime);
        base.Draw(gameTime);
    }

    private void DrawScene(GameState state, GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        if (state != GameState.Inventory)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp,
                transformMatrix: Matrix.CreateTranslation(0, HudHeight, 0));
            _levelManager.Draw(_spriteBatch);
            _projectiles.Draw(_spriteBatch, gameTime);
            _player.Draw(_spriteBatch);
            _spriteBatch.End();
        }
        else
        {
            GraphicsDevice.Clear(Color.Black);
        }

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        if (state == GameState.Inventory)
            InventoryScreen.Draw(_spriteBatch, GraphicsDevice.Viewport.Bounds);
        else
        {
            _hud.Draw(_spriteBatch, Rupees);
            _stateScreen.Draw(_spriteBatch, GraphicsDevice.Viewport.Bounds, state);
        }
        _spriteBatch.End();

    }

    private void CaptureScene(RenderTarget2D target, GameState state, GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(target);
        DrawScene(state, gameTime);
        GraphicsDevice.SetRenderTarget(null);
    }

    private void DrawTransition(ScreenTransition transition)
    {
        GraphicsDevice.Clear(Color.Black);
        var bounds = GraphicsDevice.Viewport.Bounds;
        var source = transition.IsRoomTransition
            ? new Rectangle(0, HudHeight, bounds.Width, bounds.Height - HudHeight) : bounds;
        var previousScissor = GraphicsDevice.ScissorRectangle;
        GraphicsDevice.ScissorRectangle = source;
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, rasterizerState: _transitionRasterizer);
        _spriteBatch.Draw(_outgoingScene, source.Location.ToVector2() + transition.OutgoingOffset, source, Color.White);
        _spriteBatch.Draw(_incomingScene, source.Location.ToVector2() + transition.IncomingOffset, source, Color.White);
        _spriteBatch.End();
        GraphicsDevice.ScissorRectangle = previousScissor;
        if (transition.IsRoomTransition)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _hud.Draw(_spriteBatch, Rupees);
            _spriteBatch.End();
        }
    }

    // Reset every sprite to its starting state
    public void Reset()
    {
        Transitions.Cancel();
        Rupees = 0;
        _resetCoordinator.Reset();
        InventoryScreen.Reset();
        States.Reset();
    }

    public void ToggleInventory()
    {
        var from = States.Current;
        if (!States.BeginInventoryTransition()) return;
        CaptureScene(_outgoingScene, from, new GameTime());
        InventoryScreen.Reset();
        Transitions.Begin(from, States.TransitionTarget);
    }

    private void EvaluateOutcome() => States.EvaluateOutcome(_player.IsDefeated, Inventory.HasItem(ItemType.Triforce));

    private void BeginRoomTransition(string roomId)
    {
        if (States.BeginRoomTransition())
        {
            CaptureScene(_outgoingScene, GameState.Gameplay, new GameTime());
            Transitions.Begin(GameState.Gameplay, GameState.Gameplay, roomId);
        }
    }

    private void OnItemAdded(ItemType item, int quantity) => _audio.PlaySound("ItemPickup");
    private void OnItemRemoved(ItemType item, int quantity) => _audio.PlaySound("main");

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
        _stateScreen.Dispose();
        Inventory.ItemAdded -= OnItemAdded;
        Inventory.ItemRemoved -= OnItemRemoved;
        _audio.StopMusic();
        _outgoingScene.Dispose();
        _incomingScene.Dispose();
        _transitionRasterizer.Dispose();
        base.UnloadContent();
    }}

