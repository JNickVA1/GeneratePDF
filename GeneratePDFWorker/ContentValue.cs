using System;

namespace GeneratePDFWorker
{
	/// <summary>
	/// 
	/// </summary>
	internal class ContentValue
	{
		#region Fields
		#endregion Fields

		#region Ctors
		/// <summary>
		/// The default constructor that takes the content value string as an argument.
		/// </summary>
		/// <param name="valueAsString">The entire content value string, yet to be parsed and have it's component parts extracted.</param>
		internal ContentValue(string valueAsString)
		{
			// Save the string representation of the content value.
			ContentValueString = valueAsString;

			// Evaluate the value string and extract it's component parts.
			try
			{
				ExtractContentValueParts();
			}
			catch (Exception e)
			{
				throw new ApplicationException("Error in ContentValue ctor. Exception: " + e.Message);
			}
		}
		#endregion Ctors

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		internal string ContentValueString { get; private set; }

		#endregion Properties

		#region Methods
		/// <summary>
		/// Extracts the component parts of the content value.
		/// </summary>
		/// <remarks>
		/// By default there are four components in the content value string:
		/// 1. An expression, such as an "if" statement. OPTIONAL
		/// 2. A value type. Currently the program supports Graphics (G), Text (T), and Tables (items). This list bmay be expanded. REQUIRED
		/// 3. The value to be output for the type. For example, if the value type is "G", then the value will be an image filename.
		/// </remarks>
		private void ExtractContentValueParts()
		{
			// TODO: Initialize the delimiter array from Settings.
			var delims = new char[1] {'|'};

			// Remove any leading and trailing delimiters.
			ContentValueString = ContentValueString.Trim(delims);

			//
			var delimiterIndices = ContentValueString.Split(delims, StringSplitOptions.None);
		}
		#endregion Methods
	}
}
