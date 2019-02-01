﻿using MediatR;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Models.Tosses;
using Toss.Shared.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Server.Models.Tosses
{

    public class DeleteTossCommandHandlerTest : BaseTest
    {
        private IAsyncDocumentSession _session;

        public DeleteTossCommandHandlerTest()
        {
            _session = TestFixture.GetInstance<IAsyncDocumentSession>();

        }

        [Fact]
        public async Task Handle_removes_toss()
        {
            await _session.StoreAsync(new TossEntity("test content", "user test", DateTimeOffset.Now));
            await _session.StoreAsync(new TossEntity("test content2", "user test", DateTimeOffset.Now));
            var allInsertedToss = await _session.Query<TossEntity>().ToAsyncEnumerable().ToList();

            await _mediator.Send(new DeleteTossCommand(allInsertedToss.First().Id));


            var allRemaining = await _session.Query<TossEntity>().ToAsyncEnumerable().ToList();
            Assert.Single(allRemaining);
            Assert.Null(allRemaining.FirstOrDefault(t => t.Id == allInsertedToss.First().Id));
        }
    }
}
