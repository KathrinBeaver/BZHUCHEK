using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Data.SQLite;
using System.IO;

namespace наВинду
{
    /// <summary>
    /// Логика взаимодействия для ChoiceDiet.xaml
    /// </summary>
    public partial class ChoiceDiet : Window
    {
        public ChoiceDiet()
        {
            InitializeComponent();
        }

        //Для работы с SQLite базой данных
        private static String basePersonalData = "PersonalData.sql";
        private static SQLiteConnection connectBD;
        private SQLiteCommand commandSaveDietId = new SQLiteCommand();
        
        // Переход на страницу "Личные данные"
        private void PersonalDataPage_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow personalData = new MainWindow();
            personalData.Left = (100);
            personalData.Top = (100);
            personalData.Show();
        }

        //получение связи с бд
        private void ConnectBd(Int32 idDiet)
        {
            try
            {
                connectBD = new SQLiteConnection("Data Source=" + basePersonalData + ";Version=3;" + "UseUTF16Encoding = True;");
                connectBD.Open();
                commandSaveDietId.Connection = connectBD;
                commandSaveDietId.CommandText = "UPDATE user SET diet_id = '" + idDiet + "'";
                commandSaveDietId.ExecuteNonQuery();
                connectBD.Close();
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Соединение с БД не получено!", "Предупреждение");
            }
            ChoiceOfActivityCoefficient choiceOfActivityCoefficient = new ChoiceOfActivityCoefficient();
            choiceOfActivityCoefficient.Left = (100);
            choiceOfActivityCoefficient.Top = (100);
            choiceOfActivityCoefficient.Show();
        }

        // Сохранение типа диеты (диета БУЧ)
        private void ChoiceBUCH_Click(object sender, RoutedEventArgs e)
        {
            Int32 id = 1;
            ConnectBd(1);
        }

        // Переход на страницу "Дневник питания"
        private void FoodDiaryPage_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            FoodDiary foodDiaryPage = new FoodDiary();
            foodDiaryPage.Left = (100);
            foodDiaryPage.Top = (100);
            foodDiaryPage.Show();
        }

        private void ChoiceDietPage_Click(object sender, RoutedEventArgs e)
        {

        }

        // Сохранение типа диеты (диета Низкокалорийная)
        private void ChOiceNizkoKal_Click_1(object buttonSedenteryLifeStyle, RoutedEventArgs e)
        {
            Int32 id = 2;
            ConnectBd(2);
        }

        // Сохранение типа диеты (диета Белковая)
        private void ChOiceBelk_Click_1(object sender, RoutedEventArgs e)
        {
            Int32 id = 3;
            ConnectBd(3);
        }

        // Сохранение типа диеты (диета Набор массы)
        private void ChOiceNabor_Click_1(object sender, RoutedEventArgs e)
        {
            Int32 id = 4;
            ConnectBd(4);
        }

        // Сохранение типа диеты (диета Атомная)
        private void ChOiceAtom_Click_1(object sender, RoutedEventArgs e)
        {
            Int32 id = 5;
            ConnectBd(5);
        }
    }
}
