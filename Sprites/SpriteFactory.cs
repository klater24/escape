using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using System;
using Microsoft.Xna.Framework.Content;
using System.IO;

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

    // Pick the matching tile in PixelPack_Block_Atlas.png for a named block
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
    // ContentManager caches and owns all loaded textures.
    public static ISprite CreateEnemySprite(ContentManager content, string animation, Vector2 position)
    {
        if (animation == "ManaSeed")
        {
            var layers = new Texture2D[3];
            string[] names = { "ManaSeedBody", "ManaSeedOutfit", "ManaSeedHair" };
            for (int i = 0; i < names.Length; i++)
            {
                string layerPath = $"Textures/Enemies/{names[i]}";
                layers[i] = content.Load<Texture2D>(layerPath);
                if (layers[i].Width != 512 || layers[i].Height != 512)
                    throw new InvalidDataException($"Expected a 512x512 Mana Seed sheet: {layerPath}");
            }
            return new ManaSeedSprite(layers, position);
        }
        var (fileName, frameWidth, frameHeight, frameCount, frameSeconds) = animation switch
        {
            "Run" => ("noBKG_KnightRun_strip", 96, 64, 8, 0.10f),
            "Roll" => ("noBKG_KnightRoll_strip", 180, 64, 15, 0.07f),
            "Shield" => ("noBKG_KnightShield_strip", 96, 64, 7, 0.15f),
            "Attack" => ("noBKG_KnightAttack_strip", 144, 64, 22, 0.10f),
            "DemonFlying" => ("DemonFlying", 81, 71, 4, 0.12f),
            "SorcererAttack" => ("SorcererAttack", 200, 200, 10, 0.15f),
            _ => throw new ArgumentException("Unknown enemy animation.", nameof(animation))
        };
        string path = $"Textures/Enemies/{fileName}";
        var texture = content.Load<Texture2D>(path);
        if (texture.Width != frameWidth * frameCount || texture.Height != frameHeight)
            throw new InvalidDataException($"Unexpected sprite strip dimensions: {path}");
        var frames = new Rectangle[frameCount];
        for (int i = 0; i < frameCount; i++)
            frames[i] = new Rectangle(i * frameWidth, 0, frameWidth, frameHeight);
        return SpriteFactory.CreateAnimatedSprite(texture, frames, position, frameSeconds);
    }

}