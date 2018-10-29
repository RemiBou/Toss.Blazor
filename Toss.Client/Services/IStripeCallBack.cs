using System.Threading.Tasks;

namespace Toss.Client.Services
{
    public interface IStripeCallBack
    {
        Task TokenReceived(string token);
    }
}