using UnityEngine;

public class Beetle : MonoBehaviour
{
    public float xVelocity = 3f;
    private Rigidbody2D beetleRb;
    private SpriteRenderer spriteRend;
    public float castDist = 0.2f;
    private Vector2 castDir;

    void Start()
    {
        beetleRb = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();
        castDir = Vector2.right;
    }

    void FixedUpdate()
    {
        // Keep moving horizontally
        beetleRb.linearVelocity = new Vector2(xVelocity, beetleRb.linearVelocity.y);
    }

    void Update()
    {
        // Raycast to detect walls and flip direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir, castDist);
        if (hit.collider != null && hit.collider.tag != "Player")
        {
            // Flip sprite
            spriteRend.transform.localScale = new Vector3(-spriteRend.transform.localScale.x, 1, 1);
            xVelocity *= -1;
            castDir.x *= -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Kill the player and show game over
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.SendMessage("GameOver", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
