using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using System;
using escape.Collisions;

namespace escape.Enemies;

public class EnemyA : IEnemy, ICollidable
{

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private Vector2 _direction;
    private float _directionTimer;
    private readonly Random _random = new();
    public Vector2 Position { get; set; }
    private const int ScreenWidth = 960;
    private const int ScreenHeight = 540;
    private const int SpriteWidth = 96;
    private const int SpriteHeight = 64;
    private const float MovementSpeed = 100f;
    private const float DirectionChangeInterval = 2f;
    private const int HitboxOffsetX = 40;
    private const int HitboxOffsetY = 14;
    private const int HitboxWidth = 24;
    private const int HitboxHeight = 32;

    public Rectangle Bounds => new Rectangle(
        (int)Position.X + HitboxOffsetX,
        (int)Position.Y + HitboxOffsetY,
        HitboxWidth,
        HitboxHeight
    );

    public Collision Category => Collision.Enemy;

    private const int StartingHealth = 3;
    private const float DamageFlashDuration = 0.4f;
    private const float FlashInterval = 0.08f;
    private float _damageFlashTimer;
    public int Health { get; private set; } = StartingHealth;
    public bool IsDefeated => Health == 0;
    public bool IsCollisionEnabled => !IsDefeated;

    public void TakeDamage()
    {
        if (IsDefeated)
            return;
        Health--;
        _damageFlashTimer = DamageFlashDuration;
    }

    public EnemyA(ISprite sprite, Vector2 position)
    {
        _sprite = sprite;
        _initialPosition = position;
        Position = position;
        _sprite.Position = position;
        _direction = new Vector2(1, 0);
        _directionTimer = 0f;
    }


    public void Update(GameTime gameTime)
    {
        if (IsDefeated)
            return;
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _damageFlashTimer = MathF.Max(0f, _damageFlashTimer - deltaTime);

        Position += _direction * MovementSpeed * deltaTime;

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
        if (IsDefeated || (_damageFlashTimer > 0f &&
            (int)(_damageFlashTimer / FlashInterval) % 2 == 0))
            return;
        _sprite.Draw(spriteBatch);
    }
    public void Reset()
    {
        Health = StartingHealth;
        _damageFlashTimer = 0f;
        Position = _initialPosition;
        _sprite.Reset();
        _sprite.Position = Position;
        _direction = new Vector2(1, 0);
        _directionTimer = 0f;
    }
}
