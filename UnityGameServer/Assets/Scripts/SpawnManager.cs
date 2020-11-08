using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public enum GameObjectType {
        chest,
        npcShip
    }

    public class Spawn {
        public int id;
        public GameObjectType type;
        public GameObject gameObject;
    }

    private static int _id=0;

    private static int NextId() {
        _id += 1;
        return _id;
    }
    
    private Dictionary<GameObjectType, GameObject> prefabs = new Dictionary<GameObjectType, GameObject>();
    public Dictionary<int, Spawn> objects = new Dictionary<int, Spawn>();

    private void Awake()
    {
        prefabs.Add(GameObjectType.chest, Resources.Load("Prefabs/Chest", typeof(GameObject)) as GameObject);
        prefabs.Add(GameObjectType.npcShip, Resources.Load("Prefabs/ServerAI", typeof(GameObject)) as GameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //create Chest prefab
        int id = NextId();
        GameObject chest = Instantiate(prefabs[GameObjectType.chest], new Vector3(-3.57f, -2f, -8.83f), Quaternion.identity);
        objects.Add(id, new Spawn() { id = id, type = GameObjectType.chest, gameObject = chest });

        id = NextId();
        GameObject ai = Instantiate(prefabs[GameObjectType.npcShip], new Vector3(0f, 0f, 0f), Quaternion.identity);
        objects.Add(id, new Spawn() { id = id, type = GameObjectType.npcShip, gameObject = ai });
        EnemyAI enemy = ai.GetComponent<EnemyAI>();
        enemy.id = id;
        Server.npcs.Add(id, enemy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendAllGameObjects(int to) {
        foreach (Spawn spawn in objects.Values) {
            ServerSend.SpawnGameObject(to, spawn);
        }
    }
}
