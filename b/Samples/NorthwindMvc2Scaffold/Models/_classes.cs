


using BoC.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
namespace NorthwindMvcScaffold.Models
{   
    
    /// <summary>
    /// A class which represents the Order Details table in the Northwind Database.
    /// </summary>
	public partial class OrderDetail: BaseEntity<int>
	{
		[Required] //isPK: True
		[Association(Name="FK_Order_Details_Orders", ThisKey="Id", OtherKey="Id", IsForeignKey = true)]
		virtual public Order Order {get; set;}
		[Required] //isPK: True
		[Association(Name="FK_Order_Details_Products", ThisKey="Id", OtherKey="Id", IsForeignKey = true)]
		virtual public Product Product {get; set;}
		[Required] //isPK: False
		virtual public decimal UnitPrice { get; set; }
		[Required] //isPK: False
		virtual public short Quantity { get; set; }
		[Required] //isPK: False
		virtual public decimal Discount { get; set; }


	}
	
    
    /// <summary>
    /// A class which represents the CustomerCustomerDemo table in the Northwind Database.
    /// </summary>
	public partial class CustomerCustomerDemo: BaseEntity<string>
	{
		[Required] //isPK: True
		[Association(Name="FK_CustomerCustomerDemo_Customers", ThisKey="Id", OtherKey="Id", IsForeignKey = true)]
		virtual public Customer Customer {get; set;}
		[Required] //isPK: True
		[Association(Name="FK_CustomerCustomerDemo", ThisKey="Id", OtherKey="Id", IsForeignKey = true)]
		virtual public CustomerDemographic CustomerDemographic {get; set;}


	}
	
    
    /// <summary>
    /// A class which represents the CustomerDemographics table in the Northwind Database.
    /// </summary>
	public partial class CustomerDemographic: BaseEntity<string>
	{
		virtual public string CustomerDesc { get; set; }

        ICollection<CustomerCustomerDemo> _CustomerCustomerDemos = new HashSet<CustomerCustomerDemo>();
		[Association(Name="FK_CustomerCustomerDemo", ThisKey="Id", OtherKey="Id",IsForeignKey = true)]
        virtual public ICollection<CustomerCustomerDemo> CustomerCustomerDemos
        {
            get
            {
				return _CustomerCustomerDemos;
            }
			protected set
			{
				_CustomerCustomerDemos = value;
			}
        }


	}
	
    
    /// <summary>
    /// A class which represents the Region table in the Northwind Database.
    /// </summary>
	public partial class Region: BaseEntity<int>
	{
		[Required] //isPK: False
		virtual public string RegionDescription { get; set; }

        ICollection<Territory> _Territories = new HashSet<Territory>();
		[Association(Name="FK_Territories_Region", ThisKey="Id", OtherKey="Id",IsForeignKey = true)]
        virtual public ICollection<Territory> Territories
        {
            get
            {
				return _Territories;
            }
			protected set
			{
				_Territories = value;
			}
        }


	}
	
    
    /// <summary>
    /// A class which represents the Territories table in the Northwind Database.
    /// </summary>
	public partial class Territory: BaseEntity<string>
	{
		[Required] //isPK: False
		virtual public string TerritoryDescription { get; set; }
		[Required] //isPK: False
		[Association(Name="FK_Territories_Region", ThisKey="Id", OtherKey="Id", IsForeignKey = true)]
		virtual public Region Region {get; set;}

