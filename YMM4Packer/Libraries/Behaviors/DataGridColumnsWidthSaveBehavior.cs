using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Libraries.Behaviors {

	/// <summary>
	/// DataGridのColumnWidthを保存、復元します。
	/// </summary>
	public class DataGridColumnsWidthSaveBehavior : BehaviorBase<DataGrid> {
		private ColumnsWidthSettings ColumnsWidthSettings { get; set; }

		/// <summary>
		/// アプリケーション設定グループの設定キーを取得または設定します。
		/// </summary>
		public string SettingsKey { get; set; }

		private Window window;

		protected override void OnLoaded() {
			if( this.ColumnsWidthSettings == null ) {
				this.ColumnsWidthSettings = new ColumnsWidthSettings( SettingsKey );
			}

			if( this.ColumnsWidthSettings.IsUpgrade != true ) {
				this.ColumnsWidthSettings.Upgrade();
			}

			if( window == null ) {
				window = Window.GetWindow( this.AssociatedObject );
			}
			window.Closing += window_Closing;

			if( Keyboard.Modifiers.HasFlag( ModifierKeys.Shift ) ) {
				return;
			}

			var ColumnsWidth = this.ColumnsWidthSettings.ColumnsWidth;

			if( ColumnsWidth != null && ColumnsWidth.Length == this.AssociatedObject.Columns.Count ) {
				// 各Columnの幅を復元する。
				foreach( var (item, index) in this.AssociatedObject.Columns.Select( ( x, index ) => (x, index) ) ) {
					item.Width = ColumnsWidth[index];
				}
			}
		}

		private void window_Closing( object sender, CancelEventArgs e ) {
			if( !e.Cancel ) {
				Save();
			}
		}

		protected override void OnUnloaded() {
			Save();

			window.Closing -= window_Closing;
		}

		/// <summary>
		/// Column の Width を保存する
		/// </summary>
		private void Save() {
			this.ColumnsWidthSettings.ColumnsWidth = this.AssociatedObject.Columns.Select( x => x.Width.DisplayValue ).ToArray();
			this.ColumnsWidthSettings.IsUpgrade = true;
			this.ColumnsWidthSettings.Save();
		}
	}

	/// <summary>
	/// AppData\Local の user.config に保存する
	/// </summary>
	public class ColumnsWidthSettings : ApplicationSettingsBase {

		public ColumnsWidthSettings( string settingsKey ) : base( settingsKey ) {
		}

		[UserScopedSetting]
		public double[] ColumnsWidth {
			get { return (double[])this["ColumnsWidth"]; }
			set { this["ColumnsWidth"] = value; }
		}

		[UserScopedSetting]
		public bool? IsUpgrade {
			get { return (bool?)this["IsUpgrade"]; }
			set { this["IsUpgrade"] = value; }
		}
	}
}