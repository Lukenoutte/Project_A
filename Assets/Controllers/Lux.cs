﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Lux : MonoBehaviour
{
    public bool luxMode;
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
        if (!luxMode)
        {
            luxMode = true;
        }
        else
        {
            luxMode = false;
        }

    }

}