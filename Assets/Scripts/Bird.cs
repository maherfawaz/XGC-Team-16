using UnityEngine;

/*
 * BASE BIRD CLASS. GOOD FOR IDLE BEHAVIOR.
 */

public class Bird : MonoBehaviour
{
    [Header("Base: Sound Effects")]
    [Tooltip("The time at which the Bird plays teh SFX.")]
    [SerializeField] private AudioClip birdSFX;
    [SerializeField] private float birdSFXRate = 100f;
    private float birdSFXTimer = 0f;

    private void Awake() {

        // Randdom starting value for Bird SFX timer
        birdSFXTimer = Random.Range(0, birdSFXRate);
    }

    void Update(){
        PlayAudio();
    }

    protected void PlayAudio() {

        // PLAY BIRD SFX & RESET TIMER:
        if (birdSFXTimer >= birdSFXRate) {

            // Call Sound Effect Manager singleton
            SoundEffectManager.instance.PlaySound(birdSFX);

            // Reset timer
            birdSFXTimer = 0f;

            Debug.Log("Play Bird SFX");
        } 
        else {

            // Increment bird SFX timer
            birdSFXTimer += Time.deltaTime;
        }
    }
}
