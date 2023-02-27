using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace WPFCostCalculator
{
	/// <summary>
	/// Interaction logic for CostCalculator.xaml
	/// </summary>
	public partial class CostCalculator : Window
	{
		//readonly SpacingClass spacingClass = new SpacingClass();
		private int LineSpeedDefaultValue;

		public int TargetPRDefaultValue;
		public double HourlyRateDefaultValue;
		private double LineSpeedSelectedAtX;
		private double StdBoxesPerHour = 0;
		private double expectedBoxesPerHour;
		private double expectedBqtPerHour;
		private double CostPerCase;
		private double CostPerBqt;
		private double STDMaxRate;
		private double selectedSpacing = 0;
		private bool calculatedOnce = false;
		private int numberCheck;
		private List<string> stringListbox;

		public double LineSpeed100PR;
		public double LineSpeed30PR;

		private readonly DatabaseClass db = new DatabaseClass();

		public bool jobTypeSelected = false;
		public bool spacingSelected = false;
		public bool activatedeactivateselected = false;
		public bool defaultvalue = true;
		public ToolTip _tooltip;
		//	bool calculationAvailable = false;

		public CostCalculator()
		{
			InitializeComponent();
			//ReadConfigFile("JobType.txt", JobTypeComboBox);
			//ReadConfigFile("JobCategory.txt", JobCategoryComboBox);
			//ReadConfigFile("Spacing.txt", null, SpacingListBox);
			//ReadConfigFile("LineSpeed30.txt", LineSpeed30PR);

			BuildingSelection.Items.Add("Building 1");
			BuildingSelection.Items.Add("Building 2");
			_tooltip = new ToolTip();
			//LineSelection

			//ReadConfigFile("LineSpeed100.txt", LineSpeed100PR);
			getJobCategory("JobCategory.xml");
			getSpacing("Spacing.xml");
			getJobType("JobType.xml");
			HourlyRateDefaultValue = Convert.ToDouble(HourlyRateTextBox.Text);
			ActivateDeactivateComboBoxGeneration();

			LineSpeedSelected.Text = "0";
			//MessageBox.Show(LineSpeed30PR.SelectedItem.ToString());
		}

		private void getPhysicalLine(string fileName, string building)
		{
			string cwd = Directory.GetCurrentDirectory();

			string filePath = Path.Combine(cwd, "Line.xml");
			// string filePath = string.Empty;
			if (LineSelection.Items.Count > 0)
			{
				LineSelection.Items.Clear();
			}

			try
			{
				var xml = XElement.Load(filePath);
				IEnumerable<XElement> query = from c in xml.Descendants("Line")
											  where c.Element("Building").Value == building
											  select c;

				foreach (var item in query)
				{
					//Console.WriteLine();

					LineSelection.Items.Add($"{item.Element("LineID").Value}");
				}
				//JobTypeComboBox.SelectedIndex = 0;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex}");
			}
		}

		private void getJobType(string fileName)
		{
			string cwd = Directory.GetCurrentDirectory();

			string filePath = Path.Combine(cwd, fileName);
			// string filePath = string.Empty;
			try
			{
				var xml = XElement.Load(filePath);
				IEnumerable<XElement> query = from c in xml.Descendants("JobType")
											  select c;
				JobTypeComboBox.Items.Add("Please Select");

				foreach (var item in query)
				{
					JobTypeComboBox.Items.Add($"{item.Element("Name").Value}");
				}
				JobTypeComboBox.SelectedIndex = 0;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex}");
			}
		}

		private void getJobCategory(string fileName)
		{
			string cwd = Directory.GetCurrentDirectory();

			string filePath = Path.Combine(cwd, fileName);
			// string filePath = string.Empty;
			try
			{
				var xml = XElement.Load(filePath);
				IEnumerable<XElement> query = from c in xml.Descendants("JobCategory")
											  select c;
				JobCategoryComboBox.Items.Add("Please Select");

				foreach (var item in query)
				{
					JobCategoryComboBox.Items.Add($"{item.Element("Name").Value}");
				}
				JobCategoryComboBox.SelectedIndex = 0;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex}");
			}
		}

		private void getSpacing(string fileName)
		{
			string cwd = Directory.GetCurrentDirectory();

			string filePath = Path.Combine(cwd, fileName);
			// string filePath = string.Empty;
			try
			{
				var xml = XElement.Load(filePath);
				IEnumerable<XElement> query = from c in xml.Descendants("Spacing")
											  select c;

				int i = 0;
				foreach (var item in query)
				{
					SpacingListBox.Items.Add($"{item.Element("Name").Value}");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Can't find file with name: {fileName} in {filePath}");
			}
		}

		private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			LineSpeedSelectedAtX = 0;
			LineSpeedDefaultValue = Convert.ToInt32(SpeedSlider.Value);
			if (SelectedSpeedTextBox != null)
			{
				if (SelectedSpeedTextBox.IsReadOnly != true)
				{
					SelectedSpeedTextBox.IsReadOnly = true;
					SelectedSpeedTextBox.BorderBrush = Brushes.Black;
					SelectedSpeedTextBox.Background = Brushes.White;
				}
				SelectedSpeedLabel.Content = $"Belt Speed @{Convert.ToInt32(SpeedSlider.Value)}";

				double result = (Convert.ToDouble(LineSpeed100PR) - Convert.ToDouble(LineSpeed30PR)) / 70;

				LineSpeedSelectedAtX = Convert.ToDouble(LineSpeed30PR) + (result * (Convert.ToDouble(SpeedSlider.Value) - 30));

				LineSpeedSelected.Text = LineSpeedSelectedAtX.ToString("F1");
			}

			if (calculatedOnce == true)
			{
				Calculate();
			}
		}

		private void SelectedSpeedTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			//MessageBox.Show("Clicked");
			if (SelectedSpeedTextBox.IsReadOnly == true)
			{
				SelectedSpeedTextBox.IsReadOnly = false;
				SelectedSpeedTextBox.BorderBrush = Brushes.Green;
				SelectedSpeedTextBox.Background = Brushes.LightGreen;
			}
			else
			{
				SelectedSpeedTextBox.IsReadOnly = true;
				SelectedSpeedTextBox.BorderBrush = Brushes.Black;
				SelectedSpeedTextBox.Background = Brushes.White;
			}
		}

		private void SelectedSpeedTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			//int defaultValue = Convert.ToInt32(SelectedSpeedTextBox.Text);

			if (SelectedSpeedTextBox.IsReadOnly != true && e.Key == Key.Enter)
			{
				if (Convert.ToInt32(SelectedSpeedTextBox.Text) < 30 || Convert.ToInt32(SelectedSpeedTextBox.Text) > 100)
				{
					SelectedSpeedTextBox.Text = LineSpeedDefaultValue.ToString();
					//MessageBox.Show(defaultValue.ToString());
				}
				else
				{
					SpeedSlider.Value = Convert.ToInt32(SelectedSpeedTextBox.Text);
				}
			}
			else
			{
				return;
			}
		}

		//TODO:Change to read XML File for each Line Config
		private void ReadConfigFile(string fileName, ComboBox comboBox = null, ListBox listBox = null)
		{
			string sourceDirectory = Directory.GetCurrentDirectory();

			string path = Path.Combine(sourceDirectory, fileName);

			//string[] files = Directory.GetFiles(sourceDirectory, fileName);
			if (!File.Exists(path))
			{
				MessageBox.Show($"File :.: {fileName} :.:  Not Found");
			}

			if (comboBox != null)
			{
				//MessageBox.Show(path);

				string[] data = File.ReadAllLines(path);
				if (fileName == "LineSpeed30.txt" || fileName == "LineSpeed100.txt")
				{
				}
				else
				{
					comboBox.Items.Add("Please Select");
				}

				foreach (var line in data)
				{
					comboBox.Items.Add(line);
				}
				comboBox.SelectedIndex = 0;
			}

			if (listBox != null)
			{
				//MessageBox.Show(path);

				string[] data = File.ReadAllLines(path);

				foreach (var line in data)
				{
					listBox.Items.Add(line);
				}
				//listBox.SelectedIndex = 0;
			}
		}

		private void Calculate()
		{
			if (spacingSelected)
			{
				switch (JobTypeComboBox.SelectedItem.ToString())
				{
					case "Hand-Tie":
						StdBoxesPerHour = (1 / Convert.ToDouble(HandTieSpeedTextBox.Text)) * 3600 * Convert.ToDouble(HandTieCrewTexBox.Text) * (1 / Convert.ToDouble(UnitsPerCaseTextBox.Text));

						expectedBoxesPerHour = StdBoxesPerHour * (Convert.ToDouble(TargetPRTextBox.Text) / 100);
						expectedBqtPerHour = expectedBoxesPerHour * (Convert.ToDouble(UnitsPerCaseTextBox.Text));
						CostPerBqt = (Convert.ToDouble(HourlyRateTextBox.Text) * Convert.ToDouble(STDCrewTextBox.Text)) / expectedBqtPerHour;
						CostPerCase = CostPerBqt * Convert.ToDouble(UnitsPerCaseTextBox.Text);
						STDMaxRate = StdBoxesPerHour / 60 * Convert.ToDouble(UnitsPerCaseTextBox.Text);

						break;

					case "Line Pack":

						//TODO: formula set up only to use one constant
						//StdBoxesPerHour = ((Convert.ToDouble(LineSpeedSelected.Text) / (selectedSpacing / 100)) / Convert.ToDouble(UnitsPerCaseTextBox.Text)) * 60;

						if (SpacingListBox.SelectedItem.ToString() == "Hand Finish in thirds, miss one gap (narrow or wide)" || SpacingListBox.SelectedItem.ToString() == "Hand Finish in thirds, miss two gaps (narrow and wide)" || SpacingListBox.SelectedItem.ToString() == "Hand Finish in thirds, miss three gaps (narrow and wide)")
						{
							StdBoxesPerHour = (((Convert.ToDouble(LineSpeedSelected.Text) / (selectedSpacing / 100)) / Convert.ToDouble(UnitsPerCaseTextBox.Text)) * 60);
						}
						else
						{
							StdBoxesPerHour = ((Convert.ToDouble(LineSpeedSelected.Text) / (selectedSpacing / 100)) / Convert.ToDouble(UnitsPerCaseTextBox.Text)) * 60;
						}

						expectedBoxesPerHour = StdBoxesPerHour * (Convert.ToDouble(TargetPRTextBox.Text) / 100);
						expectedBqtPerHour = expectedBoxesPerHour * (Convert.ToDouble(UnitsPerCaseTextBox.Text));
						CostPerBqt = (Convert.ToDouble(HourlyRateTextBox.Text) * Convert.ToDouble(STDCrewTextBox.Text)) / expectedBqtPerHour;
						CostPerCase = CostPerBqt * Convert.ToDouble(UnitsPerCaseTextBox.Text);
						STDMaxRate = StdBoxesPerHour / 60 * Convert.ToDouble(UnitsPerCaseTextBox.Text);

						break;
				}

				CalcStdBoxes.Content = StdBoxesPerHour.ToString("F3");
				CalcExpectedBoxes.Content = expectedBoxesPerHour.ToString("F3");
				CalcExpectedBqt.Content = expectedBqtPerHour.ToString("F3");
				CalcCostPerBqt.Content = CostPerBqt.ToString("F3");
				CalcCostPerCase.Content = CostPerCase.ToString("F3");
				CalcStdMaxRate.Content = STDMaxRate.ToString("F3");

				//enable track request button
				if (RequestTrack.IsEnabled == false)
				{
					RequestTrack.IsEnabled = true;
				}
			}
		}

		private void CalculateCost_Click(object sender, RoutedEventArgs e)
		{
			if (jobTypeSelected)
			{
				if (JobTypeComboBox.SelectedItem.ToString() == "Hand-Tie")
				{
					spacingSelected = true;
					Calculate();
					calculatedOnce = true;
					spacingSelected = false;
				}
				else
				{
					if (!spacingSelected)
					{
						MessageBox.Show("Please Select Spacing");
					}
					else
					{
						Calculate();
						calculatedOnce = true;
					}
				}
			}
			else
			{
				MessageBox.Show("Please Select Job Type");
			}
		}

		private void JobTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (RequestTrack.IsEnabled == true)
			{
				RequestTrack.IsEnabled = false;
			}

			if (JobTypeComboBox.SelectedIndex > 0)
			{
				jobTypeSelected = true;
			}
			if (JobTypeComboBox.SelectedItem.ToString() == "Hand-Tie")
			{
				HandTieCanvas.Visibility = Visibility.Visible;
				SpacingListBox.IsEnabled = false;
				SpeedSlider.IsEnabled = false;
				SelectedSpeedTextBox.IsEnabled = false;
			}
			else
			{
				HandTieCanvas.Visibility = Visibility.Hidden;
				SpacingListBox.IsEnabled = true;
				SpeedSlider.IsEnabled = true;
				SelectedSpeedTextBox.IsEnabled = true;
			}
		}

		private void TargetPRTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			TargetPRDefaultValue = Convert.ToInt32(TargetPRTextBox.Text);

			if (TargetPRTextBox.IsReadOnly != true && e.Key == Key.Enter)
			{
				/*
				double result = Convert.ToDouble(StdBoxesPerHour) * (Convert.ToDouble(TargetPRTextBox.Text)/100);

				CalcExpectedBoxes.Content = result.ToString("F3");
				*/
				Calculate();

				//ToDO:probably to remove this

				TargetPRTextBox.IsReadOnly = true;
				TargetPRTextBox.BorderBrush = Brushes.Black;
				TargetPRTextBox.Background = Brushes.White;
			}
			else
			{
				return;
			}
		}

		private void TargetPRTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			//MessageBox.Show("Clicked");
			if (TargetPRTextBox.IsReadOnly == true)
			{
				TargetPRTextBox.IsReadOnly = false;
				TargetPRTextBox.BorderBrush = Brushes.Green;
				TargetPRTextBox.Background = Brushes.LightGreen;
			}
			else
			{
				TargetPRTextBox.IsReadOnly = true;
				TargetPRTextBox.BorderBrush = Brushes.Black;
				TargetPRTextBox.Background = Brushes.White;
			}
		}

		private void HourlyRateTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (HourlyRateTextBox.IsReadOnly != true && e.Key == Key.Enter)
			{
				/*
				double result = Convert.ToDouble(StdBoxesPerHour) * (Convert.ToDouble(TargetPRTextBox.Text)/100);

				CalcExpectedBoxes.Content = result.ToString("F3");
				*/
				Calculate();

				//ToDO:probably to remove this

				HourlyRateTextBox.IsReadOnly = true;
				HourlyRateTextBox.BorderBrush = Brushes.Black;
				HourlyRateTextBox.Background = Brushes.White;
			}
			else
			{
				return;
			}
		}

		private void HourlyRateTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (HourlyRateTextBox.IsReadOnly == true)
			{
				HourlyRateTextBox.IsReadOnly = false;
				HourlyRateTextBox.BorderBrush = Brushes.Green;
				HourlyRateTextBox.Background = Brushes.LightGreen;
			}
			else
			{
				HourlyRateTextBox.IsReadOnly = true;
				HourlyRateTextBox.BorderBrush = Brushes.Black;
				HourlyRateTextBox.Background = Brushes.White;
			}
		}

		private void SpacingListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string cwd = Directory.GetCurrentDirectory();
			//private string SpacingFileXML = "";
			string filePath = Path.Combine(cwd, "Spacing.xml");

			if (SpacingListBox.SelectedItem != null)
			{
				spacingSelected = true;
			}
			try
			{
				var xml = XElement.Load(filePath);

				IEnumerable<XElement> query = from x in xml.Descendants("Spacing")
											  where x.Element("Name").Value == SpacingListBox.SelectedItem.ToString()
											  select x.Element("Distance");
				foreach (var item in query)
				{
					selectedSpacing = Convert.ToDouble(item.Value);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex}");
			}

			if (calculatedOnce == true && selectedSpacing != 0)
			{
				Calculate();
			}
		}

		public void ValueChecker(object obj)
		{
			//testing for dynamic object value checker

			string typeOfObj = obj.GetType().Name;

			switch (typeOfObj)
			{
				case "TextBox":

					TextBox _obj = (TextBox)obj;
					if (_obj.IsFocused)
					{
						bool succsessNumbeCheck = int.TryParse(_obj.Text, out numberCheck);

						if (succsessNumbeCheck)
						{
							numberCheck = Convert.ToInt32(_obj.Text);
							_obj.Text = numberCheck.ToString();
							_obj.BorderBrush = Brushes.Green;
							_obj.Background = Brushes.LightGreen;
							_obj.BorderThickness = new Thickness(2);
						}
						else
						{
							//ProductCodeTextBox.Text = numberCheck.ToString();
							string s = _obj.Text;

							if (s.Length > 0)
							{
								s = s.Substring(0, s.Length - 1);
							}
							_obj.Background = Brushes.PaleVioletRed;
							_obj.BorderThickness = new Thickness(2);

							_obj.Text = s;
							_obj.Select(s.Length, 0);
							//MessageBox.Show("Please Enter Number");
						}
					}

					break;

				case "ComboBox":

					ComboBox _obj2 = (ComboBox)obj;
					if (_obj2.SelectedItem != null)
					{
						if (_obj2.SelectedItem.ToString() == "Please Select")
						{
							//_obj2.Background = Brushes.PaleVioletRed;
							_obj2.Foreground = Brushes.Red;
						}
						else
						{
							///_obj2.Background = Brushes.LightGreen;
							_obj2.Foreground = Brushes.Green;
						}
					}
					break;

				default:
					break;
			}

			//bool succsessNumbeCheck = int.TryParse(ProductCodeTextBox.Text, out numberCheck);
		}

		private void Building_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			getPhysicalLine("Line.xml", BuildingSelection.SelectedItem.ToString());
		}

		private void getLineSpeed(string lineID)
		{
			string cwd = Directory.GetCurrentDirectory();

			string filePath = Path.Combine(cwd, "Line.xml");
			// string filePath = string.Empty;

			try
			{
				var xml = XElement.Load(filePath);
				IEnumerable<XElement> query = from c in xml.Descendants("Line")
											  where c.Element("LineID").Value == lineID
											  select c;

				foreach (var item in query)
				{
					//Console.WriteLine(item);
					LineSpeed100PR = Convert.ToDouble(item.Element("EndSpeed").Value);
					LineSpeed30PR = Convert.ToDouble(item.Element("StartSpeed").Value);
				}
				//JobTypeComboBox.SelectedIndex = 0;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex}");
			}
		}

		private void LineSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//Console.WriteLine(LineSelection.SelectedItem.ToString());
			if (LineSelection.SelectedItem != null)
			{
				getLineSpeed(LineSelection.SelectedItem.ToString());
			}

			double result = (Convert.ToDouble(LineSpeed100PR) - Convert.ToDouble(LineSpeed30PR)) / 70;

			LineSpeedSelectedAtX = Convert.ToDouble(LineSpeed30PR) + (result * (Convert.ToDouble(SpeedSlider.Value) - 30));

			LineSpeedSelected.Text = LineSpeedSelectedAtX.ToString("F2");
			if (calculatedOnce)
			{
				//MessageBox.Show("Hello");

				Calculate();
			}
		}

		private void RequestTrack_Click(object sender, RoutedEventArgs e)
		{
			TrackRequest track = new TrackRequest();

			try
			{
				track.spacing = "Not Required";
				track.handtiecrew = 0;
				track.handtiespeed = 0;

				//seting up data for importing into database

				track.productCode = ProductCodeTextBox.Text.ToString();
				track.activateDeactivate = ActivateDeactivateComboBox.SelectedItem.ToString();
				track.lineType = LineTypeTextBox.Text;
				track.productDescription = ProductDescriptionTextBox.Text.ToString();
				track.prophetDescription = ProphetProductDescription.Text.ToString();
				track.selectedSpeed = Convert.ToInt32(SelectedSpeedTextBox.Text);
				track.jobType = JobTypeComboBox.SelectedItem.ToString();
				track.jobCategory = JobTypeComboBox.SelectedItem.ToString();

				track.targetPR = Convert.ToInt32(TargetPRTextBox.Text);
				track.hourlyRate = Convert.ToDouble(HourlyRateTextBox.Text);
				track.linespeed30Pr = Convert.ToDouble(LineSpeed30PR);
				track.linespeed100pr = Convert.ToDouble(LineSpeed100PR);
				track.linespeedselected = Convert.ToDouble(LineSpeedSelected.Text);
				track.stdcrew = Convert.ToInt32(STDCrewTextBox.Text);
				track.unitspercase = Convert.ToInt32(UnitsPerCaseTextBox.Text);

				track.stdboxesperhour = Convert.ToDouble(StdBoxesPerHour.ToString("F3"));
				track.expectedboxesperhour = Convert.ToDouble(expectedBoxesPerHour.ToString("F3"));
				track.expectedbqtperhour = Convert.ToDouble(expectedBqtPerHour.ToString("F3"));
				track.costpercase = Convert.ToDouble(CostPerCase.ToString("F3"));
				track.costperbqt = Convert.ToDouble(CostPerBqt.ToString("F3"));
				track.stdmaxrate = Convert.ToDouble(STDMaxRate.ToString("F3"));
				track.groupCategory = "Flowers";
				track.stemsPerBqt = Convert.ToInt32(StemsPerBqt.Text);

				//amending values bases on jobtype selection
				if (JobTypeComboBox.SelectedItem.ToString() == "Hand-Tie")
				{
					track.spacing = "Not Required";
					track.handtiecrew = Convert.ToInt32(HandTieCrewTexBox.Text);
					track.handtiespeed = Convert.ToInt32(HandTieSpeedTextBox.Text);
				}
				else
				{
					track.spacing = SpacingListBox.SelectedItem.ToString();
					track.handtiecrew = 0;
					track.handtiespeed = 0;
				}
			}
			catch
			{
				MessageBox.Show("Something Went Wrong");
				return;
			}

			//connecting database
			db.ConnectDatabase();

			db.RequestTrack("OperationCostings",
				track.productCode,
				track.activateDeactivate,
				track.lineType,
				track.productDescription,
				track.prophetDescription,
				track.selectedSpeed,
				track.jobType,
				track.jobCategory,
				track.spacing,
				track.targetPR,
				track.hourlyRate,
				track.linespeed30Pr,
				track.linespeed100pr,
				track.linespeedselected,
				track.stdcrew,
				track.unitspercase,
				track.handtiecrew,
				track.handtiespeed,
				track.stdboxesperhour,
				track.expectedboxesperhour,
				track.expectedbqtperhour,
				track.costpercase,
				track.costperbqt,
				track.stdmaxrate,
				"Require Set Up", track.groupCategory, track.stemsPerBqt);

			//closing database
			db.CloseDatabase();
		}

		public void ActivateDeactivateComboBoxGeneration(string defaultvalue = "Yes")
		{
			if (defaultvalue == "Yes")
			{
				ActivateDeactivateComboBox.Items.Add("Please Select");
				ActivateDeactivateComboBox.SelectedIndex = 0;
			}
			ActivateDeactivateComboBox.Items.Add("Provisional");
			ActivateDeactivateComboBox.Items.Add("Activate");
			ActivateDeactivateComboBox.Items.Add("DeActivate");
		}

		private void ActivateDeactivateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int selectedIndex;
			ValueChecker(sender);

			if (defaultvalue == true)
			{
				if (ActivateDeactivateComboBox.SelectedIndex > 0)
				{
					selectedIndex = ActivateDeactivateComboBox.SelectedIndex;
					activatedeactivateselected = true;
					ActivateDeactivateComboBox.Items.Clear();
					ActivateDeactivateComboBoxGeneration("No");
					ActivateDeactivateComboBox.SelectedIndex = selectedIndex - 1;
					defaultvalue = false;
				}
			}
		}

		private void STDCrewTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ValueChecker(sender);
		}

		private void ProductCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ValueChecker(sender);
		}

		private void UnitsPerCaseTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ValueChecker(sender);
		}

		private void StemsPerBqt_TextChanged(object sender, TextChangedEventArgs e)
		{
			ValueChecker(sender);
		}

		private void JobCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//JobCategoryComboBox.SelectedIndex = 0;

			if (JobCategoryComboBox.SelectedItem.ToString() != "Please Select")
			{
				JobCategoryComboBox.Items.RemoveAt(0);
			}
		}

		private void SelectedSpeedLabel_MouseEnter(object sender, MouseEventArgs e)
		{
			_tooltip.Content = $"Line Speeds for Selected Line:\n" +
				$"Speeds:\r@30%: {LineSpeed30PR}" +
				$"\r@100% : {LineSpeed100PR}";
			_tooltip.IsOpen = true;
		}

		private void SelectedSpeedLabel_MouseLeave(object sender, MouseEventArgs e)
		{
			_tooltip.IsOpen = false;
		}
	}
}