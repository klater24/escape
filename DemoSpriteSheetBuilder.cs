using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace escape;

// Creates the colored block strip used by the startup demo
public static class DemoSpriteSheetBuilder
{
    private const int TileSize = 32;

    // Make ten colored squares for the block factory demo
    public static Texture2D CreateBlockAtlas(GraphicsDevice graphicsDevice)
    {
        const int tileCount = 10;
        var width = tileCount * TileSize;
        var texture = new Texture2D(graphicsDevice, width, TileSize);
        var pixels = new Color[width * TileSize];
        var colors = new[]
        {
            new Color(56, 162, 75),
            new Color(129, 84, 52),
            new Color(128, 128, 128),
            new Color(181, 57, 46),
            new Color(58, 120, 190),
            new Color(145, 104, 58),
            new Color(210, 183, 100),
            new Color(157, 218, 228),
            new Color(95, 99, 110),
            new Color(92, 121, 70)
        };

        for (int tileIndex = 0; tileIndex < tileCount; tileIndex++)
        {
            for (int y = 0; y < TileSize; y++)
            {
                for (int x = 0; x < TileSize; x++)
                {
                    pixels[(y * width) + (tileIndex * TileSize) + x] = colors[tileIndex];
                }
            }
        }

        texture.SetData(pixels);
        return texture;
    }
}
