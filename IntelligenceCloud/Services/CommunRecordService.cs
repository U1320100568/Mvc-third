using IntelligenceCloud.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace IntelligenceCloud.Services
{
    public class CommunRecordService : CrudGenericService<CommunRecord>
    {
        public CommunRecordService()
        {

        }

        //將通聯記錄的excel row map到entity欄位
        public Object CommunMapper(CommunRecord rec, IRow row)
        {
            DataFormatter Formatter = new DataFormatter(CultureInfo.CurrentCulture);
            string[] conArray = { "受話", "發話", "簡訊", "轉接" };
            string firstCell = Formatter.FormatCellValue(row.GetCell(0));
            //判斷是否通聯記錄內容
            if (conArray.Where(c => firstCell.Contains(c)).Any() && firstCell != "")
            {
                rec.CType = Formatter.FormatCellValue(row.GetCell(0));
                rec.CPhoneNum = Formatter.FormatCellValue(row.GetCell(1));
                rec.CCorrePhoneNum = Formatter.FormatCellValue(row.GetCell(2));

                //將字串轉換成時間
                DateTime dateTime;
                if (DateTime.TryParseExact(Formatter.FormatCellValue(row.GetCell(3))
                    , "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None
                    , out dateTime))
                {
                    rec.CStartTime = dateTime;
                    rec.CEndTime = dateTime.AddMinutes(row.GetCell(4).NumericCellValue);
                }

                rec.CIMEI = Formatter.FormatCellValue(row.GetCell(5));
                rec.CThroughPhoneNum = Formatter.FormatCellValue(row.GetCell(6));

                //基地台編號和地址分離
                string station = Formatter.FormatCellValue(row.GetCell(7));
                string[] stationDetail = station.Split('/');
                if (stationDetail.Length >= 2)
                {

                    rec.CStationNum = stationDetail[0];
                    rec.CStationAddress = stationDetail[1];
                }

                rec.isDeleted = false;
                return rec;
            }
            else
            {
                return null;
            }



        }


        //將excel 存到database
        public void CreateExcelToDatabase(Attachment attach)
        {

            FileStream fs = null;
            //NPOI
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            DataFormatter dataFormatter = new DataFormatter(CultureInfo.CurrentCulture);

            //path
            var physicalPath = HttpContext.Current.Server.MapPath(attach.AttachmentPath);
            var path = Path.Combine(physicalPath, attach.AttachmentName);
            
            try
            {
                //開啟現有檔案來讀取，存到資料串流
                using (fs = File.OpenRead(path))
                {
                    //資料串流存到NPOI物件
                    //新excel
                    if (attach.AttachmentType == ".xlsx")
                    {
                        workbook = new XSSFWorkbook(fs);
                    }
                    ///舊excel
                    if (attach.AttachmentType == ".xls")
                    {
                        workbook = new HSSFWorkbook(fs);
                    }
                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);     //NPOI sheet

                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);        //NPOI row
                                int cellCount = firstRow.LastCellNum;

                                for (int i = 0; i <= rowCount; i++)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) { continue; }

                                    CommunRecord rec = new CommunRecord();
                                    rec = CommunMapper(rec, row) as CommunRecord;
                                    if (rec != null)
                                    {
                                        rec.AttachmentId = attach.AttachmentId;
                                        CreateRecordToDatabase(rec);
                                    }


                                }
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                if (fs != null)
                {
                    fs.Close();
                }

            }

        }

        public byte[] CreateExcelFromDatabase(Attachment attach)
        {
            IWorkbook workbook = null;
           
            if(attach != null)
            {
                if (attach.AttachmentType == ".xlsx")
                {
                    workbook = new XSSFWorkbook();
                }
                ///舊excel
                if (attach.AttachmentType == ".xls")
                {
                    workbook = new HSSFWorkbook();
                }
                ISheet sheet = workbook.CreateSheet(attach.AttachmentName);
                WriteInWorkbook(sheet, attach);


                MemoryStream stream = null;
                using (stream = new MemoryStream())
                {
                    workbook.Write(stream);
                    byte[] data = new byte[stream.Length];
                    stream.Seek(0, SeekOrigin.Begin);
                    int br = stream.Read(data, 0, data.Length);
                    if (br != data.Length)
                    {
                        throw new System.IO.IOException();
                    }
                    return data;
                }
                
            }

            return null;

        }

        public ISheet WriteInWorkbook(ISheet sheet, Attachment attach) 
        {
            CommunRecord rec = new CommunRecord();
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue( GetDisplayName( rec.CType.GetType() ) ) ;
            row.CreateCell(1).SetCellValue(GetDisplayName(rec.CPhoneNum.GetType()));
            row.CreateCell(2).SetCellValue(GetDisplayName(rec.CCorrePhoneNum.GetType()));
            row.CreateCell(3).SetCellValue(GetDisplayName(rec.CStartTime.GetType()));
            row.CreateCell(4).SetCellValue("通話時間 (秒)");
            row.CreateCell(5).SetCellValue(GetDisplayName(rec.CIMEI.GetType()));
            row.CreateCell(6).SetCellValue(GetDisplayName(rec.CThroughPhoneNum.GetType()));
            row.CreateCell(7).SetCellValue("基地台編號/地址");

            var list = Search(c => c.AttachmentId == attach.AttachmentId).ToList();
            int i = 1;
            foreach(var item in list)
            {
                row = sheet.CreateRow(i++);
                row.CreateCell(0).SetCellValue(item.CType);
                row.CreateCell(1).SetCellValue(item.CPhoneNum);
                row.CreateCell(2).SetCellValue(item.CCorrePhoneNum);
                row.CreateCell(3).SetCellValue(item.CStartTime.Value);
                row.CreateCell(4).SetCellValue( ((TimeSpan)(item.CEndTime - item.CStartTime)).TotalSeconds);
                row.CreateCell(5).SetCellValue(item.CIMEI);
                row.CreateCell(6).SetCellValue(item.CThroughPhoneNum);
                row.CreateCell(7).SetCellValue(item.CStationNum == null? null: item.CStationNum +"/"+ item.CStationAddress);
            }

            return sheet;
        }

        //取的display name
        public string GetDisplayName(Type prop)
        {
            var attr = prop.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Single();
            return attr.DisplayName;
        }

        //存通聯記錄entity
        public void CreateRecordToDatabase(CommunRecord rec)
        {
            Create(rec);
            

        }

    }
}