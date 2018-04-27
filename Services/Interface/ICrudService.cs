using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace IntelligenceCloud.Services.Interface
{
    public interface ICrudService<TEntity> where TEntity:class
    {
        void Create(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        void DeleteComplete(TEntity entity);

        TEntity Get(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> MinusDeletedItem();

        void SaveChanges();

        
    }
}