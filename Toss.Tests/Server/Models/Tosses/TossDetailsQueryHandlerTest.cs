using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Shared.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{
    public class TossDetailsQueryHandlerTest : BaseCosmosTest
    {
        [Fact]
        public async Task returns_details_of_single_toss()
        {
            TossCreateCommand request = new TossCreateCommand()
            {
                Content = "lalalaalalalam z emlazek amzk mlekmzalkazmel zaml azme #test"
            };
            await _mediator.Send(request);
            var lastToss = await _mediator.Send(new TossLastQuery("test"));

            var res = await _mediator.Send(new TossDetailsQuery(lastToss.First().Id));

            Assert.Equal(request.Content, res.Content);
            Assert.Equal(lastToss.First().CreatedOn, res.CreatedOn);
            Assert.Equal(lastToss.First().UserName, res.UserName);
        }

        [Fact]
        public async Task returns_null_if_does_not_exists()
        {
            var res = await _mediator.Send(new TossDetailsQuery("iamascatman"));

            Assert.Null(res);
        }
    }
}
