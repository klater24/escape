using escape.Interfaces;

namespace escape.Inputs;

public class MoveUpCommand : ICommand
{
    private Player _player;

    public MoveUpCommand(Player player)
    {
        _player = player;
    }
    public void Execute()
    {
        _player.Move(Direction.Up);
    }
}