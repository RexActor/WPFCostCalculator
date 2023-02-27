using System;
using System.Data;
using System.Data.OleDb;

namespace WPFCostCalculator
{
	class DatabaseClass
	{

		//readonly string ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source = engineeringDatabase.accdb; Jet OLEDB:Database Password = test";
		readonly string ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source = CostCalculator.accdb;";


		readonly OleDbConnection con = new OleDbConnection();


		//connect to database
		public void ConnectDatabase()
		{
			if (con.State == ConnectionState.Closed)
			{
				OleDbConnection.ReleaseObjectPool();
				con.ConnectionString = ConnectionString;
				con.Open();

			}
			else
			{
				OleDbConnection.ReleaseObjectPool();
			}

		}


		//close and dispose database connection
		public void CloseDatabase()
		{
			con.Dispose();
			con.Close();
		}


		//check database status
		public string DBStatus()
		{
			if (con != null)
			{

				if (con.State == ConnectionState.Open)
				{
					return "DB connected";
				}
				else if (con.State == ConnectionState.Closed)
				{
					return "DB not Connected";
				}
				else
				{
					return "Some kind of error";
				}
			}
			else
			{
				return "DB not Connected";
			}


		}


		

		/// <summary>
		/// Collecting data from selected database table. Without sorting options
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>


		public OleDbDataReader DBQuery(string table)
		{
			string queryString = $"SELECT * FROM {table}";
			OleDbCommand cmd = new OleDbCommand(queryString, con);
			OleDbDataReader reader = cmd.ExecuteReader();
			cmd.Dispose();
			return reader;
		}

		public OleDbDataReader DBQuery(string table,string filter,string value)
		{
			string queryString = $"SELECT * FROM {table} WHERE {filter}='{value}'";
			OleDbCommand cmd = new OleDbCommand(queryString, con);
			OleDbDataReader reader = cmd.ExecuteReader();
			cmd.Dispose();
			return reader;
		}
		public OleDbDataReader DBQuery(string table, string filter, string value,string filter2, string value2)
		{
			string queryString = $"SELECT * FROM {table} WHERE ({filter}='{value}') AND ({filter2}='{value2}')";
			OleDbCommand cmd = new OleDbCommand(queryString, con);
			OleDbDataReader reader = cmd.ExecuteReader();
			cmd.Dispose();
			return reader;
		}

		public OleDbDataReader GetScheme(string table)
		{
			string queryString = $"SELECT * FROM  INFORMATION_SHCEMA.COLUMNS WHERE TABLE_NAME=" + table + " ORDER BY ORDINAL_POSITION";
			OleDbCommand cmd = new OleDbCommand(queryString, con);
			OleDbDataReader reader = cmd.ExecuteReader();
			cmd.Dispose();
			return reader;

		}



		#region insert data into database

		public void RequestTrack(string table,string ProductCode,string ActivateDeactivate,string LineType,string Product,string PRDescription,int LineSpeed,string JobType,string JobCategory,string Spacing,int TargetPR,double HourlyRate,double BeltSpeed30PR,double BeltSpeed100PR,double BeltSpeedSelected,int STDCrew,int UnitsPerCase,int HandTiers,int HandTieTime,double STDBoxesPerHour,double ExpectedBoxesPerHour,double ExpectedBqtPerHour,double CostPerCase,double CostPerBqt,double STDMaxRate,string Status,string GroupCategory,int stemsPerBqt)
		{
			string queryString = "INSERT INTO " + table + "(ProductCode,ActivateDeactivate,LineType,Product,PRDescription,LineSpeed,JobType,JobCategory,Spacing,TargetPR,HourlyRate,BeltSpeed30PR,LineSpeed100PR,LineSpeedSelected,STDCrew,UnitsPerCase,HandTiers,HandTieTime,STDBoxesPerHour,ExpectedBoxesPerHour,ExpectedBqtPerHour,CostPerCase,CostPerBqt,StdMaxRate,Status,GroupCategory,StemsPerBqt) " + " Values(@ProductCode,@ActivateDeactivate,@LineType,@Product,@PRDescription,@LineSpeed,@JobType,@JobCategory,@Spacing,@TargetPR,@HourlyRate,@BeltSpeed30PR,@LineSpeed100PR,@LineSpeedSelected,@STDCrew,@UnitsPerCase,@HandTiers,@HandTieTime,@STDBoxesPerHour,@ExpectedBoxesPerHour,@ExpectedBqtPerHour,@CostPerCase,@CostPerBqt,@StdMaxRate,@Status,@GroupCategory,@StemsPerBqt)";
			
			OleDbCommand cmd = new OleDbCommand(queryString, con);





			cmd.Parameters.Add("@ProductCode", OleDbType.VarWChar).Value = ProductCode;
			cmd.Parameters.Add("@ActivateDeactivate", OleDbType.VarWChar).Value = ActivateDeactivate;
			cmd.Parameters.Add("@LineType", OleDbType.VarWChar).Value = LineType;
			cmd.Parameters.Add("@Product", OleDbType.VarWChar).Value = Product;
			cmd.Parameters.Add("@PRDescription", OleDbType.VarWChar).Value = PRDescription;
			cmd.Parameters.Add("@LineSpeed", OleDbType.Integer).Value = LineSpeed;
			cmd.Parameters.Add("@JobType", OleDbType.VarChar).Value = JobType;
			cmd.Parameters.Add("@JobCategory", OleDbType.VarChar).Value = JobCategory;
			cmd.Parameters.Add("@Spacing", OleDbType.VarWChar).Value = Spacing;
			cmd.Parameters.Add("@TargetPR", OleDbType.Integer).Value = TargetPR;
			cmd.Parameters.Add("@HourlyRate", OleDbType.Double).Value = HourlyRate;
			cmd.Parameters.Add("@BeltSpeed30PR", OleDbType.Double).Value = BeltSpeed30PR;
			cmd.Parameters.Add("@LineSpeed100PR", OleDbType.Double).Value = BeltSpeed100PR;
			cmd.Parameters.Add("@LineSpeedSelected", OleDbType.Double).Value = BeltSpeedSelected;
			cmd.Parameters.Add("@STDCrew", OleDbType.Integer).Value = STDCrew;

			cmd.Parameters.Add("@UnitsPerCase", OleDbType.Integer).Value = UnitsPerCase;
			cmd.Parameters.Add("@HandTiers", OleDbType.Integer).Value = HandTiers;

			cmd.Parameters.Add("@HandTieTime", OleDbType.Integer).Value = HandTieTime;
			cmd.Parameters.Add("@STDBoxesPerHou", OleDbType.Double).Value = STDBoxesPerHour;
			cmd.Parameters.Add("@ExpectedBoxesPerHour", OleDbType.Double).Value = ExpectedBoxesPerHour;
			cmd.Parameters.Add("@ExpectedBqtPerHour", OleDbType.Double).Value = ExpectedBqtPerHour;
			cmd.Parameters.Add("@CostPerCase", OleDbType.Double).Value = CostPerCase;
			cmd.Parameters.Add("@CostPerBqt", OleDbType.Double).Value = CostPerBqt;
			cmd.Parameters.Add("@StdMaxRate", OleDbType.Double).Value = STDMaxRate;
			cmd.Parameters.Add("@Status", OleDbType.VarChar).Value = Status;
			cmd.Parameters.Add("@GroupCategory", OleDbType.VarChar).Value = GroupCategory;
			cmd.Parameters.Add("@StemsPerBqt", OleDbType.Integer).Value = stemsPerBqt;
		



			cmd.ExecuteNonQuery();
		}

		#endregion

	}
}
