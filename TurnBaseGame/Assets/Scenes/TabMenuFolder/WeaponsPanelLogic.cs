using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponsPanelLogic : MonoBehaviour
{
    public Button[] weaponSlots;       // UI buttons for equipped weapons
    public Button[] weaponCircle;      // Circular selectable weapons
    public Button leftArrow;
    public Button rightArrow;
    public GameObject infoPanel;

    public Transform weaponSlotCenter;   // center point for weaponSlots
    public Transform weaponCircleCenter; // center point for weaponCircle

    private int currentSlotIndex = 0;

    public float slotRadius;      
    public float circleRadius;    
    private float circleAngle = 0f;

    public float rotationSpeed;   

    private float currentRotationAngle = 0f;  // current rotation offset
    private float targetRotationAngle = 0f;   // desired rotation offset

    private int weaponCircleIndex = 0;
    private float currentCircleRotation = 0f;
    private float targetCircleRotation = 0f;
    public float circleRotationSpeed = 360f; // degrees per second

    void Awake()
    {
        ArrangeWeaponSlots();       // set initial positions evenly spaced
        UpdateWeaponSlotSelection();
        UpdateWeaponCirclePositions();
    }

    void Update()
    {
        HandleScrollWheel();
        SmoothRotateSlots();
        AnimateWeaponCircle();
    }


    void ArrangeWeaponSlots()
    {
        // Initially arrange slots evenly on the circle with zero rotation offset
        float angleStep = 360f / weaponSlots.Length;

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            float angle = angleStep * i + currentRotationAngle;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(radians),
                Mathf.Sin(radians),
                0f
            ) * slotRadius;

            weaponSlots[i].transform.localPosition = weaponSlotCenter.localPosition + offset;
        }
    }

    void SmoothRotateSlots()
    {
        // Smoothly move currentRotationAngle toward targetRotationAngle
        if (Mathf.Approximately(currentRotationAngle, targetRotationAngle))
            return;

        currentRotationAngle = Mathf.MoveTowardsAngle(currentRotationAngle, targetRotationAngle, rotationSpeed * Time.deltaTime);

        float angleStep = 360f / weaponSlots.Length;

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            float angle = angleStep * i + currentRotationAngle;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(radians),
                Mathf.Sin(radians),
                0f
            ) * slotRadius;

            weaponSlots[i].transform.localPosition = weaponSlotCenter.localPosition + offset;
        }
    }

    void HandleScrollWheel()
    {
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                if (scroll > 0)
                    weaponCircleIndex = (weaponCircleIndex + 1) % weaponCircle.Length;
                else
                    weaponCircleIndex = (weaponCircleIndex - 1 + weaponCircle.Length) % weaponCircle.Length;

                // Set the target rotation based on the index (negative to rotate clockwise)
                float angleStep = 360f / weaponCircle.Length;
                targetCircleRotation = weaponCircleIndex * angleStep;
            }
        }
    }
    void AnimateWeaponCircle()
    {
        if (Mathf.Approximately(currentCircleRotation, targetCircleRotation))
            return;

        currentCircleRotation = Mathf.MoveTowardsAngle(currentCircleRotation, targetCircleRotation, circleRotationSpeed * Time.deltaTime);
        UpdateWeaponCirclePositions();
    }

    void UpdateWeaponCirclePositions()
    {
        float angleStep = 360f / weaponCircle.Length;

        for (int i = 0; i < weaponCircle.Length; i++)
        {
            // Apply smooth current rotation, offset each button by index
            float angle = currentCircleRotation + angleStep * i;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(radians),
                Mathf.Sin(radians),
                0f
            ) * circleRadius;

            weaponCircle[i].transform.localPosition = weaponCircleCenter.localPosition + offset;
        }
    }
    public void OnCircleWeaponClicked(GameObject clicked)
    {
        //weaponSlots[currentSlotIndex].GetComponentInChildren<TextMeshPro>().text = clicked.GetComponentInChildren<TextMeshPro>().text;
    }

    public void OnLeftArrowClicked()
    {
        currentSlotIndex = (currentSlotIndex - 1 + weaponSlots.Length) % weaponSlots.Length;

        // Rotate the circle smoothly by one slot clockwise
        targetRotationAngle += 360f / weaponSlots.Length;

        UpdateWeaponSlotSelection();
    }

    public void OnRightArrowClicked()
    {
        currentSlotIndex = (currentSlotIndex + 1) % weaponSlots.Length;

        // Rotate the circle smoothly by one slot counter-clockwise
        targetRotationAngle -= 360f / weaponSlots.Length;

        UpdateWeaponSlotSelection();
    }

    void UpdateWeaponSlotSelection()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            ColorBlock colors = weaponSlots[i].colors;
            colors.normalColor = (i == currentSlotIndex) ? Color.yellow : Color.white;
            weaponSlots[i].colors = colors;
        }
    }
}
