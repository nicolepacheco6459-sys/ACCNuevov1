using UnityEngine;
using System;
using System.Collections.Generic;

public class SoundEffectLibrary : MonoBehaviour
{
    private Dictionary<string, List<AudioClip>> soundDictionary;

    [Header("Groups")]
    [Tooltip("Populate groups here to organize your sound effects by name.")]
    public List<SoundEffectGroup> groups = new List<SoundEffectGroup>();

    void Awake()
    {
        BuildDictionary();
    }

    public AudioClip GetRandomClip(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            List<AudioClip> clips = soundDictionary[name];
            if (clips.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, clips.Count);
                return clips[randomIndex];
            }
        }
        return null;
    }
    private void BuildDictionary()
    {
        soundDictionary = new Dictionary<string, List<AudioClip>>();
        foreach (var g in groups)
        {
            if (string.IsNullOrEmpty(g.name))
                continue;

            if (!soundDictionary.ContainsKey(g.name))
                soundDictionary[g.name] = new List<AudioClip>(g.audioClips);
        }
    }

    [Serializable]
    public struct SoundEffectGroup
    {
        public string name;
        public List<AudioClip> audioClips;
    }
}