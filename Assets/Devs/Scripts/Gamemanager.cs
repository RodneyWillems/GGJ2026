using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    [SerializeField] private GameObject scareCrow;
    [SerializeField] private GameObject player;

    [Header("Timers")]
    [SerializeField] private float scareCrowTime;
    [SerializeField] private float scareCrowTimeOr;

    [SerializeField] private float scareCrowDistance;

    private float randomX;
    private float randomZ;

    private bool timing = true;

    private int toomuch;

    private bool found = false;

    private Vector2 randompoint;
    void Start()
    {
        scareCrowTime = scareCrowTimeOr;
    }

    private void Update()
    {
        if (timing)
        {
            scareCrowTime -= Time.deltaTime;
            if (scareCrowTime < 0)
            {
                scareCrowTime = scareCrowTimeOr;
                found = false;
                FindSpawnPoint();
            }
        }
    }
    #region Scarecrow
    public void FindSpawnPoint()
    {
        while (found == false)
        {
            {
                randompoint = (Vector2)player.transform.position + Random.insideUnitCircle * 5;
                Collider[] spawns = Physics.OverlapSphere(randompoint, 2);
                toomuch++;
                if (spawns.Length == 0)
                {
                    found = true;
                    SpawnScareCrow();
                }
                else if (toomuch == 10)
                {
                    found = true;
                    toomuch = 0;
                }
            }
        }
    }

    public void SpawnScareCrow()
    {
        scareCrow.transform.position = new Vector3(randompoint.x, 0, randompoint.y);
        scareCrow.transform.GetChild(0).gameObject.SetActive(true);
        found = true;
        scareCrowTime = scareCrowTimeOr;
        timing = true;
    }
    #endregion
}