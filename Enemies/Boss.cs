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
        _sprite.Draw(spriteBatch);
    }
    public void Reset()
    {
        Position = _initialPosition;
        _sprite.Reset();
        _sprite.Position = Position;
        _direction = new Vector2(1, 0);
        _speed = MovementSpeed;
        _fireTimer = 0f;
    }
}