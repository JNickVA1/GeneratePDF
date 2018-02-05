using System;

namespace GeneratePDFWorker
{
	/// <summary>
	/// 
	/// 
	/// 1. The Page number for content item.
	/// 2. The Zone number for content item.
	/// 3. The Part number for content item.
	/// 4. The object containing the content value.
	/// </summary>
	internal class ContentItem : Tuple<string, string, string, ContentValue>
	{
		#region Fields
		#endregion Fields

		#region Ctors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item1">The Page number for content item.</param>
		/// <param name="item2">The Zone number for content item.</param>
		/// <param name="item3">The Part number for content item.</param>
		/// <param name="item4">The object containing the content value.</param>
		internal ContentItem(string item1, string item2, string item3, ContentValue item4) : base(item1, item2, item3, item4)
		{
		}
		#endregion Ctors

		#region Properties
		#endregion Properties

		#region Methods
		#endregion Methods
	}
}
