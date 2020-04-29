using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    private float jumpForce;
    public float speed;
    public GameObject player;
    private Transform target;
    public int stoppingDistance;
    private Rigidbody2D rb;
    public LayerMask groundLayers;
    private float groundCheckDistance;


    void Start()
    {
        target = player.GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        jumpForce = 1;
        groundCheckDistance = 2f;
    }

    void FixedUpdate()
    {

        if (Mathf.Round(Vector2.Distance(transform.position, target.position)) > stoppingDistance)
        {

            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }

        if (PlayerController.instance.isJumping)
        {
            if (IsGrounded())
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

    }

    private bool IsGrounded()
    {
        Ray2D ray = new Ray2D(transform.position, Vector2.down);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, groundCheckDistance, groundLayers);
        if (hit)
        {

            return true;
        }
        else
        {

            return false;
        }
    }

}