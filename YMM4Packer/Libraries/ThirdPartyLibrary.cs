using System;

namespace Libraries {

	internal class ThirdPartyLibrary {

		public ThirdPartyLibrary() {
		}

		public ThirdPartyLibrary( string name ) {
			this.Name = name;
		}

		public string Name { get; set; }
		public Uri Url { get; set; }

		public string License { get; set; }
		public string LicenseFile { get; set; }
		public Uri LicenseUrl { get; set; }

		public string Copyright { get; set; }
	}
}