using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;


public class SongManager : MonoBehaviour
{
    public static SongManager instance;
    public AudioSource audioSource;
    public Lane[] lanes;
    public float songDelayInSeconds;
    public double marginOfError; // in seconds

    public int inputDlayInMiliseconds;
    

    public string fileLocation;
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
    // Start is called before the first frame update 

    void Start()
    {
        instance = this;
        if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://"))
        {
            StartCoroutine(ReadFromWebSite());
        }
        else
        {
            ReadFromFile();
        }
    }
    private void ReadFromFile()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileLocation);

        if (!File.Exists(path))
        {
            Debug.LogError("No se encontró el MIDI en: " + path);
            return;
        }

        midiFile = MidiFile.Read(path);

        Debug.Log("MIDI cargado correctamente");

        GetDataFromMidi(); // ← ESTA LÍNEA FALTABA
    }
    private IEnumerator ReadFromWebSite()
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
                    foreach (var note in midiFile.GetNotes())
                    {
                        Debug.Log("Nota MIDI detectada: " + note.NoteName);
                    }
                }
            }
        }
  
    }

    private object SendWebRequest()
    {
        throw new NotImplementedException();
    }

    public void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes();

        Debug.Log("Total notas MIDI: " + notes.Count);

        foreach (var note in notes)
        {
            Debug.Log("Nota detectada: " + note.NoteName + " / Numero: " + note.NoteNumber);
        }

        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        foreach (var lane in lanes)
            lane.SetTimeStamps(array);

        Invoke(nameof(StartSong), songDelayInSeconds);
    }
    public void StartSong() 
    {
        audioSource.Play();
    }

    public static double GetAudioSourceTime()
    {
        return (double)instance.audioSource.timeSamples / instance.audioSource.clip.frequency;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    
}
