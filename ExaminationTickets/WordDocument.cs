using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Word = Microsoft.Office.Interop.Word;

namespace ExaminationTickets
{
    class WordDocument
    {
        private Object _missingObj = System.Reflection.Missing.Value;
        private Object _trueObj = true;
        private Object _falseObj = false;

        private Word._Application _application;
        private Word._Document _document;

        private Object _templatePathObj;

        private Word.Range _currentRange = null;

        // В конструкторе создаются экземляры объектов и прописывается путь до файла
        public WordDocument(string templatePath)
        {
            _application = new Word.Application();
            _templatePathObj = templatePath;
            _document = _application.Documents.Add(ref _templatePathObj, ref _missingObj, ref _missingObj, ref _missingObj);
        }

        // Свойтво проверки доступности работы с файлов
        private bool Closed
        {
            get
            {
                if (_application == null || _document == null) { return true; }
                else { return false; }
            }
        }

        // Свойство для подсчета количества страниц 
        public int PagesCount
        {
            get
            {
                int pagesCount = 0;
                Word.WdStatistic pagesStatType = Word.WdStatistic.wdStatisticPages;
                pagesCount = _document.ComputeStatistics(pagesStatType, ref _missingObj);
                return pagesCount;
            }
        }
        public void Replace(List<string> temp)
        {
            if (Closed) { throw new Exception("Ошибка при обращении к документу Word. Документ уже закрыт."); }

            // диапазон документа Word
            try
            {
                Word.Range wordRange;
                for (int i = 1; i <= _document.Sections.Count; i++)
                {
                    // берем всю секцию диапазоном
                    wordRange = _document.Sections[i].Range;

                    if (wordRange.Find.Execute("<temp>"))
                    {
                        wordRange.Text = "";
                        Word.Paragraph para = wordRange.Paragraphs.Add();
                        int startOfList = wordRange.Start;
                        //para.Range.InsertBefore((1) + ". " + "ойся" + "\r");
                        for (int j = 0; j < temp.Count(); j++)
                        {
                            wordRange.Text += (j + 1) + ". " + temp[j] + "\r";
                        }
                        int endOfList = wordRange.End;
                        Word.Range listRange = _document.Range(startOfList, endOfList);
                        listRange.ListFormat.ApplyNumberDefault();
                        wordRange.InsertParagraphAfter();
                    }
                }


            }
            catch (Exception error)
            {
                throw new Exception("Ошибка при выполнении загрузки в документ Word.  " + error.Message + " (ReplaceAllStrings)");
            }
        }

        // Метод по замене экземляра строки на другой экземляр, предеаются параметрами
        public void ReplaceAllStrings(string strToFind, string replaceStr)
        {
            if (Closed) { throw new Exception("Ошибка при обращении к документу Word. Документ уже закрыт."); }

            // обьектные строки для Word
            object strToFindObj = strToFind;
            object replaceStrObj = replaceStr;
            // диапазон документа Word
            Word.Range wordRange;
            //тип поиска и замены
            object replaceTypeObj;

            replaceTypeObj = Word.WdReplace.wdReplaceAll;

            try
            {
                // обходим все разделы документа
                for (int i = 1; i <= _document.Sections.Count; i++)
                {
                    // берем всю секцию диапазоном
                    wordRange = _document.Sections[i].Range;

                    Word.Find wordFindObj = wordRange.Find;


                    object[] wordFindParameters = new object[15] { strToFindObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, replaceStrObj, replaceTypeObj, _missingObj, _missingObj, _missingObj, _missingObj };

                    wordFindObj.GetType().InvokeMember("Execute", BindingFlags.InvokeMethod, null, wordFindObj, wordFindParameters);
                }
            }
            catch (Exception error)
            {
                throw new Exception("Ошибка при выполнении замене всех строк  в документе Word.  " + error.Message + " (ReplaceAllStrings)");
            }
        }

        public void InsertFile(string pathToFile)
        {
            _document.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);

            object missing = Missing.Value;
            object what = Word.WdGoToItem.wdGoToLine;
            object which = Word.WdGoToDirection.wdGoToLast;
            _currentRange = _document.GoTo(ref what, ref which, ref missing, ref missing);

            if (_currentRange == null) { throw new Exception("Ничего не выбрано"); }
            _currentRange.InsertFile(pathToFile);
            _currentRange.InsertParagraphAfter();


        }

        // Метод сохранения файла
        public void Save(string pathToSave, string newFileName)
        {
            Object pathToSaveObj = System.IO.Path.Combine(pathToSave, newFileName);
            _document.SaveAs(ref pathToSaveObj, Word.WdSaveFormat.wdFormatXMLDocument, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj);
        }

        // Удалить используемые экземляры Ms Word из задач, и они не стали фоновыми не закрытми процессами
        public void Close()
        {
            if (_document != null)
            {
                _document.Close(ref _falseObj, ref _missingObj, ref _missingObj);
            }
            _application.Quit(ref _missingObj, ref _missingObj, ref _missingObj);
            _document = null;
            _application = null;
        }
    }
}
