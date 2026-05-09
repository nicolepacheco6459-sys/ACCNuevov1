using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;

    SpriteRenderer sr;

    public void SetDirection(Lane.Direction dir, Sprite sprite)
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
    }

    void Start()
    {
        timeInstantiated = SongManager.GetAudioSourceTime();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false; // se activa cuando empieza a moverse
    }

    void Update()
    {
        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated;

        // t controla el progreso de la nota (0 = spawn, 1 = tap zone)
        float t = (float)(timeSinceInstantiated / (SongManager.instance.noteTime * 2));

        if (t > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Movimiento limpio y consistente
        transform.localPosition = Vector3.Lerp(
            new Vector3(0, SongManager.instance.noteSpawnY, 0),
            new Vector3(0, SongManager.instance.noteTapY, 0),
            t
        );

        // Activar render cuando ya está en movimiento
        if (!sr.enabled)
            sr.enabled = true;
    }
    public bool CanBeHit()
    {
        float distance = Mathf.Abs(transform.localPosition.y - SongManager.instance.noteTapY);
        return distance < 0.5f; // ajusta este valor
    }
}