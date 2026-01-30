using UnityEditor.Animations;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private string doorOrBarn;
    [SerializeField] private Animation openDoor;
    [SerializeField] private Animation openBarnDoor;



    public override void Interact(PlayerMovement player)
    {
        if (doorOrBarn == "door")
        {
            if (player.CheckInventory("key 1"))
            {
                openDoor.Play();
            }
        }
       if (doorOrBarn == "barn")
        {
            if (player.CheckInventory("key 2"))
            {
                openBarnDoor.Play();
            }
        }
       
    }
}
