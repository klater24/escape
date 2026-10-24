using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using escape.Collisions;
using System;

namespace escape.Enemies;

public class EnemyB : IEnemy, IDamageable
{

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private Vector2 _direction;
    private float _directionTimer;
    private float _speed;
    private readonly Random _random = new();
    public Vector2 Position { get; set; }
    private const int HitboxOffsetX = 12;
    private const int HitboxOffsetY = 20;
    private const int HitboxWidth = 46;
    private const int HitboxHeight = 44;
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
    private const int SpriteWidth = 81;
    private const int SpriteHeight = 71;
    private const float InitialSpeed = 100f;
    private const int MinimumSpeed = 60;
    private const int MaximumSpeed = 140;
    private const float DirectionChangeInterval = 1f;

    public EnemyB(ISprite sprite, Vector2 position)
    {
        _sprite = sprite;
        _initialPosition = position;
        Position = position;
        _sprite.Position = position;
        _direction = new Vector2(1, 0);
        _directionTimer = 0f;
        _speed = InitialSpeed;
        
    }


    public void Update(GameTime gameTime)
    {
        if (IsDefeated) return;
        _combat.Update(gameTime);
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Position += _direction * _speed * deltaTime;

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

        _directionTimer += deltaTime;

        if (_directionTimer >= DirectionChangeInterval)
        {
            _directionTimer = 0f;
            ChangeDirection();
            _speed = _random.Next(MinimumSpeed, MaximumSpeed + 1);

        }
        _sprite.Position = Position;

        _sprite.Update(gameTime);
    }

    private void ChangeDirection()
    {
        _direction = _random.Next(8) switch
        {
            0 => new Vector2(1, 0),
            1 => new Vector2(-1, 0),
            2 => new Vector2(0, 1),
            3 => new Vector2(0, -1),
            4 => new Vector2(1, 1),
            5 => new Vector2(-1, 1),
            6 => new Vector2(1, -1),
            _ => new Vector2(-1, -1)
        };
        // Keep diagonal movement at the same speed as cardinal movement.
        _direction.Normalize();
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
        _directionTimer = 0f;
        _speed = InitialSpeed;
    }
}
