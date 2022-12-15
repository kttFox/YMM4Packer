using Libraries;
using LivetEx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YMM4Packer {
	public class YMMPacker {
		internal YMMPacker() {
		}

		public YMMPacker( string file ) {
			this.TopNode = JObject.Parse( File.ReadAllText( file ) );

			this.FileName = Path.GetFileName( file );
			this.Items = this.TopNode["Timeline"]!["Items"]!.Where( x => x["FilePath"] is not null )
								.Select( x => new OriginItem( x ) )
								.GroupBy( x => x.FilePathOrigin )
								.Select( x => new Item( x ) )
								.ToList();

			this.Fonts = this.TopNode["Timeline"]!["Items"]!
								.Where( x => x["Font"] is not null )
								.Select( x => x["Font"]!.ToString() )
								.Distinct().ToList();
		}

		public string FileName { get; }

		public IReadOnlyList<Item> Items { get; internal set; }

		public IReadOnlyList<string> Fonts { get; }

		private readonly JObject TopNode;

		public bool IsShiftJIS { get; set; } = true;

		private static readonly Encoding sjis = Encoding.GetEncoding( "Shift-JIS" );
		public static readonly string DummyDir = @"C:\Dummy\";

		public void Save( string file ) {
			this.TopNode["FilePath"] = Path.Combine( DummyDir, Path.GetFileName( file ) );

			var dir = Path.GetDirectoryName( file )!;
			foreach( var item in Items ) {
				if( this.IsShiftJIS ) {
					item.Name = sjis.GetString( sjis.GetBytes( item.Name ) ).Replace( "?", "_" );
				}

				var destFileName = Path.Combine( dir, item.SubFolder, item.Name ).UniqueFile();

				var isDubg = false;
#if DEBUG
				isDubg = true;
#endif
				Directory.CreateDirectory( Path.GetDirectoryName( destFileName )! );
				if( isDubg && !File.Exists( item.FilePath ) ) {
					File.Create( destFileName ).Dispose();
				} else {
					File.Copy( item.FilePath, destFileName );
				}

				item.Name = Path.GetFileName( destFileName );
				item.Save();
			}

			var jsonString = JsonConvert.SerializeObject( this.TopNode, new JsonSerializerSettings() { Formatting = Formatting.Indented } );
			File.WriteAllText( file, jsonString, Encoding.UTF8 );

			File.AppendAllText( Path.Combine( dir, "使用フォント.txt" ), string.Join( "\r\n", this.Fonts ) );
		}
	}

	public class Item : NotificationObject {

		internal Item( string name, string path, ItemType type ) {
			this._Name = name;
			this.FilePath = path;
			this.Type = type;
		}

		public Item( IEnumerable<OriginItem> originItems ) {
			this.OriginItems = originItems.ToList();
			this._Name = this.OriginItems.First().Name;
			this.Type = this.OriginItems.First().Type;
			this.FilePath = this.OriginItems.First().FilePathOrigin;
		}

		#region Name 変更通知プロパティ

		public string Name {
			get { return _Name; }
			set {
				if( _Name != value ) {
					_Name = value;
					RaisePropertyChanged();
				}
			}
		}

		private string _Name;

		#endregion Name 変更通知プロパティ

		public string FilePath { get; }

		public ItemType Type { get; }

		public List<OriginItem> OriginItems { get; }

		#region SubFolder 変更通知プロパティ

		public string SubFolder {
			get { return _SubFolder; }
			set {
				if( _SubFolder != value ) {
					_SubFolder = value;
					RaisePropertyChanged();
				}
			}
		}

		private string _SubFolder = "";

		#endregion SubFolder 変更通知プロパティ

		public void Save() {
			foreach( var item in OriginItems ) {
				item.Origin["FilePath"] = Path.Combine( YMMPacker.DummyDir, this.SubFolder, this.Name );
			}
		}
	}

	public class OriginItem {

		public OriginItem( JToken token ) {
			this.Origin = token;

			this.Type = token["$type"]?.ToString() switch {
				"YukkuriMovieMaker.Project.Items.ImageItem, YukkuriMovieMaker" => ItemType.Image,
				"YukkuriMovieMaker.Project.Items.VideoItem, YukkuriMovieMaker" => ItemType.Video,
				"YukkuriMovieMaker.Project.Items.AudioItem, YukkuriMovieMaker" => ItemType.Audio,
				_ => throw new NotImplementedException(),
			};

			this.FilePathOrigin = token["FilePath"]?.ToString() ?? throw new ArgumentException();

			this.Name = Path.GetFileName( this.FilePathOrigin );
		}

		internal JToken Origin { get; }

		public string Name { get; }

		public string FilePathOrigin { get; }

		public ItemType Type { get; }
	}

	public enum ItemType {
		Image,
		Video,
		Audio,
	}
}