using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    SWORD,
    MAGIC,
    LAND,
    JUMP,
    HURT,
    HURTMAGE,
    FOOTSTEP
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] SoundList[] soundList;
    [SerializeField] AudioClip[] music;
    [SerializeField] AudioSource sfxAudioSource; // Requires spatial blend, so this will go on the camera in FPV/TPV Games, but if it's a top-down or a 2D game, it's better to put it on the player cuz the player and the camera are not at the same spot.
    [SerializeField] AudioSource musicAudioSource; // Ideally on the manager itself. Since it's music and doesn't require spatial blend.

    public void PlaySound(SoundType sound, float volume = 1f)
    {
        AudioClip[] clips = soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        // randomize pitch and volume between 0.85 and 1.15
        float randomPitch = UnityEngine.Random.Range(0.85f, 1.15f);
        float randomVolume = UnityEngine.Random.Range(0.85f, 1.15f) * volume;
        // sfxAudioSource.pitch = randomPitch;

        sfxAudioSource.PlayOneShot(randomClip, randomVolume);
        // sfxAudioSource.pitch = 1f; // reset pitch after playing
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        musicAudioSource.clip = clip;
        musicAudioSource.volume = volume;
        musicAudioSource.Play();
    }

    void OnEnable()
    {
#if UNITY_EDITOR
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++){
            soundList[i].name = names[i];
        }
#endif
    }
}

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [SerializeField] AudioClip[] sounds;
    public AudioClip[] Sounds { get => sounds; }
}

