using System;
using escape.Interfaces;

namespace escape.Inputs;

public class ResetCommand : ICommand
{
    private readonly Action _reset;

    public ResetCommand(Action reset)
    {
        _reset = reset;
    }

    public void Execute()
    {
        _reset();
    }
}