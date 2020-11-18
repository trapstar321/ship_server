using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomLoot : MonoBehaviour
{
    public List<Item> lootItems;
    public int total;
    public int maxLootItemCount = 5;
    public int randomNumber;

    private void Awake()
    {
        Mysql mysql = FindObjectOfType<Mysql>();
        lootItems = mysql.ReadItems();
        lootItems.Sort((a, b) => a.CompareTo(b));
    }

    void Start()
    {
        foreach (Item x in lootItems)
        {
            total += x.dropChance;
        }
    }

    public int RandomQuantity(Item item) {
        if (item.maxLootQuantity != 0)
            return (int)Random.Range(1, item.maxLootQuantity);
        return 1;
    }

    public List<ItemDrop> GenerateLoot()
    {
        List<ItemDrop> result = new List<ItemDrop>();        

        var itemCount = Random.Range(2, maxLootItemCount);
        List<Item> loot = new List<Item>(lootItems);

        bool generated = false;
        for (int i = 0; i < itemCount; i++)
        {
            generated = false;
            while (!generated)
            {
                foreach (Item item in loot)
                {
                    randomNumber = Random.Range(0, total);
                    if (randomNumber < item.dropChance)
                    {
                        if (result.Any(it => it.item.item_id == item.item_id))
                        {
                            generated = false;
                            break;
                        }
                        else
                        {
                            generated = true;

                            ItemDrop drop = new ItemDrop();
                            drop.item = item;
                            drop.quantity = RandomQuantity(item);

                            result.Add(drop);
                            loot.Remove(item);
                            total -= item.dropChance;
                            break;
                        }
                    }
                    else
                    {
                        randomNumber -= item.dropChance;
                    }
                }
            }
        }

        return result;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
