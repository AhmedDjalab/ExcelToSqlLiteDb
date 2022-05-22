using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace ExcelToSqlLiteDb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string FilePath { get; set; }
        public string selectedDegree { get; set; }
        public string selectedDesc { get; set; }
        public bool DataBaseIsEmpty { get; set; }
        SQLiteConnection Con = SeedDatabase.Con;
        SQLiteCommand com;
        string str;

        // when load must verify if there any data in dataBase 
        public bool CheckDataBaseIsEmpty()
        {
            // check if des and deg table exist and have data inside 
            var DegDesJointData = SelectFromDegreeDescFromDb();
            if (DegDesJointData.Count - 1 == 0)

            {
                MessageBox.Show("there no data in Database ,please add New Excel sheet ");

                return true;
            }
            MessageBox.Show("there  data in Database , you can ovveride it with new Excel sheet");

            return false;


        }




        private void GetDegreeAndDescriptionDatoToSelect()
        {


            // the table is not filled wwith data 


            List<List<string>> DegDesData = SelectFromDegreeAndDescFromDb();



            // show it in combobox 
            degreeItem.ItemsSource = DegDesData[0];
            descItem.ItemsSource = DegDesData[1];
        }

        public MainWindow()
        {
            InitializeComponent();
            SeedDatabase.seedData();
            DataBaseIsEmpty = CheckDataBaseIsEmpty();
            GetDegreeAndDescriptionDatoToSelect();
        }



        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {


            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            openFileDlg.DefaultExt = ".xlsx";
            openFileDlg.Filter = "Excel documents (.xlsx)|*.xlsx";


            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                FileNameTextBox.Text = openFileDlg.FileName;
                FilePath = openFileDlg.FileName;

                FillDatabase();

            }
        }



        private void FillDatabase()
        {
            // Load the data fromexcel 


            string fileName = FilePath;
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(fileName);
            Worksheet sheet = workbook.Worksheets[0];
            // this can't work in the case of duplicated nubmers
            DataTable data = sheet.ExportDataTable();



            int WidthOfTable = data.Columns.Count;
            int HeightOfTable = data.Rows.Count;

            //we need to check if there data in database 
            // here we have a new excel data and need to ovveride the database
            if(!DataBaseIsEmpty)
            {

                DeleteDegreeDescriptionTable();
            } 



            // fill the degree and description table in Database
            CreateDegreeDescriptionTable(WidthOfTable, HeightOfTable);



            // fill the degree description tAble 
            FillDegreeDescJointTable(data);

            // add to selectors 
            GetDegreeAndDescriptionDatoToSelect();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            // Write the data        

            Con.Open();


            // check if des and deg table exist and have data inside 


            //////////////////////


            Con.Close();

        }


        private List<string> SelectFromDegreeDescFromDb()
        {
            Con.Open();
            List<string> degreeDesItemsData = new List<string>();
            List<List<string>> degDes = new List<List<string>>();
            str = "select * from degreeDescription";
            com = new SQLiteCommand(str, Con);
            SQLiteDataReader r = com.ExecuteReader();
            while (r.Read())
            {
                degreeDesItemsData.Add(Convert.ToString(r["Amount"]));
            }

            Con.Close();
            return degreeDesItemsData;
        }

        private List<List<string>> SelectFromDegreeAndDescFromDb()
        {
            Con.Open();
            List<string> descItemsData = new List<string>();
            List<string> degreeItemsData = new List<string>();
            List<List<string>> degDes = new List<List<string>>();
            str = "select Title from Degree";
            com = new SQLiteCommand(str, Con);
            SQLiteDataReader r = com.ExecuteReader();
            while (r.Read())
            {
                degreeItemsData.Add(Convert.ToString(r["Title"]));
            }

            str = "select Title from Description";
            com = new SQLiteCommand(str, Con);
            SQLiteDataReader r2 = com.ExecuteReader();
            while (r2.Read())
            {
                descItemsData.Add(Convert.ToString(r2["Title"]));
            }


            degDes.Add(degreeItemsData);
            degDes.Add(descItemsData);
            Con.Close();
            return degDes;
        }

        private void FillDegreeDescJointTable(DataTable data)
        {
            int degreeId = 0;
            Con.Open();
            foreach (DataRow row in data.Rows) //iterate over all rows
            {
                degreeId++;
                for (int i = 0; i <= data.Columns.Count - 1; i++)
                {
                    str = "insert into DegreeDescription(Amount,DegreeId,DescriptionId)values(@Amount,@DegreeId,@DescriptionId)";
                    com = new SQLiteCommand(str, Con);
                    com.Parameters.AddWithValue("@Amount", row.Field<string>(i));
                    com.Parameters.AddWithValue("@DegreeId", degreeId >= data.Rows.Count - 1 ? data.Rows.Count - 1 : degreeId);
                    com.Parameters.AddWithValue("@DescriptionId", i + 1);
                    com.ExecuteNonQuery();
                }



            }
            Con.Close();
        }



        private void DeleteDegreeDescriptionTable()
        {
            Con.Open();

            str = "delete from Description";

            com = new SQLiteCommand(str, Con);

            com.ExecuteNonQuery();



            str = "delete from Degree";

            com = new SQLiteCommand(str, Con);

            com.ExecuteNonQuery();
            
            str = "delete from DegreeDescription";

            com = new SQLiteCommand(str, Con);

            com.ExecuteNonQuery();




            Con.Close();


        }

        private void CreateDegreeDescriptionTable(int WidthOfTable, int HeightOfTable)
        {
            Con.Open();
            for (int i = 1; i <= WidthOfTable; i++)
            {
                str = "insert into Description(Title , Id)values(@Title , @Id)";
                com = new SQLiteCommand(str, Con);
                com.Parameters.AddWithValue("@Title", i);
                com.Parameters.AddWithValue("@Id", i);

                com.ExecuteNonQuery();
            }

            for (int i = 1; i <= HeightOfTable; i++)
            {
                str = "insert into Degree(Title , Id)values(@Title , @Id)";
                com = new SQLiteCommand(str, Con);
                com.Parameters.AddWithValue("@Title", i);
                com.Parameters.AddWithValue("@Id", i);
                com.ExecuteNonQuery();
            }
            MessageBox.Show("Degree & Descritpion has being filled with Data , you can see them in the " +
                "" +
                "checkboxes  ");


            Con.Close();


        }

        private void degreeItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void descItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void GetAmount(string degree, string desc)
        {
            if (String.IsNullOrEmpty(degree) || String.IsNullOrEmpty(desc))
            {
                MessageBox.Show("please select both degree & description");
                return;
            }
            if (Con.State == ConnectionState.Closed)
                Con.Open();

            str = "SELECT Amount from DegreeDescription WHERE DegreeId=@deg and DescriptionId=@des";
            com = new SQLiteCommand(str, Con);
            com.Parameters.AddWithValue("@deg", int.Parse(degreeItem.SelectedItem.ToString()));
            com.Parameters.AddWithValue("@des", int.Parse(descItem.SelectedItem.ToString()));
            SQLiteDataReader datar = com.ExecuteReader();
            while (datar.Read())
            {
                amount.Text = Convert.ToString(datar["Amount"]);

            }
            Con.Close();


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GetAmount(degreeItem.SelectedItem.ToString(), descItem.SelectedItem.ToString());

        }
    }
}
