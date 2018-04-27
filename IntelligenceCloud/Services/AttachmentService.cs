using IntelligenceCloud.Helpers;
using IntelligenceCloud.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

                    if (item.AttachmentUse == "通聯記錄")
                    {
                        using (CommunRecordService srv = new CommunRecordService())
                        {
                            srv.CreateExcelToDatabase(item);
                        }
                            
                    }
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
                            AttachmentUse = viewModel.AttachmentUse,
                            isDeleted = false,
                            MemberId = IdentityHelper.UserId,
                            UploadTime = DateTime.Now
                            
                        });
                        
                    }
                }

            
            }

            
            
        }

        //下載到本機
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

        
        //將excel 存到list 給controller
        public AttachSingleViewModel ShowExcelFile(Attachment attach)
        {
            FileStream fs = null;
            //NPOI
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            DataFormatter dataFormatter = new DataFormatter(CultureInfo.CurrentCulture);

            //插入詳細資訊
            AttachSingleViewModel viewModel = new AttachSingleViewModel() {
                AttachDetail = attach
            };
            //2維列表
            AttachXmlViewModel xmlViewModel = null;
            AttachXmlRow rowViewModel = null;
            //path
            var physicalPath = HttpContext.Current.Server.MapPath(attach.AttachmentPath);
            var path = Path.Combine(physicalPath, attach.AttachmentName);
            
            //圖片檔
            Regex regex = new Regex(@"\.(jpg|jpeg|png|gif)$",RegexOptions.IgnoreCase);
            if (regex.IsMatch(attach.AttachmentType ))
            {
                viewModel.AttahcFilePath = Path.Combine(attach.AttachmentPath, attach.AttachmentName);
            }

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
                    if(attach.AttachmentType == ".xls")
                    {
                        workbook = new HSSFWorkbook(fs);
                    }
                    if(workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);     //NPOI sheet
                        xmlViewModel = new AttachXmlViewModel();
                        if(sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;
                            if (rowCount > 20) { rowCount = 20; }
                            if(rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);        //NPOI row
                                int cellCount = firstRow.LastCellNum;
                                
                                for(int i = 0; i<rowCount; i++)
                                {
                                    row = sheet.GetRow(i);
                                    if(row == null) { continue; }
                                    rowViewModel = new AttachXmlRow();
                                    for(int j = row.FirstCellNum; j < cellCount; j++)
                                    {
                                        cell = row.GetCell(j);      //NPOI column
                                        if(cell == null)
                                        {
                                            rowViewModel.XmlRow.Add("");
                                        }
                                        else
                                        {
                                            rowViewModel.XmlRow.Add(dataFormatter.FormatCellValue(cell));
                                        }

                                    }

                                    //插入列表 row
                                    xmlViewModel.Table.Add(rowViewModel);
                                }
                            }
                        }
                    }
                    //插入整個列表
                    viewModel.AttachFile = xmlViewModel;
                    return viewModel;
                }
            }
            catch(Exception e)
            {
                if(fs != null)
                {
                    fs.Close();
                }
                return viewModel;
            }

        }

        
    }
}