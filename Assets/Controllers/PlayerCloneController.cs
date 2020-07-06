using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCloneController : MonoBehaviour
{
    public static PlayerCloneController instance { set; get; }
    private Rigidbody2D rb;
    private Transform playerCloneTransform;
    public GameObject PlayerToFollow;
    private bool setPositionClone;
    private PlayerController playerControllerInstance;
    private Animator playerCloneAnimator;
    private SpriteRenderer playerSpriteRender, playerFollowRender;
    private BoxCollider2D playerToFollowCollider;
    public BoxCollider2D playerCloneCollider;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerControllerInstance = PlayerController.instance;
        playerCloneTransform = GetComponent<Transform>();
        playerSpriteRender = GetComponent<SpriteRenderer>();
        playerCloneAnimator = GetComponent<Animator>();
        playerToFollowCollider = PlayerToFollow.GetComponent<BoxCollider2D>();
        playerCloneCollider = GetComponent<BoxCollider2D>();
        playerFollowRender = PlayerToFollow.GetComponent<SpriteRenderer>();
        instance = this;


    }

    // Update is called once per frame
    void Update()
    {
       

        if (UIController.instance.luxMode)
        {


            if (!setPositionClone)
            {
                if (playerControllerInstance == null)
                {
                    playerControllerInstance = PlayerController.instance;
                }

                if (playerFollowRender.flipX)
                {
                    playerSpriteRender.flipX = true;
                }
                else
                {
                    playerSpriteRender.flipX = false;
                }

                if (playerControllerInstance.oldVelocityX != 0 | playerControllerInstance.oldVelocityY != 0)
                {

                    playerCloneTransform.position = new Vector3(PlayerToFollow.transform.position.x, PlayerToFollow.transform.position.y, 6);
                    if (!playerControllerInstance.isGroundedMain)
                    {
                        rb.velocity = new Vector2(playerControllerInstance.oldVelocityX, playerControllerInstance.oldVelocityY);
                    }
                    
                    StartCoroutine(FreezeCloneDelay());
                    setPositionClone = true;
                }
            }
        }
        else
        {
            playerCloneTransform.position = new Vector3(PlayerToFollow.transform.position.x, PlayerToFollow.transform.position.y, 6);
            gameObject.SetActive(false);
            setPositionClone = false;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }



    private IEnumerator FreezeCloneDelay()
    {



        yield return new WaitForSeconds(0.15f);

        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {


            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            playerCloneAnimator.SetBool("InTheAir", false);
        }
        else
        {
            playerCloneAnimator.SetBool("InTheAir", true);
        }

    }
}
