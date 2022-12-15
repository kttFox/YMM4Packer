using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Libraries.Behaviors {

	/// <summary>
	/// GridのColumnDefinitionsを保存、復元します。
	/// </summary>
	public class GridDefinitionSaveBehavior : BehaviorBase<Grid> {
		private GridDefinitionSettings GridDefinitionSettings { get; set; }

		/// <summary>
		/// アプリケーション設定グループの設定キーを取得または設定します。
		/// </summary>
		public string SettingsKey { get; set; }

		private Window window;

		protected override void OnLoaded() {
			if( window == null ) {
				window = Window.GetWindow( this.AssociatedObject );
			}

			if( this.GridDefinitionSettings == null ) {
				this.GridDefinitionSettings = new GridDefinitionSettings( SettingsKey ?? window.GetType().FullName );
			}
			if( this.GridDefinitionSettings.IsUpgrade != true ) {
				this.GridDefinitionSettings.Upgrade();
			}

			window.Closing += window_Closing;

			if( Keyboard.Modifiers.HasFlag( ModifierKeys.Shift ) ) {
				return;
			}

			if( this.GridDefinitionSettings.ColumnsWidth != null ) {
				var ColumnsWidth = this.GridDefinitionSettings.ColumnsWidth;

				if( ColumnsWidth.Length == this.AssociatedObject.ColumnDefinitions.Count ) {
					foreach( var (column, gridLength) in this.AssociatedObject.ColumnDefinitions.Zip( ColumnsWidth ) ) {
						column.Width = new GridLength( gridLength.Value, gridLength.GridUnitType );
					}
				}
			}
			if( this.GridDefinitionSettings.RowsHeight != null ) {
				var RowsHeight = this.GridDefinitionSettings.RowsHeight;

				if( RowsHeight.Length == this.AssociatedObject.RowDefinitions.Count ) {
					foreach( var (row, gridLength) in this.AssociatedObject.RowDefinitions.Zip( RowsHeight ) ) {
						row.Height = new GridLength( gridLength.Value, gridLength.GridUnitType );
					};
				}
			}
		}

		private void window_Closing( object sender, CancelEventArgs e ) {
			if( !e.Cancel ) {
				Save();
			}
		}

		protected override void OnUnloaded() {
			window.Closing -= window_Closing;
			Save();
		}

		private void Save() {
			this.GridDefinitionSettings.RowsHeight = this.AssociatedObject.RowDefinitions.Select( x => new GridLengthValue( x.Height ) ).ToArray();
			this.GridDefinitionSettings.ColumnsWidth = this.AssociatedObject.ColumnDefinitions.Select( x => new GridLengthValue( x.Width ) ).ToArray();

			this.GridDefinitionSettings.IsUpgrade = true;
			this.GridDefinitionSettings.Save();
		}

		[Serializable]
		public class GridLengthValue {

			public GridLengthValue() {
			}

			public GridLengthValue( System.Windows.GridLength value ) {
				this.Value = value.Value;
				this.GridUnitType = value.GridUnitType;
			}

			public GridUnitType GridUnitType { get; set; }
			public double Value { get; set; }
		}
	}

	internal class GridDefinitionSettings : ApplicationSettingsBase {

		public GridDefinitionSettings( string settingsKey ) : base( settingsKey ) {
		}

		[UserScopedSetting]
		public GridDefinitionSaveBehavior.GridLengthValue[] ColumnsWidth {
			get { return (GridDefinitionSaveBehavior.GridLengthValue[])this["ColumnsWidth"]; }
			set { this["ColumnsWidth"] = value; }
		}

		[UserScopedSetting]
		public GridDefinitionSaveBehavior.GridLengthValue[] RowsHeight {
			get { return (GridDefinitionSaveBehavior.GridLengthValue[])this["RowsHeight"]; }
			set { this["RowsHeight"] = value; }
		}

		[UserScopedSetting]
		public bool? IsUpgrade {
			get { return (bool?)this["IsUpgrade"]; }
			set { this["IsUpgrade"] = value; }
		}
	}
}