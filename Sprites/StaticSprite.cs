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
    private readonly float _rotation;
    private readonly Vector2 _initialPosition;
    private SpriteEffects _spriteEffects = SpriteEffects.None;

    public Vector2 Position { get; set; }

    // Save the texture area and starting position
    public StaticSprite(Texture2D texture, Rectangle sourceRectangle, Vector2 position, float scale = 3f, float rotation = 0f)
    {
        _texture = texture;
        _sourceRectangle = sourceRectangle;
        _scale = scale;
        _rotation = rotation;
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
        // Rotate around the image center without shifting unrotated sprites.
        var origin = new Vector2(_sourceRectangle.Width / 2f, _sourceRectangle.Height / 2f);
        spriteBatch.Draw(
            _texture,
            Position + origin * _scale,
            _sourceRectangle,
            Color.White,
            _rotation,
            origin,
            _scale,
            _spriteEffects,
            0f);
    }

    public void SetSpriteEffects(SpriteEffects effects)
    {
        _spriteEffects = effects;
    }

    // Move back to the starting position
    public void Reset()
    {
        Position = _initialPosition;
    }
}
