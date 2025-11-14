using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Player : PhysicsObject
{
    public float maxSpeed = 7;
    public LayerMask groundLayer;
    public float jumpTakeOffSpeed = 7;
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    public float jumpForce = 10f;
    public UnityEngine.Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isGrounded;
    public float horizontalInput;

    public int maxJumps = 2;
    private int jumpCount = 0;

    //Vector2 currentPosition = UnityEngine.Transform.position; 

    [Header("Lose Condition")]
    public float fallLimit = -10f;
    public GameObject gameOverScreen;
    private bool isGameOver = false;

    [Header("Win Condition")]
    public int totalKeys = 4;
    private int collectedKeys = 0;
    public GameObject winScreen;
    private bool hasWon = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }



    //void LateUpdate()
    //{
    //   if (!isGameOver && !hasWon && currentPosition.y < fallLimit)
    //  {
    //      Debug.Log("Game Over! Player fell.");
    //      GameOver();
    //      isGameOver = true;
    //  }
    //}

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded)
        {
            StartCoroutine(Jump());
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y *= 0.5f;
            }
        }

        if (move.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (move.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        animator.SetFloat("velocityY", velocity.y / maxSpeed);

        targetVelocity = move * maxSpeed;
    }

    IEnumerator Jump()
    {
        yield return new WaitForSeconds(0.1f);
        velocity.y = jumpTakeOffSpeed;
    }

    void GameOver()
    {
        if (hasWon) return;
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Key"))
        {
            Destroy(collision.gameObject);
            collectedKeys++;
            Debug.Log("Key collected: " + collectedKeys + "/" + totalKeys);
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
            if (collision.CompareTag("Enemy"))
            {
                Debug.Log("Hit enemy! Game Over.");
                GameOver();
                isGameOver = true;
            }
        }
    }

    void WinGame()
    {
        hasWon = true;
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        horizontalInput = moveInput;
        MovePlayer();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }
        setAnimationState(moveInput);
    }

    private void MovePlayer()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }
    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
    {
        jumpCount = 0; // Reset jump count when grounded
    }
    }

    private void setAnimationState(float moveinput)
    {
        if (!isGrounded)
        {
            if (moveinput == 0)
            {
                animator.Play("Idle");
            }
            else
            {
                animator.Play("Run_0");
            }
        }
    }
}
