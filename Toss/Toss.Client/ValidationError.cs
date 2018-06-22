using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Client
{
    /// <summary>
    /// Used for reading ModelStateDictionary from the server without referencing Web.mvc on the client
    /// </summary>
    public class ValidationErrorDictonary : Dictionary<string,List<string>>
    {
    }
}
