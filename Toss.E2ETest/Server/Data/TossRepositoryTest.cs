using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Toss.Server.Data;
using Xunit;

namespace Toss.Tests.Server.Data
{
    public class TossAzureTableRepositoryTest
    {
        private TossAzureTableRepository _sut;
        public TossAzureTableRepositoryTest()
        {
            _sut = new TossAzureTableRepository("UseDevelopmentStorage=true;");
        }
        [Fact]
        public async Task last_returns_last_items_from_table_ordered_desc_by_createdon()
        {

            var res = await _sut.Last();
        }

        [Fact]
        public async Task last_return_field_mapped_to_toss_last_item()
        {

        }

        [Fact]
        public async Task create_insert_item_in_azure_table()
        {

        }
    }
}
