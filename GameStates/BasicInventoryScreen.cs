using escape.Interfaces;
using escape.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace escape.GameStates;

public sealed class BasicInventoryScreen : IInventoryScreen
{
    private readonly IInventory _inventory;
    private readonly SpriteFont _font;
    private KeyboardState _previous;
    private int _selected;
    private string _selectionStatus = "";
    public ItemType SelectedItem => Equipment[_selected];
    private static readonly ItemType[] Equipment = { ItemType.Bomb, ItemType.Boomerang, ItemType.Bow, ItemType.Potion };
    private readonly Rectangle _inventoryBackground = new Rectangle(1, 11, 256, 88);
    private readonly Rectangle _triforceBackground = new Rectangle(1, 112, 256, 88);
    private readonly Rectangle _dungeonMapBackground = new Rectangle(258, 112, 256, 88);
    private enum InventoryLayout
    {
        Triforce,
        DungeonMap
    }
    private InventoryLayout _layout = InventoryLayout.DungeonMap;
    private readonly Texture2D _hudAtlas;
    public BasicInventoryScreen(IInventory inventory, SpriteFont font, Texture2D hudAtlas)
    {
        _inventory = inventory;
        _font = font;
        _hudAtlas = hudAtlas;
    }

    public void Update(GameTime gameTime) => Update(Keyboard.GetState());

    public void Update(KeyboardState keys)
    {
        // Left / Right: move horizontally within the current row.
    if (Pressed(keys, Keys.Right) || Pressed(keys, Keys.D))
    {
        int row = _selected / 4;
        int column = (_selected % 4 + 1) % 4;
        _selected = row * 4 + column;
        _selectionStatus = "";
    }

    if (Pressed(keys, Keys.Left) || Pressed(keys, Keys.A))
    {
        int row = _selected / 4;
        int column = (_selected % 4 + 3) % 4;
        _selected = row * 4 + column;
        _selectionStatus = "";
    }

    // Up / Down: move between rows, keeping the same column.
    if (Pressed(keys, Keys.Down) || Pressed(keys, Keys.S))
    {
        _selected = (_selected + 4) % 8;
        _selectionStatus = "";
    }

    if (Pressed(keys, Keys.Up) || Pressed(keys, Keys.W))
    {
        _selected = (_selected + 4) % 8;
        _selectionStatus = "";
    }

    // Space: only the top four slots currently contain items.
    if (Pressed(keys, Keys.Space))
    {
        if (_selected < Equipment.Length)
        {
            _selectionStatus = _inventory.EquipItem(SelectedItem)
                ? $"Equipped {ItemLabel(SelectedItem)}. Press X during gameplay to use."
                : SelectedItem == ItemType.Bow
                    ? "Collect both the bow and arrows to equip them."
                    : "You have not collected this item.";
        }
        else
        {
            _selectionStatus = "This slot is empty.";
        }
    }

    _previous = keys;
    }

    private bool Pressed(KeyboardState keys, Keys key) => keys.IsKeyDown(key) && _previous.IsKeyUp(key);
    private static string ItemLabel(ItemType item) => item == ItemType.Bow ? "Bow/Arrow" : item.ToString();

    private static Rectangle GetItemSource(ItemType item)
    {
        return item switch
        {
            ItemType.Bomb => new Rectangle(604, 137, 8, 16),
            ItemType.Bow => new Rectangle(633, 137, 8, 16),
            ItemType.Boomerang => new Rectangle(584, 137, 8, 16),
            ItemType.Potion => new Rectangle(704, 137, 8, 16),
            _ => Rectangle.Empty
        };
    }
    public void Draw(SpriteBatch batch, Rectangle viewport)
    {
        // Draw inventory background.
        batch.Draw( _hudAtlas, new Rectangle(0, 0, 768, 264), _inventoryBackground, Color.White);

        // Draw Triforce or dungeon map background.
        Rectangle infoBackground = _layout == InventoryLayout.Triforce
            ? _triforceBackground
            : _dungeonMapBackground;

        batch.Draw(_hudAtlas, new Rectangle(0, 264, 768, 264), infoBackground, Color.White);

        // Item positions.
        Rectangle[] itemPositions =
        {
            new Rectangle(396, 144, 24, 48), // Bomb
            new Rectangle(468, 144, 24, 48), // Boomerang
            new Rectangle(552, 144, 24, 48), // Bow
            new Rectangle(612, 144, 24, 48),  // Potion
            new Rectangle(396, 192, 24, 48), // Empty slot 5
            new Rectangle(468, 192, 24, 48), // Empty slot 6
            new Rectangle(552, 192, 24, 48), // Empty slot 7
            new Rectangle(612, 192, 24, 48)  // Empty slot 8
        };

        // Arrow position (visual companion to bow).
        Rectangle arrowPosition = new Rectangle(528, 144, 24, 48);

        // Draw selection square.
        int selectionOffset = (_selected == 2 || _selected == 6) ? 24 : 12;

        batch.Draw(_hudAtlas, new Rectangle(itemPositions[_selected].X - selectionOffset,
            itemPositions[_selected].Y, 48, 48), new Rectangle(519, 137, 16, 16), Color.White);

        // Draw selectable item sprites.
        for (int i = 0; i < Equipment.Length; i++)
        {
            ItemType item = Equipment[i];

            if (!_inventory.HasItem(item))
                continue;

            batch.Draw(_hudAtlas, itemPositions[i], GetItemSource(item), Color.White);
        }

        // Draw arrow beside bow.
        if (_inventory.HasItem(ItemType.Bow) && _inventory.HasItem(ItemType.Arrow))
        {
            batch.Draw(_hudAtlas, arrowPosition, new Rectangle(615, 137, 8, 16), Color.White);
        }

        // Draw equipment feedback.
        Vector2 origin = new(viewport.Left + 48, viewport.Top + 402);

        batch.DrawString(_font, _selectionStatus, origin, Color.Gold);
    }

    public void Reset()
    {
        _selected = _inventory.GetEquippedItem() is ItemType item ? Math.Max(0, Array.IndexOf(Equipment, item)) : 0;
        _selectionStatus = "";
        _previous = Keyboard.GetState();
    }
}
