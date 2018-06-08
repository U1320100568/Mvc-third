using IntelligenceCloud.Helpers;
using IntelligenceCloud.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace IntelligenceCloud.Services
{
    public class ExcelGenericService 
    {
        
        public ExcelGenericService() {
            
        }

        
        
        //取的display name
        public string GetDisplayName(PropertyInfo prop)
        {

            var attr = prop.GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().SingleOrDefault();

            return attr.Name;
        }
        
        //將workbook寫進資料流，回傳byte
        public byte[] WorkbookToStream( IWorkbook book)
        {
            
            //ISheet sheet = workbook.CreateSheet(attach.AttachmentName);
            //WriteInWorkbook(sheet, attach);
            if(book != null)
            {
                NpoiMemoryStream stream = null;
                
                
                /**/
                using (stream = new NpoiMemoryStream())
                {
                    try
                    {
                        stream.AllowClose = false;
                        book.Write(stream);
                        byte[] data = new byte[stream.Length];
                        stream.Seek(0, SeekOrigin.Begin);
                        int br = stream.Read(data, 0, data.Length);
                        if (br != data.Length)
                        {
                            throw new System.IO.IOException();
                        }
                        stream.Flush();
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.AllowClose = true;
                        return data;
                    }
                    catch  (Exception e)
                    {
                        throw e;
                    }
                    
                    
                }
                

            }

            

            

            return null;

        }

        //取得workbook
        public IWorkbook GetWorkbook (Attachment attach)
        {
            IWorkbook workbook = null;
            if (attach.AttachmentType == ".xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            ///舊excel
            if (attach.AttachmentType == ".xls")
            {
                workbook = new HSSFWorkbook();
            }

            return workbook;
        }
        //從資料流取得workbook - 上傳
        public IWorkbook GetWorkbook(Attachment attach,FileStream fs)
        {
            IWorkbook workbook = null;
            if (attach.AttachmentType == ".xlsx")
            {
                workbook = new XSSFWorkbook(fs);
            }
            ///舊excel
            if (attach.AttachmentType == ".xls")
            {
                workbook = new HSSFWorkbook(fs);
            }

            return workbook;
        }

        /////////列表 
        public IQueryable<T> Sort<T>(IQueryable<T> query, string orderer, bool? orderDescend)
        {
            //query 要排序的集合, orderer要排序的欄位名稱, orderDescend正序或倒序

            string firstProp = typeof(T).GetProperties().FirstOrDefault().Name;
            orderer = orderer == null ? firstProp : orderer;

            if (orderDescend == true)
            {
                return query.PropertySort(orderer, false);
            }
            else
            {
                return query.PropertySort(orderer, true);
            }
        }
    }


    //NPOI的BUG，不明原因 memory stream 會自動關閉，所以自寫一個控制關閉時機
    public class NpoiMemoryStream : MemoryStream
    {
        public bool AllowClose { get; set; }
        public NpoiMemoryStream()
        {
            AllowClose = true;
        }
        public override void Close()
        {
            if (AllowClose)
            {
                base.Close();
            }
        }
    }
}