using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 20f; 
    public float dashTime = 0.2f; 
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator; 
    private bool isMoving; 
    private bool isDead;
    private SpriteRenderer spriteRenderer; 
    private Vector2 screenBounds; 
    private bool isDashing; 
    private bool isAttacking; 
    public GameObject attackPrefab; 
    public float attackDelay = 0.3f; 
    private float lastAttackTime = 0f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        isMoving = false; 
        isDashing = false; 
        isAttacking = false; 
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

        if (Input.GetMouseButtonDown(0) && !isAttacking && Time.time > lastAttackTime + attackDelay) // Check for mouse click, if not already attacking, and if delay has passed
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            float yOffset = 0.5f; // Variable for adjusting y position
            Vector3 attackPosition = transform.position + new Vector3(spriteRenderer.flipX ? -1 : 1, yOffset, 0); // Position in front of the player, slightly higher
            float rotationAngle = spriteRenderer.flipX ? 210 : -150; // Set rotation angle based on sprite direction, flipping the prefab on y when facing right
            GameObject attackInstance = Instantiate(attackPrefab, attackPosition, Quaternion.Euler(0, spriteRenderer.flipX ? 0 : 180, rotationAngle)); // Spawn the attack object with a flipped rotation on y when facing right
            attackInstance.transform.SetParent(this.transform);
            Destroy(attackInstance, .3f); // Destroy the attack object after .3 seconds
            isAttacking = false;
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
        if (!isDashing && !isAttacking) // Only move normally if we're not dashing or attacking
        {
            Vector2 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, -screenBounds.x, screenBounds.x); // Clamp x position to screen bounds
            newPosition.y = Mathf.Clamp(newPosition.y, -screenBounds.y, screenBounds.y); // Clamp y position to screen bounds
            rb.MovePosition(newPosition);
        }
    }
}


