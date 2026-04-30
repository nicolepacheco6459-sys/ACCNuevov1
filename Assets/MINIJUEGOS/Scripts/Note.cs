using System.Collections;   
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    double timeInstanted; 
    public float assignedTime; 
    void Start()
    {
        timeInstanted = SongManager.GetAudioSourceTime();
    }
    bool canBeHit = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HitZone"))
            canBeHit = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HitZone"))
            canBeHit = false;
    }
    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstanted;
        float t = (float)(timeSinceInstantiated / (SongManager.instance.noteTime * 2));

        
        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(
                Vector3.up * SongManager.instance.noteSpawnY,
                Vector3.up * SongManager.instance.noteTapY,
                t
            );
            GetComponent<SpriteRenderer>().enabled = true;
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

}