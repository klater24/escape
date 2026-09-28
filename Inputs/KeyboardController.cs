using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using escape.Interfaces;

namespace escape.Inputs;

public class KeyboardController : IController
{
    private Game1 _game;
    private Player _player;
    private KeyboardState previousState;
    private KeyboardState currentState;
    private Dictionary<Keys, ICommand> pressedCommands;
    private Dictionary<Keys, ICommand> heldCommands;

    public KeyboardController(Game1 game, Player player)
    {
        _game = game;
        _player = player;
        previousState = new KeyboardState();
        currentState = Keyboard.GetState();
        pressedCommands = new Dictionary<Keys, ICommand>();
        heldCommands = new Dictionary<Keys, ICommand>();
    }
    public void Initialize()
    {
        heldCommands[Keys.W] = new MoveUpCommand(_player);
        heldCommands[Keys.Up] = new MoveUpCommand(_player);

        heldCommands[Keys.S] = new MoveDownCommand(_player);
        heldCommands[Keys.Down] = new MoveDownCommand(_player);

        heldCommands[Keys.A] = new MoveLeftCommand(_player);
        heldCommands[Keys.Left] = new MoveLeftCommand(_player);

        heldCommands[Keys.D] = new MoveRightCommand(_player);
        heldCommands[Keys.Right] = new MoveRightCommand(_player);

        pressedCommands[Keys.Z] = new AttackCommand(_player);
        pressedCommands[Keys.N] = new AttackCommand(_player);

        pressedCommands[Keys.Q] = new QuitCommand(_game);
        pressedCommands[Keys.R] = new ResetCommand(_game.Reset);

        pressedCommands[Keys.E] = new DamageSelfCommand(_player);
        
        pressedCommands[Keys.D1] = new UseItemCommand(1, _player);
        pressedCommands[Keys.D2] = new UseItemCommand(2, _player);
        pressedCommands[Keys.D3] = new UseItemCommand(3, _player);
        pressedCommands[Keys.D4] = new UseItemCommand(4, _player);
    }
    public void Update()
    {
        previousState = currentState;
        currentState = Keyboard.GetState();
        bool moving = false;

        foreach (var key in heldCommands.Keys)
        {
            if (currentState.IsKeyDown(key))
            {
                moving = true;
                heldCommands[key].Execute();
            }
        }
        if (!moving)
        {
            _player.StopMoving();
        }
        foreach (var key in pressedCommands.Keys)
        {
            if (currentState.IsKeyDown(key) &&
                previousState.IsKeyUp(key))
            {
                pressedCommands[key].Execute();
            }
        }
    }
}