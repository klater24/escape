
using System;
using System.IO;
using System.Text.Json;

namespace escape.Levels;

public static class RoomParser
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static RoomData Load(string filePath)
    {
        using FileStream stream = File.OpenRead(filePath);

        RoomData? room = JsonSerializer.Deserialize<RoomData>(
            stream, _options);

        return room ?? throw new InvalidDataException(
            $"Could not load room data from {filePath}");
    }
}
