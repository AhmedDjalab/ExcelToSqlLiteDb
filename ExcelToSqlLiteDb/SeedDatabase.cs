using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExcelToSqlLiteDb
{
	public static class SeedDatabase
	{


		public static string str = @"CREATE TABLE [IRGDATA] (
	[id]	INTEGER NOT NULL,
	[CASMensuel_Soumis]    NUMERIC,
	[CASIRG] NUMERIC,
	[Mensuel_Soumis] NUMERIC,
	[Mensuel_NET]    NUMERIC,
	[IRG]   NUMERIC,
	PRIMARY KEY([id] AUTOINCREMENT)
);
";


		public static SQLiteConnection Con = new SQLiteConnection("Data Source=" + Const.DATABASENAME+"; Version=3;");
		public static SQLiteCommand com;
		public static bool IsOpen = Con.State == ConnectionState.Open;


		public static void seedData()
		{
			// create database if it's not exist yet 

			// so if the database is not  exist , so we  need to create the tables 
			// again 
			if (!File.Exists(Const.DATABASENAME))
			{
				
				Con.Open();
				
                try
                {
				
                 SQLiteCommand command3 = new SQLiteCommand(str, Con);
				command3.ExecuteNonQuery();
				MessageBox.Show("Dataabse has created With Tables ");
                }
                catch (Exception e )
                {
					MessageBox.Show("Erros ::" , e.Message);

                } finally
                {
					Con.Close();
				}
				
				
				
			
			}

		}



	} 
}
	