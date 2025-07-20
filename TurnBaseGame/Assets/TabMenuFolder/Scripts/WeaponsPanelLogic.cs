using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class WeaponsPanelLogic : MonoBehaviour
{
    public Button[] weaponSlots;       // UI buttons for equipped weapons
    public GameObject weaponCircleButtonPrefab; 
    private List<Button> weaponCircle = new List<Button>();

    public Button leftArrow;
    public Button rightArrow;
    public GameObject infoPanel;

    public Transform weaponSlotCenter;   // center point for weaponSlots
    public Transform weaponCircleCenter; // center point for weaponCircle

    private int currentSlotIndex = 0;

    public float slotRadius;      
    public float circleRadius;    
    public float rotationSpeed;   

    private float currentRotationAngle = 0f;  // current rotation offset
    private float targetRotationAngle = 0f;   // desired rotation offset

    private int weaponCircleIndex = 0;
    private float currentCircleRotation = 0f;
    private float targetCircleRotation = 0f;
    public float circleRotationSpeed = 360f; // degrees per second

    private float defaultCircleRadius;
    public float minCircleRadius = 0.1f;
    public float radiusLerpSpeed = 5f;
    private float circleRadiusVelocity;


    private bool isHoveringWeapon = false;
    public Vector3 infoPanelOnScreenPosition;
    public Vector3 infoPanelOffScreenPosition;
    public float infoPanelSlideSpeed = 5f;

    private bool lastClickedWasWeapon = false;
    private float timeSinceClick = 0f;
    public float infoPanelVisibleDuration = 10f;

    private PlayerRuntime playerRuntime;
    public List<WeaponInventoryEntry> weapons;

    void Awake()
    {
        playerRuntime = GetComponentInParent<PlayerRuntime>();
        if (playerRuntime == null)
            Debug.LogError("PlayerRuntime not found in parent!");

        weapons = playerRuntime.data.Weapons;
        defaultCircleRadius = circleRadius;

        ArrangeWeaponSlots();
        PopulateWeaponCircle();
    }

    void Update()
    {
        UpdateCircleRadius();
        HandleScrollWheel();
        SmoothRotateSlots();
        AnimateWeaponCircle();
        UpdateInfoPanelPosition();

        if (lastClickedWasWeapon)
        {
            timeSinceClick += Time.deltaTime;
            if (timeSinceClick > infoPanelVisibleDuration)
            {
                lastClickedWasWeapon = false;
                timeSinceClick = 0f;
            }
        }

    }

    public void PopulateWeaponCircle()
    {
        foreach (Transform child in weaponCircleCenter)
            Destroy(child.gameObject); // Clear old buttons

        weaponCircle.Clear();

        for (int i = 0; i < weapons.Count; i++)
        {
            WeaponInventoryEntry weapon = weapons[i];

            GameObject buttonObj = Instantiate(weaponCircleButtonPrefab, weaponCircleCenter);
            buttonObj.name = "WeaponButton_" + weapon.weaponID;

            Button btn = buttonObj.GetComponent<Button>();
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = weapon.weaponID;

            btn.onClick.AddListener(() => OnCircleWeaponClicked(buttonObj));
            btn.gameObject.active=true;
            weaponCircle.Add(btn);
        }

        UpdateWeaponCirclePositions();
    }

    void OnWeaponCircleClicked(Button clicked)
    {
        string weaponName = clicked.GetComponentInChildren<TextMeshProUGUI>().text;
        weaponSlots[currentSlotIndex].GetComponentInChildren<TextMeshProUGUI>().text = weaponName;

        lastClickedWasWeapon = true;
        timeSinceClick = 0f;

    }

    void UpdateInfoPanelPosition()
    {
        Vector3 targetPosition = lastClickedWasWeapon ? infoPanelOnScreenPosition : infoPanelOffScreenPosition;
        infoPanel.transform.localPosition = Vector3.Lerp(infoPanel.transform.localPosition, targetPosition, Time.deltaTime * infoPanelSlideSpeed);
    }


    bool IsRotating() => !Mathf.Approximately(currentRotationAngle, targetRotationAngle);
    public void ArrangeWeaponSlots()
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
                    weaponCircleIndex = (weaponCircleIndex + 1) % weaponCircle.Count;
                else
                    weaponCircleIndex = (weaponCircleIndex - 1 + weaponCircle.Count) % weaponCircle.Count;

                // Set the target rotation based on the index (negative to rotate clockwise)
                float angleStep = 360f / weaponCircle.Count;
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
        float angleStep = 360f / weaponCircle.Count;
        bool rotating = IsRotating();

        // Set target opacity values
        float targetAlpha = rotating ? 0f : 1f;

        for (int i = 0; i < weaponCircle.Count; i++)
        {
            float angle = currentCircleRotation + angleStep * i;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(radians),
                Mathf.Sin(radians),
                0f
            ) * circleRadius;

            weaponCircle[i].transform.localPosition = weaponCircleCenter.localPosition + offset;

            // Smoothly adjust opacity
            Image img = weaponCircle[i].GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                float currentAlpha = c.a;
                c.a = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * 10f);
                img.color = c;
            }

            // Optionally update text alpha as well
            TextMeshProUGUI text = weaponCircle[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                Color tc = text.color;
                float currentAlpha = tc.a;
                tc.a = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * 10f);
                text.color = tc;
            }
        }
    }

    void UpdateCircleRadius()
    {
        float targetRadius = IsRotating() ? minCircleRadius : defaultCircleRadius;

        circleRadius = Mathf.SmoothDamp(circleRadius, targetRadius, ref circleRadiusVelocity, 0.1f, Mathf.Infinity, Time.deltaTime);
        UpdateWeaponCirclePositions();
    }

    public void OnCircleWeaponClicked(GameObject clicked)
    {
        string currentWeaponName = weaponSlots[currentSlotIndex].GetComponentInChildren<TextMeshProUGUI>().text;
        string CNameWithoutPrefix = currentWeaponName.Replace("WeaponButton_", "");
        WeaponInventoryEntry CWeapon = weapons.FirstOrDefault(w => w.weaponID == CNameWithoutPrefix);
        if(CWeapon!=null) CWeapon.isEquipped = false;

        string weaponName = clicked.GetComponentInChildren<TextMeshProUGUI>().text;
        weaponSlots[currentSlotIndex].GetComponentInChildren<TextMeshProUGUI>().text = weaponName;

        string nameWithoutPrefix = weaponName.Replace("WeaponButton_", "");
        WeaponInventoryEntry weapon = weapons.FirstOrDefault(w => w.weaponID == nameWithoutPrefix);
        weapon.isEquipped=true;
    }


    public void OnLeftArrowClicked()
    {
        if (IsRotating()) return;

        currentSlotIndex = (currentSlotIndex - 1 + weaponSlots.Length) % weaponSlots.Length;
        targetRotationAngle += 360f / weaponSlots.Length;
    }

    public void OnRightArrowClicked()
    {
        if (IsRotating()) return;

        currentSlotIndex = (currentSlotIndex + 1) % weaponSlots.Length;
        targetRotationAngle -= 360f / weaponSlots.Length;
    }
}
