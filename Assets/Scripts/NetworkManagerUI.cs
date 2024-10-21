using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    //References to all the buttons
    [SerializeField] Button serverBtn;
    [SerializeField] Button clientBtn;
    [SerializeField] Button hostBtn;

    // Start is called before the first frame update
    void Start()
    {
        serverBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartServer(); });
        clientBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartClient(); });
        hostBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartHost(); });


    }

    // Update is called once per frame
    void Update()
    {

    }
}
