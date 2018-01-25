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
