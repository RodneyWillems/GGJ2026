using UnityEngine;

public class PositionFix : MonoBehaviour
{
    public Transform targetPosition;

    void Update()
    {
        transform.position = targetPosition.position;
    }
}
