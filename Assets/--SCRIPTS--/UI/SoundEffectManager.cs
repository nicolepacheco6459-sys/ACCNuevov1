using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
   private static SoundEffectManager Instance;

   private AudioSource audioSource;
   private SoundEffectLibrary soundEffectLibrary;
   [SerializeField] private Slider sfxSlider;

   private void Awake()
   {
       if (Instance == null)
       {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();

            // Make this object root to avoid the warning
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
       }
       else
       {
           Destroy(gameObject);
       }
   }

   public static void Play(string soundName)
    {
        if (Instance == null) return;
        AudioClip audioClip = Instance.soundEffectLibrary.GetRandomClip(soundName);
        if (audioClip != null && Instance.audioSource != null)
        {
            Instance.audioSource.PlayOneShot(audioClip);
        }
    }

    void Start()
    {
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnValueChanged);
    }

    public static void SetVolume(float volume)
    {
        if (Instance != null && Instance.audioSource != null)
            Instance.audioSource.volume = volume;
    }

    public void OnValueChanged(float value)
    {
        SetVolume(value);
    }}
