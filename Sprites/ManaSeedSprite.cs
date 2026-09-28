using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Sprites;

// Page 1 uses 64x64 cells: standing rows 0-3 and walking rows 4-7.
// Layers are drawn body, outfit, then hair using the same animation frame.
public sealed class ManaSeedSprite : ISprite
{
    private readonly Texture2D[] _layers;
    private readonly Vector2 _initialPosition;
    private Vector2 _previousPosition;
    private int _directionRow = 1;
    private int _frame;
    private float _elapsed;
    private bool _walking;
    public Vector2 Position { get; set; }

    public ManaSeedSprite(Texture2D[] layers, Vector2 position)
    {
        _layers = layers;
        _initialPosition = position;
        Position = position;
        _previousPosition = position;
    }

    public void Update(GameTime gameTime)
    {
        Vector2 movement = Position - _previousPosition;
        _previousPosition = Position;
        _walking = movement.LengthSquared() > 0f;
        if (!_walking)
        {
            _frame = 0;
            _elapsed = 0f;
            return;
        }
        int row = movement.X > 0 ? 1 : movement.X < 0 ? 3 : movement.Y > 0 ? 2 : 0;
        if (row != _directionRow)
        {
            _frame = 0;
            _elapsed = 0f;
        }
        _directionRow = row;
        _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        while (_elapsed >= 0.135f)
        {
            _elapsed -= 0.135f;
            _frame = (_frame + 1) % 6;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int row = _directionRow + (_walking ? 4 : 0);
        var source = new Rectangle((_walking ? _frame : 0) * 64, row * 64, 64, 64);
        foreach (var layer in _layers)
            spriteBatch.Draw(layer, Position, source, Color.White);
    }

    public void Reset()
    {
        Position = _initialPosition;
        _previousPosition = Position;
        _directionRow = 1;
        _frame = 0;
        _elapsed = 0f;
        _walking = false;
    }
}
