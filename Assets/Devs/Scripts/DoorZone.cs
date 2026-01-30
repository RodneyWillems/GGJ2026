using UnityEngine;

public class DoorZone : MonoBehaviour
{
    [SerializeField] private Door m_doorToClose;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            print("hiero");
            m_doorToClose.CloseDoor();
        }
    }
}
