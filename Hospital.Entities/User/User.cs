


using BCrypt.Net;
using System;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hospital.Entities.User
{


    public class User
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }

        public string PasswordHash { get; private set; }


        private User() { }

        public User(string fullName, string email,string passwordHash,string phone)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new Exception("FullName cannot be empty");

            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email cannot be empty");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new Exception("PasswordHash cannot be empty");

            Id = Guid.NewGuid();
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            Phone= phone;
        }


        public void ChangePassword(string newPassword)
        {
            if(string.IsNullOrWhiteSpace(newPassword))
              throw new Exception("New password cannot be empty");
            PasswordHash =BCrypt.Net.BCrypt.HashPassword(newPassword);

        }







    }


}
