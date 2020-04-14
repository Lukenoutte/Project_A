using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour
{

    private Rigidbody2D rb;

    private float speed;
    private float jumpForce;

    public bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private bool tapRequested;
    private bool isDragging;
    private Vector2 startTouch, swipeDelta;
    private float sideRotation;
    private float rotationSmooth;
    private int speedR;
    private bool isPressed;

    // isGRounded
    public LayerMask groundLayers;
    private float groundCheckDistance;

    //Angle
    private float angle;
    private RaycastHit2D[] hits;
    private int h;




    // Start is called before the first frame update
    void Start()
    {
        groundCheckDistance = 2f;

        isDragging = false;
        hits = new RaycastHit2D[2];
        speedR = 80;

        rb = GetComponent<Rigidbody2D>();
        speed = 4;
        jumpForce = 7;

        tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

    }


    void Update()
    {



        h = Physics2D.RaycastNonAlloc(transform.position, -Vector2.up, hits); //cast downwards
        if (h > 1)
        { //if we hit something do stuff

            sideRotation = hits[1].normal.x;

            if (IsGrounded())
            {
                angle = Mathf.Abs(Mathf.Atan2(hits[1].normal.x, hits[1].normal.y) * Mathf.Rad2Deg); //get angle
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



        #region Touch
        if (tap)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);


            tap = false;
        }


        // Andar para direita
        if (swipeRight)
        {

            if (isPressed)
            {

                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
            else
            {
                swipeRight = false;
            }

        }
        else
        if (swipeLeft)
        {

            if (isPressed)
            {

                rb.velocity = new Vector2(-speed, rb.velocity.y);
            }
            else
            {

                swipeLeft = false;
            }

        }

        if (!isPressed)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            if(!swipeLeft && !swipeRight)
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
                rb.velocity = new Vector2(rb.velocity.y, jumpForce);
            }
            swipeUp = false;
        }
        #endregion
        // Swipe by:  thestrandedmoose 

        #region Standalone Inputs
        if (Input.GetMouseButtonDown(0))
        {
            tapRequested = true;
            isDragging = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (tapRequested) { tap = true; }
            isDragging = false;
            Reset();
        }
        #endregion



        #region Mobile Inputs
        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tapRequested = true;
                isDragging = true;
                startTouch = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                if (tapRequested) { tap = true; }
                isDragging = false;
                Reset();
            }
        }
        #endregion



        //Calculate the distance
        swipeDelta = Vector2.zero;
        if (isDragging)
        {
            if (Input.touchCount > 0) { swipeDelta = Input.touches[0].position - startTouch; }
            else if (Input.GetMouseButton(0)) { swipeDelta = (Vector2)Input.mousePosition - startTouch; }
        }


        // Lado esquerdo
        if (startTouch.x < Screen.width / 2)
        {
            if (swipeDelta.magnitude > 20)
            {
                tapRequested = false;
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
                    Reset();
                }
            }
        }



        // Lado direito
        if (startTouch.x > Screen.width / 2)
        {
            if (swipeDelta.magnitude > 50)
            {
                tapRequested = false;
                //Which direction are we swiping?
                float x = swipeDelta.x;
                float y = swipeDelta.y;

                if (Mathf.Abs(x) < Mathf.Abs(y))
                {

                    //Up or down?
                    if (y > 0)
                    {
                        swipeUp = true;

                    }
                    else { swipeDown = true; }

                }

                Reset();

            }
        }




    } // End Update




    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDragging = false;
    }





    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Dead");
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
