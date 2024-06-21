namespace TheMessages.EntityModels
{
    public class FriendRequest
    {
        public Guid Id { get; set; }
        public AppUser From { get; set; }
        public AppUser To { get; set; }
        public bool IsApplied { get; set; }
    }
}
