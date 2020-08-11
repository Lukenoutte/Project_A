
using System.Collections;

using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { set; get; }
    public Rigidbody2D rb;
    public ParticleSystem dust;
    [SerializeField]
    private float speed, jumpForce;
    private Vector3 directionGround = Vector3.zero;

    private int countCollision = 0;
    public bool firstJump;
    public bool fakeWalk, isGroundedMain, confirmGrounded;
    public bool walkingRight, walkingLeft;
    public LayerMask groundLayers;
    public float groundCheckDistance2, valueOfIncreace, fRemenberJumpTime, fRemenberJump, oldVelocityX, oldVelocityY;
    private bool blockLoop, isJumping = false;
    private float oldPosition, directionYValue,
        increaceSpeedLeft, increaceSpeedRight;


    private Animator playerAnimator;
    private SpriteRenderer playerSpriteRender;
    private Transform playerTransform;

    private UserInputMobile userInputMobileIntance;
    private UserInputPC userInputPcIntance;
    private UIController uiControlerInstance;


    // Start is called before the first frame update
    void Start()
    {


        playerTransform = GetComponent<Transform>();
        playerSpriteRender = GetComponent<SpriteRenderer>();


        playerAnimator = GetComponent<Animator>();

        directionYValue = 1.61f;
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

        if(!userInputMobileIntance.isPressingTouch && !userInputPcIntance.isPressingKeys)
        {
            walkingLeft = false;
            walkingRight = false;
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
        StopWalkAnimationIfisFake();


        #region Moviments
        if (!isGroundedMain)
        { // Reduz a velocidade se não ta clicando pra andar

            ReduceVelocityIfNotClicking();


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

        if (userInputPcIntance.upKey | userInputMobileIntance.jumpTap)
        {

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



        // Evitar que o personagem deslize
        PersonDontSlide();



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




    private IEnumerator OldPositionDelay()
    { // Pega posição antiga pra ver se o personagem não está parado 



        yield return new WaitForSeconds(0.1f);

        oldPosition = playerTransform.position.x;

    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        countCollision++;
        if (collision.gameObject.tag == "Ground") // Colisão com o chão
        {
            ContactPoint2D[] contacts = new ContactPoint2D[1];
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



    private void OnCollisionExit2D(Collision2D collision)
    {
        directionGround = transform.position - collision.gameObject.transform.position;
        countCollision--;

        if (collision.gameObject.tag == "Ground" && countCollision == 0) // Deixou o chão
        {

            isGroundedMain = false;


        }
    }


    private void ComfirmIfIsGrounded() /// Confirma se o personagem está realmente no chão
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




    private void StopWalkAnimationIfisFake()
    {
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

    private void ReduceVelocityIfNotClicking()
    {
        float newVelocity = rb.velocity.x;
        if (!userInputMobileIntance.isPressingTouch && !userInputPcIntance.isPressingKeys)
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


    private void PersonDontSlide()
    {
        if (!userInputMobileIntance.isPressingTouch && !userInputPcIntance.isPressingKeys)
        {
            if (isGroundedMain)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                increaceSpeedLeft = increaceSpeedRight = 0;
            }
        }
    }

}