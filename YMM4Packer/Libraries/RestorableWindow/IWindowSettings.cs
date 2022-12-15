namespace Libraries.RestorableWindow {

	public interface IWindowSettings {
		WINDOWPLACEMENT? Placement { get; set; }
		bool? IsUpgrade { get; set; }

		void Reload();

		void Save();

		void Upgrade();
	}
}