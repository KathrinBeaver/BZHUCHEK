using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для ProductSelection.xaml
    /// </summary>
    public partial class ProductSelection : Window
    {
        public ProductSelection(Int32 idReceptionFood, String dateOfReceptionFood)
        {
            InitializeComponent();
            TakeProductAndDishFromBD();
            this.idReceptionFood = idReceptionFood;
            this.dateOfReceptionFood = dateOfReceptionFood;
        }

        Int32 idReceptionFood;
        String dateOfReceptionFood;
        private static String basePersonalData = "PersonalData.sql";
        private static SQLiteConnection connectBD;
        private Int32 numberOfSearching = 0;
        private Grid gridProdAndDish = new Grid();

        //Вывод всех продуктов и блюд из БД
        private void TakeProductAndDishFromBD()
        {
            if (File.Exists(basePersonalData))
            {
                try
                {
                    connectBD = new SQLiteConnection("Data Source=" + basePersonalData + ";Version=3;" + "UseUTF16Encoding = True;");
                    connectBD.Open();
                    SQLiteCommand commandInNameProduct = new SQLiteCommand();
                    SQLiteCommand commandInNameDish = new SQLiteCommand();
                    commandInNameProduct.Connection = connectBD;
                    commandInNameDish.Connection = connectBD;
                    DataTable dataTable1 = new DataTable();
                    DataTable dataTable2 = new DataTable();
                    commandInNameProduct.CommandText = "SELECT name FROM product";
                    commandInNameDish.CommandText = "SELECT name FROM dish";
                    SQLiteDataReader readerForNameProduct = commandInNameProduct.ExecuteReader();
                    SQLiteDataReader readerForNameDish = commandInNameDish.ExecuteReader();

                    String sqlCom1 = "SELECT name FROM product";
                    String sqlCom2 = "SELECT name FROM dish";
                    SQLiteDataAdapter adapter1 = new SQLiteDataAdapter(sqlCom1, connectBD);
                    SQLiteDataAdapter adapter2 = new SQLiteDataAdapter(sqlCom2, connectBD);
                    adapter1.Fill(dataTable1);
                    adapter1.Fill(dataTable2);
                    int countOfRows = dataTable1.Rows.Count + dataTable2.Rows.Count;
                    string strNameProduct;
                    string strNameDish;

                    if (countOfRows > 0)
                    {
                        int i = 0;

                        while (readerForNameDish.Read())
                        {
                            strNameDish = Convert.ToString(readerForNameDish.GetString(readerForNameDish.GetOrdinal("name")));

                            Label labelNameOfDish = new Label
                            {
                                Content = strNameDish,
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 14
                            };

                            CheckBox checkBoxForChoice = new CheckBox
                            {
                                HorizontalAlignment = HorizontalAlignment.Right,
                                MaxHeight = 26,
                                MaxWidth = 100
                            };

                            gridProductAndDish.RowDefinitions.Add(new RowDefinition());

                            Grid.SetColumn(labelNameOfDish, 0);
                            Grid.SetRow(labelNameOfDish, i);
                            Grid.SetColumn(checkBoxForChoice, 1);
                            Grid.SetRow(checkBoxForChoice, i);

                            gridProductAndDish.Children.Add(labelNameOfDish);
                            gridProductAndDish.Children.Add(checkBoxForChoice);
                            i++;
                        }
                        readerForNameDish.Close();

                        while (readerForNameProduct.Read())
                        {
                            strNameProduct = Convert.ToString(readerForNameProduct.GetString(readerForNameProduct.GetOrdinal("name")));

                            Label labelNameOfProduct = new Label
                            {
                                Content = strNameProduct,
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 14
                            };

                            CheckBox checkBoxForChoice = new CheckBox
                            {
                                HorizontalAlignment = HorizontalAlignment.Right,
                                MaxHeight = 26,
                                MaxWidth = 100
                            };

                            gridProductAndDish.RowDefinitions.Add(new RowDefinition());

                            Grid.SetColumn(labelNameOfProduct, 0);
                            Grid.SetRow(labelNameOfProduct, i);
                            Grid.SetColumn(checkBoxForChoice, 1);
                            Grid.SetRow(checkBoxForChoice, i);

                            gridProductAndDish.Children.Add(labelNameOfProduct);
                            gridProductAndDish.Children.Add(checkBoxForChoice);
                            i++;
                        }
                        readerForNameProduct.Close();
                    }
                    else
                    {
                        Label labelAmpty = new Label
                        {
                            Content = " "
                        };
                        stackPanelIngredient1.Children.Add(labelAmpty);
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
        }

        //Добавление и удаление продуктов и блюд в окне при проведении поиска
        private void AddDeleteProductAndDish()
        {
            try
            {
                String textOfMessage = Convert.ToString(textBoxSearchDish.Text);
                Regex regex = new Regex(textOfMessage, RegexOptions.IgnoreCase);
                int counterOfSearch = 0;
                int counterOfRow = 0;

                foreach (var child in gridProductAndDish.Children)
                {
                    if (child.GetType().ToString().Contains("Label"))
                    {
                        Match match = regex.Match(child.ToString());
                        if (match.Success)
                        {
                            Label labelValue = new Label
                            {
                                Content = child
                            };
                            gridProdAndDish.Children.Add(labelValue);
                            counterOfSearch++;
                        }
                    }
                }
                gridProductAndDish.Children.Clear();
                if (counterOfSearch == 0)
                {
                    Label labelValue = new Label
                    {
                        Foreground = new SolidColorBrush(Colors.White),
                        FontSize = 14,
                        Content = "Нет результатов."
                    };
                    gridProductAndDish.Children.Add(labelValue);
                }
                else
                {
                    foreach (var child in gridProdAndDish.Children)
                    {
                        if (child.GetType().ToString().Contains("Label"))
                        {
                            Label labelValue = new Label
                            {
                                Foreground = new SolidColorBrush(Colors.White),
                                FontSize = 14,
                                Content = child
                            };

                            CheckBox checkBoxForChoice = new CheckBox
                            {
                                HorizontalAlignment = HorizontalAlignment.Right,
                                MaxHeight = 26,
                                MaxWidth = 100
                            };

                            Grid.SetColumn(labelValue, 0);
                            Grid.SetRow(labelValue, counterOfRow);

                            Grid.SetColumn(checkBoxForChoice, 1);
                            Grid.SetRow(checkBoxForChoice, counterOfRow);

                            gridProductAndDish.Children.Add(labelValue);
                            gridProductAndDish.Children.Add(checkBoxForChoice);
                            counterOfRow++;
                        }
                    }
                    gridProdAndDish.Children.Clear();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }

        //Поиск продукта или блюда по названию
        private void ButtonSearchDish_Click(object sender, RoutedEventArgs e)
        {
            if (numberOfSearching == 0)
            {
                AddDeleteProductAndDish();
                numberOfSearching++;
            }
            else
            {
                gridProductAndDish.Children.Clear();
                TakeProductAndDishFromBD();
                AddDeleteProductAndDish();
            }
        }

        private void TextoxSearchDish_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        //Переход к предыдущему окну при нажатии кнопки "Отмена"
        private void ButtonAnnulment_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            FoodDiary foodDiaryPage = new FoodDiary
            {
                Left = (100),
                Top = (100)
            };
            foodDiaryPage.Show();
        }

        //Переход к новому окну при нажатии кнопки "Добавить свое блюдо"
        private void ButtonAddOwnDish_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            AddingYourDish addingYourDishPage = new AddingYourDish(idReceptionFood, dateOfReceptionFood)
            {
                Left = (100),
                Top = (100)
            };
            addingYourDishPage.Show();
        }

        //Нажатие кнопки "Выбор" у продуктов и блюд
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Int32 indexCheckBox = 0;
            int indexLabel = 0;
            String nameOfChoice;
            int counterForChecked = 0;
            foreach (var childCheckBox in gridProductAndDish.Children)
            {
                if (childCheckBox.GetType().ToString().Contains("CheckBox") && childCheckBox.ToString().Contains("IsChecked:True"))
                {
                    foreach (var childLabel in gridProductAndDish.Children)
                    {
                        if (childLabel.GetType().ToString().Contains("Label") && indexLabel == indexCheckBox)
                        {
                            String[] nameLabel = childLabel.ToString().Split(new char[] { ':' });
                            String unnecessaryPart = nameLabel[0];
                            String necessaryPart = nameLabel[1];
                            nameOfChoice = necessaryPart.Remove(necessaryPart.IndexOf(' '), 1);
                            
                            ChoiceQuantityProductOrDishes choiceQuantityProductOrDishesPage = new ChoiceQuantityProductOrDishes(idReceptionFood, dateOfReceptionFood, nameOfChoice);
                            choiceQuantityProductOrDishesPage.Show();
                            choiceQuantityProductOrDishesPage.Left = (100);
                            choiceQuantityProductOrDishesPage.Top = (100);
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
            }
        }
    }
}
