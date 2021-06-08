using SerializableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager
{
    public List<Buff> buffs = new List<Buff>();
    public float buffCheckStart;
    private object ship;
    private object playerCharacter;
    private int from;

    public BuffManager(int from, object ship, object playerCharacter)
    {
        this.from = from;
        this.ship = ship;
        this.playerCharacter = playerCharacter;
    }

    public void BuffCheck()
    {
        if (Time.time - buffCheckStart > NetworkManager.parameters.buffCheckPeriod)
        {
            //remove buff koji je istekao
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                Buff buff = buffs[i];
                if ((System.DateTime.UtcNow - buff.start).TotalSeconds > buff.buff_duration * 60)
                {
                    //ako nije overtime onda smanjiti vrijednost property-ja
                    if (!buff.overtime)
                    {
                        buff.value *= -1;
                        ApplyBuff(buff);
                        ServerSend.Stats(from);
                    }
                    Debug.Log("Remove buff: " + buff.item_name);
                    buffs.RemoveAt(i);                    
                }
            }

            //za overtime buff-ove koji imaju duration
            foreach (Buff buff in buffs.Where(x => x.overtime && x.buff_duration > 0))
            {
                ApplyBuff(buff);
                ServerSend.Stats(from);
            }
            buffCheckStart = Time.time;
        }
    }

    public void AddBuff(Item item, out bool onCooldown)
    {
        onCooldown = false;
        Debug.Log("Buff: " + item.name);
        //cooldown check        
        foreach (Item it in GameServer.clients[from].player.inventory.FindAllItems(item.item_id))
        {
            if (it.cooldown > 0)
            {
                if ((DateTime.UtcNow - it.buff_start).TotalSeconds < it.cooldown * 60)
                {
                    Debug.Log("On cooldown: "+it.name);
                    onCooldown = true;
                    return;
                }
            }
        }

        Buff buff = new Buff();
        string propName;
        int value;
        GetBuffProperty(item, out propName, out value);

        buff.item_name = item.name;
        buff.property_name = propName;
        buff.overtime = item.overtime;
        buff.value = value;
        buff.buff_duration = item.buff_duration;
        buff.start = System.DateTime.UtcNow;
        buff.icon = item.iconName;
        buff.cooldown = item.cooldown;

        bool maxPropExists = PropertyExists(item, "max_" + buff.property_name);
        if (maxPropExists)
            buff.max_property_name = "max_" + buff.property_name;

        if (buffs.Where(x => x.item_name == buff.item_name).Count() == 0)
        {
            ApplyBuff(buff);
            ServerSend.BuffAdded(from, ((PlayerCharacter)playerCharacter).transform.position, buff, item);
            ServerSend.Stats(from);

            if (!buff.overtime && buff.buff_duration == 0)
            {
                foreach (Item it in GameServer.clients[from].player.inventory.FindAllItems(item.item_id))
                {
                    it.buff_start = DateTime.UtcNow;
                }
            }
        }

        bool refresh = true;
        if (buff.buff_duration > 0 && buffs.Where(x => x.item_name == buff.item_name).Count() == 0)
        {
            refresh = false;
            buffs.Add(buff);            
        }

        //refresh scrollova i overtime potiona
        if (buff.buff_duration > 0 && refresh)
        {
            Buff b = buffs.Where(x => x.item_name == item.name).FirstOrDefault();            

            if (b != null)
            {
                b.start = System.DateTime.UtcNow;
                ServerSend.BuffAdded(from, ((PlayerCharacter)playerCharacter).transform.position, buff, item);
                Debug.Log("Refreshed: " + item.name);
            }            
        }
    }

    private void ApplyBuff(Buff buff)
    {
        //1. odmah doda stat -> scroll, nakon nekog vremena se ukloni
        //2. overtime - ovo radi tick ne apply buff
        //3. npr. health potion -> odmah doda stat ali ne preko max

        //case 1. -> npr. scroll ali može biti i potion
        if (!buff.overtime && buff.buff_duration > 0)
        {
            if (ShipPropertyExists(buff.property_name))
            {
                int currentValue = GetShipPropertyValue(buff.property_name);
                int newValue = currentValue + buff.value;
                SetShipBuffProperty(buff.property_name, newValue);

                if (buff.property_name.Contains("max_"))
                {
                    string normal_property = buff.property_name.Replace("max_", "");

                    int normal_property_value = GetShipPropertyValue(normal_property);
                    if (normal_property_value > newValue)
                    {
                        SetShipBuffProperty(normal_property, newValue);
                    }
                }
            }
            if (PlayerCharacterPropertyExists(buff.property_name))
            {
                int currentValue = GetPlayerCharacterPropertyValue(buff.property_name);
                int newValue = currentValue + buff.value;
                SetPlayerCharacterBuffProperty(buff.property_name, newValue);

                if (buff.property_name.Contains("max_"))
                {
                    string normal_property = buff.property_name.Replace("max_", "");

                    int normal_property_value = GetPlayerCharacterPropertyValue(normal_property);
                    if (normal_property_value > newValue)
                    {
                        SetPlayerCharacterBuffProperty(normal_property, newValue);
                    }
                }
            }
        }

        //case 2. -> npr. energy potion overtime
        //case 3. -> npr. health potion
        if ((!buff.overtime && buff.buff_duration == 0) || (buff.overtime && buff.buff_duration > 0))
        {
            if (ShipPropertyExists(buff.max_property_name))
            {
                int currentValue = GetShipPropertyValue(buff.property_name);
                int maxValue = GetShipPropertyValue(buff.max_property_name);

                if (currentValue + buff.value > maxValue)
                {
                    SetShipBuffProperty(buff.property_name, maxValue);
                }
                else
                {
                    SetShipBuffProperty(buff.property_name, currentValue + buff.value);
                }

            }
            if (PlayerCharacterPropertyExists(buff.max_property_name))
            {
                int currentValue = GetPlayerCharacterPropertyValue(buff.property_name);
                int maxValue = GetPlayerCharacterPropertyValue(buff.max_property_name);

                if (currentValue + buff.value > maxValue)
                {
                    SetPlayerCharacterBuffProperty(buff.property_name, maxValue);
                }
                else
                {
                    SetPlayerCharacterBuffProperty(buff.property_name, currentValue + buff.value);
                }
            }
        }
    }

    private void GetBuffProperty(Item item, out string propName, out int value)
    {
        propName = null;
        value = 0;

        foreach (string p in NetworkManager.item_buff_properties)
        {
            value = GetPropertyValue(item, p);

            if (value > 0)
            {
                propName = p;
                break;
            }
        }
    }

    private void SetShipBuffProperty(string propName, int value)
    {
        if (ShipPropertyExists(propName))
        {
            Debug.Log("Set property " + propName + "=" + value);
            ship.GetType().GetField(propName).SetValue(ship, value);
        }
    }

    private void SetPlayerCharacterBuffProperty(string propName, int value)
    {
        if (PlayerCharacterPropertyExists(propName))
        {
            Debug.Log("Set property " + propName + "=" + value);
            playerCharacter.GetType().GetField(propName).SetValue(playerCharacter, value);
        }
    }

    private bool PropertyExists(Item item, string propName)
    {
        return item.GetType().GetField(propName) != null;
    }

    private bool ShipPropertyExists(string propName)
    {
        if (NetworkManager.ship_buff_properties.Contains(propName))
            return ship.GetType().GetField(propName) != null;
        return false;
    }

    private bool PlayerCharacterPropertyExists(string propName)
    {
        if (NetworkManager.player_buff_properties.Contains(propName))
            return playerCharacter.GetType().GetField(propName) != null;
        return false;
    }

    private int GetPropertyValue(Item item, string propName)
    {
        return Convert.ToInt32(item.GetType().GetField(propName).GetValue(item));
    }

    private int GetShipPropertyValue(string propName)
    {
        return Convert.ToInt32(ship.GetType().GetField(propName).GetValue(ship));
    }

    private int GetPlayerCharacterPropertyValue(string propName)
    {
        return Convert.ToInt32(playerCharacter.GetType().GetField(propName).GetValue(playerCharacter));
    }
}
