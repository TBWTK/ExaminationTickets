using System;
using Excel = Microsoft.Office.Interop.Excel;


namespace ExaminationTickets
{
    class ExcelDocument
    {
        private Excel.Application _application = null;
        private Excel.Workbook _workBook = null;
        private Excel.Worksheet _workSheet = null;
        private object _missingObj = System.Reflection.Missing.Value;

        public ExcelDocument(string pathToTemplate)
        {
            object pathToTemplateObj = pathToTemplate;

            _application = new Excel.Application();
            _workBook = _application.Workbooks.Add(pathToTemplateObj);
            _workSheet = (Excel.Worksheet)_workBook.Worksheets.get_Item(1);
        }

        public int usedRowsNum
        {
            get => _workSheet.UsedRange.Rows.Count;
        }

        public string GetCellValue(int rowIndex, int columnIndex)
        {
            string cellValue = "";

            Excel.Range cellRange = (Excel.Range)_workSheet.Cells[rowIndex, columnIndex];
            if (cellRange.Value != null)
            {
                cellValue = cellRange.Value.ToString();
            }
            return cellValue;
        }

        public void Close()
        {
            _workBook.Close(false, _missingObj, _missingObj);
            _application.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(_application);
            _application = null;
            _workBook = null;
            _workSheet = null;
            GC.Collect();
        }
    }
}
