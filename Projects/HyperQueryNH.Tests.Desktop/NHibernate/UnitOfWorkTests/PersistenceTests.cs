using System;
using System.Data;
using System.Data.SqlServerCe;
using HyperQueryNH.Utilities;
using NUnit.Framework;

using DataGenerationEngine.MockServices;
using HyperQueryNH.Core;
using HyperQueryNH.Tests.Model;

namespace HyperQueryNH.Tests.Desktop.NHibernate.UnitOfWorkTests.PersistenceTests
{
	[TestFixture]
	public class When_persisting_a_single_mapped_object_to_a_data_source : NHConcerns
	{
		private Customer _customer;
		private const string CUSTOMER_QUERY = "SELECT *" +
		                                      "FROM tblCustomers";

		[SetUp]
		public void Context()
		{
			_customer = MockService.GetMockedObject<Customer>();
			_customer.DateOfCreation = DateTime.Now;
			_customer.CustomerDiscount = 5;
			_customer.Address = null;
			_customer.Orders = null;
			UnitOfWork.AddToSession(_customer);
			NHSession.CommitSession();
		}

		[Test]
		public void The_object_is_successfully_saved()
		{
			Customer customer = null;

			using (var conn = new SqlCeConnection(ConnectionString))
			{
				conn.Open();
				var command = conn.CreateCommand();
				command.CommandText = "tblCustomers";
				command.CommandType = CommandType.TableDirect;
				var dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					customer = new Customer
									{
										ID = (Guid)dataReader.GetValue(0),
										DateOfCreation = (DateTime)dataReader.GetValue(1),
										FirstName = (string)dataReader.GetValue(2),
										MiddleInitial = ((string)dataReader.GetValue(3))[0],
										LastName = (string)dataReader.GetValue(4),
										WebAddress = (string)dataReader.GetValue(5),
										Email = (string)dataReader.GetValue(6),
										Establishment = (string)dataReader.GetValue(7),
										CustomerDiscount = double.Parse(dataReader.GetValue(8).ToString()),
										DateSubscribed = (DateTime)dataReader.GetValue(9)
									};
				}

				dataReader.Close();
			}

			HyperAssert.ThatPropertiesAreEqual(_customer, customer);
		}
	}

	[TestFixture]
	public class When_deleting_a_single_mapped_object_to_a_data_source : NHConcerns
	{
		private Customer _customer;
		private Customer _savedCustomer;
		private Customer _deletedCustomer;

		[SetUp]
		public void Context()
		{
			_customer = MockService.GetMockedObject<Customer>();
			NHSession.CurrentSession.Save(_customer);
			NHSession.CommitSession();
            //TODO: Need to clear session before re-querying customer
			_savedCustomer = NHSession.CurrentSession.Get<Customer>(_customer.ID);
			
            UnitOfWork.Delete(_savedCustomer);
			NHSession.CommitSession();

            NHSession.CurrentSession.Reconnect();
			_deletedCustomer = NHSession.CurrentSession.Get<Customer>(_customer.ID);
		}

		[Test]
		public void The_object_exsisted_in_the_database_befoer_deleting()
		{
			HyperAssert.ThatPropertiesAreEqual(_customer, _savedCustomer);
		}

		[Test]
		public void The_object_is_successfully_deleted()
		{
			Assert.IsNull(_deletedCustomer);
		}
	}
}
