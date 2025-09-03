using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutatedBird : MonoBehaviour
{
    [Header("Inscribed")]
    [Tooltip("The speed at which the MutatedBird moves")]
    [SerializeField]
    float speed = 1f;
    [Tooltip("The health of the MutatedBird")]
    public int health = 2;
    [Tooltip("The player that the MutatedBird will follow")]
    public Transform player;
    [Tooltip("The distance at which the MutatedBird will start following the player")]
    public float distance = 10;
    [Header("Dynamic")]
    [Tooltip("Whether the MutatedBird is invincible or not, depending on the distance from the player")]
    public bool invincible;

    void Update() {
        if (health == 0) {
            Destroy(gameObject);
        }
    }

    void FixedUpdate() {
        if (Vector3.Distance(transform.position, player.position) < distance) {
            invincible = false;
            transform.LookAt(player);
            transform.Rotate(new Vector3(0, -90, 0), Space.Self);
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        } else {
            invincible = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (invincible) return;
        if (collision.gameObject.CompareTag("Player")) {
            health -= 1;
        }
    }
}
