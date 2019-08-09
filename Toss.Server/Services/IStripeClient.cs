﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using Stripe;

namespace Toss.Server.Services {
    public interface IStripeClient {
        Task<bool> Charge (string token, int amount, string description, string email);
    }

    public class StripeClient : IStripeClient {
        private readonly HttpClient httpClient = new HttpClient ();
        public StripeClient (string stripeSecretKey) {
            StripeConfiguration.ApiKey = stripeSecretKey;
        }
        public async Task<bool> Charge (string token, int amount, string description, string email) {

            var chargeOptions = new ChargeCreateOptions () {
                Amount = amount,
                Currency = "eur",
                Description = description,
                Source = token,
                ReceiptEmail = email
            };
            var chargeService = new ChargeService ();
            Charge charge = await chargeService.CreateAsync (chargeOptions);
            return charge.FailureMessage == null && charge.Paid;
        }
    }
}