namespace escape.Interfaces;

// Environmental collision objects declare whether they block movement
public interface ISolidCollider : ICollidable
{
    bool IsSolid { get; }
}
