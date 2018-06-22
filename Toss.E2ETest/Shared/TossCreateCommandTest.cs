using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Toss.Shared;
using Toss.Tests.Infrastructure;
using Xunit;

namespace Toss.Tests.Shared
{
    public class TossCreateCommandTest
    {

        [Fact]
        public void invalid_if_length_over_32000()
        {
            var res = new List<ValidationResult>();
            var sut = new TossCreateCommand()
            {
                Content = string.Join("", Enumerable.Range(0, 3201).Select(i => "lorem ipsu"))
            };
            Assert.Equal(32010, sut.Content.Length);

            bool isValid = ValidationHelper.IsValid(res, sut);
            Assert.False(isValid);
            Assert.Contains(res, r => r.MemberNames.Contains("Content"));

        }


    }
}
