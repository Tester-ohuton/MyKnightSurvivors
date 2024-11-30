using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    private void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener((value) =>
            {
                // value‚Í0`1‚Ì’l‚ğŠú‘Ò‚·‚éB‚»‚ê‚ğ•ÛØ‚·‚é‚½‚ß‚Ìˆ—
                value = Mathf.Clamp01(value);

                float decibel = 20f * Mathf.Log10(value);
                decibel = Mathf.Clamp(decibel, -80f, 0f);
                audioMixer.SetFloat("MasterVolume", decibel);
            });
        }
    }
}