using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Enemies;

public class Boss : IEnemy
{

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private readonly float _leftBoundary;
    private readonly float _rightBoundary;
    private Vector2 _direction;
    public Vector2 Position { get; set; }
    private float _speed;
    private float _fireTimer;

    public Boss(ISprite sprite, Vector2 position)
    {
        _sprite = sprite;
        _initialPosition = position;
        Position = position;
        _sprite.Position = position;
        _direction = new Vector2(1, 0);
        _speed = 75f;
        _fireTimer = 0f;
        _leftBoundary = position.X - 150f;
        _rightBoundary = position.X + 150f;
    }


    public void Update(GameTime gameTime)
    {
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
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch);
    }
    public void Reset()
    {
        Position = _initialPosition;
        _sprite.Reset();
        _sprite.Position = Position;
        _direction = new Vector2(1, 0);
        _speed = 75f;
        _fireTimer = 0f;
    }
}