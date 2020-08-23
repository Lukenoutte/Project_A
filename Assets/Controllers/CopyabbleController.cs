using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyabbleController : MonoBehaviour
{

    public bool isTransformationCopyActive, isTransformed;
    public static CopyabbleController instance { set; get; }
    private CopyableObjectCharacteristics lastTuchedCopyableObject;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

    }

    // Update is called once per frame
    void Update()
    {
        TransformInCopyableObject();
    
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Copyable") // Inimigo
        {

            lastTuchedCopyableObject = collision.gameObject.GetComponent<CopyableObjectCharacteristics>();
        }
    }


    private void TransformInCopyableObject()
    {
        if (isTransformationCopyActive && lastTuchedCopyableObject == null)
        {
            isTransformationCopyActive = false;
            return;
        }

        if (isTransformationCopyActive && lastTuchedCopyableObject != null && !isTransformed)
        {

            print("Transformed");
            print(lastTuchedCopyableObject.canWalk);
            isTransformed = true;
        }
        else if (!isTransformationCopyActive && isTransformed)
        {
            print("Destransformed");
            isTransformed = false;
        }
    }

}
