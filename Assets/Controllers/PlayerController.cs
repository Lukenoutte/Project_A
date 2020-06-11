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
    private Vector3 directionGround = Vector3.zero;
    public GameObject seta;
    private int countCollision = 0;
    public bool isGroundedMain, firstJump, doubleJump, isDragging1, tapRequested1, isDragging2,
        tapRequested2, tap1, tap2, rightSideScreen, leftSideScreen, leftFrist, rightFirst;
    private bool swipeLeft, swipeRight, swipeUp, swipeDown,
        isPressed, isPressedKeys = false;

    private Vector2 startTouch, startTouch2 = Vector2.zero;

    public bool confirmGrounded;
    public LayerMask groundLayers;
    public float groundCheckDistance;

    public bool fakeWalk, walkingRight, walkingLeft;

    private bool jumpTap, blockLoop,
        upKey, rightKey, leftKey = false;

    private float oldPosition, directionYValue, setaPosition;

    //private Vector3 lastTouch0;
    //private Vector3 lastTouch1;



    private Animator setaAnimator, playerAnimator;
    private SpriteRenderer playerSpriteRender;
    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {

        playerTransform = GetComponent<Transform>();
        playerSpriteRender = GetComponent<SpriteRenderer>();
        setaAnimator = seta.GetComponent<Animator>();
        playerAnimator = GetComponent<Animator>();
        setaPosition = 134;
        directionYValue = 0.54f;
        instance = this;




        isPressedKeys = false;
        rb = GetComponent<Rigidbody2D>();


        swipeLeft = swipeRight = swipeUp = swipeDown = false;

    }


    void Update()
    {

        ComfirmIfIsGrounded();
        if (walkingRight)
        {
            setaAnimator.SetBool("WalkingRight", true);
        }
        else
        {
            setaAnimator.SetBool("WalkingRight", false);
        }

        if (walkingLeft)
        {
            setaAnimator.SetBool("WalkingLeft", true);
        }
        else
        {
            setaAnimator.SetBool("WalkingLeft", false);
        }

        if (walkingLeft | walkingRight)
        {
            StartCoroutine(OldPositionDelay());
            if (playerTransform.position.x == oldPosition && countCollision > 1)
            {
                fakeWalk = true;

            }
            else
            {
                fakeWalk = false;
                oldPosition = 0;
            }


        }



        if (fakeWalk)
        {
            playerAnimator.SetBool("FakeWalk", true);
        }
        else
        {
            playerAnimator.SetBool("FakeWalk", false);
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
            playerAnimator.SetBool("WalkRight", true);
            playerSpriteRender.flipX = true;
            walkingRight = true;
        }
        else
        {

            if (!leftKey)
                playerAnimator.SetBool("WalkRight", false);
            if (!isDragging1)
                walkingRight = false;

        }


        if (leftKey)
        {
            playerAnimator.SetBool("WalkRight", true);
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            playerSpriteRender.flipX = false;
            walkingLeft = true;
        }
        else
        {

            if (!rightKey)
                playerAnimator.SetBool("WalkRight", false);

            if (!isDragging1)
                walkingLeft = false;

        }






        // Andar para direita




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
            walkingLeft = false;
            walkingRight = false;
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
                playerAnimator.SetBool("Jump", true);
                StartCoroutine(JumpOffDelay());


            }



            jumpTap = false;
            upKey = false;

        }

        if (!isGroundedMain)
        {
            playerAnimator.SetBool("InTheAir", true);
        }
        else
        {

            playerAnimator.SetBool("InTheAir", false);

            if (!blockLoop)
            {

                firstJump = false;
                doubleJump = false;
            }


        }

        if (tap2 | tap1)
        {
            if (rightSideScreen)
            {
                jumpTap = true;

                rightSideScreen = false;
            }
            tap1 = false;
            tap2 = false;
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


        if (startTouch.x > setaPosition && leftSideScreen)
        {
            if (isDragging1)
            {
                walkingLeft = false;
                walkingRight = true;
                rb.velocity = new Vector2(speed, rb.velocity.y);
                playerAnimator.SetBool("WalkRight", true);
                playerSpriteRender.flipX = true;
            }
            else
            {
                playerAnimator.SetBool("WalkRight", false);

            }

        }



        if (startTouch.x < setaPosition && leftSideScreen)
        {
            if (isDragging1)
            {
                walkingRight = false;
                walkingLeft = true;
                playerAnimator.SetBool("WalkRight", true);
                rb.velocity = new Vector2(-speed, rb.velocity.y);
                playerSpriteRender.flipX = false;
            }
            else
            {

                playerAnimator.SetBool("WalkRight", false);

            }
        }


        #region Mobile Inputs
        if (Input.touchCount > 0)
        {


            if (Input.touches[0].phase == TouchPhase.Began)
            {

                isDragging1 = true;
                tapRequested1 = true;
                if (Input.touchCount == 1)
                {

                    if (Input.touches[0].position.x < (Screen.width / 2))
                    {
                        startTouch = Input.touches[0].position;
                        leftSideScreen = true;
                        leftFrist = true;

                    }
                    else if (Input.touches[0].position.x > (Screen.width / 2))
                    {
                        startTouch2 = Input.touches[0].position;
                        rightSideScreen = true;
                        rightFirst = true;

                    }
                }


            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                if (tapRequested1)
                {
                    tap1 = true;

                    tapRequested1 = false;
                }
                isDragging1 = false;
                Reset1();
            }


            if (Input.touchCount > 1)
            {
                if (Input.touches[1].phase == TouchPhase.Began)
                {
                    isDragging2 = true;
                    tapRequested2 = true;


                    if (Input.touchCount == 2)
                    {

                        if (Input.touches[1].position.x < Screen.width / 2)
                        {
                            leftSideScreen = true;
                            if (leftSideScreen && rightSideScreen)
                            {
                                startTouch = Input.touches[1].position;
                                
                            }
                            
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
                    if (tapRequested2)
                    {
                        tap2 = true;
                        tapRequested2 = false;
                        isDragging2 = false;
                    }

                    Reset2();
                }
            }
        }
        #endregion


        if (isDragging1)
        {
            if (Input.touchCount > 0)
            {
                if (startTouch != Vector2.zero && leftSideScreen && leftFrist)
                {
                    startTouch = Input.touches[0].position;

                }
                else if (startTouch != Vector2.zero && leftSideScreen && rightFirst)
                {
                    if(Input.touchCount > 1) 
                    startTouch = Input.touches[1].position;
                }
            }
        }



    } // End Update




    private void Reset1()
    {

        startTouch = Vector2.zero;
        isDragging1 = tapRequested1 = leftSideScreen = rightFirst = leftFrist = false;
    }

    private void Reset2()
    {

        startTouch2 = Vector2.zero;
        isDragging2 = tapRequested2 = false;

    }



    private IEnumerator JumpOffDelay()
    {



        yield return new WaitForSeconds(0.2f);

        playerAnimator.SetBool("Jump", false);
        swipeUp = false;
        blockLoop = false;

    }


    private IEnumerator OldPositionDelay()
    {



        yield return new WaitForSeconds(1f);

        oldPosition = playerTransform.position.x;

    }





    private void OnCollisionEnter2D(Collision2D collision)
    {
        countCollision++;
        if (collision.gameObject.tag == "Ground")
        {

            directionGround = transform.position - collision.gameObject.transform.position;


            if (directionGround.y >= directionYValue)
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
        directionGround = transform.position - collision.gameObject.transform.position;
        countCollision--;

        if (collision.gameObject.tag == "Ground" && directionGround.y >= directionYValue)
        {

            isGroundedMain = false;


        }
    }


    private void ComfirmIfIsGrounded()
    {

        Ray2D ray = new Ray2D(transform.position, Vector2.down);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, groundCheckDistance, groundLayers);


        if (countCollision <= 1)
        {
            if (hit)
            {
                confirmGrounded = true;
                if (!isGroundedMain)
                {
                    isGroundedMain = true;
                }

            }
            else
            {
                confirmGrounded = false;
            }

        }
    }

}
