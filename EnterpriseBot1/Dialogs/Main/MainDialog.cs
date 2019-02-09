// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseBot1.Dialogs.Escalate;
using EnterpriseBot1.Dialogs.Onboarding;
using EnterpriseBot1.Dialogs.Shared;
using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace EnterpriseBot1.Dialogs.Main
{
    public class MainDialog : RouterDialog
    {
        private BotServices _services;
        private StateBotAccessors _accessors;

        private MainResponses _responder = new MainResponses();

        public MainDialog(BotServices services, StateBotAccessors stateBotAccessors)
            : base(nameof(MainDialog))
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _accessors = stateBotAccessors;

            AddDialog(new OnboardingDialog(_services, _accessors.UserState.CreateProperty<OnboardingState>(nameof(OnboardingState))));
            AddDialog(new EscalateDialog(_services, _accessors));
        }

        protected override async Task OnStartAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var context = dc.Context;

            // set userprofile and conversationdata
            await _accessors.ConversationDataAccessor.SetAsync(context, new ConversationData(dc.Context.Activity.ChannelId));
            await _accessors.UserProfileAccessor.SetAsync(context, new UserProfile("Elena of Avalor", "crownedprincess@avalor.com", "111-222-3333"));

            var view = new MainResponses();
            await view.ReplyWith(context, MainResponses.ResponseIds.Intro);
        }

        protected override async Task RouteAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            // for testing
            if (dc.Context.Activity.Text == "trigger escalate")
            {
                await dc.BeginDialogAsync(nameof(EscalateDialog));
                return;
            }

            // Check dispatch result
            var dispatchResult = await _services.DispatchRecognizer.RecognizeAsync<Dispatch>(dc.Context, true, CancellationToken.None);
            var intent = dispatchResult.TopIntent().intent;

            if (intent == Dispatch.Intent.l_General)
            {
                // If dispatch result is general luis model
                _services.LuisServices.TryGetValue("general", out var luisService);

                if (luisService == null)
                {
                    throw new Exception("The specified LUIS Model could not be found in your Bot Services configuration.");
                }
                else
                {
                    var result = await luisService.RecognizeAsync<General>(dc.Context, true, CancellationToken.None);

                    var generalIntent = result?.TopIntent().intent;

                    // switch on general intents
                    switch (generalIntent)
                    {
                        case General.Intent.Greeting:
                            {
                                // send greeting response
                                await _responder.ReplyWith(dc.Context, MainResponses.ResponseIds.Greeting);
                                break;
                            }

                        case General.Intent.Help:
                            {
                                // send help response
                                await _responder.ReplyWith(dc.Context, MainResponses.ResponseIds.Help);
                                break;
                            }

                        case General.Intent.Cancel:
                            {
                                // send cancelled response
                                await _responder.ReplyWith(dc.Context, MainResponses.ResponseIds.Cancelled);

                                // Cancel any active dialogs on the stack
                                await dc.CancelAllDialogsAsync();
                                break;
                            }

                        case General.Intent.Escalate:
                            {
                                // start escalate dialog
                                await dc.BeginDialogAsync(nameof(EscalateDialog));
                                break;
                            }

                        case General.Intent.None:
                        default:
                            {
                                // No intent was identified, send confused message
                                await _responder.ReplyWith(dc.Context, MainResponses.ResponseIds.Confused);
                                break;
                            }
                    }
                }
            }
            else if (intent == Dispatch.Intent.q_FAQ)
            {
                _services.QnAServices.TryGetValue("faq", out var qnaService);

                if (qnaService == null)
                {
                    throw new Exception("The specified QnAMaker Service could not be found in your Bot Services configuration.");
                }
                else
                {
                    var answers = await qnaService.GetAnswersAsync(dc.Context);

                    if (answers != null && answers.Count() > 0)
                    {
                        await dc.Context.SendActivityAsync(answers[0].Answer);
                    }
                }
            }
        }

        protected override async Task OnEventAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Check if there was an action submitted from intro card
            if (dc.Context.Activity.Value != null)
            {
                dynamic value = dc.Context.Activity.Value;
                if (value.action == "startOnboarding")
                {
                    await dc.BeginDialogAsync(nameof(OnboardingDialog));
                    return;
                }
            }
        }

        protected override async Task CompleteAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            // The active dialog's stack ended with a complete status
            await _responder.ReplyWith(dc.Context, MainResponses.ResponseIds.Completed);
        }
    }
}
