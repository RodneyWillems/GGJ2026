using System.Collections;
using UnityEngine;

public class Aids : MonoBehaviour
{
    public GameObject TestObject;
    public Color EmissionColor;

    public void StartEmissionChange(float deathTime)
    {
        StartCoroutine(ChangeEmission(deathTime));
    }

    private IEnumerator ChangeEmission(float deathTime)
    {
        float intensity = 0f;
        float maxIntensity = 50f;
        TestObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        TestObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", EmissionColor * intensity);
        while (intensity < maxIntensity)
        {
            intensity++;
            print(intensity);
            TestObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", EmissionColor * intensity);
            yield return new WaitForSeconds(deathTime / maxIntensity);
        }
        TestObject.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
    }

}
