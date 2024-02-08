using System.Collections;
using UnityEngine;

public class FistMovement : MonoBehaviour
{
    public float speed = 5f;
    public float slamSpeed = 10f;
    public float returnSpeed = 2f; // Speed at which the fist returns to max height
    public float minHeight = 0.5f;
    public float maxHeight = 5f;
    private bool slamming = false;
    private bool returning = false; // Added returning state
    private bool shaking = false;
    private Vector3 startPosition;
    private Vector3 slamPosition;
    private Camera mainCamera;
    private GameObject player;
    private float shakeDuration = 0.5f;
    private float shakeMagnitude = 0.1f;
    private bool movingRight = true; // Added to control direction

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
            // Move horizontally across the max height
            float step = speed * Time.deltaTime;
            if (movingRight)
            {
                transform.position += new Vector3(step, 0, 0);
                if (transform.position.x > mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x)
                {
                    movingRight = false; // Change direction
                }
            }
            else
            {
                transform.position -= new Vector3(step, 0, 0);
                if (transform.position.x < mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x)
                {
                    movingRight = true; // Change direction
                }
            }

            // Check if player is directly below
            if (Mathf.Abs(transform.position.x - player.transform.position.x) < 0.5f && !shaking)
            {
                StartCoroutine(Shake());
            }
        }
        else if (slamming)
        {
            // Slam down
            transform.position = Vector3.MoveTowards(transform.position, slamPosition, slamSpeed * Time.deltaTime);
            if (Mathf.Abs(transform.position.y - minHeight) < 0.01f)
            {
                slamming = false; // Reset slamming state
                returning = true; // Start returning to start position
            }
        }
        else if (returning)
        {
            // Smoothly return to start position
            transform.position = Vector3.MoveTowards(transform.position, startPosition, returnSpeed * Time.deltaTime);
            if (transform.position == startPosition)
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
        slamPosition = new Vector3(transform.position.x, minHeight, transform.position.z);
        shaking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit");
        }
    }
}