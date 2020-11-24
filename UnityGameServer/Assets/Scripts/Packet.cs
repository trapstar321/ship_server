using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

/// <summary>Sent from server to client.</summary>
public enum ServerPackets
{
    welcome = 1,
    spawnPlayer,
    playerPosition,
    playerDisconnected,
    playerHealth,
    playerRespawned,
    createItemSpawner,
    itemSpawned,
    itemPickedUp,
    spawnProjectile,
    projectilePosition,
    projectileExploded,
    spawnEnemy,
    enemyPosition,
    enemyHealth,
    wavesMesh,
    time,
    inventory,
    addToInventory,
    spawnGameObject,
    shipEquipment,
    playerEquipment,
    onGameStart,
    shoot,
    takeDamage,
	stats,
    cannonRotate,
    cannonRotateAngle,
    npcPosition,
    npcShoot,
    baseStats,
    npcStats,
    onLootDropped,
    hello,
    loginFailed,
    chatMessage,
    onGameMessage,
    groupCreateStatus,
    groupList,
    playerAppliedToGroup,
    groupMembers,
    kickedFromGroup,
    playerList
}

/// <summary>Sent from client to server.</summary>
public enum ClientPackets
{
    welcomeReceived = 1,
    playerMovement,
    playerShoot,
    playerThrowItem,
    joystick,
    position,
    test,
    getInventory,
    dropItem,
    dragAndDrop,
    searchChest,
    addShipEquipment,
    removeShipEquipment,
    getShipEquipment,
    removeItemFromInventory,
    replaceShipEquipment,
    addItemToInventory,
    getPlayerEquipment,
    replacePlayerEquipment,
    removePlayerEquipment,
    onGameStart,
    shoot,
    cannonRotate,
    collectLoot,
    discardLoot,
    login,
    chatMessage,
    createGroup,
    getGroupList,
    applyToGroup,
    acceptGroupApplicant,
    declineGroupApplicant,
    kickGroupMember,
    leaveGroup,
    getPlayerList,
    invitePlayer
}

public class Packet: IDisposable
{
    public List<byte> buffer;
    private byte[] readableBuffer;
    private int readPos;

    /// <summary>Creates a new empty packet (without an ID).</summary>
    public Packet()
    {
        buffer = new List<byte>(); // Initialize buffer
        readPos = 0; // Set readPos to 0
    }

    /// <summary>Creates a new packet with a given ID. Used for sending.</summary>
    /// <param name="_id">The packet ID.</param>
    public Packet(int _id)
    {
        buffer = new List<byte>(); // Initialize buffer
        readPos = 0; // Set readPos to 0

        Write(_id); // Write packet id to the buffer
    }

    /// <summary>Creates a packet from which data can be read. Used for receiving.</summary>
    /// <param name="_data">The bytes to add to the packet.</param>
    public Packet(byte[] _data)
    {
        buffer = new List<byte>(); // Initialize buffer
        readPos = 0; // Set readPos to 0

        SetBytes(_data);
    }

    #region Functions
    /// <summary>Sets the packet's content and prepares it to be read.</summary>
    /// <param name="_data">The bytes to add to the packet.</param>
    public void SetBytes(byte[] _data)
    {
        Write(_data);
        readableBuffer = buffer.ToArray();
    }

