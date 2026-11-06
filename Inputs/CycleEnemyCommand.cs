using escape.Interfaces;
using escape.Levels;

namespace escape.Inputs;

// Purpose: Cycles through the list of enemies in the level depending on the direction provided
public sealed class CycleEnemyCommand : ICommand
{
    private readonly LevelManager _levelManager;
    private readonly int _direction;

    public CycleEnemyCommand(LevelManager levelManager, int direction)
    {
        _levelManager = levelManager;
        _direction = direction;
    }

    public void Execute()
    {
        if (_direction < 0)
        {
            _levelManager.SelectPreviousEnemy();
        }
        else
        {
            _levelManager.SelectNextEnemy();
        }
    }
}