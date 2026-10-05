using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Items;

public class Potion : IItem
{
    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private const float BobHeight = 6f;
    private const float BobPeriodSeconds = 2f;
    private float _bobTime;

    public Vector2 Position { get; set; }

    public Potion(ISprite sprite, Vector2 position)
    {
        _sprite = sprite;
        _initialPosition = position;
        Position = position;
        _sprite.Position = position;
    }

    public void Update(GameTime gameTime)
    {
        _bobTime = (_bobTime + (float)gameTime.ElapsedGameTime.TotalSeconds) % BobPeriodSeconds;
        float offset = MathF.Sin(_bobTime / BobPeriodSeconds * MathHelper.TwoPi) * BobHeight;
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
    }
}