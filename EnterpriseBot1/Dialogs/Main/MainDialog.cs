// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using EnterpriseBot1.Dialogs.Escalate;
using EnterpriseBot1.Dialogs.Shared;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EnterpriseBot1.Dialogs.Main
{
    public class MainDialog : RouterDialog
    {
        private BotServices _services;
        private StateBotAccessors _accessors;

        public MainDialog(BotServices services, StateBotAccessors stateBotAccessors)
            : base(nameof(MainDialog))
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _accessors = stateBotAccessors;

            AddDialog(new EscalateDialog(_services, _accessors));
        }

        protected override async Task OnStartAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var context = dc.Context;

            // set userprofile and conversationdata
            await _accessors.ConversationDataAccessor.SetAsync(context, new ConversationData(dc.Context.Activity.ChannelId));
            await _accessors.UserProfileAccessor.SetAsync(context, new UserProfile("Elena of Avalor", "crownedprincess@avalor.com", "111-222-3333"));

            await dc.Context.SendActivityAsync($"ConversationData and UserProfile were set in MainDialog.OnStartAsync()");

            await ReplyPrintOutState(dc.Context);
        }

        protected override async Task RouteAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            // for testing
            if (dc.Context.Activity.Text == "print out state")
            {
                await dc.BeginDialogAsync(nameof(EscalateDialog));
                return;
            }

            else
            {
                await ReplyPrintOutState(dc.Context);
            }
        }

        protected override async Task OnEventAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
        }

        protected override async Task CompleteAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            // The active dialog's stack ended with a complete status
            await dc.Context.SendActivityAsync($"Completed");
        }

        private async Task ReplyPrintOutState(ITurnContext context)
        {
            var reply = context.Activity.CreateReply();
            reply.Text = "Say 'print out state' or click the button.";
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                    {
                        new CardAction(type: ActionTypes.ImBack, title: "Test State", value: "print out state"),
                    }
            };
            await context.SendActivityAsync(reply);
        }
    }
}
