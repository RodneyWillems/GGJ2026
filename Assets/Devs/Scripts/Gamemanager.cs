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
                Vector2 randomPoint = (Vector2)player.transform.position + Random.insideUnitCircle * 5;
                scareCrow.transform.position = new Vector3(randomPoint.x, 1, randomPoint.y);
                Collider[] spawns = Physics.OverlapSphere(randomPoint, 2);
                toomuch++;
                if (spawns.Length == 0)
                {
                    found = true;
                    SpawnScareCrow();
                }
                else if (toomuch == 10)
                {
                    print("gaat te ver kil");
                    found = true;
                    toomuch = 0;
                }
            }
        }
    }

    public void SpawnScareCrow()
    {
        scareCrow.transform.GetChild(0).gameObject.SetActive(true);
        found = true;
        //timing = false;
    }
    #endregion
}