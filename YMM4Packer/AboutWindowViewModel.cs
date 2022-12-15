using Libraries;
using LivetEx;
using System;
using System.Collections.Generic;

namespace YMM4Packer {
	internal class AboutWindowViewModel : ViewModel {

		public void Initialize() {
		}

		public ProductInfo ProductInfo => ProductInfo.ExecutingAssembly;

		public IReadOnlyList<ThirdPartyLibrary> Libraries {
			get {
				return new List<ThirdPartyLibrary>() {
					new ThirdPartyLibrary( "Livet" ){
						Url = new Uri("https://github.com/ugaya40/Livet"),
						Copyright = "© 2010-2011 ugaya40",
						License = "zlib/libpng",
						LicenseUrl = new Uri( "https://github.com/ugaya40/Livet/blob/master/license-jp.txt" ),
					},

					new ThirdPartyLibrary( "ReactiveProperty" ){
						Url = new Uri("https://github.com/runceel/ReactiveProperty"),
						Copyright = "© 2018 neuecc, xin9le, okazuki",
						License = "MIT License",
						LicenseUrl = new Uri( "https://github.com/runceel/ReactiveProperty/blob/master/LICENSE.txt" ),
					},

					new ThirdPartyLibrary( "Json.NET" ){
						Url = new Uri("https://www.newtonsoft.com/json"),
						Copyright = "Copyright (c) 2007 James Newton-King",
						License = "MIT License",
						LicenseUrl = new Uri( "https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md" ),
					},
				};
			}
		}
	}
}