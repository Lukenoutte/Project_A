using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputMobile : MonoBehaviour
{

    public bool isDragging1Click, tapRequested1Click, isDragging2Click, isDragging1BeforeLux,
        tapRequested2Click, tap1, tap2, rightFirst, leftFrist, rightSideScreen, leftSideScreen, isPressed, jumpTap;

    public Vector2 startTouchLeft, startTouchRight, touchPositionLux = Vector2.zero;

    private PlayerController playerControllerInstance;

    public static UserInputMobile instance { set; get; }

    // Start is called before the first frame update
    void Start()
    {
        playerControllerInstance = PlayerController.instance;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // Side screen
        if (Input.touchCount >= 1)
        {
            if (Input.touchCount == 1)
            {
                if (Input.touches[0].position.x < (Screen.width / 2))
                {
                    leftSideScreen = true;

                    if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                    {
                        leftSideScreen = false;
                    }
                }


                if (Input.touches[0].position.x > (Screen.width / 2))
                {
                    rightSideScreen = true;

                    if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                    {
                        StartCoroutine(RightSideDelay());
                    }
                }

            }
            else if (Input.touchCount == 2)
            {
                if (Input.touches[0].position.x < (Screen.width / 2))
                {
                    leftSideScreen = true;
                    if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                    {
                        leftSideScreen = false;
                    }
                }
                else if (Input.touches[1].position.x < (Screen.width / 2))
                {
                    leftSideScreen = true;
                    if (Input.touches[1].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Canceled)
                    {
                        leftSideScreen = false;
                    }

                }


                if (Input.touches[0].position.x > (Screen.width / 2))
                {
                    rightSideScreen = true;
                    if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                    {
                        StartCoroutine(RightSideDelay());
                    }
                }
                else if (Input.touches[1].position.x > (Screen.width / 2))
                {
                    rightSideScreen = true;
                    if (Input.touches[1].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Canceled)
                    {
                        StartCoroutine(RightSideDelay());
                    }

                }

            }
        }

        // Está precionado?
        if (Input.touchCount > 0)
        {


            isPressed = true;

        }
        else 
        {
            tapRequested2Click = isDragging2Click = false;
            isPressed = false;
            if (playerControllerInstance != null)
            {
                playerControllerInstance.walkingLeft = false;
                playerControllerInstance.walkingRight = false;
            }
            else
            {
                playerControllerInstance = PlayerController.instance;

            }

        }


        #region Mobile Inputs
        if (Input.touchCount > 0)
        {


            if (Input.touches[0].phase == TouchPhase.Began)
            {
                // Lux
                if (UIController.instance.luxMode && !UIController.instance.uIClick && Input.touchCount == 1)
                {


                    touchPositionLux = Input.touches[0].position;


                }

                if (!UIController.instance.luxMode)
                {
                    isDragging1BeforeLux = true;
                }
                // End Lux

                isDragging1Click = true;
                tapRequested1Click = true;
                if (Input.touchCount == 1)
                {

                    if (Input.touches[0].position.x < (Screen.width / 2)) // Cliques a esquerda da tela
                    {
                        startTouchLeft = Input.touches[0].position;

                        rightFirst = false;
                        leftFrist = true;

                    }
                    else if (Input.touches[0].position.x > (Screen.width / 2))// Cliques a direita da tela
                    {
                        startTouchRight = Input.touches[0].position;

                        leftFrist = false;
                        rightFirst = true;


                    }

                }


            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                if (tapRequested1Click)
                {
                    tap1 = true;

                }


                Reset1();
            }


            if (Input.touchCount == 2) // 2 Cliques
            {


                if (Input.touches[1].phase == TouchPhase.Began)
                {
                    // Lux
                    if (UIController.instance.luxMode && !UIController.instance.uIClick)
                    {

                        touchPositionLux = Input.touches[1].position;

                    }
                    // End Lux

                    isDragging2Click = true;
                    tapRequested2Click = true;


                    if (Input.touches[1].position.x > Screen.width / 2) // Clique a direita da tela
                    {
                        startTouchRight = Input.touches[1].position;


                    }


                    if (rightFirst)
                    {

                        if (Input.touches[0].position.x > (Screen.width / 2) && Input.touches[1].position.x < (Screen.width / 2))
                        { // Pega clique da esquerda quando está segurando na direita

                            startTouchLeft = Input.touches[1].position;

                        }

                    }

                }
                else if (Input.touches[1].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Canceled)
                {
                    if (tapRequested2Click)
                    {
                        tap2 = true;

                    }

                    Reset2();
                }
            }




        }// end if Touch > 0
        #endregion


        if (isDragging1Click | isDragging2Click) // Atualiza posição quando o clique é segurado
        {

            // Se está no Lux Mode
            if (UIController.instance.luxMode && !UIController.instance.uIClick)
            {
                if (Input.touchCount == 1 && !isDragging1BeforeLux)
                {

                    touchPositionLux = Input.touches[0].position; // Atualiza posição do lux
                }
                if (Input.touchCount == 2 && isDragging1BeforeLux) // Se tiver mais que 1 clique
                    touchPositionLux = Input.touches[1].position;
            }
            // end Lux

            if (leftFrist)
            { // Clique começa pela esquerda e atualiza a esquerda
                if (Input.touches[0].position.x < (Screen.width / 2))
                    startTouchLeft = Input.touches[0].position;

                if (Input.touchCount == 2)
                {
                    if (Input.touches[0].position.x < (Screen.width / 2) && Input.touches[1].position.x > (Screen.width / 2))
                    {
                        startTouchLeft = Input.touches[0].position;

                    }
                    else if (Input.touches[0].position.x > (Screen.width / 2) && Input.touches[1].position.x < (Screen.width / 2))
                    {
                        startTouchLeft = Input.touches[1].position;

                    }
                }
            }
            else if (rightFirst)
            { // Começa o primeiro clique pela direita e segura
                if ((Input.touches[0].position.x < (Screen.width / 2)))
                {
                    // Atualiza a posição do clique
                    startTouchLeft = Input.touches[0].position;


                }

                if (Input.touches[0].position.x > (Screen.width / 2))
                { // Clique começa pela direita e atualiza direita porque o primeiro touch (direita) foi retirado 

                    startTouchRight = Input.touches[0].position;

                }

                if (Input.touchCount == 2)
                {
                    if ((Input.touches[1].position.x < (Screen.width / 2)))
                    {
                        // Atualiza posição do clique da esqueda caso tenha mais de 1 clique
                        startTouchLeft = Input.touches[1].position;

                    }
                }


            }


            if (!leftSideScreen && playerControllerInstance.isGroundedMain)
            {
                playerControllerInstance.walkingLeft = false;
                playerControllerInstance.walkingRight = false;
                playerControllerInstance.rb.velocity = new Vector2(0, playerControllerInstance.rb.velocity.y);
            }



        }




        if (tap2 | tap1)
        {

            if (rightSideScreen)
            {

                if (!UIController.instance.luxMode)
                {
                    jumpTap = true;

                }

            }
            StartCoroutine(ResetTapDelay());
        }


    }

    private void Reset1()
    {

        tapRequested1Click = isDragging1Click = isDragging1BeforeLux = false;

    }

    private void Reset2()
    {

        tapRequested2Click = isDragging2Click = false;


    }

    public void Tap()
    {
        if (tap2 | tap1)
        {

            if (rightSideScreen)
            {

                if (!UIController.instance.luxMode)
                {
                    jumpTap = true;

                }

            }
            StartCoroutine(ResetTapDelay());
        }
    }

    public IEnumerator ResetTapDelay()
    {// Reseta configurações após pulo



        yield return new WaitForSeconds(0.1f);

        tap1 = tap2 = false;

    }


    private IEnumerator RightSideDelay()
    {
        yield return new WaitForSeconds(0.2f);

        rightSideScreen = false;
    }



}
