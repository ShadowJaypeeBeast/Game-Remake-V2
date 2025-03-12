using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    //Jumping mechanics
    private Collider2D coll;
    [SerializeField] private LayerMask ground;
    public int cherry = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Jumping mechanics
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float hDirection = Input.GetAxis("Horizontal");

        if (hDirection > 0)
        {
            rb.velocity = new Vector2(5, 0);
            //flipping of character
            transform.localScale = new Vector2(1, 1);
        }

        else if (hDirection < 0)
        {
            rb.velocity = new Vector2(-5, 0);
            //flipping of character
            transform.localScale = new Vector2(-1, 1);
        }
        // Jumping mechanics
        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, 10f);
        }
    }
public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            cherry += 1;
        }
    }
}