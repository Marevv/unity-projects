using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Transform spawnedObjectPrefab;


    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    private NetworkVariable<MyCustomData> randomstuff = new NetworkVariable<MyCustomData>(new MyCustomData
    {
        _int = 56,
        _bool = true,
        message = "Test msg"
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    public override void OnNetworkSpawn()
    {
        randomstuff.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
        };
    }

    void Update()
    {
        

        if (!IsOwner)
            return;


        if (Input.GetKeyDown(KeyCode.T))
        {
            
            Transform spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
            //TestClientRPC();
            //TestServerRPC();
            //randomNumber.Value = Random.Range(0, 100);

            //randomstuff.Value = new MyCustomData {
            //    _int = Random.Range(0, 100), 
            //    _bool = true,
            //    message = "It works!"

            //};
            
        }


        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) { moveDir.z = +1f; }
        if (Input.GetKey(KeyCode.S)) { moveDir.z = -1f; }
        if (Input.GetKey(KeyCode.A)) { moveDir.x = -1f; }
        if (Input.GetKey(KeyCode.D)) { moveDir.x = +1f; }

        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }


    //Kod wywoływany na Serwerze, sposób komunikacji klienta z serwerem
    [ServerRpc]
    private void TestServerRPC()
    {
        Debug.Log("Test Server RPC" + OwnerClientId);
    }


    //Kod wywoływany na Kliencie, może doprecyzować na którym, sposób komunikacji serwerem z klientem
    [ClientRpc]
    private void TestClientRPC()
    {
        Debug.Log("Test Client RPC");
    }
}
