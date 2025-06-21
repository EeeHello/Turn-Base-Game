using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject camPrefab;

    private void Start()
    {
        if (PlayerDataCarrier.Instance == null || PlayerDataCarrier.Instance.LoadedPlayerData == null)
        {
            Debug.LogError("No player data found to initialize.");
            return;
        }

        // Spawn the camera first
        GameObject camObj = Instantiate(camPrefab);
        Camera cam = camObj.GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("Camera prefab does not contain a Camera component!");
            return;
        }

        // Now spawn the player
        GameObject playerObj = Instantiate(playerPrefab,new Vector3(0,10,0), Quaternion.identity);

        // Assign the camera to the SideScrollerCamera script AFTER it's been instantiated
        SideScrollerCamera scCam = playerObj.GetComponent<SideScrollerCamera>();
        if (scCam != null)
        {
            scCam.cam = cam;
        }

        // Initialize the player with its data
        PlayerRuntime runtime = playerObj.GetComponent<PlayerRuntime>();
        if (runtime != null)
        {
            runtime.Initialize(PlayerDataCarrier.Instance.LoadedPlayerData);
        }
    }
}
