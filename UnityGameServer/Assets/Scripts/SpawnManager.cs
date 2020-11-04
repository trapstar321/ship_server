using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public enum GameObjectType {
        chest
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
    }

    // Start is called before the first frame update
    void Start()
    {
        //create Chest prefab
        int id = NextId();
        GameObject chest = Instantiate(prefabs[GameObjectType.chest], new Vector3(-3.57f, -2f, -8.83f), Quaternion.identity);        
        objects.Add(id, new Spawn() { id=id, type=GameObjectType.chest, gameObject = chest });
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
