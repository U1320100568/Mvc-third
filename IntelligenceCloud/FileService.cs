using IntelligenceCloud.Helpers;
using IntelligenceCloud.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace IntelligenceCloud.Services
{
    public class FileService<TEntity> : CrudRepository<TEntity>
        where TEntity : class  
    {
        private CrudRepository<TEntity> repository;

        public FileService()
        {
            repository = new CrudRepository<TEntity>();
        }
        //用viewModel為了通用 不同的class
        //此處viewModel 可能包含HttpPostFileBase物件
        public void CreateViewModelToDatabase<TViewModel>(TViewModel viewModel)
        {
            

            SaveHttpPostFile(viewModel, "~/UploadFiles");

            var prop = viewModel.GetType().GetProperties().FirstOrDefault(p => p.PropertyType== typeof(List<TEntity>) );
            if(prop != null)
            {
                var fileNames = prop.GetValue(viewModel) as List<TEntity>;
                foreach(var item in fileNames)
                {   
                    repository.Create(item);
                    
                }
            }
        }

        //將檔案存到路徑下 
        public void SaveHttpPostFile(Object viewModel, string storedFolder )
        {
            //在ViewModel取出欄位叫 HttpPostedFileBase[]
            var prop = viewModel.GetType().GetProperties().FirstOrDefault(p => p.PropertyType == typeof(HttpPostedFileBase[]));
            var propFileNames = viewModel.GetType().GetProperty("AttachDetails");


            if (prop != null)
            {
                var httpPosts = prop.GetValue(viewModel) as HttpPostedFileBase[];

                //在不知道型別時，要先建立實體，才能加入List<T> ，若知道型別，直接建立List<型別>即可
                object instance = Activator.CreateInstance(propFileNames.PropertyType);
                IList list = (IList)instance;

                //可能多個檔案上傳
                foreach (var httpPost in httpPosts)
                {
                    if (!String.IsNullOrEmpty(httpPost.FileName)  && httpPost.ContentLength>0)
                    {
                        var fileName = Path.GetFileName(httpPost.FileName);
                        var path = Path.Combine(HttpContext.Current.Server.MapPath(storedFolder), fileName);
                        Regex regex = new Regex("\\.[^.]*$", RegexOptions.IgnoreCase);  //找出副檔名
                        Match match = regex.Match(httpPost.FileName);

                        //如果不存在資料夾就建立一個
                        if (!Directory.Exists(Path.GetDirectoryName(path)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(path));
                        }

                        ////檢查檔名是否重複，若有重複檔名後加上(2)，以此類推編號
                        int fileExtandNum = 0;
                        string fileNameTmp = "";
                        while (System.IO.File.Exists(path))
                        {
                            fileExtandNum++;

                            fileNameTmp = fileName.Replace(match.Value.ToString(), "") + "(" + fileExtandNum + ")" + match.Value;
                            path = Path.Combine(HttpContext.Current.Server.MapPath(storedFolder), fileNameTmp);

                            
                        }
                        fileName = fileNameTmp;
                        httpPost.SaveAs(path);

                        

                        
                        TEntity entity = Activator.CreateInstance(typeof(TEntity)) as TEntity;

                        entity.GetType().GetProperty("AttachmentName").SetValue(entity, fileName);
                        entity.GetType().GetProperty("AttachmentPath").SetValue(entity, storedFolder);
                        entity.GetType().GetProperty("AttachmentType").SetValue(entity, match.Success ? match.Value : "unknown");
                        entity.GetType().GetProperty("isDeleted").SetValue(entity, false);
                        entity.GetType().GetProperty("MemberId").SetValue(entity, IdentityHelper.MemberId);

                        list.Add(entity); //將檔案class加入List
                        
                    }
                }

            propFileNames.SetValue(viewModel, list);// viewmodel裡面的propFileNames欄位 ，存入值
            }

            
            
        }

        public byte[] Download(TEntity entity)
        {
            var path = typeof(TEntity).GetProperty("AttachmentPath").GetValue(entity).ToString();
            var fileName = typeof(TEntity).GetProperty("AttachmentName").GetValue(entity).ToString();
            
            string filePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);
            //找不到檔案
            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }
            
                
            ////讀成串流
            Stream iStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] data = new byte[iStream.Length];
            int br = iStream.Read(data, 0, data.Length);
            if(br != iStream.Length)
            {
                throw new System.IO.IOException(filePath);
            }
            return data;
        }
    }
}