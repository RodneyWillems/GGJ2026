using UnityEngine;

public class SCSpawnPoint : MonoBehaviour
{
    private Gamemanager m_Gamemanager;

    private void Awake()
    {
       m_Gamemanager = FindAnyObjectByType<Gamemanager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            m_Gamemanager.FindSpawnPoint();
        }
        else
        {
            m_Gamemanager.SpawnScareCrow();
        }
    }
}
