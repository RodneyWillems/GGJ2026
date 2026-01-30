using UnityEngine;

public class KeysAxe : Interactable
{
    [SerializeField] private string itemName;
    public override void Interact(PlayerMovement player)
    {
        player.UpdateInventory(itemName);

        
    }
}
