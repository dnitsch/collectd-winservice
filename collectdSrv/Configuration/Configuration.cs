namespace collectdSrv.Configuration
{
    using System;
    using System.Configuration;
    public class PerfCounterConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("Counters", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(CounterCollection))]  //  AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove"
        public CounterCollection Counters
        {
            get { return (CounterCollection)base["Counters"]; }
           // set { this["performanceCounters"] = value; }
        }
    }

    public class CounterCollection : ConfigurationElementCollection
    {
        public CounterCollection()
        {
           // Console.WriteLine("ConfigCollection Constructor");
        }

        public CounterConfig this[int idx]
        {
            get { return (CounterConfig)BaseGet(idx); }
            set
            {
                if (BaseGet(idx) != null)
                {
                    BaseRemoveAt(idx);
                }
                BaseAdd(idx, value);
            }
        }

        public void Add(CounterConfig counterConfig)
        {
            BaseAdd(counterConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CounterConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CounterConfig)element).Name;
        }

        public void Remove(CounterConfig counterConfig)
        {
            BaseRemove(counterConfig.Name);
        }

        public void RemoveAt(int idx)
        {
            BaseRemoveAt(idx);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }
    }

    public class CounterConfig : ConfigurationElement
    {
        public CounterConfig() { }

        public CounterConfig(string name, string counterName, string instanceName)
        {
            Name = name;
            CounterName = counterName;
            InstanceName = instanceName;
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("counterName", IsRequired = false, IsKey = false)]
        public string CounterName
        {
            get { return (string)this["counterName"]; }
            set { this["counterName"] = value; }
        }

        [ConfigurationProperty("instanceName", IsRequired = false, IsKey = false)]
        public string InstanceName
        {
            get { return (string)this["instanceName"]; }
            set { this["instanceName"] = value; }
        }
    }
}