using IntelligenceCloud.Helpers;
using IntelligenceCloud.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace IntelligenceCloud.Services
{
    public class AttachmentService : CrudGenericService<Attachment>
    {
        
        public AttachmentService()
        {
            
        }
        //用viewModel為了通用 不同的class
        //此處viewModel 可能包含HttpPostFileBase物件
        public void CreateViewModelToDatabase(AttachViewModel viewModel)
        {
            SaveHttpPostFile(viewModel, "~/UploadFiles");
            
            if(viewModel.AttachDetails != null)
            {
                foreach(var item in viewModel.AttachDetails)
                {
                    Create(item);
                }
            }
        }

        //將檔案存到路徑下 
        public void SaveHttpPostFile(AttachViewModel viewModel, string storedFolder )
        {

            if (viewModel.AttachFiles != null)
            {
                viewModel.AttachDetails = new List<Attachment>();

                //可能多個檔案上傳
                foreach (var httpPost in viewModel.AttachFiles)
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
                        string fileNameTmp = fileName;
                        while (System.IO.File.Exists(path))
                        {
                            fileExtandNum++;

                            fileNameTmp = fileName.Replace(match.Value.ToString(), "") + "(" + fileExtandNum + ")" + match.Value;
                            path = Path.Combine(HttpContext.Current.Server.MapPath(storedFolder), fileNameTmp);

                            
                        }
                        fileName = fileNameTmp;
                        httpPost.SaveAs(path);

                        
                        AttachmentRecord record = new AttachmentRecord();
                        viewModel.AttachDetails.Add(new Attachment()
                        {
                            AttachmentName = fileName,
                            AttachmentOriginName = httpPost.FileName,
                            AttachmentPath = storedFolder,
                            AttachmentType = match.Success ? match.Value : "unknown",
                            isDeleted = false,
                            MemberId = IdentityHelper.UserId,
                            UploadTime = DateTime.Now
                            
                        });
                        

                    }
                }

            
            }

            
            
        }

        public byte[] Download(Attachment attach)
        {
            string filePath = Path.Combine(HttpContext.Current.Server.MapPath(attach.AttachmentPath), attach.AttachmentName);

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

            //更新下載記錄
            attach.AttachmentRecord.Add(new AttachmentRecord()
            {
                TimeDownload = DateTime.Now,
                AttachmentId = attach.AttachmentId
            });
            Update(attach);

            return data;
        }
    }
}