using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    public static DogController instance { set; get; }
    public float jumpForce;
    public float speed;
    public GameObject player;
    private Transform target;
    public float stoppingDistance;
    private Rigidbody2D rb;
    public LayerMask groundLayers;
    public float groundCheckDistance;

    private float sideRotation;
    private float rotationSmooth;
    private float angle;
    private RaycastHit2D[] hits;
    private int h;
    private int speedR;
    private RaycastHit2D hitsMain;
    public bool isGrounded = false;


    Vector2 relativePoint;

    void Start()
    {
        hits = new RaycastHit2D[2];
        target = player.GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();

        groundCheckDistance = 1.3f;
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
            }
            else if (hits[1].collider.tag == "Ground")
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


        if (IsGrounded())
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }


            relativePoint = transform.InverseTransformPoint(target.position);


            // Mover pra perto do Player
            if (Mathf.Round(Vector2.Distance(transform.position, target.position)) > stoppingDistance)
            {

                if (Mathf.Round(relativePoint.x) < 0.0)
                { // Right
                    rb.velocity = new Vector2(-speed, rb.velocity.y);

                }
                else if (Mathf.Round(relativePoint.x) > 0.0)
                { // Left
                    rb.velocity = new Vector2(speed, rb.velocity.y);

                }
                else if (Mathf.Round(relativePoint.x) == 0)
                {

                    rb.velocity = new Vector2(0, rb.velocity.y);
                }



            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
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