namespace LoyaltyViewerWpf {
	public class ItemDataResult {
		public int Total { get; set; }
		public double PercentLove { get; set; }
		public double PercentHappy { get; set; }
		public double PercentNeutral { get; set; }
		public double PercentSad { get; set; }
		public double PercentAngry { get; set; }
		public double PercentLike { get; set; }
		public double PercentDontKnow { get; set; }
		public double PercentDislike { get; set; }
		public string Description { get; set; }

		public ItemDataResult() {
			Total = 0;
			PercentLove = 0;
			PercentHappy = 0;
			PercentNeutral = 0;
			PercentSad = 0;
			PercentAngry = 0;
			PercentLike = 0;
			PercentDontKnow = 0;
			PercentDislike = 0;
			Description = string.Empty;
		}
	}
}
