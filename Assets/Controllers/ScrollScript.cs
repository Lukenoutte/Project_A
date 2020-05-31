using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollScript : MonoBehaviour
{
    public float speed;
    private float aux = 0;
   

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerController.instance.walkingLeft)
        {
            aux -= Time.deltaTime * speed;
            GetComponent<Renderer>().material.mainTextureOffset = new Vector2(aux, 0f);
            
        }
        
        if (PlayerController.instance.walkingRight)
        {
            aux += Time.deltaTime * speed;
            GetComponent<Renderer>().material.mainTextureOffset = new Vector2(aux, 0f);
            
        }


    }






}
