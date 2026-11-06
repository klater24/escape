using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using escape.Collisions;

namespace escape.Enemies;

public class Boss : IEnemy, IDamageable, IEnvironmentEnemy
{

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private Vector2 _direction;
    private float _directionTimer;
    private readonly Random _random = new();
    public Vector2 Position { get; set; }
    public IReadOnlyList<ISolidCollider> Obstacles { get; set; } = Array.Empty<ISolidCollider>();
    private const int HitboxOffsetX = 64;
    private const int HitboxOffsetY = 55;
    private const int HitboxWidth = 55;
    private const int HitboxHeight = 129;
    public const int MaxHealth = 6;
    private readonly EnemyCombatState _combat = new(MaxHealth);
    public int Health => _combat.Health;
    public bool IsDefeated => _combat.IsDefeated;
    public bool IsCollisionEnabled => !IsDefeated;
    public Collision Category => Collision.Enemy;
    public Rectangle Bounds => new((int)Position.X + HitboxOffsetX,
        (int)Position.Y + HitboxOffsetY, HitboxWidth, HitboxHeight);
    public void TakeDamage() => _combat.TakeDamage();
    private float _speed;
    private float _fireTimer;

    private const float FireInterval = 2f;

    private const float MovementSpeed = 75f;
    private const float DirectionChangeInterval = 1.25f;
    private static readonly Vector2 ProjectileOffset = new(96f, 32f);

    // game1 handles the actual projectile
    public event System.Action<Vector2>? FireRequested;

    public Boss(ISprite sprite, Vector2 position)
    {
        _sprite = sprite;
        _initialPosition = position;
        Position = position;
        _sprite.Position = position;
        _direction = new Vector2(1, 0);
        _speed = MovementSpeed;
        _directionTimer = 0f;
        _fireTimer = 0f;
    }


    public void Update(GameTime gameTime)
    {
        if (IsDefeated) return;
        _combat.Update(gameTime);
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _directionTimer += deltaTime;
        if (_directionTimer >= DirectionChangeInterval)
        {
            _directionTimer %= DirectionChangeInterval;
            ChangeDirection();
        }

        _direction = EnemyMovement.Move(this, _direction, _speed * deltaTime);

        _sprite.Position = Position;

        _sprite.Update(gameTime);

        _fireTimer += deltaTime;

        if (_fireTimer >= FireInterval)
        {
            _fireTimer %= FireInterval;
            Vector2 projectilePosition = Position + ProjectileOffset;
            FireRequested?.Invoke(projectilePosition);
        }
    }

    private void ChangeDirection()
    {
        _direction = _random.Next(4) switch
        {
            0 => Vector2.UnitX,
            1 => -Vector2.UnitX,
            2 => Vector2.UnitY,
            _ => -Vector2.UnitY
        };
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_combat.IsVisible) return;
        _sprite.Draw(spriteBatch);
    }
    public void Reset()
    {
        _combat.Reset();
        Position = _initialPosition;
        _sprite.Reset();
        _sprite.Position = Position;
        _direction = new Vector2(1, 0);
        _speed = MovementSpeed;
        _directionTimer = 0f;
        _fireTimer = 0f;
    }
}
