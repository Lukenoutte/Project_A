using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject lux1, buttonLeft, buttonRight, playerClone1;

    private GameObject LuxMain;

    public static PlayerController instance { set; get; }
    public Rigidbody2D rb;
    public ParticleSystem dust;
    [SerializeField]
    private float speed, jumpForce;
    private Vector3 directionGround = Vector3.zero;
    public GameObject setaOld;
    private int countCollision = 0;
    public bool firstJump;


    

    public bool fakeWalk, walkingRight, walkingLeft, isGroundedMain, confirmGrounded;
    public LayerMask groundLayers;
    public float groundCheckDistance2, valueOfIncreace, fRemenberJumpTime, fRemenberJump, oldVelocityX, oldVelocityY;
    private bool blockLoop, wasLuxMode, isJumping = false;
    private float oldPosition, directionYValue, positionMiddleArrows,
        increaceSpeedLeft, increaceSpeedRight;


    private Animator setaOldAnimator, playerAnimator, buttonWalkLeftAnimator, buttonWalkRightAnimator;
    private SpriteRenderer playerSpriteRender;
    private Transform playerTransform, luxTransform;

    private UserInputMobile userInputMobileIntance;
    private UserInputPC userInputPcIntance;

    // Start is called before the first frame update
    void Start()
    {
        userInputMobileIntance = UserInputMobile.instance;
        userInputPcIntance = UserInputPC.instance;
        LuxMain = lux1;
        buttonWalkLeftAnimator = buttonLeft.GetComponent<Animator>();
        buttonWalkRightAnimator = buttonRight.GetComponent<Animator>();

        playerTransform = GetComponent<Transform>();
        playerSpriteRender = GetComponent<SpriteRenderer>();

        playerAnimator = GetComponent<Animator>();
        positionMiddleArrows = Screen.width / 5;
        directionYValue = 1.61f;
        instance = this;
        
        if (setaOld != null)
        {
            setaOldAnimator = setaOld.GetComponent<Animator>();
        }

     
        rb = GetComponent<Rigidbody2D>();



    }

    
    void Update()
    {


        SomeAnimations();

        if (positionMiddleArrows != Screen.width / 5)
        {
            positionMiddleArrows = Screen.width / 5;
        }

        // Death
        if (playerTransform.position.y < -13f)
        {
            SceneManager.LoadScene(0);
        }

        ComfirmIfIsGrounded();


        if (walkingLeft | walkingRight)
        {
            if (countCollision > 1)
            {
                StartCoroutine(OldPositionDelay());
            }

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

                    if (!userInputMobileIntance.isPressed && !userInputPcIntance.isPressedKeys)
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

                if (userInputPcIntance.rightKey)
                {
                    WalkRight();
                }
                else
                {

                    if (!userInputPcIntance.leftKey)
                        playerAnimator.SetBool("WalkRight", false);
                    if (userInputMobileIntance.isDragging1Click)
                        walkingRight = false;

                }


                if (userInputPcIntance.leftKey)
                {

                    WalkLeft();
                }
                else
                {

                    if (!userInputPcIntance.rightKey)
                        playerAnimator.SetBool("WalkRight", false);

                    if (!userInputMobileIntance.isDragging1Click)
                        walkingLeft = false;

                }
                #endregion

                #region Mobile Moviments
                if (userInputMobileIntance.startTouchLeft.x > positionMiddleArrows)// Andar para direita (mobile)
                {

                    if (userInputMobileIntance.leftSideScreen)
                    {
                        if (userInputMobileIntance.isDragging1Click | userInputMobileIntance.isDragging2Click)
                        {
                            WalkRight();

                        }
                        else
                        {
                            playerAnimator.SetBool("WalkRight", false);

                        }
                    }
                }



                if (userInputMobileIntance.startTouchLeft.x < positionMiddleArrows) // Andar para esquerda (mobile)
                {
                    if (userInputMobileIntance.leftSideScreen)
                    {
                        if (userInputMobileIntance.isDragging1Click | userInputMobileIntance.isDragging2Click)
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

                if (userInputPcIntance.upKey | userInputMobileIntance.jumpTap)
                {
                    if (!UIController.instance.luxMode)
                        fRemenberJump = fRemenberJumpTime;
                    userInputMobileIntance.jumpTap = false;
                    userInputPcIntance.upKey = false;

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



                }

                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                playerAnimator.SetFloat("Velocity", 1f);
            }
            else
            { // Lux mode ativo
                fRemenberJump = 0;
                userInputMobileIntance.jumpTap = false;
                userInputPcIntance.upKey = false;
                StartCoroutine(ShowClone1Delay());
   
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
        if (!userInputMobileIntance.isPressed && !userInputPcIntance.isPressedKeys)
        {
            if (isGroundedMain)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                increaceSpeedLeft = increaceSpeedRight = 0;
            }
        }



        if (!isGroundedMain)
        {
            playerAnimator.SetBool("InTheAir", true);


        }
        else
        {

            playerAnimator.SetBool("InTheAir", false);

            if (!blockLoop) // Proíbe o pulo de resetar no momento do pulo
            {

                firstJump = false;

            }


        }

       

        // Criar Lux se estiver em lux mode
        CreateLux();

    } // End Update




    

   

    private IEnumerator JumpOffDelay()
    { // Reseta configurações após pulo


        isJumping = true;

        yield return new WaitForSeconds(0.8f);

        isJumping = false;
    }
    private IEnumerator JumpOffDelay2()
    {// Reseta configurações após pulo



        yield return new WaitForSeconds(0.2f);

        playerAnimator.SetBool("Jump", false);
        blockLoop = false;

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
            ContactPoint2D [] contacts = new ContactPoint2D[1];
            Tilemap map = collision.gameObject.GetComponent<Tilemap>();
            collision.GetContacts(contacts);
            
            Vector3Int colliderPos = map.WorldToCell(contacts[0].point);
           
            
            directionGround = transform.position - colliderPos;
            
           
            if (directionGround.y >= directionYValue)
            {

                dust.Play();

                isGroundedMain = true;

            }



        }

        if (collision.gameObject.tag == "Enemy") // Inimigo
        {

            SceneManager.LoadScene(0);

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
        userInputMobileIntance.startTouchRight = Vector2.zero;

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




    private void CreateLux()
    {
        if (userInputMobileIntance.touchPositionLux != Vector2.zero)
        {

            Vector3 auxLux = Camera.main.ScreenToWorldPoint(userInputMobileIntance.touchPositionLux);
            Debug.DrawRay(userInputMobileIntance.touchPositionLux, Camera.main.transform.forward, Color.green);
            if (!UIController.instance.uIClick && UIController.instance.luxMode)
            {
                luxTransform = LuxMain.GetComponent<Transform>();
                luxTransform.position = new Vector3(auxLux.x, auxLux.y, luxTransform.position.z);
                userInputMobileIntance.touchPositionLux = Vector2.zero;
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

                    userInputMobileIntance.touchPositionLux = Input.mousePosition;
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