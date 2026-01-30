using UnityEngine;

public class DoorZone : MonoBehaviour
{
    [SerializeField] private Animation closeDoor;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            closeDoor.Play();
        }
    }
}
