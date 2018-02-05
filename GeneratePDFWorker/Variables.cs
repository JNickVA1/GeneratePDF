using System.Collections.Generic;

namespace GeneratePDFWorker
{
	/// <summary>
	/// The class that repersents the usable information from the Variables file.
	/// </summary>
	internal class Variables : InputData
	{
		#region Fields
		// The local Dictionary object.
		private Dictionary<int, string> _variableKeysAndValues;
		#endregion Fields

		#region Ctors
		#endregion Ctors

		#region Properties
		/// <summary>
		/// The Dictionary object containing the index and value for each variable.
		/// </summary>
		internal Dictionary<int, string> VariableKeysAndValues => _variableKeysAndValues ?? (_variableKeysAndValues = new Dictionary<int, string>());
		#endregion Properties

		#region Methods
		#endregion Methods
	}
}
