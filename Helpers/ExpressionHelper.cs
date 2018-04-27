using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace IntelligenceCloud.Helpers
{
    public class ExpressionHelper
    {
        ////自行建立expression tree，才能在entity framework，使用reflected properties
        public static Expression<Func<TItem,bool>> PropertyEquals<TItem, TValue>(
            PropertyInfo property, TValue value)
        {
            Expression typeFilter = Expression.Constant(value);

            //因為欄位可能是nullable，所以把傳進來的value轉換成nullable，expression才能equal
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var filter = Expression.Constant(Convert.ChangeType(value, property.PropertyType.GetGenericArguments()[0]));
                typeFilter = Expression.Convert(filter, property.PropertyType);
            }

            var param = Expression.Parameter(typeof(TItem));
            var body = Expression.Equal(Expression.Property(param, property) , typeFilter);
                
            //var body = Expression.Equal(Expression.Property(param, property),Expression.Constant(value)); //一般直接用constant(value)即可
            return Expression.Lambda<Func<TItem, bool>>(body, param);
        }

        public static Expression<Func<TItem, bool>> PropertyContain<TItem, TValue>(
            PropertyInfo property, TValue value)
        {
            Expression typeFilter = Expression.Constant(value);

            //因為欄位可能是nullable，所以把傳進來的value轉換成nullable，expression才能equal
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var filter = Expression.Constant(Convert.ChangeType(value, property.PropertyType.GetGenericArguments()[0]));
                typeFilter = Expression.Convert(filter, property.PropertyType);
            }


            var param = Expression.Parameter(typeof(TItem));
            var propertyExp = Expression.Property(param, property);
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var body = Expression.Call(propertyExp, method, typeFilter);

            //var body = Expression.Equal(Expression.Property(param, property),Expression.Constant(value)); //一般直接用constant(value)即可
            return Expression.Lambda<Func<TItem, bool>>(body, param);
        }
    }
}