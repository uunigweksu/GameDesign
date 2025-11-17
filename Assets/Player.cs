using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : PhysicsObject
{
    [Header("Movement")]
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Lose Condition")]
    public float fallLimit = -10f;
    public GameObject gameOverScreen;
    private bool isGameOver = false;

    [Header("Win Condition")]
    public int totalKeys = 4;
    private int collectedKeys = 0;
    public GameObject winScreen;
    private bool hasWon = false;

    [Header("UI")]
    public TextMeshProUGUI keyCounterText;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        if (!isGameOver && !hasWon && transform.position.y < fallLimit)
        {
            Debug.Log("Game Over! Player fell.");
            GameOver();
            isGameOver = true;
        }
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y *= 0.5f;
            }
        }

        if (move.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (move.x < -0.01f)
            spriteRenderer.flipX = true;

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        animator.SetFloat("velocityY", velocity.y / maxSpeed);

        targetVelocity = move * maxSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Key"))
        {
            collectedKeys++;

            if (keyCounterText != null)
                keyCounterText.text = collectedKeys + " / " + totalKeys;

            Destroy(collision.gameObject);

            Debug.Log("Gem collected: " + collectedKeys + "/" + totalKeys);
        }

        
        if (collision.CompareTag("Enemy") && !isGameOver)
        {
            Debug.Log("Hit enemy! Game Over.");
            GameOver();
            isGameOver = true;
        }

        
        if (collision.CompareTag("Door"))
        {
            if (collectedKeys >= totalKeys)
            {
                WinGame();
            }
            else
            {
                Debug.Log("You need all keys to finish!");
            }
        }
    }

    void GameOver()
    {
        if (hasWon) return;

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        Time.timeScale = 0f;
    }

    void WinGame()
    {
        hasWon = true;

        if (winScreen != null)
            winScreen.SetActive(true);

        Time.timeScale = 0f;
    }
}
