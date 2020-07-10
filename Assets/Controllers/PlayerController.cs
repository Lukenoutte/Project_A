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
    private GameObject lux1, lux2, buttonLeft, buttonRight, buttonLux1, buttonLux2, playerClone1;

    private GameObject LuxMain;

    public static PlayerController instance { set; get; }
    private Rigidbody2D rb;
    public ParticleSystem dust;
    [SerializeField]
    private float speed, jumpForce;
    private Vector3 directionGround = Vector3.zero;
    public GameObject setaOld;
    private int countCollision = 0;
    public bool firstJump, isDragging1Click, tapRequested1Click, isDragging2Click, isDragging1BeforeLux,
        tapRequested2Click, tap1, tap2;
    private bool swipeLeft, swipeRight, swipeUp, swipeDown, isPressed, isPressedKeys = false;

    public Vector2 startTouchLeft, startTouchRight, touchPositionLux = Vector2.zero;

    public bool fakeWalk, walkingRight, walkingLeft, isGroundedMain, rightFirst, leftFrist, confirmGrounded, jumpTap, rightSideScreen, leftSideScreen;
    public LayerMask groundLayers;
    public float groundCheckDistance2, valueOfIncreace, fRemenberJumpTime, fRemenberJump, oldVelocityX, oldVelocityY;
    private bool blockLoop, upKey, rightKey, leftKey, wasLuxMode, wasGoingToLeft, wasGoingToRight, isJumping = false;
    private float oldPosition, directionYValue, setaPosition,
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

                float newVelocity = rb.velocity.x;
                if (!isGroundedMain)
                { // Reduz a velocidade se não ta clicando pra andar

                    if (!isPressed && !isPressedKeys)
                    {
                        if (newVelocity > 0)
                        {
                            newVelocity -= Time.deltaTime;
                        }
                        else
                        {
                            newVelocity += Time.deltaTime;
                        }

                        rb.velocity = new Vector2(newVelocity, rb.velocity.y);
                    }
                }

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
                if (startTouchLeft.x > setaPosition)// Andar para direita (mobile)
                {

                    if (leftSideScreen)
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
                }



                if (startTouchLeft.x < setaPosition) // Andar para esquerda (mobile)
                {
                    if (leftSideScreen)
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
                }
                #endregion


                // Mobile and PC Jump

                if (fRemenberJump >= 0)
                {
                    fRemenberJump -= Time.deltaTime;
                }

                if (upKey | jumpTap)
                {
                    if (!UIController.instance.luxMode)
                        fRemenberJump = fRemenberJumpTime;
                    jumpTap = false;
                    upKey = false;

                }

                // Pulo
                if (fRemenberJump > 0)
                {
                    Jump();

                    if (!firstJump)
                    {
                        fRemenberJump = 0;
                    }

                }



                if (wasLuxMode)
                { // Reseta configurações depois do lux mode
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
            { // Lux mode ativo
                fRemenberJump = 0;
                jumpTap = false;
                upKey = false;
                StartCoroutine(ShowClone1Delay());
                buttonLux1.SetActive(true);
                buttonLux2.SetActive(true);
                playerAnimator.SetBool("IsLuxMode", true);
                dust.playbackSpeed = 0;
                fakeWalk = true;

                if (oldVelocityX == 0 && oldVelocityY == 0)
                { // Pega velocidade que estava antes do Lux mode
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


        // Side screen
        if (Input.touchCount >= 1)
        {
            if (Input.touchCount == 1)
            {
                if (Input.touches[0].position.x < (Screen.width / 2))
                {
                    leftSideScreen = true;

                    if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                    {
                        leftSideScreen = false;
                    }
                }


                if (Input.touches[0].position.x > (Screen.width / 2))
                {
                    rightSideScreen = true;

                    if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                    {
                        StartCoroutine(RightSideDelay());
                    }
                }

            }
            else if (Input.touchCount == 2)
            {
                if (Input.touches[0].position.x < (Screen.width / 2))
                {
                    leftSideScreen = true;
                    if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                    {
                        leftSideScreen = false;
                    }
                }
                else if(Input.touches[1].position.x < (Screen.width / 2))
                {
                    leftSideScreen = true;
                    if (Input.touches[1].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Canceled)
                    {
                        leftSideScreen = false;
                    }

                }


                if (Input.touches[0].position.x > (Screen.width / 2))
                {
                    rightSideScreen = true;
                    if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                    {
                        StartCoroutine(RightSideDelay());
                    }
                }
                else if (Input.touches[1].position.x > (Screen.width / 2))
                {
                    rightSideScreen = true;
                    if (Input.touches[1].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Canceled)
                    {
                        StartCoroutine(RightSideDelay());
                    }

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
            tapRequested2Click = isDragging2Click = false;

        }



        if (!isGroundedMain)
        {
            playerAnimator.SetBool("InTheAir", true);

            // Controlar velocidade de queda
            if (rb.velocity.y < -2.5)
            {
                rb.velocity = new Vector2(rb.velocity.x, -2.5f);
            }

        }
        else
        {

            playerAnimator.SetBool("InTheAir", false);

            if (!blockLoop) // Proíbe o pulo de resetar no momento do pulo
            {

                firstJump = false;

            }


        }

        if (tap2 | tap1)
        {

            if (rightSideScreen)
            {

                if (!UIController.instance.luxMode)
                {
                    jumpTap = true;

                }

            }
            StartCoroutine(ResetTapDelay());
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

                    if (Input.touches[0].position.x < (Screen.width / 2)) // Cliques a esquerda da tela
                    {
                        startTouchLeft = Input.touches[0].position;

                        rightFirst = false;
                        leftFrist = true;

                    }
                    else if (Input.touches[0].position.x > (Screen.width / 2))// Cliques a direita da tela
                    {
                        startTouchRight = Input.touches[0].position;

                        leftFrist = false;
                        rightFirst = true;


                    }

                }


            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                if (tapRequested1Click)
                {
                    tap1 = true;

                }


                Reset1();
            }


            if (Input.touchCount == 2) // 2 Cliques
            {


                if (Input.touches[1].phase == TouchPhase.Began)
                {
                    // Lux
                    if (UIController.instance.luxMode && !UIController.instance.uIClick)
                    {

                        touchPositionLux = Input.touches[1].position;

                    }
                    // End Lux

                    isDragging2Click = true;
                    tapRequested2Click = true;


                    if (Input.touches[1].position.x > Screen.width / 2) // Clique a direita da tela
                    {
                        startTouchRight = Input.touches[1].position;


                    }


                    if (rightFirst)
                    {

                        if (Input.touches[0].position.x > (Screen.width / 2) && Input.touches[1].position.x < (Screen.width / 2))
                        { // Pega clique da esquerda quando está segurando na direita

                            startTouchLeft = Input.touches[1].position;

                        }

                    }

                }
                else if (Input.touches[1].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Canceled)
                {
                    if (tapRequested2Click)
                    {
                        tap2 = true;

                    }

                    Reset2();
                }
            }




        }// end if Touch > 0
        #endregion


        if (isDragging1Click | isDragging2Click) // Atualiza posição quando o clique é segurado
        {

            // Se está no Lux Mode
            if (UIController.instance.luxMode && !UIController.instance.uIClick)
            {
                if (Input.touchCount == 1 && !isDragging1BeforeLux)
                {

                    touchPositionLux = Input.touches[0].position; // Atualiza posição do lux
                }
                if (Input.touchCount == 2 && isDragging1BeforeLux) // Se tiver mais que 1 clique
                    touchPositionLux = Input.touches[1].position;
            }
            // end Lux

            if (leftFrist)
            { // Clique começa pela esquerda e atualiza a esquerda
                if (Input.touches[0].position.x < (Screen.width / 2))
                    startTouchLeft = Input.touches[0].position;

                if (Input.touchCount == 2)
                {
                    if (Input.touches[0].position.x < (Screen.width / 2) && Input.touches[1].position.x > (Screen.width / 2))
                    {
                        startTouchLeft = Input.touches[0].position;

                    }
                    else if (Input.touches[0].position.x > (Screen.width / 2) && Input.touches[1].position.x < (Screen.width / 2))
                    {
                        startTouchLeft = Input.touches[1].position;

                    }
                }
            }
            else if (rightFirst)
            { // Começa o primeiro clique pela direita e segura
                if ((Input.touches[0].position.x < (Screen.width / 2)))
                {
                    // Atualiza a posição do clique
                    startTouchLeft = Input.touches[0].position;


                }

                if (Input.touches[0].position.x > (Screen.width / 2))
                { // Clique começa pela direita e atualiza direita porque o primeiro touch (direita) foi retirado 

                    startTouchRight = Input.touches[0].position;

                }

                if (Input.touchCount == 2)
                {
                    if ((Input.touches[1].position.x < (Screen.width / 2)))
                    {
                        // Atualiza posição do clique da esqueda caso tenha mais de 1 clique
                        startTouchLeft = Input.touches[1].position;

                    }
                }


            }


                if (!leftSideScreen && isGroundedMain)
                {
                    walkingLeft = false;
                    walkingRight = false;
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
         

        }



    } // End Update




    private void Reset1()
    {

        tapRequested1Click = isDragging1Click = isDragging1BeforeLux = false;

    }

    private void Reset2()
    {

        tapRequested2Click = isDragging2Click = false;



    }

    private IEnumerator RightSideDelay()
    {
        yield return new WaitForSeconds(0.2f);

        rightSideScreen = false;
    }

    private IEnumerator JumpOffDelay()
    { // Reseta configurações após pulo


        isJumping = true;

        yield return new WaitForSeconds(0.5f);

        isJumping = false;
    }
    private IEnumerator JumpOffDelay2()
    {// Reseta configurações após pulo



        yield return new WaitForSeconds(0.2f);

        playerAnimator.SetBool("Jump", false);
        blockLoop = false;

    }
    private IEnumerator ResetTapDelay()
    {// Reseta configurações após pulo



        yield return new WaitForSeconds(0.1f);

        tap1 = tap2 = false;

    }


    private IEnumerator OfffLuxModeDelay()
    { // Delay depois que desativa o Lux mode



        yield return new WaitForSeconds(0.4f);

        UIController.instance.luxMode = false;

    }
    private IEnumerator SetGravityDelay()
    { // Muda gravidade por alguns milesimos dps que pega lux


        GetComponent<Rigidbody2D>().gravityScale = 0.1f;

        yield return new WaitForSeconds(0.1f);

        GetComponent<Rigidbody2D>().gravityScale = 0.6f;

    }

    private IEnumerator OldPositionDelay()
    { // Pega posição antiga pra ver se o personagem não está parado 



        yield return new WaitForSeconds(0.1f);

        oldPosition = playerTransform.position.x;

    }
    private IEnumerator ResetSpeedDelay()
    {

        speed = 5f;

        yield return new WaitForSeconds(0.1f);

        speed = 0.9f;

    }

    private IEnumerator ShowLuxDelay()
    {



        yield return new WaitForSeconds(0.1f);

        LuxMain.SetActive(true);

    }

    private IEnumerator ShowClone1Delay()
    { // Delay para mostrar Clone 1



        yield return new WaitForSeconds(0.2f);

        playerClone1.SetActive(true);

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        countCollision++;
        if (collision.gameObject.tag == "Ground") // Colisão com o chão
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
        if (collision.gameObject.CompareTag("Lux1")) // Colisão com Lux 1
        {
            if (!UIController.instance.luxMode)
            {
                firstJump = false;

                if (isJumping)
                {
                    fRemenberJump = 0;

                }
                StartCoroutine(SetGravityDelay());
            }

            collision.gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Lux2"))// Colisão com Lux 2
        {
            if (!UIController.instance.luxMode)
            {
                StartCoroutine(ResetSpeedDelay());
            }

            collision.gameObject.SetActive(false);
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        directionGround = transform.position - collision.gameObject.transform.position;
        countCollision--;

        if (collision.gameObject.tag == "Ground" && countCollision == 0) // Deixou o chão
        {

            isGroundedMain = false;


        }
    }


    private void ComfirmIfIsGrounded() /// Confirma se o personagem está realmente no chão (evitar bugs)
    {

        Ray2D ray = new Ray2D(transform.position, Vector2.down);

        RaycastHit2D hit2 = Physics2D.Raycast(ray.origin, ray.direction, groundCheckDistance2, groundLayers);



        if (hit2)
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
        if (countCollision == 0 && isGroundedMain)
        {
            isGroundedMain = false;
        }

    }


    private void Jump()
    {
        if (!firstJump)
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
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            playerAnimator.SetBool("Jump", true);

            StartCoroutine(JumpOffDelay());
            StartCoroutine(JumpOffDelay2());

        }
        startTouchRight = Vector2.zero;

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
                StartCoroutine(OfffLuxModeDelay());

            }

        }

        // Criar Lux com clique do mouse
        if (UIController.instance != null)
        {
            if (UIController.instance.luxMode && !UIController.instance.uIClick)
            {
                if (Input.touchCount == 0 && Input.GetMouseButtonDown(0))
                {

                    touchPositionLux = Input.mousePosition;
                }
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