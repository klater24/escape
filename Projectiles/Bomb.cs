using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Projectiles;

public class Bomb : IProjectile
{

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private Vector2 _direction;
    public Vector2 Position { get; set;}

    public Bomb(ISprite sprite, Vector2 position)
    {
        _sprite = sprite;
        _initialPosition = position;
        Position = position;
        _sprite.Position = position;
        _direction = new Vector2(1, 0);
    }


    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;


        _sprite.Position = Position;

        _sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        float deltaTime = (float)gameTime.TotalGameTime.TotalSeconds;

        if(deltaTime < 2) {
            _sprite.Draw(spriteBatch);
        }
    }
    public void Reset()
    {
        Position = _initialPosition;
       // _sprite.Reset();
        _sprite.Position = Position;
        _direction = new Vector2(1, 0);
    }
}