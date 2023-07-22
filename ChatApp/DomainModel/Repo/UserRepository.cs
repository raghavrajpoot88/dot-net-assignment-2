using ChatApp.DomainModel.Models;
using ChatApp.DomainModel.Repo.Interfaces;
using ChatApp.MiddleLayer.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ChatApp.DomainModel.Repo
{
    public class UserRepository : IUser
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public ICollection<User> GetUsers()
        {
            return _applicationDbContext.users.OrderBy(u => u.UserId).ToList();
        }
        public User GetUserById(Guid id)
        {
            return _applicationDbContext.users.Where(u => u.UserId == id).FirstOrDefault();
        }

        public void AddUser(User registeredUser)
        {
            _applicationDbContext.users.Add(registeredUser);
            _applicationDbContext.SaveChanges();
        }
        public void ValidateRegistration(UserDTO u)
        {
            try
            {
                //Validations
                if (string.IsNullOrEmpty(u.Email) || u.Email == "string")
                    throw new Exception("Email Required");
                if (string.IsNullOrEmpty(u.Name) || u.Name == "string")
                    throw new Exception("User name Required");
                //if (string.IsNullOrEmpty(registered.Password) || registered.Password == "string")
                //    throw new Exception("Password Required");
                var CheckUserExists = (from user in GetUsers()
                                       where user.Email.Equals(u.Email)
                                       select user).Count();
                if (CheckUserExists > 0)
                {
                    throw new Exception("User Already exist");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public User ValidateLogin(loginDTO login)
        {
            var user =  _applicationDbContext.users.FirstOrDefault(ul => ul.Email == login.Email);
            return user;
        }

    }
}
