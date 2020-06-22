using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Lux : MonoBehaviour
{
    public bool luxMode;
    public bool uIClick;
    public static Lux instance { set; get; }
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LuxMode()
    {
        uIClick = true;
        StartCoroutine(UiBooDelay());

        if (!luxMode)
        {
            luxMode = true;
           
        }
        else
        {
            luxMode = false;
            
        }

    }
    private IEnumerator UiBooDelay()
    {

       

        yield return new WaitForSeconds(0.5f);

        uIClick = false;

    }

}
