using System.Collections.Generic;
using escape.Interfaces;
using Microsoft.Xna.Framework;

namespace escape.Collisions;

public sealed class SwordHitbox : ICollidable
{
    private const int Reach = 33;
    private const int Thickness = 18;
    private readonly Player _owner;
    private readonly HashSet<ICollidable> _hitTargets = new();

    public SwordHitbox(Player owner) => _owner = owner;
    public Collision Category => Collision.Sword;
    public bool IsCollisionEnabled => _owner.IsSwordActive;
    public Rectangle Bounds
    {
        get
        {
            Rectangle body = _owner.Bounds;
            int x = body.Center.X - Thickness / 2;
            int y = body.Center.Y - Thickness / 2;
            return _owner.GetFacingDirection() switch
            {
                Direction.Up => new Rectangle(x, body.Top - Reach, Thickness, Reach),
                Direction.Down => new Rectangle(x, body.Bottom, Thickness, Reach),
                Direction.Left => new Rectangle(body.Left - Reach, y, Reach, Thickness),
                _ => new Rectangle(body.Right, y, Reach, Thickness)
            };
        }
    }

    public void Reset() => _hitTargets.Clear();
    public bool TryHit(ICollidable target) =>
        AABBCollisionDetector.IsColliding(this, target) && _hitTargets.Add(target);
}
