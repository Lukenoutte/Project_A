
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public bool uIClick;
    private GraphicRaycaster canvasRaycaster;
    public float positionMiddleArrows;
    private Animator buttonWalkLeftAnimator, buttonWalkRightAnimator;
    public GameObject buttonLeft, buttonRight;
    private PlayerController playerControllerInstance;

    public static UIController instance { set; get; }

    private void Start()
    {
        instance = this;
        positionMiddleArrows = Screen.width / 5;
        playerControllerInstance = PlayerController.instance;
        buttonWalkLeftAnimator = buttonLeft.GetComponent<Animator>();
        buttonWalkRightAnimator = buttonRight.GetComponent<Animator>();

    }

    private void Update()
    {
        if (positionMiddleArrows != Screen.width / 5)
        {
            positionMiddleArrows = Screen.width / 5;
        }

        if (playerControllerInstance != null)
        {
            VirtualKeysAnimations();

        }
        else
        {
            playerControllerInstance = PlayerController.instance;
        }

    }

    private void VirtualKeysAnimations()
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
        public void UiClick(BaseEventData data)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData pointerData = data as PointerEventData;

        if (canvasRaycaster == null)
        {
            canvasRaycaster = GetComponent<GraphicRaycaster>();
        }

        canvasRaycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {


            if (result.gameObject.name == "TransformButton")
            {
                uIClick = true;
                if (!playerControllerInstance.isTransformationCopyActive)
                {
                    playerControllerInstance.isTransformationCopyActive = true;
                }
                else
                {
                    playerControllerInstance.isTransformationCopyActive = false;
                }
            }

        }

        if (results.Count == 1)
        {
            uIClick = false;

        }
    }





}
