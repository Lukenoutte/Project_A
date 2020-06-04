using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogText : MonoBehaviour
{


    public void ButtonContinue()
    {
        DialogManager.instance.continueButton = true;
    }
}
