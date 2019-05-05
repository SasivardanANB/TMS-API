using System;
using System.Collections.Generic;
using System.Text;

using Helper.Model.Common;

namespace Helper.Model.DependencyResolver
{
    #region interface

    /// <summary>
    /// 
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetImplementationOf<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetImplementationOf<T>(string key);
    }

    #endregion interface

    #region implementation

    /// <summary>
    /// 
    /// </summary>
    public abstract class DependencyResolverProvider : CommonProvider, IResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public abstract T GetImplementationOf<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract T GetImplementationOf<T>(string key);

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion
    }

    #endregion implementation
}
