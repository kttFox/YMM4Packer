using Libraries;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace YMM4Packer {

	/// <summary>
	/// AboutWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class AboutWindow : Window {

		public AboutWindow() {
			InitializeComponent();
		}

		private void Hyperlink_RequestNavigate( object sender, System.Windows.Navigation.RequestNavigateEventArgs e ) {
			Process.Start( e.Uri.AbsoluteUri );
			e.Handled = true;
		}

		private void Hyperlink_Click( object sender, RoutedEventArgs e ) {
			var s = (Hyperlink)sender;

			var item = (ThirdPartyLibrary)s.DataContext;
			if( File.Exists( item.LicenseFile ) ) {
				Process.Start( Path.GetFullPath( item.LicenseFile ) );
			} else if( item.LicenseUrl != null ) {
				Process.Start( item.LicenseUrl.AbsoluteUri );
			}
		}
	}
}