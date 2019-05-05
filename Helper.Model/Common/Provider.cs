using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using System.Configuration.Provider;

namespace Helper.Model.Common
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CommonProvider : ProviderBase
    {
        public const string CONFIG = "config";
        public const string CONFIG_NAME = "name";
        public const string CONFIG_DESCRIPTION = "description";
        public const string CONFIG_SOURCE = "providerConfiguration";


        string _configurationFile = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            // check if config is correctly populated

            if (null == config)
            {
                throw new ArgumentNullException(CommonProvider.CONFIG);
            }


            // set the default name
            if (String.IsNullOrEmpty(name))
            {
                name = GetProviderName();
            }



            if (String.IsNullOrEmpty(config[CommonProvider.CONFIG_DESCRIPTION]))
            {

                config.Remove(CommonProvider.CONFIG_DESCRIPTION);

                config.Add(CommonProvider.CONFIG_DESCRIPTION, GetProviderDescription());

            }


            if (!String.IsNullOrEmpty(config[CommonProvider.CONFIG_SOURCE]))
            {
                //MappingFile = config[DependencyResolverProvider.CONFIG_MAPPING_FILE];

                // validate that the config file is found

                if (!System.IO.File.Exists(GetPathToConfigFile(config[CommonProvider.CONFIG_SOURCE])))
                {
                    // TO DO : enhance FileNotFoundException

                    throw new System.IO.FileNotFoundException();
                }


                _configurationFile = config[CommonProvider.CONFIG_SOURCE];

                config.Remove(config[CommonProvider.CONFIG_SOURCE]);
            }



            base.Initialize(name, config);
        }


        /// <summary>
        /// 
        /// </summary>
        public string ProviderConfiguration
        {
            get
            {
                return _configurationFile;
            }

            protected set
            {
                _configurationFile = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        protected internal string GetPathToConfigFile(string configFile)
        {

            if (!System.IO.File.Exists(configFile))
            {

                string s = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);

                if (System.IO.File.Exists(s))
                {
                    configFile = s;
                }

            }

            return configFile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract string GetProviderName();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract string GetProviderDescription();
    }
}
