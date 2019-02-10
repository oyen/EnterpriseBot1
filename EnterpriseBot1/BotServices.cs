// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.ApplicationInsights;
using Microsoft.Bot.Configuration;

namespace EnterpriseBot1
{
    /// <summary>
    /// Represents references to external services.
    ///
    /// For example, LUIS services are kept here as a singleton.  This external service is configured
    /// using the <see cref="BotConfiguration"/> class.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    /// <seealso cref="https://www.luis.ai/home"/>
    public class BotServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotServices"/> class.
        /// </summary>
        /// <param name="botConfiguration">The <see cref="BotConfiguration"/> instance for the bot.</param>
        public BotServices(BotConfiguration botConfiguration)
        {
            foreach (var service in botConfiguration.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.AppInsights:
                        {
                            var appInsights = service as AppInsightsService;
                            TelemetryClient = new TelemetryClient();
                            break;
                        }

                    case ServiceTypes.Generic:
                        {
                            if (service.Name == "Authentication")
                            {
                                var authentication = service as GenericService;

                                if (!string.IsNullOrEmpty(authentication.Configuration["Azure Active Directory v2"]))
                                {
                                    AuthConnectionName = authentication.Configuration["Azure Active Directory v2"];
                                }
                            }

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Gets the set of the Authentication Connection Name for the Bot application.
        /// </summary>
        /// <remarks>The Authentication Connection Name  should not be modified while the bot is running.</remarks>
        /// <value>
        /// A string based on configuration in the .bot file.
        /// </value>
        public string AuthConnectionName { get; }

        /// <summary>
        /// Gets the set of AppInsights Telemetry Client used.
        /// </summary>
        /// <remarks>The AppInsights Telemetry Client should not be modified while the bot is running.</remarks>
        /// <value>
        /// A <see cref="TelemetryClient"/> client instance created based on configuration in the .bot file.
        /// </value>
        public TelemetryClient TelemetryClient { get; }

    }
}
