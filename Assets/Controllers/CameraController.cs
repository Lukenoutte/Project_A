using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject character;
    private Vector3 position;
    // Start is called before the first frame update
    void Start()
    {


        position = character.GetComponent<Transform>().position;

    }

    // Update is called once per frame
    void Update()
    {
        position = character.GetComponent<Transform>().position;
        gameObject.GetComponent<Transform>().position = new Vector3(position.x, position.y+0.1f, gameObject.GetComponent<Transform>().position.z);
        
    }
}
