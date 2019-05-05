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
    /// <typeparam name="T"></typeparam>
    public abstract class CommonProviderCollection<T> : ProviderCollection where T : ProviderBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public new T this[string name]
        {
            get { return (T)base[name]; }
        }
    }
}
