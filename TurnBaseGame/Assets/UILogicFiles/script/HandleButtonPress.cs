using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HandleButtonPress : MonoBehaviour
{

    public Button basicAction;
    public Button skillAction;
    public Button burstAction;

    [Header("Fight Data")]
    private FightDataManager fdm;
    private GameObject Player;
    public WeaponInventoryEntry currentWeapon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = transform.parent.gameObject;
        fdm = FindFirstObjectByType<FightDataManager>();
        currentWeapon = fdm.FetchMyWeapons(fdm.FetchMyPlayersData(Player))[0];

    }

    // Update is called once per frame
    void Update()
    {
        CheckkeyboardPress();
    }

    public void OnbasicActionPressed()
    {
        int button = basicAction.GetComponent<HoverScrollButton>().ScrollIndex;
    }
    public void OnskillActionPressed()
    {
        int button = skillAction.GetComponent<HoverScrollButton>().ScrollIndex;
    }
    public void OnburstActionPressed()
    {
        int button = burstAction.GetComponent<HoverScrollButton>().ScrollIndex;
    }
    private void CheckkeyboardPress()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            currentWeapon = fdm.GiveActiveWeapon(-1, fdm.FetchMyPlayersData(Player), currentWeapon);
        }
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentWeapon = fdm.GiveActiveWeapon(1, fdm.FetchMyPlayersData(Player), currentWeapon);
        }
    }
}
