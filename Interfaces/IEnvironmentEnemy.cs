namespace escape.Interfaces;

public interface IEnvironmentEnemy : IMoveCollider
{
    IReadOnlyList<ISolidCollider> Obstacles { get; set; }
}
