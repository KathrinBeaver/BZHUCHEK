using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
using System.Globalization;

namespace наВинду
{
    /// <summary>
    /// Логика взаимодействия для CreatingDishByIngredients.xaml
    /// </summary>
    public partial class CreatingDishByIngredients : Window
    {
        public CreatingDishByIngredients(Int32 idReceptionFood, String dateOfReceptionFood)
        {
            InitializeComponent();
            this.idReceptionFood = idReceptionFood;
            this.dateOfReceptionFood = dateOfReceptionFood;
            SetPropertyOfGrid();
            TakeIngredientsFromBD();
        }
        Int32 idReceptionFood;
        String dateOfReceptionFood;
        private static String basePersonalData = "PersonalData.sql";
        private static SQLiteConnection connectBD;
        public TextBox textBoxForNecessaryPart = new TextBox();
        public TextBox textBoxForDishId = new TextBox();
        private SQLiteCommand commandDish;
        private Int32 idDish = 0;
        Int32 portion = 0;
        String nameDish;

        //Установка свойств grid
        private void SetPropertyOfGrid()
        {
            gridForIngredients.ColumnDefinitions.Add(new ColumnDefinition());
            gridForIngredients.ColumnDefinitions.Add(new ColumnDefinition());
        }

        //Создание таблицы dish, сохранение в нее имени и количества порций блюда
        private void CreateTableDish()
        {
            portion = Convert.ToInt32(textBoxQuantityOfPortion.Text);
            nameDish = Convert.ToString(textBoxNameOfDishWithIngredients.Text);

            try
            {
                ConnectBd();
                commandDish.CommandText = "INSERT INTO dish ('name', 'serving_size', 'units') values ('" + nameDish + "' , '" + portion + "' , '" + "порц" + "')";
                commandDish.ExecuteNonQuery();
                connectBD.Close();
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Соединение с БД не получено!", "Окно CreatingDishByIngredient");
            }
        }

        //Получение id блюда
        private void GetIdOfDish()
        {
            ConnectBd();
            commandDish.CommandText = "SELECT id FROM dish WHERE name = '" + nameDish + "'";
            SQLiteDataReader reader = commandDish.ExecuteReader();
            reader.Read();
            idDish = Convert.ToInt32(reader.GetValue(0));
            textBoxForDishId.Text = idDish.ToString();
            reader.Close();
            connectBD.Close();
        }

