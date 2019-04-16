using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Toss.Client.Services
{
    public interface IJsInterop
    {
        Task<string> Captcha(string actionName);
        Task<string> GetCookie();
        Task HideModal(ElementRef elementRef);
        Task<string[]> Languages();
        Task OpenStripe(IStripeCallBack stripeCallBack, int amountInCts);
        Task ShowModal(ElementRef elementRef);
        Task ShowModal(ElementRef elementRef, IModalCloseCallback closeCallback);
    }
}