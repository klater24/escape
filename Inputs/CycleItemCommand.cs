using escape.Interfaces;
using escape.Levels;

namespace escape.Inputs;

// Purpose: Cycles through the list of items in the level depending on the direction provided
public sealed class CycleItemCommand : ICommand
{
    private readonly LevelManager _levelManager;
    private readonly int _direction;

    public CycleItemCommand(LevelManager levelManager, int direction)
    {
        _levelManager = levelManager;
        _direction = direction;
    }

    public void Execute()
    {
        if (_direction < 0)
        {
            _levelManager.SelectPreviousItem();
        }
        else
        {
            _levelManager.SelectNextItem();
        }
    }
}