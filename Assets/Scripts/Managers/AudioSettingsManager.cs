using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private TextMeshProUGUI BGMVolumeTxt;
    [SerializeField] private TextMeshProUGUI SFXVolumeTxt;

    private void Start()
    {
        Load();
        SetSFXVolume();
        SetBGMVolume();
        this.gameObject.SetActive(false);
    }

    public void SetBGMVolume()
    {
        mixer.SetFloat("BGMVolume",Mathf.Log10(BGMSlider.value) * 20);
        BGMVolumeTxt.text = Mathf.RoundToInt((BGMSlider.value / 1 * 100)).ToString();
    }

    public void SetSFXVolume()
    {
        mixer.SetFloat("SFXVolume",Mathf.Log10(SFXSlider.value) * 20);
        SFXVolumeTxt.text = Mathf.RoundToInt((SFXSlider.value / 1 * 100)).ToString();
    }

    private void Show()
    {
        this.gameObject.SetActive(true);
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void TurnOnOffAudioMenu()
    {
        if(!this.gameObject.activeInHierarchy)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void SaveSFX()
    {
        PlayerPrefs.SetFloat("SFXVolume",SFXSlider.value);
    }

    public void SaveBGM()
    {
        PlayerPrefs.SetFloat("BGMVolume",BGMSlider.value);
    }

    public void Load()
    {
        Debug.Log(PlayerPrefs.GetFloat("SFXVolume"));

        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        BGMSlider.value = PlayerPrefs.GetFloat("BGMVolume");
    }
}
