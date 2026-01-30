using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    [SerializeField] private GameObject scareCrow;
    [SerializeField] private GameObject player;

    [Header("Timers")]
    [SerializeField] private float scareCrowTime;
    [SerializeField] private float scareCrowTimeOr;

    [SerializeField] private float scareCrowDistance;

    private float randomX;
    private float randomZ;

    private bool timing;

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
        float plyercircleX = player.transform.position.x + scareCrowDistance;
        float plyercircleZ = player.transform.position.z + scareCrowDistance;

        randomX = UnityEngine.Random.Range(plyercircleX, -plyercircleX);
        randomZ = UnityEngine.Random.Range(plyercircleZ, -plyercircleZ);
        scareCrow.transform.position = new Vector3(randomX, 0, randomZ);
    }

    public void SpawnScareCrow()
    {
        scareCrow.transform.GetChild(0).gameObject.SetActive(true);
        timing = false;
    }
    #endregion
}