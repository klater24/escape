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
                new Rectangle(10 + (i * 27), 10, 24, 24),
                source,
                Color.White
            );
        }
    }

    public void Draw(SpriteBatch spriteBatch, int rupees)
    {
        spriteBatch.Draw(
            _hudAtlas,
            new Rectangle(0, 0, 768, 168),
            _hudBackground,
            Color.White
        );
        DrawHearts(spriteBatch);

        string hudText =
            $"Rupees: {rupees}\n" +
            $"Keys: {_inventory.GetQuantity(ItemType.Key)}";

        spriteBatch.DrawString(
            _font,
            hudText,
            new Vector2(10, 10),
            Color.White
        );
    }
}
