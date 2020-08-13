using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RulesController : MonoBehaviour
{
    private PlayerController playerControllerInstance;
    // Start is called before the first frame update
    void Start()
    {
        playerControllerInstance = PlayerController.instance;

    }

    // Update is called once per frame
    void Update()
    {
        if (playerControllerInstance == null)
        {
            playerControllerInstance = PlayerController.instance;

        }

        // Death
        if (playerControllerInstance.playerTransform.position.y < -13f)
        {
            SceneManager.LoadScene(0);
        }

    }
}
