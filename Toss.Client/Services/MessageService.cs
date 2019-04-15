using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    public class MessageService : IMessageService
    {
        public event EventHandler<string> OnInfo;

        public event EventHandler<string> OnError;
        public event EventHandler<bool> OnLoading;

        public void Info(string message)
        {
            this.OnInfo?.Invoke(this, message);
        }

        public void Error(string message)
        {

            this.OnError?.Invoke(this, message);
        }

        public void Loading()
        {
            this.OnLoading.Invoke(this, true);
        }

        public void LoadingDone()
        {
            this.OnLoading.Invoke(this, false);
        }
    }
}
