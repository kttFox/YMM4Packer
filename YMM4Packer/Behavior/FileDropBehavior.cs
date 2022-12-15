using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Input;

namespace YMM4Packer.Behavior {

	public class FileDropBehavior : Behavior<FrameworkElement> {

		#region Register Command

		public ICommand Command {
			get { return (ICommand)GetValue( CommandProperty ); }
			set { SetValue( CommandProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register( "Command", typeof( ICommand ), typeof( FileDropBehavior ), new PropertyMetadata( null ) );

		#endregion Register Command

		protected override void OnAttached() {
			base.OnAttached();

			this.AssociatedObject.AllowDrop = true;

			this.AssociatedObject.DragOver += AssociatedObject_DragOver;
			this.AssociatedObject.Drop += AssociatedObject_Drop;
		}

		private void AssociatedObject_DragOver( object sender, DragEventArgs e ) {
			var files = e.Data.GetData( DataFormats.FileDrop ) as string[];

			if( Command.CanExecute( files ) ) {
				e.Effects = DragDropEffects.All;
			} else {
				e.Effects = DragDropEffects.None;
			}

			e.Handled = true;
		}

		private void AssociatedObject_Drop( object sender, DragEventArgs e ) {
			string[] fileNames = e.Data.GetData( DataFormats.FileDrop ) as string[];

			if( Command.CanExecute( fileNames ) ) {
				this.AssociatedObject.Dispatcher.BeginInvoke(
					new Action( () => Command.Execute( fileNames ) )
				);
				e.Handled = true;
			}
		}

		protected override void OnDetaching() {
			base.OnDetaching();

			this.AssociatedObject.DragOver -= AssociatedObject_DragOver;
			this.AssociatedObject.Drop -= AssociatedObject_Drop;
		}
	}
}