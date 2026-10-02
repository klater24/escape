using escape.Interfaces;
using escape.Blocks;

namespace escape.Inputs;

public class CycleNextBlocksCommand : ICommand
{
    private readonly BlockManager _blockManager;

    public CycleNextBlocksCommand(BlockManager blockManager)
    {
        _blockManager = blockManager;
    }
    public void Execute()
    {
        _blockManager.SelectNext();
    }
}