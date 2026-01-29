using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("UI Elements")]
    [SerializeField] private GameObject m_pauseMenu;
    [SerializeField] private TextMeshProUGUI m_textBox;

    [Header("Movement")]
    [SerializeField] private int m_walkingSpeed;
    [SerializeField] private int m_rotatingSpeed;

    [Header("Inventory")]
    [SerializeField] private GameObject m_key1Slot;
    [SerializeField] private GameObject m_key2Slot;
    [SerializeField] private GameObject m_axeSlot;

    private bool m_hasKey1;
    private bool m_hasKey2;
    private bool m_hasAxe;

    [Header("Flashlight")]
    [SerializeField] private GameObject m_camera;
    [SerializeField] private GameObject m_flashlightPivot;
    [SerializeField] private float m_flashlightPower;
    [SerializeField] private int m_flashlightReloadRate;
    [SerializeField] private int m_flashlightDrainRate;
    [SerializeField] private int m_frameDelay;
    [SerializeField] private LayerMask m_interactableLayer;
    [SerializeField] private Light m_flashlight;

    private float m_flashlightIntensity;
    private float m_maxFlashlightPower;
    private bool m_flashlightOn;
    private List<Quaternion> m_rotationList;

    private Player m_inputs;

    #endregion

    #region Setup

    private void OnEnable()
    {
        //Setup inputs
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

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_rotationList = new();
        m_maxFlashlightPower = m_flashlightPower;
        m_flashlightIntensity = m_flashlight.intensity;
    }

    #endregion

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
        //First check if you're looking at something interactable before interacting
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1f, m_interactableLayer))
        {
            //hit.transform.GetComponent<Interactable>().Interact();
        }
    }

    private void Pause(InputAction.CallbackContext context)
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            print("Pausing");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            print("Unpausing");
        }
    }

    private void Click(InputAction.CallbackContext context)
    {
        m_flashlightOn = !m_flashlightOn;
        if (m_flashlightOn)
        {
            m_flashlight.intensity = m_flashlightIntensity;
            StartCoroutine(Flashlight());
        }
        else
        {
            m_flashlight.intensity = 0;
        }
    }

    #endregion

    #region Movement

    private void Moving()
    {
        //To make sure the player moves depending on where they're looking I use transform.forward and transform.right
        Vector2 direction = m_inputs.Default.Walking.ReadValue<Vector2>();
        transform.position += transform.forward * direction.y * m_walkingSpeed * Time.deltaTime;
        transform.position += transform.right * direction.x * m_walkingSpeed * Time.deltaTime;
    }

    private void Rotate()
    {
        //Rotate the player and the camera based on mouse movement
        Vector2 direction = m_inputs.Default.Mouse.ReadValue<Vector2>();
        transform.Rotate(Vector3.up * direction.x * m_rotatingSpeed * Time.deltaTime);
        m_camera.transform.Rotate(Vector3.right * -direction.y * m_rotatingSpeed * Time.deltaTime);
        if (m_camera.transform.localRotation.x > 0.9f || m_camera.transform.localRotation.x < -0.9f)
        {
            m_camera.transform.Rotate(Vector3.right * direction.y * m_rotatingSpeed * Time.deltaTime);
            return;
        }
        m_rotationList.Add(m_camera.transform.rotation);
        FlashRotate();
    }

    void FixedUpdate()
    {
        Moving();
        Rotate();
    }

    #endregion

    #region Flashlight

    private void FlashRotate()
    {
        //Make the flashlight rotate with the camera but on a delay
        if (m_rotationList.Count >= m_frameDelay)
        {
            m_rotationList.RemoveAt(0);
        }

        if (m_rotationList.Count > 1)
        {
            m_flashlightPivot.transform.rotation = m_rotationList[0];
        }
    }

    private IEnumerator Flashlight()
    {
        //Coroutine to drain the flashlight
        while (m_flashlightOn)
        {
            m_flashlightPower -= m_flashlightDrainRate * Time.deltaTime;
            if (m_flashlightPower <= 0)
            {
                m_flashlightPower = 0;
                m_flashlightOn = false;
            }
        }
        yield return null;
    }

    private IEnumerator ReloadFlashlight()
    {
        while (m_flashlightPower < m_maxFlashlightPower)
        {
            m_flashlightPower += m_maxFlashlightPower / m_flashlightReloadRate * Time.deltaTime;
            yield return null;
        }
    }

    #endregion

    #region Inventory

    public void UpdateInventory(string item)
    {
        switch (item)
        {
            case "key 1":
                m_hasKey1 = true;
                m_key1Slot.SetActive(true);
                break;
            case "key 2":
                m_hasKey2 = true;
                m_key2Slot.SetActive(true);
                break;
            case "axe":
                m_hasAxe = true;
                m_axeSlot.SetActive(true);
                break;
        }
    }

    public bool CheckInventory(string item)
    {
        switch (item)
        {
            case "key 1":
                return m_hasKey1;
            case "key 2":
                return m_hasKey2;
            case "axe":
                return m_hasAxe;
            default:
                return false;
        }
    }

    #endregion

}
