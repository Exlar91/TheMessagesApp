namespace TheMessages.ModelsDTO.Users
{
    public class UserSearchDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string DisplayName { get; set; }       
        public bool CurrentUser { get; set; }
        public string City { get; set; }
        public bool RequestSended { get; set; }
        public bool RequestApllied { get; set; }
        public bool RequestRecieced { get; set; }
    }
}
