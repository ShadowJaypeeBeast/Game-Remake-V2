using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerController : MonoBehaviour
{
    // Variables for physics and animation
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    // Enum to define the player's state
    private enum State { idle, running, jumping, falling, hurt };
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
        healthAmount.text = health.ToString();
    }

    private void HandleHealtht()
    {
        health -= 1;
        healthAmount.text = health.ToString();
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
    // Called once per frame
    void Update()
    {
        if (state != State.hurt)
        {
            Movement();
        }
    }

private void Movement()
    {
        // Get horizontal input
        float hDirection = Input.GetAxis("Horizontal");

        // Handle horizontal movement
        if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, 0);
            transform.localScale = new Vector2(1, 1); // Face right
        }
        else if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, 0);
            transform.localScale = new Vector2(-1, 1); // Face left
        }

        // Handle jumping
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            state = State.jumping; // Set state to jumping
        }

        // Update animation state
        VelocityState();
        anim.SetInteger("state", (int)state);
    }

    // Determine player's state based on velocity
    private void VelocityState()
    {
        if (state == State.jumping)
        {
            if (rb.velocity.y < .1f)
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
            if (Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle; // Transition to idle
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.running; // Transition to running
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
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (state == State.falling) // Fixed semicolon and corrected state comparison
            {
                Destroy(other.gameObject); // Destroy the enemy
            }
            else
            {
                state = State.hurt; // Set player state to hurt
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    // Enemy is to right
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    // Enemy to the left
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }
        }
    }
}


