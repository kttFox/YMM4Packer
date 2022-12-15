using System.Windows.Controls;

namespace YMM4Packer {

	public class DataGrid : System.Windows.Controls.DataGrid {

		public DataGrid() {
			this.AutoGenerateColumns = false;
			this.CanUserAddRows = false;
			this.CanUserDeleteRows = false;
			this.CanUserReorderColumns = false;
			this.CanUserResizeRows = false;
			this.CanUserSortColumns = true;
			this.GridLinesVisibility = DataGridGridLinesVisibility.None;
			this.HeadersVisibility = DataGridHeadersVisibility.Column;

			VirtualizingPanel.SetScrollUnit( this, ScrollUnit.Pixel );
		}

		protected override void OnLoadingRow( DataGridRowEventArgs e ) {
			base.OnLoadingRow( e );

			e.Row.Background = this.Background;
		}
	}
}