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
    /// Логика взаимодействия для ChangeOrDeleteProdDish.xaml
    /// </summary>
    public partial class ChangeOrDeleteProdDish : Window
    {
        public ChangeOrDeleteProdDish(String nameOfProdDish, String receptionFood, String dateOfReceptionFood, FoodDiary fD)
        {
            InitializeComponent();
            this.nameOfProdDish = nameOfProdDish;
            this.receptionFood = receptionFood;
            this.dateOfReceptionFood = dateOfReceptionFood;
            this.fD = fD;
            SearchUnitsOfProductOrDishInBD();
            SearchDataOfProdOrDishInBD();
        }
        String nameOfProdDish;
        String receptionFood;
        String dateOfReceptionFood;
        FoodDiary fD;

        private static String basePersonalData = "PersonalData.sql";
        private static SQLiteConnection connectBD;
        private SQLiteCommand command;
        private bool indexExecution;

        private Double ccal;
        private Double proteins;
        private Double fats;
        private Double carbohydrates;
        private Double servingSize;
        private String unitsOfPD;

        private Double servingSizeInSelectedProdDish;
        private Double ccalInSelectedProdDish;
        private Double proteinsInSelectedProdDish;
        private Double fatsInSelectedProdDish;
        private Double carbohydratesInSelectedProdDish;

        //Проведение поиска
        private void SearchDataPD(String resFood)
        {
            command.CommandText = "SELECT quantity FROM '"+ resFood +"' WHERE name_of_product = '" + nameOfProdDish + "' AND data_num = '" + dateOfReceptionFood + "'";
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                servingSize = Convert.ToDouble(reader.GetValue(0));
                reader.Close();

                command.CommandText = "SELECT ccal FROM '" + resFood + "' WHERE name_of_product = '" + nameOfProdDish + "' AND data_num = '" + dateOfReceptionFood + "'";
                reader = command.ExecuteReader();
                reader.Read();
                ccal = Convert.ToDouble(reader.GetValue(0));
                reader.Close();

                command.CommandText = "SELECT proteins FROM '" + resFood + "' WHERE name_of_product = '" + nameOfProdDish + "' AND data_num = '" + dateOfReceptionFood + "'";
                reader = command.ExecuteReader();
                reader.Read();
                proteins = Convert.ToDouble(reader.GetValue(0));
                reader.Close();

                command.CommandText = "SELECT fats FROM '" + resFood + "' WHERE name_of_product = '" + nameOfProdDish + "' AND data_num = '" + dateOfReceptionFood + "'";
                reader = command.ExecuteReader();
                reader.Read();
                fats = Convert.ToDouble(reader.GetValue(0));
                reader.Close();

                command.CommandText = "SELECT carbohydrates FROM '" + resFood + "' WHERE name_of_product = '" + nameOfProdDish + "' AND data_num = '" + dateOfReceptionFood + "'";
                reader = command.ExecuteReader();
                reader.Read();
                carbohydrates = Convert.ToDouble(reader.GetValue(0));
                reader.Close();
                connectBD.Close();
                DisplayDataOfProdDish();
            }
            else
            {
                connectBD.Close();
            }
        }

        //Поиск данных о продукте или блюде по имени, приему пищи и дате
        private void SearchDataOfProdOrDishInBD()
        {
            try
            {
                ConnectBd();
                switch (receptionFood)
                {
                    case "breakfast_in_date":
                        SearchDataPD(receptionFood);
                        break;
                    case "dinner_in_date":
                        SearchDataPD(receptionFood);
                        break;
                    case "supper_in_date":
                        SearchDataPD(receptionFood);
                        break;
                    case "nosh_in_date":
                        SearchDataPD(receptionFood);
                        break;
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Ошибка поиска!", "Окно ChangeOrDeleteProdDish");
            }
        }

        //Поиск единиц измерения продукта или блюда по имени в бд
        private void SearchUnitsOfProductOrDishInBD()
        {
            String type = "product";
            SearchUnitsFromBD(type);
            if (indexExecution == false)
            {
                type = "dish";
                SearchUnitsFromBD(type);
            }
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

        //Проведение поиска
        private void SearchUnitsFromBD(String typeOfProduct)
        {
            try
            {
                ConnectBd();
                command.CommandText = "SELECT units FROM '" + typeOfProduct + "' WHERE name = '" + nameOfProdDish + "'";
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    unitsOfPD = Convert.ToString(reader.GetValue(0));
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
                MessageBox.Show("Нет соединения!", "Окно ChangeOrDeleteProdDish");
            }
            
        }

        //Вывод данных о продукте на экран
        private void DisplayDataOfProdDish()
        {
            namePOrD.Content = nameOfProdDish;
            quantityOfPD.Text = servingSize.ToString();
            units.Content = unitsOfPD;
        }

        //Расчет кбжу выбранного продукта/блюда
        private void CalculateCPFCOfProdDish()
        {
            servingSizeInSelectedProdDish = Convert.ToDouble(quantityOfPD.Text);
            ccalInSelectedProdDish = (servingSizeInSelectedProdDish * ccal) / servingSize;
            proteinsInSelectedProdDish = (servingSizeInSelectedProdDish * proteins) / servingSize;
            fatsInSelectedProdDish = (servingSizeInSelectedProdDish * fats) / servingSize;
            carbohydratesInSelectedProdDish = (servingSizeInSelectedProdDish * carbohydrates) / servingSize;
            quantityOfPD.Clear();
        }

        //Нажатие кнопки "Отмена"
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Сохранение изменений в таблице приема пищи
        private void SaveChanges()
        {
            try
            {
                command.CommandText = "UPDATE '" + receptionFood + "' SET quantity = '" + servingSizeInSelectedProdDish + "', ccal = '" + ccalInSelectedProdDish + "', proteins = '" + proteinsInSelectedProdDish + "', fats = '" + fatsInSelectedProdDish + "', carbohydrates = '" + carbohydratesInSelectedProdDish + "' WHERE name_of_product = '" + nameOfProdDish + "' AND data_num = '" + dateOfReceptionFood + "'";
                command.ExecuteNonQuery();
                connectBD.Close();
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Соединение с БД не получено!", "Окно ChoiceQuantityProductOrDish");
            }
        }

        //Расчет и сохранение данных КБЖУ в бд по нажатию кнопки "Добавить"
        private void ButtonChangeQuantity_Click(object sender, RoutedEventArgs e)
        {
            int indexMistake = 0;
            if (quantityOfPD.Text == "0")
            {
                Delete();
                ExecuteFunc();
            }
            else
            {
                try
                {
                    CalculateCPFCOfProdDish();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Вводите числа!");
                    quantityOfPD.Clear();
                    indexMistake++;
                }
                if (indexMistake == 0)
                {
                    ConnectBd();
                    SaveChanges();
                    MessageBox.Show("Изменено!");
                    ExecuteFunc();
                    this.Hide();
                }
            }
        }

        //Много полезных функций
        private void ExecuteFunc()
        {
            fD.ClearCPFC();
            fD.IndicationBasicIndicators(dateOfReceptionFood);
            fD.ClearStackPanelReceptionFood();
            fD.OutputDataOfReceptionsFood(Convert.ToString(dateOfReceptionFood));
        }

        //Удаление объекта по нажатию кнопки "Удалить"
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
            ExecuteFunc();
            this.Hide();
        }

        //Непосредственное удаление
        private void Delete()
        {
            try
            {
                ConnectBd();
                command.CommandText = "DELETE FROM '" + receptionFood + "' WHERE name_of_product = '" + nameOfProdDish + "' AND data_num = '" + dateOfReceptionFood + "'";
                command.ExecuteNonQuery();
                connectBD.Close();
                MessageBox.Show("Продукт удален!");
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Ошибка! Удаление не произошло!");
            }
        }
    }
}
