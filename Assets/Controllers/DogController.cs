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

    private float sideRotation;
    private float rotationSmooth;
    private float angle;
    private RaycastHit2D[] hits;
    private int h;
    private int speedR;
    private RaycastHit2D hitsMain;

    void Start()
    {
        hits = new RaycastHit2D[2];
        target = player.GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        jumpForce = 2f;
        groundCheckDistance = 1.5f;
        speedR = 80;
        Physics2D.IgnoreLayerCollision(10, 11);
    }

    void FixedUpdate()
    {

        // Rotacionar
        h = Physics2D.RaycastNonAlloc(transform.position, -Vector2.up, hits); //cast downwards
        if (h > 1)
        { //if we hit something do stuff

            if (hits[0].collider.tag == "Ground")
            {
                hitsMain = hits[0];
            }  else if (hits[1].collider.tag == "Ground")
            {
                hitsMain = hits[1];
            }

            if (hitsMain != null)
            {
                sideRotation = hitsMain.normal.x;



                if (IsGrounded())
                {
                    angle = Mathf.Abs(Mathf.Atan2(hitsMain.normal.x, hitsMain.normal.y) * Mathf.Rad2Deg); //get angle
                }
                else
                {

                    angle = 0;
                }

                if (rotationSmooth <= angle)
                {
                    rotationSmooth += Time.deltaTime * speedR;
                }


                if (rotationSmooth >= angle)
                {
                    rotationSmooth -= Time.deltaTime * speedR;
                }

                if (sideRotation > 0) // Esquerdo
                {

                    transform.rotation = Quaternion.Euler(0, 0, -rotationSmooth);


                }
                else if (sideRotation < 0)// Direito
                {

                    transform.rotation = Quaternion.Euler(0, 0, rotationSmooth);

                }
                else // Reto
                {

                    transform.rotation = Quaternion.Euler(0, 0, -rotationSmooth);
                }
            }

        }







        // Mover pra perto do Player
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

    // Checa se está no chão
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