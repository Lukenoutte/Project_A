
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public bool uIClick;
    private GraphicRaycaster canvasRaycaster;
    public float positionMiddleArrows;

    public static UIController instance { set; get; }

    private void Start()
    {
        instance = this;
        positionMiddleArrows = Screen.width / 5;

    }

    private void Update()
    {
        if (positionMiddleArrows != Screen.width / 5)
        {
            positionMiddleArrows = Screen.width / 5;
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


            if (result.gameObject.name == "teclaRight" | result.gameObject.name == "teclaLeft")
            {
                uIClick = true;

            }



        }

        if (results.Count == 1)
        {
            uIClick = false;

        }
    }





}
