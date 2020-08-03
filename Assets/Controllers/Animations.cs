using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    public Animator setaOldAnimator, playerAnimator, buttonWalkLeftAnimator, buttonWalkRightAnimator;
    public GameObject buttonLeft, buttonRight;
    public GameObject setaOld;
    private PlayerController playerControllerInstance;
    // Start is called before the first frame update
    void Start()
    {

        if (setaOld != null)
        {
            setaOldAnimator = setaOld.GetComponent<Animator>();
        }
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
        if (setaOld != null)
        {
            if (playerControllerInstance.walkingRight)
            {
                setaOldAnimator.SetBool("WalkingRight", true);
            }
            else
            {
                setaOldAnimator.SetBool("WalkingRight", false);
            }

            if (playerControllerInstance.walkingLeft)
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

