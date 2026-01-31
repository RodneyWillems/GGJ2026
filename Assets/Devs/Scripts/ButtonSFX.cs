using UnityEngine;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour
{
    [SerializeField] private AudioClip m_hoverSFX;
    [SerializeField] private AudioClip m_clickSFX;

    public void PlayHoverSound()
    {
        GetComponent<AudioSource>().clip = m_hoverSFX;
        GetComponent<AudioSource>().Play();
    }

    public void PlayClickSound()
    {
        GetComponent<AudioSource>().clip = m_clickSFX;
        GetComponent<AudioSource>().Play();
    }
}
