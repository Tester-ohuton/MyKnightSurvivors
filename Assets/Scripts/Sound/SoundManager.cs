using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource seAudioSource;
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private List<BGMSoundData> bgmSoundDatas;
    [SerializeField] private List<SESoundData> seSoundDatas;

    public float masterVolume = 1;
    public float bgmMasterVolume = 1;
    public float seMasterVolume = 1;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(BGMSoundData.BGM bgm)
    {
        BGMSoundData data = bgmSoundDatas.Find(d => d.bgm == bgm);
        if (data != null)
        {
            bgmAudioSource.clip = data.audioClip;
            bgmAudioSource.volume = data.volume * bgmMasterVolume * masterVolume;
            bgmAudioSource.Play();
        }
    }

    public void StopBGM()
    {
        bgmAudioSource.Stop();
    }

    public void PlaySE(SESoundData.SE se)
    {
        SESoundData data = seSoundDatas.Find(d => d.se == se);
        if (data != null)
        {
            seAudioSource.PlayOneShot(data.audioClip, data.volume * seMasterVolume * masterVolume);
        }
    }
}

[System.Serializable]
public class BGMSoundData
{
    public enum BGM
    {
        Title,
        Dungeon,
        Battle
    }

    public BGM bgm;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}

[System.Serializable]
public class SESoundData
{
    public enum SE
    {
        LevelUp,
        Button,
    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}