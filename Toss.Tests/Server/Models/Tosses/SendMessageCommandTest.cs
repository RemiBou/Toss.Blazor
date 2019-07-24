using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{
    public class SendMessageCommandTest : BaseTest
    {
        [Fact]
        public void cannot_send_message_if_not_owner_or_writer()
        {
        }

        [Fact]
        public void cannot_send_message_if_conversation_do_not_exists()
        {

        }

    }
}