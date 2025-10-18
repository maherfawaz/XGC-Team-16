using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager instance;
    private AudioSource audioSource;

    [SerializeField] private float pitchMin = 0.9f;
    [SerializeField] private float pitchMax = 1.1f;

    private void Awake() 
    {
        // SINGLETON PATTERN
        if (instance == null) 
        {
            instance = this;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip) 
    {
        // ADJUST PITCH FOR VARIETY
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.PlayOneShot(clip);
    }
}
