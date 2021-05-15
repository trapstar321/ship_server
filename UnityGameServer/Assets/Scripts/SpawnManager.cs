using SerializableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public class Spawn {
        public int id;
        public GameObjectType type;
        public ObjectType objectType;
        public GameObject gameObject;
    }

    private static int _id=0;

    private static int NextId() {
        _id += 1;
        return _id;
    }
    
    private Dictionary<GameObjectType, GameObject> prefabs = new Dictionary<GameObjectType, GameObject>();
    public Dictionary<int, Spawn> objects = new Dictionary<int, Spawn>();
    private Mysql mysql;

    public NavMeshSurface land;

    private void Awake()
    {
        mysql = FindObjectOfType<Mysql>();
        prefabs.Add(GameObjectType.chest, Resources.Load("Prefabs/Chest", typeof(GameObject)) as GameObject);
        prefabs.Add(GameObjectType.npcShip, Resources.Load("Prefabs/ServerAI", typeof(GameObject)) as GameObject);
        prefabs.Add(GameObjectType.palmTree, Resources.Load("Prefabs/PalmTree", typeof(GameObject)) as GameObject);
        prefabs.Add(GameObjectType.goldRock, Resources.Load("Prefabs/GoldRock", typeof(GameObject)) as GameObject);
        prefabs.Add(GameObjectType.ironRock, Resources.Load("Prefabs/IronRock", typeof(GameObject)) as GameObject);
        prefabs.Add(GameObjectType.coal, Resources.Load("Prefabs/Coal", typeof(GameObject)) as GameObject);
        prefabs.Add(GameObjectType.silverRock, Resources.Load("Prefabs/SilverRock", typeof(GameObject)) as GameObject);
        prefabs.Add(GameObjectType.tinRock, Resources.Load("Prefabs/TinRock", typeof(GameObject)) as GameObject);
        prefabs.Add(GameObjectType.traderGeneric, Resources.Load("Prefabs/TraderGeneric", typeof(GameObject)) as GameObject);
        prefabs.Add(GameObjectType.tradeBroker, Resources.Load("Prefabs/TradeBroker", typeof(GameObject)) as GameObject);
        prefabs.Add(GameObjectType.cookingSpot, Resources.Load("Prefabs/CookingSpot", typeof(GameObject)) as GameObject);
        prefabs.Add(GameObjectType.dragon, Resources.Load("Prefabs/DragonNPC", typeof(GameObject)) as GameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        List<ResourceSpawn> resourceSpawns = mysql.ReadResourceSpawns();
        List<SerializableObjects.Trader> traders = mysql.ReadTraders();
        List<TradeBroker> brokers = mysql.ReadTradeBrokers();
        List<CraftingSpotSpawn> craftingSpots = mysql.ReadCraftingSpots();
        List<NPCSpawn> npcSpots = mysql.ReadNPCSpawns();

        //create Chest prefab
        int id = NextId();
        GameObject chest = Instantiate(prefabs[GameObjectType.chest], new Vector3(-3.57f, -2f, -8.83f), Quaternion.identity);
        objects.Add(id, new Spawn() { id = id, type = GameObjectType.chest, gameObject = chest, objectType=ObjectType.CHEST });

        foreach (ResourceSpawn spawn in resourceSpawns) {
            id = NextId();
            GameObject go = Instantiate(prefabs[(GameObjectType)spawn.RESOURCE.RESOURCE_TYPE], new Vector3(spawn.X, spawn.Y, spawn.Z), Quaternion.identity);
            objects.Add(id, new Spawn() { id = id, type = (GameObjectType)spawn.RESOURCE.RESOURCE_TYPE, gameObject = go, objectType=ObjectType.RESOURCE });

            Item item = mysql.ReadItem(spawn.RESOURCE.ITEM_ID);
            
            Resource resource = go.GetComponent<Resource>();
            resource.maxHp = spawn.RESOURCE.RESOURCE_HP;
            resource.resourceCount = spawn.RESOURCE.RESOURCE_COUNT;
            resource.respawnTime = spawn.RESPAWN_TIME;
            resource.itemId = spawn.RESOURCE.ITEM_ID;
            resource.skill_type = (SkillType)spawn.RESOURCE.SKILL_TYPE;
            resource.experience = spawn.RESOURCE.EXPERIENCE;
            resource.item = item;

            resource.Initialize();
        }

        foreach (SerializableObjects.Trader trader in traders) {
            id = NextId();
            GameObject go = Instantiate(prefabs[(GameObjectType)trader.game_object_type], new Vector3(trader.x, trader.y, trader.z), Quaternion.identity);
            go.transform.eulerAngles = new Vector3(0, trader.y_rot, 0);
            objects.Add(id, new Spawn() { id = id, type = (GameObjectType)trader.game_object_type, gameObject = go, objectType=ObjectType.TRADER });

            Trader traderScript = go.GetComponent<Trader>();
            traderScript.id = trader.id;
        }

        foreach (TradeBroker broker in brokers)
        {
            id = NextId();
            GameObject go = Instantiate(prefabs[(GameObjectType)broker.game_object_type], new Vector3(broker.x, broker.y, broker.z), Quaternion.identity);
            go.transform.eulerAngles = new Vector3(0, broker.y_rot, 0);
            objects.Add(id, new Spawn() { id = id, type = (GameObjectType)broker.game_object_type, gameObject = go, objectType = ObjectType.TRADE_BROKER });            
        }

        foreach (CraftingSpotSpawn craftingSpot in craftingSpots)
        {
            id = NextId();
            GameObject go = Instantiate(prefabs[(GameObjectType)craftingSpot.gameObjectType], new Vector3(craftingSpot.x, craftingSpot.y, craftingSpot.z), Quaternion.identity);
            go.transform.eulerAngles = new Vector3(0, craftingSpot.Y_rot, 0);
            objects.Add(id, new Spawn() { id = id, type = (GameObjectType)craftingSpot.gameObjectType, gameObject = go, objectType = ObjectType.CRAFTING_SPOT });

            CraftingSpot craftingSpotScript = go.GetComponent<CraftingSpot>();
            craftingSpotScript.skillType = craftingSpot.skillType;
        }

        land.BuildNavMesh();

        /*id = NextId();
        GameObject palmTree = Instantiate(prefabs[GameObjectType.palmTree], new Vector3(32.62526f, 0.8707776f, 45.96268f), Quaternion.identity);
        objects.Add(id, new Spawn() { id = id, type = GameObjectType.palmTree, gameObject = palmTree });

        id = NextId();
        GameObject ironRock = Instantiate(prefabs[GameObjectType.ironRock], new Vector3(28.95738f, 0.64f, 48.02f), Quaternion.identity);
        objects.Add(id, new Spawn() { id = id, type = GameObjectType.ironRock, gameObject = ironRock });

        id = NextId();
        GameObject goldRock = Instantiate(prefabs[GameObjectType.goldRock], new Vector3(28.16f, 0.75f, 50.88757f), Quaternion.identity);
        objects.Add(id, new Spawn() { id = id, type = GameObjectType.goldRock, gameObject = goldRock });*/
        /*
        id = NextId();
        GameObject ai = Instantiate(prefabs[GameObjectType.npcShip], new Vector3(0f, 0f, 0f), Quaternion.identity);
        objects.Add(id, new Spawn() { id = id, type = GameObjectType.npcShip, gameObject = ai });
        EnemyAI enemy = ai.GetComponent<EnemyAI>();
        enemy.id = id;
        Server.npcs.Add(id, enemy);*/


        foreach (NPCSpawn spawn in npcSpots)
        {
            if (spawn.enabled)
            {
                id = NextId();
                GameObject go = Instantiate(prefabs[(GameObjectType)spawn.gameObjectType], new Vector3(spawn.x, spawn.y, spawn.z), Quaternion.identity);
                go.transform.eulerAngles = new Vector3(0, spawn.Y_rot, 0);
                objects.Add(id, new Spawn() { id = id, type = GameObjectType.dragon, objectType = ObjectType.NPC, gameObject = go });
                NPC npc = go.GetComponent<NPC>();
                npc.id = id;
                npc.aggro_range = spawn.aggro_range;
                Server.npcs.Add(id, npc);
            }
        }
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
