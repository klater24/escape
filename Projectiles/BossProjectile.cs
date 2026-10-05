using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Projectiles;

public class BossProjectile : IProjectile
{

    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private Vector2 _direction;

    int direct;
    public Vector2 Position { get; set;}


    public BossProjectile(ISprite sprite,  Vector2 position, int direct)
    {
        _sprite = sprite;
        _initialPosition = position;
        Position = position;
        _sprite.Position = position;
        _direction = new Vector2(1, 0);
        this.direct = direct;
    }


    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        

    if (direct == 1)
        _direction = new Vector2(1, 0);
    else if (direct == 2)
        _direction = new Vector2(1, -1);
    else if (direct == 3)
        _direction = new Vector2(1, 1);

            _sprite.Position +=  _direction * deltaTime * 100;
        

    

        _sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        float deltaTime = (float)gameTime.TotalGameTime.TotalSeconds;

        
            _sprite.Draw(spriteBatch);
        
    }
    public void Reset()
    {
        Position = _initialPosition;
        _sprite.Position = _initialPosition;
        _direction = new Vector2(1, 0);
        direct = 1;
    }
}