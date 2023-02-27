namespace WPFCostCalculator
{/// <summary>
 /// setting up track request class for data manipulation if required
 /// </summary>
	class TrackRequest
	{

#pragma warning disable IDE1006 // Naming Styles
		public int id { get; set; }
		public string spacing { get; set; }
		public int handtiecrew { get; set; }
		public int handtiespeed { get; set; }

		//seting up data for importing into database

		public string productCode { get; set; }

		public string activateDeactivate { get; set; }
		
		public string lineType { get; set; }
		public string productDescription { get; set; }
		public string prophetDescription { get; set; }
		public int selectedSpeed { get; set; }
		public string jobType { get; set; }
		public string jobCategory { get; set; }
		public string groupCategory { get; set; }
		public int stemsPerBqt { get; set; }

		public int targetPR { get; set; }
		public double hourlyRate { get; set; }
		public double linespeed30Pr { get; set; }
		public double linespeed100pr { get; set; }
		public double linespeedselected { get; set; }
		public int stdcrew { get; set; }
		public int unitspercase { get; set; }

		public double stdboxesperhour { get; set; }
		public double expectedboxesperhour { get; set; }
		public double expectedbqtperhour { get; set; }
		public double costpercase { get; set; }

		public double costperbqt { get; set; }

		public double stdmaxrate { get; set; }
#pragma warning restore IDE1006 // Naming Styles





	}
}
