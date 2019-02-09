// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using EnterpriseBot1.Dialogs.Main;
using EnterpriseBot1.Dialogs.Shared;

namespace EnterpriseBot1
{
    /// <summary>
    /// Main entry point and orchestration for bot.
    /// </summary>
    public class EnterpriseBot1 : IBot
    {
        private readonly BotServices _services;
        private readonly ConversationState _conversationState;
        private readonly UserState _userState;
        private DialogSet _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnterpriseBot1"/> class.
        /// </summary>
        /// <param name="botServices">Bot services.</param>
        /// <param name="conversationState">Bot conversation state.</param>
        /// <param name="userState">Bot user state.</param>
        public EnterpriseBot1(BotServices botServices, StateBotAccessors stateBotAccessors)
        {
            _services = botServices ?? throw new ArgumentNullException(nameof(botServices));

            _dialogs = new DialogSet(stateBotAccessors.DialogStateAccessor);
            _dialogs.Add(new MainDialog(_services, stateBotAccessors));
        }

        /// <summary>
        /// Run every turn of the conversation. Handles orchestration of messages.
        /// </summary>
        /// <param name="turnContext">Bot Turn Context.</param>
        /// <param name="cancellationToken">Task CancellationToken.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            // Client notifying this bot took to long to respond (timed out)
            if (turnContext.Activity.Code == EndOfConversationCodes.BotTimedOut)
            {
                _services.TelemetryClient.TrackTrace($"Timeout in {turnContext.Activity.ChannelId} channel: Bot took too long to respond.");
                return;
            }

            var dc = await _dialogs.CreateContextAsync(turnContext);

            if (dc.ActiveDialog != null)
            {
                var result = await dc.ContinueDialogAsync();
            }
            else
            {
                await dc.BeginDialogAsync(nameof(MainDialog));
            }
        }
    }
}
