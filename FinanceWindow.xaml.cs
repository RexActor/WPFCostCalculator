using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPFCostCalculator
{

	/// <summary>
	/// Interaction logic for FinanceWindow.xaml
	/// </summary>
	public partial class FinanceWindow : Window
	{


		/// <summary>
		/// Required Information for Finance is
		/// Product Number
		/// Product Name
		/// Stems Per Bqt
		/// Job Category
		/// Crew
		/// Max Output @100%
		/// </summary>

		readonly DatabaseClass db = new DatabaseClass();
		TrackRequest track;


		public FinanceWindow()
		{
			InitializeComponent();

			CreateGroupComboBox();
			CreateComboBox();

		}


		public void PopulateTrackList(string filter, string value, string filter2 = null, string value2 = null)
		{
			db.ConnectDatabase();
			TrackList.Items.Clear();

			var databaseData = db.DBQuery("OperationCostings", filter, value, filter2, value2);



			while (databaseData.Read())
			{
				track = new TrackRequest
				{
					id = Convert.ToInt32(databaseData["ID"].ToString()),
					productCode = databaseData["ProductCode"].ToString(),
					activateDeactivate = databaseData["ActivateDeactivate"].ToString(),
					lineType = databaseData["LineType"].ToString(),
					productDescription = databaseData["Product"].ToString(),
					prophetDescription = databaseData["PRDescription"].ToString(),
					selectedSpeed = Convert.ToInt32(databaseData["LineSpeed"].ToString()),
					jobType = databaseData["JobType"].ToString(),
					jobCategory = databaseData["JobCategory"].ToString(),
					spacing = databaseData["Spacing"].ToString(),
					targetPR = Convert.ToInt32(databaseData["TargetPR"].ToString()),
					hourlyRate = Convert.ToDouble(databaseData["HourlyRate"].ToString()),
					linespeed30Pr = Convert.ToDouble(databaseData["BeltSpeed30PR"].ToString()),
					linespeed100pr = Convert.ToDouble(databaseData["LineSpeed100PR"].ToString()),
					linespeedselected = Convert.ToDouble(databaseData["LineSpeedSelected"].ToString()),
					stdcrew = Convert.ToInt32(databaseData["STDCrew"].ToString()),
					unitspercase = Convert.ToInt32(databaseData["UnitsPerCase"].ToString()),
					handtiecrew = Convert.ToInt32(databaseData["HandTiers"].ToString()),
					handtiespeed = Convert.ToInt32(databaseData["HandTieTime"].ToString()),
					stdboxesperhour = Convert.ToDouble(databaseData["STDBoxesPerHour"].ToString()),
					expectedboxesperhour = Convert.ToDouble(databaseData["ExpectedBoxesPerHour"].ToString()),
					expectedbqtperhour = Convert.ToDouble(databaseData["ExpectedBqtPerHour"].ToString()),
					costpercase = Convert.ToDouble(databaseData["CostPerCase"].ToString()),
					costperbqt = Convert.ToDouble(databaseData["CostPerBqt"].ToString()),
					stdmaxrate = Convert.ToDouble(databaseData["StdMaxRate"].ToString()),
					stemsPerBqt = Convert.ToInt32(databaseData["StemsPerBqt"].ToString())
					
				};



				TrackList.Items.Add(track);

			}



			db.CloseDatabase();
		}


		public void CreateComboBox()
		{
			ActivateDeactivateComboBox.Items.Add("Activate");
			ActivateDeactivateComboBox.Items.Add("DeActivate");
			ActivateDeactivateComboBox.SelectedIndex = 0;
		}

		public void CreateGroupComboBox()
		{
			GroupComboBox.Items.Add("Flowers");
			GroupComboBox.Items.Add("Plants");
			GroupComboBox.SelectedIndex = 0;
		}

		private void ActivateDeactivateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (GroupComboBox.SelectedItem != null && ActivateDeactivateComboBox.SelectedItem!=null)
			{
				
				PopulateTrackList("ActivateDeactivate", ActivateDeactivateComboBox.SelectedItem.ToString(), "GroupCategory", GroupComboBox.SelectedItem.ToString());
			}
		}

		private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (GroupComboBox.SelectedItem != null && ActivateDeactivateComboBox.SelectedItem != null)
			{

				PopulateTrackList("ActivateDeactivate", ActivateDeactivateComboBox.SelectedItem.ToString(), "GroupCategory", GroupComboBox.SelectedItem.ToString());
			}
		}
	}



	//option to hide columns from listview
	/// <summary>
	/// usage local:GridViewColumnVisibilityManager.IsVisible="False" <-- add into XAML GridViewColumn
	/// </summary>
	public class GridViewColumnVisibilityManager
	{
		readonly static Dictionary<GridViewColumn, double> originalColumnWidths = new Dictionary<GridViewColumn, double>();

		public static bool GetIsVisible(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsVisibleProperty);
		}

		public static void SetIsVisible(DependencyObject obj, bool value)
		{
			obj.SetValue(IsVisibleProperty, value);
		}

		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.RegisterAttached("IsVisible", typeof(bool), typeof(GridViewColumnVisibilityManager), new UIPropertyMetadata(true, OnIsVisibleChanged));

		private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GridViewColumn gc = d as GridViewColumn;
			if (gc == null)
				return;

			if (GetIsVisible(gc) == false)
			{
				originalColumnWidths[gc] = gc.Width;
				gc.Width = 0;
			}
			else
			{
				if (gc.Width == 0)
					gc.Width = originalColumnWidths[gc];
			}
		}
	}
}
