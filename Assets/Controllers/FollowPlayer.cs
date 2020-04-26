using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public GameObject player;
    private Rigidbody2D rbPlayer;
    public float followDistance;
    public float distCheck;
    private SpriteRenderer sRenderer;

    private Rigidbody2D rb;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sRenderer = GetComponent<SpriteRenderer>();
        rbPlayer = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        if (!rbPlayer)
        {
            Debug.Log("The player gameobject was not set");
        }

       

        if (!sRenderer)
        {
            Debug.Log("sRenderer was not set");
        }
        else
        {
            sRenderer.enabled = false;
        }

        if (followDistance == 0)
        {
            Debug.Log("Please set distance higher then 0");
        }
    }

    void Start()
    {
        StartCoroutine(Viewable()); //enable the image
    }

    void Update()
    {
        float dist = Vector2.Distance(rb.position, rbPlayer.position); //distance between player and follower

        if (dist < distCheck)
        {
            //follower is to close to the player
            //Debug.Log(dist);
            StopAllCoroutines(); //stop future movements
            rb.velocity = Vector2.zero; //stop in place
        }
        else
        {
            StartCoroutine(ReplaceRigidBody(rbPlayer.position)); //move closer to player
        }

    }

    IEnumerator ReplaceRigidBody(Vector2 setTo)
    {
        yield return new WaitForSeconds(followDistance);
        sRenderer.enabled = true;
        rb.position = setTo;
    }

    IEnumerator Viewable()
    {
        yield return new WaitForSeconds(followDistance);
        sRenderer.enabled = true;
    }
}
