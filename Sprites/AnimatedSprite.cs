using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Sprites;

// A sprite that moves through a list of frames
public class AnimatedSprite : ISprite
{
    private readonly Texture2D _texture;
    private readonly Rectangle[] _frames;
    private readonly float _frameSeconds;
    private readonly Vector2 _initialPosition;
    private readonly float _scale;
    private SpriteEffects _spriteEffects = SpriteEffects.None;

    private int _currentFrame;
    private float _elapsed;

    public Vector2 Position { get; set; }
    public Vector2 FrameAnchor { get; set; }
    public Vector2 AnchorBodySize { get; set; }

    // Set up the animation and remember where it starts
    public AnimatedSprite(Texture2D texture, Rectangle[] frames, Vector2 position, float frameSeconds = 0.12f, float scale = 3f)
    {
        _texture = texture;
        _frames = frames;
        _frameSeconds = frameSeconds;
        _initialPosition = position;
        _scale = scale;
        Position = position;
        _currentFrame = 0;
        _elapsed = 0f;
    }

    // Move to the next frame when enough time has passed
    public void Update(GameTime gameTime)
    {
        _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

        while (_elapsed >= _frameSeconds)
        {
            _elapsed -= _frameSeconds;
            _currentFrame = (_currentFrame + 1) % _frames.Length;
        }
    }

    // Draw the current frame
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _texture,
            Position - new Vector2(
                MathHelper.Max(0, _frames[_currentFrame].Width - AnchorBodySize.X) * FrameAnchor.X,
                MathHelper.Max(0, _frames[_currentFrame].Height - AnchorBodySize.Y) * FrameAnchor.Y) * _scale,
            _frames[_currentFrame],
            Color.White,
            0f,
            Vector2.Zero,
            _scale,
            _spriteEffects,
            0f);
    }

    public void SetSpriteEffects(SpriteEffects effects)
    {
        _spriteEffects = effects;
    }

    // Start the animation over and return to the starting position
    public void Reset()
    {
        Position = _initialPosition;
        _currentFrame = 0;
        _elapsed = 0f;
    }
}
