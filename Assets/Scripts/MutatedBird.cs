using UnityEngine;

public class MutatedBird : MonoBehaviour
{
    [Header("Inscribed")]
    [Tooltip("The speed at which the MutatedBird moves")]
    [SerializeField]
    float speed = 1f;
    [Tooltip("The health of the MutatedBird")]
    public int health = 2;
    [Tooltip("The distance at which the MutatedBird will start following the player")]
    public float distance = 10;

    [Header("Dynamic")]
    [Tooltip("The player that the MutatedBird will follow")]
    public Transform player;
    [Tooltip("The collider of the MutatedBird")]
    public Collider col;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        col = GetComponent<Collider>();
    }
    
    void Update() {
        if (health == 0) {
            Destroy(gameObject);
        }
    }

    void FixedUpdate() {
        if (Vector3.Distance(transform.position, player.position) < distance) {
            transform.LookAt(player);
            transform.Rotate(new Vector3(0, -90, 0), Space.Self);
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
    }

    void OnCollisionEnter(Collision coll) {
        if (coll.gameObject.CompareTag("Player")) {
            //If the player hit the object when above the center of the collider
            if (coll.gameObject.transform.position.y > transform.position.y + col.bounds.extents.y) {
                //Die
                Destroy(gameObject);
            }
        }
    }
}
