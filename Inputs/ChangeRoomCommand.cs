using Microsoft.Xna.Framework;
using escape.Interfaces;
using escape.Levels;

namespace escape.Inputs;

public class ChangeRoomCommand : ICommand
{
    private readonly LevelManager _levelManager;
    private readonly string _roomId;
    private readonly Vector2 _spawnPosition;

    public ChangeRoomCommand(
        LevelManager levelManager,
        string roomId,
        Vector2 spawnPosition)
    {
        _levelManager = levelManager;
        _roomId = roomId;
        _spawnPosition = spawnPosition;
    }

    public void Execute()
    {
        _levelManager.ChangeRoom(_roomId, _spawnPosition);
    }
}
