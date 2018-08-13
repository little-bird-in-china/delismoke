using org.in2bits.MyXls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BlueStone.Smoke.Service
{
    /// <summary>
    /// 导出数据值转换器
    /// </summary>
    /// <typeparam name="T">要导出数据的类型</typeparam>
    /// <param name="rowData">当前列数据</param>
    /// <param name="columnName">导出的当前行的名称</param>
    /// <returns></returns>
    public delegate object ColumnValueConvert<in T>(T rowData);

    public class ColumnSetting<T> where T : class
    {
        /// <summary>
        /// 导出的列的名称
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 导出的列绑定的数据属性名称
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 导出的数据的格式化字符串，如果数据是时间类型，则可以是：yyyy-MM-dd HH:ss
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 导出的数据转换器
        /// </summary>
        public ColumnValueConvert<T> ValueConverter { get; set; }
        private ColumnValueConvert<T> defaultConverter;

        public object GetPropertyValue(T rowData)
        {
            if (ValueConverter != null) return ValueConverter(rowData);
            return defaultConverter(rowData);
        }

        protected virtual object DefaultValueConverter(T rowData)
        {
            return GetPropertyValue(rowData, PropertyName);
        }

        protected virtual object GetPropertyValue(object rowData, string propertyNameSetting)
        {
            if (rowData == null || string.IsNullOrWhiteSpace(propertyNameSetting))
                return "";
            Type type = rowData.GetType();

            string[] propertyNames = propertyNameSetting.Trim().Split('.');

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;
            object value = null;
            if (propertyNames.Length > 0)
            {
                int i = 0;
                Type currentType = type;
                object currentValue = rowData;
                do
                {
                    string propertyName = propertyNames[i];
                    bool isField = false;
                    PropertyInfo pInfo = currentType.GetProperty(propertyName, flags);
                    FieldInfo fInfo = null;
                    if (pInfo == null)
                    {
                        fInfo = type.GetField(propertyName, flags);
                        if (fInfo == null)
                        {
                            break;
                        }
                        isField = true;
                    }
                    if (i == propertyNames.Length - 1)
                    {
                        if (isField)
                        {
                            value = fInfo.GetValue(currentValue);
                        }
                        else if (pInfo.CanRead)
                        {
                            value = pInfo.GetValue(currentValue, null);
                        }
                    }
                    else
                    {
                        if (isField)
                        {
                            currentType = fInfo.FieldType;
                            currentValue = fInfo.GetValue(currentValue);
                        }
                        else if (pInfo.CanRead)
                        {
                            currentType = pInfo.PropertyType;
                            currentValue = pInfo.GetValue(currentValue, null);
                        }
                    }
                    if (currentValue == null && currentType.IsValueType)
                    {
                        break;
                    }
                }
                while (++i < propertyNames.Length);
            }

            return FormatValue(value, Format);
        }

        public ColumnSetting()
        {
            defaultConverter = new ColumnValueConvert<T>(DefaultValueConverter);
            ValueConverter = defaultConverter;
        }
        private string GetDescription(object enumValue)
        {
            Type t = enumValue.GetType();
            FieldInfo f = t.GetField(enumValue.ToString());
            object[] atts = f.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (atts != null && atts.Length > 0)
            {
                return (atts[0] as System.ComponentModel.DescriptionAttribute).Description;
            }
            return enumValue.ToString();
        }

        protected virtual object FormatValue(object value, string format)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }
            Type type = value.GetType();
            while (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && type.GetGenericArguments() != null
                    && type.GetGenericArguments().Length == 1)
            {
                type = type.GetGenericArguments()[0];
            }
            if (type.IsEnum)
            {
                return GetDescription(value);
            }
            TypeCode code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.DateTime:
                    if (format == null || format.Trim().Length <= 0)
                    {
                        return Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    return Convert.ToDateTime(value).ToString(format);
                case TypeCode.Decimal:
                    if (format == null || format.Trim().Length <= 0)
                    {
                        return Convert.ToDecimal(value).ToString("#,##0.00");
                    }
                    return Convert.ToDecimal(value).ToString(format);
                case TypeCode.Double:
                    if (format == null || format.Trim().Length <= 0)
                    {
                        return value.ToString();
                    }
                    return Convert.ToDouble(value).ToString(format);
                case TypeCode.Single:
                    if (format == null || format.Trim().Length <= 0)
                    {
                        return value.ToString();
                    }
                    return Convert.ToSingle(value).ToString(format);
                case TypeCode.Byte:
                    if (format == null || format.Trim().Length <= 0)
                    {
                        return value.ToString();
                    }
                    return Convert.ToByte(value).ToString(format);
                case TypeCode.Int16:
                    if (format == null || format.Trim().Length <= 0)
                    {
                        return value.ToString();
                    }
                    return Convert.ToInt16(value).ToString(format);
                case TypeCode.Int32:
                    if (format == null || format.Trim().Length <= 0)
                    {
                        return value.ToString();
                    }
                    return Convert.ToInt32(value).ToString(format);
                case TypeCode.Int64:
                    if (format == null || format.Trim().Length <= 0)
                    {
                        return value.ToString();
                    }
                    return Convert.ToInt64(value).ToString(format);
                case TypeCode.SByte:
                    if (format == null || format.Trim().Length <= 0)
                    {
                        return value.ToString();
                    }
                    return Convert.ToSByte(value).ToString(format);
                case TypeCode.UInt16:
                    if (format == null || format.Trim().Length <= 0)
                    {
                        return value.ToString();
                    }
                    return Convert.ToUInt16(value).ToString(format);
                case TypeCode.UInt32:
                    if (format == null || format.Trim().Length <= 0)
                    {
                        return value.ToString();
                    }
                    return Convert.ToUInt32(value).ToString(format);
                case TypeCode.UInt64:
                    if (format == null || format.Trim().Length <= 0)
                    {
                        return value.ToString();
                    }
                    return Convert.ToUInt64(value).ToString(format);
                case TypeCode.Char:
                case TypeCode.Boolean:
                case TypeCode.String:
                case TypeCode.Object:
                    if (format == null || format.Trim().Length <= 0)
                    {
                        return value.ToString();
                    }
                    return string.Format(format, value.ToString());
                case TypeCode.DBNull:
                case TypeCode.Empty:
                    return null;
                default:
                    return value;
            }
        }

    }

    public class ColumnSetting : ColumnSetting<DataRow>
    {
        protected override object DefaultValueConverter(DataRow rowData)
        {
            if (rowData == null || string.IsNullOrWhiteSpace(PropertyName))
                return "";

            string[] propertyNames = PropertyName.Trim().Split('.');

            object value = rowData[propertyNames[0]];
            if (propertyNames.Length == 1 || value == null)
            {
                return value == null ? "" : FormatValue(value, Format);
            }
            var propertyNameSetting = string.Join(".", propertyNames, 1, propertyNames.Length - 1);

            return GetPropertyValue(value, propertyNameSetting);
        }

        public ColumnSetting()
            : base()
        {
        }
    }

    public interface IFileExporter<T> where T : class
    {
        string Export(List<T> data, IList<ColumnSetting<T>> column, string title);
    }

    public interface IDataTableFileExporter
    {
        string Export(DataTable data, IList<ColumnSetting> column);
        string Export(string title, DataTable data, IList<ColumnSetting> column);
    }

    public static class ExcelFileExporterHelper
    {
        internal static void MergeRegion(Worksheet ws, XF xf, string content, int startRow, int startCol, int endRow, int endCol)
        {
            for (int i = startCol; i <= endCol; i++)
            {
                for (int j = startRow; j <= endRow; j++)
                {
                    ws.Cells.Add(j, i, content, xf);
                }
            }
            if (endCol > 0)
            {
                ws.Cells.Merge(startRow, endRow, startCol, endCol);
            }
        }
        internal static XF GetSheetTitleXF(XlsDocument xlsDoc)
        {
            XF xf = xlsDoc.NewXF();
            xf.HorizontalAlignment = HorizontalAlignments.Centered;
            xf.VerticalAlignment = VerticalAlignments.Centered;
            xf.Font.Height = 240;
            xf.Font.Weight = FontWeight.Bold;
            xf.LeftLineStyle = 1;
            xf.LeftLineColor = Colors.Black;
            xf.TopLineStyle = 1;
            xf.TopLineColor = Colors.Black;
            xf.RightLineStyle = 1;
            xf.RightLineColor = Colors.Black;
            xf.BottomLineStyle = 1;
            xf.BottomLineColor = Colors.Black;
            xf.TextWrapRight = true;
            return xf;
        }
        internal static XF GetDataCellXF(XlsDocument xlsDoc)
        {
            XF xf = xlsDoc.NewXF();
            xf.LeftLineStyle = 1;
            xf.LeftLineColor = Colors.Black;
            xf.TopLineStyle = 1;
            xf.TopLineColor = Colors.Black;
            xf.RightLineStyle = 1;
            xf.RightLineColor = Colors.Black;
            xf.BottomLineStyle = 1;
            xf.BottomLineColor = Colors.Black;
            xf.TextWrapRight = true;
            return xf;
        }
        internal static string GetFileName(string name)
        {
            name = Regex.Replace((name ?? ""), "[\\:*?\"<>|]+", "");
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name + "_";
                if (name.Length > 20) { name = name.Substring(0, 20); }
            }
            return name + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
        }

        private static string _exportFileFolder;
        public static string ExportFileFolder
        {
            get
            {
                if (_exportFileFolder == null)
                {
                    string path = (ConfigurationManager.AppSettings["ExportFileFolder"] ?? "Temp").Trim();
                    if (path.IndexOf(":") > 0)
                    {
                        _exportFileFolder = path;
                    }
                    else if (path.StartsWith("~"))
                    {
                        _exportFileFolder = System.Web.HttpContext.Current.Server.MapPath(path);
                    }
                    else
                    {
                        path = path.TrimStart('\\', '/');
                        _exportFileFolder = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, path);
                    }
                    if (!Directory.Exists(_exportFileFolder))
                    {
                        Directory.CreateDirectory(_exportFileFolder);
                    }
                }
                return _exportFileFolder;

            }
        }

        public static string GetExportFileUrl(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return "";
            string path = (ConfigurationManager.AppSettings["ExportFileBaseUrl"] ?? "~/Temp").Trim();
            return path.TrimEnd('/', '\\') + "/" + fileName.TrimStart('/', '\\');
        }

        public static string GetFilePath(string fileName)
        {
            string filePath = Path.Combine(ExportFileFolder, fileName);
            string d = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(d))
            {
                Directory.CreateDirectory(d);
            }
            return filePath;
        }

        public static void SaveFile(string filePath, byte[] fileData)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                stream.Write(fileData, 0, fileData.Length);
            }
        }
    }

    public class ExcelFileExporter<T> : IFileExporter<T> where T : class
    {
        public ExcelFileExporter()
        {
        }
        public virtual string Export(List<T> data, IList<ColumnSetting<T>> columnMap, string title = null)
        {
            XlsDocument xlsDoc = new XlsDocument();
            if (data != null)
            {
                BuildSheet(xlsDoc, 1, data, columnMap, title);
            }
            string fileName = ExcelFileExporterHelper.GetFileName(title);
            xlsDoc.FileName = fileName;
            xlsDoc.Save(ExcelFileExporterHelper.ExportFileFolder, true);
            return fileName;
        }

        protected virtual void BuildSheet(XlsDocument xlsDoc, int sheetIndex, List<T> data, IList<ColumnSetting<T>> columnMap, string title)
        {
            int startDataRowIndex;
            Worksheet worksheet = CreateSheet(xlsDoc, sheetIndex, columnMap, title, out startDataRowIndex);

            BuildSheetData(xlsDoc, worksheet, startDataRowIndex, data, columnMap);

        }

        protected virtual Worksheet CreateSheet(XlsDocument xlsDoc, int sheetIndex, IList<ColumnSetting<T>> columnMap, string title, out int startDataRowIndex)
        {
            Worksheet worksheet = xlsDoc.Workbook.Worksheets.Count > 0 ? xlsDoc.Workbook.Worksheets[0] : xlsDoc.Workbook.Worksheets.Add("Sheet" + sheetIndex.ToString());
            int headerRowIndex = 1;
            //if (string.IsNullOrWhiteSpace(templateExcelPath))
            //{
            if (!string.IsNullOrWhiteSpace(title))
            {
                ExcelFileExporterHelper.MergeRegion(worksheet, ExcelFileExporterHelper.GetSheetTitleXF(xlsDoc), title.Trim(), headerRowIndex, 1, headerRowIndex, columnMap.Count);
                headerRowIndex = headerRowIndex + 1;
            }
            var xf = ExcelFileExporterHelper.GetDataCellXF(xlsDoc);
            int excelColIndex = 0;
            foreach (var col in columnMap)
            {
                excelColIndex++;
                worksheet.Cells.Add(headerRowIndex, excelColIndex, col.ColumnName, xf);
            }

            decimal[] rstContainer = new decimal[columnMap.Count];
            headerRowIndex = headerRowIndex + 1;
            startDataRowIndex = headerRowIndex;
            return worksheet;
        }

        protected virtual void BuildSheetData(XlsDocument xlsDoc, Worksheet worksheet, int startRowIndex, List<T> data, IList<ColumnSetting<T>> columnMap)
        {
            var xf = ExcelFileExporterHelper.GetDataCellXF(xlsDoc);
            int j = 0;
            for (j = 0; j < data.Count; j++)
            {
                for (int c = 1; c <= columnMap.Count; c++)
                {
                    var columnSetting = columnMap[c - 1];
                    object d = columnSetting.GetPropertyValue(data[j]);
                    worksheet.Cells.Add(j + startRowIndex, c, d, xf);
                }
            }
        }
    }

    public class ExcelFileExporter : IDataTableFileExporter
    {
        public ExcelFileExporter()
        {
        }
        public string Export(DataTable table, IList<ColumnSetting> columnMap)
        {
            return Export(null, table, columnMap);
        }

        protected void BuildSheet(XlsDocument xlsDoc, int sheetIndex, string title, DataTable data, IList<ColumnSetting> columnMap)
        {
            Worksheet worksheet = xlsDoc.Workbook.Worksheets.Add("Sheet" + sheetIndex.ToString());
            int headerRowIndex = 1;
            if (string.IsNullOrWhiteSpace(title))
            {
                title = data.TableName;
            }
            if (!string.IsNullOrWhiteSpace(title))
            {
                ExcelFileExporterHelper.MergeRegion(worksheet, ExcelFileExporterHelper.GetSheetTitleXF(xlsDoc), title, headerRowIndex, 1, headerRowIndex, columnMap.Count);
                headerRowIndex = headerRowIndex + 1;
            }
            int excelColIndex = 0;
            var xf = ExcelFileExporterHelper.GetDataCellXF(xlsDoc);
            foreach (var col in columnMap)
            {
                excelColIndex++;
                worksheet.Cells.Add(headerRowIndex, excelColIndex, col.ColumnName, xf);
            }

            decimal[] rstContainer = new decimal[columnMap.Count];
            headerRowIndex = headerRowIndex + 1;
            BuildSheetData(xlsDoc, worksheet, headerRowIndex, data, columnMap);

        }

        protected virtual void BuildSheetData(XlsDocument xlsDoc, Worksheet worksheet, int startRowIndex, DataTable data, IList<ColumnSetting> columnMap)
        {
            var xf = ExcelFileExporterHelper.GetDataCellXF(xlsDoc);
            int j = 0;
            for (j = 0; j < data.Rows.Count; j++)
            {
                for (int c = 1; c <= columnMap.Count; c++)
                {
                    var columnSetting = columnMap[c - 1];
                    object d = columnSetting.GetPropertyValue(data.Rows[j]);

                    worksheet.Cells.Add(j + startRowIndex, c, d, xf);
                }
            }
        }

        public string Export(string title, DataTable table, IList<ColumnSetting> columnMap)
        {
            XlsDocument xlsDoc = new XlsDocument();

            if (table != null && table.Rows.Count > 0)
            {
                BuildSheet(xlsDoc, 1, title, table, columnMap);
            }
            string fileName = ExcelFileExporterHelper.GetFileName(table.TableName);
            xlsDoc.FileName = fileName;
            xlsDoc.Save(ExcelFileExporterHelper.ExportFileFolder, true);
            return fileName;
        }
    }

}
