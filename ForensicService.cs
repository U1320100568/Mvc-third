using IntelligenceCloud.Helpers;
using IntelligenceCloud.Models;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using IntelligenceCloud.Helpers;

namespace IntelligenceCloud.Services
{
    public class ForensicService : ExcelGenericService
    {
        public CrudGenericService<ForensicContact> contService;
        public CrudGenericService<ForensicSMS> smsService;
        
        public ForensicService()
        {
            contService = new CrudGenericService<ForensicContact>();
            smsService = new CrudGenericService<ForensicSMS>();
            
        }

        ////////////匯入
        public void Import(Attachment attach, HttpContext hctx)
        {

            FileStream fs = null;
            //NPOI
            IWorkbook workbook = null;
            ISheet sheet = null;

            DataFormatter dataFormatter = new DataFormatter(CultureInfo.CurrentCulture);

            //path
            var physicalPath = hctx.Server.MapPath(attach.AttachmentPath);
            var path = Path.Combine(physicalPath, attach.AttachmentName);

            try
            {
                //開啟現有檔案來讀取，存到資料串流
                using (fs = File.OpenRead(path))
                {
                    //資料串流存到NPOI物件
                    //新excel
                    workbook = GetWorkbook(attach, fs);
                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);     //SMS訊息
                        SMSCreate(sheet, attach);
                        sheet = workbook.GetSheetAt(1);     //聯絡人
                        ContaCreate(sheet, attach);
                        
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

        //map table欄位 - ForensicSMS
        public ForensicSMS SMSMapper(ForensicSMS sms, IRow row)
        {
            DataFormatter Formatter = new DataFormatter(CultureInfo.CurrentCulture);
            string firstCell = Formatter.FormatCellValue(row.GetCell(0));

            //判斷是否通聯記錄內容
            if (int.TryParse(firstCell, out int result) && firstCell != "")
            {
                sms.FSheetNum = 0;
                sms.FSheetName = row.Sheet.SheetName;
                sms.FNum = result;
                sms.FCorrespond = Formatter.FormatCellValue(row.GetCell(1));

                //將字串轉換成時間
                sms.FDatetime = ParseHelper.ParseDateTime(Formatter.FormatCellValue(row.GetCell(2)));

                return sms;
            }
            else
            {
                return null;
            }



        }

        ///存到資料庫- ForensicSMS
        public void SMSCreate(ISheet sheet, Attachment attach)
        {
            IRow row = null;
            if (sheet != null)
            {
                int rowCount = sheet.LastRowNum;
                if (rowCount > 0)
                {
                    IRow firstRow = sheet.GetRow(0);        //NPOI row

                    for (int i = 0; i <= rowCount; i++)
                    {
                        row = sheet.GetRow(i);
                        if (row == null) { continue; }
                        ForensicSMS sms = new ForensicSMS();
                        sms = SMSMapper(sms, row);
                        if (sms != null)
                        {
                            sms.GetType().GetProperty("AttachmentId").SetValue(sms, attach.AttachmentId);
                            smsService.Create(sms);

                        }

                        Helpers.FileHelper.Count++;///signalR 處理進度
                    }
                }
            }
        }

        //map table欄位 - ForensicContact
        public ForensicContact ContaMapper(ForensicContact conta, IRow row)
        {
            DataFormatter Formatter = new DataFormatter(CultureInfo.CurrentCulture);
            string firstCell = Formatter.FormatCellValue(row.GetCell(0));
            
            //判斷是否通聯記錄內容
            if (int.TryParse(firstCell,out int result) && firstCell != "")
            {
                conta.FSheetNum = 1;
                conta.FSheetName = row.Sheet.SheetName;
                conta.FNum = result;
                conta.FName = Formatter.FormatCellValue(row.GetCell(1));
                conta.FGroup = Formatter.FormatCellValue(row.GetCell(2));

                conta.FModifiedTime = ParseHelper.ParseDateTime(Formatter.FormatCellValue(row.GetCell(3)));

                conta.FClause = Formatter.FormatCellValue(row.GetCell(4));
                conta.FNote = Formatter.FormatCellValue(row.GetCell(5));
                conta.FSource = Formatter.FormatCellValue(row.GetCell(6));
                conta.isDeleted = Formatter.FormatCellValue(row.GetCell(7)) == "是" ? true : false;
                
                return conta;
            }
            else
            {
                return null;
            }



        }

        ///存到資料庫- ForensicContact
        public void ContaCreate(ISheet sheet, Attachment attach)
        {
            IRow row = null;
            if (sheet != null)
            {
                int rowCount = sheet.LastRowNum;
                if (rowCount > 0)
                {
                    IRow firstRow = sheet.GetRow(0);        //NPOI row
                    
                    for (int i = 0; i <= rowCount; i++)
                    {
                        


                        row = sheet.GetRow(i);
                        if (row == null) { continue; }
                        ForensicContact conta = new ForensicContact();
                        conta = ContaMapper(conta, row);
                        if (conta != null)
                        {
                            conta.GetType().GetProperty("AttachmentId").SetValue(conta, attach.AttachmentId);
                            contService.Create(conta);

                        }

                        Helpers.FileHelper.Count++;///signalR 處理進度
                    }
                }
            }
        }

        
        //////////匯出
        public byte[] Export(Attachment attach)
        {

            IWorkbook workbook = GetWorkbook(attach);
            ISheet sheet1 = workbook.CreateSheet(smsService.Get(i => 1 ==1).FSheetName);
            SmsIntoWorkbook(sheet1, attach);
            ISheet sheet2 = workbook.CreateSheet(contService.Get(i => 1 == 1).FSheetName);
            ContaIntoWorkbook(sheet2, attach);
            return WorkbookToStream(workbook);
        }

        //將檔案寫進workbook - ForensicSMS
        public ISheet SmsIntoWorkbook(ISheet sheet, Attachment attach)
        {

            Type t = typeof(Models.ForensicSMS.ForensicSMSMetaData );
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue(GetDisplayName(t.GetProperty("FNum")));
            row.CreateCell(1).SetCellValue(GetDisplayName(t.GetProperty("FCorrespond")));
            row.CreateCell(2).SetCellValue(GetDisplayName(t.GetProperty("FDatetime")));
            

            var list = smsService.Search(c => c.AttachmentId == attach.AttachmentId).ToList();
            
            int i = 1;
            foreach (var item in list)
            {
                row = sheet.CreateRow(i++);
                row.CreateCell(0).SetCellValue(item.FNum.Value);
                row.CreateCell(1).SetCellValue(item.FCorrespond);
                row.CreateCell(2).SetCellValue(item.FDatetime.GetValueOrDefault().ToString("MM/dd/yyyy  HH:mm:ss"));
                
            }

            return sheet;
        }

        //將檔案寫進workbook - ForensicContact
        public ISheet ContaIntoWorkbook(ISheet sheet, Attachment attach)
        {

            Type t = typeof(Models.ForensicContact.ForensicContactMetaData);
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue(GetDisplayName(t.GetProperty("FNum")));
            row.CreateCell(1).SetCellValue(GetDisplayName(t.GetProperty("FName")));
            row.CreateCell(2).SetCellValue(GetDisplayName(t.GetProperty("FGroup")));
            row.CreateCell(3).SetCellValue(GetDisplayName(t.GetProperty("FModifiedTime")));
            row.CreateCell(4).SetCellValue(GetDisplayName(t.GetProperty("FClause")));
            row.CreateCell(5).SetCellValue(GetDisplayName(t.GetProperty("FNote")));
            row.CreateCell(6).SetCellValue(GetDisplayName(t.GetProperty("FSource")));
            row.CreateCell(7).SetCellValue(GetDisplayName(t.GetProperty("isDeleted")));

            var list = contService.Search(c => c.AttachmentId == attach.AttachmentId).ToList();
            int i = 1;
            foreach (var item in list)
            {
                row = sheet.CreateRow(i++);
                row.CreateCell(0).SetCellValue(item.FNum.Value);
                row.CreateCell(1).SetCellValue(item.FName);
                row.CreateCell(2).SetCellValue(item.FGroup);
                var datetime = item.FModifiedTime;
                row.CreateCell(3).SetCellValue(datetime == null ? "": datetime.GetValueOrDefault().ToString("MM/dd/yyyy  HH:mm:ss"));
                row.CreateCell(4).SetCellValue(item.FClause);
                row.CreateCell(5).SetCellValue(item.FNote);
                row.CreateCell(6).SetCellValue(item.FSource);
                row.CreateCell(7).SetCellValue(item.isDeleted ==true ? "是":"");
            }

            return sheet;
        }

        
    }
}