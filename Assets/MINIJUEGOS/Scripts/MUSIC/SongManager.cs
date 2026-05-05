using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class SongManager : MonoBehaviour
{
    public static SongManager instance;

    [Header("Audio")]
    public AudioSource audioSource;
    public float songDelayInSeconds = 1f;

    [Header("Lanes")]
    public Lane[] lanes;

    [Header("Timing")]
    public double marginOfError;
    public int inputDelayInMilliseconds;

    // ✅ ENUM SEPARADO (SIN HEADER)
    public enum Difficulty { Easy, Medium, Hard }

    [Header("Difficulty")]
    public Difficulty difficulty;

    [Header("MIDI")]
    public string fileLocation;

    [Header("Note Movement")]
    public float noteTime;
    public float noteSpawnY;
    public float noteTapY;

    public float noteDespawnY
    {
        get
        {
            return noteTapY - (noteSpawnY - noteTapY);
        }
    }

    public static MidiFile midiFile;

    void Start()
    {
        instance = this;

        SetupDifficulty();

        // 🔥 Valores clave para que se vean las notas
        noteSpawnY = 5f;
        noteTapY = -3f;

        if (Application.streamingAssetsPath.StartsWith("http://") ||
            Application.streamingAssetsPath.StartsWith("https://"))
        {
            StartCoroutine(ReadFromWebSite());
        }
        else
        {
            ReadFromFile();
        }
    }

    void SetupDifficulty()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                marginOfError = 0.25;
                noteTime = 1.5f;
                break;

            case Difficulty.Medium:
                marginOfError = 0.15;
                noteTime = 1.2f;
                break;

            case Difficulty.Hard:
                marginOfError = 0.08;
                noteTime = 0.9f;
                break;
        }
    }

    void ReadFromFile()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileLocation);

        if (!File.Exists(path))
        {
            Debug.LogError("No se encontró el MIDI en: " + path);
            return;
        }

        midiFile = MidiFile.Read(path);

        Debug.Log("MIDI cargado correctamente");

        GetDataFromMidi();
    }

    IEnumerator ReadFromWebSite()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileLocation))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                byte[] results = www.downloadHandler.data;

                using (var stream = new MemoryStream(results))
                {
                    midiFile = MidiFile.Read(stream);
                    GetDataFromMidi();
                }
            }
        }
    }

    public void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes();

        Debug.Log("Total notas MIDI: " + notes.Count);

        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        foreach (var lane in lanes)
        {
            lane.SetTimeStamps(array);
        }

        
    }

    public void StartSong()
    {
        audioSource.Play();
    }

    public static double GetAudioSourceTime()
    {
        return (double)instance.audioSource.timeSamples / instance.audioSource.clip.frequency;
    }
}
