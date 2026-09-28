using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using System;

namespace escape.Enemies;

public class EnemyA : IEnemy
{

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private Vector2 _direction;
    private float _directionTimer;
    private readonly Random _random = new();
    public Vector2 Position { get; set; }

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
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float speed = 100f;

        Position += _direction * speed * deltaTime;

        if (Position.X < 0)
        {
            Position = new Vector2(0, Position.Y);
            _direction = new Vector2(1, 0);
        }

        if (Position.X > 960 - 96)
        {
            Position = new Vector2(960 - 96, Position.Y);
            _direction = new Vector2(-1,0);
        }

        if (Position.Y < 0)
        {
            Position = new Vector2(Position.X, 0);
            _direction = new Vector2(0, 1);
        }

        if (Position.Y > 540 - 64)
        {
            Position = new Vector2(Position.X, 540 - 64);
            _direction = new Vector2(0, -1);
        }

        _directionTimer += deltaTime;

        if (_directionTimer >= 2f)
        {
            _directionTimer = 0f;
            int directionChoice = _random.Next(4);

            if (directionChoice == 0)
            {
                _direction = new Vector2(1, 0);
            }

            else if (directionChoice == 1)
            {
                _direction = new Vector2(-1,0);
            }
            else if (directionChoice == 2)
            {
                _direction = new Vector2(0,1);
            }
            else
            {
                _direction = new Vector2(0, -1);
            }

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
        _directionTimer = 0f;
    }
}