        ICollection<EmployeeTerritory> _EmployeeTerritories = new HashSet<EmployeeTerritory>();
		[Association(Name="FK_EmployeeTerritories_Territories", ThisKey="Id", OtherKey="Id",IsForeignKey = true)]
        virtual public ICollection<EmployeeTerritory> EmployeeTerritories
        {
            get
            {
				return _EmployeeTerritories;
            }
			protected set
			{
				_EmployeeTerritories = value;
			}
        }


	}
	
    
    /// <summary>
    /// A class which represents the EmployeeTerritories table in the Northwind Database.
    /// </summary>
	public partial class EmployeeTerritory: BaseEntity<int>
	{
		[Required] //isPK: True
		[Association(Name="FK_EmployeeTerritories_Employees", ThisKey="Id", OtherKey="Id", IsForeignKey = true)]
		virtual public Employee Employee {get; set;}
		[Required] //isPK: True
		[Association(Name="FK_EmployeeTerritories_Territories", ThisKey="Id", OtherKey="Id", IsForeignKey = true)]
		virtual public Territory Territory {get; set;}


	}
	
    
    /// <summary>
    /// A class which represents the Employees table in the Northwind Database.
    /// </summary>
	public partial class Employee: BaseEntity<int>
	{
		[Required] //isPK: False
		virtual public string LastName { get; set; }
		[Required] //isPK: False
		virtual public string FirstName { get; set; }
		virtual public string Title { get; set; }
		virtual public string TitleOfCourtesy { get; set; }
		virtual public DateTime? BirthDate { get; set; }
		virtual public DateTime? HireDate { get; set; }
		virtual public string Address { get; set; }
		virtual public string City { get; set; }
		virtual public string Region { get; set; }
		virtual public string PostalCode { get; set; }
		virtual public string Country { get; set; }
		virtual public string HomePhone { get; set; }
		virtual public string Extension { get; set; }
		virtual public byte[] Photo { get; set; }
		virtual public string Notes { get; set; }
		[Association(Name="FK_Employees_Employees", ThisKey="Id", OtherKey="Id", IsForeignKey = true)]
		virtual public Employee ReportsTo {get; set;}
		virtual public string PhotoPath { get; set; }

        ICollection<EmployeeTerritory> _EmployeeTerritories = new HashSet<EmployeeTerritory>();
		[Association(Name="FK_EmployeeTerritories_Employees", ThisKey="Id", OtherKey="Id",IsForeignKey = true)]
        virtual public ICollection<EmployeeTerritory> EmployeeTerritories
        {
            get
            {
				return _EmployeeTerritories;
            }
			protected set
			{
				_EmployeeTerritories = value;
			}
        }

        ICollection<Order> _Orders = new HashSet<Order>();
		[Association(Name="FK_Orders_Employees", ThisKey="Id", OtherKey="Id",IsForeignKey = true)]
        virtual public ICollection<Order> Orders
        {
            get
            {
				return _Orders;
            }
			protected set
			{
				_Orders = value;
			}
        }


	}
	
    
    /// <summary>
    /// A class which represents the Categories table in the Northwind Database.
    /// </summary>
	public partial class Category: BaseEntity<int>
	{
		[Required] //isPK: False
		virtual public string CategoryName { get; set; }
		virtual public string Description { get; set; }
		virtual public byte[] Picture { get; set; }

        ICollection<Product> _Products = new HashSet<Product>();
		[Association(Name="FK_Products_Categories", ThisKey="Id", OtherKey="Id",IsForeignKey = true)]
        virtual public ICollection<Product> Products
        {
            get
            {
				return _Products;
            }
			protected set
			{
				_Products = value;
			}
        }


	}
	
    
    /// <summary>
    /// A class which represents the Customers table in the Northwind Database.
    /// </summary>
	public partial class Customer: BaseEntity<string>
	{
		[Required] //isPK: False
		virtual public string CompanyName { get; set; }
		virtual public string ContactName { get; set; }
		virtual public string ContactTitle { get; set; }
		virtual public string Address { get; set; }
		virtual public string City { get; set; }
		virtual public string Region { get; set; }
		virtual public string PostalCode { get; set; }
		virtual public string Country { get; set; }
		virtual public string Phone { get; set; }
		virtual public string Fax { get; set; }

        ICollection<CustomerCustomerDemo> _CustomerCustomerDemos = new HashSet<CustomerCustomerDemo>();
		[Association(Name="FK_CustomerCustomerDemo_Customers", ThisKey="Id", OtherKey="Id",IsForeignKey = true)]
        virtual public ICollection<CustomerCustomerDemo> CustomerCustomerDemos
        {
            get
            {
				return _CustomerCustomerDemos;
            }
			protected set
			{
				_CustomerCustomerDemos = value;
			}
        }

        ICollection<Order> _Orders = new HashSet<Order>();
		[Association(Name="FK_Orders_Customers", ThisKey="Id", OtherKey="Id",IsForeignKey = true)]
        virtual public ICollection<Order> Orders
        {
            get
            {
				return _Orders;
            }
			protected set
			{
				_Orders = value;
			}
        }


	}
	
    
    /// <summary>
    /// A class which represents the Shippers table in the Northwind Database.
    /// </summary>
	public partial class Shipper: BaseEntity<int>
	{
		[Required] //isPK: False
		virtual public string CompanyName { get; set; }
		virtual public string Phone { get; set; }

