using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using System;
using System.Collections.Generic;
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
    // The caller owns loaded textures and disposes them when game content unloads.
    public static ISprite CreateEnemySprite(GraphicsDevice graphics, ICollection<Texture2D> ownedTextures, string animation, Vector2 position)
    {
        if (animation == "ManaSeed")
        {
            var layers = new Texture2D[3];
            string[] names = { "ManaSeedBody.png", "ManaSeedOutfit.png", "ManaSeedHair.png" };
            for (int i = 0; i < names.Length; i++)
            {
                string layerPath = Path.Combine(AppContext.BaseDirectory, "Content", "Textures", "Enemies", names[i]);
                using var layerStream = File.OpenRead(layerPath);
                layers[i] = Texture2D.FromStream(graphics, layerStream);
                ownedTextures.Add(layers[i]);
                if (layers[i].Width != 512 || layers[i].Height != 512)
                    throw new InvalidDataException($"Expected a 512x512 Mana Seed sheet: {layerPath}");
            }
            return new ManaSeedSprite(layers, position);
        }
        var (fileName, frameWidth, frameHeight, frameCount, frameSeconds) = animation switch
        {
            "Run" => ("noBKG_KnightRun_strip.png", 96, 64, 8, 0.10f),
            "Roll" => ("noBKG_KnightRoll_strip.png", 180, 64, 15, 0.07f),
            "Shield" => ("noBKG_KnightShield_strip.png", 96, 64, 7, 0.15f),
            "Attack" => ("noBKG_KnightAttack_strip.png", 144, 64, 22, 0.10f),
            "DemonFlying" => ("DemonFlying.png", 81, 71, 4, 0.12f),
            "SorcererAttack" => ("SorcererAttack.png", 200, 200, 10, 0.15f),
            _ => throw new ArgumentException("Unknown enemy animation.", nameof(animation))
        };
        string path = Path.Combine(AppContext.BaseDirectory, "Content", "Textures", "Enemies",
            fileName);
        using var stream = File.OpenRead(path);
        var texture = Texture2D.FromStream(graphics, stream);
        ownedTextures.Add(texture);
        if (texture.Width != frameWidth * frameCount || texture.Height != frameHeight)
            throw new InvalidDataException($"Unexpected sprite strip dimensions: {path}");
        // The sorcerer sheet has an opaque, flat background. Color-key it at load
        // time so the original asset stays intact and the game background shows.
        if (animation == "SorcererAttack")
        {
            var pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);
            var background = new Color(47, 72, 78);
            for (int i = 0; i < pixels.Length; i++)
                if (pixels[i] == background) pixels[i] = Color.Transparent;
            texture.SetData(pixels);
        }
        var frames = new Rectangle[frameCount];
        for (int i = 0; i < frameCount; i++)
            frames[i] = new Rectangle(i * frameWidth, 0, frameWidth, frameHeight);
        return SpriteFactory.CreateAnimatedSprite(texture, frames, position, frameSeconds);
    }

}