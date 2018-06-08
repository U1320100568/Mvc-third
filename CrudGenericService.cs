using IntelligenceCloud.Helpers;
using IntelligenceCloud.Models;
using IntelligenceCloud.Services.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;


namespace IntelligenceCloud.Services
{
    public class CrudGenericService<TEntity> : ICrudService<TEntity>, IDisposable
        where TEntity : class
    {
        private DbContext Db { set; get; }
        public CrudGenericService(): this(new IntelligenceCloudEntities())
        {

        }
        public CrudGenericService(DbContext db)
        {
            if(db ==null)
            {
                throw new ArgumentNullException("db");
            }
            Db = db;
        }
        public CrudGenericService(ObjectContext db)
        {
            if(db == null)
            {
                throw new ArgumentNullException("db");

            }
            Db = new DbContext(db, true);
        }


        public void Create(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            else
            {
                ValueSettingHelper.SetCreateValue<TEntity>(entity);
                Db.Set<TEntity>().Add(entity);
                SaveChanges();
            }
            
        }
        public void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            else
            {
                ValueSettingHelper.SetUpdateValue<TEntity>(entity);
                Db.Entry(entity).State = EntityState.Modified;
                SaveChanges();
            }

        }
        public void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            else
            {
                //如果有isDeleted欄位就設定true，如果沒有就真的刪除
                var isDeletedPro = entity.GetType().GetProperty("isDeleted");
                if(isDeletedPro != null)
                {
                    
                    isDeletedPro.SetValue(entity,true,null)  ;
                    
                    Db.Entry(entity).State = EntityState.Modified;
                    SaveChanges();
                }
                else
                {
                    DeleteComplete(entity);
                }
                
            }

        }
        public void DeleteComplete(TEntity entity)
        {
            Db.Entry(entity).State = EntityState.Deleted;
            SaveChanges();
        }

        public TEntity Get(Expression<Func<TEntity,bool>> predicate)
        {
            return Db.Set<TEntity>().FirstOrDefault(predicate);
        }

        public IQueryable<TEntity> GetAll()
        {
            return MinusDeletedItem();
        }
        
        //排除已刪除的項目
        public IQueryable<TEntity> MinusDeletedItem()
        {
            ////已deleted的項目不能出現
            Type t = typeof(TEntity);
            if (t.GetProperties().Any(p => p.Name == "isDeleted"))
            {
                var prop = t.GetProperty("isDeleted");
                return Db.Set<TEntity>().Where(ExpressionHelper.PropertyEquals<TEntity, bool?>(prop, false)).AsQueryable();
                
            }
            return Db.Set<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> Search(string searchString,string searchProp)
        {
            //要彙整的List
            var partialUnionQueries = new List<IQueryable<TEntity>>();
            //要搜尋的字串List
            List<string> searchList = searchString.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //var props = typeof(TEntity).GetProperties().Where(p => p.PropertyType == typeof(string));
           
            var props = typeof(TEntity).GetProperties().Where(p => searchProp.Contains(p.Name));
            foreach (var searchWord in searchList)
            {

                foreach (var prop in props)
                {
                    partialUnionQueries.Add((MinusDeletedItem().Where(ExpressionHelper.PropertyContain<TEntity, string>(prop, searchWord))));
                    
                }

            }
            //把List彙整成 結果union 結果使用List<IQueryable<TEntity>> .Aggregate()
            var result = partialUnionQueries.Aggregate((a, b) => a.Union(b));


            return result;
        }

        public IQueryable<TEntity> Search(Expression<Func<TEntity, bool>> predicate)
        {
            return MinusDeletedItem().Where(predicate);
        }

        public void SaveChanges()
        {
            
            Db.SaveChanges();
            
           
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Db != null)
                {
                    Db.Dispose();
                    Db = null;
                }
            }
        }
    }
}