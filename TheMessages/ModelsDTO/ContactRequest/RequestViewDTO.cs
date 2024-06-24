using TheMessages.EntityModels;
using TheMessages.ModelsDTO.Users;

namespace TheMessagesWebApi.ModelsDTO.ContactRequest
{
    public class RequestViewDTO
    {
        public Guid Id { get; set; }
        public bool isInput { get; set; }  
        public bool isApplied { get; set; }
        public UserSearchDTO User { get; set; }
        
    
    }
}
