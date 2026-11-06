using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using escape.Interfaces;

namespace escape.Items;

// Item Manager purpose is to manage the items in the game, allowing for cycling through them, updating their state, and drawing them on the screen
// It implements the IGameResettable interface to allow for resetting the state of all managed items.
public sealed class ItemManager : escape.Interfaces.IGameResettable
{
    private readonly IReadOnlyList<IItem> _items;
    public int SelectedIndex { get; private set; }
    public IItem? Current => _items.Count == 0 ? null : _items[SelectedIndex];
    public ItemManager(IReadOnlyList<IItem> items)
    {
        _items = items;
    }

    public void Cycle(int direction)
    {
        if (_items.Count == 0)
        {
            return;
        }

        SelectedIndex = ((SelectedIndex + direction) % _items.Count + _items.Count) % _items.Count;
    }

    public void Update(GameTime gameTime) => Current?.Update(gameTime);
    public void Draw(SpriteBatch spriteBatch) => Current?.Draw(spriteBatch);

    public void Reset()
    {
        foreach (var item in _items) item.Reset();
        SelectedIndex = 0;
    }
}
