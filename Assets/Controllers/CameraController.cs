
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject character;
    private Vector3 positionT;
    public float smoothTime;
    private Transform cameraTransform, characterTransform;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(720, 360, true);
        positionT = character.GetComponent<Transform>().position;
        cameraTransform = GetComponent<Transform>();
        characterTransform = character.GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        
        positionT = characterTransform.position;
        if(positionT.y < -0.67f)
        {
            positionT.y = -0.67f;
        }
        Vector3 desiredPosition = positionT + offset;
        Vector3 smoothedPosition = Vector3.Lerp(cameraTransform.position, desiredPosition, smoothTime);
        cameraTransform.position = smoothedPosition;

    }
}
