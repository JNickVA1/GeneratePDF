using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using GeneratePDFWorker.Properties;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using PdfWriter = iText.Kernel.Pdf.PdfWriter;

namespace GeneratePDFWorker
{
	/// <summary>
	/// The known file types.
	/// </summary>
	public enum InputTypes
	{
		PageDefinition,
		ContentDefinition,
		Variables,
		CustomerData
	}

	/// <summary>
	/// The PdfWorker is responsible for:
	/// 1. Validating inputs
	/// 2. Creating a new PDF document using the iTextSharp library.
	/// 3. Using the data supplied in the input files to populate the new PDF document.
	/// 4. Save the PDF document.
	/// 5. Report any errors.
	/// </summary>
	public class PdfWorker
	{
		#region Fields
		#endregion Fields

		#region Ctors
		public PdfWorker(string filename1, string filename2, string filename3, string filename4)
		{
			// Read the input file location from Settings.
			var path = Settings.Default.TestFilesLocation;

			// Store the parameters.
			PageDefinitionFilename = path + "\\" + filename1;
			ContentDefinitionFilename = path + "\\" + filename2;
			VariablesDefinitionFilename = path + "\\" + filename3;
			CustomerDataFilename = path + "\\" + filename4;
		}
		#endregion Ctors

		#region Properties
		private string ContentDefinitionFilename { get; }
		private string CustomerDataFilename { get; }
		private string PageDefinitionFilename { get; }
		private string VariablesDefinitionFilename { get; }

		private Variables VariablesObject { get; set; }
		private Pages PageDefinitionObject { get; set; }
		private Invoices CustomerDataObject { get; set; }
		private Content ContentObject { get; set; }
		#endregion Properties

		#region Methods
		/// <summary>
		/// Checks program input and throws an exception if rules for a specific file type are not met.
		/// </summary>
		/// <returns></returns>
		public void ValidateInputs()
		{
			// Validate the page definition file. It can be either .txt or .xml
			if (!IsValidInput(PageDefinitionFilename, InputTypes.PageDefinition))
			{
				throw new ApplicationException("Page definition file is not valid.");
			}
			// Validate the content definition file. It should be a pipe-delimited txt file.
			if (!IsValidInput(ContentDefinitionFilename, InputTypes.ContentDefinition))
			{
				throw new ApplicationException("Content definition file is not valid.");
			}
			// Validate the variables file. It should be a pipe-delimited txt file.
			if (!IsValidInput(VariablesDefinitionFilename, InputTypes.Variables))
			{
				throw new ApplicationException("Variables file is not valid.");
			}
			// Validate the customer data file. It can be xml or csv or txt.
			if (!IsValidInput(CustomerDataFilename, InputTypes.CustomerData))
			{
				throw new ApplicationException("Customer data file is not valid.");
			}
		}

