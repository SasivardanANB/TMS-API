using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Helper.Model.Common
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CommonConfigurationSection : ConfigurationSection
    {
        public const string CONFIG_PROP_DEFAULT_PROVIDER = "defaultProvider";

        public const string CONFIG_PROP_PROVIDERS = "providers";


        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty(CommonConfigurationSection.CONFIG_PROP_DEFAULT_PROVIDER)]
        public string DefaultProvider
        {
            get
            {
                return (string)base[CommonConfigurationSection.CONFIG_PROP_DEFAULT_PROVIDER];
            }
            set
            {
                base[CommonConfigurationSection.CONFIG_PROP_DEFAULT_PROVIDER] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty(CommonConfigurationSection.CONFIG_PROP_PROVIDERS)]
        public System.Configuration.ProviderSettingsCollection Providers
        {
            get
            {
                return (System.Configuration.ProviderSettingsCollection)base[CommonConfigurationSection.CONFIG_PROP_PROVIDERS];
            }
        }
    }
}
