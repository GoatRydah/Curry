using Curry.DataAccess.Data.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Curry.DataAccess.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        //Grabs a connection to the actual db to connect to this class
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
        }

        public ICategoryRepository Category { get; private set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
