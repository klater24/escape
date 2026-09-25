using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using escape.Interfaces;

namespace escape.Inputs;

public class KeyboardController : IController
{
    private Game game;
    private KeyboardState previousState;
    private KeyboardState currentState;
    private Dictionary<Keys, ICommand> pressedCommands;
    private Dictionary<Keys, ICommand> heldCommands;

    public KeyboardController(Game game)
    {
        this.game = game;
        previousState = new KeyboardState();
        currentState = Keyboard.GetState();
        pressedCommands = new Dictionary<Keys, ICommand>();
        heldCommands = new Dictionary<Keys, ICommand>();
    }
    public void Initialize()
    {
        heldCommands[Keys.W] = new MoveUpCommand();
        heldCommands[Keys.Up] = new MoveUpCommand();

        heldCommands[Keys.S] = new MoveDownCommand();
        heldCommands[Keys.Down] = new MoveDownCommand();

        heldCommands[Keys.A] = new MoveLeftCommand();
        heldCommands[Keys.Left] = new MoveLeftCommand();

        heldCommands[Keys.D] = new MoveRightCommand();
        heldCommands[Keys.Right] = new MoveRightCommand();

        pressedCommands[Keys.Z] = new AttackCommand();
        pressedCommands[Keys.N] = new AttackCommand();

        pressedCommands[Keys.Q] = new QuitCommand(game);
        pressedCommands[Keys.R] = new ResetCommand();

        pressedCommands[Keys.E] = new DamageSelfCommand();
        
        pressedCommands[Keys.D1] = new UseItemCommand(1);
        pressedCommands[Keys.D2] = new UseItemCommand(2);
        pressedCommands[Keys.D3] = new UseItemCommand(3);
        pressedCommands[Keys.D4] = new UseItemCommand(4);
    }
    public void Update()
    {
        previousState = currentState;
        currentState = Keyboard.GetState();

        foreach (var key in heldCommands.Keys)
        {
            if (currentState.IsKeyDown(key))
            {
                heldCommands[key].Execute();
            }
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