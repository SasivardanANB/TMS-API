using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Configuration.Provider;
using System.Web;
using System.Web.Configuration;


namespace Helper.Model.DependencyResolver
{

    /// <summary>
    /// 
    /// </summary>
    public static class DependencyResolver
    {

        internal const string CONFIG_SECTION = "Helper.Model/dependencyResolver";

        static readonly DependencyResolverProvider _provider;

        static readonly DependencyResolverProviderCollection _providers;



        /// <summary>
        /// 
        /// </summary>
        public static DependencyResolverProvider Provider
        {
            get { return DependencyResolver._provider; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyResolverProviderCollection Providers
        {
            get { return DependencyResolver._providers; }
        }


        static DependencyResolver()
        {
            object o = null;

            try
            {
                o = ConfigurationManager.GetSection(DependencyResolver.CONFIG_SECTION);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            DependencyResolverConfigurationSection section = (DependencyResolverConfigurationSection)o;

            //DependencyResolverConfigurationSection section =
            //    (DependencyResolverConfigurationSection)ConfigurationManager.GetSection(DependencyResolver.CONFIG_SECTION);

            _providers = new DependencyResolverProviderCollection();


            // instaniate providers

            ProviderSettingsCollection psc = section.Providers;


            ProvidersHelper.InstantiateProviders(
                psc /*section.Providers*/,
                Providers,
                typeof(DependencyResolverProvider));

            Providers.SetReadOnly();




            // check for default provider
            if (string.IsNullOrEmpty(section.DefaultProvider))
            {
                throw new ProviderException("No default provider specified");
            }

            try
            {
                _provider = (DependencyResolverProvider)_providers[section.DefaultProvider];
            }
            catch { }


            if (null == _provider)
            {

                PropertyInformation pi = section.ElementInformation.Properties[DependencyResolverConfigurationSection.CONFIG_PROP_DEFAULT_PROVIDER];

                throw new ConfigurationErrorsException("Default provider not found", pi.Source, pi.LineNumber);

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetImplementationOf<T>()
        {
            return DependencyResolver.Provider.GetImplementationOf<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetImplementationOf<T>(string key)
        {
            return DependencyResolver.Provider.GetImplementationOf<T>(key);
        }



    }
}
