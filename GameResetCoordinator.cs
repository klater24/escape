using escape.Interfaces;

namespace escape;

// Calls reset on each registered game object or system
public sealed class GameResetCoordinator
{
    private readonly List<IGameResettable> _systems = new();

    public void Register(IGameResettable system)
    {
        ArgumentNullException.ThrowIfNull(system);

        if (!_systems.Contains(system))
        {
            _systems.Add(system);
        }
    }

    public void Reset()
    {
        foreach (var system in _systems)
        {
            system.Reset();
        }
    }
}