using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Items;

namespace escape.UI;

public class HUD
{
    private readonly Player _player;
    private readonly Inventory _inventory;
    private readonly SpriteFont _font;
    private readonly Texture2D _itemAtlas;
    private readonly Texture2D _hudAtlas;
    private readonly Rectangle _hudBackground = new Rectangle(258, 11, 256, 56);
    private readonly Rectangle _swordSprite = new Rectangle(555, 137, 8, 16);
    Rectangle emptyHeart = new Rectangle(627, 117, 8, 8);
    Rectangle halfHeart  = new Rectangle(636, 117, 8, 8);
    Rectangle fullHeart  = new Rectangle(645, 117, 8, 8);

    public HUD(Player player, Inventory inventory, SpriteFont font, Texture2D itemAtlas, Texture2D hudAtlas)
    {
        _player = player;
        _inventory = inventory;
        _font = font;
        _itemAtlas = itemAtlas;
        _hudAtlas = hudAtlas;
    }

    private void DrawHudText(SpriteBatch spriteBatch, string text, Vector2 position)
    {
        foreach (char character in text)
        {
            int characterIndex = character switch
            {
                'X' => 0,
                '0' => 1,
                '1' => 2,
                '2' => 3,
                '3' => 4,
                '4' => 5,
                '5' => 6,
                '6' => 7,
                '7' => 8,
                '8' => 9,
                '9' => 10,
                'A' => 11,
                _ => -1
            };

            if (characterIndex >= 0)
            {
                Rectangle source = new Rectangle(
                    519 + characterIndex * 9,
                    117,
                    8,
                    8
                );

                spriteBatch.Draw(
                    _hudAtlas,
                    new Rectangle((int)position.X, (int)position.Y, 24, 24),
                    source,
                    Color.White
                );
            }

            position.X += 24;
        }
    }

    private void DrawHudItem(SpriteBatch spriteBatch, Rectangle source, Vector2 position)
    {
        spriteBatch.Draw(
            _hudAtlas,
            new Rectangle(
                (int)position.X,
                (int)position.Y,
                source.Width * 3,
                source.Height * 3
            ),
            source,
            Color.White
        );
    }

    private void DrawHearts(SpriteBatch spriteBatch)
    {
        Rectangle fullHeart = new Rectangle(645, 117, 8, 8);
        Rectangle halfHeart = new Rectangle(636, 117, 8, 8);
        Rectangle emptyHeart = new Rectangle(627, 117, 8, 8);

        for (int i = 0; i < _player.MaxHealth; i++)
        {
            Rectangle source;

            if (_player.Health >= i + 1)
                source = fullHeart;
            else if (_player.Health >= i + 0.5f)
                source = halfHeart;
            else
                source = emptyHeart;

            spriteBatch.Draw(
                _hudAtlas,
                new Rectangle(528 + (i * 24), 96, 24, 24),
                source,
                Color.White
            );
        }
    }

    public void Draw(SpriteBatch spriteBatch, int rupees)
    {
        // Draw HUD background
        spriteBatch.Draw(
            _hudAtlas,
            new Rectangle(0, 0, 768, 168),
            _hudBackground,
            Color.White
        );

        // Draw player health
        DrawHearts(spriteBatch);
        DrawHudItem(spriteBatch, _swordSprite, new Vector2(456, 72));

        // Draw resource counts
        // Rupees
        DrawHudText(spriteBatch, $"X{rupees}", new Vector2(288, 48));

        // Keys
        DrawHudText(spriteBatch,
            $"X{_inventory.GetQuantity(ItemType.Key)}",
            new Vector2(288, 96));

        // Bombs
        DrawHudText(spriteBatch,
            $"X{_inventory.GetQuantity(ItemType.Bomb)}",
            new Vector2(288, 120));
    }
}
