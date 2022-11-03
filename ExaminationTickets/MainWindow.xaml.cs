using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExaminationTickets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // для перезаписи ячеек дат
        private string yearDateExam = "";
        private string dayDateExam = "";
        private string monthDateExam = "";

        private string yearDateEvent = "";
        private string dayDateEvent = "";
        private string monthDateEvent = "";

        //Количество вопросов и задач
        private int quantityQuestion = 0;
        private int quantityTask = 0;

        WordDocument wordDocument = null;
        ExcelDocument excelDocument = null;
        FileInfo pathToExcel;
        FileInfo pathToWord;

        // Списки для вопросов и задач, считаных с файла
        private List<string> questions;
        private List<string> tasks;

        // Списки для готовых пар рандомынх вопросов
        private List<List<string>> randomQuestions;
        private List<List<string>> randomTasks;

        // Вариации шаблонов билетов: 1 вопрос/1 вопрос 1 задача/1 вопроса 2 задачи/2 вопроса/2 вопроса 1 задача/2 вопроса 2 задачи
        // вопрос = 1
        // задача = 2
        // максимальное значение 6
        private int variationsTemplates;

        // Переменная общее количество возможных вопросов
        private long totalQuestions;
        // Переменная общее количество возможных задач
        private long totalTasks;

        public MainWindow()
        {
            InitializeComponent();
            string fileNameExcel = "Templates\\Tasks.xlsx";
            string fileNameWord = "Templates\\ExamTicket.docx";

            questions = new List<string>();
            tasks = new List<string>();

            randomQuestions = new List<List<string>>();
            randomTasks = new List<List<string>>();

            pathToExcel = new FileInfo(fileNameExcel);
            pathToWord = new FileInfo(fileNameWord);


            if (pathToExcel.Exists)
            {
                excelDocument = new ExcelDocument(pathToExcel.FullName);
                int excelCellB = 2; // Колона с вопросами
                int excelCellC = 3; // Колона с задачами

                for (int i = 2; i <= excelDocument.usedRowsNum; i++)
                {
                    if (excelDocument.GetCellValue(i, excelCellB) != "")
                        questions.Add(excelDocument.GetCellValue(i, excelCellB));
                    if (excelDocument.GetCellValue(i, excelCellC) != "")
                        tasks.Add(excelDocument.GetCellValue(i, excelCellC));
                }
                excelDocument.Close();
            }
            else
            {
                MessageBox.Show("Приложение автоматически закроется, обратитесь к разработчику! Приложение не доступно");
                this.Close();
            }
        }

        private void DateExam_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateExam.SelectedDate != null)
            {
                dayDateExam = Convert.ToString(DateExam.SelectedDate.Value.Date.Day);
                monthDateExam = ChoiceMonth(DateExam.SelectedDate.Value.Date.Month);
                yearDateExam = Convert.ToString(DateExam.SelectedDate.Value.Date.Year);
            }
            if (DateEvent.SelectedDate != null)
            {
                dayDateEvent = Convert.ToString(DateEvent.SelectedDate.Value.Date.Day);
                monthDateEvent = ChoiceMonth(DateEvent.SelectedDate.Value.Date.Month);
                yearDateEvent = Convert.ToString(DateEvent.SelectedDate.Value.Date.Year);
            }
        }

        private string ChoiceMonth(int x)
        {
            switch (x)
            {
                case 1:
                    return "января";
                case 2:
                    return "февраля";
                case 3:
                    return "марта";
                case 4:
                    return "апреля";
                case 5:
                    return "мая";
                case 6:
                    return "июня";
                case 7:
                    return "июля";
                case 8:
                    return "августа";
                case 9:
                    return "сентября";
                case 10:
                    return "октября";
                case 11:
                    return "ноября";
                case 12:
                    return "декабря";
                default:
                    return "";
            }
        }

        private void ClearItem()
        {
            DateExam.SelectedDate = null;
            ProfileTextBox.Text = "";
            SpecializationTextBox.Text = "";
            CourseTextBox.Text = "";
            GroupTextBox.Text = "";
            TermTextBox.Text = "";
            TeacherTextBox.Text = "";
            MeetingTextBox.Text = "";
            ProtocolTextBox.Text = "";
            ChairmanTextBox.Text = "";
            quentityTicketTextBox.Text = "";
            neNameFileTextBox.Text = "";
            CountVariationsTemplates.Text = "0";
        }


        // Функция подсчета факториала
        private long factorial(int number)
        {
            if (number == 1)
                return 1;
            else
                return number * factorial(number - 1);
        }
        //перемешка вопросов и задач
        private void shuffleQuestionsOfOne()
        {
            questions = Randomize(questions);
            questions = Randomize(questions);
        }
        private void shuffleTasksOfOne()
        {
            tasks = Randomize(tasks);
            tasks = Randomize(tasks);
        }

        // Генерация списка вопросов и задач
        private void generatingQuestionsOfTwo()
        {
            for (int i = 0; i < questions.Count(); i++)
            {
                int j = i;
                while (j < questions.Count() - 1)
                {
                    j += 1;
                    var tempList = new List<string>();
                    tempList.Add(questions[i]);
                    tempList.Add(questions[j]);
                    randomQuestions.Add(tempList);
                }
            }
            shuffleQuestionsOfTwo();
        }
        private void generatingTasksOfTwo()
        {
            for (int i = 0; i < tasks.Count(); i++)
            {
                int j = i;
                while (j < tasks.Count() - 1)
                {
                    j += 1;
                    var tempList = new List<string>();
                    tempList.Add(tasks[i]);
                    tempList.Add(tasks[j]);
                    randomTasks.Add(tempList);
                }
            }
            shuffleTasksOfTwo();
        }
        // Перемешка списков
        private void shuffleQuestionsOfTwo()
        {

            for (int i = 0; i < randomQuestions.Count; i++)
            {
                if (i % 2 == 0)
                {
                    var temp = randomQuestions[i][0];
                    randomQuestions[i][0] = randomQuestions[i][1];
                    randomQuestions[i][1] = temp;
                }
            }
            randomQuestions = Randomize(randomQuestions);
        }
        private void shuffleTasksOfTwo()
        {

            for (int i = 0; i < randomTasks.Count; i++)
            {
                if (i % 2 == 0)
                {
                    var temp = randomTasks[i][0];
                    randomTasks[i][0] = randomTasks[i][1];
                    randomTasks[i][1] = temp;
                }
            }
            randomTasks = Randomize(randomTasks);
        }
        private List<T> Randomize<T>(List<T> list)
        {
            List<T> randomizedList = new List<T>();
            Random rnd = new Random();
            while (list.Count > 0)
            {
                int index = rnd.Next(0, list.Count);
                randomizedList.Add(list[index]);
                list.RemoveAt(index);
            }
            return randomizedList;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton pressed = (RadioButton)sender;
            string tempChoice = pressed.Content.ToString();
            MessageBox.Show("Вы выбрали создать билет из:\n" + tempChoice);
            switch (tempChoice)
            {
                case "Вопросов: 1 | Задач: 0":
                    quantityQuestion = 1;
                    quantityTask = 0;
                    break;
                case "Вопросов: 1 | Задач: 1":
                    quantityQuestion = 1;
                    quantityTask = 1;
                    break;
                case "Вопросов: 1 | Задач: 2":
                    quantityQuestion = 1;
                    quantityTask = 2;
                    break;
                case "Вопросов: 2 | Задач: 0":
                    quantityQuestion = 2;
                    quantityTask = 0;
                    break;
                case "Вопросов: 2 | Задач: 1":
                    quantityQuestion = 2;
                    quantityTask = 1;
                    break;
                case "Вопросов: 2 | Задач: 2":
                    quantityQuestion = 2;
                    quantityTask = 2;
                    break;
            }

            variationsTemplates = quantityQuestion * 1 + quantityTask * 2;

            //MessageBox.Show($"{variationsTemplates} {quantityQuestion} {quantityTask}");
            long x1 = factorial(questions.Count());
            long x2 = factorial(quantityQuestion) * factorial(questions.Count() - quantityQuestion);
            totalQuestions = x1 / x2;
            if (quantityQuestion == 1)
                shuffleQuestionsOfOne();
            else
                generatingQuestionsOfTwo();

            if (quantityTask != 0)
            {
                x1 = factorial(tasks.Count());
                x2 = factorial(quantityTask) * factorial(tasks.Count() - quantityTask);
                totalTasks = x1 / x2;
                if (quantityTask == 1)
                    shuffleTasksOfOne();
                else
                    generatingTasksOfTwo();
            }
            if (totalTasks == 0)
            {
                CountVariationsTemplates.Text = $"{totalQuestions}";
            }
            else
            {
                CountVariationsTemplates.Text = $"{totalQuestions * totalTasks}";

            }
            totalQuestions = 0;
            totalTasks = 0;

        }

        private void GenerateReportWordButton_Click(object sender, RoutedEventArgs e)
        {
            if (quentityTicketTextBox.Text != "" && Convert.ToInt32(quentityTicketTextBox.Text) <= Convert.ToInt32(CountVariationsTemplates.Text) )
            {
                if (dayDateExam != "" && ProfileTextBox.Text != "" && SpecializationTextBox.Text != ""
                    && CourseTextBox.Text != "" && GroupTextBox.Text != "" && TeacherTextBox.Text != ""
                    && MeetingTextBox.Text != "" && ProtocolTextBox.Text != "" && ChairmanTextBox.Text != ""
                    && neNameFileTextBox.Text != "")
                {
                    CreateFileWithTickets(Convert.ToInt32(quentityTicketTextBox.Text));
                    ClearItem();
                }
                else
                {
                    MessageBox.Show("Введите вспомогательную информацию!");
                }
            }
            else
            {
                MessageBox.Show("Введите количество билетов!");
            }
        }

        private void CreateFileWithTickets(int countTicket)
        {
            wordDocument = new WordDocument(pathToWord.FullName);


            int itemQuestion = 0;
            int itemQTask = 0;

            for (int i = 0; i < countTicket; i++)
            {
                // Производим выбор нужного шаблона и заполняем словарь нужными данными
                string fileNameTicket = "Templates\\Template.docx";


                var tempList = new List<string>();
                if (itemQuestion >= questions.Count() - 1)
                    itemQuestion = 0;
                if (randomQuestions.Count() != 0)
                    if (itemQTask >= randomQuestions.Count() - 1)
                        itemQTask = 0;
                
                
                if (itemQTask >= tasks.Count() - 1)
                    itemQTask = 0;
                if(randomTasks.Count() != 0)
                    if(itemQTask >= tasks.Count() - 1)
                        itemQTask = 0;

                // Условная кострукция для выбора шаблона
                switch (variationsTemplates)
                {
                    case 1:
                        {
                            tempList.Add(questions[itemQuestion]);
                            break;
                        }
                    case 2:
                        {
                            tempList.Add(randomQuestions[itemQuestion][0]);
                            tempList.Add(randomQuestions[itemQuestion][1]);
                            break;
                        }
                    case 3:
                        {
                            tempList.Add(questions[itemQuestion]);
                            tempList.Add(tasks[itemQTask]);
                            break;
                        }
                    case 4:
                        {
                            tempList.Add(randomQuestions[itemQuestion][0]);
                            tempList.Add(randomQuestions[itemQuestion][1]);
                            tempList.Add(tasks[itemQTask]);
                            break;
                        }
                    case 5:
                        {
                            tempList.Add(questions[itemQuestion]);
                            tempList.Add(randomTasks[itemQTask][0]);
                            tempList.Add(randomTasks[itemQTask][1]);
                            break;
                        }
                    case 6:
                        {
                            tempList.Add(randomQuestions[itemQuestion][0]);
                            tempList.Add(randomQuestions[itemQuestion][1]);
                            tempList.Add(randomTasks[itemQTask][0]);
                            tempList.Add(randomTasks[itemQTask][1]);
                            break;
                        }
                }
                // открываем шаблон
                FileInfo fileInfoTicket = new FileInfo(fileNameTicket);
                WordDocument ticket = new WordDocument(fileInfoTicket.FullName);


                ticket.ReplaceAllStrings("<ticket>", $"{i + 1}");
                ticket.Replace(tempList);
                // сохраняем и закрываем измененный шаблон
                ticket.Save(pathToWord.DirectoryName, "templateDone");
                ticket.Close();

                // добавляем готовый билет в основной документ
                string fileNameTicketDone = "Templates\\templateDone.docx";
                FileInfo fileInfoTicketDone = new FileInfo(fileNameTicketDone);
                wordDocument.InsertFile(fileInfoTicketDone.FullName);
                itemQuestion += 1;
                itemQTask += 1;
            }

            var specialValuesItems = new Dictionary<string, string>
            {
                { "<day>", dayDateExam},
                { "<month>", monthDateExam},
                { "<year>", yearDateExam},
                { "<d1>", dayDateEvent},
                { "<m1>", monthDateEvent},
                { "<y1>", yearDateEvent},
                { "<term>", TermTextBox.Text },
                { "<profile>", ProfileTextBox.Text },
                { "<specialization>", SpecializationTextBox.Text},
                { "<course>", CourseTextBox.Text},
                { "<group>", GroupTextBox.Text},
                { "<teacher>", TeacherTextBox.Text},
                { "<meeting>", MeetingTextBox.Text},
                { "<protocol>", ProtocolTextBox.Text},
                { "<chairman>", ChairmanTextBox.Text}
            };

            foreach (var item in specialValuesItems)
            {
                wordDocument.ReplaceAllStrings(item.Key, item.Value);
            }
            string projectPath = Environment.CurrentDirectory;
            projectPath += "\\ReadyTemplates";

            DirectoryInfo path = new DirectoryInfo(projectPath);

            wordDocument.Save(path.FullName, neNameFileTextBox.Text);
            wordDocument.Close();
            MessageBox.Show("Процесс выполнен!");
        }
    }
}
