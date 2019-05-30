using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
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
    /// Логика взаимодействия для AddingQuantityOfIngredient.xaml
    /// </summary>
    public partial class AddingQuantityOfIngredient : Window
    {
        public AddingQuantityOfIngredient(string strNameIngr, Int32 dishId)
        {
            InitializeComponent();
            this.strNameIngr = strNameIngr;
            this.dishId = dishId;
            GetDataIngredient();
            OutputDataIngredient();
        }
        Int32 dishId;
        string strNameIngr;
        private static String basePersonalData = "PersonalData.sql";
        private static SQLiteConnection connectBD;
        SQLiteCommand commandForIngredient;
        private Double servingSize = 0;
        private Double ccal = 0;
        private Double proteins = 0;
        private Double fats = 0;
        private Double carbohydrates = 0;
        private String units;
        private Double servingSizePreset = 0;
        private Int32 id = 0;
        private Double ccalForDish = 0;
        private Double proteinsForDish = 0;
        private Double fatsForDish = 0;
        private Double carbohydratesForDish = 0;

        //Подключение к бд
        private void ConnectBd()
        {
            connectBD = new SQLiteConnection("Data Source=" + basePersonalData + ";Version=3;" + "UseUTF16Encoding = True;");
            connectBD.Open();
            commandForIngredient = new SQLiteCommand();
            commandForIngredient.Connection = connectBD;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        //Получение данных ингредиента по его имени
        private void GetDataIngredient()
        {
            try
            {
                ConnectBd();
                commandForIngredient.CommandText = "SELECT serving_size FROM product WHERE name = '" + strNameIngr + "'";
                SQLiteDataReader readerForIngredient = commandForIngredient.ExecuteReader();

                if (readerForIngredient.Read())
                {
                    servingSize = Convert.ToDouble(readerForIngredient.GetValue(0));
                    readerForIngredient.Close();

                    commandForIngredient.CommandText = "SELECT ccal FROM product WHERE name = '" + strNameIngr + "'";
                    readerForIngredient = commandForIngredient.ExecuteReader();
                    readerForIngredient.Read();
                    ccal = Convert.ToDouble(readerForIngredient.GetValue(0));
                    readerForIngredient.Close();

                    commandForIngredient.CommandText = "SELECT proteins FROM product WHERE name = '" + strNameIngr + "'";
                    readerForIngredient = commandForIngredient.ExecuteReader();
                    readerForIngredient.Read();
                    proteins = Convert.ToDouble(readerForIngredient.GetValue(0));
                    readerForIngredient.Close();

                    commandForIngredient.CommandText = "SELECT fats FROM product WHERE name = '" + strNameIngr + "'";
                    readerForIngredient = commandForIngredient.ExecuteReader();
                    readerForIngredient.Read();
                    Object str = readerForIngredient.GetValue(0);
                    fats = Convert.ToDouble(str);
                    readerForIngredient.Close();

                    commandForIngredient.CommandText = "SELECT carbohydrates FROM product WHERE name = '" + strNameIngr + "'";
                    readerForIngredient = commandForIngredient.ExecuteReader();
                    readerForIngredient.Read();
                    carbohydrates = Convert.ToDouble(readerForIngredient.GetValue(0));
                    readerForIngredient.Close();

                    commandForIngredient.CommandText = "SELECT units FROM product WHERE name = '" + strNameIngr + "'";
                    readerForIngredient = commandForIngredient.ExecuteReader();
                    readerForIngredient.Read();
                    units = Convert.ToString(readerForIngredient.GetValue(0));
                    readerForIngredient.Close();

                    commandForIngredient.CommandText = "SELECT id FROM product WHERE name = '" + strNameIngr + "'";
                    readerForIngredient = commandForIngredient.ExecuteReader();
                    readerForIngredient.Read();
                    id = Convert.ToInt32(readerForIngredient.GetValue(0));
                    readerForIngredient.Close();

                    connectBD.Close();
                }
                else if (!readerForIngredient.Read())
                {
                    MessageBox.Show("Указанного продукта нет");
                    readerForIngredient.Close();
                    connectBD.Close();
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Нет соединения!", "Предупреждение");
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка!", "Предупреждение");
            }
        }

        //Вывод данных на окно
        private void OutputDataIngredient()
        {
            nameOfIngredient.Content = strNameIngr;
            unitOfIngredient.Content = units;
        }

        //Расчет кбжу для выбранного количества ингредиента
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            servingSizePreset = Convert.ToDouble(quantityOfIngredient.Text);
            ccal = (servingSizePreset * ccal) / servingSize;
            proteins = (servingSizePreset * proteins) / servingSize;
            fats = (servingSizePreset * fats) / servingSize;
            carbohydrates = (servingSizePreset * carbohydrates) / servingSize;
            ConnectBdDishRead();
            AddingDataToBD();

            MessageBox.Show("Ингредиент добавлен!");
            this.Close();
        }

        //Сохранение данных в бд
        private void AddingDataToBD()
        {
            if (File.Exists(basePersonalData))
            {
                try
                {
                    ConnectBd();
                    commandForIngredient.CommandText = "CREATE TABLE IF NOT EXISTS product_in_dish(dish_id INTEGER, product_id INTEGER, ccal DOUBLE, proteins DOUBLE, fats DOUBLE, carbohydrates DOUBLE, FOREIGN KEY (dish_id) REFERENCES dish (id), FOREIGN KEY (product_id) REFERENCES product (id))";
                    commandForIngredient.ExecuteNonQuery();
                    commandForIngredient.CommandText = "INSERT INTO product_in_dish ('product_id', 'dish_id',  'ccal', 'proteins', 'fats', 'carbohydrates') values ('" + id + "' , '" + dishId + "' , '" + ccal + "' , '" + proteins + "' , '" + fats + "' , '" + carbohydrates + "')";
                    commandForIngredient.ExecuteNonQuery();
                    connectBD.Close();
                }
                catch (SQLiteException)
                {
                    MessageBox.Show("Соединение с БД не получено!", "Предупреждение");
                }
            }
        }

        //подключение к базе данных dish по dish_id для сохранения данных
        private void ConnectBdDishSave()
        {
            if (File.Exists(basePersonalData))
            {
                try
                {
                    ConnectBd();

                    commandForIngredient.CommandText = "UPDATE dish SET ccal = '" + ccalForDish + "', proteins = '" + proteinsForDish + "', fats = '" + fatsForDish + "', carbohydrates = '" + carbohydratesForDish + "' WHERE id = '" + dishId + "'";
                    commandForIngredient.ExecuteNonQuery();
                    connectBD.Close();
                }
                catch (SQLiteException)
                {
                    MessageBox.Show("Соединение с БД не получено!", "Предупреждение");
                }
            }
            else
            {
                try
                {
                    SQLiteConnection.CreateFile(basePersonalData);
                    ConnectBd();

                    commandForIngredient.CommandText = "UPDATE dish SET ccal = '" + ccalForDish + "', proteins = '" + proteinsForDish + "', fats = '" + fatsForDish + "', carbohydrates = '" + carbohydratesForDish + "' WHERE id = '" + dishId + "'";
                    commandForIngredient.ExecuteNonQuery();
                    connectBD.Close();
                }
                catch (SQLiteException)
                {
                    MessageBox.Show("База данных не создана!", "Предупреждение");
                }
            }
        }

        //подключение к базе данных dish по dish_id для считывания данных
        private void ConnectBdDishRead()
        {
            ConnectBd();
            DataTable dataTable = new DataTable();
            commandForIngredient.CommandText = "SELECT ccal FROM dish WHERE id = '" + dishId + "'";
            SQLiteDataReader readerForDish = commandForIngredient.ExecuteReader();
            readerForDish.Read();
            if (!readerForDish.IsDBNull(0))
            {
                ccalForDish = Convert.ToDouble(readerForDish.GetValue(0));
                readerForDish.Close();
                
                commandForIngredient.CommandText = "SELECT proteins FROM dish WHERE id = '" + dishId + "'";
                readerForDish = commandForIngredient.ExecuteReader();
                readerForDish.Read();
                proteinsForDish = Convert.ToDouble(readerForDish.GetValue(0));
                readerForDish.Close();

                commandForIngredient.CommandText = "SELECT fats FROM dish WHERE id = '" + dishId + "'";
                readerForDish = commandForIngredient.ExecuteReader();
                readerForDish.Read();
                fatsForDish = Convert.ToDouble(readerForDish.GetValue(0));
                readerForDish.Close();

                commandForIngredient.CommandText = "SELECT carbohydrates FROM dish WHERE id = '" + dishId + "'";
                readerForDish = commandForIngredient.ExecuteReader();
                readerForDish.Read();
                carbohydratesForDish = Convert.ToDouble(readerForDish.GetValue(0));
                readerForDish.Close();
                connectBD.Close();

                CalculationPFCForDish();
            }
            else 
            {
                ccalForDish = 0;
                proteinsForDish = 0;
                fatsForDish = 0;
                carbohydratesForDish = 0;
                readerForDish.Close();
                connectBD.Close();

                CalculationPFCForDish();
            }
        }

        //Расчет кбжу для созданного блюда (сложение всех кбжу выбранных ингредиентов)
        private void CalculationPFCForDish()
        {
            ccalForDish += ccal;
            proteinsForDish += proteins;
            fatsForDish += fats;
            carbohydratesForDish += carbohydrates;

            ConnectBdDishSave();
        }
    }
}
