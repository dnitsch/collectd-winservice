using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace collectdSrv.Configuration
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Defines the application config section for Document Repository settings.
    /// </summary>
    public class PerfCounterConfigurationSection : ConfigurationSection
    {
        private const string ConfigurationSectionName = "externalConstants/perf_counters";
        private PerfCounterConfigurationSection section;

        /// <summary>
        /// Gets a value indicating whether Couchbase development views should be queried rather than production views.
        /// </summary>
        [ConfigurationProperty("counter", IsRequired = true)]
        public Counter counter
        {
            get { return (Counter)this["counter"]; }
        }

        public class Counter : ConfigurationElement
        {
            [ConfigurationProperty("name", DefaultValue = "", IsRequired = true)]
            [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
            public String Name
            {
                get
                {
                    return (String)this["name"];
                }
                set
                {
                    this["name"] = value;
                }
            }

            [ConfigurationProperty("instance", DefaultValue = "", IsRequired = false)]
            [IntegerValidator(ExcludeRange = false, MaxValue = 4096, MinValue = 0)]
            public String Size
            {
                get
                { return (String)this["instance"]; }
                set
                { this["instance"] = value; }
            }
        }


        /// <summary>
        /// Gets the configuration section defined in the current application config.
        /// </summary>
        /// <returns>A <see cref="DocumentRepositoryConfigurationSection"/> if it has been defined; otherwise, <b>null</b>.</returns>
        public PerfCounterConfigurationSection GetSection()
        {
            if (this.section == null)
            {
                this.section =
                    ConfigurationManager.GetSection(ConfigurationSectionName) as PerfCounterConfigurationSection;

                // This section is currently optional because it only configures development settings.  Uncomment if it is to be mandatory.
                //if (this.section == null)
                //    throw new ConfigurationErrorsException(
                //        string.Format("Could not find a section with the name '{0}' in the application config.",
                //                      ConfigurationSectionName));
            }
            return this.section;
        }

        /// <summary>
        /// Gets a string representation of the config section.
        /// </summary>
        /// <returns>A string describing the section.</returns>
        public override string ToString()
        {
            return string.Format("name: {0}", this.counter);
        }
    }
}
