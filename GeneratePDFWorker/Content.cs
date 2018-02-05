using System;
using System.Collections.Generic;

namespace GeneratePDFWorker
{
	/// <summary>
	/// The Content class represents the data in the content xml input file. 
	/// 
	/// Each record in the input file contains 4 parts:
	///		1. A unique identifier that corresponds to the same identifier in the Page Definition file
	///		2. A condition that must be met for the content to be used. This can be a Boolean condition or some other function.
	///		3. A content type. Common types are 'T' for Text, 'G' for Graphic, and 'A:name' for an array name used for Tables.
	///		4. A content value that can include markup, text, paragraph style, line spacing, image names, tables, graphs and charts.
	/// </summary>
	/// <remarks>
	/// The Content file will make use of the data that is contained within the content marked as %%1%%.  This will indicate that a 
	/// replacement will need to occur.  %%1%% will be the first data element produced by the application that will be user defined.  
	/// If the variable is passed as non numeric, ie-%%1_AcctNum%% then it will be pulled from the customer data file, known as the 
	/// first database.
	/// </remarks>
	internal class Content : InputData
	{
		#region Fields
		#endregion Fields

		#region Ctors
		/// <summary>
		/// 
		/// </summary>
		internal Content()
		{
			ContentItems = new List<ContentItem>();
		}
		#endregion Ctors

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		internal List<ContentItem> ContentItems { get; }
		#endregion Properties

		#region Methods
		#endregion Methods
	}
}
