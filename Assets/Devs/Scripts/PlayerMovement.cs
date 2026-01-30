using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("UI Elements")]
    [SerializeField] private GameObject m_pauseMenu;
    [SerializeField] private GameObject m_deathScreen;
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
    private Coroutine m_flashlightRoutine;
    private Coroutine m_flashlightReloadRoutine;

    [Header("Camera FOV")]
    [SerializeField] private int m_endFOV;

    private float m_startFOV;

    [Header("Mask")]
    [SerializeField] private GameObject m_maskObject;
    [SerializeField] private Animator m_maskAnimator;

    private bool m_maskOn;

    [Header("Scarecrow")]
    [SerializeField] private LayerMask m_scareCrowLayer;
    [SerializeField] private int m_timeTillDeath;

    private bool m_checkingForCrow;

    //Misc
    private Player m_inputs;
    private Rigidbody m_rb;

    #endregion

    #region Setup

    private void OnEnable()
    {
        //Setup inputs
        m_inputs = new Player();
        m_inputs.Enable();
        m_inputs.Default.Reload.started += StartReload;
        m_inputs.Default.Reload.canceled += StopReload;
        m_inputs.Default.Mask.performed += Mask;
        m_inputs.Default.Interact.performed += Interact;
        m_inputs.Default.Pause.performed += Pause;
        m_inputs.Default.Click.performed += Click;

        Time.timeScale = 1;
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
        m_flashlightOn = true;
        m_flashlightRoutine = StartCoroutine(Flashlight());
        m_rb = GetComponent<Rigidbody>();
        m_checkingForCrow = true;
        m_startFOV = m_camera.GetComponent<Camera>().fieldOfView;
    }

    #endregion

    #region Inputs

    private void StartReload(InputAction.CallbackContext context)
    {
        m_flashlightReloadRoutine = StartCoroutine(ReloadFlashlight());
        m_flashlight.GetComponent<Animator>().SetBool("Reloading", true);
        m_flashlightOn = false;
        m_flashlight.intensity = 0;

        m_inputs.Default.Walking.Disable();
        m_inputs.Default.Mask.Disable();
        m_inputs.Default.Click.Disable();

        if (m_flashlightRoutine != null) 
        {
            StopCoroutine(m_flashlightRoutine);
            m_flashlightRoutine = null;
        }
    }

    private void StopReload(InputAction.CallbackContext context)
    {
        if (m_flashlightReloadRoutine != null)
            StopCoroutine(m_flashlightReloadRoutine);
        m_flashlight.GetComponent<Animator>().SetBool("Reloading", false);

        m_inputs.Default.Walking.Enable();
        m_inputs.Default.Mask.Enable();
        m_inputs.Default.Click.Enable();
    }

    private void Mask(InputAction.CallbackContext context)
    {
        m_maskOn = !m_maskOn;
        if (m_maskOn)
        {
            m_maskObject.SetActive(true);
            m_inputs.Default.Walking.Disable();
            m_inputs.Default.Mouse.Disable();
            m_inputs.Default.Click.Disable();
            m_inputs.Default.Reload.Disable();
            m_maskAnimator.SetTrigger("Down");
        }
        else
        {
            m_inputs.Default.Walking.Enable();
            m_inputs.Default.Mouse.Enable();
            m_inputs.Default.Click.Enable();
            m_inputs.Default.Reload.Enable();
            m_maskAnimator.SetTrigger("Up");
            StartCoroutine(MaskAnimation());
        }
    }

    private IEnumerator MaskAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        m_maskObject.SetActive(false);
    }

    private void Interact(InputAction.CallbackContext context)
    {
        //First check if you're looking at something interactable before interacting
        if(Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, 3f, m_interactableLayer))
        {
            hit.transform.GetComponent<Interactable>().Interact(this);
        }
    }

    private void Pause(InputAction.CallbackContext context)
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            m_pauseMenu.SetActive(true);
            m_inputs.Default.Click.Disable();
        }
        else
        {
            UnPause();
        }
    }

    public void UnPause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        m_pauseMenu.SetActive(false);
        m_inputs.Default.Click.Enable();
    }

    private void Click(InputAction.CallbackContext context)
    {
        m_flashlightOn = !m_flashlightOn;
        if (m_flashlightOn)
        {
            m_flashlight.intensity = m_flashlightIntensity;
            m_flashlightRoutine = StartCoroutine(Flashlight());
        }
        else
        {
            m_flashlight.intensity = 0;
            StopCoroutine(m_flashlightRoutine);
        }
    }

    #endregion

    #region Movement

    private void Moving()
    {
        //To make sure the player moves depending on where they're looking I use transform.forward and transform.right
        Vector2 direction = m_inputs.Default.Walking.ReadValue<Vector2>();
        Vector3 moveDirection = (transform.forward * direction.y) + (transform.right * direction.x);

        if (direction == Vector2.zero)
            m_rb.linearVelocity = Vector3.zero;
        m_rb.linearVelocity = moveDirection * m_walkingSpeed;
    }

    private void Rotate()
    {
        //Rotate the player and the camera based on mouse movement
        Vector2 direction = m_inputs.Default.Mouse.ReadValue<Vector2>();
        transform.Rotate(Vector3.up * direction.x * m_rotatingSpeed * Time.deltaTime);
        m_camera.transform.Rotate(Vector3.right * -direction.y * m_rotatingSpeed * Time.deltaTime);
        if (m_camera.transform.localRotation.x > 0.7f || m_camera.transform.localRotation.x < -0.7f)
        {
            m_camera.transform.Rotate(Vector3.right * direction.y * m_rotatingSpeed * Time.deltaTime);
            return;
        }
        m_rotationList.Add(m_camera.transform.rotation);
        FlashRotate();
    }

    private void RaycastCheck()
    {
        if (!m_checkingForCrow) return;
        bool hitLeft = Physics.Raycast(transform.position + Vector3.up, transform.forward - transform.right, out RaycastHit leftHit, Mathf.Infinity);
        bool hitRight = Physics.Raycast(transform.position + Vector3.up, transform.forward + transform.right, out RaycastHit rightHit, Mathf.Infinity);
        bool hitStraight = Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit straightHit, Mathf.Infinity);
        if (hitLeft || hitRight || hitStraight)
        {
            print("Hit something");
            if (hitLeft && leftHit.transform.CompareTag("Scarecrow"))
            {
                transform.LookAt(leftHit.transform);
                m_camera.transform.LookAt(leftHit.transform.GetChild(0));
            }
            else if (hitStraight && straightHit.transform.CompareTag("Scarecrow"))
            {
                transform.LookAt(straightHit.transform);
                m_camera.transform.LookAt(straightHit.transform.GetChild(0));
            }
            else if (hitRight && rightHit.transform.CompareTag("Scarecrow"))
            {
                transform.LookAt(rightHit.transform);
                m_camera.transform.LookAt(rightHit.transform.GetChild(0));
            }
            else return;
            m_inputs.Default.Walking.Disable();
            m_inputs.Default.Mouse.Disable();
            m_inputs.Default.Click.Disable();
            m_inputs.Default.Reload.Disable();
            m_flashlight.intensity = 0;
            StopCoroutine(m_flashlightRoutine);
            m_checkingForCrow = false;
            StartCoroutine(SeenCrow());
            StartCoroutine(CameraZoom());
            Gamemanager.Instance.FocusOnPlayer(m_timeTillDeath);
        }
    }

    private IEnumerator SeenCrow()
    {
        yield return new WaitForSeconds(m_timeTillDeath);
        if (m_maskOn)
        {
            m_inputs.Default.Walking.Enable();
            m_inputs.Default.Mouse.Enable();
            m_inputs.Default.Click.Enable();
            m_inputs.Default.Reload.Enable();
            Gamemanager.Instance.FindSpawnPoint(); 
            m_checkingForCrow = true;
            m_camera.GetComponent<Camera>().fieldOfView = m_startFOV;
        }
        else
        {
            Time.timeScale = 0;
            m_inputs.Default.Disable();
            m_deathScreen.SetActive(true);
        }
        yield return null;
    }

    private IEnumerator CameraZoom()
    {
        while (m_camera.GetComponent<Camera>().fieldOfView > m_endFOV)
        {
            m_camera.GetComponent<Camera>().fieldOfView -= 1;
            yield return new WaitForSeconds(m_timeTillDeath / m_endFOV);
        }
    }

    void FixedUpdate()
    {
        Moving();
        Rotate();
        RaycastCheck();
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
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
                m_flashlight.intensity = 0;
                m_flashlightRoutine = null;
            }
            yield return null;
        }
    }

    private IEnumerator ReloadFlashlight()
    {
        while (m_flashlightPower < m_maxFlashlightPower)
        {
            m_flashlightPower += m_maxFlashlightPower / m_flashlightReloadRate * Time.deltaTime;
            yield return null;
        }
        m_flashlight.GetComponent<Animator>().SetBool("Reloading", false);
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
