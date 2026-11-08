using escape.Interfaces;

namespace escape.GameStates;

public sealed class GameStateManager : IGameResettable
{
    public GameState Current { get; private set; } = GameState.Gameplay;
    public GameState TransitionTarget { get; private set; } = GameState.Gameplay;
    public bool CanUpdateGameplay => Current == GameState.Gameplay;
    public event Action<GameState, GameState>? Changed;

    public void TogglePause()
    {
        if (Current == GameState.Gameplay) SetState(GameState.Paused);
        else if (Current == GameState.Paused) SetState(GameState.Gameplay);
    }

    public bool BeginInventoryTransition()
    {
        if (Current is not (GameState.Gameplay or GameState.Inventory)) return false;
        TransitionTarget = Current == GameState.Gameplay ? GameState.Inventory : GameState.Gameplay;
        SetState(GameState.Transition);
        return true;
    }

    public bool BeginRoomTransition()
    {
        if (!CanUpdateGameplay) return false;
        TransitionTarget = GameState.Gameplay;
        SetState(GameState.Transition);
        return true;
    }

    public void CompleteTransition()
    {
        if (Current == GameState.Transition) SetState(TransitionTarget);
    }

    public void EvaluateOutcome(bool defeated, bool hasTriforce)
    {
        if (!CanUpdateGameplay) return;
        if (defeated) SetState(GameState.GameOver);
        else if (hasTriforce) SetState(GameState.Victory);
    }

    public void Reset()
    {
        TransitionTarget = GameState.Gameplay;
        SetState(GameState.Gameplay);
    }

    private void SetState(GameState next)
    {
        if (Current == next) return;
        var previous = Current;
        Current = next;
        Changed?.Invoke(previous, next);
    }
}
