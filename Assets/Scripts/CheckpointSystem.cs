using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    private GameObject player;
    [Header("Info")]
    [SerializeField] private float velocityRespawnLimit = -10f;
    [SerializeField][ReadOnly] private MovementV2 movement;
    [SerializeField][ReadOnly] private Vector3 currentCheckpointPosition;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip checkpointSFX;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        movement = player.GetComponent<MovementV2>();
        currentCheckpointPosition = player.transform.position;
    }

    void Update()
    {
        // CHECK IF PLAYER VELOCITY HAS REACHED FALLING LIMIT
        if (movement.GetVelocityY() < velocityRespawnLimit)
        {
            // TELEPORT TO LAST CHECKPOINT POSITION
            StartCoroutine(movement.ResetPosition(currentCheckpointPosition));
            Debug.Log("Player respawned at checkpoint!");
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // UPDATE NEW CHECKPOINT
        if (hit.gameObject.tag == "Checkpoint")
        {
            currentCheckpointPosition = hit.gameObject.transform.position;
            Destroy(hit.gameObject);

            SoundEffectManager.instance.PlaySound(checkpointSFX);

            Debug.Log("Checkpoint updated!");
        }
    }
}
