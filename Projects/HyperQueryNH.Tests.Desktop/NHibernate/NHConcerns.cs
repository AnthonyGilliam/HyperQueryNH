using System;
using System.Configuration;

using NUnit.Framework;

using HyperQueryNH.Core;
using HyperQueryNH.Tests.Model;

namespace HyperQueryNH.Tests.Desktop.NHibernate
{
	public abstract class NHConcerns
	{
		protected readonly string ConnectionString = ConfigurationManager.ConnectionStrings["TestDatabase"].ConnectionString;
		protected IUnitOfWork<Guid> UnitOfWork;
		[SetUp]
		protected void InitiateTest()
		{
			//Initialize Static Session
			var staticModelAssemblyHeldInSession = new Customer().GetType().Assembly;
			NHSession.Init(staticModelAssemblyHeldInSession);

			//Reset database
			NHSession.DropAllTables();
			NHSession.ExportModelToDatabase();
			UnitOfWork = new UnitOfWork<Guid>(NHSession.CurrentSession);
		}

		[TearDown]
		public void FixtureTearDown()
		{
			//Reset database
			NHSession.Disconnect();
			NHSession.DropAllTables();
			NHSession.ExportModelToDatabase();
		}
	}
}
