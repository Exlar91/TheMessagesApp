using TheMessages.EntityModels;

namespace TheMessagesWebApi.ModelsDTO.ContactRequest
{
    public class RequestViewDTO
    {
        public Guid Id { get; set; }
        public bool isInput { get; set; }        
        public int userId { get; set; }
        public string userDisplayName { get; set; }
        public string userCity { get; set; }
    }
}
