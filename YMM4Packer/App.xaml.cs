using System.Text;
using System.Windows;

namespace YMM4Packer {

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {

		public App() {
			Encoding.RegisterProvider( CodePagesEncodingProvider.Instance );
		}
	}
}