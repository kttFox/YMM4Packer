using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace YMM4Packer {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Libraries.RestorableWindow.Window {

		public MainWindow() {
			InitializeComponent();

			this.CommandBindings.Add( new CommandBinding( ApplicationCommands.Close, ( sender, e ) => {
				var s = (Window)sender;
				s.Close();
			} ) );

			this.CommandBindings.Add( new CommandBinding( MainWindow.VersionWindow, ( s, e ) => {
				var f = new AboutWindow() {
					Owner = this,
				};
				f.ShowDialog();
			} ) );

			this.CommandBindings.Add( new CommandBinding( MainWindow.OptionWindow, ( s, e ) => {
				var f = new OptionWindow() {
					Owner = this,
					DataContext = new OptionWindowViewModel()
				};
				f.ShowDialog();
			} ) );
		}

		public static readonly RoutedUICommand VersionWindow = new RoutedUICommand( "バージョン情報", "VersionWindow", typeof( MainWindow ) );
		public static readonly RoutedUICommand OptionWindow = new RoutedUICommand( "オプション", "OptionWindow", typeof( MainWindow ), new InputGestureCollection() { new KeyGesture( Key.T, ModifierKeys.Control ) } );

		private MainWindowViewModel vm => (MainWindowViewModel)this.DataContext;

		private void DataGrid_SelectedCellsChanged( object sender, SelectedCellsChangedEventArgs e ) {
			var s = (DataGrid)sender;
			vm.SelectedCells.Value = s.SelectedCells.Select( x => (Item)x.Item ).Distinct().ToList();
		}

		private void DataGrid_Sorting( object sender, DataGridSortingEventArgs e ) {
			var s = (DataGrid)sender;
			var column = e.Column;

			e.Handled = true;

			column.SortDirection = column.SortDirection switch {
				null => ListSortDirection.Ascending,
				ListSortDirection.Ascending => ListSortDirection.Descending,
				ListSortDirection.Descending => null,
				_ => throw new NotImplementedException(),
			};
			var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView( s.ItemsSource );
			if( lcv is not null ) {
				if( column.SortDirection is null ) {
					lcv.CustomSort = null;
				} else {
					lcv.CustomSort = column.SortMemberPath switch {
						"Type" => new CustomSort( column.SortDirection.Value ),
						_ => new CustomStringSort( column.SortDirection.Value, column.SortMemberPath ),
					};
				}
			}
		}

		public class CustomSort : IComparer {
			private int _direction;

			public CustomSort( ListSortDirection direction ) {
				_direction = direction == ListSortDirection.Ascending ? 1 : -1;
			}

			public int Compare( object? x, object? y ) {
				var item1 = (Item)x!;
				var item2 = (Item)y!;

				return ( SortOrder.IndexOf( item1.Type ) - SortOrder.IndexOf( item2.Type ) ) * _direction;
			}

			private static List<ItemType> SortOrder = new List<ItemType>() { ItemType.Video, ItemType.Audio, ItemType.Image };
		}

		public class CustomStringSort : IComparer {
			private int _direction;
			private string target;

			public CustomStringSort( ListSortDirection direction, string target ) {
				_direction = direction == ListSortDirection.Ascending ? 1 : -1;
				this.target = target;
			}

			public int Compare( object? x, object? y ) {
				var item1 = GetValue( (Item)x! );
				var item2 = GetValue( (Item)y! );

				return StringComparer.InvariantCulture.Compare( item1, item2 ) * _direction;
			}

			private string GetValue( Item item ) => target switch {
				"SubFolder" => item.SubFolder,
				"Name" => item.Name,
				"FilePath" => item.FilePath,
				_ => throw new NotImplementedException(),
			};
		}
	}
}