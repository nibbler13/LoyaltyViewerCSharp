using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyViewerWpf {
	public class ItemPromoJustNow {
		public Dictionary<string, ItemDepartment> Departments { get; set; }

		public ItemPromoJustNow() {
			Departments = new Dictionary<string, ItemDepartment>();
		}
    }

	public class ItemDepartment {
		public string Name { get; set; }
		public List<ItemDoctor> Doctors { get; set; }

		public ItemDepartment(string Name) {
			this.Name = Name;
			Doctors = new List<ItemDoctor>();
		}
	}

	public class ItemDoctor {
		public string Name { get; set; }
		public List<ItemFreeCell> FreeCells { get; set; }

		public ItemDoctor(string Name) {
			this.Name = Name;
			FreeCells = new List<ItemFreeCell>();
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
