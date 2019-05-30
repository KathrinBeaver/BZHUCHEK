using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace наВинду
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TakeDataFromBD();
        }

        //Для работы с SQLite базой данных
        private static String basePersonalData = "PersonalData.sql";
        private static SQLiteConnection connectBD;
        private SQLiteCommand commandOut = new SQLiteCommand();
        private SQLiteCommand commandIn = new SQLiteCommand();

        //Проверка введенных данных
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int numOfExeptions = 0;
            try
            {
                if (Convert.ToInt32(age.Text) < 16 || Convert.ToInt32(age.Text) > 100)
                {
                    MessageBox.Show("Недопустимый возраст! Ваш возраст должен превышать 16 лет!", "Предупреждение");
                    numOfExeptions += 1;
                }

                //отслеживание недостаточный вес относительно роста
                if ((Convert.ToInt32(growth.Text) > 150 && Convert.ToDouble(weightDesired.Text) <= 45)
                || (Convert.ToInt32(growth.Text) > 160 && Convert.ToDouble(weightDesired.Text) <= 50)
                || (Convert.ToInt32(growth.Text) > 170 && Convert.ToDouble(weightDesired.Text) <= 55)
                || (Convert.ToInt32(growth.Text) > 175 && Convert.ToDouble(weightDesired.Text) <= 60)
                || (Convert.ToInt32(growth.Text) > 180 && Convert.ToDouble(weightDesired.Text) <= 65)
                || (Convert.ToInt32(growth.Text) > 190 && Convert.ToDouble(weightDesired.Text) <= 70)
                || (Convert.ToInt32(growth.Text) > 195 && Convert.ToDouble(weightDesired.Text) <= 75)
                || (Convert.ToInt32(growth.Text) > 205 && Convert.ToDouble(weightDesired.Text) <= 80)
                || (Convert.ToInt32(growth.Text) > 210 && Convert.ToDouble(weightDesired.Text) <= 85))
                {
                    MessageBox.Show("Введенный Вами желаемый вес недостаточен для Вашего роста! Пожалуйста, введите другой.", "Предупреждение");
                    numOfExeptions += 1;
                }

                //отслеживание избыточного веса 
                if ((Convert.ToInt32(growth.Text) >= 140 && Convert.ToInt32(growth.Text) <= 145) && Convert.ToDouble(weightDesired.Text) >= 50)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }
                else if (Convert.ToInt32(growth.Text) <= 150 && Convert.ToDouble(weightDesired.Text) >= 55)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }
                else if (Convert.ToInt32(growth.Text) <= 155 && Convert.ToDouble(weightDesired.Text) >= 60)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }
                else if (Convert.ToInt32(growth.Text) <= 160 && Convert.ToDouble(weightDesired.Text) >= 65)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }
                else if (Convert.ToInt32(growth.Text) <= 165 && Convert.ToDouble(weightDesired.Text) >= 70)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }
                else if (Convert.ToInt32(growth.Text) <= 175 && Convert.ToDouble(weightDesired.Text) >= 80)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }
                else if (Convert.ToInt32(growth.Text) <= 180 && Convert.ToDouble(weightDesired.Text) >= 85)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }
                else if (Convert.ToInt32(growth.Text) <= 185 && Convert.ToDouble(weightDesired.Text) >= 90)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }
                else if (Convert.ToInt32(growth.Text) <= 190 && Convert.ToDouble(weightDesired.Text) >= 95)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }
                else if (Convert.ToInt32(growth.Text) <= 195 && Convert.ToDouble(weightDesired.Text) >= 100)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }
                else if (Convert.ToInt32(growth.Text) <= 200 && Convert.ToDouble(weightDesired.Text) >= 105)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }
                else if (Convert.ToInt32(growth.Text) <= 205 && Convert.ToDouble(weightDesired.Text) >= 110)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }
                else if (Convert.ToInt32(growth.Text) <= 210 && Convert.ToDouble(weightDesired.Text) >= 115)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }
                else if (Convert.ToInt32(growth.Text) <= 215 && Convert.ToDouble(weightDesired.Text) >= 125)
                {
                    MessageBox.Show("Выбранный Вами вес является избыточным.", "Предупреждение");
                    numOfExeptions += 1;
                }

                //отслеживание кол-ва сбрасываемых кг за выбранный период
                if (((Convert.ToDouble(weightCurrent.Text) - Convert.ToDouble(weightDesired.Text)) > 1 && Convert.ToString(lenghtOfDiet.Text).Equals("1 неделя"))
                || ((Convert.ToDouble(weightCurrent.Text) - Convert.ToDouble(weightDesired.Text)) > 2 && Convert.ToString(lenghtOfDiet.Text).Equals("2 недели"))
                || ((Convert.ToDouble(weightCurrent.Text) - Convert.ToDouble(weightDesired.Text)) > 3 && Convert.ToString(lenghtOfDiet.Text).Equals("3 недели"))
                || ((Convert.ToDouble(weightCurrent.Text) - Convert.ToDouble(weightDesired.Text)) > 4 && Convert.ToString(lenghtOfDiet.Text).Equals("1 месяц"))
                || ((Convert.ToDouble(weightCurrent.Text) - Convert.ToDouble(weightDesired.Text)) > 6 && Convert.ToString(lenghtOfDiet.Text).Equals("1,5 месяца"))
                || ((Convert.ToDouble(weightCurrent.Text) - Convert.ToDouble(weightDesired.Text)) > 8 && Convert.ToString(lenghtOfDiet.Text).Equals("2 месяца"))
                || ((Convert.ToDouble(weightCurrent.Text) - Convert.ToDouble(weightDesired.Text)) > 12 && Convert.ToString(lenghtOfDiet.Text).Equals("3 месяца"))
                || ((Convert.ToDouble(weightCurrent.Text) - Convert.ToDouble(weightDesired.Text)) > 16 && Convert.ToString(lenghtOfDiet.Text).Equals("4 месяца"))
                || ((Convert.ToDouble(weightCurrent.Text) - Convert.ToDouble(weightDesired.Text)) > 24 && Convert.ToString(lenghtOfDiet.Text).Equals("0,5 года"))
                || ((Convert.ToDouble(weightCurrent.Text) - Convert.ToDouble(weightDesired.Text)) > 48 && Convert.ToString(lenghtOfDiet.Text).Equals("год")))
                {
                    MessageBox.Show("Cбрасывать выбранное количество кг в выбранный период времени опасно для здоровья! Пожалуйста, выберите более продолжительный период или укажите меньшее количество килограмм!", "Предупреждение");
                    numOfExeptions += 1;
                }
            }

            catch (FormatException)
            {
                MessageBox.Show("Пожалуйста, вводите числа.");
                numOfExeptions += 1;
            }

            if (numOfExeptions == 0)
            {
                CreateBasePersonalData();
                MessageBox.Show("Пользователь сохранен!");
            }
        }

        //Создание бд и добавление в неё данных
        private void CreateBasePersonalData()
        {
            if (File.Exists(basePersonalData))
            {
                try
                {
                    ConnectBd();
                    commandIn.CommandText = "SELECT gender FROM user";
                    SQLiteDataReader reader = commandIn.ExecuteReader();
                    if (!reader.Read())
                    {
                        commandOut.CommandText = "INSERT INTO user ('gender', 'age', 'growth', 'weight', 'weight_desired', 'lenght_of_diet') values ('" + Convert.ToString(gender.Text) + "' , '" + Convert.ToInt16(age.Text) + "' , '" + Convert.ToInt16(growth.Text) + "' , '" + Convert.ToDouble(weightCurrent.Text) + "' , '" + Convert.ToDouble(weightDesired.Text) + "' , '" + Convert.ToString(lenghtOfDiet.Text) + "')";
                        commandOut.ExecuteNonQuery();
                        reader.Close();
                        connectBD.Close();
                    }
                    else
                    {
                        commandOut.CommandText = "UPDATE user SET gender = '" + Convert.ToString(gender.Text) + "', age = '" + Convert.ToInt16(age.Text) + "' , growth = '" + Convert.ToInt16(growth.Text) + "' , weight = '" + Convert.ToDouble(weightCurrent.Text) + "' , weight_desired = '" + Convert.ToDouble(weightDesired.Text) + "', lenght_of_diet = '" + Convert.ToString(lenghtOfDiet.Text) + "'";
                        commandOut.ExecuteNonQuery();
                        reader.Close();
                        connectBD.Close();
                    }
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
                    commandOut.CommandText = "CREATE TABLE user(gender STRING, age INTEGER, growth INTEGER, weight DOUBLE, weight_desired DOUBLE, lenght_of_diet STRING, diet_id INTEGER, coefficient_of_active DOUBLE)";
                    commandOut.ExecuteNonQuery();

                    commandOut.CommandText = "INSERT INTO user ('gender', 'age', 'growth', 'weight', 'weight_desired', 'lenght_of_diet') values ('" + Convert.ToString(gender.Text) + "' , '" + Convert.ToInt16(age.Text) + "' , '" + Convert.ToInt16(growth.Text) + "' , '" + Convert.ToDouble(weightCurrent.Text) + "' , '" + Convert.ToDouble(weightDesired.Text) + "' , '" + Convert.ToString(lenghtOfDiet.Text) + "')";
                    commandOut.ExecuteNonQuery();
                    connectBD.Close();
                }
                catch (SQLiteException)
                {
                    MessageBox.Show("База данных не создана!", "Предупреждение");
                }
            }
        }

        //Выставление пустых данных
        private void SetEmptyData()
        {
            gender.Text = " ";
            age.Text = " ";
            growth.Text = " ";
            weightCurrent.Text = " ";
            weightDesired.Text = " ";
            lenghtOfDiet.Text = " ";
        }

        //Считывание данных
        private void TakeDataFromBD()
        {
            if (!File.Exists(basePersonalData))
            {
                SetEmptyData();
            }
            else if (File.Exists(basePersonalData))
            {
                try
                {
                    ConnectBd();
                    commandOut.CommandText = "CREATE TABLE IF NOT EXISTS user(gender STRING, age INTEGER, growth INTEGER, weight DOUBLE, weight_desired DOUBLE, lenght_of_diet STRING, diet_id INTEGER, coefficient_of_active DOUBLE)";
                    commandOut.ExecuteNonQuery();
                    commandIn.CommandText = "SELECT gender FROM user";
                    SQLiteDataReader reader = commandIn.ExecuteReader();
                    if (reader.Read())
                    {
                        String str = Convert.ToString(reader.GetValue(0));
                        gender.Text = str;
                        reader.Close();

                        commandIn.CommandText = "SELECT age FROM user";
                        reader = commandIn.ExecuteReader();
                        reader.Read();
                        age.Text = Convert.ToString(reader.GetValue(0));
                        reader.Close();

                        commandIn.CommandText = "SELECT growth FROM user";
                        reader = commandIn.ExecuteReader();
                        reader.Read();
                        growth.Text = Convert.ToString(reader.GetValue(0));
                        reader.Close();

                        commandIn.CommandText = "SELECT weight FROM user";
                        reader = commandIn.ExecuteReader();
                        reader.Read();
                        weightCurrent.Text = Convert.ToString(reader.GetValue(0));
                        reader.Close();

                        commandIn.CommandText = "SELECT weight_desired FROM user";
                        reader = commandIn.ExecuteReader();
                        reader.Read();
                        weightDesired.Text = Convert.ToString(reader.GetValue(0));
                        reader.Close();

                        commandIn.CommandText = "SELECT lenght_of_diet FROM user";
                        reader = commandIn.ExecuteReader();
                        reader.Read();
                        lenghtOfDiet.Text = Convert.ToString(reader.GetValue(0));
                        reader.Close();

                        connectBD.Close();
                    }

                    else if (!reader.Read())
                    {
                        SetEmptyData();
                        reader.Close();
                        connectBD.Close();
                    }
                    else
                    {
                        SetEmptyData();
                        connectBD.Close();
                        reader.Close();
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

        //Подключение к БД
        private void ConnectBd()
        {
            try
            {
                connectBD = new SQLiteConnection("Data Source=" + basePersonalData + ";Version=3;" + "UseUTF16Encoding = True;");
                connectBD.Open();
                commandOut = new SQLiteCommand();
                commandOut.Connection = connectBD;
                commandIn.Connection = connectBD;
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

        private void Age_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Growth_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void WeightDesired_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void WeightCurrent_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        // Переход на с раницу "Выбор диеты"
        public void ChoiceDietPage_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ChoiceDiet choiceDietPage = new ChoiceDiet();
            choiceDietPage.Left = (100);
            choiceDietPage.Top = (100);
            choiceDietPage.Show();
        }

        private void PersonalDataPage_Click(object sender, RoutedEventArgs e)
        {

        }

        // Переход на с раницу "Дневник питания"
        private void FoodDiaryPage_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            FoodDiary foodDiaryPage = new FoodDiary();
            foodDiaryPage.Left = (100);
            foodDiaryPage.Top = (100);
            foodDiaryPage.Show();
        }

        private void Gender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
