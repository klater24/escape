using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using escape.Interfaces;

namespace escape.Items;

// Only the selected item runs in the sprint demonstration.
public sealed class ItemManager : escape.Interfaces.IGameResettable
{
    private readonly List<IItem> _items;
    public int SelectedIndex { get; private set; }
    public IItem Current => _items[SelectedIndex];
    public ItemManager(List<IItem> items)
    {
        if (items.Count == 0) throw new ArgumentException("At least one item is required.", nameof(items));
        _items = items;
    }

    public void Cycle(int direction) => SelectedIndex =
        ((SelectedIndex + direction) % _items.Count + _items.Count) % _items.Count;

    public void Update(GameTime gameTime) => Current.Update(gameTime);
    public void Draw(SpriteBatch spriteBatch) => Current.Draw(spriteBatch);

    public void Reset()
    {
        foreach (var item in _items) item.Reset();
        SelectedIndex = 0;
    }
}
