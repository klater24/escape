using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Sprites;

// A sprite that always draws one part of a texture
public class StaticSprite : ISprite
{
    private readonly Texture2D _texture;
    private readonly Rectangle _sourceRectangle;
    private readonly float _scale;
    private readonly Vector2 _initialPosition;

    public Vector2 Position { get; set; }

    // Save the texture area and starting position
    public StaticSprite(Texture2D texture, Rectangle sourceRectangle, Vector2 position, float scale = 1f)
    {
        _texture = texture;
        _sourceRectangle = sourceRectangle;
        _scale = scale;
        _initialPosition = position;
        Position = position;
    }

    // Static sprites do not change during updates
    public void Update(GameTime gameTime)
    {
    }

    // Draw the saved texture area
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _texture,
            Position,
            _sourceRectangle,
            Color.White,
            0f,
            Vector2.Zero,
            _scale,
            SpriteEffects.None,
            0f);
    }

    // Move back to the starting position
    public void Reset()
    {
        Position = _initialPosition;
    }
}
