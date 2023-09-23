using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace PiFramework.Serialization
{
    public enum StorageLocation { PlayerPref, DataPath, PersistentDataPath, WWW };
    //public enum StorageType { PlayerPref, File };

    internal class SerializerProvider
    {
        Dictionary<string, ISerializer> _serializers;
        public ISerializer GetSerializer()
        {
            return GetSerializer("default");
        }

        public ISerializer GetSerializer(string bin)
        {
            return _serializers[bin];
        }

        public ISerializer CreateSerializer(string bin, SerializerSettings settings)
        {
            return null;
        }

    }

    public interface ISerializer {
        
        
    }

    public class SerializerSettings
    {
        public StorageLocation Location;
        public bool Encrypt;
        public bool SaveReference; 
    }

 
}