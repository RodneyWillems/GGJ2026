using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private string doorOrBarn;

    [SerializeField] private GameObject winScreen;

    private AudioSource m_source;

    private void Start()
    {
        m_source = GetComponent<AudioSource>();
    }

    public override void Interact(PlayerMovement player)
    {
        if (doorOrBarn == "door")
        {
            print("nice door");
            if (player.CheckInventory("key 1"))
            {
                GetComponent<Animator>().SetTrigger("OpenDoor");
                m_source.Play();
            }
        }
       if (doorOrBarn == "barn")
        {
            if (player.CheckInventory("key 2"))
            {
                winScreen.SetActive(true);
                player.WinGame();
                Time.timeScale = 0;
            }
        }
       
    }

    public void CloseDoor()
    {
        GetComponent<Animator>().SetTrigger("Close");
        m_source.Play();
    }
}
