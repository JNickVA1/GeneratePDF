using GeneratePDFWorker;
using System;
using System.Linq;

namespace GeneratePDFConsole
{
	internal static class Program
	{
		private const int ErrorValue = -1;
		private const int SuccessValue = 0;

		/// <summary>
		/// The startup method for the console application.
		/// </summary>
		/// <param name="args">The filenames for the data files. Should be in the order:
		/// PageDefinition, ContentDefinition, Variables, CustomerData.</param>
		private static int Main(string[] args)
		{
			// Set the default return value.
			var retVal = ErrorValue;

			// Verify that we have 4 input file names.
			// NOTE: The files will be parsed and validated for content by the GeneratePDFWorker.
			if (args.Length != 4)
			{
				Console.WriteLine("Please enter the required program arguments.");
				Console.WriteLine("Usage: GeneratePDFConsole <PageDefinition.txt|xml> <ContentDefnition.txt>");
				Console.WriteLine("                          <VariablesFile.txt> <CustomerData.txt|xml>");
			}
			else
			{
				// We have 4 input strings, so let the worker determine if the strings are valid input files.
				try
				{
					// Create a new PdfWorker with the 4 input strings.
					var worker = new PdfWorker(args[0], args[1], args[2], args[3]);

					// Have the PdfWorker validate the inputs.
					worker.ValidateInputs();

					// If the inputs are valid, instruct the PdfWorker to perform the necessary actions on the inputs.
					worker.Work();

					// Inform the user that processing is complete.
					Console.WriteLine("Processing complete.");

				}
				catch (Exception ex)
				{
					// Inform the user of any errors.
					Console.WriteLine("Error during processing. Error: " + ex.Message);
				}
			}
			// Tell the user how to continue.
			Console.WriteLine("Press any key to quit. \n");

			// Read the next key entered.
			Console.ReadKey();

			return retVal;
		}
	}
}
