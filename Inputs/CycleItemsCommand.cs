using escape.Interfaces;

namespace escape.Inputs;

public sealed class CycleItemsCommand : ICommand
{
    private readonly Game1 _game;
    private readonly int _direction;

    public CycleItemsCommand(Game1 game, int direction)
    {
        _game = game;
        _direction = direction;
    }

    public void Execute() => _game.CycleItems(_direction);
}
