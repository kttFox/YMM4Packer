using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace Libraries.Behaviors {

	public abstract class BehaviorBase<T> : Behavior<T> where T : FrameworkElement {

		protected override void OnAttached() {
			base.OnAttached();

			//WeakEventManager<FrameworkElement, RoutedEventArgs>.AddHandler( this.AssociatedObject, "Loaded", AssociatedObject_Loaded );
			//WeakEventManager<FrameworkElement, RoutedEventArgs>.AddHandler( this.AssociatedObject, "Unloaded", AssociatedObject_Unloaded );

			this.AssociatedObject.Loaded += AssociatedObject_Loaded;
			this.AssociatedObject.Unloaded += AssociatedObject_Unloaded;
		}

		protected override void OnDetaching() {
			this.AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
			this.AssociatedObject.Loaded -= AssociatedObject_Loaded;

			base.OnDetaching();
		}

		private bool IsLoaded;

		private void AssociatedObject_Loaded( object sender, RoutedEventArgs e ) {
			if( !IsLoaded ) {
				IsLoaded = true;
				OnLoaded();
			}
		}

		private void AssociatedObject_Unloaded( object sender, RoutedEventArgs e ) {
			OnUnloaded();
			IsLoaded = false;
		}

		protected abstract void OnLoaded();

		protected abstract void OnUnloaded();

		public void Refresh() {
			OnUnloaded();
			OnLoaded();
		}
	}
}