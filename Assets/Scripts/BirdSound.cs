using UnityEngine;

public class BirdSound : MonoBehaviour
{
    public AudioClip birdSound;
    
    void Update() {
        int randomValue = Random.Range(1, 1000);
        if (randomValue == 50 || randomValue == 25) {
            PlayBirdSound();
        }
    }

    void PlayBirdSound() {
        AudioSource.PlayClipAtPoint(birdSound, transform.position);
    }
}
