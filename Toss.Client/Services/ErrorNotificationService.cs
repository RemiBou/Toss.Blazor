using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    public class ExceptionNotificationService : TextWriter, IExceptionNotificationService
    {
        private TextWriter _decorated;

        public override Encoding Encoding => Encoding.UTF8;

        /// <summary>
        /// Raised is an exception occurs. The exception message will be send to the listeners
        /// </summary>
        public event EventHandler<string> OnException;

        public ExceptionNotificationService()
        {
            _decorated = Console.Error;
            Console.SetError(this);
        }
        //THis is the method called by Blazor
        public override void WriteLine(string value)
        {
            OnException?.Invoke(this,value);

            _decorated.WriteLine(value);
        }
    }
}
