using ChatApp.DomainModel.Models;
using ChatApp.MiddleLayer.DTOs;

namespace ChatApp.DomainModel.Repo.Interfaces
{
    public interface IUser
    {
        public ICollection<User> GetUsers();

        public User GetUserById(Guid id);

        public void AddUser(User registeredUser);

        public void ValidateRegistration(UserDTO user);

        public User ValidateLogin(loginDTO login);
       
    }
}
