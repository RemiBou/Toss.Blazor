using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    public class MessageService : IMessageService
    {
        private int onGoingLoad = 0;
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
            this.onGoingLoad++;
            this.OnLoading?.Invoke(this, true);
        }

        public void LoadingDone()
        {
            this.onGoingLoad--;
            if (this.onGoingLoad == 0)
            {
                this.OnLoading?.Invoke(this, false);
            }
        }
    }
}
