using escape.Interfaces;
using Microsoft.Xna.Framework;

namespace escape.Collisions;

public sealed class EnemyDoorBarrier : ISolidCollider
{
    private readonly ICollidable _door;
    public EnemyDoorBarrier(ICollidable door) => _door = door;
    public Rectangle Bounds => _door.Bounds;
    public Collision Category => Collision.Environment;
    public bool IsCollisionEnabled => _door.IsCollisionEnabled;
    public bool IsSolid => true;
}
