using escape.Interfaces;
using escape.Items;

namespace escape.Inputs;

public sealed class UseItemCommand : ICommand
{
    private readonly ItemType _item;
    private readonly EquippedItemUse _itemUse;

    public UseItemCommand(ItemType item, EquippedItemUse itemUse)
    {
        _item = item;
        _itemUse = itemUse;
    }

    public void Execute() => _itemUse.TryUse(_item);
}
