using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("UI Elements")]
    [SerializeField] private GameObject m_key1Slot;
    [SerializeField] private GameObject m_key2Slot;
    [SerializeField] private GameObject m_axeSlot;
    [SerializeField] private GameObject m_pauseMenu;
    [SerializeField] private TextMeshProUGUI m_textBox;

    [Header("Movement")]
    [SerializeField] private int m_walkingSpeed;

    private Player m_inputs;

    #endregion

    private void OnEnable()
    {
        m_inputs = new Player();
        m_inputs.Enable();
        m_inputs.Default.Reload.performed += Reload;
        m_inputs.Default.Mask.performed += Mask;
        m_inputs.Default.Interact.performed += Interact;
        m_inputs.Default.Pause.performed += Pause;
    }

    private void OnDisable()
    {
        m_inputs.Disable();
    }

    #region Inputs

    private void Reload(InputAction.CallbackContext context)
    {
        print("Reloading Flashlight");
    }

    private void Mask(InputAction.CallbackContext context)
    {
        print("Putting mask on");
    }

    private void Interact(InputAction.CallbackContext context)
    {
        print("Interacting with an object");
    }

    private void Pause(InputAction.CallbackContext context)
    {
        print("Pausing");
    }

    #endregion

    private void Moving()
    {
        Vector2 direction = m_inputs.Default.Walking.ReadValue<Vector2>();
        transform.position += m_walkingSpeed * Time.deltaTime * new Vector3(direction.x, 0, direction.y);
    }

    private void Rotate()
    {
        Vector2 direction = m_inputs.Default.Mouse.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        Moving();
        Rotate();
    }
}
