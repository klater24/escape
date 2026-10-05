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
    private float _timer = 0f;


    public Arrow(ISprite sprite, Vector2 position, Vector2 direction)
    {
        _sprite = sprite;
        _initialPosition = position;
        Position = position;
        _sprite.Position = position;
        _direction = ProjectileFactory.NormalizeDirection(direction);
        
    }

    public void Shoot(Vector2 position)
    {
        Position = position;
        _sprite.Position = position;
        _timer = 0f;
    }


    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _timer += deltaTime;

        if (_timer < 2f)
        {
            Position += _direction * movementSeconds * 100;
        }

        _sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        float deltaTime = (float)gameTime.TotalGameTime.TotalSeconds;

        if (_timer  < 3f) {
            _sprite.Draw(spriteBatch);
        }
    }
    public void Reset()
    {
        Position = _initialPosition;
        _ageSeconds = 0f;
        _sprite.Reset();
        _sprite.Position = Position;
        _direction = new Vector2(1, 0);
        _timer = 0f;
    }
}
