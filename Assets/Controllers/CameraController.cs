
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject character;
    private Vector3 positionT;
    public float smoothTime;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(800, 400, true);
        positionT = character.GetComponent<Transform>().position;

    }

    // Update is called once per frame
    void FixedUpdate() {
        
        positionT = character.GetComponent<Transform>().position;
        Vector3 desiredPosition = positionT + offset;
        Vector3 smoothedPosition = Vector3.Lerp(gameObject.GetComponent<Transform>().position, desiredPosition, smoothTime);
        gameObject.GetComponent<Transform>().position = smoothedPosition;

    }
}
