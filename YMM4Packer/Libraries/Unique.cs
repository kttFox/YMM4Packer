using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Libraries {

	internal static class Unique {

		/// <summary>
		/// 一意の文字列を作成します テキスト(2)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="uniqueTargets">一意を判断する対象</param>
		/// <returns></returns>
		public static string UniqueString( this string value, IEnumerable<string> uniqueTargets )
			 => UniqueString( value, uniqueTargets, 2, index => $" ({index})" );

		/// <summary>
		/// 一意の文字列を作成します テキスト(2)
		/// </summary>
		public static string UniqueString( this string value, IEnumerable<string> uniqueTargets, int startIndex )
			 => UniqueString( value, uniqueTargets, startIndex, index => $" ({index})" );

		/// <summary>
		/// 一意の文字列を作成します テキスト(2)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="uniqueTargets">一意を判断する対象</param>
		/// <param name="startIndex"></param>
		/// <param name="format"></param>
		public static string UniqueString( this string value, IEnumerable<string> uniqueTargets, int startIndex, Func<int, string> format ) {
			if( uniqueTargets is HashSet<string> set ) {
				return UniqueString( value, set, startIndex, index => $" ({index})" );
			} else {
				return UniqueString( value, uniqueTargets.ToHashSet(), startIndex, format );
			}
		}

		/// <summary>
		/// 一意の文字列を作成します テキスト(2)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="uniqueTargets"></param>
		/// <param name="startIndex"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string UniqueString( this string value, HashSet<string> uniqueTargets, int startIndex, Func<int, string> format ) {
			var count = 0;
			var index = startIndex;
			var result = value;

			while( uniqueTargets.Contains( result ) ) {
				if( UInt16.MaxValue <= count ) {
					throw new OverflowException( "一意の文字列を作成できませんでした。" );
				}

				result = value + format( index );

				index++;
				count++;
			}

			return result;
		}

		public static string UniqueFileName( this string fileName, IEnumerable<string> files ) {
			return UniqueFileName( fileName, files, index => $" ({index})", 1 );
		}

		public static string UniqueFileName( this string fileName, IEnumerable<string> files, int startIndex ) {
			return UniqueFileName( fileName, files, index => $" ({index})", startIndex );
		}

		public static string UniqueFileName( this string fileName, IEnumerable<string> files, Func<int, string> format, int startIndex ) {
			var name = Path.GetFileNameWithoutExtension( fileName );
			var extension = Path.GetExtension( fileName );

			var result = fileName;

			var index = startIndex;
			var count = 0;
			while( files.Contains( result ) ) {
				if( UInt16.MaxValue <= count ) {
					throw new OverflowException( "一意のファイル名を作成できませんでした。" );
				}
				result = name + format( index ) + extension;

				index++;
				count++;
			}

			return result;
		}

		public static string UniqueFile( this string file ) => UniqueFile( file, index => $" ({index})", 1 );

		public static string UniqueFile( this string file, Func<int, string> format ) => UniqueFile( file, format, 1 );

		public static string UniqueFile( this string file, Func<int, string> format, int startIndex ) {
			var index = startIndex;
			var result = file;

			var dir = Path.GetDirectoryName( file );
			var filename = Path.GetFileNameWithoutExtension( file );
			var extension = Path.GetExtension( file );

			var count = 0;

			while( File.Exists( result ) ) {
				if( UInt16.MaxValue <= count ) {
					throw new OverflowException( "一意のファイル名を作成できませんでした。" );
				}
				result = Path.Combine( dir, filename + format( index ) + extension );

				index++;
				count++;
			}

			return result;
		}

		/// <summary>
		/// 唯一のディレクトリ名を取得します。重複する場合は(1)と数字を付与します。
		/// </summary>
		/// <param name="dir">対象となるディレクトリパス</param>
		/// <returns>作成可能なディレクトリパス</returns>
		public static string UniqueDirectory( this string dir ) => UniqueDirectory( dir, 1, index => $" ({index})" );

		/// <summary>
		/// 唯一のディレクトリ名を取得します。
		/// </summary>
		/// <param name="dir">対象となるディレクトリ</param>
		/// <param name="startIndex">StartIndexを指定</param>
		/// <param name="format">重複した場合に付与する文字を返す関数</param>
		/// <returns>作成可能なディレクトリパス</returns>
		public static string UniqueDirectory( this string dir, int startIndex, Func<int, string> format ) {
			var index = startIndex;
			var result = dir;

			var basedir = Path.GetDirectoryName( result );
			var dirname = Path.GetFileName( dir );

			var count = 0;
			while( Directory.Exists( result ) ) {
				if( UInt16.MaxValue <= count ) {
					throw new OverflowException( "一意のフォルダ名を作成できませんでした。" );
				}

				result = Path.Combine( basedir, dirname + format( index ) );

				index++;
				count++;
			}

			return result;
		}
	}
}