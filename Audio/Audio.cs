using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
namespace escape.Sounds;
public class Audio
{
    private readonly Dictionary<string, SoundEffect> _soundEffects = new();
    private readonly Dictionary<string, Song> _music = new();
    private string? _currentMusic;
    public float SoundVolume { get; set; } = 0.7f;

    public void LoadContent(ContentManager content)
    {
        _soundEffects["main"] = content.Load<SoundEffect>("SoundEffects/effect.wav");
        _music["MainTheme"] =  content.Load<Song>("SoundEffects/bg.mp3");
    }
       public void PlaySound(string name)
    {
        if (!_soundEffects.TryGetValue(name, out SoundEffect? sound))
            return;

        sound.Play(SoundVolume, 0f, 0f);
    }
       public void PlayMusic(string name)
    {
       if (!_music.TryGetValue(name, out Song? song))
            return;

        if (_currentMusic == name &&MediaPlayer.State == MediaState.Playing)
            return;

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
