using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Projectiles;

public enum BombState { Fuse, Exploding, Finished }

public class Bomb : IProjectile
{
    private readonly ISprite _sprite;
    private readonly ISprite _explosionSprite;
    private readonly Vector2 _initialPosition;
    private float _ageSeconds;
    private const float FuseSeconds = 2f;
    private const float ExplosionSeconds = 1f;
    public BombState State { get; private set; } = BombState.Fuse;
    public bool IsActive => State != BombState.Finished;
    public Vector2 Position { get; set;}

    public Bomb(ISprite sprite, ISprite explosionSprite, Vector2 position)
    {
        _sprite = sprite;
        _explosionSprite = explosionSprite;
        _initialPosition = position;
        Position = position;
        _sprite.Position = position;
        _explosionSprite.Position = position;
    }

    public void Update(GameTime gameTime)
    {
        if (!IsActive) 
            return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _ageSeconds += deltaTime;
        State = _ageSeconds >= FuseSeconds + ExplosionSeconds ? BombState.Finished
            : _ageSeconds >= FuseSeconds ? BombState.Exploding : BombState.Fuse;
        _sprite.Position = Position;
        _explosionSprite.Position = Position;

        if (State == BombState.Fuse) 
            _sprite.Update(gameTime);
        else if (State == BombState.Exploding)
             _explosionSprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (State == BombState.Fuse) 
            _sprite.Draw(spriteBatch);
        else if (State == BombState.Exploding) 
            _explosionSprite.Draw(spriteBatch);
    }

    public void Reset()
    {
        Position = _initialPosition;
        _ageSeconds = 0f;
        _sprite.Reset();
        _explosionSprite.Reset();
        _sprite.Position = Position;
        _explosionSprite.Position = Position;
        State = BombState.Fuse;
    }
}