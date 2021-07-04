using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomLoot
{
    public long id;
    public long dbid;
    private static long counter = 0;
    public List<SerializableObjects.Item> lootItems;
    public List<ItemDrop> droppedItems;
    public int total;
    public int maxLootItemCount = 5;
    public int randomNumber;
    public float damagePercentage;
    public System.DateTime generatedTime;
    public Vector3 position;
    public int remainingTime;

    public RandomLoot(List<ItemDrop> droppedItems, int remainingTime, Vector3 position) {
        counter += 1;
        id = counter;

        this.droppedItems = droppedItems;
        this.position = position;
        this.remainingTime = remainingTime;
    }

    public RandomLoot(int npc_type, float damage, int max_loot_count) {
        counter += 1;
        id = counter;
        maxLootItemCount = max_loot_count;
        damagePercentage = damage;

        lootItems = NetworkManager.instance.mysql.GetNPCLoot(npc_type, damage);
        lootItems.Sort((a, b) => a.CompareTo(b));

        foreach (SerializableObjects.Item x in lootItems)
        {
            total += x.dropChance;
        }
    }

    public int RandomQuantity(SerializableObjects.Item item) {
        if (item.maxLootQuantity != 0)
            return (int)Random.Range(1, item.maxLootQuantity);
        return 1;
    }

    public void GenerateLoot()
    {
        List<ItemDrop> result = new List<ItemDrop>();        

        var itemCount = Random.Range(1, GetMaxLootCount(damagePercentage));
        List<SerializableObjects.Item> loot = new List<SerializableObjects.Item>(lootItems);

        bool generated = false;
        for (int i = 0; i < itemCount; i++)
        {
            generated = false;
            while (!generated)
            {
                foreach (SerializableObjects.Item item in loot)
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

        droppedItems = result;
        generatedTime = System.DateTime.UtcNow;
    }

    protected int GetMaxLootCount(float damagePercentage) {
        if (damagePercentage >= 50) {
            return maxLootItemCount;
        } else if (damagePercentage <= 50 && damagePercentage >= 30) {
            return Round(maxLootItemCount * 0.75f);
        } else if (damagePercentage <= 30 && damagePercentage >= 10) {
            return Round(maxLootItemCount * 0.5f);
        } else if (damagePercentage <= 10) {
            return 1;
        }

        return maxLootItemCount;
    }

    protected int Round(float result)
    {
        var value = result - System.Math.Truncate(result);

        if (value >= 0.5f)
            return (int)result + 1;
        else
            return (int)result;
    }
}
