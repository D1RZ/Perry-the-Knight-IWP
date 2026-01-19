using UnityEngine;

[CreateAssetMenu(fileName = "Volume", menuName = "Volume")]
public class SoundVolume : ScriptableObject
{
    [SerializeField] public float SfxVolume;
    [SerializeField] public float MusicVolume;

    private void Awake()
    {
        SfxVolume = 100;
        MusicVolume = 100;
    }
}

