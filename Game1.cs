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
    private Texture2D _itemAtlas;
    private Texture2D _projectileAtlas;
    private Texture2D _bossAtlas;
     private readonly List<ISprite> _sprites = new();
    private readonly List<IItem> _items = new();
    private  List<IProjectile> _projectiles = new();


    private readonly List<Texture2D> _enemyTextures = new();
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
        _blockAtlas = TextureLoader.Load(GraphicsDevice, "Content/Textures/Blocks/PixelPack_Block_Atlas.png");

        _playerSheet = Content.Load<Texture2D>("Textures/Player/link");
        _player = new Player(new Vector2(100, 100), _playerSheet);

        RegisterResettable(_player);

        _keyboardController = new KeyboardController(this, _player, _blockManager);
        _keyboardController.Initialize();

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
        _enemies = new EnemyManager(new IEnemy[]
        {
            new EnemyA(SpriteFactory.CreateEnemySprite(GraphicsDevice, _enemyTextures, "Run", enemyPosition), enemyPosition),
            new EnemyB(SpriteFactory.CreateEnemySprite(GraphicsDevice, _enemyTextures, "DemonFlying", enemyPosition), enemyPosition),
            new EnemyC(SpriteFactory.CreateEnemySprite(GraphicsDevice, _enemyTextures, "ManaSeed", enemyPosition), enemyPosition),
            new Boss(SpriteFactory.CreateEnemySprite(GraphicsDevice, _enemyTextures, "SorcererAttack", enemyPosition), enemyPosition)
        });
        RegisterResettable(_enemies);

        var bombSprite = ProjectileSprites.CreateBomb(_projectileAtlas, new Vector2(100, 100), 2f);
        _projectiles.Add(new Bomb(bombSprite, new Vector2(100, 100)));

        var arrowSprite = ProjectileSprites.CreateArrow(_projectileAtlas, new Vector2(200, 200), 2f);
        _projectiles.Add(new Arrow(arrowSprite, new Vector2(200, 200)));

        var boomerangSprite = ProjectileSprites.CreateBoomerang(_projectileAtlas, new Vector2(300, 300), 2f);
        _projectiles.Add(new Boomerang(boomerangSprite, new Vector2(300, 300)));

        var bossProjectileSprite = ProjectileSprites.CreateBossProjectile(_bossAtlas, new Vector2(400, 400), 2f);
        _projectiles.Add(new BossProjectile(bossProjectileSprite, new Vector2(400, 400)));

        var bookSprite = ItemSprites.CreateBook(_itemAtlas, new Vector2(500, 500), 2f);
        _items.Add(new Book(bookSprite, new Vector2(500, 500)));

        var keySprite = ItemSprites.CreateKey(_itemAtlas, new Vector2(600, 500), 2f);
        _items.Add(new Key(keySprite, new Vector2(600, 500)));

        var watchSprite = ItemSprites.CreateWatch(_itemAtlas, new Vector2(700, 500), 2f);
        _items.Add(new Watch(watchSprite, new Vector2(700, 500)));

        var heartSprite = ItemSprites.CreateHeart(_itemAtlas, new Vector2(200, 400), 2f);
        _items.Add(new Heart(heartSprite, new Vector2(200, 400)));

        var potionSprite = ItemSprites.CreatePotion(_itemAtlas, new Vector2(200, 300), 2f);
        _items.Add(new Potion(potionSprite, new Vector2(200, 300)));

    }

    // Update every sprite once per frame
    protected override void Update(GameTime gameTime)
    {
        _blockManager.Update(gameTime);

        _enemies.Update(gameTime);

        foreach(var item in _items)
        {
            item.Update(gameTime);
        }

        foreach (var projectile in _projectiles)
        {
            projectile.Update(gameTime);
        }

        _keyboardController.Update();
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

        foreach (var item in _items)
        {
            item.Draw(_spriteBatch);
        }

        foreach (var projectile in _projectiles)
        {
            projectile.Draw(_spriteBatch, gameTime);
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
    public void CycleEnemy(int direction) => _enemies.Cycle(direction);

    protected override void UnloadContent()
    {
        foreach (var texture in _enemyTextures) texture.Dispose();
        _enemyTextures.Clear();
        _blockAtlas.Dispose();
        _playerSheet.Dispose();
        _spriteBatch.Dispose();
        base.UnloadContent();
    }}
