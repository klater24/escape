using escape.Interfaces;

namespace escape.Inputs;

public class MoveLeftCommand : ICommand
{
    private Player _player;

    public MoveLeftCommand(Player player)
    {
        _player = player;
    }
    public void Execute()
    {
        _player.Move(Direction.Left);
    }
}