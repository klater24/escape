using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Projectiles;

public class Arrow : IProjectile
{

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private readonly Vector2 _direction;
    private float _ageSeconds;
    public bool IsActive => _ageSeconds < 3f;
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
        Position = _initialPosition;
        _ageSeconds = 0f;
        _sprite.Reset();
        _sprite.Position = Position;
    }
}
