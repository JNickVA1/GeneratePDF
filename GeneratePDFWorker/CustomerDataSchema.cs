using System.Xml.Serialization;

namespace GeneratePDFWorker
{
	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCode("xsd", "4.6.1055.0")]
	[System.Serializable()]
	[System.Diagnostics.DebuggerStepThrough()]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType=true)]
	[XmlRoot(Namespace="", IsNullable=false)]
	internal class Invoices : InputData
	{
		#region Properties
		#endregion Properties

		private Invoice[] _invoiceField;
	
		/// <remarks/>
		[XmlElement("Invoice")]
		public Invoice[] Invoice {
			get {
				return _invoiceField;
			}
			set {
				_invoiceField = value;
			}
		}
	}

	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
	[System.Serializable()]
	[System.Diagnostics.DebuggerStepThrough()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[XmlType(AnonymousType=true)]
	public class Invoice {
	
		private byte _idField;
	
		private string _transactionDateField;
	
		private string _languageField;
	
		private string _companyField;
	
		private string _add1Field;
	
		private string _cityField;
	
		private string _stateField;
	
		private ushort _zIpField;
	
		private string _billtoCompanyField;
	
		private string _billtoadd1Field;
	
		private string _billtoCityField;
	
		private string _billtoStateField;
	
		private ushort _billtoZipField;
	
		private string _shiptoCompanyField;
	
		private string _shiptoadd1Field;
	
		private string _shiptoCityField;
	
		private string _shiptoStateField;
	
		private ushort _shiptoZipField;
	
		private string _smField;
	
		private InvoiceItem[] _itemsField;
	
		private decimal _gStField;
	
		private string _totalField;
	
		/// <remarks/>
		public byte Id {
			get {
				return _idField;
			}
			set {
				_idField = value;
			}
		}
	
		/// <remarks/>
		public string TransactionDate {
			get {
				return _transactionDateField;
			}
			set {
				_transactionDateField = value;
			}
		}
	
		/// <remarks/>
		public string Language {
			get {
				return _languageField;
			}
			set {
				_languageField = value;
			}
		}
	
		/// <remarks/>
		public string Company {
			get {
				return _companyField;
			}
			set {
				_companyField = value;
			}
		}
	
		/// <remarks/>
		public string Add1 {
			get {
				return _add1Field;
			}
			set {
				_add1Field = value;
			}
		}
	
		/// <remarks/>
		public string City {
			get {
				return _cityField;
			}
			set {
				_cityField = value;
			}
		}
	
		/// <remarks/>
		public string State {
			get {
				return _stateField;
			}
			set {
				_stateField = value;
			}
		}
	
		/// <remarks/>
		public ushort Zip {
			get {
				return _zIpField;
			}
			set {
				_zIpField = value;
			}
		}
	
		/// <remarks/>
		public string BilltoCompany {
			get {
				return _billtoCompanyField;
			}
			set {
				_billtoCompanyField = value;
			}
		}
	
		/// <remarks/>
		public string Billtoadd1 {
			get {
				return _billtoadd1Field;
			}
			set {
				_billtoadd1Field = value;
			}
		}
	
		/// <remarks/>
		public string BilltoCity {
			get {
				return _billtoCityField;
			}
			set {
				_billtoCityField = value;
			}
		}
	
		/// <remarks/>
		public string BilltoState {
			get {
				return _billtoStateField;
			}
			set {
				_billtoStateField = value;
			}
		}
	
		/// <remarks/>
		public ushort BilltoZip {
			get {
				return _billtoZipField;
			}
			set {
				_billtoZipField = value;
			}
		}
	
		/// <remarks/>
		public string ShiptoCompany {
			get {
				return _shiptoCompanyField;
			}
			set {
				_shiptoCompanyField = value;
			}
		}
	
		/// <remarks/>
		public string Shiptoadd1 {
			get {
				return _shiptoadd1Field;
			}
			set {
				_shiptoadd1Field = value;
			}
		}
	
		/// <remarks/>
		public string ShiptoCity {
			get {
				return _shiptoCityField;
			}
			set {
				_shiptoCityField = value;
			}
		}
	
		/// <remarks/>
		public string ShiptoState {
			get {
				return _shiptoStateField;
			}
			set {
				_shiptoStateField = value;
			}
		}
	
		/// <remarks/>
		public ushort ShiptoZip {
			get {
				return _shiptoZipField;
			}
			set {
				_shiptoZipField = value;
			}
		}
	
		/// <remarks/>
		public string Sm {
			get {
				return _smField;
			}
			set {
				_smField = value;
			}
		}
	
		/// <remarks/>
		[XmlArrayItem("item", IsNullable=false)]
		public InvoiceItem[] Items {
			get {
				return _itemsField;
			}
			set {
				_itemsField = value;
			}
		}
	
		/// <remarks/>
		public decimal Gst {
			get {
				return _gStField;
			}
			set {
				_gStField = value;
			}
		}
	
		/// <remarks/>
		public string Total {
			get {
				return _totalField;
			}
			set {
				_totalField = value;
			}
		}
	}

	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
	[System.Serializable()]
	[System.Diagnostics.DebuggerStepThrough()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[XmlType(AnonymousType=true)]
	public class InvoiceItem {
	
		private string _descriptionField;
	
		private string _unitPriceField;
	
		/// <remarks/>
		public string Description {
			get {
				return _descriptionField;
			}
			set {
				_descriptionField = value;
			}
		}
	
		/// <remarks/>
		public string UnitPrice {
			get {
				return _unitPriceField;
			}
			set {
				_unitPriceField = value;
			}
		}
	}
}