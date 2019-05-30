using System;
using System.Collections.Generic;
using System.Data;
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
using System.Globalization;
using System.IO;
using System.Threading;

namespace наВинду
{
    /// <summary>
    /// Логика взаимодействия для FoodDiary.xaml
    /// </summary>
    public partial class FoodDiary : Window
    {
        public FoodDiary()
        {
            InitializeComponent();
            CalculationBasicIndicators();
            DefinitionCurrentDate();
            CreateTableForProduct();
        }

        private static String basePersonalData = "PersonalData.sql";
        private static SQLiteConnection connectBD;
        private DateTime dayMonth;
        private SQLiteCommand command;

        TextBox textBoxForDate = new TextBox();
        TextBox textBoxForIdOfReceptionOfFood = new TextBox();
        
        private Int32 dietId;
        private Double calories;
        private Double protein;
        private Double fat;
        private Double carbohydrates;
        private String typeOfDay;

        private Double proteinPDay;
        private Double fatPDay;
        private Double carbohydratesPDay;

        private Double proteinCDay;
        private Double fatCDay;
        private Double carbohydratesCDay;

        private Double ccalsReceived;
        private Double proteinsReceived;
        private Double fatsReceived;
        private Double carbohydratesReceived;


        //переход в окно изменения/удаления объекта в приеме пищи
        private void OpenWindowchangeOrDeletePD(String nameOfProdDish, String reseptionFood, String dateOfReceptionFood)
        {
            ChangeOrDeleteProdDish changeOrDeleteProdDishPage = new ChangeOrDeleteProdDish(nameOfProdDish, reseptionFood, dateOfReceptionFood, this);
            changeOrDeleteProdDishPage.Left = (100);
            changeOrDeleteProdDishPage.Top = (100);
            changeOrDeleteProdDishPage.Show();
        }

        //реакция на нажатие продукта/блюда в завтраке
        private void ProdDishBreakfast_Click(object sender, RoutedEventArgs e)
        {
            String nameOfProdDish = ((Button)sender).Content.ToString();
            String nameOfResFood = "breakfast_in_date";
            String dayMon = labelDayMonth.Content.ToString();
            OpenWindowchangeOrDeletePD(nameOfProdDish, nameOfResFood, dayMon);
        }

        //реакция на нажатие продукта/блюда в обеде
        private void ProdDishDinner_Click(object sender, RoutedEventArgs e)
        {
            String nameOfProdDish = ((Button)sender).Content.ToString();
            String nameOfResFood = "dinner_in_date";
            String dayMon = labelDayMonth.Content.ToString();
            OpenWindowchangeOrDeletePD(nameOfProdDish, nameOfResFood, dayMon);
        }

        //реакция на нажатие продукта/блюда в ужине
        private void ProdDishSupper_Click(object sender, RoutedEventArgs e)
        {
            String nameOfProdDish = ((Button)sender).Content.ToString();
            String nameOfResFood = "supper_in_date";
            String dayMon = labelDayMonth.Content.ToString();
            OpenWindowchangeOrDeletePD(nameOfProdDish, nameOfResFood, dayMon);
        }

        //реакция на нажатие продукта/блюда в перекусах
        private void ProdDishSnack_Click(object sender, RoutedEventArgs e)
        {
            String nameOfProdDish = ((Button)sender).Content.ToString();
            String nameOfResFood = "nosh_in_date";
            String dayMon = labelDayMonth.Content.ToString();
            OpenWindowchangeOrDeletePD(nameOfProdDish, nameOfResFood, dayMon);
        }

        //Создание таблицы для продуктов и блюд
        private void CreateTableForProduct()
        {
            ConnectBd();
            command.CommandText = "CREATE TABLE IF NOT EXISTS product (id INTEGER UNIQUE NOT NULL, name STRING UNIQUE NOT NULL, serving_size DOUBLE NOT NULL, ccal DOUBLE NOT NULL, proteins DOUBLE NOT NULL, fats DOUBLE NOT NULL, carbohydrates DOUBLE NOT NULL, units STRING NOT NULL, PRIMARY KEY (id))";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE TABLE IF NOT EXISTS dish(id INTEGER UNIQUE NOT NULL, name STRING NOT NULL, serving_size DOUBLE NOT NULL, ccal DOUBLE, proteins DOUBLE, fats DOUBLE, carbohydrates DOUBLE, units STRING, PRIMARY KEY (id))";
            command.ExecuteNonQuery();
            connectBD.Close();
        }

