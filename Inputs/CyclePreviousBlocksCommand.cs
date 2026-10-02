using escape.Interfaces;
using escape.Blocks;

namespace escape.Inputs;



public class CyclePreviousBlocksCommand : ICommand
{
    private readonly BlockManager _blockManager;

    public CyclePreviousBlocksCommand(BlockManager blockManager)
    {
        _blockManager = blockManager;
    }
    public void Execute()
    {
        _blockManager.SelectPrevious();
    }
}