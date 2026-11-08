using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Items;

namespace escape.UI;

// Self-contained HUD. The source archive does not include the old HUD atlas or font.
public sealed class HUD : IDisposable
{
    private readonly Player _player;
    private readonly Inventory _inventory;
    private readonly Texture2D _pixel;
    private static readonly string[] Digits =
    {
        "111101101101111", "010110010010111", "111001111100111",
        "111001111001111", "101101111001001", "111100111001111",
        "111100111101111", "111001001001001", "111101111101111",
        "111101111001111"
    };

    public HUD(Player player, Inventory inventory, GraphicsDevice graphics)
    {
        _player = player;
        _inventory = inventory;
        _pixel = new Texture2D(graphics, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    private void Rect(SpriteBatch batch, int x, int y, int w, int h, Color color)
        => batch.Draw(_pixel, new Rectangle(x, y, w, h), color);

    private void Number(SpriteBatch batch, int value, int x, int y)
    {
        foreach (char c in Math.Max(0, value).ToString())
        {
            string bitmap = Digits[c - '0'];
            for (int row = 0; row < 5; row++)
                for (int col = 0; col < 3; col++)
                    if (bitmap[row * 3 + col] == '1')
                        Rect(batch, x + col * 4, y + row * 4, 4, 4, Color.White);
            x += 16;
        }
    }

    public void Draw(SpriteBatch batch, int rupees)
    {
        Rect(batch, 0, 0, 768, 104, new Color(18, 28, 50, 235));
        Rect(batch, 0, 101, 768, 3, Color.Goldenrod);
        // Health hearts (one heart for each whole HP).
        for (int i = 0; i < _player.MaxHealth; i++)
        {
            int x = 480 + (i % 10) * 26;
            int y = 18 + (i / 10) * 26;
            Color heart = i < _player.Health ? Color.Red : Color.DarkRed;
            Rect(batch, x + 3, y, 7, 7, heart);
            Rect(batch, x + 13, y, 7, 7, heart);
            Rect(batch, x, y + 6, 23, 8, heart);
            Rect(batch, x + 4, y + 14, 15, 5, heart);
            Rect(batch, x + 8, y + 19, 7, 4, heart);
        }
        // Colored icons distinguish rupees (green), keys (gold), bombs (gray).
        Rect(batch, 24, 20, 18, 18, Color.LimeGreen);
        Number(batch, rupees, 54, 20);
        Rect(batch, 170, 20, 18, 18, Color.Gold);
        Number(batch, _inventory.GetQuantity(ItemType.Key), 200, 20);
        Rect(batch, 320, 20, 18, 18, Color.Gray);
        Number(batch, _inventory.GetQuantity(ItemType.Bomb), 350, 20);
    }

    public void Dispose() => _pixel.Dispose();
}
