using UnityEngine;

public class LadderClimb : MonoBehaviour
{
    private bool isClimbing = false;
    private Rigidbody2D playerRigidbody;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isClimbing = true;
            playerRigidbody = other.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {
                playerRigidbody.gravityScale = 0;
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0); // Reset vertical movement
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopClimbing();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isClimbing && other.CompareTag("Player"))
        {
            float vertical = Input.GetAxis("Vertical");

            if (playerRigidbody != null)
            {
                if (Mathf.Abs(vertical) > 0.1f)
                {
                    // ✅ Climbing movement
                    playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, vertical * 3f);
                }
                else
                {
                    // ✅ Stop vertical movement when no input
                    playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
                }

                // ✅ Stop climbing and restore gravity on jump
                if (Input.GetButtonDown("Jump"))
                {
                    StopClimbing();
                }
            }
        }
    }

    private void StopClimbing()
    {
        isClimbing = false;
        if (playerRigidbody != null)
        {
            // ✅ Restore gravity immediately
            playerRigidbody.gravityScale = 1;
            // ✅ Let PlayerController handle jumping — avoid overriding velocity!
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y);
            playerRigidbody = null;
        }
    }
}
