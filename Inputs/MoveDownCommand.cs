using escape.Interfaces;

namespace escape.Inputs;

public class MoveDownCommand : ICommand
{
    private Player _player;

    public MoveDownCommand(Player player)
    {
        _player = player;
    }
    public void Execute()
    {
        _player.Move(Direction.Down);
    }
}