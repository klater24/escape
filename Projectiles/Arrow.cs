using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using escape.Collisions;

namespace escape.Projectiles;

public class Arrow : ICombatProjectile
{
    private const int HitboxWidth = 16;
    private const int HitboxHeight = 32;
    private readonly ProjectileHitTracker _hits = new();
    public bool HitsPlayer => false;
    public Collision Category => Collision.PlayerProjectile;
    public bool IsCollisionEnabled => IsActive;
    public Rectangle Bounds => MathF.Abs(_direction.X) > MathF.Abs(_direction.Y)
        ? new Rectangle((int)Position.X - (HitboxHeight - HitboxWidth) / 2, (int)Position.Y + (HitboxHeight - HitboxWidth) / 2, HitboxHeight, HitboxWidth)
        : new Rectangle((int)Position.X, (int)Position.Y, HitboxWidth, HitboxHeight);
    public bool TryHit(ICollidable target)
    {
        if (!_hits.TryHit(this, target)) return false;
        _consumed = true;
        return true;
    }


    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private readonly Vector2 _direction;
    private bool _consumed;
    public void HitWall() => _consumed = true;
    private float _ageSeconds;
    public bool IsActive => !_consumed && _ageSeconds < 3f;
    public Vector2 Position { get; set;}


    public Arrow(ISprite sprite, Vector2 position, Vector2 direction)
    {
        _sprite = sprite;
        _initialPosition = position;
        Position = position;
        _sprite.Position = position;
        _direction = ProjectileFactory.NormalizeDirection(direction);

    }

    public void Update(GameTime gameTime)
    {
        if (!IsActive) return;
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Only count the part of this update before movement ends.
        float movementSeconds = MathHelper.Clamp(2f - _ageSeconds, 0f, deltaTime);
        _ageSeconds += deltaTime;
        if (movementSeconds > 0f)
        {
            Position += _direction * movementSeconds * 100;
        }

        _sprite.Position = Position;
        _sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (IsActive) {
            _sprite.Draw(spriteBatch);
        }
    }
    public void Reset()
    {
        _hits.Reset();
        _consumed = false;
        Position = _initialPosition;
        _ageSeconds = 0f;
        _sprite.Reset();
        _sprite.Position = Position;
    }
}
