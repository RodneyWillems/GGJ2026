using UnityEngine;

public class TestInteract : Interactable
{
    public override void Interact(PlayerMovement player)
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}
