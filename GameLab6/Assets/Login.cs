using System;
using UnityEngine;

namespace Login
{
    [Serializable]
    public class RequestLoginData
    {
        public string email;
        public string password;
        public string twoFactorCode = "";
        public string twoFacterRecoveryCode = "";

        public RequestLoginData(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }
    [Serializable]
    public class ResponseLogin
    {
        public bool isSuccess;
        public string notification;
        public LoginUserData data;
    }
    [Serializable]
    public class LoginUserData
    {
        public string token;
        public User user;
    }
    [Serializable]
    public class User
    {
        public string id;
        public string email;
        public string name;
        public string avatar;
        public int regionId;
    }
}
