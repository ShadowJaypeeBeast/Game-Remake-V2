using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Physics and animation components for controlling player behavior
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    // Define player states for animation and behavior control
    private enum State { idle, run, jump, falling, hurt };
    private State state = State.idle; // Set initial state to idle

    // Parameters for setting gameplay variables via Unity inspector
    [SerializeField] private LayerMask ground; // Specify what counts as ground
    [SerializeField] public int cherry = 0; // Track collected cherries
    [SerializeField] private TextMeshProUGUI cherryText; // UI element for cherry count
    [SerializeField] private float speed = 5f; // Player horizontal movement speed
    [SerializeField] private float jumpForce = 10f; // Force applied when jumping
    [SerializeField] private float hurtForce = 10f; // Force applied during knockback
    [SerializeField] private int health; // Player's health points
    [SerializeField] private TextMeshProUGUI healthAmount; // UI element for health display
    [SerializeField] private float countdownTime = 60f; // Time limit for the level
    [SerializeField] private TextMeshProUGUI countdownText; // UI element for the timer
    [SerializeField] private TextMeshProUGUI levelMessage; // UI element for level restart messages

    private bool isTimeUp; // Flag to indicate if the timer has expired

    // Initialize components and set initial UI values
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        healthAmount.text = health.ToString(); // Display initial health
    }

    // Update is called once per frame to handle movement and game logic
    void Update()
    {
        // Allow movement if not recovering from being hurt
        if (state != State.hurt || Mathf.Abs(rb.velocity.x) < 0.1f)
        {
            Movement();
        }

        // Update animation state based on player's velocity
        VelocityState();
        anim.SetInteger("state", (int)state); // Sync animation with state

        // Update the level timer
        UpdateTimer();
    }

    // Countdown timer for the level
    private void UpdateTimer()
    {
        if (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime; // Decrease time remaining
            countdownText.text = Mathf.Ceil(countdownTime).ToString(); // Display time in whole seconds
        }
        else if (!isTimeUp) // Prevent multiple triggers
        {
            isTimeUp = true;
            RestartLevel(); // Handle level restart when time runs out
        }
    }

    // Display a message and restart the level after a short delay
    private void RestartLevel()
    {
        levelMessage.text = "Time Run Out! Restarting level...";
        Debug.Log("Time Run Out! Restarting level...");
        StartCoroutine(RestartAfterDelay());
    }

    // Coroutine to delay level restart by 2 seconds
    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    // Handle player movement and jumping
    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal"); // Get horizontal input

        // Move right or left based on input
        if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y); // Move right while keeping vertical velocity
            transform.localScale = new Vector2(1, 1); // Face right
        }
        else if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y); // Move left while keeping vertical velocity
            transform.localScale = new Vector2(-1, 1); // Face left
        }

        // Jump if grounded and jump button is pressed
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Add upward velocity for jump
            state = State.jump; // Set state to jumping
        }
    }

    // Update player's state based on velocity and collisions
    private void VelocityState()
    {
        if (state == State.jump)
        {
            if (rb.velocity.y < 0.1f)
            {
                state = State.falling; // Transition to falling if ascending velocity decreases
            }
        }
        else if (state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle; // Transition to idle upon landing
            }
        }
        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < 0.5f) // Stop lingering knockback
            {
                rb.velocity = new Vector2(0, rb.velocity.y); // Stop horizontal movement
                state = State.idle;
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.run; // Transition to running if horizontal velocity is high
        }
        else
        {
            state = State.idle; // Default state is idle
        }
    }

    // Reduce health and restart scene if health reaches zero
    private void HandleHealth()
    {
        health -= 1; // Decrease health
        healthAmount.text = health.ToString(); // Update health UI

        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current level
        }
    }

    // Handle collection of items like cherries
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable")
        {
            Destroy(collision.gameObject); // Remove collected item
            cherry += 1; // Increment cherry count
            cherryText.text = cherry.ToString(); // Update cherry count UI
        }
    }

    // Handle collisions with enemies
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (state == State.falling)
            {
                Destroy(other.gameObject); // Defeat enemy by jumping on it
            }
            else
            {
                state = State.hurt; // Transition to hurt state

                // Apply knockback based on collision direction
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y); // Knock back to the left
                }
                else
                {
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y); // Knock back to the right
                }

                HandleHealth(); // Reduce player's health
            }
        }
    }
}