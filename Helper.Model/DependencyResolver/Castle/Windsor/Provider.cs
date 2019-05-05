using System;
using System.Collections.Generic;
using System.Text;

using Helper.Model.DependencyResolver;

using Castle.Core;
using Castle.Windsor;
using Castle.Windsor.Configuration;
using Castle.Windsor.Configuration.Interpreters;

namespace Helper.Model.DependencyResolver.Castle.Windsor
{
    public class CastleWindsorDependencyResolverProvider : DependencyResolverProvider, IResolver
    {
        internal const string DEFAULT_PROVIDER_NAME = "CastleWindsorDependencyResolverProvider";
        internal const string DEFAULT_PROVIDER_DESCRIPTION = "Castle Windsor Dependency Resolver Provider";

        IWindsorContainer _container = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
        }


        public CastleWindsorDependencyResolverProvider()
            : this(String.Empty)
        {
        }

        public CastleWindsorDependencyResolverProvider(string providerConfiguration)
        {
            ProviderConfiguration = providerConfiguration;
        }



        /// <summary>
        /// 
        /// </summary>
        IWindsorContainer Container
        {
            get
            {
                if (null == _container)
                {

                    if (String.IsNullOrEmpty(ProviderConfiguration))
                    {

                        // configuration read from applications config file ( i.e. web.config )

                        _container = new WindsorContainer();

                    }
                    else
                    {
                        _container = new WindsorContainer(
                            new XmlInterpreter(GetPathToConfigFile(ProviderConfiguration))
                            );
                    }
                }
                return _container;

            }
        }


        #region DependencyResolverProvider abstract implementation

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetProviderName()
        {
            return CastleWindsorDependencyResolverProvider.DEFAULT_PROVIDER_NAME;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetProviderDescription()
        {
            return CastleWindsorDependencyResolverProvider.DEFAULT_PROVIDER_DESCRIPTION;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override T GetImplementationOf<T>()
        {
            T t = Container.Resolve<T>();

            return t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public override T GetImplementationOf<T>(string key)
        {
            T t = Container.Resolve<T>(key);

            return t;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            if (null != _container)
            {
                _container.Release(_container);
            }

            base.Dispose();
        }

        #endregion DependencyResolverProvider abstract implementation

    }
}
