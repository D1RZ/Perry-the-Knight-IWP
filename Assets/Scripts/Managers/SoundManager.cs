using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
    }

    public static SoundManager instance { get; private set; }

    [SerializeField] private List<AudioSource> SfxPlayer = new List<AudioSource>();
    [SerializeField] private int AudioPool = 4;
    [SerializeField] private AudioSource MusicPlayer;

    [SerializeField] private List<SoundEffect> SfxSounds = new List<SoundEffect>();

    [SerializeField] private Slider SfxSlider;
    [SerializeField] private Slider MusicSlider;

    [SerializeField] private SoundVolume SoundVolume;

    public void Awake()
    {
        instance = this;

        SfxSlider.value = SoundVolume.SfxVolume / 100;
        MusicSlider.value = SoundVolume.MusicVolume / 100;

        for (int i = 0; i < AudioPool; i++)
        {
            GameObject AudioSourceObj = new GameObject("SfxPlayer " + i);
            AudioSourceObj.transform.SetParent(this.transform);
            AudioSource audio = AudioSourceObj.AddComponent<AudioSource>();
            SfxPlayer.Add(audio);
        }
    }

    private void Start()
    {
        MusicPlayer.volume = Mathf.Clamp01(MusicSlider.value);

        MusicPlayer.Play();
    }

    public void SetVolume()
    {
        SoundVolume.SfxVolume = (SfxSlider.value * 100);
        SoundVolume.MusicVolume = (MusicSlider.value * 100);

        MusicPlayer.volume = Mathf.Clamp01(MusicSlider.value);

        foreach (AudioSource audio in SfxPlayer)
        {
            audio.volume = Mathf.Clamp01(SfxSlider.value);
        }
    }

    public void PlaySoundEffect(string soundName)
    {
        foreach (var effect in SfxSounds)
        {
            if (effect.name == soundName)
            {
                if (soundName == "Delicious")
                {
                    foreach (AudioSource audio in SfxPlayer)
                    {
                        if (audio.clip == effect.clip && audio.isPlaying)
                        {
                            return;
                        }
                    }
                }

                PlaySFX(effect.clip, 1f);
            }
        }
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        foreach (var Player in SfxPlayer)
        {
            if (!Player.isPlaying)
            {
                Player.clip = clip;
                Player.volume = Mathf.Clamp01(SfxSlider.value);
                Player.Play();
                return;
            }
        }

        Debug.Log("No Available Sound");
    }

    public void PlayMusic(AudioClip clip)
    {
        MusicPlayer.clip = clip;
        MusicPlayer.volume = Mathf.Clamp01(MusicSlider.value);

        MusicPlayer.Play();
    }

    private void Update()
    {
        SetVolume();
    }

}

