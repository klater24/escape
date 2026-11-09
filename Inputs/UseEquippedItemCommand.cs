using escape.Interfaces;
using escape.Items;

namespace escape.Inputs;

public sealed class UseEquippedItemCommand : ICommand
{
    private readonly EquippedItemUse _itemUse;

    public UseEquippedItemCommand(EquippedItemUse itemUse) => _itemUse = itemUse;

    public void Execute() => _itemUse.TryUse();
}
