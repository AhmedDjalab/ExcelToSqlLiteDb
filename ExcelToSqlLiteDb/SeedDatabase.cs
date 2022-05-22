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


		public static string str = @"CREATE TABLE [company] (
	[id] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
	[title] text, 
	[nbank] text
)

;
CREATE TABLE[degree] (

   [title] text,

   [id] integer NOT NULL PRIMARY KEY AUTOINCREMENT
)
;

CREATE TABLE[description] (

   [id] integer NOT NULL PRIMARY KEY AUTOINCREMENT,

   [title] text
)
;

CREATE TABLE[degreeDescription] (

   [id] integer NOT NULL PRIMARY KEY AUTOINCREMENT,

   [degreeId] integer NOT NULL, 
	[descriptionId] integer NOT NULL, 
	[amount] double, 
	FOREIGN KEY([degreeId])
		REFERENCES[degree] ([id])
	   ON UPDATE NO ACTION ON DELETE CASCADE,
   FOREIGN KEY([descriptionId])
		REFERENCES[description] ([id])
	   ON UPDATE NO ACTION ON DELETE CASCADE
)
;

CREATE TABLE[images] (

   [id_img] integer NOT NULL PRIMARY KEY AUTOINCREMENT,

   [imge] blob, 
	[tag] text, 
	[date_imge] integer, 
	[type]
		nvarchar(254)
);


CREATE TABLE[info_emp] (

   [id] integer NOT NULL PRIMARY KEY AUTOINCREMENT,

   [certfica] text, 
	[post] text, 
	[ccp_count] integer, 
	[nb_social] integer, 
	[degre] integer, 
	[class] integer, 
	[date_entre] integer, 
	[date_install] integer
)
;

CREATE TABLE[employee] (

   [id] integer NOT NULL PRIMARY KEY AUTOINCREMENT,

   [name] text, 
	[prenom] text, 
	[date_naiss] datetime, 
	[place_naiss] text, 
	[gender] text, 
	[stat] text, 
	[nb_child] integer, 
	[age] integer, 
	[address] text, 
	[nb_mobile] integer, 
	[companyId] integer NOT NULL, 
	[imageId] integer, 
	[degDescId] integer NOT NULL, 
	[infoEmpId] integer NOT NULL, 
	FOREIGN KEY([infoEmpId])
		REFERENCES[info_emp] ([id])
	   ON UPDATE NO ACTION ON DELETE CASCADE,
   FOREIGN KEY([degDescId])
		REFERENCES[degreeDescription] ([id])
	   ON UPDATE NO ACTION ON DELETE CASCADE,
   FOREIGN KEY([imageId])
		REFERENCES[images] ([id_img])
	   ON UPDATE NO ACTION ON DELETE CASCADE
)
;

CREATE TABLE[mois] (

   [id_mois] integer NOT NULL PRIMARY KEY AUTOINCREMENT,

   [ref] integer, 
	[mois] text
)
;

CREATE TABLE[salaire] (

   [id_sal] integer NOT NULL PRIMARY KEY AUTOINCREMENT,

   [sall_base] integer, 
	[experince] integer, 
	[time175] integer, 
	[time200] integer, 
	[recupration] integer, 
	[zone] integer, 
	[rendement] integer, 
	[sall_post] integer, 
	[cnas] integer, 
	[stand] integer, 
	[txavec_cnas] integer, 
	[irg] integer, 
	[B_salaire] integer, 
	[ordinaires] integer, 
	[hebergement] integer, 
	[trnsport] integer, 
	[scolaire] integer, 
	[sal_net] integer, 
	[empId] integer, 
	[degDesId] integer, 
	[moisId] integer, 
	FOREIGN KEY([moisId])
		REFERENCES[mois] ([id_mois])
	   ON UPDATE NO ACTION ON DELETE CASCADE,
   FOREIGN KEY([empId])
		REFERENCES[employee] ([id])
	   ON UPDATE NO ACTION ON DELETE CASCADE,
   FOREIGN KEY([degDesId])
		REFERENCES[degreeDescription] ([id])
	   ON UPDATE NO ACTION ON DELETE CASCADE
)";


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
	