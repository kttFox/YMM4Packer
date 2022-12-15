using System.Collections.Generic;

namespace YMM4Packer {
#if DEBUG

	public static class design {

		public static MainWindowViewModel MainWindowViewModel => new MainWindowViewModel() {
			YMMPacker = { Value = new YMMPacker() {
				Items = new List<Item>() {
							new Item("name", @"C:\audio\1.mp4", ItemType.Video ),
							new Item("name", @"C:\image\a.png", ItemType.Image ),
							new Item("name", @"C:\audio\2.mp4", ItemType.Audio ),
							new Item("name", @"C:\image\image2\a.png", ItemType.Image ),
					},
				}
			},
			Fonts = { Value = "aaa\r\nbbbb" },
			FontsCount = { Value = 2 },
		};
	}

#endif
}