		/// <summary>
		/// A static method used to check the validity of an input file.
		/// </summary>
		/// <param name="inputFilename">The filename of the input to be checked.</param>
		/// <param name="fileType">The type of input.</param>
		/// <returns>'true' if the file is valid, 'false' if invalid, throws an exception for errors.</returns>
		private bool IsValidInput(string inputFilename, InputTypes fileType)
		{
			var bRet = false;

			// Make sure that we have a valid input filename string.
			if (string.IsNullOrEmpty(inputFilename)) throw new ArgumentNullException(nameof(inputFilename));

			// Ensure that the file exists.
			if (!File.Exists(inputFilename))
			{
				throw new ApplicationException("Invalid file path or insufficient permissions for file: " + inputFilename);
			}

			// Call the file type specific method to perform validation.
			switch (fileType)
			{
				case InputTypes.PageDefinition:
					bRet = CheckForValidPageDefinitionFile(inputFilename);
					break;
				case InputTypes.ContentDefinition:
					bRet = CheckForValidContentFile(inputFilename);
					break;
				case InputTypes.Variables:
					bRet = CheckForValidVariablesFile(inputFilename);
					break;
				case InputTypes.CustomerData:
					bRet = CheckForValidCustomerDataFile(inputFilename);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
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
			// We know that the file exists because that was checked in the 
			// IsValidInput method, so we need to create a Variables object.
			VariablesObject = new Variables();

			try
			{
				// Save the input filename in case we need it later.
				VariablesObject.InputFilename = inputFilename;

				// Read each line of the file into a string array. Each 
				// element of the array is one line of the file.
				var lines = File.ReadAllLines(inputFilename);

				// Save the array of lines in case we need it again.
				VariablesObject.FileData = lines;

				// Call the method to fill the object.
				ReadVariables(lines, VariablesObject);

				// Return if no exceptions.
				return true;
			}
			catch (Exception ex)
			{
				throw new ApplicationException(@"Error in CheckForValidVariablesFile", ex.InnerException);
			}
		}

		/// <summary>
		/// Reads the Variables file and populates the Variables object for each variable defined.
		/// </summary>
		/// <param name="lines">The array of lines read from the input file.</param>
		/// <param name="variablesObject">The Variables object that will be populated with the information from the input file.</param>
		private void ReadVariables(string[] lines, Variables variablesObject)
		{
			// Parse each line in the lines array and populate the object.
			foreach (var line in lines)
			{
				// Find the locations of the first and last '|' in the line.
				// ToDo: Make all delimiters configurable in Settings.
				var firstPipeLoc = line.IndexOf("|", StringComparison.Ordinal);
				var lastPipeLoc = line.LastIndexOf("|", StringComparison.Ordinal);

				// Extract the index from the line. The index must be an integer.
				var index = Convert.ToInt32(line.Substring(0, firstPipeLoc));

				// Extract the value.
				var firstCharOfValue = firstPipeLoc + 1;
				var value = line.Substring(firstCharOfValue, lastPipeLoc - firstCharOfValue);

				// Add the index and value to the Variables object's Dictionary.
				variablesObject.VariableKeysAndValues.Add(index, value);
			}
		}

		/// <summary>
		/// Checks the Page Definition file for completeness.
		/// </summary>
		/// <param name="inputFilename">The file to check.</param>
		/// <returns>'true' if valid. 'false' or an Exception if invalid.</returns>
		/// <remarks>
		/// We are allowing two file types for the time being; text files and xml files.
		/// However, the text file must still contain a representation of the XML. A schema 
		/// will be forthcoming.
		/// </remarks>
		private bool CheckForValidPageDefinitionFile(string inputFilename)
		{
			try
			{
				// Create an XmlDocument that we will use to hold and manipulate the XML data file.
				var xd = new XmlDocument();

				// Load the XML file.
				xd.Load((inputFilename));

				// Add the input specific schema to the XmlDocument.
				var xsdPath = Settings.Default.InputLocation + "\\" + Settings.Default.PageDefinitionsSchema;
				xd.Schemas.Add(null, xsdPath);

				// Create the schema validation event handler.
				ValidationEventHandler eventHandler = ValidationEventHandler;

				// Validate the XML data against the schema.
				xd.Validate(eventHandler);

				// Get the XML document's root element and verify that its what we expect.
				var elRoot = xd.DocumentElement;
				if (elRoot != null && elRoot.Name == "Pages")
				{
					// Create the Pages object that will be populated with the 
					// Page Definition information from the XML file input.
					PageDefinitionObject = new Pages();

					// Call the method to fill the object.
					ReadPageDefinition(elRoot, PageDefinitionObject);
				}
			}
			catch (XmlSchemaValidationException exSchema)
			{
				throw new ApplicationException(@"Error in CheckForValidPageDefinitionFile", exSchema.InnerException);
			}
			catch (XPathException exXPath)
			{
				throw new ApplicationException(@"Error in CheckForValidPageDefinitionFile", exXPath.InnerException);
			}
			catch (Exception ex)
			{
				throw new ApplicationException(@"Error in CheckForValidPageDefinitionFile", ex.InnerException);
			}
			return true;
		}

		/// <summary>
		/// Reads the Page Definition file and populates the PagesPage object for each Page defined.
		/// </summary>
		/// <param name="elRoot">The root Pages element in the input XML.</param>
		/// <param name="pageDefinitionObject">The Pages object that will be populated with the information from the input file.</param>
		private void ReadPageDefinition(XmlElement elRoot, Pages pageDefinitionObject)
		{
			// Find all Page elements.
			var nodeList = elRoot.SelectNodes("descendant::Page");

			// There must be a minimum of ONE Page element.
			if (nodeList == null || nodeList.Count == 0)
			{
				throw new ApplicationException("Missing Page element in XML input file. At least one is required.");
			}
			// Create a Pages array to hold the total number of Pages in the XML file.
			pageDefinitionObject.Page = new PagesPage[nodeList.Count];

			// Start with the first PAge in the Page node list.
			var pageIndex = 0;

			// Process each Page element.
			foreach (XmlNode pageNode in nodeList)
			{
				// Create a new Page.
				pageDefinitionObject.Page[pageIndex] = new PagesPage();

				// Read the Page attribute(s).
				var att = pageNode.Attributes;

				// Save the Page number if present. This attribute is required.
				// The value must be an unsigned short integer.
				if (att != null && att.Count == 1)
				{
					pageDefinitionObject.Page[pageIndex].Pagenumber = Convert.ToUInt16(att["Pagenumber"].Value);

					// TODO: Add code to ensure that page numbers are consecutive starting at '1'.

				}
				else
				{
					if (att == null || att.Count == 0)
						throw new ApplicationException("Missing Pagenumber attribute for Page element" + pageIndex + " in XML input file.");
					throw new ApplicationException("Incorrect number of attributes for Page element " + pageIndex + " in XML input file.");
				}
				// Read and save the page size. This element is NOT nillable (it must be present).
				// Only one Pagesize element per Page is allowed.
				// TODO: Make Pagesize an attribute of Page.
				var pageSize = pageNode.SelectNodes("descendant::Pagesize");
				if (pageSize != null && pageSize.Count == 1)
				{
					pageDefinitionObject.Page[pageIndex].Pagesize = pageSize[0].InnerText;

					// TODO: Add code to validate the format of the page size value. It should be in the form: widthXheight
				}
				else
				{
					if (pageSize == null || pageSize.Count <= 1)
						throw new ApplicationException("Missing required Pagesize element for Page " + pageIndex +
													   " in XML input file.");
					throw new ApplicationException("Excess Pagesize elements for Page " + pageIndex +
												   " in XML input file. Only one Pagesize per Page is allowed.");
				}
				// Read and save the condition. This element IS nillable (it might be present).
				// Only one Pagesize element per Page is allowed if it is present.
				// TODO: Make Condition an attribute of Page.
				var cond = pageNode.SelectNodes("descendant::Condition");
				if (cond != null && cond.Count == 1)
				{
					pageDefinitionObject.Page[pageIndex].Condition = cond[0].InnerText;

					// TODO: Add code to validate the condition value.
				}
				else
				{
					if (cond == null || cond.Count <= 1)
						throw new ApplicationException("Missing required Condition element for Page " + pageIndex +
													   " in XML input file.");
					throw new ApplicationException("Excess Condition elements for Page " + pageIndex +
												   " in XML input file. Only one Condition per Page is allowed.");
				}
				// Read and save the Overflow value. This element IS nillable (it might be present).
				// If it is present, it must be 'true' or 'false'.
				// TODO: Make Overflow an attribute of Page.
				var over = pageNode.SelectNodes("descendant::Overflow");
				if (over != null && over.Count == 1)
				{
					pageDefinitionObject.Page[pageIndex].Overflow = Convert.ToBoolean(over[0].InnerText);
				}
				else if (over != null && over.Count > 1)
						throw new ApplicationException("Excess Overflow elements for Page " + pageIndex +
													   " in XML input file. Only one Overflow per Page is allowed.");
				// Read and save the image information, if it exists.
				// Only 1 image element may be present per Page, but it is not required. However, if an 
				// image element is present, all Image attributes are required.
				var imageElement = pageNode.SelectNodes("descendant::Image");
				if (imageElement != null && imageElement.Count == 1)
				{
					// Create a new Image for the current Page.
					pageDefinitionObject.Page[pageIndex].Image = new PagesPageImage();

					// Read the Page attribute(s).
					var imageAtt = imageElement[0].Attributes;

					// Save the Image attributes. These attributes are required.
					if (imageAtt != null && imageAtt.Count == 5)
					{
						// The Image name as a string.
						pageDefinitionObject.Page[pageIndex].Image.Imagename = imageAtt["Imagename"].Value;

						// The values must be an unsigned short integer.
						pageDefinitionObject.Page[pageIndex].Image.Xstart = Convert.ToUInt16(imageAtt["Xstart"].Value);
						pageDefinitionObject.Page[pageIndex].Image.XEnd = Convert.ToUInt16(imageAtt["XEnd"].Value);
						pageDefinitionObject.Page[pageIndex].Image.YStart = Convert.ToUInt16(imageAtt["YStart"].Value);
						pageDefinitionObject.Page[pageIndex].Image.YEnd = Convert.ToUInt16(imageAtt["YEnd"].Value);
					}
					else
					{
						if (imageAtt == null)
							throw new ApplicationException("Missing Image attributes for Image on Page " + pageIndex + " in XML input file.");
						throw new ApplicationException("Incorrect number of Image attributes for Image on Page " + pageIndex + " in XML input file.");
					}
				}
				else if (imageElement != null && imageElement.Count > 1)
					throw new ApplicationException("Excess Image elements for Page " + pageIndex +
												   " in XML input file. Only one Image element per Page is allowed.");
				// Determine if there is a Zones element. It is not required, 
				// but if it exists, there can only be a single instance.
				var zonesElement = pageNode.SelectNodes("descendant::Zones");
				if (zonesElement != null && zonesElement.Count == 1)
				{
					// Find all Zone elements.
					var zoneList = zonesElement[0].SelectNodes("descendant::Zone");

					// There must be a minimum of ONE Zone element.
					if (zoneList == null || zoneList.Count == 0)
					{
						throw new ApplicationException("Missing Zone element in XML input file. " +
													   "At least one is required when the Zones element is present.");
					}
					// Create a Zones array to hold the total number of Zones in the current Zones element.
					pageDefinitionObject.Page[pageIndex].Zones = new PagesPageZone[zoneList.Count];

					// Just to make it easier to read, save the Zones to a local variable.
					var zones = pageDefinitionObject.Page[pageIndex].Zones;

					// Set the initial index of the zone list.
					var zoneIndex = 0;

					// Save the information for each Zone.
					foreach (var zoneElement in zoneList)
					{
						// Add a new Zone to the list of Zones.
						var newZone = zones[zoneIndex] = new PagesPageZone();

						// Read the Zone attribute(s).
						var zoneAtt = ((XmlNode)zoneElement).Attributes;

						// Save the Zone attributes. These attributes are required.
						if (zoneAtt != null && zoneAtt.Count == 5)
						{
							// The Zone name as a string.
							newZone.Zonename = zoneAtt["Zonename"].Value;

							// The values must be an unsigned short integer.
							newZone.Xstart = Convert.ToUInt16(zoneAtt["Xstart"].Value);
							newZone.XEnd = Convert.ToUInt16(zoneAtt["XEnd"].Value);
							newZone.YStart = Convert.ToUInt16(zoneAtt["YStart"].Value);
							newZone.YEnd = Convert.ToUInt16(zoneAtt["YEnd"].Value);

							// Determine if there is a Parts element.  It is not required, 
							// but if it exists, there can only be a single instance.
							var partsElement = ((XmlNode)zoneElement).SelectNodes("descendant::Parts");
							if (partsElement != null && partsElement.Count == 1)
							{
								// Find all Part elements.
								var partList = partsElement[0].SelectNodes("descendant::Part");

								// There must be a minimum of ONE Part element.
								if (partList == null || partList.Count == 0)
								{
									throw new ApplicationException("Missing Part element in XML input file. " +
												"At least one is required when the Parts element is present.");
								}
								// Create a Zones array to hold the total number of Zones in the current Zones element.
								newZone.Parts = new PagesPageZonePart[partList.Count];

								// Set the initial index of the Part list.
								var partIndex = 0;

								// Loop through each Part and save the attributes.
								foreach (var partElement in partList)
								{
									// Add a new Part to the list of Parts.
									var newPart = newZone.Parts[partIndex] = new PagesPageZonePart();

									// Read the Part attribute(s).
									var partAtt = ((XmlNode)partElement).Attributes;

									// Save the Part attributes. These attributes are required.
									if (partAtt != null && partAtt.Count == 5)
									{
										// The Zone name as a string.
										newPart.Partname = partAtt["Partname"].Value;

										// The values must be an unsigned short integer.
										newPart.Xstart = Convert.ToUInt16(partAtt["Xstart"].Value);
										newPart.XEnd = Convert.ToUInt16(partAtt["XEnd"].Value);
										newPart.YStart = Convert.ToUInt16(partAtt["YStart"].Value);
										newPart.YEnd = Convert.ToUInt16(partAtt["YEnd"].Value);
									}
									else
									{
										throw new ApplicationException("Missing Part attributes for Part on Page " + pageIndex + " in XML input file.");
									}
									// Increment the Part index.
									partIndex++;
								}
							}
							else if (partsElement != null && partsElement.Count > 1)
								throw new ApplicationException("Excess Parts elements for Page " + pageIndex +
											" in XML input file. Only one Parts element per Zone is allowed.");
						}
						else
						{
							throw new ApplicationException("Missing Zone attributes for Zone on Page " + pageIndex + " in XML input file.");
						}
						// Increment the Zone index.
						zoneIndex++;
					}
				}
				else if (zonesElement != null && zonesElement.Count > 1)
					throw new ApplicationException("Excess Zones elements for Page " + pageIndex +
												   " in XML input file. Only one Zones element per Page is allowed.");
				// Increment the page index.
				pageIndex++;
			}
		}

		private static void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			switch (e.Severity)
			{
				case XmlSeverityType.Error:
					Console.WriteLine("Error: {0}", e.Message);
					break;
				case XmlSeverityType.Warning:
					Console.WriteLine("Warning {0}", e.Message);
					break;
			}

		}
		/// <summary>
		/// Checks the Customer Data file for completeness.
		/// </summary>
		/// <param name="inputFilename">The file to check.</param>
		/// <returns>'true' if valid. 'false' or an Exception if invalid.</returns>
		/// <remarks>
		/// We are allowing one file type for the time being, XML. A schema 
		/// will be forthcoming. Other file types may be added later. 
		/// </remarks>
		private bool CheckForValidCustomerDataFile(string inputFilename)
		{
			// We know that the file exists because that was checked in the 
			// IsValidInput method, so we need to create a CustomerData object.
			CustomerDataObject = new Invoices();

			try
			{
				// Save the input filename in case we need it later.
				CustomerDataObject.InputFilename = inputFilename;
			}
			catch (Exception ex)
			{
				throw new ApplicationException(@"Error in CheckForValidCustomerDataFile", ex.InnerException);
			}
			return true;
		}