        ICollection<Order> _Orders = new HashSet<Order>();
		[Association(Name="FK_Orders_Shippers", ThisKey="Id", OtherKey="Id",IsForeignKey = true)]
        virtual public ICollection<Order> Orders
        {
            get
            {
				return _Orders;
            }
			protected set
			{
				_Orders = value;
			}
        }


	}
	
    
    /// <summary>
    /// A class which represents the Suppliers table in the Northwind Database.
    /// </summary>
	public partial class Supplier: BaseEntity<int>
	{
		[Required] //isPK: False
		virtual public string CompanyName { get; set; }
		virtual public string ContactName { get; set; }
		virtual public string ContactTitle { get; set; }
		virtual public string Address { get; set; }
		virtual public string City { get; set; }
		virtual public string Region { get; set; }
		virtual public string PostalCode { get; set; }
		virtual public string Country { get; set; }
		virtual public string Phone { get; set; }
		virtual public string Fax { get; set; }
		virtual public string HomePage { get; set; }

        ICollection<Product> _Products = new HashSet<Product>();
		[Association(Name="FK_Products_Suppliers", ThisKey="Id", OtherKey="Id",IsForeignKey = true)]
        virtual public ICollection<Product> Products
        {
            get
            {
				return _Products;
            }
			protected set
			{
				_Products = value;
			}
        }


	}
	
    
    /// <summary>
    /// A class which represents the Orders table in the Northwind Database.
    /// </summary>
	public partial class Order: BaseEntity<int>
	{
		[Association(Name="FK_Orders_Customers", ThisKey="Id", OtherKey="Id", IsForeignKey = true)]
		virtual public Customer Customer {get; set;}
		[Association(Name="FK_Orders_Employees", ThisKey="Id", OtherKey="Id", IsForeignKey = true)]
		virtual public Employee Employee {get; set;}
		virtual public DateTime? OrderDate { get; set; }
		virtual public DateTime? RequiredDate { get; set; }
		virtual public DateTime? ShippedDate { get; set; }
		[Association(Name="FK_Orders_Shippers", ThisKey="Id", OtherKey="Id", IsForeignKey = true)]
		virtual public Shipper ShipVia {get; set;}
		virtual public decimal? Freight { get; set; }
		virtual public string ShipName { get; set; }
		virtual public string ShipAddress { get; set; }
		virtual public string ShipCity { get; set; }
		virtual public string ShipRegion { get; set; }
		virtual public string ShipPostalCode { get; set; }
		virtual public string ShipCountry { get; set; }

        ICollection<OrderDetail> _OrderDetails = new HashSet<OrderDetail>();
		[Association(Name="FK_Order_Details_Orders", ThisKey="Id", OtherKey="Id",IsForeignKey = true)]
        virtual public ICollection<OrderDetail> OrderDetails
        {
            get
            {
				return _OrderDetails;
            }
			protected set
			{
				_OrderDetails = value;
			}
        }


	}
	
    
    /// <summary>
    /// A class which represents the Products table in the Northwind Database.
    /// </summary>
	public partial class Product: BaseEntity<int>
	{
		[Required] //isPK: False
		virtual public string ProductName { get; set; }
		[Association(Name="FK_Products_Suppliers", ThisKey="Id", OtherKey="Id", IsForeignKey = true)]
		virtual public Supplier Supplier {get; set;}
		[Association(Name="FK_Products_Categories", ThisKey="Id", OtherKey="Id", IsForeignKey = true)]
		virtual public Category Category {get; set;}
		virtual public string QuantityPerUnit { get; set; }
		virtual public decimal? UnitPrice { get; set; }
		virtual public short? UnitsInStock { get; set; }
		virtual public short? UnitsOnOrder { get; set; }
		virtual public short? ReorderLevel { get; set; }
		[Required] //isPK: False
		virtual public bool Discontinued { get; set; }

        ICollection<OrderDetail> _OrderDetails = new HashSet<OrderDetail>();
		[Association(Name="FK_Order_Details_Products", ThisKey="Id", OtherKey="Id",IsForeignKey = true)]
        virtual public ICollection<OrderDetail> OrderDetails
        {
            get
            {
				return _OrderDetails;
            }
			protected set
			{
				_OrderDetails = value;
			}
        }


	}
	
}