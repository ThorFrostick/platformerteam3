using JetBrains.Annotations;
using UnityEngine;

public class Rotate_Z : MonoBehaviour
{
    //Set the speed of the rotation
    [SerializeField]
    private float rotationSpeed;
    
    // Update is called once per frame
    void Update()
    {
        //Every frame, rotate along the z axis a small amount
        transform.Rotate(new Vector3(0, 0, 1) * rotationSpeed * Time.deltaTime);
    }
}
