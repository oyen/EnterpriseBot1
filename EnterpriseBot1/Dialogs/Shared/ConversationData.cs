namespace EnterpriseBot1.Dialogs.Shared
{
    // Defines a state property used to track conversation data.
    public class ConversationData
    {
        public ConversationData(string channelId)
        {
            ChannelId = channelId;
        }

        // The ID of the user's channel.
        public string ChannelId { get; set; }
    }
}
