using Libraries;
using LivetEx;
using LivetEx.Commands;
using LivetEx.Messaging;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;

namespace YMM4Packer {
	public class MainWindowViewModel : ViewModel {
		public MainWindowViewModel() {
			this.SelectedDir.Where( x => _SelectedDirSwitch ).Subscribe( x => {
				foreach( var item in SelectedCells.Value ) {
					item.SubFolder = x;
				}
			} );

			this.SelectedCells.Subscribe( x => {
				var dirs = x.Select( x => x.SubFolder ).Distinct();

				if( dirs.Count() == 1 ) {
					this.SelectedDir.Value = dirs.First();
				} else {
					_SelectedDirSwitch = false;
					this.SelectedDir.Value = "";
					_SelectedDirSwitch = true;
				}
			} );
		}

		public ReactiveProperty<string> Fonts { get; } = new();
		public ReactiveProperty<int> FontsCount { get; } = new();

		public ReactiveProperty<bool> IsShiftJIS { get; } = new( true );

		public void Initialize( Window window ) {
			window.CommandBindings.Add( new CommandBinding( ApplicationCommands.Open, ( s, e ) => Open() ) );
			window.CommandBindings.Add( new CommandBinding( ApplicationCommands.SaveAs, ( s, e ) => SaveAs() ) );

			this.FileDropCommand.TryExecute( Environment.GetCommandLineArgs() );
		}

		private void Open() {
			var r = this.Messenger.GetResponse( new OpenFileDialogMessage() {
				Title = "プロジェクトファイルを選択",
				Filter = @"ymmp|*.ymmp|" +
							"すべてのファイル|*.*",
			} );

			var file = r?.FirstOrDefault( x => x.ToLower().EndsWith( ".ymmp" ) );
			if( file is not null ) {
				Open( file );
			}
		}

		public ReactiveProperty<YMMPacker> YMMPacker { get; } = new();

		public ReactiveProperty<string> SelectedDir { get; } = new( "" );
		private bool _SelectedDirSwitch = true;

		public ReactiveProperty<IList<Item>> SelectedCells { get; } = new( new List<Item>() );

		private void Open( string file ) {
			this.YMMPacker.Value = new YMMPacker( file );

			this.Fonts.Value = string.Join( "\r\n", this.YMMPacker.Value.Fonts );
			this.FontsCount.Value = this.YMMPacker.Value.Fonts.Count;
		}

		private void SaveAs() {
			var f = this.Messenger.GetResponse( new SaveFileDialogMessage() {
				FileName = this.YMMPacker.Value.FileName,
				Filter = $"プロジェクトファイル|*ymmp|すべてのファイル|*.*",
				OverwritePrompt = false,
			} );

			var file = f?.FirstOrDefault();
			if( file == null ) {
				return;
			}

			// 保存先にファイルが存在した場合は、フォルダを生成する
			var dir = Path.GetDirectoryName( file );
			if( Directory.Exists( dir ) && Directory.EnumerateFileSystemEntries( dir ).Any() ) {
				dir = Path.Combine( dir, Path.GetFileNameWithoutExtension( file ) ).UniqueDirectory();
				file = Path.Combine( dir, Path.GetFileName( file ) );
			}

			this.YMMPacker.Value.IsShiftJIS = IsShiftJIS.Value;
			this.YMMPacker.Value.Save( file );

			Process.Start( @"explorer.exe", $@"/select,""{file}""" );
		}

		#region TypeApplyCommand

		private ReactiveCommand? _TypeApplyCommand;

		public ReactiveCommand TypeApplyCommand => _TypeApplyCommand ??=
														this.YMMPacker.Select( x => x is not null )
														.ToReactiveCommand()
														.WithSubscribe( DoTypeApplyCommand );

		private void DoTypeApplyCommand() {
			foreach( var item in this.YMMPacker.Value.Items ) {
				item.SubFolder = item.Type.ToString();
			}
		}

		#endregion TypeApplyCommand

		#region FileDropCommand

		private DelegateCommand<string[]>? _FileDropCommand;

		public DelegateCommand<string[]> FileDropCommand {
			get {
				if( _FileDropCommand == null ) {
					_FileDropCommand = new DelegateCommand<string[]>( DoFileDropCommand, CanFileDropCommand );
				}
				return _FileDropCommand;
			}
		}

		private bool CanFileDropCommand( string[] files ) {
			return files.Any( x => x.ToLower().EndsWith( ".ymmp" ) );
		}

		private void DoFileDropCommand( string[] files ) {
			var file = files.First( x => x.ToLower().EndsWith( ".ymmp" ) );

			Open( file );
		}

		#endregion FileDropCommand

		#region PackageCommand

		private ReactiveCommand? _PackageCommand;

		public ReactiveCommand PackageCommand => _PackageCommand ??=
													this.YMMPacker.Select( x => x is not null )
													.ToReactiveCommand()
													.WithSubscribe( DoPackageCommand );

		private void DoPackageCommand() {
			SaveAs();
		}

		#endregion PackageCommand
	}
}