		/// <summary>
		/// Checks the Content file for completeness.
		/// </summary>
		/// <param name="inputFilename">The file to check.</param>
		/// <returns>'true' if valid. 'false' or an Exception if invalid.</returns>
		private bool CheckForValidContentFile(string inputFilename)
		{
			// We know that the file exists because that was checked in the 
			// IsValidInput method, so we need to create a Content object.
			ContentObject = new Content();

			try
			{
				// Save the input filename in case we need it later.
				ContentObject.InputFilename = inputFilename;

				// Read each line of the file into a string array. Each 
				// element of the array is one line of the file.
				var lines = File.ReadAllLines(inputFilename);

				// Verify that we have lines of data.
				if (!lines.Any()) throw new ApplicationException("ERROR: No data read from content file, " + inputFilename);
				
				// Save the array of lines in case we need it again.
				ContentObject.FileData = lines;

				// Call the method to fill the object.
				ReadContent(lines, ContentObject);

				// Return if no exceptions.
				return true;
			}
			catch (Exception ex)
			{
				throw new ApplicationException(@"Error in CheckForValidContentFile: " + ex.Message) ;
			}
		}

		/// <summary>
		/// Reads each line of the content file and populates the Content object.
		/// </summary>
		/// <param name="lines"></param>
		/// <param name="contentObject"></param>
		private void ReadContent(string[] lines, Content contentObject)
		{
			// Parse each line in the lines array and populate the object.
			foreach (var line in lines)
			{
				// Find the locations of the first '|' in the line.
				// TODO: Make all delimiters configurable in Settings.
				var firstPipeLoc = line.IndexOf("|", StringComparison.Ordinal);

				// Extract the index of the first delimiter found in the line.
				var index = line.Substring(0, firstPipeLoc);

				// Extract the parts of the index. Initially there will be 3 parts to the index, the Page Number,
				// the Zone Number, and the Part Number, each separated by a delimiter.
				// NOTE: Even though I refer to page NUMBER and zone NUMBER and part NUMBER, the actual values may be 
				//		 alpha, alphanumeric, or numeric. Therefore, I leave the values unchanged after extracting them
				//		 from the content input file.
				// TODO: Make all delimiters configurable in Settings.

				// Extract the page number starting at the first character 
				// and ending before the first delimiter in the index string.
				var pageNumber = index.Substring(0, index.IndexOf("_", StringComparison.Ordinal));

				// Extract the zone number starting at the first character after the first delimiter
				// and ending at the character before the second, or last, delimiter.
				var zoneNumber = index.Substring(index.IndexOf("_", StringComparison.Ordinal) + 1,
					index.LastIndexOf("_", StringComparison.Ordinal) - index.IndexOf("_", StringComparison.Ordinal) - 1);

				// Extract the part number starting at the first character after the second delimiter
				// and ending at the last character in the index string.
				var partNumber = index.Substring(index.LastIndexOf("_", StringComparison.Ordinal) + 1);

				// If we have all three parts of the index, create and add a ContentItem to the list in the Content object.
				contentObject.ContentItems.Add(new ContentItem(pageNumber, zoneNumber, partNumber, new ContentValue(line.Substring(firstPipeLoc))));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Work()
		{
			//
			var dest = Settings.Default.OutputLocation + "\\" + "12345678.pdf";

			//Initialize PDF writer
			PdfWriter writer = new PdfWriter(dest);
			//Initialize PDF document
			PdfDocument pdf = new PdfDocument(writer);
			// Initialize document
			Document document = new Document(pdf);
			//Add paragraph to the document
			var para = new Paragraph("Hello World!");
			para.SetTextAlignment(TextAlignment.CENTER);
			document.Add(para);
			para = new Paragraph("Hello World! For the second time!!");
			para.SetTextAlignment(TextAlignment.LEFT);
			document.Add(para);
			para = new Paragraph("Hello World! For the third time!!!");
			para.SetTextAlignment(TextAlignment.RIGHT);
			document.Add(para);
			para = new Paragraph("Hello World! For the fourth time!!!!");
			para.SetTextAlignment(TextAlignment.JUSTIFIED_ALL);
			document.Add(para);
			// Add an image to the document.
			var imagePath = Settings.Default.ImageFilesLocation;
			var fox = new Image(ImageDataFactory.Create(imagePath + "\\" + "desert.jpg"));
			fox.Scale((float).25, (float).25);
			var dog = new Image(ImageDataFactory.Create(imagePath + "\\" + "koala.jpg"));
			dog.Scale((float).25, (float).25);
			var p = new Paragraph()
						.Add(fox)
						.Add(dog);
			document.Add(p);
			//Close document
			document.Close();
		}
		#endregion Methods
	}
}
