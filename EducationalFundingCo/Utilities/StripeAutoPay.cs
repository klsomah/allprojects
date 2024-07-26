using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;




namespace EducationalFundingCo.Utilities
{
    public class StripeAutoPay
    {

        public Subscription Subscription1st(string apiKey, string description, string customerEmail, string priceId)
        {
            // Set your Stripe API secret key
            StripeConfiguration.ApiKey = apiKey; // _config["Stripe:SecretKey"];

            var customerOptions = new CustomerCreateOptions
            {
                Description = description,
                Email = customerEmail,
            };

            var customerService = new CustomerService();
            var customer = customerService.Create(customerOptions);

            // Set the start date for the subscription
            var startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            // Create a new subscription
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = priceId,
                    }
                },
                BillingCycleAnchor = startDate,
            };

            var subscriptionService = new SubscriptionService();
            var subscription = subscriptionService.Create(subscriptionOptions);

            return subscription;
        }

        public Subscription Subscription15th(string apiKey, string description, string customerEmail, string priceId)
        {
            // Set your Stripe API secret key
            StripeConfiguration.ApiKey = apiKey; // _config["Stripe:SecretKey"];

            var customerOptions = new CustomerCreateOptions
            {
                Description = description,
                Email = customerEmail,
            };

            var customerService = new CustomerService();
            var customer = customerService.Create(customerOptions);

            // Set the start date for the subscription
            var startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 15);

            // Create a new subscription
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = priceId,
                    }
                },
                BillingCycleAnchor = startDate,
            };

            var subscriptionService = new SubscriptionService();
            var subscription = subscriptionService.Create(subscriptionOptions);

            return subscription;
        }
    }
}
