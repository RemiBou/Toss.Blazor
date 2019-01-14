using System;

namespace Toss.Server.Services
{
    /// <summary>
    /// This interface gives the ability to fake the current date during tests
    /// </summary>
    public interface INow
    {
        DateTimeOffset Get();
    }

    public class FakeNow : INow
    {
        public static DateTimeOffset? Current { get; set; }
        public DateTimeOffset Get()
        {
            if (!Current.HasValue)
            {
                return DateTimeOffset.Now;
            }
            return Current.Value;
        }
    }
}