using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Variables for physics and animation
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    // Enum to define the player's state
    private enum State { idle, run, jump, falling, hurt };
    private State state = State.idle; // Initial state

    // Serialized fields for setting parameters in the inspector
    [SerializeField] private LayerMask ground;
    [SerializeField] public int cherry = 0;
    [SerializeField] private TextMeshProUGUI cherryText;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float hurtForce = 10f;
    [SerializeField] private int health;
    [SerializeField] private TextMeshProUGUI healthAmount;

    // Called before the first frame update
    void Start()
    {
        // Initialize components
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        // Ensure health is displayed at the start
        healthAmount.text = health.ToString();
    }

    // Called once per frame
    void Update()
    {
        if (state != State.hurt || Mathf.Abs(rb.velocity.x) < 0.1f) // Allow movement when recovering
        {
            Movement();
        }

        VelocityState(); // Ensure state updates properly
        anim.SetInteger("state", (int)state); // Update animation state
    }

    private void HandleHealth() // Renamed to fix the typo ("HandleHealtht")
    {
        health -= 1; // Decrease health by 1
        healthAmount.text = health.ToString(); // Update UI

        if (health <= 0) // Reload scene when health is 0
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Movement()
    {
        // Get horizontal input
        float hDirection = Input.GetAxis("Horizontal");

        // Handle horizontal movement
        if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y); // Fixed Y velocity retention
            transform.localScale = new Vector2(1, 1); // Face right
        }
        else if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y); // Fixed Y velocity retention
            transform.localScale = new Vector2(-1, 1); // Face left
        }

        // Handle jumping
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Jump by adding Y velocity
            state = State.jump; // Set state to jumping
        }

        // Update animation state
        VelocityState();
        anim.SetInteger("state", (int)state);
    }

    // Determine player's state based on velocity
    private void VelocityState()
    {
        if (state == State.jump)
        {
            if (rb.velocity.y < 0.1f)
            {
                state = State.falling; // Transition to falling
            }
        }
        else if (state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle; // Transition to idle
            }
        }
        else if (state == State.hurt)
                {
                    if (Mathf.Abs(rb.velocity.x) < 0.5f) // Lower threshold to transition back faster
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y); // Stop lingering knockback
                        state = State.idle;
                    }
                }
                else if (Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.run; // Transition to running
        }
        else
        {
            state = State.idle; // Default to idle
        }
    }

    // Handle trigger collisions
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable")
        {
            Destroy(collision.gameObject); // Destroy collectable
            cherry += 1; // Increment cherry count
            cherryText.text = cherry.ToString(); // Update UI
        }
    }

    // Handle collisions with enemies
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (state == State.falling)
            {
                Destroy(other.gameObject); // Destroy the enemy
            }
            else
            {
                state = State.hurt; // Set player state to hurt

                // Handle knockback direction
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y); // Knockback to the left
                }
                else
                {
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y); // Knockback to the right
                }

                HandleHealth(); // Reduce health when hurt
            }
        }
    }
}
