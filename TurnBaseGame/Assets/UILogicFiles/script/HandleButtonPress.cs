using UnityEngine;
using UnityEngine.UI;

public class HandleButtonPress : MonoBehaviour
{

    public Button basicAction;
    public Button skillAction;
    public Button burstAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnbasicActionPressed()
    {
        int button = basicAction.GetComponent<HoverScrollButton>().scrollIndex;
    }
    public void OnskillActionPressed()
    {
        int button = skillAction.GetComponent<HoverScrollButton>().scrollIndex;
    }
    public void OnburstActionPressed()
    {
        int button = burstAction.GetComponent<HoverScrollButton>().scrollIndex;
    }
}
