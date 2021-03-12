using SerializableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Vector3 = UnityEngine.Vector3;

public class Crafting: MonoBehaviour
{
    private int recipeId;
    public  int from;
    private Inventory inventory;
    private List<RecipeItemRequirement> itemsNeeded;
    private Recipe recipe;
    private Item craftingItem;
    private Mysql mysql;

    public void Initialize(int from, int recipeId, Mysql mysql) {
        this.recipeId = recipeId;
        this.from = from;
        this.mysql = mysql;        

        itemsNeeded = null;

        foreach (Recipe recipe in NetworkManager.recipes)
        {
            if (recipe.id == recipeId)
            {
                itemsNeeded = recipe.items;
                this.recipe = recipe;
                break;
            }
        }

        craftingItem = mysql.ReadItem(recipe.item_id);
        inventory = Server.clients[from].player.inventory;
    }

    public List<RecipeItemPossessed> itemsPossessed = new List<RecipeItemPossessed>();

    public int GetMaxCraftAmount() {
        int[] canMakeList = new int[itemsNeeded.Count];
        int i = 0;
        foreach (RecipeItemRequirement item in itemsNeeded)
        {
            RecipeItemPossessed itemPossessed = CheckIfPlayerHasRequiredItem(item.item_id);            

            if (itemPossessed != null)
                itemsPossessed.Add(itemPossessed);

            canMakeList[i] = CanMake(item);
            i++;
        }

        return canMakeList.Min();
    }

    RecipeItemPossessed CheckIfPlayerHasRequiredItem(int itemId)
    {
        Inventory inventory = Server.clients[from].player.inventory;

        foreach (InventorySlot slot in inventory.items)
        {
            if (slot.item != null)
                if (itemId == slot.item.item_id)
                {
                    RecipeItemPossessed itemPossessed = new RecipeItemPossessed();
                    itemPossessed.itemId = slot.item.item_id;
                    itemPossessed.quantity = slot.quantity;
                    return itemPossessed;
                }
        }
        return null;
    }

    int CanMake(RecipeItemRequirement rItem)
    {
        foreach (RecipeItemPossessed item in itemsPossessed)
        {
            if (rItem.item_id == item.itemId)
            {
                return item.quantity / rItem.quantity;
            }
        }
        return 0;
    }

    public class RecipeItemPossessed
    {
        public int itemId;
        public int quantity;
    }

    IEnumerator CraftCoroutine;
    public bool stopCrafting = false;

    public void Craft(int amount, int modifier, float time_to_craft) {
        float time = time_to_craft - time_to_craft / modifier;
        Player player = Server.clients[from].player;

        ServerSend.CraftStatus(player.id, amount, time, craftingItem.iconName, craftingItem.name);

        IEnumerator CraftCoroutine() {
            int i = amount;
            while (i > 0) {
                yield return new WaitForSeconds(time);

                if (stopCrafting)
                {
                    stopCrafting = false;
                    yield break;
                }

                mysql.InventoryAdd(player, craftingItem, 1);

                foreach (RecipeItemRequirement itemRequirement in itemsNeeded)
                {
                    int item_id = itemRequirement.item_id;
                    int quantity = itemRequirement.quantity;

                    InventorySlot slot = FindSlot(item_id);
                    inventory.RemoveAmount(slot.slotID, quantity);

                    if (slot.item == null)
                    {
                        mysql.RemoveInventoryItem(player.dbid, slot.slotID);
                    }
                    else {
                        mysql.UpdateItemQuantity(player.dbid, slot);
                    }
                }

                mysql.UpdateSkillExperience(player.dbid, (int)recipe.skill_id, recipe.experience);
                player.ExperienceGained((SkillType)recipe.skill_id, recipe.experience, player);
                player.skills = mysql.ReadPlayerSkills(player.dbid);

                i--;
                ServerSend.Inventory(from, Server.clients[from].player.inventory);
                ServerSend.ExperienceGained(from, recipe.experience);
            }
        }

        IEnumerator coroutine = CraftCoroutine();
        StartCoroutine(coroutine);
    }

    public void Stop()
    {
        stopCrafting = true;
    }

    public InventorySlot FindSlot(int item_id) {
        foreach (InventorySlot slot in inventory.items) {
            if (slot.item!=null && slot.item.item_id == item_id) {
                return slot;
            }
        }
        return null;
    }
}
