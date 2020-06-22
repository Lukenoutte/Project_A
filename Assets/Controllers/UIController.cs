using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public bool uIClick;
    private GraphicRaycaster canvasRaycaster;
    public bool luxMode;
    public static UIController instance { set; get; }

    private void Start()
    {
        instance = this;
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
           
            if (result.gameObject.name == "LuxButton")
            {
                uIClick = true;
                LuxMode();
            }

       }
       
        if(results.Count == 1)
        {
            uIClick = false;
        }
    }




    public void LuxMode()
    {


        if (!luxMode)
        {
            luxMode = true;

        }
        else
        {
            luxMode = false;

        }

    }
}
