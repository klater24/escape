using escape.Items;

namespace escape.Interfaces;

public interface IInventory
{
    void AddItem(ItemType item, int quantity = 1);

    bool RemoveItem(ItemType item, int quantity = 1);

    int GetQuantity(ItemType item);

    bool HasItem(ItemType item);

    bool EquipItem(ItemType item);

    ItemType? GetEquippedItem();
}