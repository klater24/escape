using escape.Interfaces;
using escape.Sounds;

namespace escape.Inputs;

public class AttackCommand : ICommand
{
   private readonly Player _player;
    private readonly Audio _audio;

    public AttackCommand(Player player, Audio audio)
    {
        _player = player;
        _audio = audio;
    }

    public void Execute()
    {
        if (!_player.CanStartAttack)
            return;

        _player.Attack();

        _audio.PlaySound("main");
    }
}
