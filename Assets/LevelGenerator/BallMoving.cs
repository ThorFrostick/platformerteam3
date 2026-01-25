using UnityEngine;

public class BallMoving : MonoBehaviour
{
    public float movingSpeed = 5f;

    void Update()
    {
        transform.Translate(Vector3.forward * movingSpeed * Time.deltaTime);
    }
}
