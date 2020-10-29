using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public static Waves wavesScript;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject projectilePrefab;
    public static float visibilityRadius=200;

    float lastPositionUpdateTime=-1;
    float positionUpdateDifference = 5;

    Mysql mysql;

    private void Awake()
    {
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

        mysql = FindObjectOfType<Mysql>();
    }

    private void Update()
    {
        ServerSend.Time(Time.deltaTime);
        UpdatePlayerPosition();
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

    public Player InstantiatePlayer(float x, float z)
    {
        return Instantiate(playerPrefab, new Vector3(-6.83f, 0.2f, -27.9f), Quaternion.identity).GetComponent<Player>();
    }

    public void InstantiateEnemy(Vector3 _position)
    {
        Instantiate(enemyPrefab, _position, Quaternion.identity);
    }

    public Projectile InstantiateProjectile(Transform _shootOrigin)
    {
        return Instantiate(projectilePrefab, _shootOrigin.position + _shootOrigin.forward * 0.7f, Quaternion.identity).GetComponent<Projectile>();
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

    void UpdatePlayerPosition() {
        if (Time.time - lastPositionUpdateTime < positionUpdateDifference && lastPositionUpdateTime != -1)
            return;

        lastPositionUpdateTime = Time.time;

        foreach (Client client in Server.clients.Values) {
            Player player = client.player;

            if (player != null) {
                mysql.UpdatePlayerPosition(player.id, player.transform.position.x, player.transform.position.z);
            }
        }
    }
}
