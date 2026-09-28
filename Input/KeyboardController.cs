using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace escape.Input;

public interface ICommand
{
    void Execute();
}

// Adapts a game action without coupling keyboard handling to game objects.
public sealed class ActionCommand : ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action) => _action = action;
    public void Execute() => _action();
}

public sealed class KeyboardController
{
    private readonly Dictionary<Keys, ICommand> _commands = new();
    private KeyboardState _previous;

    public void Bind(Keys key, ICommand command) => _commands[key] = command;

    public void Update(KeyboardState current)
    {
        foreach (var binding in _commands)
        {
            if (current.IsKeyDown(binding.Key) && !_previous.IsKeyDown(binding.Key))
                binding.Value.Execute();
        }
        _previous = current;
    }
}