        //Создание таблиц в бд для каждго приема пищи
        private void CreateTableOfReceptionFood(String reception)
        {
            ConnectBd();
            try
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS '" + reception + "' (name_of_product STRING, quantity DOUBLE, data_num DATE, ccal DOUBLE, proteins DOUBLE, fats DOUBLE, carbohydrates DOUBLE)";
                command.ExecuteNonQuery();
                connectBD.Close();
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Таблицы приемов пищи не созданы!");
            }
        }

        //Вывод сохраненных данных (из таблиц завтрака, обеда, ужина, перекусов)
        public void OutputDataOfReceptionsFood(String dayOfReceptionFood)
        {
            ConnectBd();
            String breakfast = "breakfast_in_date";
            CreateTableOfReceptionFood(breakfast);
            SearchAndSaveDataOfReceptionFood(breakfast, dayOfReceptionFood);
            String dinner = "dinner_in_date";
            CreateTableOfReceptionFood(dinner);
            SearchAndSaveDataOfReceptionFood(dinner, dayOfReceptionFood);
            String supper = "supper_in_date";
            CreateTableOfReceptionFood(supper);
            SearchAndSaveDataOfReceptionFood(supper, dayOfReceptionFood);
            String nosh = "nosh_in_date";
            CreateTableOfReceptionFood(nosh);
            SearchAndSaveDataOfReceptionFood(nosh, dayOfReceptionFood);
        }

        //Прохождение по таблицам, сохранение данных в переменные для КБЖУ для дальнейшего их подсчета (для вывода на экран количество употребленных КБЖУ)
        private void SearchAndSaveDataOfReceptionFood(String receptionOfFood, String dayOfReception)
        {
            String nameOfProductOFReceptionFood;
            DataTable dataTable = new DataTable();
            String sqlCom1 = "SELECT name_of_product FROM '" + receptionOfFood + "' WHERE data_num = '" + dayOfReception + "'";
            SQLiteDataAdapter adapter1 = new SQLiteDataAdapter(sqlCom1, connectBD);
            adapter1.Fill(dataTable);
            int countOfRows = dataTable.Rows.Count;
            String[] namesOfProdInRecFood = new string[20];
            int index = 0;

            if (countOfRows > 0)
            {
                ConnectBd();
                command.CommandText = "SELECT name_of_product FROM '" + receptionOfFood + "' WHERE data_num = '" + dayOfReception + "'";
                SQLiteDataReader readerForName = command.ExecuteReader();
                while (readerForName.Read())
                {
                    namesOfProdInRecFood[index] = Convert.ToString(readerForName.GetString(readerForName.GetOrdinal("name_of_product")));
                    index++;
                }
                readerForName.Close();
                for (int i = 0; i < 20; i++)
                {
                    if (i < index)
                    {
                        nameOfProductOFReceptionFood = namesOfProdInRecFood[i];
                        AddNewProdOrDishInReceptionFood(receptionOfFood, nameOfProductOFReceptionFood);
                        SaveDataOfProdInReceptionFood(receptionOfFood, dayOfReception, nameOfProductOFReceptionFood);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                nameOfProductOFReceptionFood = null;
            }
        }

        //Вывод нового продукта в приеме пищи в окно
        private void AddNewProdOrDishInReceptionFood(String resFood, String name)
        {
            Button buttonProdForReceptionFood = new Button
            {
                Height = 27,
                Width = 325,
                Content = name,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            switch (resFood)
            {
                case "breakfast_in_date":
                    breakfastStackPanel.Children.Add(buttonProdForReceptionFood);
                    buttonProdForReceptionFood.Click += new RoutedEventHandler(ProdDishBreakfast_Click);
                    break;
                case "dinner_in_date":
                    dinnerStackPanel.Children.Add(buttonProdForReceptionFood);
                    buttonProdForReceptionFood.Click += new RoutedEventHandler(ProdDishDinner_Click);
                    break;
                case "supper_in_date":
                    supperStackPanel.Children.Add(buttonProdForReceptionFood);
                    buttonProdForReceptionFood.Click += new RoutedEventHandler(ProdDishSupper_Click);
                    break;
                case "nosh_in_date":
                    snackStackPanel.Children.Add(buttonProdForReceptionFood);
                    buttonProdForReceptionFood.Click += new RoutedEventHandler(ProdDishSnack_Click);
                    break;
            }
        }

        //Сохранение данных по имени продукта в приеме пищи
        private void SaveDataOfProdInReceptionFood(String recFood, String dayRecFood, String nameOfProd)
        {
            Double ccalOfReceptionFood;
            Double proteinsOfReceptionFood;
            Double fatsOfReceptionFood;
            Double carbohydratesOfReceptionFood;

            command.CommandText = "SELECT ccal FROM '" + recFood + "' WHERE data_num = '" + dayRecFood + "' AND name_of_product = '" + nameOfProd + "'";
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                ccalOfReceptionFood = Convert.ToDouble(reader.GetValue(0));
            }
            else
            {
                ccalOfReceptionFood = 0;
            }
            reader.Close();

            command.CommandText = "SELECT proteins FROM '" + recFood + "' WHERE data_num = '" + dayRecFood + "' AND name_of_product = '" + nameOfProd + "'";
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                proteinsOfReceptionFood = Convert.ToDouble(reader.GetValue(0));
            }
            else
            {
                proteinsOfReceptionFood = 0;
            }
            reader.Close();

            command.CommandText = "SELECT fats FROM '" + recFood + "' WHERE data_num = '" + dayRecFood + "' AND name_of_product = '" + nameOfProd + "'";
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                fatsOfReceptionFood = Convert.ToDouble(reader.GetValue(0));
            }
            else
            {
                fatsOfReceptionFood = 0;
            }
            reader.Close();

            command.CommandText = "SELECT carbohydrates FROM '" + recFood + "' WHERE data_num = '" + dayRecFood + "' AND name_of_product = '" + nameOfProd + "'";
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                carbohydratesOfReceptionFood = Convert.ToDouble(reader.GetValue(0));
            }
            else
            {
                carbohydratesOfReceptionFood = 0;
            }
            reader.Close();

