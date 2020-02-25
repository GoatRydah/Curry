using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Curry.DataAccess.Data.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //Get object by ID
        T Get(int id);

        //Get all objects as IEnumerable
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null);

        //Get first or default object
        T GetFirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null);

        //Add object 
        void Add(T entity);

        //Remove by ID
        void Remove(int id);

        //Remove by object
        void Remove(T entity);

        //Remove list of objects
        void RemoveRange(IEnumerable<T> entity);
    }
}
