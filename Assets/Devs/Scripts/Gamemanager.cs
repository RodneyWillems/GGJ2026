using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance;

    public Aids AidsScript;

    [SerializeField] private GameObject scareCrow;
    [SerializeField] private GameObject player;

    [Header("Timers")]
    [SerializeField] private float scareCrowTime;
    [SerializeField] private float scareCrowTimeOr;

    [SerializeField] private float scareCrowDistance;

    private float randomX;
    private float randomZ;

    private bool timing = false;
    private int toomuch;
    private bool found = false;
    private Vector2 randompoint;
    private bool changing = false;
    void Start()
    {
        scareCrowTime = scareCrowTimeOr;
        Instance = this;
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
        scareCrow.transform.GetChild(0).GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
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

    public void FocusOnPlayer(float deathTime)
    {
        scareCrow.transform.LookAt(player.transform.position);
        scareCrow.transform.rotation = Quaternion.Euler(0, scareCrow.transform.rotation.y, 0);
        AidsScript.StartEmissionChange(deathTime);
    }

    //private IEnumerator ColorChange(float deathTime)
    //{
    //    float intensity = 0f;
    //    float maxIntensity = 50;
    //    print("Doing the cool thing");
    //    Material material = scareCrow.transform.GetChild(0).GetComponent<MeshRenderer>().material;
    //    Color emissionColor = material.GetColor("_EmissionColor");
    //    material.EnableKeyword("_EMISSION");
    //    material.SetColor("_EmissionColor", emissionColor * intensity);
    //    while (intensity < maxIntensity)
    //    {
    //        print(intensity);
    //        intensity++;
    //        material.SetColor("_EmissionColor", emissionColor * intensity);
    //        yield return new WaitForSeconds(deathTime / maxIntensity);
    //    }
    //}

    #endregion
}