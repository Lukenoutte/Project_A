using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeController : MonoBehaviour
{
    [SerializeField]
    private GameObject character;
    private Vector3 positionT;
    private Transform characterTransform, beforePosition;
    // Start is called before the first frame update
    void Start()
    {
        positionT = character.GetComponent<Transform>().position;
        characterTransform = character.GetComponent<Transform>();
        beforePosition = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        positionT = characterTransform.position;
        beforePosition.position = new Vector3(positionT.x, beforePosition.position.y, beforePosition.position.z);
    }
}
