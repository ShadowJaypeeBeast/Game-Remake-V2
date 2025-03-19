using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // SerializeField allows you to adjust these values in the Unity Inspector
    [SerializeField] private float leftCap; // Left boundary for movement
    [SerializeField] private float rightCap; // Right boundary for movement
    [SerializeField] private float jumpLength = 2; // Horizontal movement speed
    [SerializeField] private float jumpHeight = 2; // Vertical jump strength
    [SerializeField] private LayerMask ground; // Layer mask to identify the ground

    private Collider2D Coll; // Reference to the enemy's collider component
    private Rigidbody2D Rb; // Reference to the enemy's Rigidbody2D component
    private bool facingLeft = true; // Direction the enemy is facing

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the collider and rigidbody components
        Coll = GetComponent<Collider2D>();
        Rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Physics-related logic is better handled in FixedUpdate()
        MoveEnemy();
    }

    void FixedUpdate()
    {
        MoveEnemy();
    }

    // Function to handle enemy movement
    private void MoveEnemy()
    {
        if (facingLeft)
        {
            // Check if the enemy is within bounds on the left side
            if (transform.position.x > leftCap)
            {
                // Ensure the enemy is facing left
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }

                // Check if the enemy is grounded before jumping
                if (Coll.IsTouchingLayers(ground))
                {
                    Rb.velocity = new Vector2(-jumpLength, jumpHeight); // Jump left
                }
            }
            else
            {
                // Reverse direction when reaching the left boundary
                facingLeft = false;
            }
        }
        else
        {
            // Check if the enemy is within bounds on the right side
            if (transform.position.x < rightCap)
            {
                // Ensure the enemy is facing right
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }

                // Check if the enemy is grounded before jumping
                if (Coll.IsTouchingLayers(ground))
                {
                    Rb.velocity = new Vector2(jumpLength, jumpHeight); // Jump right
                }
            }
            else
            {
                // Reverse direction when reaching the right boundary
                facingLeft = true;
            }
        }
    }
}
