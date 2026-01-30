using UnityEngine;

public class Wall : Interactable
{
   // [SerializeField] private GameObject wallWhole;
    [SerializeField] private GameObject wallBroken;

    private void Start()
    {
        wallBroken.SetActive(false);
    }

    public override void Interact(PlayerMovement player)
    {
       if (player.CheckInventory("axe"))
        {
            gameObject.SetActive(false);
            wallBroken.SetActive(true);
       }
    }
}
