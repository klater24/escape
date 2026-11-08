using escape.Collisions;
using escape.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace escape.Items;

public sealed class Triforce : IItem
{
    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private const int Size = 32;
    public Vector2 Position { get; set; }
    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size, Size);
    public Collision Category => Collision.Pickup;
    public bool IsCollisionEnabled => true;

    public Triforce(ISprite sprite, Vector2 position)
    {
        _sprite = sprite;
        _initialPosition = Position = position;
    }

    public void Update(GameTime gameTime)
    {
        _sprite.Position = Position;
        _sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch) => _sprite.Draw(spriteBatch);

    public void Reset()
    {
        Position = _initialPosition;
        _sprite.Reset();
        _sprite.Position = Position;
    }
}
