using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 20f; // Speed of the dash
    public float dashTime = 0.2f; // Duration of the dash
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator; // Added to handle animations
    private bool isMoving; // Declare isMoving variable
    private SpriteRenderer spriteRenderer; // Added to handle sprite flipping
    private Vector2 screenBounds; // Added to store screen bounds
    private bool isDashing; // Declare isDashing variable

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Initialize the Animator component
        spriteRenderer = GetComponent<SpriteRenderer>(); // Initialize the SpriteRenderer component
        isMoving = false; // Initialize isMoving as false
        isDashing = false; // Initialize isDashing as false
        animator.SetBool("isMoving", isMoving);
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)); // Calculate screen bounds
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        isMoving = Mathf.Abs(movement.x) > 0 || Mathf.Abs(movement.y) > 0;
        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        float startTime = Time.time; // Remember the time when we started dashing

        while (Time.time < startTime + dashTime)
        {
            rb.MovePosition(rb.position + movement * dashSpeed * Time.fixedDeltaTime);
            yield return null; // Leave the coroutine and return here in the next frame
        }

        isDashing = false;
    }

    void FixedUpdate()
    {
        if (!isDashing) // Only move normally if we're not dashing
        {
            Vector2 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, -screenBounds.x, screenBounds.x); // Clamp x position to screen bounds
            newPosition.y = Mathf.Clamp(newPosition.y, -screenBounds.y, screenBounds.y); // Clamp y position to screen bounds
            rb.MovePosition(newPosition);
        }
    }
}

