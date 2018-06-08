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
using System.Threading;
using System.Threading.Tasks;
using System.Web;


namespace IntelligenceCloud.Services
{
    public class AttachmentService : CrudGenericService<Attachment>
    {
        private ExcelGenericService ExcelSrv;
        public AttachmentService()
        {
            ExcelSrv = new ExcelGenericService();
        }

        //將上傳的檔案，存到資料庫
        public string CreateViewModelToDatabase(AttachViewModel viewModel)
        {
            SaveHttpPostFile(viewModel, "~/UploadFiles");
            string errStr = String.Empty;
            try
            {
                if (viewModel.AttachDetails != null)
                {
                    //計算excel檔案大小，用來回傳signalR
                    CalculateFileSize(viewModel.AttachDetails);

                    
                    HttpContext hctx = HttpContext.Current;//使用非同步處理 httpContext.current = null
                    //使用task非同步處理
                    var task = Task.Factory.StartNew(() =>
                    {

                        foreach (var item in viewModel.AttachDetails)
                        {
                            Create(item);   //無相關資料表
                            FileHelper.FileName = item.AttachmentName;
                            if (item.AttachmentUse == "通聯記錄")
                            {

                                CommunRecordService srv = new CommunRecordService();
                                srv.Import(item, hctx);

                                /// test
                                //總筆數及目前處理筆數
                                //FileHelper.Total = 3000;
                                ////使用Task進行非同步處理
                                //Random rnd = new Random();
                                //for (var i = 0; i < 3000; i++)
                                //{
                                //    //假裝將資料寫入資料庫，Delay 0-4ms
                                //    Thread.Sleep(rnd.Next(5));
                                //    FileHelper.Count++;
                                //}



                            }
                            if (item.AttachmentUse == "Cellebrite UFED報告檔")
                            {

                                ForensicService srv = new ForensicService();
                                srv.Import(item, hctx);
                            }

                        
                        }
                    });

                    float ratio = 0;
                    Action updateProgress = () =>
                    {
                        if (FileHelper.Total != 0)
                        {
                            ratio = (float)FileHelper.Count / FileHelper.Total;
                            UploaderHub.UpdateProgress(viewModel.ConnId, FileHelper.FileName,
                                                      ratio * 100, string.Format("{0:n0} /{1:n0}", FileHelper.Count, FileHelper.Total));

                        }
                    };

                    while (!task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                    {
                        updateProgress();
                        Thread.Sleep(200);//0.2秒傳一次
                    }
                    updateProgress();
                    if (task.IsCompleted && !task.IsFaulted)
                    {
                        return "OK";
                    }
                    else
                    {
                        return string.Join(" | ",
                            task.Exception.InnerExceptions.Select(o => o.Message).ToArray());
                    }
                }
            }
            catch(Exception err)
            {
                
                errStr =  err.Message;
            }
            UploaderHub.UpdateProgress(viewModel.ConnId, "-", 0, "-");
            return errStr;

        }

        public void CalculateFileSize(List<Attachment> attachs)
        {
            FileHelper.Total = 0;
            FileHelper.Count = 0;
            FileHelper.FileName = "";
            foreach (var item in attachs)
            {
                if(item.AttachmentType == ".xlsx" || item.AttachmentType == ".xls")
                {
                    IWorkbook workbook = null;
                    var physicalPath = HttpContext.Current.Server.MapPath(item.AttachmentPath);
                    var path = Path.Combine(physicalPath, item.AttachmentName);
                    using (FileStream fs = File.OpenRead(path))
                    {
                        workbook = ExcelSrv.GetWorkbook(item,fs);
                        if ( workbook != null)
                        {
                            for (int s = 0; s < workbook.NumberOfSheets; s++)
                            {
                                Helpers.FileHelper.Total += workbook.GetSheetAt(s).LastRowNum + 1;  //signalR檔案大小
                            }
                        }
                    }
                }
            }
        }

        //將上傳檔案存到某路徑 
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

        //下載檔案到用戶端
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

        
        //將excel 存到 List 給controller
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
            AttachExcelViewModel xmlViewModel = null;
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
                        xmlViewModel = new AttachExcelViewModel();
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
                                            rowViewModel.ExcelRow.Add("");
                                        }
                                        else
                                        {
                                            rowViewModel.ExcelRow.Add(dataFormatter.FormatCellValue(cell));
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

        
        //取出檔案共享人列表
        public AttachMemberShared GetMemberShared(int? id)
        {
            Attachment attachment = Get(a => a.AttachmentId == id);
            AttachMemberShared shared = new AttachMemberShared();
            shared.Attach = attachment;


            var memArr = attachment.MemberShared == null ? string.Empty.Split(',') : attachment.MemberShared.Split(',');
            var memIntArr = memArr.Select(m => {
                int.TryParse(m, out int value);
                return value;
            });

            using (MemberService memSrv = new MemberService())
            {
                var memShared = memIntArr.Select(ms => memSrv.Get(m => m.MemberId == ms));
                shared.MemberShared = memShared.ToList();
                shared.MemberUnshared = memSrv.GetAll().ToList().Except(shared.MemberShared).ToList();
            }
            if (IdentityHelper.UserId != null)  //未共享名單 刪除檔案擁有者
            {
                shared.MemberUnshared.RemoveAll(m => m.MemberId == IdentityHelper.UserId);
            }
            return shared;
        }

        //修改檔案共享人
        public Attachment ModifyMemberShared(int attachId, int memberId)
        {
            Attachment attach = Get(a => a.AttachmentId == attachId);
            var memArr = attach.MemberShared == null ? string.Empty.Split(',') : attach.MemberShared.Split(',');
            var memIntArr = memArr.Select(m => {
                int.TryParse(m, out int value);
                return value;
            });
            if (memIntArr.Any(m => m == memberId))
            {
                memArr = memArr.Where(m => m != memberId.ToString()).ToArray();
            }
            else
            {
                var memList = memArr.ToList();
                memList.Add(memberId.ToString());
                memList.RemoveAll(m => m == "");
                memArr = memList.ToArray();

            }
            attach.MemberShared = string.Join(",", memArr);
            return attach;
        }

        //取出可存取的檔案 :擁有 + 共享
        public IQueryable<Attachment> ShowAccessAttach()
        {
            if (IdentityHelper.UserId != null)
            {
                List<IEnumerable<Attachment>> unoin = new List<IEnumerable<Attachment>>() ;

                var own = Search(a => a.MemberId == IdentityHelper.UserId);
                var rest = Search(a => a.MemberShared != null && a.MemberShared != "").Except(own).ToList();
                string userId = IdentityHelper.UserId.ToString();
                var  shared = rest.Where(a => a.MemberShared.Split(',').Contains(userId));

                unoin.Add(own.ToList());
                unoin.Add(shared.ToList());
                var result = unoin.Aggregate((o, r) => o.Union(r));
                return result.AsQueryable();
            }

            return null;
        }
    }
}