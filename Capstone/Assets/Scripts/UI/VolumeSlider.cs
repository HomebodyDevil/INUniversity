using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
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

        slider.value = 0.5f;
        OnValueChanged();
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

        Debug.Log($"{volumeType} : {volume}");
    }
}
