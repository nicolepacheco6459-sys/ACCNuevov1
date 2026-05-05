using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input;
    public GameObject notePrefab;

    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();

    [Header("Sprites")]
    public Sprite leftSprite;
    public Sprite downSprite;
    public Sprite upSprite;
    public Sprite rightSprite;

    public enum Direction
    {
        Left,
        Down,
        Up,
        Right
    }

    public Direction direction;

    int spawnIndex = 0;
    int inputIndex = 0;

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] notes)
    {
        foreach (var note in notes)
        {
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(
                    note.Time,
                    SongManager.midiFile.GetTempoMap()
                );

                timeStamps.Add(
                    (double)metricTimeSpan.Minutes * 60f +
                    metricTimeSpan.Seconds +
                    (double)metricTimeSpan.Milliseconds / 1000f
                );
            }
        }
    }

    void Update()
    {
        notes.RemoveAll(n => n == null);

        // SPAWN DE NOTAS
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.instance.noteTime)
            {
                var noteObj = Instantiate(notePrefab, transform);
                Note noteScript = noteObj.GetComponent<Note>();

                noteScript.assignedTime = (float)timeStamps[spawnIndex];
                noteScript.SetDirection(direction, GetSpriteByDirection());

                notes.Add(noteScript);
                spawnIndex++;
            }
        }

        // INPUT DEL JUGADOR
        if (Input.GetKeyDown(input))
        {
            if (notes.Count > inputIndex)
            {
                Note note = notes[inputIndex];

                if (note.CanBeHit())
                {
                    Hit("Perfect");
                    Destroy(note.gameObject);
                    notes.RemoveAt(inputIndex);
                }
                else
                {
                    Miss();

                    if (notes.Count > inputIndex && notes[inputIndex] != null)
                    {
                        Destroy(notes[inputIndex].gameObject);
                        notes.RemoveAt(inputIndex);
                    }
                }
            }
        }

        if (spawnIndex >= timeStamps.Count && notes.Count == 0)
        {
            SongManager.instance.audioSource.Stop();
            FindFirstObjectByType<GameManager_MUSIC>().GameOver();
        }

    }

    // SPRITE SEGÚN DIRECCIÓN
    Sprite GetSpriteByDirection()
    {
        switch (direction)
        {
            case Direction.Left: return leftSprite;
            case Direction.Down: return downSprite;
            case Direction.Up: return upSprite;
            case Direction.Right: return rightSprite;
        }
        return null;
    }

    private void Hit(string accuracy)
    {
        ScoreManager.Hit(accuracy);
    }

    private void Miss()
    {
        ScoreManager.Miss();
    }
}
