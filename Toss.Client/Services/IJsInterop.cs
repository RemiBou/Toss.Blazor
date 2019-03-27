using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Toss.Client.Services
{
    public interface IJsInterop
    {
        Task AjaxLoaderHide(int id);
        Task<int> AjaxLoaderShow(ElementRef elementRef);
        Task<string> Captcha(string actionName);
        Task<string> GetCookie();
        Task<string> GetFileData(ElementRef fileInputRef);
        Task HideModal(ElementRef elementRef);
        Task<string[]> Languages();
        Task OpenStripe(IStripeCallBack stripeCallBack, int amountInCts);
        Task ShowModal(ElementRef elementRef);
        Task ShowModal(ElementRef elementRef, IModalCloseCallback closeCallback);
        Task Toastr(string toastType, string message);
    }
}