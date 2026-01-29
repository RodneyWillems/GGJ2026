using UnityEngine;

public class NoteMap : Interactable
{
    [SerializeField] private string mapOrNote;

    [SerializeField] private GameObject map;
    [SerializeField] private GameObject note;

    private bool mapOpen = false;
    private bool noteOpen = false;

    public override void Interact(PlayerMovement player)
    {
        if (mapOrNote == "map")
        {
            if (mapOpen) 
            {
                map.SetActive(false);
                mapOpen = false;
            }
            else
            {
                map.SetActive(true);
                mapOpen = true;
            }
        }
        if (mapOrNote == "note")
        {
            if (noteOpen)
            {
                note.SetActive(false);
                noteOpen = false;
            }
            else
            {
                note.SetActive(true);
                noteOpen = true;
            }
        }
    }


}
