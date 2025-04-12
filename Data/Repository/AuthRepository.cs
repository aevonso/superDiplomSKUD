using AutorizationDomain;
using AutorizationDomain.Queries.Object;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly Connection _connection;

        public AuthRepository(Connection connection)
        {
            _connection = connection;
        }

        public Employeer? Autorization(string login, string password)
        {
            var employeer = _connection.Employees
                                       .AsNoTracking()
                                       .Include(row => row.Post)
                                       .FirstOrDefault(row => row.Login.ToLower() == login.ToLower() &&
                                                                row.Password == password);
            if(employeer!=null)
            {
                Employeer user = new Employeer();
                user.Login = login;
                user.Roles = new List<string> { employeer.Post.Name };
                return user;
            }
            return null;
        }
    }
}
