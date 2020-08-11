using UnityEngine;

public class Animations : MonoBehaviour
{
    public Animator  buttonWalkLeftAnimator, buttonWalkRightAnimator;
    public GameObject buttonLeft, buttonRight;
    private PlayerController playerControllerInstance;
    // Start is called before the first frame update
    void Start()
    {

        playerControllerInstance = PlayerController.instance;

        buttonWalkLeftAnimator = buttonLeft.GetComponent<Animator>();
        buttonWalkRightAnimator = buttonRight.GetComponent<Animator>();
    
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControllerInstance != null)
        {
            SomeAnimations();

        }
        else
        {
            playerControllerInstance = PlayerController.instance;
        }
    }

    private void SomeAnimations()
    {

        if (buttonLeft != null && buttonRight != null)
        {
            if (playerControllerInstance.walkingRight)
            {
                buttonWalkRightAnimator.SetBool("Press", true);
            }
            else
            {
                buttonWalkRightAnimator.SetBool("Press", false);
            }

            if (playerControllerInstance.walkingLeft)
            {
                buttonWalkLeftAnimator.SetBool("Press", true);
            }
            else
            {
                buttonWalkLeftAnimator.SetBool("Press", false);
            }

        }


    }
}

