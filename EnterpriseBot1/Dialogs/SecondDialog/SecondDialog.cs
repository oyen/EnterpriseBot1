﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using EnterpriseBot1.Dialogs.Shared;
using Microsoft.Bot.Builder.Dialogs;

namespace EnterpriseBot1.Dialogs.Second
{
    public class SecondDialog : ComponentDialog
    {
        private StateBotAccessors _accessors;

        public SecondDialog(BotServices botServices, StateBotAccessors stateBotAccessors)
            : base(nameof(SecondDialog))
        {
            _accessors = stateBotAccessors;

            InitialDialogId = nameof(SecondDialog);

            var escalate = new WaterfallStep[]
            {
                SendPhone,
            };

            AddDialog(new WaterfallDialog(InitialDialogId, escalate));
        }

        private async Task<DialogTurnResult> SendPhone(WaterfallStepContext sc, CancellationToken cancellationToken)
        {
            var conversationState = await _accessors.ConversationDataAccessor.GetAsync(sc.Context, () => null);
            var userProfile = await _accessors.UserProfileAccessor.GetAsync(sc.Context, () => null);

            await sc.Context.SendActivityAsync($"conversationState.ChannelId: {conversationState?.ChannelId}");
            await sc.Context.SendActivityAsync($"userProfile.UserName: {userProfile?.UserName}");

            return await sc.EndDialogAsync();
        }
    }
}
