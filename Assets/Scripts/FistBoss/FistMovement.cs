using System.Collections;
using UnityEngine;

public class FistMovement : MonoBehaviour
{
    public float speed = 5f;
    public float slamSpeed = 10f;
    public float returnSpeed = 2f; // Speed at which the fist returns to its position before the slam
    public float minHeight = 0.5f;
    public float maxHeight = 5f;
    private bool slamming = false;
    private bool returning = false; // Added returning state
    private bool shaking = false;
    private Vector3 startPosition;
    private Vector3 preSlamPosition; // Position before the slam
    private Vector3 slamPosition;
    private Camera mainCamera;
    private GameObject player;
    private float shakeDuration = 0.5f;
    private float shakeMagnitude = 0.1f;
    public ParticleSystem slamEffect; // Particle system for the slam effect

    void Start()
    {
        mainCamera = Camera.main;
        startPosition = new Vector3(transform.position.x, maxHeight, transform.position.z); // Set start position to top
        transform.position = startPosition; // Move fist to start position
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (!slamming && !returning)
        {
            // Move towards the player's position at maxHeight
            Vector3 playerPositionAtMaxHeight = new Vector3(player.transform.position.x, maxHeight, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, playerPositionAtMaxHeight, speed * Time.deltaTime);

            // Check if player is directly below
            if (Mathf.Abs(transform.position.x - player.transform.position.x) < 0.5f && !shaking)
            {
                preSlamPosition = transform.position; // Save current position before starting the shake
                StartCoroutine(Shake());
            }
        }
        else if (slamming)
        {
            // Slam down to the player's current y position, maintaining the fist's current x position
            Vector3 playerPositionAtCurrentHeight = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, playerPositionAtCurrentHeight, slamSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, playerPositionAtCurrentHeight) < 0.01f)
            {
                slamming = false; // Reset slamming state
                returning = true; // Start returning to pre-slam position
                CreateSlamEffect(); // Create particle effect at the point of impact
            }
        }
        else if (returning)
        {
            // Smoothly return to pre-slam position
            transform.position = Vector3.MoveTowards(transform.position, preSlamPosition, returnSpeed * Time.deltaTime);
            if (transform.position == preSlamPosition)
            {
                returning = false; // Reset returning state
            }
        }
    }

    IEnumerator Shake()
    {
        shaking = true;
        Vector3 originalPosition = transform.position;

        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition;
        slamming = true;
        // Update slamPosition to target the player's current y position while maintaining the fist's x position
        slamPosition = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        shaking = false;
    }

    private void CreateSlamEffect()
    {
        if (slamEffect != null)
        {
            
            var go = Instantiate(slamEffect, transform.position, Quaternion.identity);
            go.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit");
        }
    }
}