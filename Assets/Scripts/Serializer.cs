using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Managers;
using UnityEngine;

public class Serializer
{
    private static BinaryFormatter _bin = new BinaryFormatter();

    public static void Serialize(string path, object obj)
    {
        using (FileStream stream = File.Create(path))
        {
            try
            {
                _bin.Serialize(stream, obj);
            }
            catch (SerializationException se)
            {
                Debug.LogError("Serialization failed: " + se.Message);
            }
        }
    }

    public static T Deserialize<T>(string path)
    {
        T obj;
        if (!File.Exists(path)) return default;
        using (FileStream stream = File.OpenRead(path))
        {
            try
            {
                obj = (T) _bin.Deserialize(stream);
            }
            catch (SerializationException se)
            {
                Debug.LogError("Deserialization failed: " + se.Message);
                throw;
            }
        }

        return obj;
    }
}