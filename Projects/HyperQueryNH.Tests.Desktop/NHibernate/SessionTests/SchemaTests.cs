using System.Configuration;
using System.Data;
using System.Data.SqlServerCe;
using NUnit.Framework;

using HyperQueryNH.Core;
using HyperQueryNH.Tests.Model;

namespace HyperQueryNH.Tests.Desktop.NHibernate.SessionTests.SchemaTests
{
	[TestFixture]
	public class When_creating_data_tables_from_mapped_objects
	{
		private readonly string _connectionString = ConfigurationManager.ConnectionStrings["TestDatabase"].ConnectionString;
		private DataTable _schemaInfo;

		[SetUp]
		public void Context()
		{
			//Initialize Static Session
			var staticModelAssemblyHeldInSession = new Customer().GetType().Assembly;
			NHSession.Init(staticModelAssemblyHeldInSession);
		}

		[Test]
		public void A_connection_can_be_established_with_datastore()
		{
			using (SqlCeConnection conn = new SqlCeConnection(_connectionString))
			{
				conn.Open();

				_schemaInfo = conn.GetSchema();
			}

			Assert.That(_schemaInfo.IsInitialized && !_schemaInfo.HasErrors);
		}

		[Test]
		public void All_previously_existing_tables_can_be_dropped()
		{
			NHSession.DropAllTables();
			
			using (SqlCeConnection conn = new SqlCeConnection(_connectionString))
			{
				conn.Open();

				_schemaInfo = conn.GetSchema("Tables");
			}

			Assert.That(_schemaInfo.Rows.Count == 0);
		}

		[Test]
		public void The_data_tables_are_created_upon_schema_export()
		{
			NHSession.DropAllTables();
			NHSession.ExportModelToDatabase();

			using (SqlCeConnection conn = new SqlCeConnection(_connectionString))
			{
				conn.Open();

				_schemaInfo = conn.GetSchema("Tables");
			}

			Assert.That(_schemaInfo.Rows.Count == 1);
		}

		[TearDown]
		public void TearDown()
		{
			NHSession.Disconnect();
		}
	}
}
