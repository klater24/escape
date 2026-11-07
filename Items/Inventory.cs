using System;
using System.Collections.Generic;
using escape.Interfaces;

namespace escape.Items;

public class Inventory : IInventory, IGameResettable
{
    private readonly Dictionary<ItemType, int> _items = new();

    private ItemType? _equippedItem;

    public void AddItem(ItemType item, int quantity = 1)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (!_items.ContainsKey(item))
        {
            _items[item] = 0;
        }

        _items[item] += quantity;
    }

    public bool RemoveItem(ItemType item, int quantity = 1)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (GetQuantity(item) < quantity)
        {
            return false;
        }

        _items[item] -= quantity;

        if (_items[item] == 0)
        {
            _items.Remove(item);

            if (_equippedItem == item)
            {
                _equippedItem = null;
            }
        }

        return true;
    }

    public int GetQuantity(ItemType item)
    {
        return _items.TryGetValue(item, out int quantity)
            ? quantity
            : 0;
    }

    public bool HasItem(ItemType item)
    {
        return GetQuantity(item) > 0;
    }

    public bool EquipItem(ItemType item)
    {
        if (!HasItem(item))
        {
            return false;
        }

        _equippedItem = item;
        return true;
    }

    public ItemType? GetEquippedItem()
    {
        return _equippedItem;
    }

    public void Reset()
    {
        _items.Clear();
        _equippedItem = null;
    }
}

