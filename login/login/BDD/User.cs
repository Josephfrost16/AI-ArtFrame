namespace login.BDD
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? ConfirmationToken { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;
    }
}
