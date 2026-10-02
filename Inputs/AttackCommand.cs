using escape.Interfaces;

namespace escape.Inputs;

public class AttackCommand : ICommand
{
    private Player _player;

    public AttackCommand(Player player)
    {
        _player = player;
    }
    public void Execute()
    {
        _player.Attack();
    }
}