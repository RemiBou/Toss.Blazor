using System;

namespace Toss.Client.Services
{
    public interface IExceptionNotificationService
    {
        event EventHandler<string> OnException;
    }
}