using System;
using System.Linq;
using System.Linq.Expressions;

namespace CamSistemDataLayer.Models.GenericRepo
{
    interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        T Add(T entity);
        void Delete(T entity);
        void Edit(T entity);
        void Save();
        T SaveAndReturnEntity(T entity);
        void AddAndSave(T entity);
        void EditAndSave(T entity);
        void DeleteAndSave(T entity);
        T EditSaveAndReturnEntity(T entity);
        void DeleteAllAndSave(IQueryable<T> entitys);
    }
}
