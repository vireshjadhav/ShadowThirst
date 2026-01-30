using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance {  get { return instance; } }

    [Header("Audio Sources")]
    public AudioSource soundMusic;
    public AudioSource soundEffect;

    [Header("Sound List")]
    public SoundType[] sound;

    [Header("Settings")]
    public float musicVolume = 1.0f;
    public float effectVolume = 1.0f;
    public bool isMute = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);


            //Load PlayerPrefs here
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            effectVolume = PlayerPrefs.GetFloat("EffectVolume", 1f);
            isMute = PlayerPrefs.GetInt("IsMute", 0) == 1;

            soundMusic.volume = musicVolume;
            soundEffect.volume = effectVolume;

            soundMusic.mute = isMute;
            soundEffect.mute = isMute;  

        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!soundMusic.isPlaying)
            PlayMusic(global::Sounds.Music);

        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        soundMusic.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    public void SetEffectVolume(float volume)
    {
        effectVolume = Mathf.Clamp01(volume);
        soundEffect.volume = effectVolume;
        PlayerPrefs.SetFloat("EffectVolume", effectVolume);
        PlayerPrefs.Save();
    }

    public float GetMusicVolume() => musicVolume;

    public float GetEffectVolume() => effectVolume;

    public void PlayMusic(Sounds sounds)
    {
        AudioClip clip = GetSoundClip(sounds);

        if (clip != null)
        {
            soundMusic.clip = clip;
            soundMusic.loop = true;
            soundMusic.Play();
        }
        else
        {
            Debug.Log("Clip not found for sound type: " + sounds);
        }
    }

    public void Play(Sounds sounds)
    {
        AudioClip clip = GetSoundClip(sounds);

        if (clip != null)
        {
            soundEffect.PlayOneShot(clip);
        }
        else
        {
            Debug.Log("Clip not found for sound type:" + sounds);
        }
    }

    public void Mute(bool status)
    {
        isMute = status;
        soundMusic.mute = isMute;
        soundEffect.mute = isMute;

        PlayerPrefs.SetInt("IsMute", isMute ? 1 : 0);
        PlayerPrefs.Save();
    }

    private AudioClip GetSoundClip(Sounds sounds)
    {
        SoundType item = Array.Find(sound, i => i.soundType == sounds);
        if (item != null)
            return item.soundClip;
        return null;
    }

    public void RestartMusic()
    {
        PlayMusic(global::Sounds.Music);
    }
}

[Serializable]
public class SoundType
{
    public Sounds soundType;
    public AudioClip soundClip;
}


public enum Sounds
{
    Music,
    ButtonClick,
    PlayerDeath,
    BloodVialPickUp,
    PoisonVialPickUp,
    SpeedBoostVialPickUp,
    ShieldVialPickUp,
    EnemyDeath,
    PlayerHurt,
    PlayerAttack,
    EnemyAttack
}
