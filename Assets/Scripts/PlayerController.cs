using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 2.0f;
    NetworkVariable<float> randomNumber = new NetworkVariable<float>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct PlayerData : INetworkSerializable
    {
        public FixedString128Bytes playerName;
        public bool isPlayerAlive;
        public int health;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerName);
            serializer.SerializeValue(ref isPlayerAlive);
            serializer.SerializeValue(ref health);
        }
    }

    NetworkVariable<PlayerData> playerInfo = new NetworkVariable<PlayerData>(new PlayerData() { playerName="Potato", health=56, isPlayerAlive = true},NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    [SerializeField] GameObject spherePrefab;
    GameObject sphereObject;

    // Start is called before the first frame update
    void Start()
    {
        randomNumber.OnValueChanged += (float previousVal,float newVal) => { Debug.Log(OwnerClientId + "'s number is " + newVal); };
        playerInfo.OnValueChanged += (PlayerData previousVal, PlayerData newVal) => { Debug.Log(newVal.playerName + " is still alive: " + newVal.isPlayerAlive + " current player health: " + newVal.health); };
    }




    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        float horizontalInput = Input.GetAxis("Horizontal");//A & D movement
        float verticalInput = Input.GetAxis("Vertical");//W & S movement

        Vector3 movement= new Vector3 (horizontalInput,0.0f, verticalInput);

        transform.position += movement * Time.deltaTime * moveSpeed;

        if (Input.GetKeyDown(KeyCode.T))TestServerRpc();
        
        if(Input.GetKeyDown(KeyCode.Y)) TestClientRpc();

        if (Input.GetKeyDown(KeyCode.U)) randomNumber.Value = Random.Range(10, 100);

        if (Input.GetKeyDown(KeyCode.I)) playerInfo.Value = new PlayerData() { playerName = playerInfo.Value.playerName, isPlayerAlive = false, health = Random.Range(10, 100) };

        if (Input.GetKeyDown(KeyCode.O))
        {
            sphereObject=Instantiate(spherePrefab);
            sphereObject.GetComponent<NetworkObject>().Spawn(true);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            sphereObject.GetComponent<NetworkObject>().Despawn(true);
            Destroy(sphereObject);
        }
    }

    [ServerRpc]
    void TestServerRpc()
    {
        Debug.Log("This message is coming from " + OwnerClientId);
    }

    [ClientRpc]
    void TestClientRpc()
    {
        Debug.Log("This message is coming from " + OwnerClientId + " Client Rpc Function");

    }

    [Rpc(SendTo.Server)]
    void TestRpc()
    {
        Debug.Log("This message is coming from " + OwnerClientId);
    }
}
