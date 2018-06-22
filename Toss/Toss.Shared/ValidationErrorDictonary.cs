using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Shared
{
    /// <summary>
    /// Used for reading ModelStateDictionary from the server without referencing Web.mvc on the client
    /// </summary>
    public class ValidationErrorDictonary : Dictionary<string,List<string>>
    {
        public void AddModelError(string key,string msg)
        {
            List<string> msgs;
            if(!TryGetValue(key,out msgs))
            {
                msgs = new List<string>();
                Add(key,msgs);
            }
            msgs.Add(msg);
        }
    }
}
