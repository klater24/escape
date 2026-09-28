using Microsoft.Xna.Framework.Input;
using escape.Interfaces;

namespace escape.Inputs;

public class UseItemCommand : ICommand
{
    private int _itemSlot;

    public UseItemCommand(int itemSlot)
    {
        _itemSlot = itemSlot;
    }
    public void Execute()
    {
        
    }
}