using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Projectiles;

public class Boomerang : IProjectile
{

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private Vector2 _direction;
    public Vector2 Position { get; set;}

    public Boomerang(ISprite sprite, Vector2 position)
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
        

    if (gameTime.TotalGameTime.TotalSeconds < 3){
        _sprite.Position +=  _direction * deltaTime * 50;
    } else
        {
            _sprite.Position -=  _direction * deltaTime * 50;
        }

        _sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        float deltaTime = (float)gameTime.TotalGameTime.TotalSeconds;
        if (deltaTime < 6) {
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