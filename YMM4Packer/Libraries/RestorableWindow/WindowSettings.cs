using System.Configuration;

namespace Libraries.RestorableWindow {
	/// <summary>
	/// exe.config に Placement と IsUpgrade を作成する
	/// </summary>

	public class WindowSettings : ApplicationSettingsBase, IWindowSettings {

		public WindowSettings( string settingsKey ) : base( settingsKey ) {
		}

		[UserScopedSetting]
		public WINDOWPLACEMENT? Placement {
			get { return (WINDOWPLACEMENT?)this[nameof( Placement )]; }
			set { this[nameof( Placement )] = value; }
		}

		[UserScopedSetting]
		public bool? IsUpgrade {
			get { return (bool?)this[nameof( IsUpgrade )]; }
			set { this[nameof( IsUpgrade )] = value; }
		}
	}
}