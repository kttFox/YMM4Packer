using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Libraries.RestorableWindow {

	public class Window : System.Windows.Window {

		public Window() {
			// ユーザーデータを読み込む前に破損していないかチェック
			try {
				ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.PerUserRoamingAndLocal );
			} catch( ConfigurationErrorsException ex ) {
				try { System.IO.File.Delete( ex.Filename ); } catch { }
			}

			this.SourceInitialized += this.Window_SourceInitialized;
		}

		private void Window_SourceInitialized( object sender, EventArgs e ) {
			// 外部からウィンドウ設定の保存・復元クラスが与えられていない場合は、既定実装を使用する
			if( this.WindowSettings == null ) {
				this.WindowSettings = new WindowSettings( this.GetType().FullName );
			}

			if( this.WindowSettings.IsUpgrade != true ) {
				this.WindowSettings.Upgrade();
			}

			if( Keyboard.Modifiers.HasFlag( ModifierKeys.Shift ) ) {
				return;
			}

			if( this.WindowSettings.Placement.HasValue ) {
				if( this.IsRestoreSizeOnly ) {
					var dpiScale = GetDpiScaleFactor( this );

					var normalPosition = this.WindowSettings.Placement.Value.normalPosition;
					this.Width = ( normalPosition.Right - normalPosition.Left ) / dpiScale.X;
					this.Height = ( normalPosition.Bottom - normalPosition.Top ) / dpiScale.Y;

					// この3パターンしか無い
					switch( this.WindowSettings.Placement.Value.showCmd ) {
						case SW.ShowMaximized: {
							this.WindowState = WindowState.Maximized;

							break;
						}
						case SW.ShowNormal:
						case SW.ShowMinimized:
						default: {
							break;
						}
					}
				} else {
					var hwnd = new WindowInteropHelper( this ).Handle;
					var placement = this.WindowSettings.Placement.Value;
					placement.length = Marshal.SizeOf( typeof( WINDOWPLACEMENT ) );
					placement.flags = 0;
					placement.showCmd = ( placement.showCmd == SW.ShowMinimized ) ? SW.ShowNormal : placement.showCmd;

					this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
					NativeMethods.SetWindowPlacement( hwnd, ref placement );
				}
			}
		}

		/// <summary>
		/// ウインドウサイズのみを復元します
		/// </summary>
		public bool IsRestoreSizeOnly { get; set; }

		#region WindowSettings 依存関係プロパティ

		public IWindowSettings WindowSettings {
			get { return (IWindowSettings)this.GetValue( WindowSettingsProperty ); }
			set { this.SetValue( WindowSettingsProperty, value ); }
		}

		public static readonly DependencyProperty WindowSettingsProperty = DependencyProperty.Register( "WindowSettings", typeof( IWindowSettings ), typeof( Window ), new UIPropertyMetadata( null ) );

		#endregion WindowSettings 依存関係プロパティ

		protected override void OnClosing( CancelEventArgs e ) {
			base.OnClosing( e );

			if( !e.Cancel ) {
				var hwnd = new WindowInteropHelper( this ).Handle;
				NativeMethods.GetWindowPlacement( hwnd, out var placement );

				if( this.WindowSettings != null ) {
					this.WindowSettings.Placement = placement;
					this.WindowSettings.IsUpgrade = true;
					this.WindowSettings.Save();
				}
			}
		}

		/// <summary>
		/// 現在の <see cref="T:System.Windows.Media.Visual"/> から、DPI 倍率を取得します。
		/// </summary>
		/// <returns>
		/// X 軸 および Y 軸それぞれの DPI 倍率を表す <see cref="T:System.Windows.Point"/>
		/// 構造体。取得に失敗した場合、(1.0, 1.0) を返します。
		/// </returns>
		public static Point GetDpiScaleFactor( Visual visual ) {
			var source = PresentationSource.FromVisual( visual );
			if( source != null && source.CompositionTarget != null ) {
				return new Point(
					source.CompositionTarget.TransformToDevice.M11,
					source.CompositionTarget.TransformToDevice.M22 );
			}

			return new Point( 1.0, 1.0 );
		}
	}
}