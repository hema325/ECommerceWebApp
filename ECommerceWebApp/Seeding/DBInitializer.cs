using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Constrains;
using Microsoft.AspNetCore.Identity;

namespace ECommerceWebApp.Seeding
{
    public class DBInitializer :IDBInitializer
    {
        private readonly IUnitOfWork UnitOfWork;
        private readonly IPasswordHasher<User> PasswordHasher;

        public DBInitializer(IUnitOfWork unitOfWork,IPasswordHasher<User> passwordHasher)
        {
            UnitOfWork = unitOfWork;
            PasswordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            if(await UnitOfWork.Roles.CountAsync() == 0)
            {
                await UnitOfWork.Roles.AddAsync(new Role { Name = Roles.Admin });
                await UnitOfWork.Roles.AddAsync(new Role { Name = Roles.Client });
            }

            var user = new User
            {
                FirstName = "Ibrahim",
                LastName = "Moawad",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                LastSeen = DateTime.Now,
                LockOutEnd = DateTime.Now.AddDays(-1),
                ImgUrl = DefaultImages.User
            };

            if (await UnitOfWork.Users.FindByEmailAsync(user.Email) == null)
            {
                user.Password = PasswordHasher.HashPassword(user, "123456");
                int userId = await UnitOfWork.Users.AddAsync<int>(user);

                var role = await UnitOfWork.Roles.GetRoleByNameAsync(Roles.Admin);
                await UnitOfWork.UserRoles.AddAsync(new UserRole
                {
                    UserId = userId,
                    RoleId = role.Id
                });
            }

        }
    }
}
