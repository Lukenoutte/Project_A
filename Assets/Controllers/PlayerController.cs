using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { set; get; }
    private Rigidbody2D rb;

    private float speed;
    private float jumpForce;

    public bool swipeLeft, swipeRight, swipeUp, swipeDown;

    public bool isDragging;
    public Vector2 startTouch, startTouch2, swipeDelta, swipeDelta2;
    private float sideRotation;
    private float rotationSmooth;
    private int speedR;
    private bool isPressed;
    public bool isJumping;
    // isGRounded
    public LayerMask groundLayers;
    private float groundCheckDistance;

    //Angle
    private float angle;
    private RaycastHit2D[] hits;
    private int h;

    private RaycastHit2D hitsMain;


    // Start is called before the first frame update
    void Start()
    {
        isJumping = false;
        instance = this;
        groundCheckDistance = 2f;
        swipeDelta = Vector2.zero;
        swipeDelta2 = Vector2.zero;
        isDragging = false;
        hits = new RaycastHit2D[2];
        speedR = 80;

        rb = GetComponent<Rigidbody2D>();
        speed = 4;
        jumpForce = 7;

        swipeLeft = swipeRight = swipeUp = swipeDown = false;

    }


    void Update()
    {



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






        // Andar para direita
        if (swipeRight)
        {

            if (isPressed)
            {

                rb.velocity = new Vector2(speed, rb.velocity.y);
                GetComponent<Animator>().SetBool("WalkRight", true);
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                swipeRight = false;
                GetComponent<Animator>().SetBool("WalkRight", false);
            }

        }
        else
        if (swipeLeft)
        {

            if (isPressed)
            {
                GetComponent<Animator>().SetBool("WalkRight", true);
                rb.velocity = new Vector2(-speed, rb.velocity.y);
                GetComponent<SpriteRenderer>().flipX = true;

            }
            else
            {
                GetComponent<Animator>().SetBool("WalkRight", false);
                swipeLeft = false;

            }

        }

        if (!isPressed)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            if (!swipeLeft && !swipeRight)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        // Está precionado?
        if (Input.GetMouseButtonDown(0))
        {

            isPressed = true;

        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;

        }

        if (swipeUp)
        {
            if (IsGrounded())
            {
                GetComponent<Animator>().SetBool("Jump", true);
                StartCoroutine(JumpOffDelay());
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isJumping = true;
            }
           

        }



        // Swipe by:  thestrandedmoose 


        if (!IsGrounded())
        {
            GetComponent<Animator>().SetBool("InTheAir", true);
        }
        else
        {


            GetComponent<Animator>().SetBool("InTheAir", false);
        }


        #region Mobile Inputs
        if (Input.touchCount > 0)
        {

            if (Input.touches[0].phase == TouchPhase.Began)
            {

                isDragging = true;
                if (Input.touchCount == 1)
                {

                    if (Input.touches[0].position.x < (Screen.width / 2))
                    {
                        startTouch = Input.touches[0].position;
                    }
                    else if (Input.touches[0].position.x > (Screen.width / 2))
                    {
                        startTouch2 = Input.touches[0].position;
                    }
                }


            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {

                isDragging = false;
                Reset1();
            }


            if (Input.touchCount > 1)
            {
                if (Input.touches[1].phase == TouchPhase.Began)
                {
                    if (Input.touchCount == 2)
                    {

                        if (Input.touches[1].position.x < Screen.width / 2)
                        {
                            startTouch = Input.touches[1].position;
                        }
                        if (Input.touches[1].position.x > Screen.width / 2)
                        {
                            startTouch2 = Input.touches[1].position;
                        }
                    }
                }
                else if (Input.touches[1].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Canceled)
                {


                    Reset2();
                }
            }
        }
        #endregion


        //Calculate the distance

        if (isDragging)
        {
            if (Input.touchCount > 0)
            {
                if (startTouch != Vector2.zero)
                {
                    if (Input.touchCount == 1)
                    {
                        swipeDelta = Input.touches[0].position - startTouch;
                    }
                    if (Input.touchCount == 2)
                    {
                        if (swipeDelta == Vector2.zero)
                        {
                            swipeDelta = Input.touches[1].position - startTouch;
                        }
                    }
                }
                if (startTouch2 != Vector2.zero)
                {
                    if (Input.touchCount == 1)
                    {
                        swipeDelta2 = Input.touches[0].position - startTouch2;
                    }
                    if (Input.touchCount == 2)
                    {
                        swipeDelta2 = Input.touches[1].position - startTouch2;
                    }
                }
            }

        }



        // Lado esquerdo

        if (swipeDelta.magnitude > 10)
        {

            //Which direction are we swiping?
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {

                //Left or right?
                if (x > 0)
                {
                    swipeRight = true;
                    swipeLeft = false;
                }
                else
                {
                    swipeLeft = true;
                    swipeRight = false;
                }

            }

            if (!isPressed)
            {
                Reset1();
            }
        }




        // Lado direito

        if (swipeDelta2.magnitude > 50)
        {

            //Which direction are we swiping?
            float x = swipeDelta2.x;
            float y = swipeDelta2.y;

            if (Mathf.Abs(x) < Mathf.Abs(y))
            {

                //Up or down?
                if (y > 0)
                {
                    swipeUp = true;

                }
                else { swipeDown = true; }

            }

            Reset2();

        }





    } // End Update




    private void Reset1()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDragging = false;
    }

    private void Reset2()
    {
        startTouch2 = swipeDelta2 = Vector2.zero;

    }




    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Dead");
        }
    }



    private IEnumerator JumpOffDelay()
    {



        yield return new WaitForSeconds(0.5f);
        isJumping = false;
        GetComponent<Animator>().SetBool("Jump", false);
        swipeUp = false;
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
