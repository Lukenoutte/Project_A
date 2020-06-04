using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.PlayerLoop;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance { set; get; }
    public TextMeshProUGUI textDisplay;
    public string[] sentences;
    private int index;
    private float speed;
    public bool continueButton = false;
    private bool allowedContinue = false;

    void Start()
    {
        instance = this;
        textDisplay.text = "";
        speed = 0.05f;
        StartCoroutine(Type());
        
    }

    void Update()
    {
        if(sentences[index] == textDisplay.text)
        {
            allowedContinue = true;
        }

        if (continueButton && allowedContinue)
        {
            NextSentence();
            continueButton = false;
        }
    }

    IEnumerator Type()
    {
        {
            foreach (char letter in sentences[index].ToCharArray())
            {
                textDisplay.text += letter;
                yield return new WaitForSeconds(speed);
            }

        }

    }

    public void NextSentence()
    {
        if(index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
    }
}