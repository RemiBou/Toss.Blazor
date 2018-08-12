using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Data;
using Toss.Server.Models.Tosses;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Shared.Tosses
{
    [Collection("CosmosDBFixture")]
    public class TossListAdminQueryHandlerTest : BaseCosmosTest, IClassFixture<CosmosDBFixture>
    {
        private CosmosDBTemplate<TossEntity> _tossCosmosDB;
        private TossListAdminQueryHandler _sut;
        public TossListAdminQueryHandlerTest(CosmosDBFixture cosmosDBFixture) : base(cosmosDBFixture)
        {
            _tossCosmosDB = new CosmosDBTemplate<TossEntity>(_client, CosmosDBFixture.DatabaseName);
            _sut = new TossListAdminQueryHandler(_tossCosmosDB);
        }

        [Fact]
        public async Task Handle_returns_toss_count()
        {
            for (int i = 0; i < 59; i++)
            {
                await _tossCosmosDB.Insert(new TossEntity("test", "test", DateTimeOffset.Now));
            }

            var res = await _sut.Handle(new Toss.Shared.Tosses.TossListAdminQuery(), new System.Threading.CancellationToken());

            Assert.Equal(59, res.Count);
        }

        [Fact]
        public async Task Handle_return_last_items()
        {
            for (int i = 0; i < 27; i++)
            {
                await _tossCosmosDB.Insert(new TossEntity("test", "test", DateTimeOffset.Now.AddDays(-i)));
            }

            var res = await _sut.Handle(new Toss.Shared.Tosses.TossListAdminQuery(15, null), new System.Threading.CancellationToken());

            Assert.Equal(15, res.Result.Count());
            Assert.Null(res.Result.FirstOrDefault(r => r.CreatedOn <= DateTimeOffset.Now.AddDays(-15)));
        }

        [Fact]
        public async Task Handle_filter_on_max_date()
        {
            for (int i = 0; i < 27; i++)
            {
                await _tossCosmosDB.Insert(new TossEntity("test", "test", DateTimeOffset.Now.AddDays(-i)));
            }

            var res = await _sut.Handle(new Toss.Shared.Tosses.TossListAdminQuery(15, DateTimeOffset.Now.AddDays(-14)), new System.Threading.CancellationToken());

            Assert.Equal(13, res.Result.Count());
            Assert.Null(res.Result.FirstOrDefault(r => r.CreatedOn > DateTimeOffset.Now.AddDays(-14)));
        }

        [Fact]
        public async Task Handle_map_fields()
        {
            TossEntity instance = new TossEntity("lorem ipsum", "test", DateTimeOffset.Now);
            await _tossCosmosDB.Insert(instance);

            var res = await _sut.Handle(new Toss.Shared.Tosses.TossListAdminQuery(15, null), new System.Threading.CancellationToken());

            var first = res.Result.FirstOrDefault();

            Assert.Equal("lorem ipsum", first.Content);
            Assert.Equal(instance.CreatedOn, first.CreatedOn);
            Assert.Equal("test", first.UserName);
            Assert.NotNull(first.Id);
        }
    }
}
