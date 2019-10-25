namespace LoyaltyViewerWpf {
	public class ItemDataResult {
		public int Total { get; set; } = 0;
		public int PercentLove { get; set; } = 0;
		public int PercentHappy { get; set; } = 0;
		public int PercentNeutral { get; set; } = 0;
		public int PercentSad { get; set; } = 0;
		public int PercentAngry { get; set; } = 0;
		public int PercentLike { get; set; } = 0;
		public int PercentDontKnow { get; set; } = 0;
		public int PercentDislike { get; set; } = 0;
		public string Description { get; set; } = string.Empty;
	}
}
