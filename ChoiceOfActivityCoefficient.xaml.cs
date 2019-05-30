using System;
using System.Collections.Generic;
using System.Data.SQLite;
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

namespace наВинду
{
    /// <summary>
    /// Логика взаимодействия для ChoiceOfActivityCoefficient.xaml
    /// </summary>
    public partial class ChoiceOfActivityCoefficient : Window
    {
        public ChoiceOfActivityCoefficient()
        {
            InitializeComponent();
        }

        //Для работы с БД
        public static String basePersonalData = "PersonalData.sql";
        public static SQLiteConnection connectBD;
        public SQLiteCommand commandGetCoefOfActive = new SQLiteCommand();
        private String coefficient;

        //Сохранение коэффициентов
        private void SaveCoefficient(String coef)
        {
            try
            {
                connectBD = new SQLiteConnection("Data Source=" + basePersonalData + ";Version=3;" + "UseUTF16Encoding = True;");
                connectBD.Open();
                commandGetCoefOfActive.Connection = connectBD;
                commandGetCoefOfActive.CommandText = "UPDATE user SET coefficient_of_active = '" + coef + "'";
                commandGetCoefOfActive.ExecuteNonQuery();
                connectBD.Close();
                MessageBox.Show("Диета выбрана!");
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Соединение с БД не получено!", "Предупреждение");
            }
        }

        //Сохранение коэффициета физичсекой активности (1.2)
        private void ButtonSedenteryLifeStyle_Click(object sender, RoutedEventArgs e)
        {
            coefficient = "1.2";
            SaveCoefficient(coefficient);
        }

        //Сохранение коэффициета физичсекой активности (1.3)
        private void ButtonAverangeActivity_Click(object sender, RoutedEventArgs e)
        {
            coefficient = "1.3";
            SaveCoefficient(coefficient);
        }

        //Сохранение коэффициета физичсекой активности (1.6)
        private void ButtonHighActivity_Click(object sender, RoutedEventArgs e)
        {
            coefficient = "1.6";
            SaveCoefficient(coefficient);
        }

        //Сохранение коэффициета физичсекой активности (1.7)
        private void ButtonExtraHighActivity_Click(object sender, RoutedEventArgs e)
        {
            coefficient = "1.7";
            SaveCoefficient(coefficient);
        }

        //Сохранение коэффициета физичсекой активности (1.9)
        private void ButtonHeavyActivity_Click(object sender, RoutedEventArgs e)
        {
            coefficient = "1.9";
            SaveCoefficient(coefficient);
        }
    }
}
