
using System.Collections;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float speed, jumpForce;


    public static PlayerController instance { set; get; }
    [HideInInspector] public Rigidbody2D rb;
    public ParticleSystem dust;

    public Vector3 valueToDecreace, valueToIncreace;
    
    public bool isGrounded, confirmIfIsGrounded, walkingRight, walkingLeft, firstJump;
    public float groundCheckDistance;
    public LayerMask groundLayers;
    [HideInInspector] public float valueOfIncreace, fRemenberJumpTime, fRemenberJump;
    [HideInInspector] public Transform playerTransform;

    private Vector3 groundSideValue = Vector3.zero;
    private int countCollision = 0;     
    private bool blockLoop, isJumping, jumpTap = false;
    private float increaceSpeedLeft, increaceSpeedRight;
    private Animator playerAnimator;
    private SpriteRenderer playerSpriteRender;
    private UserInputMobile userInputMobileIntance;
    private UserInputPC userInputPcIntance;
    private UIController uiControlerInstance;




    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GetComponent<Transform>();
        playerSpriteRender = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        instance = this;
        rb = GetComponent<Rigidbody2D>();

    }


    void Update()
    {

        #region Not null instances
        if (userInputMobileIntance == null)
        {
            userInputMobileIntance = UserInputMobile.instance;
        }
        if (userInputPcIntance == null)
        {
            userInputPcIntance = UserInputPC.instance;
        }
        if (uiControlerInstance == null)
        {
            uiControlerInstance = UIController.instance;
        }
        # endregion

        if(!userInputMobileIntance.isPressingTouch && !userInputPcIntance.isPressingKeys) // Reseta varariaveis de movimento se estiver parado
        {
            walkingLeft = false;
            walkingRight = false;

            if (isGrounded)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }


        #region Moviments
        if (!isGrounded)
        { // Reduz a velocidade se não ta clicando pra andar
            ReduceVelocityIfNotClicking();
        }
        else { 

            if (!blockLoop) // Proíbe o pulo de resetar no momento do pulo
            {

                firstJump = false;

            }
        }

        // Movimento usando teclas (PC)
        if (userInputPcIntance.rightKey) // Direita
        {
            WalkRight();
        }
        else
        {
            if (!userInputPcIntance.leftKey)
                playerAnimator.SetBool("WalkRight", false);
        }


        if (userInputPcIntance.leftKey) // Esquerda
        {
            WalkLeft();
        }
        else
        {
            if (!userInputPcIntance.rightKey)
                playerAnimator.SetBool("WalkRight", false);
        }
        #endregion

        #region Mobile Moviments
        if (userInputMobileIntance.startTouchLeft.x > uiControlerInstance.positionMiddleArrows)// Andar para direita (mobile)
        {

            if (userInputMobileIntance.leftSideScreen)
            {
                if (userInputMobileIntance.isDragging1Touch | userInputMobileIntance.isDragging2Touch)
                {
                    WalkRight();

                }
                else
                {
                    playerAnimator.SetBool("WalkRight", false);

                }
            }
        }



        if (userInputMobileIntance.startTouchLeft.x < uiControlerInstance.positionMiddleArrows) // Andar para esquerda (mobile)
        {
            if (userInputMobileIntance.leftSideScreen)
            {
                if (userInputMobileIntance.isDragging1Touch | userInputMobileIntance.isDragging2Touch)
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

        if (fRemenberJump >= 0) // Pulo timer
        {
            fRemenberJump -= Time.deltaTime;
        }

        if (userInputPcIntance.upKey | jumpTap)
        {

            fRemenberJump = fRemenberJumpTime;
            jumpTap = false;
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



        PersonDontSlide();
        IsTappedToJump();
        IsGroundedFunc();

        
    } // End Update



    private IEnumerator isJumpingDelay()
    { // Reseta configurações após pulo


        isJumping = true;

        yield return new WaitForSeconds(0.8f);

        isJumping = false;
    }
    private IEnumerator JumpOffAnimationsDelay()
    {// Reseta configurações após pulo



        yield return new WaitForSeconds(0.2f);

        playerAnimator.SetBool("Jump", false);
        blockLoop = false;

    }


   
    private void IsGroundedFunc() /// Confirma se o personagem está realmente no chão
    {

        

        Ray2D rayLeft = new Ray2D(transform.position - valueToDecreace, -Vector3.up);
        Ray2D rayRight = new Ray2D(transform.position + valueToIncreace, -Vector3.up);
        

        RaycastHit2D hit1 = Physics2D.Raycast(rayLeft.origin, rayLeft.direction, groundCheckDistance, groundLayers);
        RaycastHit2D hit2 = Physics2D.Raycast(rayRight.origin, rayRight.direction, groundCheckDistance, groundLayers);
        Debug.DrawRay(transform.position - valueToDecreace, Vector2.down, Color.green);
        Debug.DrawRay(transform.position + valueToIncreace, Vector2.down, Color.green);

        if (hit1 && hit2)
        {
            isGrounded = true;
            playerAnimator.SetBool("InTheAir", false);
    
        }
        else if(!hit1 && !hit2)
        {
            isGrounded = false;
            playerAnimator.SetBool("InTheAir", true);
        }
        else if( (!hit1 && hit2) || (hit1 && !hit2))
        {
            if (confirmIfIsGrounded)
            {
                isGrounded = true;
                playerAnimator.SetBool("InTheAir", false);
            }
        }



    }


    private void Jump()
    {
        if (!firstJump)
        {
            blockLoop = true;
            if (!firstJump)
            {

                if (isGrounded)
                {

                    dust.Play();

                }

                firstJump = true;


            }
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            playerAnimator.SetBool("Jump", true);

            StartCoroutine(isJumpingDelay());
            StartCoroutine(JumpOffAnimationsDelay());

        }
        userInputMobileIntance.startTouchRight = Vector2.zero;

    }
    private void WalkRight()
    {

        if (isGrounded)
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
        if (isGrounded)
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






    private void ReduceVelocityIfNotClicking()
    {
        float newVelocity = rb.velocity.x;
        if (!userInputMobileIntance.isPressingTouch && !userInputPcIntance.isPressingKeys)
        {
            if (newVelocity > 0)
            {
                newVelocity -= Time.deltaTime * 6;
            }
            else
            {
                newVelocity += Time.deltaTime * 6;
            }

            rb.velocity = new Vector2(newVelocity, rb.velocity.y);
        }
    }

    public void IsTappedToJump()
    {
        if (userInputMobileIntance.tap2 | userInputMobileIntance.tap1)
        {

            if (userInputMobileIntance.rightSideScreen)
            {
                jumpTap = true;
            }
            StartCoroutine(userInputMobileIntance.ResetTapDelay());
        }
    }

    private void PersonDontSlide()
    {
        if (!userInputMobileIntance.isPressingTouch && !userInputPcIntance.isPressingKeys)
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                increaceSpeedLeft = increaceSpeedRight = 0;
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        countCollision++;
        if (collision.gameObject.layer == 9) // Colisão com o chão  (9 é a layer do Ground)
        {

            if (collision.gameObject.GetComponent<Tilemap>() != null)
            {

                ContactPoint2D[] contacts = new ContactPoint2D[1];
                Tilemap map = collision.gameObject.GetComponent<Tilemap>();
                collision.GetContacts(contacts);
                Vector3Int colliderPosTile = map.WorldToCell(contacts[0].point);
                groundSideValue = transform.position - colliderPosTile;
            }
            else
            {
                groundSideValue = transform.position - collision.transform.position;
             
            }
            print(collision.collider.bounds.extents.y);
            print(groundSideValue.y);
            if (groundSideValue.y >= 1.1f)
            {
                confirmIfIsGrounded = true;

            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
      
        countCollision--;

        if (collision.gameObject.layer == 9) // Deixou o chão
        {

            confirmIfIsGrounded = false;

        }
    }




}