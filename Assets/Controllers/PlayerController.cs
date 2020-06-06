using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { set; get; }
    private Rigidbody2D rb;

    [SerializeField]
    private float speed, jumpForce;

    public GameObject seta;
    private float directionYValue;
    private bool swipeLeft, swipeRight, swipeUp, swipeDown;
    public bool walkingRight, walkingLeft = false;
    private bool isDragging;
    private Vector2 startTouch, startTouch2, swipeDelta, swipeDelta2 = Vector2.zero;
    private float sideRotation;
    private float rotationSmooth;
    private int speedR;
    private bool isPressed;
    private bool isPressedKeys;
    private Transform setaTrans;
    private bool firstJump = false;
    private bool doubleJump = false;
    private bool tapRequested, tap;
    private bool jumpTap = false;
    private bool rightSideScreen = false;
    private bool blockLoop = false;

    private float oldPosition = 0;
    public bool fakeWalk = false;
    //private Vector3 lastTouch0;
    //private Vector3 lastTouch1;

    private bool isGroundedMain;

    //Angle
    private float angle;
    private RaycastHit2D[] hits;
    private int h;

    private RaycastHit2D hitsMain;

    private bool upKey = false;
    private bool rightKey = false;
    private bool leftKey = false;


    // Start is called before the first frame update
    void Start()
    {

        directionYValue = 0.55f;
        instance = this;

        swipeDelta = Vector2.zero;
        swipeDelta2 = Vector2.zero;
        isDragging = false;
        hits = new RaycastHit2D[2];
        speedR = 80;
        isPressedKeys = false;
        rb = GetComponent<Rigidbody2D>();


        swipeLeft = swipeRight = swipeUp = swipeDown = false;

    }


    void Update()
    {

        if (walkingRight)
        {
            seta.GetComponent<Animator>().SetBool("WalkingRight", true);
        }
        else
        {
            seta.GetComponent<Animator>().SetBool("WalkingRight", false);
        }

        if (walkingLeft)
        {
            seta.GetComponent<Animator>().SetBool("WalkingLeft", true);
        }
        else
        {
            seta.GetComponent<Animator>().SetBool("WalkingLeft", false);
        }

        if (walkingLeft | walkingRight)
        {
            StartCoroutine(OldPositionDelay());
            if (GetComponent<Transform>().position.x == oldPosition)
            {
                fakeWalk = true;
            }
            else
            {
                fakeWalk = false;
            }

            seta.SetActive(true);
            if (startTouch != Vector2.zero)
            {

                setaTrans = seta.GetComponent<Transform>();
                setaTrans.position = new Vector3(startTouch.x, startTouch.y, setaTrans.position.z);

            }
        }
        else
        {
            seta.SetActive(false);
        }


        if (fakeWalk)
        {
            GetComponent<Animator>().SetBool("FakeWalk", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("FakeWalk", false);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            leftKey = true;
            isPressedKeys = true;
        }
        else
        {
            leftKey = false;

        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            rightKey = true;
            isPressedKeys = true;
        }
        else
        {
            rightKey = false;

        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            upKey = true;
            isPressedKeys = true;
        }
        else
        {
            upKey = false;

        }

        if (!leftKey && !rightKey && !upKey)
        {
            isPressedKeys = false;
        }


        if (rightKey)
        {

            rb.velocity = new Vector2(speed, rb.velocity.y);
            GetComponent<Animator>().SetBool("WalkRight", true);
            GetComponent<SpriteRenderer>().flipX = true;
            walkingRight = true;
        }
        else
        {
            if (!isPressed)
            {
                if (!leftKey)
                    GetComponent<Animator>().SetBool("WalkRight", false);
                walkingRight = false;
            }
        }


        if (leftKey)
        {
            GetComponent<Animator>().SetBool("WalkRight", true);
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            GetComponent<SpriteRenderer>().flipX = false;
            walkingLeft = true;
        }
        else
        {
            if (!isPressed)
            {
                if (!rightKey)
                    GetComponent<Animator>().SetBool("WalkRight", false);
                walkingLeft = false;


            }

        }


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

                if (isGroundedMain)
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
                GetComponent<SpriteRenderer>().flipX = true;
                walkingRight = true;
            }
            else
            {
                swipeRight = false;
                GetComponent<Animator>().SetBool("WalkRight", false);
             
            }

        }
        else
        {
            walkingRight = false;
        }
        


        if (swipeLeft)
        {

            if (isPressed)
            {
                walkingLeft = true;
                GetComponent<Animator>().SetBool("WalkRight", true);
                rb.velocity = new Vector2(-speed, rb.velocity.y);
                GetComponent<SpriteRenderer>().flipX = false;

            }
            else
            {
                
                GetComponent<Animator>().SetBool("WalkRight", false);
                swipeLeft = false;

            }

        }
        else
        {
            walkingLeft = false;
        }

        if (!isPressed && !isPressedKeys)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            if (!isPressedKeys)
            {
                if (!swipeLeft && !swipeRight)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
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

        if (upKey | jumpTap)
        {


            if (!firstJump | !doubleJump)
            {
                blockLoop = true;
                if (!firstJump)
                {
                    firstJump = true;

                }
                else if (!doubleJump && firstJump)
                {
                    doubleJump = true;
                }


                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                GetComponent<Animator>().SetBool("Jump", true);
                StartCoroutine(JumpOffDelay());


            }



            jumpTap = false;
            upKey = false;

        }

        if (!isGroundedMain)
        {
            GetComponent<Animator>().SetBool("InTheAir", true);
        }
        else
        {

            GetComponent<Animator>().SetBool("InTheAir", false);

            if (!blockLoop)
            {

                firstJump = false;
                doubleJump = false;
            }


        }

        if (tap && rightSideScreen)
        {
            jumpTap = true;
            tap = false;
            rightSideScreen = false;
        }
        else if (tap && !rightSideScreen)
        {
            tap = false;
        }


        //if (tap)
        //{
        //    RaycastHit2D hit;
        //    if (!isDragging)
        //    {
        //        hit = Physics2D.Raycast(lastTouch0, Vector2.zero);
        //    }
        //    else
        //    {
        //        hit = Physics2D.Raycast(lastTouch1, Vector2.zero);

        //    }

        //    if (hit.collider != null)
        //    {

        //        if (hit.collider.tag == "Jump")
        //        {
        //            jumpTap = true;
        //        }
        //    }
        //    tap = false;
        //    lastTouch0 = Vector3.zero;
        //    lastTouch1 = Vector3.zero;
        //}

        // Swipe by:  thestrandedmoose 





        #region Mobile Inputs
        if (Input.touchCount > 0)
        {


            if (Input.touches[0].phase == TouchPhase.Began)
            {

                isDragging = true;
                tapRequested = true;
                if (Input.touchCount == 1)
                {

                    if (Input.touches[0].position.x < (Screen.width / 2))
                    {
                        startTouch = Input.touches[0].position;

                    }
                    else if (Input.touches[0].position.x > (Screen.width / 2))
                    {
                        startTouch2 = Input.touches[0].position;
                        rightSideScreen = true;

                    }
                }


            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                if (tapRequested)
                {
                    tap = true;
                    // lastTouch0 = Camera.main.ScreenToWorldPoint(Input.touches[0].position);

                }
                isDragging = false;
                Reset1();
            }


            if (Input.touchCount > 1)
            {
                if (Input.touches[1].phase == TouchPhase.Began)
                {
                    isDragging = true;
                    tapRequested = true;


                    if (Input.touchCount == 2)
                    {

                        if (Input.touches[1].position.x < Screen.width / 2)
                        {
                            startTouch = Input.touches[1].position;
                        }
                        if (Input.touches[1].position.x > Screen.width / 2)
                        {
                            startTouch2 = Input.touches[1].position;
                            rightSideScreen = true;
                        }
                    }
                }
                else if (Input.touches[1].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Canceled)
                {
                    if (tapRequested)
                    {
                        tap = true;
                        // lastTouch1 = Camera.main.ScreenToWorldPoint(Input.touches[1].position);
                    }

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
            if (Input.touchCount <= 1)
            {
                tapRequested = false;
            }
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
            tapRequested = false;
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
        tapRequested = false;
    }

    private void Reset2()
    {

        startTouch2 = swipeDelta2 = Vector2.zero;

        tapRequested = false;
    }



    private IEnumerator JumpOffDelay()
    {



        yield return new WaitForSeconds(0.2f);

        GetComponent<Animator>().SetBool("Jump", false);
        swipeUp = false;
        blockLoop = false;

    }


    private IEnumerator OldPositionDelay()
    {



        yield return new WaitForSeconds(0.3f);

        oldPosition = GetComponent<Transform>().position.x;

    }





    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Ground")
        {
            Vector3 direction = transform.position - collision.gameObject.transform.position;


            if (direction.y >= directionYValue)
            {

                isGroundedMain = true;
            }



        }


        if (collision.gameObject.tag == "Reset")
        {
            SceneManager.LoadScene(0);
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        Vector3 direction = transform.position - collision.gameObject.transform.position;

        if (collision.gameObject.tag == "Ground" && direction.y >= directionYValue)
        {

            isGroundedMain = false;


        }
    }



}