            CalculationRemainingKPFC(ccalOfReceptionFood, proteinsOfReceptionFood, fatsOfReceptionFood, carbohydratesOfReceptionFood);
        }

        //Расчет оставшихся КБЖУ и вывод их на экран
        private void CalculationRemainingKPFC(Double ccalOfProduct, Double proteinsOfProduct, Double fatsOfProduct, Double carbohydratesOfProduct)
        {
            double ccalRemaining, prLeft, fatsLeft, carbLeft;
            ccalsReceived += ccalOfProduct;
            labelCcalReceived.Content = ccalsReceived.ToString("#.##");

            if (LabelCcalAim.Content.Equals("0") || labelCcalReceived.Content.Equals("0") || labelCcalReceived.Content.Equals(" ") || LabelCcalAim.Content.Equals("") || LabelCcalAim.Content.Equals(" ") || labelCcalReceived.Content.Equals(""))
            {
                labelCcalRemaining.Content = 0;
            }
            else
            {
                ccalRemaining = Convert.ToDouble(LabelCcalAim.Content) - Convert.ToDouble(labelCcalReceived.Content);
                labelCcalRemaining.Content = ccalRemaining.ToString("#.##");
            }

            proteinsReceived += proteinsOfProduct;
            if (labelPNum.Content.Equals("0") || labelPNum.Content.Equals(""))
            {
                labelgrPLeft.Content = 0;
            }
            else
            {
                prLeft = Convert.ToDouble(labelPNum.Content) - proteinsReceived;
                labelgrPLeft.Content = prLeft.ToString("#.##");
            }

            fatsReceived += fatsOfProduct;
            if (labelFNum.Content.Equals("0") || labelFNum.Content.Equals(""))
            {
                labelgrFLeft.Content = 0;
            }
            else
            {
                fatsLeft = Convert.ToDouble(labelFNum.Content) - fatsReceived;
                labelgrFLeft.Content = fatsLeft.ToString("#.##");
            }

            carbohydratesReceived += carbohydratesOfProduct;
            if (labelCNum.Content.Equals("0") || labelCNum.Content.Equals(""))
            {
                labelgrCLeft.Content = 0;
            }
            else
            {
                carbLeft = Convert.ToDouble(labelCNum.Content) - carbohydratesReceived;
                labelgrCLeft.Content = carbLeft.ToString("#.##");
            }
        }

        //Определение текущей даты и совершение дальнейших операций над ней
        private void DefinitionCurrentDate()
        {
            dayMonth = DateTime.Now;
            labelDayMonth.Content = dayMonth.ToString("dd.MM.yy");
            DefinitionOfDateTime(dayMonth);
            IndicationBasicIndicators(dayMonth.ToString("dd.MM.yy"));
            OutputDataOfReceptionsFood(Convert.ToString(dayMonth.ToString("dd.MM.yy")));
        }

        private void FoodDiaryPage_Click(object sender, RoutedEventArgs e)
        {

        }

        //Расчет оставшихся ккал, белков, жиров, углеводов
        private void CalculateBalanceParametrs()
        {
            double cR, pL, fL, cL = 0;
            cR = Convert.ToDouble(LabelCcalAim.Content) - Convert.ToDouble(labelCcalReceived.Content);
            labelCcalRemaining.Content = cR.ToString("#.##");
            
            pL = Convert.ToDouble(labelPNum.Content) - proteinsReceived;
            labelgrPLeft.Content = pL.ToString("#.##");

            fL = Convert.ToDouble(labelFNum.Content) - fatsReceived;
            labelgrFLeft.Content = fL.ToString("#.##");

            cL = Convert.ToDouble(labelCNum.Content) - carbohydratesReceived;
            labelgrCLeft.Content = cL.ToString("#.##");
        }

        //Определение типа дня
        private void DefinitionTypeOfDay(DateTime dayMon)
        {
            if (dietId == 0)
            {
                typeOfDay = " ";
            }
            if (dietId == 1)
            {
                int day = Convert.ToInt32(dayMon.ToString("dd"));
                int a = 1;
                for (int h = 1; h <= 31; h++)
                {
                    if (h == day)
                    {
                        switch (a)
                        {
                            case 1:
                                typeOfDay = "Белковый день";
                                a++;
                                break;
                            case 2:
                                typeOfDay = "Углеводный день";
                                a++;
                                break;
                            case 3:
                                typeOfDay = "Смешанный день";
                                a = 0;
                                break;
                        }
                    }
                    else
                    {
                        if (a < 3)
                        {
                            a++;
                        }
                        else if (a == 3)
                        {
                            a = 1;
                        }
                    }
                }
            }
            else if (dietId == 5)
            {
                int i = Convert.ToInt32(dayMon.ToString("dd"));
                i = i % 2;
                if (i == 0)
                {
                    typeOfDay = "Белковый день";
                }
                else
                {
                    typeOfDay = "Углеводный день";
                }
            }
            else
            {
                typeOfDay = " ";
            }
        }

        //Получение связи с бд
        private void ConnectBd()
        {
            connectBD = new SQLiteConnection("Data Source=" + basePersonalData + ";Version=3;" + "UseUTF16Encoding = True;");
            connectBD.Open();
            command = new SQLiteCommand();
            command.Connection = connectBD;
        }

        //Определение даты, создание таблицы для неё
        public void DefinitionOfDateTime(DateTime date)
        {
            DefinitionTypeOfDay(date);
            try
            {
                ConnectBd();
                command.CommandText = "CREATE TABLE IF NOT EXISTS date (day DATE NOT NULL UNIQUE, type_of_day STRING, PRIMARY KEY (day))";
                command.ExecuteNonQuery();

                command.CommandText = "SELECT day FROM date WHERE day = '" + date.ToString("dd.MM.yy") + "'";
                SQLiteDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    reader.Close();
                    command.CommandText = "INSERT INTO date ('day', 'type_of_day') values ('" + date.ToString("dd.MM.yy") + "' , '" + typeOfDay.ToString() + "' ) ";
                    command.ExecuteNonQuery();
                    reader.Close();
                    connectBD.Close();
                }
                else
                {
                    reader.Close();
                    connectBD.Close();
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Соединение с БД не получено!", "Предупреждение");
            }
        }

        //Вывод данных бжу (смешанного дня)
        private void DefinitionPFCMixedDay()
        {
            if (protein == 0)
            {
                labelPNum.Content = "0";
            }
            else
            {
                labelPNum.Content = protein.ToString("#.##");
            }

            if (fat == 0)
            {
                labelFNum.Content = "0";
            }
            else
            {
                labelFNum.Content = fat.ToString("#.##");
            }

            if (carbohydrates == 0)
            {
                labelCNum.Content = "0";
            }
            else
            {
                labelCNum.Content =carbohydrates.ToString("#.##");
            }
        }

        //Вывод данных бжу (белкового дня)
        private void DefinitionPFCProteinDay()
        {
            labelPNum.Content = proteinPDay.ToString("#.##");
            labelFNum.Content = fatPDay.ToString("#.##");
            labelCNum.Content = carbohydratesPDay.ToString("#.##");
        }

        //Вывод данных бжу (углеводного дня)
        private void DefinitionPFCCarbohydrateDay()
        {
            labelPNum.Content = proteinCDay.ToString("#.##");
            labelFNum.Content = fatCDay.ToString("#.##");
            labelCNum.Content = carbohydratesCDay.ToString("#.##");
        }

        //Вывод данных на окно в зависимости от типа дня<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        public void IndicationBasicIndicators(String date)
        {
            //DefinitionTypeOfDay(date);
            LabelCcalAim.Content = calories.ToString("#.##");
            switch (dietId)
            {
                case 0:
                    DefinitionPFCMixedDay();
                    break;
                case 1:
                    switch (typeOfDay)
                    {
                        case "Белковый день":
                            DefinitionPFCProteinDay();
                            break;
                        case "Углеводный день":
                            DefinitionPFCCarbohydrateDay();
                            break;
                        case "Смешанный день":
                            DefinitionPFCMixedDay();
                            break;
                    }
                    break;
                case 2:
                    DefinitionPFCMixedDay();
                    break;
                case 3:
                    DefinitionPFCMixedDay();
                    break;
                case 4:
                    DefinitionPFCMixedDay();
                    break;
                case 5:
                    switch (typeOfDay)
                    {
                        case "Белковый день":
                            DefinitionPFCProteinDay();
                            break;
                        case "Углеводный день":
                            DefinitionPFCCarbohydrateDay();
                            break;
                    }
                    break;
            }
        }

        //Расчет калоража и БЖУ по выбранной диете
        private void CalculationBasicIndicators()
        {
            Int32 growth;
            Int32 age;
            String gender;
            Double weightDesired;
            Double coefficientOfActive;

            try
            {
                ConnectBd();

                command.CommandText = "SELECT diet_id FROM user";
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if(reader.GetValue(0).ToString() == "")
                    {
                        dietId = 0;
                        reader.Close();
                    }
                    else
                    {
                        dietId = Convert.ToInt32(reader.GetValue(0));
                        reader.Close();
                    }

                    command.CommandText = "SELECT coefficient_of_active FROM user";
                    reader = command.ExecuteReader();
                    reader.Read();
                    if (reader.GetValue(0).ToString() == "")
                    {
                        coefficientOfActive = 1;
                        reader.Close();
                    }
                    else
                    {
                        coefficientOfActive = Convert.ToDouble(reader.GetValue(0));
                        reader.Close();
                    }

                    command.CommandText = "SELECT growth FROM user";
                    reader = command.ExecuteReader();
                    reader.Read();
                    growth = Convert.ToInt32(reader.GetValue(0));
                    reader.Close();

                    command.CommandText = "SELECT age FROM user";
                    reader = command.ExecuteReader();
                    reader.Read();
                    age = Convert.ToInt32(reader.GetValue(0));
                    reader.Close();

                    command.CommandText = "SELECT gender FROM user";
                    reader = command.ExecuteReader();
                    reader.Read();
                    gender = Convert.ToString(reader.GetValue(0));
                    reader.Close();

                    command.CommandText = "SELECT weight_desired FROM user";
                    reader = command.ExecuteReader();
                    reader.Read();
                    weightDesired = Convert.ToDouble(reader.GetValue(0));
                    reader.Close();

                    int idDiet = Convert.ToInt32(dietId);

                    switch (idDiet)
                    {
                        case 0:
                            calories = (9.99 * weightDesired + 6.25 * growth - 4.92 * age - 161) * coefficientOfActive;
                            protein = ((calories * 30) / 100) / 4;
                            fat = ((calories * 30) / 100) / 9;
                            carbohydrates = ((calories * 40) / 100) / 4;
                            break;
                        case 1:
                            if (gender.Equals("Ж"))
                            {
                                calories = (9.99 * weightDesired + 6.25 * growth - 4.92 * age - 161) * coefficientOfActive;
                                proteinPDay = ((calories * 40) / 100) / 4;
                                fatPDay = ((calories * 20) / 100) / 9;
                                carbohydratesPDay = ((calories * 40) / 100) / 4;

                                proteinCDay = ((calories * 20) / 100) / 4;
                                fatCDay = ((calories * 30) / 100) / 9;
                                carbohydratesCDay = ((calories * 50) / 100) / 4;

                                protein = ((calories * 30) / 100) / 4;
                                fat = ((calories * 30) / 100) / 9;
                                carbohydrates = ((calories * 40) / 100) / 4;
                            }
                            else if (gender.Equals("М"))
                            {
                                calories = (9.99 * weightDesired + 6.25 * growth - 4.92 * age + 5) * coefficientOfActive;
                                proteinPDay = ((calories * 40) / 100) / 4;
                                fatPDay = ((calories * 20) / 100) / 9;
                                carbohydratesPDay = ((calories * 40) / 100) / 4;

                                proteinCDay = ((calories * 20) / 100) / 4;
                                fatCDay = ((calories * 30) / 100) / 9;
                                carbohydratesCDay = ((calories * 50) / 100) / 4;

                                protein = ((calories * 30) / 100) / 4;
                                fat = ((calories * 30) / 100) / 9;
                                carbohydrates = ((calories * 40) / 100) / 4;
                            }
                            break;
                        case 2:
                            if (gender.Equals("Ж"))
                            {
                                calories = (9.99 * weightDesired + 6.25 * growth - 4.92 * age - 161) * coefficientOfActive - 400;

                                protein = ((calories * 30) / 100) / 4;
                                fat = ((calories * 30) / 100) / 9;
                                carbohydrates = ((calories * 40) / 100) / 4;
                            }
                            else if (gender.Equals("М"))
                            {
                                calories = (9.99 * weightDesired + 6.25 * growth - 4.92 * age + 5) * coefficientOfActive - 300;

                                protein = ((calories * 30) / 100) / 4;
                                fat = ((calories * 30) / 100) / 9;
                                carbohydrates = ((calories * 40) / 100) / 4;
                            }
                            break;
                        case 3:
                            if (gender.Equals("Ж"))
                            {
                                calories = (9.99 * weightDesired + 6.25 * growth - 4.92 * age - 161) * coefficientOfActive;

                                protein = ((calories * 20) / 100) / 4;
                                fat = ((calories * 40) / 100) / 9;
                                carbohydrates = ((calories * 40) / 100) / 4;
                            }
                            else if (gender.Equals("М"))
                            {
                                calories = (9.99 * weightDesired + 6.25 * growth - 4.92 * age + 5) * coefficientOfActive;

                                protein = ((calories * 20) / 100) / 4;
                                fat = ((calories * 40) / 100) / 9;
                                carbohydrates = ((calories * 40) / 100) / 4;
                            }
                            break;
                        case 4:
                            if (gender.Equals("Ж"))
                            {
                                calories = (9.99 * weightDesired + 6.25 * growth - 4.92 * age - 161) * coefficientOfActive + 300;

                                protein = ((calories * 30) / 100) / 4;
                                fat = ((calories * 30) / 100) / 9;
                                carbohydrates = ((calories * 40) / 100) / 4;
                            }
                            else if (gender.Equals("М"))
                            {
                                calories = (9.99 * weightDesired + 6.25 * growth - 4.92 * age + 5) * coefficientOfActive + 500;

                                protein = ((calories * 30) / 100) / 4;
                                fat = ((calories * 30) / 100) / 9;
                                carbohydrates = ((calories * 40) / 100) / 4;
                            }
                            break;
                        case 5:
                            if (gender.Equals("Ж"))
                            {
                                calories = (9.99 * weightDesired + 6.25 * growth - 4.92 * age - 161) * coefficientOfActive;
                                proteinPDay = ((calories * 40) / 100) / 4;
                                fatPDay = ((calories * 20) / 100) / 9;
                                carbohydratesPDay = ((calories * 40) / 100) / 4;

                                proteinCDay = ((calories * 20) / 100) / 4;
                                fatCDay = ((calories * 30) / 100) / 9;
                                carbohydratesCDay = ((calories * 50) / 100) / 4;
                            }
                            else if (gender.Equals("М"))
                            {
                                calories = (9.99 * weightDesired + 6.25 * growth - 4.92 * age + 5) * coefficientOfActive;
                                proteinPDay = ((calories * 40) / 100) / 4;
                                fatPDay = ((calories * 20) / 100) / 9;
                                carbohydratesPDay = ((calories * 40) / 100) / 4;

                                proteinCDay = ((calories * 20) / 100) / 4;
                                fatCDay = ((calories * 30) / 100) / 9;
                                carbohydratesCDay = ((calories * 50) / 100) / 4;
                            }
                            break;
                    }
                }
                else if (!reader.Read())
                {
                    calories = 0;
                    protein = 0;
                    fat = 0;
                    carbohydrates = 0;
                }
                connectBD.Close();
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

        // Переход на страницу "Выбор диет"
        private void ChoiceDietPage_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ChoiceDiet choiceDietPage = new ChoiceDiet();
            choiceDietPage.Left = (100);
            choiceDietPage.Top = (100);
            choiceDietPage.Show();
        }

        // Переход на страницу "Личные данные"
        private void PersonalDataPage_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow personalData = new MainWindow();
            personalData.Left = (100);
            personalData.Top = (100);
            personalData.Show();
        }

        // Добавить продукт на завтраk
        private void ButtonBreakfast_Click(object sender, RoutedEventArgs e)
        {
            String date = Convert.ToString(labelDayMonth.Content);
            Int32 idOfReceptionOfFood = 1;
            this.Hide();
            ProductSelection productSelectionPage = new ProductSelection(idOfReceptionOfFood, date)
            {
                Left = (100),
                Top = (100)
            };
            productSelectionPage.Show();
        }

        // Дабавить продукт на ужин
        private void ButtonLunch_Click(object sender, RoutedEventArgs e)
        {
            String date = Convert.ToString(labelDayMonth.Content);
            Int32 idOfReceptionOfFood = 3;
            this.Hide();
            ProductSelection productSelectionPage = new ProductSelection(idOfReceptionOfFood, date)
            {
                Left = (100),
                Top = (100)
            };
            productSelectionPage.Show();

        }

        // Добавить продукт на обед
        private void ButtonDinner_Click(object sender, RoutedEventArgs e)
        {
            String date = Convert.ToString(labelDayMonth.Content);
            Int32 idOfReceptionOfFood = 2;
            this.Hide();
            ProductSelection productSelectionPage = new ProductSelection(idOfReceptionOfFood, date)
            {
                Left = (100),
                Top = (100)
            };
            productSelectionPage.Show();
        }

        // Добавить продукт в перекус
        private void ButtonSnack_Click(object sender, RoutedEventArgs e)
        {
            String date = Convert.ToString(labelDayMonth.Content);
            Int32 idOfReceptionOfFood = 4;
            this.Hide();
            ProductSelection productSelectionPage = new ProductSelection(idOfReceptionOfFood, date)
            {
                Left = (100),
                Top = (100)
            };
            productSelectionPage.Show();
        }

        //Очищение панели приемов пищи
        public void ClearStackPanelReceptionFood()
        {
            breakfastStackPanel.Children.Clear();
            dinnerStackPanel.Children.Clear();
            supperStackPanel.Children.Clear();
            snackStackPanel.Children.Clear();
        }

        //Очищение КБЖУ оставшихся и употребленных
        public void ClearCPFC()
        {
            labelCcalReceived.Content = "0";
            labelCcalRemaining.Content = "0";
            labelgrPLeft.Content = "0";
            labelgrFLeft.Content = "0";
            labelgrCLeft.Content = "0";
            ccalsReceived = 0;
            proteinsReceived = 0;
            fatsReceived = 0;
            carbohydratesReceived = 0;
        }

        //Переход на следущий день
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            dayMonth = dayMonth.AddDays(i + 1);
            labelDayMonth.Content = dayMonth.ToString("dd.MM.yy");
            ClearCPFC();
            DefinitionOfDateTime(dayMonth);
            IndicationBasicIndicators(dayMonth.ToString("dd.MM.yy"));
            ClearStackPanelReceptionFood();
            OutputDataOfReceptionsFood(Convert.ToString(dayMonth.ToString("dd.MM.yy")));
        }

        //Переход на предыдущий день
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int i = 0;
            dayMonth = dayMonth.AddDays(i - 1);
            labelDayMonth.Content = dayMonth.ToString("dd.MM.yy");
            ClearCPFC();
            DefinitionOfDateTime(dayMonth);
            IndicationBasicIndicators(dayMonth.ToString("dd.MM.yy"));
            ClearStackPanelReceptionFood();
            OutputDataOfReceptionsFood(Convert.ToString(dayMonth.ToString("dd.MM.yy")));
        }
    }
}
