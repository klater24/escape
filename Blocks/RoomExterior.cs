using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace escape.Levels;

public class RoomExterior
{
    private readonly Texture2D _texture;
    private readonly Vector2 _position;
    private readonly float _scale;

    public RoomExterior(Texture2D texture, Vector2 position, float scale = 2f)
    {
        _texture = texture;
        _position = position;
        _scale = scale;
    }

public void DrawOpening(
    SpriteBatch spriteBatch,
    Texture2D blockAtlas,
    bool hasDoor,
    string direction)
{
    Rectangle source = direction switch
    {
        "up" => hasDoor
            ? new Rectangle(320, 0, 32, 32)
            : new Rectangle(448, 0, 32, 32),

        "left" => hasDoor
            ? new Rectangle(352, 0, 32, 32)
            : new Rectangle(480, 0, 32, 32),

        "right" => hasDoor
            ? new Rectangle(384, 0, 32, 32)
            : new Rectangle(512, 0, 32, 32),

        "down" => hasDoor
            ? new Rectangle(416, 0, 32, 32)
            : new Rectangle(544, 0, 32, 32),

        _ => throw new ArgumentException(
            $"Unknown direction: {direction}")
    };

    Vector2 markerPosition = direction switch
    {
        "up" => new Vector2(112, 0),
        "left" => new Vector2(0, 72),
        "right" => new Vector2(224, 72),
        "down" => new Vector2(112, 144),
        _ => throw new ArgumentException(
            $"Unknown direction: {direction}")
    };

    Vector2 drawPosition = _position + markerPosition * _scale;

    spriteBatch.Draw(blockAtlas, drawPosition, source, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
}


    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, _position, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
    }
}
