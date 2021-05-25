using AuthenticationAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthenticationAPI.UserData
{
    public class SqlUserData : IUserData
    {
        private UsersContext _usersContext;
        public SqlUserData(UsersContext usersContext)
        {
            _usersContext = usersContext;
        }

        public User AddUser(User user)
        {
            user.Id = Guid.NewGuid();
            _usersContext.Users.Add(user);
            _usersContext.SaveChanges();
            return user;
        }

        public void DeleteUser(User user)
        {
            _usersContext.Users.Remove(user);
            _usersContext.SaveChanges();
        }

        public List<User> GetAllUsers()
        {
            return _usersContext.Users.ToList();
        }

        public User GetUserById(Guid id)
        {
            return _usersContext.Users.Find(id);
        }

        public User ModifyUser(User user)
        {
            var existingUser = _usersContext.Users.Find(user.Id);
            if (existingUser != null)
            {
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;
                existingUser.DisplayName = user.DisplayName;

                _usersContext.Users.Update(existingUser);
                _usersContext.SaveChanges();
            }
            return user;
        }
    }
}
