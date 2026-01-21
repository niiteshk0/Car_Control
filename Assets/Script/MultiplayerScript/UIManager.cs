using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class UIManager : MonoBehaviour
{
    [SerializeField] Button startServerButton;
    [SerializeField] Button startHostButton;
    [SerializeField] Button startClientButton;
    // TextMeshProUGUI playersInGameText;

    private void Awake() {
        startServerButton.onClick.AddListener(()=>{
            NetworkManager.Singleton.StartServer();
        });

        startHostButton.onClick.AddListener(()=>{
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host Started");
        });

        startClientButton.onClick.AddListener(()=>{
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client Started");
        });
    }
}
