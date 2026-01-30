using UnityEditor.Animations;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private string doorOrBarn;


    public override void Interact(PlayerMovement player)
    {
        if (doorOrBarn == "door")
        {
            print("nice door");
            if (player.CheckInventory("key 1"))
            {
                GetComponent<Animator>().SetTrigger("OpenDoor");
            }
        }
       if (doorOrBarn == "barn")
        {
            if (player.CheckInventory("key 2"))
            {
                // Win game
            }
        }
       
    }

    public void CloseDoor()
    {
        GetComponent<Animator>().SetTrigger("Close");
    }
}
