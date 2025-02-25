using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public static Action<float> OnSetVolume;
    public static Action OnUpdateSlider;

    public static float bgmVolume = 0.0f;
    public static float effectVolume = 0.0f;

    enum VolumeType
    {
        BGM,
        Effect,
    }

    [SerializeField] private VolumeType type;
    [SerializeField, Range(-20.0f, 0.0f)] private float minVolume;
    [SerializeField, Range(0.0f, 10.0f)] private float maxVolume;
    [SerializeField] Transform fillBar;

    private Slider slider;
    private AudioMixer audioMixer;

    private void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        audioMixer = SoundManager.Instance().GetAudioMixer();

        UpdateSlider();
        OnValueChanged();
    }

    public void UpdateSlider()
    {
        float volumeValue = 0.0f;
        if (type == VolumeType.BGM)
        {
            volumeValue = (bgmVolume - minVolume) / (maxVolume - minVolume);
            slider.value = volumeValue;
        }
        else if (type == VolumeType.Effect)
        {
            volumeValue = (effectVolume - minVolume) / (maxVolume - minVolume);
            slider.value = volumeValue;
        }
    }

    private void OnEnable()
    {
        OnSetVolume -= SetVolume;
        OnSetVolume += SetVolume;

        OnUpdateSlider -= UpdateSlider;
        OnUpdateSlider += UpdateSlider;
    }

    private void OnDisable()
    {
        OnSetVolume -= SetVolume;
    }

    public void SetVolume(float volume)
    {
        string volumeType = type.ToString();

        float bgmRatio = minVolume + bgmVolume * (maxVolume - minVolume);
        float effectRatio = minVolume + effectVolume * (maxVolume - minVolume);

        if (type == VolumeType.BGM)
        {
            slider.value = bgmVolume;
            SoundManager.Instance().SetAudioVolume(volumeType, volume);
        }
        else if (type == VolumeType.Effect)
        {
            slider.value = effectVolume;
            SoundManager.Instance().SetAudioVolume(volumeType, volume);
        }
    }

    public void OnValueChanged()
    {
        float sliderValue = slider.value;
        fillBar.gameObject.SetActive(sliderValue > 0.001f);

        if (audioMixer == null)
        {
            Debug.Log("audioMixer is null");
            return;
        }

        float gap = maxVolume - minVolume;
        float volume = gap * sliderValue + minVolume;

        string volumeType = type.ToString();
        //audioMixer.FindMatchingGroups(volumeType)[0].audioMixer.SetFloat(volumeType, volume);
        //audioMixer.SetFloat(volumeType, volume);

        SoundManager.Instance().SetAudioVolume(volumeType, volume);

        if (type == VolumeType.BGM)
        {
            bgmVolume = volume;
        }
        else if (type == VolumeType.Effect)
        {
            effectVolume = volume;
        }

        Debug.Log($"{volumeType} : {volume}");
    }
}
