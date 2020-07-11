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
    private PlayerController player;
    
    private void Start()
    {
        player = PlayerController.instance;
        mesh = GetComponent<MeshRenderer>();
        offset = Vector2.zero;
        mesh.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (player != null)
        {
            if (!player.fakeWalk)
            {
                if (player.walkingLeft)
                {
                    aux -= Convert.ToSingle(Math.Round(Time.deltaTime * speed, 5));

                    offset = new Vector2(aux, 0);

                    mesh.sharedMaterial.SetTextureOffset("_MainTex", offset);

                }

                if (player.walkingRight)
                {
                    aux += Convert.ToSingle(Math.Round(Time.deltaTime * speed, 5));

                    offset = new Vector2(aux, 0);

                    mesh.sharedMaterial.SetTextureOffset("_MainTex", offset);

                }

            }
        }
        else
        {
            player = PlayerController.instance;
            
        }

    }
}






