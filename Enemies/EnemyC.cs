using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using escape.Collisions;
using System;

namespace escape.Enemies;

public class EnemyC : IEnemy, IDamageable, IEnvironmentEnemy
{

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private Vector2 _direction;
    private float _stateTimer;
    private readonly Random _random = new();
    private bool _isMoving;
    public Vector2 Position { get; set; }
    public IReadOnlyList<ISolidCollider> Obstacles { get; set; } = Array.Empty<ISolidCollider>();
    private const int HitboxOffsetX = 22;
    private const int HitboxOffsetY = 16;
    private const int HitboxWidth = 20;
    private const int HitboxHeight = 28;
    private readonly EnemyCombatState _combat = new(3);
    public int Health => _combat.Health;
    public bool IsDefeated => _combat.IsDefeated;
    public bool IsCollisionEnabled => !IsDefeated;
    public Collision Category => Collision.Enemy;
    public Rectangle Bounds => new((int)Position.X + HitboxOffsetX,
        (int)Position.Y + HitboxOffsetY, HitboxWidth, HitboxHeight);
    public void TakeDamage() => _combat.TakeDamage();

    private const int ScreenWidth = 960;
    private const int ScreenHeight = 540;
    private const int SpriteWidth = 64;
    private const int SpriteHeight = 64;
    private const float MovementSpeed = 100f;
    private const float MoveDuration = 1f;
    private const float PauseDuration = 0.75f;

    public EnemyC(ISprite sprite, Vector2 position)
    {
        _sprite = sprite;
        _initialPosition = position;
        Position = position;
        _sprite.Position = position;
        _direction = new Vector2(1, 0);
        _stateTimer = 0f;
        _isMoving = true;
    }

    public void Update(GameTime gameTime)
    {
        if (IsDefeated) return;
        _combat.Update(gameTime);
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_isMoving)
        {
            _direction = EnemyMovement.Move(this, _direction, MovementSpeed * deltaTime);
        }

        if (Position.X < 0)
        {
            Position = new Vector2(0, Position.Y);
            _direction = new Vector2(1, 0);
        }

        if (Position.X > ScreenWidth - SpriteWidth)
        {
            Position = new Vector2(ScreenWidth - SpriteWidth, Position.Y);
            _direction = new Vector2(-1,0);
        }

        if (Position.Y < 0)
        {
            Position = new Vector2(Position.X, 0);
            _direction = new Vector2(0, 1);
        }

        if (Position.Y > ScreenHeight - SpriteHeight)
        {
            Position = new Vector2(Position.X, ScreenHeight - SpriteHeight);
            _direction = new Vector2(0, -1);
        }

        _stateTimer += deltaTime;

        if (_isMoving && _stateTimer >= MoveDuration)
        {
            _isMoving = false;
            _stateTimer = 0f;
        }
        else if (!_isMoving && _stateTimer >= PauseDuration)
        {
            _isMoving = true;
            _stateTimer = 0f;
            ChangeDirection();
        }

        _sprite.Position = Position;

        _sprite.Update(gameTime);
    }

    private void ChangeDirection()
    {
        _direction = _random.Next(4) switch
        {
            0 => new Vector2(1, 0),
            1 => new Vector2(-1, 0),
            2 => new Vector2(0, 1),
            _ => new Vector2(0, -1)
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
        _stateTimer = 0f;
        _isMoving = true;
    }
}
