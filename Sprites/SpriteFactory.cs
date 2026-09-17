using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Sprites;

// One place for creating the different sprite types
public static class SpriteFactory
{
    // Create a sprite that uses one texture area
    public static ISprite CreateStaticSprite(Texture2D texture, Rectangle sourceRectangle, Vector2 position, float scale = 1f)
    {
        return new StaticSprite(texture, sourceRectangle, position, scale);
    }

    // Create a sprite that plays through several frames
    public static ISprite CreateAnimatedSprite(Texture2D texture, Rectangle[] frames, Vector2 position, float frameSeconds = 0.12f, float scale = 1f)
    {
        return new AnimatedSprite(texture, frames, position, frameSeconds, scale);
    }

    // Pick the texture area for a named block
    public static ISprite CreateBlockSprite(Texture2D atlas, string blockType, Vector2 position, float scale = 1f)
    {
        var source = blockType.ToLowerInvariant() switch
        {
            "grass" => new Rectangle(0, 0, 32, 32),
            "dirt" => new Rectangle(32, 0, 32, 32),
            "stone" => new Rectangle(64, 0, 32, 32),
            "brick" => new Rectangle(96, 0, 32, 32),
            "water" => new Rectangle(128, 0, 32, 32),
            "wood" => new Rectangle(160, 0, 32, 32),
            "sand" => new Rectangle(192, 0, 32, 32),
            "ice" => new Rectangle(224, 0, 32, 32),
            "metal" => new Rectangle(256, 0, 32, 32),
            "platform" => new Rectangle(288, 0, 32, 32),
            _ => new Rectangle(0, 0, 32, 32)
        };

        return new StaticSprite(atlas, source, position, scale);
    }

    // Create a basic animated sprite for a player
    public static ISprite CreatePlayerSprite(Texture2D atlas, Vector2 position, float scale = 1f)
    {
        var frames = new[]
        {
            new Rectangle(0, 0, 32, 32),
            new Rectangle(32, 0, 32, 32),
            new Rectangle(64, 0, 32, 32)
        };

        return new AnimatedSprite(atlas, frames, position, 0.12f, scale);
    }
}