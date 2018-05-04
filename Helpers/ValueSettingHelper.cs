using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Helpers
{
    public class ValueSettingHelper
    {
        public static void SetCreateValue<T> (T entity) where T : class
        {
            SetUpdateValue(entity);
            var isDeleted = typeof(T).GetProperty("isDeleted");
            if(isDeleted != null)
            {
                isDeleted.SetValue(entity, false);
            }
            
        }

        public static void SetUpdateValue<T>(T entity) where T:class
        {
            var props = typeof(T).GetProperties();
            var updateDate = props.FirstOrDefault(p => p.Name == "UpdateDate");
            if(updateDate != null)
            {
                updateDate.SetValue(entity, DateTime.Now);
            }
            var updaterId = props.FirstOrDefault(p => p.Name == "UpdaterId");
            if(updaterId != null)
            {
                updaterId.SetValue(entity, IdentityHelper.UserId);
            }
            
        }
    }
}