using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputPC : MonoBehaviour
{
    public bool upKey, rightKey, leftKey, isPressedKeys;
    public static UserInputPC instance { set; get; }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        PressKeysPc();
    }


    public void PressKeysPc()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            leftKey = true;
            isPressedKeys = true;
        }
        else
        {
            leftKey = false;

        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            rightKey = true;
            isPressedKeys = true;
        }
        else
        {
            rightKey = false;

        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            upKey = true;
            isPressedKeys = true;
        }
        else
        {
            upKey = false;

        }

        if (!leftKey && !rightKey && !upKey)
        {
            isPressedKeys = false;
        }
    }
}
