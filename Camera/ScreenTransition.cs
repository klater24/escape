using escape.GameStates;
using escape.Interfaces;
using Microsoft.Xna.Framework;

namespace escape.View;

public sealed class ScreenTransition : IGameTransition
{
    private const float DurationSeconds = 0.35f;
    private readonly Camera _camera = new(Vector2.Zero);
    private readonly Func<string, Vector2> _roomDirection;
    private readonly Vector2 _screenSize;
    private readonly float _hudHeight;
    private Vector2 _destination;

    public GameState From { get; private set; }
    public GameState To { get; private set; }
    public bool IsRoomTransition { get; private set; }
    public bool IsTransitioning => _camera.IsTransitioning;
    public Vector2 OutgoingOffset => -_camera.Position;
    public Vector2 IncomingOffset => _destination - _camera.Position;

    public ScreenTransition(Vector2 screenSize, float hudHeight, Func<string, Vector2> roomDirection)
    {
        _screenSize = screenSize;
        _hudHeight = hudHeight;
        _roomDirection = roomDirection;
    }

    public void Begin(GameState from, GameState to, string? destinationRoom = null)
    {
        From = from;
        To = to;
        IsRoomTransition = destinationRoom is not null;
        Vector2 direction = IsRoomTransition ? _roomDirection(destinationRoom!)
            : to == GameState.Inventory ? -Vector2.UnitY : Vector2.UnitY;
        if (direction == Vector2.Zero) direction = Vector2.UnitX;
        _destination = direction * new Vector2(_screenSize.X, IsRoomTransition ? _screenSize.Y - _hudHeight : _screenSize.Y);
        _camera.Transition(Vector2.Zero, 1f, 0f);
        _camera.Transition(_destination, 1f, DurationSeconds);
    }

    public void Update(GameTime gameTime) => _camera.Update(gameTime);

    public void Cancel()
    {
        _camera.Transition(Vector2.Zero, 1f, 0f);
        _destination = Vector2.Zero;
    }
}
