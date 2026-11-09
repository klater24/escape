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
    private static readonly ItemType[] Equipment = { ItemType.Bomb, ItemType.Bow, ItemType.Boomerang, ItemType.Potion };

    public BasicInventoryScreen(IInventory inventory, SpriteFont font)
    {
        _inventory = inventory;
        _font = font;
    }

    public void Update(GameTime gameTime) => Update(Keyboard.GetState());

    public void Update(KeyboardState keys)
    {
        if (Pressed(keys, Keys.Down)) { _selected = (_selected + 1) % Equipment.Length; _selectionStatus = ""; }
        if (Pressed(keys, Keys.Up)) { _selected = (_selected + Equipment.Length - 1) % Equipment.Length; _selectionStatus = ""; }
        if (Pressed(keys, Keys.Space))
            _selectionStatus = _inventory.EquipItem(SelectedItem)
                ? $"Equipped {ItemLabel(SelectedItem)}. Press X during gameplay to use."
                : SelectedItem == ItemType.Bow ? "Collect both the bow and arrows to equip them." : "You have not collected this item.";
        _previous = keys;
    }

    private bool Pressed(KeyboardState keys, Keys key) => keys.IsKeyDown(key) && _previous.IsKeyUp(key);
    private static string ItemLabel(ItemType item) => item == ItemType.Bow ? "Bow/Arrow" : item.ToString();

    public void Draw(SpriteBatch batch, Rectangle viewport)
    {
        Vector2 origin = new(viewport.Left + 48, viewport.Top + 32);
        batch.DrawString(_font, "INVENTORY", origin, Color.Gold);
        batch.DrawString(_font, "Up/Down: Select   Space: Equip   Enter: Return", origin + new Vector2(0, 40), Color.White);
        for (int i = 0; i < Equipment.Length; i++)
        {
            var item = Equipment[i];
            string equipped = _inventory.GetEquippedItem() == item ? " (equipped)" : "";
            string availability = item == ItemType.Bow
                ? (_inventory.HasItem(ItemType.Bow) && _inventory.HasItem(ItemType.Arrow) ? "Ready - 1 rupee/shot" : "Requires bow and arrows")
                : _inventory.GetQuantity(item).ToString();
            batch.DrawString(_font, $"{(i == _selected ? ">" : " ")} {ItemLabel(item)}: {availability}{equipped}",
                origin + new Vector2(0, 95 + i * 38), i == _selected ? Color.Gold : Color.White);
        }
        batch.DrawString(_font, $"Keys: {_inventory.GetQuantity(ItemType.Key)}   Map: {_inventory.HasItem(ItemType.Map)}   Compass: {_inventory.HasItem(ItemType.Compass)}",
            origin + new Vector2(0, 320), Color.White);
        batch.DrawString(_font, _selectionStatus, origin + new Vector2(0, 370), Color.Gold);
    }

    public void Reset()
    {
        _selected = _inventory.GetEquippedItem() is ItemType item ? Math.Max(0, Array.IndexOf(Equipment, item)) : 0;
        _selectionStatus = "";
        _previous = Keyboard.GetState();
    }
}