        //Поиск выбранных ингредиентов при нажатии кнопки добавить
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textBoxNameOfDishWithIngredients.Text == "" || textBoxNameOfDishWithIngredients.Text == " ")
                {
                    MessageBox.Show("Введите название!");
                }
                else
                {
                    int indexCheckBox = 0;
                    int indexLabel = 0;
                    int counterForChecked = 0;
                    CreateTableDish();
                    foreach (var childCheckBox in gridForIngredients.Children)
                    {
                        if (childCheckBox.GetType().ToString().Contains("CheckBox") && childCheckBox.ToString().Contains("IsChecked:True"))
                        {
                            foreach (var childLabel in gridForIngredients.Children)
                            {
                                if (childLabel.GetType().ToString().Contains("Label") && indexLabel == indexCheckBox)
                                {
                                    String[] nameLabel = childLabel.ToString().Split(new char[] { ':' });
                                    String unnecessaryPart = nameLabel[0];
                                    String necessaryPart = nameLabel[1];
                                    necessaryPart = necessaryPart.Remove(necessaryPart.IndexOf(' '), 1);

                                    GetIdOfDish();
                                    textBoxForNecessaryPart.Text = necessaryPart;
                                    AddingQuantityOfIngredient addingQuantityOfIngredientPage = new AddingQuantityOfIngredient(this.textBoxForNecessaryPart.Text, Convert.ToInt32(this.textBoxForDishId.Text));
                                    addingQuantityOfIngredientPage.Show();
                                    counterForChecked++;
                                }
                                if (childLabel.GetType().ToString().Contains("Label"))
                                {
                                    indexLabel++;
                                }
                            }
                            indexLabel = 0;
                        }
                        if (childCheckBox.GetType().ToString().Contains("CheckBox"))
                        {
                            indexCheckBox++;
                        }
                    }

                    if (counterForChecked == 0)
                    {
                        MessageBox.Show("Выберите ингридиенты!");
                        connectBD.Close();
                        DeleteTable();
                    }
                }
                ClearSelectedIngredients();
            }
            catch (FormatException)
            {
                MessageBox.Show("Введен неверный формат данных");
            }
        }

        //Удаление таблицы
        private void DeleteTable()
        {
            GetIdOfDish();
            ConnectBd();
            commandDish.CommandText = "DELETE FROM dish WHERE id = '" + idDish + "'";
            commandDish.ExecuteNonQuery();
            connectBD.Close();
        }

        //Очищение выделенных ингредиентов
        private void ClearSelectedIngredients()
        {
            textBoxNameOfDishWithIngredients.Clear();
            textBoxQuantityOfPortion.Clear();
            gridForIngredients.Children.Clear();
            TakeIngredientsFromBD();
        }

        //Подключение к бд
        private void ConnectBd()
        {
            connectBD = new SQLiteConnection("Data Source=" + basePersonalData + ";Version=3;" + "UseUTF16Encoding = True;");
            connectBD.Open();
            commandDish = new SQLiteCommand();
            commandDish.Connection = connectBD;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        //Вывод на экран всех продуктов и блюд
        private void TakeIngredientsFromBD()
        {
            if (File.Exists(basePersonalData))
            {
                try
                {
                    ConnectBd();
                    DataTable dataTable = new DataTable();
                    commandDish.CommandText = "SELECT name FROM product";
                    SQLiteDataReader readerForName = commandDish.ExecuteReader();

                    String sqlCom1 = "SELECT name FROM product";
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlCom1, connectBD);
                    adapter.Fill(dataTable);
                    int countOfRows = dataTable.Rows.Count;
                    string strName;
                    DataTable dt = readerForName.GetSchemaTable();
                    if (countOfRows > 0)
                    {
                        int i = 0;

                        while (readerForName.Read())
                        {
                            strName = Convert.ToString(readerForName.GetString(readerForName.GetOrdinal("name")));

                            Label labelNameOfProduct = new Label();
                            labelNameOfProduct.Content = strName;
                            labelNameOfProduct.Foreground = new SolidColorBrush(Colors.White);
                            labelNameOfProduct.FontSize = 14;

                            CheckBox checkBoxForIngredient = new CheckBox();
                            checkBoxForIngredient.HorizontalAlignment = HorizontalAlignment.Left;
                            checkBoxForIngredient.HorizontalContentAlignment = HorizontalAlignment.Left;
                            // checkBoxForIngredient.IsEnabledChanged += new EventHandler(this.ChoiceCheckBox);
                            //checkBoxForIngredient.IsEnabledChanged += (s, e) => MessageBox.Show("Clicked checkbox " + i);

                            gridForIngredients.RowDefinitions.Add(new RowDefinition());
                            gridForIngredients.ShowGridLines = true;

                            Grid.SetColumn(labelNameOfProduct, 0);
                            Grid.SetRow(labelNameOfProduct, i);
                            Grid.SetColumn(checkBoxForIngredient, 1);
                            Grid.SetRow(checkBoxForIngredient, i);

                            gridForIngredients.Children.Add(labelNameOfProduct);
                            gridForIngredients.Children.Add(checkBoxForIngredient);
                            i++;
                        }
                        readerForName.Close();
                    }
                    else
                    {
                        Label labelAmpty = new Label();
                        labelAmpty.Content = " ";
                        stackPanelIngredient.Children.Add(labelAmpty);
                    }
                }
                catch (SQLiteException)
                {
                    MessageBox.Show("Нет соединения!", "Окно CreatingDishByIngredient");
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка!", "Предупреждение");
                }
            }
        }

        private void TextBoxNameOfDish_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBoxSizeOfDish_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ButtonShowIngredients1_Click(object sender, RoutedEventArgs e)
        {

        }

        //Нажатие кнопки "Отмена"
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            AddingYourDish addingNewDishPage = new AddingYourDish(idReceptionFood, dateOfReceptionFood.ToString());
            addingNewDishPage.Left = (100);
            addingNewDishPage.Top = (100);
            addingNewDishPage.Show();
        }
    }
}
