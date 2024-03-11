using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class LoginService
    {
        private LoginRepo repo;

        public LoginService()
        {
            repo = new LoginRepo();
        }

        public Login ValidateUser(string username, string password)
        {
            return repo.ValidateUser(username, password);
        }
    }
}
