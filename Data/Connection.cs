using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Tables.Data.Tables;
using Data.Tables;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class Connection : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<AccessMatrix> AccessMatrices { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<MobileDevice> MobileDevices { get; set; }
        public DbSet<PointOfPassage> PointOfPassages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<QrCode> QrCodes { get; set; } = null!;


        public Connection(DbContextOptions<Connection> options) : base(options)
        {

        }


    }
}