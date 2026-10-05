using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using escape.Interfaces;
using escape.Blocks;
using escape.Projectiles;

namespace escape.Inputs;

public class KeyboardController : IController
{
    private Game1 _game;
    private Player _player;
    private KeyboardState previousState;
    private KeyboardState currentState;
    private Dictionary<Keys, ICommand> pressedCommands;
    private Dictionary<Keys, ICommand> heldCommands;
    private BlockManager _blockManager;
    private ProjectileManager _projectiles;


    public KeyboardController(Game1 game, Player player, BlockManager blockManager, ProjectileManager projectiles)
    {
        _game = game;
        _player = player;
        _blockManager = blockManager;
        _projectiles = projectiles;

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

        pressedCommands[Keys.O] = new CycleEnemyCommand(_game, -1);
        pressedCommands[Keys.P] = new CycleEnemyCommand(_game, 1);

        pressedCommands[Keys.U] = new CycleItemsCommand(_game, -1);
        pressedCommands[Keys.I] = new CycleItemsCommand(_game, 1);

        pressedCommands[Keys.Q] = new QuitCommand(_game);
        pressedCommands[Keys.R] = new ResetCommand(_game.Reset);

        pressedCommands[Keys.E] = new DamageSelfCommand(_player);

        pressedCommands[Keys.T] = new CyclePreviousBlocksCommand(_blockManager);
        pressedCommands[Keys.Y] = new CycleNextBlocksCommand(_blockManager);
        
        pressedCommands[Keys.D1] = new UseItemCommand(1, _player, _projectiles);
        pressedCommands[Keys.D2] = new UseItemCommand(2, _player, _projectiles);
        pressedCommands[Keys.D3] = new UseItemCommand(3, _player, _projectiles);
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
