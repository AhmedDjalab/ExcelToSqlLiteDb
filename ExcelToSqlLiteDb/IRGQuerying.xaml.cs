using Spire.Xls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ExcelToSqlLiteDb
{
    /// <summary>
    /// Interaction logic for IRGQuerying.xaml
    /// </summary>


    public partial class IRGQuerying : Window
    {
        public string FilePath { get; set; }

        public  DataTable dataRecords { get; set; }

        public bool DataBaseIsEmpty { get; set; }
        SQLiteConnection Con = SeedDatabase.Con;
        SQLiteCommand com;
        string str;


        public IRGQuerying()
        {
            InitializeComponent();
            SeedDatabase.seedData();
            selectFromIRGTable();
        }

        //cehcking the database 
        public bool CheckDataBaseIsEmpty()
        {
            // check if des and deg table exist and have data inside 

            if (selectFromIRGTable())

            {
                MessageBox.Show("there no data in Database ,please add New Excel sheet ");

                return true;
            }
            MessageBox.Show("there  data in Database , you can ovveride it with new Excel sheet");

            return false;


        }

        private bool selectFromIRGTable()
        {
            Con.Open();

            bool isExisted = false;

            str = "SELECT name FROM sqlite_master WHERE type='table' AND name='IRGDATA'";
            com = new SQLiteCommand(str, Con);
            SQLiteDataReader r = com.ExecuteReader();
            while (r.Read())
            {
                isExisted = !String.IsNullOrEmpty(Convert.ToString(r["name"]));
            }
            Con.Close();
            DataBaseIsEmpty = !isExisted;
            return isExisted;



        }




        private  void BrowseButton_Click(object sender, RoutedEventArgs e)
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

                dataRecords =  FillDatabase();

            }
        }

        private DataTable FillDatabase()
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


            return data; 

            
            
            
            
           
           
        }


        private async Task deleteAllRecords()
        {
            Con.Open();
            str = "delete  from IRGDATA";
            com = new SQLiteCommand(str, Con);
            await com.ExecuteNonQueryAsync();
            Con.Close();

        }
        private async Task createIRGDATA_table()
        {
            Con.Open();

          

            int count = 0;
            //com.ExecuteNonQuery();
            foreach (DataRow row in dataRecords.Rows)
            {
                count++;
                Application.Current.Dispatcher.Invoke(() => {
                   progrssbar.Value = count * 100 / dataRecords.Rows.Count; 
                });
               
                if (count >= 3)
                {
                    str = "insert into IRGDATA(CASMensuel_Soumis , CASIRG ,Mensuel_Soumis , Mensuel_NET, IRG )values(@CASMensuel_Soumis ,@CASIRG ,@Mensuel_Soumis ,@Mensuel_NET , @IRG )";
                    com = new SQLiteCommand(str, Con);
                    com.Parameters.AddWithValue("@Mensuel_Soumis", Double.Parse(row.ItemArray[0].ToString()));
                    com.Parameters.AddWithValue("@IRG", Double.Parse(row.ItemArray[1].ToString()));
                    com.Parameters.AddWithValue("@CASMensuel_Soumis", Double.Parse(row.ItemArray[2].ToString()));
                    com.Parameters.AddWithValue("@CASIRG", Double.Parse(row.ItemArray[3].ToString()));
                    com.Parameters.AddWithValue("@Mensuel_NET", Double.Parse(row.ItemArray[4].ToString()));
                   
                   await  com.ExecuteNonQueryAsync();

                }
            

            }


            

            MessageBox.Show("database has being filled with excel records");


            Con.Close();
            
        }

        private async void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            progrssbar.Visibility = Visibility.Visible;
            SaveBt.Visibility = Visibility.Collapsed;
            if (!DataBaseIsEmpty)
            {
                // check if the table existe delete it and new data 
                await deleteAllRecords();


                await Task.Run(() => createIRGDATA_table());



            }
            else
            {
                await Task.Run(() => createIRGDATA_table());

            }
            Application.Current.Dispatcher.Invoke(() => {
                progrssbar.Visibility = Visibility.Collapsed;
                SaveBt.Visibility = Visibility.Visible;
                
            });

            // else create data 
        }


        private List<string> queryingDatabase(String salaryNet)
        {
            Con.Open();

           
            
            List<string> list = new List<string>();
            str = "SELECT * FROM IRGDATA WHERE Mensuel_Soumis=@salaryNet";
            com = new SQLiteCommand(str, Con);
            com.Parameters.AddWithValue("@salaryNet", Double.Parse(salaryNet));
            SQLiteDataReader r = com.ExecuteReader();
            while (r.Read())
            {
               list.Add(Convert.ToString(r["IRG"]));
               list.Add(Convert.ToString(r["CASIRG"]));
            }
            Con.Close();
            return list; 
          
        }
        private void salaryNet_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return)
            {
                if (!string.IsNullOrEmpty(salaryNet.Text))
                {
                    var correctSalary = Math.Round(double.Parse(salaryNet.Text));
                    var correctSalary2 = Math.Round(correctSalary/10) * 10 ;
                    var listresult = queryingDatabase(correctSalary2.ToString());
                    // query database 
                    IRG.Text = Math.Round(double.Parse(listresult[0])).ToString(); 
                    CASIRG.Text = Math.Round(double.Parse(listresult[1])).ToString();
                }
            }
        }
    }
}
