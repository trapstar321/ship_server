using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static ServerSend send;
    public static NetworkManager instance;    
    public static Waves wavesScript;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject projectilePrefab;
    public static float visibilityRadius=20;

    private void Awake()
    {
        send = FindObjectOfType<ServerSend>();
        StartCoroutine(Tick());
        wavesScript = GameObject.FindWithTag("Waves").GetComponent<Waves>();        

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Update()
    {
        
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(50, 26950);
    }

    private void OnApplicationQuit()
    {        
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(-6.83f, 0.2f, -27.9f), Quaternion.identity).GetComponent<Player>();
    }

    public static void SendHealthStats(int from) {
        //send player stats to all
        ServerSend.HealthStats(from);

        //send all player stats to player
        foreach (Client client in Server.clients.Values) {
            if(client.player!=null && client.id!=from)
                ServerSend.HealthStats(client.id, from);
        }
    }

    IEnumerator Tick()
    {
        while (true)
        {
            foreach (Client client in Server.clients.Values)
            {
                if (client.player != null)
                {
                    int lastInputSequenceNumber = 0;
                    PlayerInputs lastInput = null;
                    //Debug.Log("To process: "+client.inputBuffer.Count);
                    
                    int end = client.inputBuffer.Count;
                    for (int i=0; i<end; i++)
                    {
                        Debug.Log("SN "+ client.inputBuffer[i].inputSequenceNumber);
                        PlayerInputs input = client.inputBuffer[i];
                        lastInput = input;
                        lastInputSequenceNumber = input.inputSequenceNumber;
                        client.player.Move(new Vector3(input.left ? 1 : 0, input.right ? 1 : 0, input.forward ? 1 : 0));
                        //client.inputBuffer.RemoveAt(i);                        
                    }
                    if(end!=0)
                        client.inputBuffer.RemoveRange(0, end);
                    
                    if(lastInputSequenceNumber!=0)
                    {
                        client.lastInputSequenceNumber = lastInputSequenceNumber;
                        send.PlayerPosition(lastInput, client.lastInputSequenceNumber, client.player, visibilityRadius);
                    }
                    //Debug.Log("LSN:" + client.lastInputSequenceNumber);
                }
            }
            yield return new WaitForSeconds(1/50);
        }        
    }
}
