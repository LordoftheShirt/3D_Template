using UnityEngine;


// Insanely basic audio system which supports 3D sound.
// Ensure you change the 'Sounds' audio source to use 3D spatial blend if you intend to use 3D sounds.
public class AudioSystem : Singleton<AudioSystem>
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundsSource;

    [SerializeField] public AudioClip[] songs;

    public void PlayMusic(AudioClip clip, float FastForward = 0f)
    {
        _musicSource.clip = clip;
        _musicSource.time = FastForward;
        _musicSource.Play();
    }

    public void PlaySound(AudioClip clip, Vector3 pos, float vol = 1)
    {
        _soundsSource.transform.position = pos;
        _soundsSource.PlayOneShot(clip, vol);

    }

    public void PlaySound(AudioClip clip, float vol = 1)
    {
        _soundsSource.PlayOneShot(clip, vol);

    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    public AudioSource GetMusicSource()
    {
        return _musicSource;
    }
}
