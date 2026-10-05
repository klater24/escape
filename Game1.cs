using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Blocks;
using escape.Enemies;
using escape.Interfaces;
using escape.Sprites;
using escape.Inputs;
using escape.Projectiles;
using escape.Items;

namespace escape;

// Runs the game and keeps the demo objects together
public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _blockAtlas = null!;
    private readonly BlockManager _blockManager = new();
    private readonly GameResetCoordinator _resetCoordinator = new();
    private IController _keyboardController = null!;
    private Player _player = null!;
    private Texture2D _playerSheet = null!;
    private Texture2D _itemAtlas = null!;
    private Texture2D _projectileAtlas = null!;
    private Texture2D _bossAtlas = null!;
     private readonly List<ISprite> _sprites = new();
    private ItemManager _items = null!;
    private ProjectileManager _projectiles = null!;
    private ProjectileFactory _projectileFactory = null!;



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

        _itemAtlas = Content.Load<Texture2D>("zeldaitems");
        _projectileAtlas = Content.Load<Texture2D>("zelda");
        _bossAtlas = Content.Load<Texture2D>("boses");

        // Load the PixelPack block tiles
        _blockAtlas = Content.Load<Texture2D>("Textures/Blocks/PixelPack_Block_Atlas");

        _playerSheet = Content.Load<Texture2D>("Textures/Player/link");
        _player = new Player(new Vector2(100, 100), _playerSheet);

        RegisterResettable(_player);

        

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

        var enemyPosition = new Vector2(450, 260);
        var boss = new Boss(
            SpriteFactory.CreateEnemySprite(Content, "SorcererAttack", enemyPosition),
            enemyPosition);
        boss.FireRequested += SpawnBossProjectile;
        _enemies = new EnemyManager(new IEnemy[]
        {
            new EnemyA(SpriteFactory.CreateEnemySprite(Content, "Run", enemyPosition), enemyPosition),
            new EnemyB(SpriteFactory.CreateEnemySprite(Content, "DemonFlying", enemyPosition), enemyPosition),
            new EnemyC(SpriteFactory.CreateEnemySprite(Content, "ManaSeed", enemyPosition), enemyPosition),
            boss
        });
        RegisterResettable(_enemies);

        var itemPosition = new Vector2(700, 260);
        _items = new ItemManager(new List<IItem>
        {
            new Book(ItemSprites.CreateBook(_itemAtlas, itemPosition, 2f), itemPosition),
            new Key(ItemSprites.CreateKey(_itemAtlas, itemPosition, 2f), itemPosition),
            new Watch(ItemSprites.CreateWatch(_itemAtlas, itemPosition, 2f), itemPosition),
            new Heart(ItemSprites.CreateHeart(_itemAtlas, itemPosition, 2f), itemPosition),
            new Potion(ItemSprites.CreatePotion(_itemAtlas, itemPosition, 2f), itemPosition)
        });
        RegisterResettable(_items);

        _projectileFactory = new ProjectileFactory(_projectileAtlas, _bossAtlas);
        _projectiles = new ProjectileManager(_projectileFactory);
        RegisterResettable(_projectiles);

        _keyboardController = new KeyboardController(this, _player, _blockManager, _projectiles);
        _keyboardController.Initialize();
    }

    // Update every sprite once per frame
    protected override void Update(GameTime gameTime)
    {
        _blockManager.Update(gameTime);

        _enemies.Update(gameTime);

        _items.Update(gameTime);

        _projectiles.Update(gameTime);
        _projectiles.RemoveOffscreenBossProjectiles(GraphicsDevice.Viewport.Width);

        _keyboardController.Update();
        _player.Update(gameTime);
        Window.Title = $"Escape | Enemy {_enemies.SelectedIndex + 1}/4: {_enemies.Current.GetType().Name} | O/P: enemies | T/Y: blocks | U/I: items | R: reset | Q: quit";
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

        _items.Draw(_spriteBatch);

        _projectiles.Draw(_spriteBatch, gameTime);

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

    private void SpawnBossProjectile(Vector2 position)
    {
        _projectiles.SpawnBossProjectile(position, Vector2.UnitX);
    }

    public void CycleItems(int direction) => _items.Cycle(direction);

    protected override void UnloadContent()
    {
        // Content owns and disposes the textures loaded through Content.Load.
        _spriteBatch.Dispose();
        base.UnloadContent();
    }}
