using Microsoft.Xna.Framework.Graphics;

namespace escape.Sprites;

// Loads PNG files copied beside the game output
public static class TextureLoader
{
    public static Texture2D Load(GraphicsDevice graphicsDevice, string relativePath)
    {
        if (Path.IsPathRooted(relativePath))
        {
            throw new ArgumentException("Use a path relative to the project assets", nameof(relativePath));
        }

        var fullPath = Path.Combine(AppContext.BaseDirectory, relativePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Texture was not found: {fullPath}", fullPath);
        }

        using var stream = File.OpenRead(fullPath);
        return Texture2D.FromStream(graphicsDevice, stream);
    }
}