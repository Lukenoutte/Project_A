using System.Collections;

using UnityEngine;

public class UserInputMobile : MonoBehaviour
{

    public bool isDragging1Touch, tapRequested1Touch, isDragging2Touch,
        tapRequested2Touch, tap1, tap2, rightFirst, leftFrist, rightSideScreen, leftSideScreen, isPressingTouch, jumpTap;

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


            isPressingTouch = true;

        }
        else 
        {
            tapRequested2Touch = isDragging2Touch = false;
            isPressingTouch = false;

        }


        #region Mobile Inputs
        if (Input.touchCount > 0)
        {


            if (Input.touches[0].phase == TouchPhase.Began)
            {
 
               

                isDragging1Touch = true;
                tapRequested1Touch = true;
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
                if (tapRequested1Touch)
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
                    if (!UIController.instance.uIClick)
                    {

                        touchPositionLux = Input.touches[1].position;

                    }
                    // End Lux

                    isDragging2Touch = true;
                    tapRequested2Touch = true;


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
                    if (tapRequested2Touch)
                    {
                        tap2 = true;

                    }

                    Reset2();
                }
            }




        }// end if Touch > 0
        #endregion


        if (isDragging1Touch | isDragging2Touch) // Atualiza posição quando o clique é segurado
        {

          

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

        Tap();

    }

    private void Reset1()
    {

        tapRequested1Touch = isDragging1Touch = false;

    }

    private void Reset2()
    {

        tapRequested2Touch = isDragging2Touch = false;


    }

    public void Tap()
    {
        if (tap2 | tap1)
        {

            if (rightSideScreen)
            {


                    jumpTap = true;

                

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
