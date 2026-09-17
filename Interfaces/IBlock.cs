using Microsoft.Xna.Framework;

namespace escape.Interfaces;

// The shared rules for a level block
public interface IBlock : ISprite
{
    // The name used to choose the block art
    string BlockType { get; }

    // Shows whether the block stays in place
    bool IsStationary { get; }

    // Shows whether the block should be used and drawn
    bool IsActive { get; set; }

    // Leaves room for future block cycling controls
    void Cycle();
}
