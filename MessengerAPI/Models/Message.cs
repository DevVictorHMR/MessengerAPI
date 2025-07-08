namespace MessengerAPI.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public int SenderId { get; set; }
        public virtual User Sender { get; set; } = null!;

        public int ReceiverId { get; set; }
        public virtual User Receiver { get; set; } = null!;
    }
}