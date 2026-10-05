using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Projectiles;

public class BossProjectile : IProjectile
{

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private readonly Vector2 _direction;
    private float _ageSeconds;
    private const float LifetimeSeconds = 6f;
    public bool IsActive => _ageSeconds < LifetimeSeconds;

    public Vector2 Position { get; set;}


    public BossProjectile(ISprite sprite, Vector2 position, Vector2 direction)
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
        float movementSeconds = MathHelper.Clamp(LifetimeSeconds - _ageSeconds, 0f, deltaTime);
        _ageSeconds += deltaTime;


        Position += _direction * movementSeconds * 100;

        _sprite.Position = Position;
        _sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (IsActive)
            _sprite.Draw(spriteBatch);

    }
    public void Reset()
    {
        Position = _initialPosition;
        _ageSeconds = 0f;
        _sprite.Reset();
        _sprite.Position = _initialPosition;
    }
}
