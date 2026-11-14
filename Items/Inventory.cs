using System;
using System.Collections.Generic;
using escape.Interfaces;

namespace escape.Items;

public class Inventory : IInventory, IGameResettable
{
    public const int BombCapacity = 10;
    public event Action<ItemType, int>? ItemAdded;
    public event Action<ItemType, int>? ItemRemoved;
    public event Action<ItemType>? ItemEquipped;
    private readonly Dictionary<ItemType, int> _items = new();

    private ItemType? _equippedItem;

    public void AddItem(ItemType item, int quantity = 1)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (item == ItemType.Bomb)
        {
            quantity = Math.Min(quantity, BombCapacity - GetQuantity(item));
            if (quantity == 0) return;
        }

        if (!_items.ContainsKey(item))
        {
            _items[item] = 0;
        }

        _items[item] += quantity;
        ItemAdded?.Invoke(item, quantity);
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

        if (_equippedItem == item || (_equippedItem == ItemType.Bow && item == ItemType.Arrow))
        {
            _equippedItem = null;
        }
    }

    ItemRemoved?.Invoke(item, quantity);

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
    if (item == ItemType.Arrow) item = ItemType.Bow;
    if (item == ItemType.Bow && !HasItem(ItemType.Arrow)) return false;
    if (!HasItem(item))
    {
        return false;
    }

    _equippedItem = item;

    ItemEquipped?.Invoke(item);

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

