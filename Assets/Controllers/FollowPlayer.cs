using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public float speed;
    public GameObject player;
    private Transform target;
    public int stoppingDistance;



    void Start()
    {
        target = player.GetComponent<Transform>();


    }

    void FixedUpdate()
    {
       
        if (Mathf.Round(Vector2.Distance(transform.position, target.position)) > stoppingDistance)
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

    }
}