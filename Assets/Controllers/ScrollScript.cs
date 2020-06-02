using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollScript : MonoBehaviour
{
    public float speed;
    private float aux = 0;
    private Vector2 offset;
    private MeshRenderer mesh;
    

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
      
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerController.instance.walkingLeft)
        {
            aux -= Convert.ToSingle(Math.Round(Time.deltaTime * speed, 5));
       
              
                offset = new Vector2(aux, 0);
                
                mesh.sharedMaterial.SetTextureOffset("_MainTex", offset);
               
         
        }

        if (PlayerController.instance.walkingRight)
        {
            aux += Convert.ToSingle(Math.Round(Time.deltaTime * speed, 5));
     
                
                print("atual: " + aux);
                offset = new Vector2(aux, 0);
                
                mesh.sharedMaterial.SetTextureOffset("_MainTex", offset);
               
               
         
        }

    }








}