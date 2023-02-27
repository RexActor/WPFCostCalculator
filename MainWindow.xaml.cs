using WPFCostCalculator;
using System.Windows;

namespace WPFCostCalculator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

	
		public MainWindow()
		{
			InitializeComponent();
		}


		private void CostCalculatorButton_Click(object sender, RoutedEventArgs e)
		{
			CostCalculator costCalc = new CostCalculator();
			costCalc.Show();
		}

		private void FinanceWindow_Click(object sender, RoutedEventArgs e)
		{

			FinanceWindow financeWindow = new FinanceWindow();
			financeWindow.Show();

		}

		private void CostCalculatorButtonPlants_Click(object sender, RoutedEventArgs e)
		{
			CostCalculatorPlants costCalcPlants = new CostCalculatorPlants();
			costCalcPlants.Show();
		}
	}
}
