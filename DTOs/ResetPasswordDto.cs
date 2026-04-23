namespace OpenBooksBackMobile.DTOs
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NuevaContraseña { get; set; }
    }
}
