using BCrypt.Net;
using login.BDD;  // Asegúrate de que estás usando el espacio de nombres correcto
using Microsoft.EntityFrameworkCore;

namespace login.Servicios
{
    public class UserService
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Register(string name, string email, string password)
        {
            // Validar dominio de correo
            if (!IsAllowedDomain(email))
            {
                throw new Exception("El dominio del correo no está permitido.");
            }

            // Verificar si el usuario ya existe
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                throw new Exception("El correo ya está registrado.");
            }

            // Crear un nuevo usuario
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = hashedPassword,
                IsEmailConfirmed = true // Configuramos como confirmado automáticamente
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User?> Login(string email, string password)
        {
            // Buscar usuario por correo
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            // Validar usuario y contraseña
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }

        private bool IsAllowedDomain(string email)
        {
            // Lista de dominios permitidos
            var allowedDomains = new[] { "gmail.com", "hotmail.com", "yahoo.com" };

            // Validar el dominio del correo
            var domain = email.Split('@').LastOrDefault();
            return domain != null && allowedDomains.Contains(domain.ToLower());
        }
    }
}
