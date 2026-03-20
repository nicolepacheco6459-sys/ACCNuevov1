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

            // ensure we're preserving the root GameObject
            GameObject root = gameObject.transform.root.gameObject;
            if (root != gameObject)
                Debug.LogWarning("SoundEffectManager is not on a root GameObject; DontDestroyOnLoad will be applied to the root instead.");
            DontDestroyOnLoad(root);
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
