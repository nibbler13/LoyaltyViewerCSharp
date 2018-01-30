using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyViewerWpf {
	public class ItemPromoJustNow {
		public DateTime DateTimeUpdated { get; set; }
		public SortedDictionary<string, ItemDepartment> Departments { get; set; }

		public ItemPromoJustNow() {
			Departments = new SortedDictionary<string, ItemDepartment>();
		}

		public override string ToString() {
			string result = string.Empty;
			if (DateTimeUpdated != null)
				result += "Updated: " + DateTimeUpdated.ToLongTimeString() + Environment.NewLine;
			foreach (KeyValuePair<string, ItemDepartment> department in Departments) {
				result += "---" + department.Key + Environment.NewLine;
				foreach (KeyValuePair<string, ItemDoctor> doctor in department.Value.Doctors) {
					result += "-" + doctor.Key + Environment.NewLine;
				}

			}
			return base.ToString() + Environment.NewLine + result;
		}
	}

	public class ItemDepartment {
		public string Name { get; set; }
		public SortedDictionary<string, ItemDoctor> Doctors { get; set; }

		public ItemDepartment(string Name) {
			this.Name = Name;
			Doctors = new SortedDictionary<string, ItemDoctor>();
		}
	}

	public class ItemDoctor {
		public string Name { get; set; }
		public SortedDictionary<DateTime, ItemFreeCell> FreeCells { get; set; }

		public ItemDoctor(string Name) {
			this.Name = Name;
			FreeCells = new SortedDictionary<DateTime, ItemFreeCell>();
		}
	}

	public class ItemFreeCell {
		public DateTime Begin { get; set; }
		public DateTime End { get; set; }
		public TimeSpan Duration { get; set; }

		public ItemFreeCell(DateTime Begin, DateTime End) {
			this.Begin = Begin;
			this.End = End;
			Duration = End - Begin;
		}
	}
}
