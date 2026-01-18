using LoggAutorz.Users;
namespace LoggAutorz.Repositorie
{
    public static class UserRepository
    {
        public static UsersEntity Get(string UserName, string Password) {
            var UserRepo = new List<UsersEntity> {
        new UsersEntity { UserName = "Eduardo", PasswordHash = "eduardo123" },
        new UsersEntity { UserName = "Emilly", PasswordHash = "Empregado" }
        };
           return UserRepo.Where(u => u.UserName == UserName && u.PasswordHash == Password).FirstOrDefault();

        }
    }
}
