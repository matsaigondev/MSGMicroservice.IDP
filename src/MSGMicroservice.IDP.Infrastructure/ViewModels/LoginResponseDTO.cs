namespace MSGMicroservice.IDP.Infrastructure.ViewModels
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string access_token { get; set; }
    }
}