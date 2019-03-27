using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Toss.Client.Services
{
    /// <summary>
    /// This class is used for picking the environment given a Uri
    /// </summary>
    public class EnvironmentChooser
    {
        private const string QueryStringKey = "Environment";
        private readonly string defaultEnvironment;
        private readonly Dictionary<string, Tuple<string, bool>> _hostMapping = new Dictionary<string, Tuple<string,bool>>();

        /// <summary>
        /// Build a chooser
        /// </summary>
        /// <param name="defaultEnvironment">If no environment is found on the domain name or query then this will be returned</param>
        public EnvironmentChooser(string defaultEnvironment)
        {
            if (string.IsNullOrWhiteSpace(defaultEnvironment))
            {
                throw new ArgumentException("message", nameof(defaultEnvironment));
            }

            this.defaultEnvironment = defaultEnvironment;
        }

        public string DefaultEnvironment => defaultEnvironment;

        /// <summary>
        /// Add a new binding between a hostname and an environment
        /// </summary>
        /// <param name="hostName">The hostname that must fully match the uri</param>
        /// <param name="env">The environement that'll be returned</param>
        /// <param name="queryCanOverride">If false, we can't override the environement with a "Environment" in the GET parameters</param>
        /// <returns></returns>
        public EnvironmentChooser Add(string hostName, string env, bool queryCanOverride = false)
        {
            this._hostMapping.Add(hostName, new Tuple<string,bool>(env, queryCanOverride));
           
            return this;
        }

        /// <summary>
        /// Get the current environment givent the uri
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetCurrent(Uri url)
        {
            var parsedQueryString = HttpUtility.ParseQueryString(url.Query);
            bool urlContainsEnvironment = parsedQueryString.AllKeys.Contains(QueryStringKey);
            if (_hostMapping.ContainsKey(url.Authority))
            {

                Tuple<string, bool> hostMapping = _hostMapping[url.Authority];
                if(hostMapping.Item2 && urlContainsEnvironment)
                {
                    return parsedQueryString.GetValues(QueryStringKey).First();
                }
                return hostMapping.Item1;
            }
            if (urlContainsEnvironment)
            {

                return parsedQueryString.GetValues(QueryStringKey).First();
            }
           
            return DefaultEnvironment;
        }
    }
}
