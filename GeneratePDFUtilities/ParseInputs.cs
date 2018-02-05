using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratePDFWorker
{

    public class ParseInputs
    {
		/// <summary>
		/// A static method used to check the validity of an input file.
		/// </summary>
		/// <param name="inputFilename">The filename of the input to be checked.</param>
		/// <param name="fileType">The type of input.</param>
		/// <returns>'true' if the file is valid, 'false' if invalid, throws an exception for errors.</returns>
		public bool IsValidInput(string inputFilename, InputTypes fileType)
		{
			var bRet = false;

			// Ensure that the file exists.
			if (!File.Exists(inputFilename))
			{
				throw new ApplicationException("Invalid file path or insufficient permissions for file: " + inputFilename);
			}

			// Call the file type specific method to perform validation.
			switch (fileType)
			{
				case InputTypes.ContentDefinition:
					bRet = CheckForValidContentFile(inputFilename);
					break;
				case InputTypes.CustomerData:
					bRet = CheckForValidCustomerDataFile(inputFilename);
					break;
				case InputTypes.PageDefinition:
					bRet = CheckForValidPageDefinitionFile(inputFilename);
					break;
				case InputTypes.Variables:
					bRet = CheckForValidVariablesFile(inputFilename);
					break;
				default:
					break;
			}

			return bRet;
		}

		/// <summary>
		/// Checks the Variables file for required attributes.
		/// </summary>
		/// <param name="inputFilename">The file to check.</param>
		/// <returns>'true' if valid. 'false' or an Exception if invalid.</returns>
		/// <remarks>
		/// The variables file will contain zero or more records. 
		/// Each record will contain 2 values, seperated by a '|' (pipe).
		/// The first value in each record will be the unique identifier that is specified in the Content file.
		/// The second value in each record will be the content used to replace the placeholder in the Contnt file.
		/// </remarks>
		private bool CheckForValidVariablesFile(string inputFilename)
		{
			return true;
		}

		private bool CheckForValidPageDefinitionFile(string inputFilename)
		{
			return true;
		}

		private bool CheckForValidCustomerDataFile(string inputFilename)
		{
			return true;
		}

		private bool CheckForValidContentFile(string inputFilename)
		{
			return true;
		}
	}
}