    /// <summary>Inserts the length of the packet's content at the start of the buffer.</summary>
    public void WriteLength()
    {
        buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count)); // Insert the byte length of the packet at the very beginning
    }

    /// <summary>Inserts the given int at the start of the buffer.</summary>
    /// <param name="_value">The int to insert.</param>
    public void InsertInt(int _value)
    {
        buffer.InsertRange(0, BitConverter.GetBytes(_value)); // Insert the int at the start of the buffer
    }

    /// <summary>Gets the packet's content in array form.</summary>
    public byte[] ToArray()
    {
        readableBuffer = buffer.ToArray();
        return readableBuffer;
    }

    /// <summary>Gets the length of the packet's content.</summary>
    public int Length()
    {
        return buffer.Count; // Return the length of buffer
    }

    /// <summary>Gets the length of the unread data contained in the packet.</summary>
    public int UnreadLength()
    {
        return Length() - readPos; // Return the remaining length (unread)
    }

    /// <summary>Resets the packet instance to allow it to be reused.</summary>
    /// <param name="_shouldReset">Whether or not to reset the packet.</param>
    public void Reset(bool _shouldReset = true)
    {
        if (_shouldReset)
        {
            buffer.Clear(); // Clear buffer
            readableBuffer = null;
            readPos = 0; // Reset readPos
        }
        else
        {
            readPos -= 4; // "Unread" the last read int
        }
    }
    #endregion

    #region Write Data
    /// <summary>Adds a byte to the packet.</summary>
    /// <param name="_value">The byte to add.</param>
    public void Write(byte _value)
    {
        buffer.Add(_value);
    }
    /// <summary>Adds an array of bytes to the packet.</summary>
    /// <param name="_value">The byte array to add.</param>
    public void Write(byte[] _value)
    {
        buffer.AddRange(_value);
    }
    /// <summary>Adds a short to the packet.</summary>
    /// <param name="_value">The short to add.</param>
    public void Write(short _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>Adds an int to the packet.</summary>
    /// <param name="_value">The int to add.</param>
    public void Write(int _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>Adds a long to the packet.</summary>
    /// <param name="_value">The long to add.</param>
    public void Write(long _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>Adds a float to the packet.</summary>
    /// <param name="_value">The float to add.</param>
    public void Write(float _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>Adds a bool to the packet.</summary>
    /// <param name="_value">The bool to add.</param>
    public void Write(bool _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>Adds a string to the packet.</summary>
    /// <param name="_value">The string to add.</param>
    public void Write(string _value)
    {
        Write(_value.Length); // Add the length of the string to the packet
        buffer.AddRange(Encoding.ASCII.GetBytes(_value)); // Add the string itself
    }

    public void Write(MeshSerializable.MeshSerializable _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(List<SerializableObjects.InventorySlot> _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(SerializableObjects.InventorySlot _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(List<SerializableObjects.Item> _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(List<SerializableObjects.ItemDrop> _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(List<SerializableObjects.BaseStat> _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(SerializableObjects.BaseStat _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(List<SerializableObjects.Experience> _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(SerializableObjects.PlayerData _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(SerializableObjects.Message _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(SerializableObjects.Group _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(List<SerializableObjects.Group> _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(List<SerializableObjects.GroupMember> _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(List<SerializableObjects.PlayerData> _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    public void Write(Time _value)
    {
        byte[] data = ObjectToByteArray(_value);
        Write(data.Length); // Add the length of the string to the packet
        buffer.AddRange(data); // Add the string itself
    }

    byte[] ObjectToByteArray(object obj)
    {
        if (obj == null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    /// <summary>Adds a Vector3 to the packet.</summary>
    /// <param name="_value">The Vector3 to add.</param>
    public void Write(Vector3 _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
    }
    /// <summary>Adds a Quaternion to the packet.</summary>
    /// <param name="_value">The Quaternion to add.</param>
    public void Write(Quaternion _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
        Write(_value.w);
    }
    #endregion

    #region Read Data
    /// <summary>Reads a byte from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public byte ReadByte(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            byte _value = readableBuffer[readPos]; // Get the byte at readPos' position
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += 1; // Increase readPos by 1
            }
            return _value; // Return the byte
        }
        else
        {
            throw new Exception("Could not read value of type 'byte'!");
        }
    }

    /// <summary>Reads an array of bytes from the packet.</summary>
    /// <param name="_length">The length of the byte array.</param>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public byte[] ReadBytes(int _length, bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            byte[] _value = buffer.GetRange(readPos, _length).ToArray(); // Get the bytes at readPos' position with a range of _length
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += _length; // Increase readPos by _length
            }
            return _value; // Return the bytes
        }
        else
        {
            throw new Exception("Could not read value of type 'byte[]'!");
        }
    }

    /// <summary>Reads a short from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public short ReadShort(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            short _value = BitConverter.ToInt16(readableBuffer, readPos); // Convert the bytes to a short
            if (_moveReadPos)
            {
                // If _moveReadPos is true and there are unread bytes
                readPos += 2; // Increase readPos by 2
            }
            return _value; // Return the short
        }
        else
        {
            throw new Exception("Could not read value of type 'short'!");
        }
    }

    /// <summary>Reads an int from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public int ReadInt(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            int _value = BitConverter.ToInt32(readableBuffer, readPos); // Convert the bytes to an int
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += 4; // Increase readPos by 4
            }
            return _value; // Return the int
        }
        else
        {
            throw new Exception("Could not read value of type 'int'!");
        }
    }

    /// <summary>Reads a long from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public long ReadLong(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            long _value = BitConverter.ToInt64(readableBuffer, readPos); // Convert the bytes to a long
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += 8; // Increase readPos by 8
            }
            return _value; // Return the long
        }
        else
        {
            throw new Exception("Could not read value of type 'long'!");
        }
    }

    /// <summary>Reads a float from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public float ReadFloat(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            float _value = BitConverter.ToSingle(readableBuffer, readPos); // Convert the bytes to a float
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += 4; // Increase readPos by 4
            }
            return _value; // Return the float
        }
        else
        {
            throw new Exception("Could not read value of type 'float'!");
        }
    }

    /// <summary>Reads a bool from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public bool ReadBool(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            bool _value = BitConverter.ToBoolean(readableBuffer, readPos); // Convert the bytes to a bool
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += 1; // Increase readPos by 1
            }
            return _value; // Return the bool
        }
        else
        {
            throw new Exception("Could not read value of type 'bool'!");
        }
    }

    /// <summary>Reads a string from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public string ReadString(bool _moveReadPos = true)
    {
        try
        {
            int _length = ReadInt(); // Get the length of the string
            string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length); // Convert the bytes to a string
            if (_moveReadPos && _value.Length > 0)
            {
                // If _moveReadPos is true string is not empty
                readPos += _length; // Increase readPos by the length of the string
            }
            return _value; // Return the string
        }
        catch
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }

    /// <summary>Reads a Vector3 from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public Vector3 ReadVector3(bool _moveReadPos = true)
    {
        return new Vector3(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
    }

    /// <summary>Reads a Quaternion from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public Quaternion ReadQuaternion(bool _moveReadPos = true)
    {
        return new Quaternion(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
    }

    public List<int> ReadIntList(bool _moveReadPos = true)
    {
        try
        {
            int _length = ReadInt(); // Get the length of the string
            byte[] data = new byte[_length];
            Array.Copy(readableBuffer, readPos, data, 0, _length);
            if (_moveReadPos && data.Length > 0)
            {
                // If _moveReadPos is true string is not empty
                readPos += _length; // Increase readPos by the length of the string
            }
            return FromByteArray<List<int>>(data); // Return the string
        }
        catch
        {
            throw new Exception("Could not read value of type 'List<int>'!");
        }
    }

    public SerializableObjects.InventorySlot ReadInventorySlot(bool _moveReadPos = true)
    {
        try
        {
            int _length = ReadInt(); // Get the length of the string
            byte[] data = new byte[_length];
            Array.Copy(readableBuffer, readPos, data, 0, _length);
            if (_moveReadPos && data.Length > 0)
            {
                // If _moveReadPos is true string is not empty
                readPos += _length; // Increase readPos by the length of the string
            }
            return FromByteArray<SerializableObjects.InventorySlot>(data); // Return the string
        }
        catch
        {
            throw new Exception("Could not read value of type 'SerializableObjects.InventorySlot'!");
        }
    }

    public SerializableObjects.Item ReadItem(bool _moveReadPos = true)
    {
        try
        {
            int _length = ReadInt(); // Get the length of the string
            byte[] data = new byte[_length];
            Array.Copy(readableBuffer, readPos, data, 0, _length);
            if (_moveReadPos && data.Length > 0)
            {
                // If _moveReadPos is true string is not empty
                readPos += _length; // Increase readPos by the length of the string
            }
            return FromByteArray<SerializableObjects.Item>(data); // Return the string
        }
        catch
        {
            throw new Exception("Could not read value of type 'SerializableObjects.InventorySlot'!");
        }
    }

    public SerializableObjects.Message ReadMessage(bool _moveReadPos = true)
    {
        try
        {
            int _length = ReadInt(); // Get the length of the string
            byte[] data = new byte[_length];
            Array.Copy(readableBuffer, readPos, data, 0, _length);
            if (_moveReadPos && data.Length > 0)
            {
                // If _moveReadPos is true string is not empty
                readPos += _length; // Increase readPos by the length of the string
            }
            return FromByteArray<SerializableObjects.Message>(data); // Return the string
        }
        catch
        {
            throw new Exception("Could not read value of type 'SerializableObjects.Message'!");
        }
    }

    public T FromByteArray<T>(byte[] data)
    {
        if (data == null)
            return default(T);
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream(data))
        {
            object obj = bf.Deserialize(ms);
            return (T)obj;
        }
    }
    #endregion

    private bool disposed = false;

    protected virtual void Dispose(bool _disposing)
    {
        if (!disposed)
        {
            if (_disposing)
            {
                buffer = null;
                readableBuffer = null;
                readPos = 0;
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
