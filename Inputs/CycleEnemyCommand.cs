using escape.Interfaces;

namespace escape.Inputs;

public sealed class CycleEnemyCommand : ICommand
{
    private readonly Game1 _game;
    private readonly int _direction;

    public CycleEnemyCommand(Game1 game, int direction)
    {
        _game = game;
        _direction = direction;
    }

    public void Execute() => _game.CycleEnemy(_direction);
}
