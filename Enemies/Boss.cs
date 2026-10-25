using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using escape.Collisions;

namespace escape.Enemies;

public class Boss : IEnemy, IDamageable
{

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private readonly float _leftBoundary;
    private readonly float _rightBoundary;
    private Vector2 _direction;
    public Vector2 Position { get; set; }
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
    private const float PatrolDistance = 150f;
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
        _fireTimer = 0f;
        _leftBoundary = position.X - PatrolDistance;
        _rightBoundary = position.X + PatrolDistance;
    }


    public void Update(GameTime gameTime)
    {
        if (IsDefeated) return;
        _combat.Update(gameTime);
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Position += _direction * _speed * deltaTime;

        if (Position.X <= _leftBoundary)
        {
            Position = new Vector2(_leftBoundary, Position.Y);
            _direction = new Vector2(1, 0);
        }

        if (Position.X >= _rightBoundary)
        {
            Position = new Vector2(_rightBoundary, Position.Y);
            _direction = new Vector2(-1, 0);
        }

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
        _fireTimer = 0f;
    }
}
