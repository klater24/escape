using Microsoft.Xna.Framework.Input;
using escape.Interfaces;
using Microsoft.Xna.Framework;

namespace escape.Inputs;

public class UseItemCommand : ICommand
{
    private int _itemSlot;

    public UseItemCommand(int itemSlot, Player player)
    {
        _itemSlot = itemSlot;
    }
    public void Execute()
    {
        
    }
}