using escape.Interfaces;

namespace escape.Inputs;

public class MoveRightCommand : ICommand
{
    private Player _player;

    public MoveRightCommand(Player player)
    {
        _player = player;
    }
    public void Execute()
    {
        _player.Move(Direction.Right);
    }
}