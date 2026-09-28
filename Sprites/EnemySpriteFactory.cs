using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace escape.Sprites;

// Owns the raw PNG textures and frame layouts of the supplied enemy strips.
public sealed class EnemySpriteFactory : IDisposable
{
    private readonly GraphicsDevice _graphics;
    private readonly List<Texture2D> _textures = new();

    public EnemySpriteFactory(GraphicsDevice graphics) => _graphics = graphics;

    public ISprite Create(string animation, Vector2 position)
    {
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
        var texture = Texture2D.FromStream(_graphics, stream);
        _textures.Add(texture);
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

    public void Dispose()
    {
        foreach (var texture in _textures) texture.Dispose();
        _textures.Clear();
    }
}
