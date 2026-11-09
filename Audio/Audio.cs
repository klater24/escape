using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace escape.Sounds;

public sealed class Audio
{
    private readonly Dictionary<string, SoundEffect> _soundEffects = new();
    private readonly Dictionary<string, Song> _music = new();
    private string? _currentMusic;
    public float SoundVolume { get; set; } = 0.7f;
    public bool IsMusicMuted => MediaPlayer.IsMuted;
    public void ToggleMusicMute() => MediaPlayer.IsMuted = !MediaPlayer.IsMuted;

    public void LoadContent(ContentManager content)
    {
        var effect = content.Load<SoundEffect>("SoundEffects/effect");
        _soundEffects["main"] = effect;
        _soundEffects["ItemPickup"] = effect;
        _music["MainTheme"] = content.Load<Song>("SoundEffects/bg");
    }

    public void PlaySound(string name)
    {
        if (_soundEffects.TryGetValue(name, out var sound))
            sound.Play(MathHelper.Clamp(SoundVolume, 0f, 1f), 0f, 0f);
    }

    public void PlayMusic(string name)
    {
        if (!_music.TryGetValue(name, out var song)) return;
        if (_currentMusic == name && MediaPlayer.State == MediaState.Playing) return;
        MediaPlayer.Stop();
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(song);
        _currentMusic = name;
    }

    public void StopMusic()
    {
        MediaPlayer.Stop();
        _currentMusic = null;
    }
}
