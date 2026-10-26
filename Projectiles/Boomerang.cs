using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using escape.Collisions;

namespace escape.Projectiles;

public class Boomerang : ICombatProjectile
{
    private const int HitboxWidth = 16;
    private const int HitboxHeight = 32;
    private readonly ProjectileHitTracker _hits = new();
    public bool HitsPlayer => false;
    public Collision Category => Collision.PlayerProjectile;
    public bool IsCollisionEnabled => IsActive;
    public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, HitboxWidth, HitboxHeight);
    public bool TryHit(ICollidable target)
    {
        if (!_hits.TryHit(this, target)) return false;

        return true;
    }

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private readonly Vector2 _direction;
    private float _ageSeconds;
    private float _returnAt = 3f;
    private bool _finished;
    public bool IsActive => !_finished && _ageSeconds < 2f * _returnAt;
    public bool IsReturning => IsActive && _ageSeconds >= _returnAt;
    public void HitWall()
    {
        if (IsReturning) _finished = true;
        else _returnAt = _ageSeconds;
    }
    public Vector2 Position { get; set;}

    public Boomerang(ISprite sprite, Vector2 position, Vector2 direction)
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

        // Split an update that crosses the turnaround or end time.
        float nextAge = _ageSeconds + deltaTime;
        float outboundSeconds = MathHelper.Clamp(nextAge, 0f, _returnAt)
            - MathHelper.Clamp(_ageSeconds, 0f, _returnAt);
        float returnSeconds = MathHelper.Clamp(nextAge - _returnAt, 0f, _returnAt)
            - MathHelper.Clamp(_ageSeconds - _returnAt, 0f, _returnAt);
        Position += _direction * (outboundSeconds - returnSeconds) * 50;
        _ageSeconds = nextAge;
        if (!IsActive) Position = _initialPosition;

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
        Position = _initialPosition;
        _ageSeconds = 0f;
        _returnAt = 3f;
        _finished = false;
        _sprite.Reset();
        _sprite.Position = Position;

    }
}
