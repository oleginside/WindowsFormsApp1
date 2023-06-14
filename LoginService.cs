using System.Collections.Generic;

namespace LoginFormExample
{
    public class LoginService
    {
        private Dictionary<string, string> whitelist;

        public LoginService()
        {
            // Здесь можно добавить пользователей в whitelist
            whitelist = new Dictionary<string, string>
            {
                { "user1", "password1" },
                { "user2", "password2" },
                { "user3", "password3" }
            };
        }

        public bool Login(string username, string password)
        {
            // Проверяем, есть ли пользователь с указанным логином в whitelist
            if (whitelist.ContainsKey(username))
            {
                // Проверяем, соответствует ли введенный пароль паролю пользователя из whitelist
                if (whitelist[username] == password)
                {
                    return true; // Верный логин и пароль
                }
            }

            return false; // Неверный логин или пароль
        }
    }
}
