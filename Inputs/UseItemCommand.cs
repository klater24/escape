using Microsoft.Xna.Framework.Input;
using escape.Interfaces;

namespace escape.Inputs;

public class UseItemCommand : ICommand
{
    private int itemNumber;

    public UseItemCommand(int itemNumber)
    {
        this.itemNumber = itemNumber;
    }
    public void Execute()
    {
        
    }
}