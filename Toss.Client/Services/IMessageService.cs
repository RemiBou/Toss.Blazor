using System;

namespace Toss.Client.Services
{
    public interface IMessageService
    {
        event EventHandler<string> OnError;
        event EventHandler<string> OnInfo;
        event EventHandler<bool> OnLoading;

        void Error(string message);
        void Info(string message);
        void Loading();
        void LoadingDone();
    }
}