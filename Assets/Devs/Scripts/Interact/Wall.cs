using UnityEngine;

public class Wall : Interactable
{
    [SerializeField] private GameObject wallWhole;
    [SerializeField] private GameObject wallBroken;

    public override void Interact(PlayerMovement player)
    {
        if (player.CheckInventory("axe"))
        {
            wallWhole.SetActive(false);
            wallBroken.SetActive(true);
        }
    }
}
