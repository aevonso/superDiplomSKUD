using AutorizationDomain;
using AutorizationDomain.Queries.Object;
using Data.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Data.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly Connection _db;
        public AuthRepository(Connection db) => _db = db;

        public Employeer? Autorization(string login, string password)
        {
            var emp = _db.Employees
                         .AsNoTracking()
                         .Include(e => e.Post)
                         .FirstOrDefault(e => e.Login == login && e.Password == password);
            if (emp == null) return null;
            return new Employeer
            {
                Login = emp.Login,
                Roles = new[] { emp.Post.Name }
            };
        }
        public DateTime? GetRefreshExpiry(string refreshToken)
        {
            return _db.Employees
                      .Where(e => e.RefreshToken == refreshToken)
                      .Select(e => e.RefreshTokenExpiryTime)
                      .FirstOrDefault();
        }
        public void UpdateRefreshToken(string login, string refreshToken, DateTime expiry)
        {
            var emp = _db.Employees.Single(e => e.Login == login);
            emp.RefreshToken = refreshToken;
            emp.RefreshTokenExpiryTime = expiry;
            _db.SaveChanges();
        }



        public Employeer? GetByRefreshToken(string refreshToken)
        {
            var emp = _db.Employees
                      .AsNoTracking()
                      .Include(e => e.Post)
                      .FirstOrDefault(e => e.RefreshToken == refreshToken);
            if (emp == null) return null;
            return new Employeer
            {
                Login = emp.Login,
                Roles = new[] { emp.Post.Name }
            };
        }
    }
}
