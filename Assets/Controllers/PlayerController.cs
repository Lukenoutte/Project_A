using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject lux1, lux2, buttonLeft, buttonRight, buttonLux1, buttonLux2;

    private GameObject LuxMain;

    public static PlayerController instance { set; get; }
    private Rigidbody2D rb;
    public ParticleSystem dust;
    [SerializeField]
    private float speed, jumpForce;
    private Vector3 directionGround = Vector3.zero;
    public GameObject setaOld;
    private int countCollision = 0;
    public bool firstJump, doubleJump, isDragging1Click, tapRequested1Click, isDragging2Click, isDragging1BeforeLux,
        tapRequested2Click, tap1, tap2, rightSideScreen, leftSideScreen;
    private bool swipeLeft, swipeRight, swipeUp, swipeDown, isPressed, isPressedKeys = false;

    public Vector2 startTouchLeft, startTouchRight, touchPositionLux = Vector2.zero;

    public bool fakeWalk, walkingRight, walkingLeft, isGroundedMain, rightFirst, leftFrist;
    public LayerMask groundLayers;
    public float  groundCheckDistance2, valueOfIncreace, fRemenberJumpTime;
    private bool jumpTap, blockLoop, upKey, rightKey, leftKey, wasLuxMode, wasGoingToLeft, wasGoingToRight, confirmGrounded = false;
    private float oldPosition, directionYValue, setaPosition, oldVelocityX, oldVelocityY, fRemenberJump,
        increaceSpeedLeft, increaceSpeedRight;


    private Animator setaOldAnimator, playerAnimator, buttonWalkLeftAnimator, buttonWalkRightAnimator;
    private SpriteRenderer playerSpriteRender;
    private Transform playerTransform, luxTransform;

    // Start is called before the first frame update
    void Start()
    {

        LuxMain = lux1;
        buttonWalkLeftAnimator = buttonLeft.GetComponent<Animator>();
        buttonWalkRightAnimator = buttonRight.GetComponent<Animator>();

        playerTransform = GetComponent<Transform>();
        playerSpriteRender = GetComponent<SpriteRenderer>();

        playerAnimator = GetComponent<Animator>();
        setaPosition = 104;
        directionYValue = 0.54f;
        instance = this;

        if (setaOld != null)
        {
            setaOldAnimator = setaOld.GetComponent<Animator>();
        }

        isPressedKeys = false;
        rb = GetComponent<Rigidbody2D>();


        swipeLeft = swipeRight = swipeUp = swipeDown = false;

    }


    void Update()
    {
        SomeAnimations();

        if (UIController.instance != null)
        {
            if (UIController.instance.luxButton1)
            {
                LuxMain = lux1;
                UIController.instance.luxButton1 = false;

            }

            if (UIController.instance.luxButton2)
            {
                LuxMain = lux2;
                UIController.instance.luxButton2 = false;
            }
        }


        // Death
        if (playerTransform.position.y < -2f)
        {
            SceneManager.LoadScene(0);
        }

        ComfirmIfIsGrounded();


        if (walkingLeft | walkingRight)
        {
            StartCoroutine(OldPositionDelay());
            if (playerTransform.position.x == oldPosition && countCollision > 1)
            {
                fakeWalk = true;

            }
            else
            {
                if (playerTransform.position.x == oldPosition && !isGroundedMain)
                {
                    fakeWalk = true;
                }
                else
                {
                    fakeWalk = false;
                    oldPosition = 0;
                }
            }


        }



        if (UIController.instance != null)
        {
            if (!UIController.instance.luxMode)
            {
                // Movimento usando teclas (PC)
                #region PC Moviments
                PressKeysPc();



                if (rightKey)
                {
                    WalkRight();
                }
                else
                {

                    if (!leftKey)
                        playerAnimator.SetBool("WalkRight", false);
                    if (!isDragging1Click)
                        walkingRight = false;

                }


                if (leftKey)
                {

                    WalkLeft();
                }
                else
                {

                    if (!rightKey)
                        playerAnimator.SetBool("WalkRight", false);

                    if (!isDragging1Click)
                        walkingLeft = false;

                }
                #endregion

                #region Mobile Moviments
                if (startTouchLeft.x > setaPosition && leftSideScreen)
                {
                    if (isDragging1Click | isDragging2Click)
                    {
                        WalkRight();

                    }
                    else
                    {
                        playerAnimator.SetBool("WalkRight", false);

                    }

                }



                if (startTouchLeft.x < setaPosition && leftSideScreen)
                {
                    if (isDragging1Click | isDragging2Click)
                    {
                        WalkLeft();
                    }
                    else
                    {

                        playerAnimator.SetBool("WalkRight", false);

                    }
                }
                #endregion


                // Mobile and PC Jump

                if (fRemenberJump >= 0)
                {
                    fRemenberJump -= Time.deltaTime;
                }

                if (upKey | jumpTap)
                {
                    fRemenberJump = fRemenberJumpTime;

                }

                // Pulo
                if (fRemenberJump > 0 && isGroundedMain)
                {
                    Jump();
                    fRemenberJump = 0;
                }
                else if (fRemenberJump > 0 && !isGroundedMain)
                {
                    Jump();
                }

                if (wasLuxMode)
                {
                    rb.velocity = new Vector2(oldVelocityX, oldVelocityY);

                    wasLuxMode = false;
                    fakeWalk = false;
                    oldVelocityX = oldVelocityY = 0;
                    dust.playbackSpeed = 1;
                    playerAnimator.SetBool("IsLuxMode", false);
                    buttonLux1.SetActive(false);
                    buttonLux2.SetActive(false);

                }

                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                playerAnimator.SetFloat("Velocity", 1f);
            }
            else
            {
                buttonLux1.SetActive(true);
                buttonLux2.SetActive(true);
                playerAnimator.SetBool("IsLuxMode", true);
                dust.playbackSpeed = 0;
                fakeWalk = true;
                if (oldVelocityX == 0 && oldVelocityY == 0)
                {
                    oldVelocityX = rb.velocity.x;
                    oldVelocityY = rb.velocity.y;

                }

                wasLuxMode = true;
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                playerAnimator.SetFloat("Velocity", 0f);
            }
        }
        // Evitar que o personagem deslize
        if (!isPressed && !isPressedKeys)
        {
            if (isGroundedMain)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                increaceSpeedLeft = increaceSpeedRight = 0;
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
            isDragging2Click = tapRequested2Click = false;
            startTouchRight = Vector2.zero;

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
                if (!UIController.instance.uIClick)
                    jumpTap = true;

                rightSideScreen = false;
            }

            tap1 = false;
            tap2 = false;

            rightSideScreen = false;
        }

        // Criar Lux se estiver em lux mode
        CreateLux();



        #region Mobile Inputs
        if (Input.touchCount > 0)
        {
            

            if (Input.touches[0].phase == TouchPhase.Began)
            {
                // Lux
                if (UIController.instance.luxMode && !UIController.instance.uIClick && Input.touchCount == 1)
                {


                    touchPositionLux = Input.touches[0].position;


                }

                if (!UIController.instance.luxMode)
                {
                    isDragging1BeforeLux = true;
                }
                // End Lux

                isDragging1Click = true;
                tapRequested1Click = true;
                if (Input.touchCount == 1)
                {

                    if (Input.touches[0].position.x < (Screen.width / 2))
                    {
                        startTouchLeft = Input.touches[0].position;
                        leftSideScreen = true;
                        leftFrist = true;

                    }
                    else if (Input.touches[0].position.x > (Screen.width / 2))
                    {
                        startTouchRight = Input.touches[0].position;
                        rightSideScreen = true;
                        rightFirst = true;


                    }

                }
                
                
                if(rightFirst && isDragging2Click && Input.touches[0].position.x > (Screen.width / 2))
                {
                    startTouchRight = Input.touches[0].position;
                    rightSideScreen = true;
                }


            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                if (tapRequested1Click)
                {
                    tap1 = true;


                    tapRequested1Click = false;

                }
                isDragging1Click = false;
                isDragging1BeforeLux = false;
                Reset1();
            }


            if (Input.touchCount > 1)
            {




                if (Input.touches[1].phase == TouchPhase.Began)
                {
                    if (UIController.instance.luxMode && !UIController.instance.uIClick)
                    {

                        touchPositionLux = Input.touches[1].position;

                    }

                    isDragging2Click = true;
                    tapRequested2Click = true;


                    if (Input.touchCount == 2)
                    {

                        if (Input.touches[1].position.x > Screen.width / 2)
                        {
                            startTouchRight = Input.touches[1].position;
                            rightSideScreen = true;

                        }
                    }

                    if (Input.touchCount == 2 && rightFirst)
                    {
                        
                        if (Input.touches[0].position.x > (Screen.width / 2) && Input.touches[1].position.x < (Screen.width / 2))
                        {
                            
                            startTouchLeft = Input.touches[1].position;

                            leftSideScreen = true;
                        }
                    }
                }
                else if (Input.touches[1].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Canceled)
                {
                    if (tapRequested2Click)
                    {
                        tap2 = true;
                        tapRequested2Click = false;
                        isDragging2Click = false;


                    }

                    Reset2();
                }
            }




        }// end if Touch > 0
        #endregion


        if (isDragging1Click | isDragging2Click)
        {
            if (Input.touchCount > 0)
            {
                if (UIController.instance.luxMode && !UIController.instance.uIClick)
                {
                    if (Input.touchCount == 1 && !isDragging1BeforeLux)
                    {

                        touchPositionLux = Input.touches[0].position;
                    }
                    if (Input.touchCount == 2 && isDragging1BeforeLux)
                        touchPositionLux = Input.touches[1].position;
                }

                if (startTouchLeft != Vector2.zero && leftSideScreen && leftFrist)
                {
                    startTouchLeft = Input.touches[0].position;

                }
                else if (startTouchLeft != Vector2.zero && leftSideScreen && rightFirst)
                {
                    if (Input.touchCount > 1)
                    {
                        startTouchLeft = Input.touches[1].position;
                    }
                    else
                    {
                        startTouchLeft = Input.touches[0].position;
                    }
                }
            }
        }



    } // End Update




    private void Reset1()
    {

        if (!isDragging2Click)
            leftSideScreen = rightFirst = leftFrist = false;

    }

    private void Reset2()
    {

        startTouchRight = Vector2.zero;

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



        yield return new WaitForSeconds(0.1f);

        oldPosition = playerTransform.position.x;

    }

    private IEnumerator ShowLuxDelay()
    {



        yield return new WaitForSeconds(0.1f);

        LuxMain.SetActive(true);

    }




    private void OnCollisionEnter2D(Collision2D collision)
    {
        countCollision++;
        if (collision.gameObject.tag == "Ground")
        {

            directionGround = transform.position - collision.gameObject.transform.position;


            if (directionGround.y >= directionYValue)
            {

                dust.Play();

                isGroundedMain = true;

            }



        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Lux1"))
        {
            if (!UIController.instance.luxMode)
            {
                rb.velocity = new Vector2(rb.velocity.x, 2.5f);
            }

            collision.gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Lux2"))
        {
            if (!UIController.instance.luxMode)
            {
                rb.velocity = new Vector2(2.5f, rb.velocity.y);
            }

            collision.gameObject.SetActive(false);
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        directionGround = transform.position - collision.gameObject.transform.position;
        countCollision--;

        if (collision.gameObject.tag == "Ground" && countCollision == 0)
        {

            isGroundedMain = false;


        }
    }


    private void ComfirmIfIsGrounded()
    {

        Ray2D ray = new Ray2D(transform.position, Vector2.down);

        RaycastHit2D hit2 = Physics2D.Raycast(ray.origin, ray.direction, groundCheckDistance2, groundLayers);



        if (hit2)
        {
            confirmGrounded = true;
            if (!isGroundedMain)
            {
                firstJump = doubleJump = false;
                isGroundedMain = true;
            }

        }
        else
        {
            confirmGrounded = false;
        }

    }


    private void Jump()
    {
        if (!firstJump | !doubleJump)
        {
            blockLoop = true;
            if (!firstJump)
            {

                if (isGroundedMain)
                {


                    dust.Play();

                }

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
    private void WalkRight()
    {
        if (isGroundedMain)
        {
            increaceSpeedRight = 0;
        }


        if (increaceSpeedRight == 0 && rb.velocity.x != 0)
        {
            increaceSpeedRight = rb.velocity.x;

        }
        else if (rb.velocity.x == 0)
        {
            increaceSpeedRight = rb.velocity.x;
        }

        if (increaceSpeedRight < speed)
        {
            increaceSpeedRight += Time.deltaTime * valueOfIncreace;
            rb.velocity = new Vector2(increaceSpeedRight, rb.velocity.y);
        }

        playerAnimator.SetBool("WalkRight", true);
        playerSpriteRender.flipX = true;
        walkingRight = true;
        walkingLeft = false;
        increaceSpeedLeft = 0;
    }

    private void WalkLeft()
    {
        if (isGroundedMain)
        {

            increaceSpeedLeft = 0;
        }



        if (increaceSpeedLeft == 0 && rb.velocity.x != 0)
        {
            increaceSpeedLeft = rb.velocity.x;

        }
        else if (rb.velocity.x == 0)
        {
            increaceSpeedLeft = rb.velocity.x;
        }

        if (increaceSpeedLeft > -speed)
        {
            increaceSpeedLeft += Time.deltaTime * -valueOfIncreace;
            rb.velocity = new Vector2(increaceSpeedLeft, rb.velocity.y);
        }


        playerAnimator.SetBool("WalkRight", true);
        playerSpriteRender.flipX = false;
        walkingLeft = true;
        walkingRight = false;
        increaceSpeedRight = 0;
    }

    public void PressKeysPc()
    {
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
    }


    private void CreateLux()
    {
        if (touchPositionLux != Vector2.zero)
        {

            Vector3 auxLux = Camera.main.ScreenToWorldPoint(touchPositionLux);
            Debug.DrawRay(touchPositionLux, Camera.main.transform.forward, Color.green);
            if (!UIController.instance.uIClick && UIController.instance.luxMode)
            {
                luxTransform = LuxMain.GetComponent<Transform>();
                luxTransform.position = new Vector3(auxLux.x, auxLux.y, luxTransform.position.z);
                touchPositionLux = Vector2.zero;
                StartCoroutine(ShowLuxDelay());
            }

        }

        // Create Lux if mouse
        if (UIController.instance.luxMode && !UIController.instance.uIClick)
        {
            if (Input.touchCount == 0 && Input.GetMouseButtonDown(0))
            {

                touchPositionLux = Input.mousePosition;
            }
        }
    }


    private void SomeAnimations()
    {
        if (setaOld != null)
        {
            if (walkingRight)
            {
                setaOldAnimator.SetBool("WalkingRight", true);
            }
            else
            {
                setaOldAnimator.SetBool("WalkingRight", false);
            }

            if (walkingLeft)
            {
                setaOldAnimator.SetBool("WalkingLeft", true);
            }
            else
            {
                setaOldAnimator.SetBool("WalkingLeft", false);
            }
        }


        if (buttonLeft != null && buttonRight != null)
        {
            if (walkingRight)
            {
                buttonWalkRightAnimator.SetBool("Press", true);
            }
            else
            {
                buttonWalkRightAnimator.SetBool("Press", false);
            }

            if (walkingLeft)
            {
                buttonWalkLeftAnimator.SetBool("Press", true);
            }
            else
            {
                buttonWalkLeftAnimator.SetBool("Press", false);
            }

        }

        // Está andando e esbarrando em algo
        if (fakeWalk)
        {
            playerAnimator.SetBool("FakeWalk", true);
        }
        else
        {
            playerAnimator.SetBool("FakeWalk", false);
        }

    }
}