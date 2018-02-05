using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratePDFWorker
{
	/// <summary>
	/// The base class for the objects used to access the input data files.
	/// </summary>
	/// <remarks>
	/// Contains commoan properties, fields, and methods used across all input data objects.
	/// </remarks>
	internal class InputData
	{
		#region Properties
		/// <summary>
		/// The file used for input.
		/// </summary>
		internal string InputFilename { get; set; }

		/// <summary>
		/// The array of lines representing the input file's contents.
		/// </summary>
		internal string[] FileData { get; set; }
		#endregion Properties
	}
}
