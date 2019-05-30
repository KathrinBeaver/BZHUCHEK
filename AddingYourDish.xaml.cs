using System;
using System.Collections.Generic;
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
using System.Xml.Serialization;

namespace наВинду
{
    /// <summary>
    /// Логика взаимодействия для AddingYourDish.xaml
    /// </summary>
    public partial class AddingYourDish : Window
    {
        public AddingYourDish(Int32 idReceptionFood, String dateOfReceptionFood)
        {
            InitializeComponent();
            this.idReceptionFood = idReceptionFood;
            this.dateOfReceptionFood = dateOfReceptionFood;
        }
        Int32 idReceptionFood;
        String dateOfReceptionFood;

        public static String basePersonalData = "PersonalData.sql";
        public static SQLiteConnection connectBD;
        public SQLiteCommand command;

        //Переход к предыдущему окну, при нажатии кнопки "Отмена"
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            ProductSelection productSelectionPage = new ProductSelection(idReceptionFood, dateOfReceptionFood.ToString())
            {
                Left = (100),
                Top = (100)
            };
            productSelectionPage.Show();
            this.Hide();
        }

        //Установление соединения с бд
        private void ConnectBd()
        {
            connectBD = new SQLiteConnection("Data Source=" + basePersonalData + ";Version=3;" + "New= True;" + "UseUTF16Encoding = True;");
            connectBD.Open();
            command = new SQLiteCommand();
            command.Connection = connectBD;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        //Добавление нового блюда по бжу и ккал при нажатии кнопки "Добавить"
        private void ButtonAddDish_Click(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            try
            {
                ConnectBd();
                command.CommandText = "SELECT name FROM product WHERE name = '" + Convert.ToString(textBoxNameOfProduct.Text) + "'";
                command.ExecuteNonQuery();
                SQLiteDataReader reader = command.ExecuteReader();

                if (!reader.Read())
                {
                    reader.Close();
                    command.CommandText = "INSERT INTO product ('name', 'serving_size', 'ccal', 'proteins', 'fats', 'carbohydrates', 'units') values ('" + Convert.ToString(textBoxNameOfProduct.Text) + "' , '" + Convert.ToInt16(textBoxSizeOfProduct.Text) + "' , '" + Convert.ToInt16(textBoxCcalOfProduct.Text) + "' , '" + Convert.ToDouble(textBoxProteinOfProduct.Text) + "' , '" + Convert.ToDouble(textBoxFatOfProduct.Text) + "' , '" + Convert.ToDouble(textBoxCarbohydratesOfProduct.Text) + "', '" + Convert.ToString(comboBoxMeasureOfWeight.Text) + "' ) ";
                    command.ExecuteNonQuery();
                    connectBD.Close();
                }
                else
                {
                    MessageBox.Show("Такой продукт уже существует!", "Предупреждение");
                    ClearFields();
                    reader.Close();
                    connectBD.Close();
                    counter++;
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Соединение с БД не получено!", "Окно AddingYourDish");
                counter++;
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат данных!\nЕсли вы хотите ввести число, которое имеет дробную часть, то ее следует писать через точку после целой части.");
                counter++;
            }
            if (counter == 0)
            {
                MessageBox.Show("Продукт добавлен!");
                ClearFields();
            }
        }

        //Очищение полей окна
        private void  ClearFields()
        {
            textBoxNameOfProduct.Clear();
            textBoxSizeOfProduct.Clear();
            textBoxCcalOfProduct.Clear();
            textBoxProteinOfProduct.Clear();
            textBoxFatOfProduct.Clear();
            textBoxCarbohydratesOfProduct.Clear();
        }

        private void TextBoxNameOfDish_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBoxSizeOfDish_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBoxCcalOfDish_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBoxProteinOfDish_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBoxFatOfDish_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBoxCarbohydratesOfDish_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        //Переход к новому окну, в котором, используя возможность добавление ингридиентов, пользователь создает новые блюда
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            CreatingDishByIngredients creatingDishByIngredientsPage = new CreatingDishByIngredients(idReceptionFood, dateOfReceptionFood);
            creatingDishByIngredientsPage.Left = (100);
            creatingDishByIngredientsPage.Top = (100);
            creatingDishByIngredientsPage.Show();
        }
    }
}

