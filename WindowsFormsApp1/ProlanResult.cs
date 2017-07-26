namespace LoyaltyViewer {
	public class ProlanResult {
		public enum Types {Quality, Recommendation};

		public int total = 0;
		public int percentLove = 0;
		public int percentHappy = 0;
		public int percentNeutral = 0;
		public int percentSad = 0;
		public int percentAngry = 0;
		public int percentLike = 0;
		public int percentDontKnow = 0;
		public int percentDislike = 0;

		public string title = "";

		public ProlanResult() {

		}
	}
}
