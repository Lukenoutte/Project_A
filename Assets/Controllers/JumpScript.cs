
using UnityEngine;

public class JumpScript : MonoBehaviour
{

    public void JumpMethod()
    {
        PlayerController.instance.jumpTap = true;
    }
}

