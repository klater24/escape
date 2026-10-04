using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Items;

public class Potion : IItem
{

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private Vector2 _direction;
    public Vector2 Position { get; set;}


    public Potion(ISprite sprite,  Vector2 position)
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
        
  Position +=  _direction * deltaTime * 10;
          if (Position.Y < 0)
        {
            Position = new Vector2(Position.X, 0);
            _direction = new Vector2(0, 1);
            
        }

        if (Position.Y > 10)
        {
            Position = new Vector2(Position.X, 10);
            _direction = new Vector2(0, -1);
            
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
       // _sprite.Reset();
        _sprite.Position = Position;
        _direction = new Vector2(1, 0);
    }
}