using escape.Interfaces;
using escape.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace escape.Items;

public sealed class InventoryPickup : IItem
{
    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private const float BobHeight = 6f;
    private const float BobPeriodSeconds = 2f;
    private float _bobTime;

    public ItemType ItemType { get; }

    public Vector2 Position { get; set; }

    public Rectangle Bounds =>
        new((int)Position.X, (int)Position.Y, 14, 30);

    public Collision Category => Collision.Pickup;

    public bool IsCollisionEnabled { get; private set; } = true;

    public InventoryPickup(ISprite sprite, Vector2 position, ItemType itemType)
    {
        _sprite = sprite;
        _initialPosition = position;
        Position = position;
        ItemType = itemType;
        _sprite.Position = position;
    }

    public void Update(GameTime gameTime)
    {
        _bobTime = (_bobTime +
            (float)gameTime.ElapsedGameTime.TotalSeconds) % BobPeriodSeconds;

        float offset = MathF.Sin(
            _bobTime / BobPeriodSeconds * MathHelper.TwoPi) * BobHeight;

        Position = _initialPosition + new Vector2(0f, offset);
        _sprite.Position = Position;
        _sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch);
    }

    public void Reset()
    {
        _bobTime = 0f;
        Position = _initialPosition;
        _sprite.Reset();
        _sprite.Position = Position;
        IsCollisionEnabled = true;
    }
}
