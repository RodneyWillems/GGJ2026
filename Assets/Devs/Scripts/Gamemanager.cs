using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    [SerializeField] private GameObject scareCrow;
    [SerializeField] private GameObject player;

    [SerializeField] private float scareCrowTime;
    [SerializeField] private float scareCrowTimeOr;

    void Start()
    {
        scareCrowTime = scareCrowTimeOr;
    }

    private void Update()
    {
        scareCrowTime -= Time.deltaTime;
        if (scareCrowTime < 0)
        {
            scareCrowTime = scareCrowTimeOr;
            SpawnScareCrow();
        }
    }

    private void SpawnScareCrow()
    {
        float sdaf = player.transform.position.x + 40;
        print(sdaf);
        Instantiate(scareCrow);
    }
}
