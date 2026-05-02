using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;

namespace CamSistemDataLayer.Models.GenericRepo
{
    public class GenericRepository<T> : IGenericRepository<T>
       where T : class, new()
    {
        internal CamSistemDbEntities _entities;

        public GenericRepository(CamSistemDbEntities entities)
        {
            _entities = entities;
        }

        public GenericRepository()
        {
            _entities = new CamSistemDbEntities();
        }
        
        public virtual IQueryable<T> GetAll()
        {
            var query = _entities.Set<T>();
            return query;
        }

        public IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            var query = _entities.Set<T>().Where(predicate);
            return query;
        }        

        public virtual T Add(T entity)
        {
            try
            {
                _entities.Set<T>().Add(entity);
                return entity;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual void Delete(T entity)
        {
            try
            {
                _entities.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                _entities.Set<T>().Remove(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void DeleteAllAndSave(IQueryable<T> entitys)
        {
            try
            {
                _entities.Set<T>().RemoveRange(entitys);
                Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public virtual void Edit(T entity)
        {
            _entities.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }

        public virtual void Save()
        {
            try
            {
                _entities.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Trace.TraceInformation(string.Format("Entity türü \"{0}\" şu hatalara sahip \"{1}\" Geçerlilik hataları:", eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Trace.TraceInformation(string.Format("- Özellik: \"{0}\", Hata: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }
            }
        }

        public virtual T SaveAndReturnEntity(T entity)
        {
            try
            {
                Add(entity);
                Save();
                return entity;
            } 
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void AddAndSave(T entity)
        {
            try
            {
                Add(entity);
                Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void DeleteAndSave(T entity)
        {
            try
            {
                Delete(entity);
                Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void EditAndSave(T entity)
        {
            try
            {
                Edit(entity);
                Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual T EditSaveAndReturnEntity(T entity)
        {
            try
            {
                Edit(entity);
                Save();
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
