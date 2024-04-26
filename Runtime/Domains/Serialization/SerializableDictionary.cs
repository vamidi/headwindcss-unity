
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeadWindCSS.Domains.Serialization
{
    [Serializable]
    public class SerializeDictionaryElement<TKey, TValue>
    {
        public TKey key;
        public TValue value;
        
        public SerializeDictionaryElement(TKey key, TValue value)
        { 
            this.key = key; 
            this.value = value;
        }
    }
    
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<SerializeDictionaryElement<TKey, TValue>> elements = new ();

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            elements.Clear();
            
            foreach(KeyValuePair<TKey, TValue> pair in this)
            {
                elements.Add(new SerializeDictionaryElement<TKey, TValue>(pair.Key, pair.Value));
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            Clear();

            foreach (var element in elements)
            {
                this[element.key] = element.value;
            }
        }
    }
}