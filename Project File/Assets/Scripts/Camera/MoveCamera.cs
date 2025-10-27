using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPoint;

    void Update()
    {
        transform.position = cameraPoint.position;
    }
}
