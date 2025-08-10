using UnityEngine;

public class CollapsiblePanel : MonoBehaviour
{
    [SerializeField] private GameObject content;

    public void TogglePanel()
    {
        content.SetActive(!content.activeSelf);
    }
}