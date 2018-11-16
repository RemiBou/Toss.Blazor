using System.Threading.Tasks;

namespace Toss.Server.Services
{
    public interface ICaptchaValidator
    {
        Task Check(string token);
    }
}