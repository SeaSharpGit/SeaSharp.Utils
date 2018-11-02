using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace SeaSharp.Utils
{
    public static class ExcelUtils
    {
        #region List转DataTable
        public static DataTable ConvertToDataTable<T>(this IEnumerable<T> collection) where T : class, new()
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());//添加列名
            var count = collection.Count();
            if (collection == null || count == 0)
            {
                return dt;
            }
            for (int i = 0; i < count; i++)
            {
                var list = new ArrayList();
                foreach (PropertyInfo pi in props)
                {
                    list.Add(pi.GetValue(collection.ElementAt(i), null));//将一行的每一列放入数组中
                }
                dt.LoadDataRow(list.ToArray(), true);//添加一行数据
            }
            return dt;
        }
        #endregion

        #region DataTable转List
        public static List<T> ConvertToList<T>(this DataTable dt) where T : class, new()
        {
            if (dt == null)
            {
                return new List<T>();
            }
            List<T> list = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                var t = Activator.CreateInstance<T>();
                var propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    var piName = pi.Name;
                    if (dt.Columns.Contains(piName))
                    {
                        if (!pi.CanWrite)
                        {
                            continue;
                        }
                        var value = dr[piName];
                        if (value != DBNull.Value)
                        {
                            pi.SetValue(t, value, null);
                        }
                    }
                }
                list.Add(t);
            }
            return list;
        }
        #endregion

        #region NPOI导入
        /// <summary>
        /// NPOI导入
        /// </summary>
        /// <param name="filePath">地址需要加入：HttpRuntime.AppDomainAppPath</param>
        /// <param name="columns">
        ///     var columns = new DataColumn[] {
        ///         new DataColumn("ID", Type.GetType("System.Int32")),
        ///         new DataColumn("Name", Type.GetType("System.String"))
        ///     };
        /// </param>
        public static DataTable ExcelToDataTable(string filePath, DataColumn[] columns)
        {
            var dt = new DataTable();
            dt.Columns.AddRange(columns);
            using (var fs = File.OpenRead(filePath))
            {
                //Office2003版.xls使用HSSFWorkbook
                //Office2007版.xlsx使用XSSFWorkbook
                var wk = new XSSFWorkbook(fs);
                var sheet = wk.GetSheetAt(0);
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var dr = dt.NewRow();
                    dr[0] = i;
                    for (int j = 0; j < row.LastCellNum; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell == null)
                        {
                            var type = columns[j + 1].DataType;
                            dr[j + 1] = type.IsValueType ? Activator.CreateInstance(type) : null;
                        }
                        else
                        {
                            switch (cell.CellType)
                            {
                                case CellType.Blank:
                                    dr[j + 1] = "";
                                    break;
                                case CellType.Numeric:
                                    short format = cell.CellStyle.DataFormat;
                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                    if (format == 14 || format == 31 || format == 57 || format == 58 || format == 180)
                                    {
                                        dr[j + 1] = cell.DateCellValue;
                                    }
                                    else
                                    {
                                        dr[j + 1] = cell.NumericCellValue;
                                    }
                                    break;
                                case CellType.String:
                                    dr[j + 1] = cell.StringCellValue;
                                    break;
                            }
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
        #endregion

        #region NPOI导出(NoTest)
        public static void ExportExcel(DataTable dt, string fileName, string sheetName = "Sheet1")
        {
            var book = new HSSFWorkbook();
            var sheet = book.CreateSheet(sheetName);
            var rowName = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                rowName.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var rowtemp = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    rowtemp.CreateCell(j).SetCellValue(dt.Rows[i][j].ToString().Trim());
                }
            }
            var response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
            using (var ms = new MemoryStream())
            {
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                response.BinaryWrite(ms.ToArray());
                response.Flush();
                response.End();
            }
        }
        #endregion

        #region WebApi导出（NoTest）
        /// <summary>
        /// WebApi导出
        /// </summary>
        public static HttpResponseMessage ExportExcelWebApi<T>(string fileName, IEnumerable<T> list, string[] cells)
        {
            if (cells.Count() != list.Count())
            {
                throw new Exception("导出Excel出错：list参数与cells参数的数量不相同");
            }
            var props = typeof(T).GetProperties();
            var hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet("1");
            //标题行
            IRow rowHeader = sheet.CreateRow(0);
            //内容
            var i = 0;
            foreach (var item in list)
            {
                var row = sheet.CreateRow(i + 1);
                for (int j = 0; j < list.Count(); j++)
                {
                    row.CreateCell(j).SetCellValue(props[j].GetValue(item)?.ToString() ?? "");
                }
                sheet.AutoSizeColumn(i++);//自适应
            }
            var file = new MemoryStream();
            hssfworkbook.Write(file);
            file.Seek(0, SeekOrigin.Begin);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(file)
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName + ".xls"
            };
            return result;
        }
        #endregion

        #region Excel导出第二种方法
        public static void ExportExcelWeb2(DataTable dt, string fileName = "导出")
        {
            var sb = new StringBuilder();
            sb.Append("<table border='1' cellspacing='1' cellpadding='1'><thead>");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                sb.Append("<th>" + dt.Columns[j].ColumnName + "</th>");
            }
            sb.Append("</thead><tbody>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("<tr>");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sb.Append("<td>" + dt.Rows[i][j] + "</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");
            var response = System.Web.HttpContext.Current.Response;
            response.ContentType = "application/vnd.ms-excel";//设置Mime类型
            response.ContentEncoding = System.Text.Encoding.UTF8;//标识的编码
            response.Charset = "utf-8";//显示的编码
            response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
            response.Output.Write(sb);
            response.Flush();//发送缓冲输出
            response.End();
        }
        #endregion

        #region Excel导出其他方法（NoTest）
        ///// 根据XML导出Excel,无需第三方插件，可以对单元格进行合并和设置样式
        //public static void ExportExcelWeb(string fileName = "导出")
        //{
        //    var sb = new StringBuilder();
        //    //XML头部
        //    sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\nxmlns:x=\"urn:schemas-microsoft-com:office:excel\"\nxmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"\nxmlns:html=\"http://www.w3.org/TR/REC-html40\">\n");
        //    //设置样式，ID可以在下面调用
        //    sb.Append("<Styles>");
        //    sb.Append("<Style ss:ID=\"Default\" ss:Name=\"Normal\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"宋体\" ss:Size=\"11\" ss:Color=\"#000000\"/></Style>");//全局样式
        //    sb.Append("<Style ss:ID=\"Title\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"宋体\" x:CharSet=\"134\" ss:Size=\"11\" ss:Color=\"#000000\" ss:Bold=\"1\"/></Style>");//标题样式
        //    sb.Append("</Styles>");
        //    //worksheet开始,DefaultColumnWidth默认宽度，DefaultRowHeight默认高度，
        //    sb.Append("<Worksheet ss:Name=\"Sheet1\">\n");
        //    sb.Append("<Table ss:StyleID=\"Default\" ss:DefaultColumnWidth=\"100\" ss:DefaultRowHeight=\"20\">\n");
        //    //特殊设定
        //    //设置某一列的高度和宽度,1开始：sb.Append("<Column ss:Index=\"1\" ss:AutoFitWidth=\"0\" ss:Width=\"30\"/>\n");
        //    //纵向合并单元格：<Cell ss:MergeDown="1">，横向合并单元格：<Cell ss:MergeAcross="1">
        //    //正文：
        //    sb.Append("<Row ss:StyleID=\"Title\" ss:AutoFitHeight=\"0\" ss:Height=\"20\">\n");//引用样式，无样式默认为Default
        //    sb.Append("<Cell><Data ss:Type=\"String\">员工编号</Data></Cell>\n");//所有单元格内容都是用本行添加
        //    sb.Append("</Row>\n");
        //    sb.Append("</Table>\n");
        //    sb.Append("</Worksheet>\n");
        //    sb.Append("</Workbook>\n");
        //    var response = System.Web.HttpContext.Current.Response;
        //    response.Clear();
        //    response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
        //    response.Charset = "gb2312";
        //    response.ContentType = "application/ms-excel";
        //    response.Write(sb.ToString());
        //    response.End();
        //}

        ///// 导出（可合并单元格）,需要引入AppLibrary.dll
        //public static void ExportExcelWeb()
        //{
        //    var doc = new AppLibrary.WriteExcel.XlsDocument();
        //    doc.FileName = "导出.xls";
        //    doc.SummaryInformation.Title = "export";//标题
        //    doc.SummaryInformation.Subject = "dahai";//主题
        //    doc.SummaryInformation.Author = "dahai";//作者
        //                                            //单元格样式
        //    var XFstyle = doc.NewXF();
        //    XFstyle.HorizontalAlignment = AppLibrary.WriteExcel.HorizontalAlignments.Default;
        //    XFstyle.Font.FontName = "Arial";
        //    XFstyle.UseMisc = true;
        //    XFstyle.TextDirection = AppLibrary.WriteExcel.TextDirections.LeftToRight;
        //    XFstyle.Font.Bold = true;
        //    XFstyle.BottomLineStyle = 1;
        //    XFstyle.LeftLineStyle = 1;
        //    XFstyle.TopLineStyle = 1;
        //    XFstyle.RightLineStyle = 1;
        //    XFstyle.UseBorder = true;
        //    XFstyle.PatternColor = AppLibrary.WriteExcel.Colors.Red;//单元格背景颜色
        //    XFstyle.Pattern = 1;//单元格颜色渐变，1为不渐变，显示颜色必须填写此行
        //    var SheetName = "";
        //    var list = new ArrayList();
        //    foreach (var item in list)
        //    {
        //        SheetName = "1";
        //        var sheet = doc.Workbook.Worksheets.Add(SheetName);
        //        var cells = sheet.Cells;
        //        //第一行表头
        //        cells.Add(1, 1, "员工编号", XFstyle);
        //        cells.Add(1, 2, "员工姓名", XFstyle);
        //        cells.Merge(1, 1, 2, 2);//合并单元格
        //        //设置宽度
        //        var colInfo = new AppLibrary.WriteExcel.ColumnInfo(doc, sheet);
        //        colInfo.Width = 20 * 256;
        //        colInfo.ColumnIndexStart = 1;
        //        colInfo.ColumnIndexEnd = 2;
        //        sheet.AddColumnInfo(colInfo);
        //    }
        //    doc.Send();
        //    var response = System.Web.HttpContext.Current.Response;
        //    response.Flush();
        //    response.End();
        //} 
        #endregion
    }
}
