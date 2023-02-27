namespace WPFCostCalculator
{
	public static class Constants
	{
		//spacings for Line Pack job Type
		public const double EveryWidegap = 36; //38 original

		public const double EveryOtherWideOrNarrow = 76;
		public const double EveryGapNarrowAndWide = 19;

		//TODO:need to double check!?
		public const double SlowMissThreeGapsNarrowAndWide = 76;

		public const double ExtraSlowMissTwoWideGaps = 114;

		//TODO:probably not required ExtraExtraSlowMissTwoWideGaps = 114;
		public const double ExtraExtraSlowMissTwoWideGaps = 114;

		public const double BQTIn3WideThenMissOne = 152;//152 //76
		public const double BQTInWideHoleX5ThenMissOne = 114; //228
		public const double BQTIn2WideThenMissOne = 57;//114

		//this is  +/- matching cost calculator
		public const double BQTInEveryOtherWide = 76; //76

		//spacings for Hand-Finish job type
		/// <summary>
		///  total bunch count needs to be divided on 3 to get 1 case
		/// </summary>
		public const double HandFinishInThirdsMissOneGapNarrowOrWide = 76;

		/// <summary>
		/// Total bunch count needs to be divided on 3 to get 1 case
		/// </summary>
		public const double HandFinishInThirdsMissTwoGapsNarrowAndWide = 95;

		/// <summary>
		/// Total Bunch count needs to be divided on 3 to get 1 case
		/// </summary>
		public const double HandFinishInThirdsMissThreeGapsNarrowAndWide = 114;

		//TODO:need to implement below spacings - but how to?!
		public const double BQTInEveryWideGap = 1;

		//spacing for MecaFlora
		public const double MecafloraEveryBCD = 1;

		public const double MecafloraEveryOtherBCD = 1;

		//packed offline?!

		public const double PackedOffline = 1;
	}

	internal class SpacingClass
	{
	}
}