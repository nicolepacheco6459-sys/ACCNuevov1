using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input;
    public GameObject notePrefab;
    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();

    int spawnIndex = 0;
    int inputIndex = 0;
    private bool canBeHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] notes)
    {
        foreach (var note in notes)
        {
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.midiFile.GetTempoMap());    
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if(SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.instance.noteTime)
            {
                var note = Instantiate(notePrefab, transform);
                notes.Add(note.GetComponent<Note>());
                note.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];
                spawnIndex++;
            }
        }
    
        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double marginOfError = SongManager.instance.marginOfError;
            double audioTime = SongManager.GetAudioSourceTime() - (SongManager.instance.inputDlayInMiliseconds / 1000.0);

            if (Input.GetKeyDown(input))
            {
                if(Math.Abs(audioTime - timeStamp) < marginOfError)
                {
                    Hit();
                    print($"Nota {inputIndex} perfecta");
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                }
                else
                {
                    print($"Fallaste en {inputIndex} la nota. Diferencia de tiempo: {Math.Abs(audioTime - timeStamp)}");
                }
            }
            if (timeStamp + marginOfError <= audioTime)
            {
                Miss();
                print($"Fallaste {inputIndex} la nota");
                inputIndex++;
            }
        }
        if (Input.GetKeyDown(KeyCode.F)) // o la tecla de la lane
        {
            if (canBeHit)
            {
                ScoreManager.Hit();
                Destroy(gameObject);
            }
            else
            {
                ScoreManager.Miss();
            }
        }
    }

    private void Hit()
    {
        ScoreManager.Hit();  
    }

    private void Miss()
    {
        ScoreManager.Miss();
    }
}
