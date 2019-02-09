namespace EnterpriseBot1.Dialogs.Shared
{
    using System;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;

    // https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-state?view=azure-bot-service-4.0&tabs=csharp
    public class StateBotAccessors
    {
        public StateBotAccessors(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(ConversationDataName);
            UserState = userState ?? throw new ArgumentNullException(UserProfileName);
        }

        public static string UserProfileName { get; } = "UserProfile";

        public static string ConversationDataName { get; } = "ConversationData";

        public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; set; }

        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }

        public ConversationState ConversationState { get; }

        public UserState UserState { get; }

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for ConversationDialogState.
        /// </summary>
        /// <value>
        /// The accessor stores the dialog state for the conversation.
        /// </value>
        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }
    }
}
