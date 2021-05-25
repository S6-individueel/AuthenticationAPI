using AuthenticationAPI.Models;
using System;
using System.Collections.Generic;

namespace AuthenticationAPI.UserData
{
    public interface IUserData
    {
        List<User> GetAllUsers();

        User GetUserById(Guid id);

        User AddUser(User user);

        void DeleteUser(User user);

        User ModifyUser(User user);
    }
}
