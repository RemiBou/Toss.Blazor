using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Toss.Shared.Tosses;
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
                Content = string.Join("", Enumerable.Range(0, 3201).Select(i => "lorem #psu"))
            };
            Assert.Equal(32010, sut.Content.Length);

            bool isValid = ValidationHelper.IsValid(res, sut);
            Assert.False(isValid);
            Assert.Contains(res, r => r.MemberNames.Contains("Content"));

        }
        [Fact]
        public void cannot_send_toss_without_hashtag()
        {
            var res = new List<ValidationResult>();
            var sut = new TossCreateCommand()
            {
                Content = "lorem ipsum lrorem peozeproizeri zerizeporizop eir "
            };

            bool isValid = ValidationHelper.IsValid(res, sut); 

            Assert.False(isValid);


            sut = new TossCreateCommand()
            {
                Content = "lorem ipsum #lrorem peozeproizeri zerizeporizop eir "
            };

            isValid = ValidationHelper.IsValid(res, sut); 

            Assert.True(isValid);
        }

        [Fact]
        public void can_send_toss_with_new_line()
        {
            var res = new List<ValidationResult>();
            var sut = new TossCreateCommand()
            {
                Content = @"lorem ipsum lrorem peozeproizeri
zerizeporizop eir
zerzerezr 
#toto"
            };
            bool isValid = ValidationHelper.IsValid(res, sut);

            Assert.True(isValid);
        }

    }
}
