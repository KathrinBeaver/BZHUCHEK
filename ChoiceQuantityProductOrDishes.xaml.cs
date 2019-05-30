using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для ChoiceQuantityProductOrDishes.xaml
    /// </summary>
    public partial class ChoiceQuantityProductOrDishes : Window
    {
        public ChoiceQuantityProductOrDishes(Int32 idReceptionFood, String dateOfReceptionFood, string nameOfChoice)
        {
            InitializeComponent();
            this.idReceptionFood = idReceptionFood;
            this.dateOfReceptionFood = dateOfReceptionFood;
            this.nameOfChoice = nameOfChoice;
            SearchProductOrDishInBD();
        }
        Int32 idReceptionFood;
        String dateOfReceptionFood;
        String nameOfChoice;
        TextBox textBoxIdReceptionFood = new TextBox();
        TextBox textBoxDateOfReceptionFood = new TextBox();

        private static String basePersonalData = "PersonalData.sql";
        private static SQLiteConnection connectBD;
        private SQLiteCommand command;

        private Double ccal;
        private Double proteins;
        private Double fats;
        private Double carbohydrates;
        private Double servingSize;
        private String units;

        private Double ccalS;
        private Double proteinsS;
        private Double fatsS;
        private Double carbohydratesS;
        private Double servingSizeS;
        private String unitsOfPDS;

        private Double servingSizeInSelectedProdDish = 0;
        private Double ccalInSelectedProdDish;
        private Double proteinsInSelectedProdDish;
        private Double fatsInSelectedProdDish;
        private Double carbohydratesInSelectedProdDish;

        private bool indexExecution;

        //Поиск продукта или блюда по имени в бд
        private void SearchProductOrDishInBD()
        {
            String type = "product";
            SearchFromBD(type);
            if (indexExecution == false)
            {
                type = "dish";
                SearchFromBD(type);
            }
        }

        //Проведение поиска
        private void SearchFromBD(String typeOfProduct)
        {
            try
            {
                ConnectBd();

                command.CommandText = "SELECT ccal FROM '" + typeOfProduct + "' WHERE name = '" + nameOfChoice + "'";
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ccal = Convert.ToDouble(reader.GetValue(0));
                    reader.Close();

                    command.CommandText = "SELECT proteins FROM '" + typeOfProduct + "' WHERE name = '" + nameOfChoice + "'";
                    reader = command.ExecuteReader();
                    reader.Read();
                    proteins = Convert.ToDouble(reader.GetValue(0));
                    reader.Close();

                    command.CommandText = "SELECT fats FROM '" + typeOfProduct + "' WHERE name = '" + nameOfChoice + "'";
                    reader = command.ExecuteReader();
                    reader.Read();
                    fats = Convert.ToDouble(reader.GetValue(0));
                    reader.Close();

                    command.CommandText = "SELECT carbohydrates FROM '" + typeOfProduct + "' WHERE name = '" + nameOfChoice + "'";
                    reader = command.ExecuteReader();
                    reader.Read();
                    carbohydrates = Convert.ToDouble(reader.GetValue(0));
                    reader.Close();

                    command.CommandText = "SELECT serving_size FROM '" + typeOfProduct + "' WHERE name = '" + nameOfChoice + "'";
                    reader = command.ExecuteReader();
                    reader.Read();
                    servingSize = Convert.ToDouble(reader.GetValue(0));
                    reader.Close();

                    command.CommandText = "SELECT units FROM '" + typeOfProduct + "' WHERE name = '" + nameOfChoice + "'";
                    reader = command.ExecuteReader();
                    reader.Read();
                    units = Convert.ToString(reader.GetValue(0));
                    reader.Close();

                    connectBD.Close();
                    indexExecution = true;
                }
                else
                {
                    indexExecution = false;
                    connectBD.Close();
                }
             }
             catch (SQLiteException)
             {
                 MessageBox.Show("Нет соединения!", "Окно ChoiceQuantityProductOrDish");
             }
            DisplayData();
        }

        //Получение связи с бд
        private void ConnectBd()
        {
            connectBD = new SQLiteConnection("Data Source=" + basePersonalData + ";Version=3;" + "UseUTF16Encoding = True;");
            connectBD.Open();
            command = new SQLiteCommand();
            command.Connection = connectBD;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        //Нажатие кнопки "Отмена"
        private void ButtonForBack_Click(object sender, RoutedEventArgs e)
        {
            ProductSelection productSelectionPage = new ProductSelection(idReceptionFood, dateOfReceptionFood);
            productSelectionPage.Show();
            this.Hide();
            productSelectionPage.Left = (100);
            productSelectionPage.Top = (100);
        }

        //Вывод данных на экран
        private void DisplayData()
        {
            labelForNameProductOrDish.Content = nameOfChoice;
            LabelUnit.Content = units;
        }

        //Расчет кбжу выбранного продукта/блюда
        private void CalculateCPFCOfProdDish()
        {
            servingSizeInSelectedProdDish = Convert.ToDouble(textBoxForQuantityProductOrDish.Text);
            ccalInSelectedProdDish = (servingSizeInSelectedProdDish * ccal) / servingSize;
            proteinsInSelectedProdDish = (servingSizeInSelectedProdDish * proteins) / servingSize;
            fatsInSelectedProdDish = (servingSizeInSelectedProdDish * fats) / servingSize;
            carbohydratesInSelectedProdDish = (servingSizeInSelectedProdDish * carbohydrates) / servingSize;
            textBoxForQuantityProductOrDish.Clear();
        }

        //Создание таблицы для для завтрака/обеда/ужина/перекусов
        private void ChoiceOfTypeOfReceptionOfFood()
        {
            switch(idReceptionFood)
            {
                case 1:
                    String breakfast = "breakfast_in_date";
                    SearchSameProd(breakfast);
                    break;
                case 2:
                    String dinner = "dinner_in_date";
                    SearchSameProd(dinner);
                    break;
                case 3:
                    String supper = "supper_in_date";
                    SearchSameProd(supper);
                    break;
                case 4:
                    String nosh = "nosh_in_date";
                    SearchSameProd(nosh);
                    break;
            }
        }

        //Подключение и создание таблиц для завтрака/обеда/ужина/перекусов
        private void CreateTableForBreakfastSupperDinnerNosh(String date, String receptionFood)
        {
            ConnectBd();
            try
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS '" + receptionFood + "' (name_of_product STRING, quantity DOUBLE, data_num DATE, ccal DOUBLE, proteins DOUBLE, fats DOUBLE, carbohydrates DOUBLE)";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO '" + receptionFood + "' ('name_of_product', 'quantity', 'data_num', 'ccal', 'proteins', 'fats', 'carbohydrates') values ('" + nameOfChoice + "','" + servingSizeInSelectedProdDish + "', '" + date + "', '" + ccalInSelectedProdDish + "', '" + proteinsInSelectedProdDish + "', '" + fatsInSelectedProdDish + "', '" + carbohydratesInSelectedProdDish + "') ";
                command.ExecuteNonQuery();
                connectBD.Close();
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Соединение с БД не получено!", "Окно ChoiceQuantityProductOrDish");
            }
        }

        //Расчет и сохранение данных КБЖУ в бд
        private void ButtonForAdd_Click(object sender, RoutedEventArgs e)
        {
            int indexMistake = 0;
            try
            {
                CalculateCPFCOfProdDish();
            }
            catch (FormatException)
            {
                MessageBox.Show("Вводите числа!");
                textBoxForQuantityProductOrDish.Clear();
                indexMistake++;
            }
            if (indexMistake == 0)
            {
                ConnectBd();
                ChoiceOfTypeOfReceptionOfFood();
                MessageBox.Show("Продукт добавлен!");
                this.Hide();
            }
        }

        //Поиск сохраненного в приме пищи продукта, совпадающего с существующим 
        private void SearchSameProd(string receptionFood)
        {
            try
            {
                ConnectBd();
                command.CommandText = "SELECT * FROM '" + receptionFood + "' WHERE data_num = '" + dateOfReceptionFood + "' AND name_of_product = '" + nameOfChoice + "'";
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    reader.Close();
                    SearchDataSameProd(receptionFood);
                    ccalInSelectedProdDish += ccalS;
                    proteinsInSelectedProdDish += proteins;
                    fatsInSelectedProdDish += fatsS;
                    carbohydratesInSelectedProdDish += carbohydratesS;
                    servingSizeInSelectedProdDish += servingSizeS;
                    SaveChanges(receptionFood);
                }
                else
                {
                    ConnectBd();
                    CreateTableForBreakfastSupperDinnerNosh(dateOfReceptionFood, receptionFood);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Поиск совпадения не произошел!");
            }
        }

        //Сохранение изменений в таблице приема пищи
        private void SaveChanges(string recF)
        {
            try
            {
                ConnectBd();
                command.CommandText = "UPDATE '" + recF + "' SET quantity = '" + servingSizeInSelectedProdDish + "', ccal = '" + ccalInSelectedProdDish + "', proteins = '" + proteinsInSelectedProdDish + "', fats = '" + fatsInSelectedProdDish + "', carbohydrates = '" + carbohydratesInSelectedProdDish + "' WHERE name_of_product = '" + nameOfChoice + "' AND data_num = '" + dateOfReceptionFood + "'";
                command.ExecuteNonQuery();
                connectBD.Close();
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Соединение с БД не получено!", "Окно ChoiceQuantityProductOrDish");
            }
        }

        //Проведение поиска совпадающего по названию продукта
        private void SearchDataSameProd(string receptionOfF)
        {
            command.CommandText = "SELECT quantity FROM '" + receptionOfF + "' WHERE name_of_product = '" + nameOfChoice + "' AND data_num = '" + dateOfReceptionFood + "'";
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                servingSize = Convert.ToDouble(reader.GetValue(0));
                reader.Close();

                command.CommandText = "SELECT ccal FROM '" + receptionOfF + "' WHERE name_of_product = '" + nameOfChoice + "' AND data_num = '" + dateOfReceptionFood + "'";
                reader = command.ExecuteReader();
                reader.Read();
                ccalS = Convert.ToDouble(reader.GetValue(0));
                reader.Close();

                command.CommandText = "SELECT proteins FROM '" + receptionOfF + "' WHERE name_of_product = '" + nameOfChoice + "' AND data_num = '" + dateOfReceptionFood + "'";
                reader = command.ExecuteReader();
                reader.Read();
                proteinsS = Convert.ToDouble(reader.GetValue(0));
                reader.Close();

                command.CommandText = "SELECT fats FROM '" + receptionOfF + "' WHERE name_of_product = '" + nameOfChoice + "' AND data_num = '" + dateOfReceptionFood + "'";
                reader = command.ExecuteReader();
                reader.Read();
                fatsS = Convert.ToDouble(reader.GetValue(0));
                reader.Close();

                command.CommandText = "SELECT carbohydrates FROM '" + receptionOfF + "' WHERE name_of_product = '" + nameOfChoice + "' AND data_num = '" + dateOfReceptionFood + "'";
                reader = command.ExecuteReader();
                reader.Read();
                carbohydratesS = Convert.ToDouble(reader.GetValue(0));
                reader.Close();
                connectBD.Close();
            }
            else
            {
                reader.Close();
                connectBD.Close();
            }
        }
    }
}
