using System;
using System.Data;
using System.Reflection;
using System.Web;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

using HyperQueryNH.Core.Enums;

namespace HyperQueryNH.Core
{
    public static class FNHSession
    {
        private const string NHIBERNATE_SESSION_KEY = "__NHibernateSession";

        private static ISession _session;
        private static ISessionFactory _sessionFactory;
		private static IPersistenceConfigurer _databaseConfig;
		private static Configuration _nHibernateConfiguration;
		private static FluentConfiguration _sessionFactoryConfiguration;
    	private static PlatformEnvironment _platformEnvironment;

    	//TODO:  Use overrides to wire up Fluent Automapping to work.
		public static void Init(IPersistenceConfigurer databaseConfiguration
			, Assembly classMapAssembly
			, Assembly autoMapAssembly = null
			, Assembly autoMappingOverrideAssembly = null
			, object baseClass = null
			, Type[] otherClassObjectsToMap = null
			, bool ignoreBase = false
			, SessionDefaultCascadeMode defaultCascade = SessionDefaultCascadeMode.SaveUpdate
			, bool defaultLazyLoad = true)
        {

		}

		public static void Init(FluentConfiguration sessionFactoryconfiguration)
		{
			_sessionFactoryConfiguration = sessionFactoryconfiguration;
		}

		/// <summary>
		/// Describes the way the NHibernate Session object is stored.
		/// </summary>
        public static PlatformEnvironment Environment
        {
            get
            {
				if(_platformEnvironment != PlatformEnvironment.None)
				{
					return _platformEnvironment;
				}

                return System.Web.HttpContext.Current == null
                    ? PlatformEnvironment.Desktop
                    : PlatformEnvironment.Browser;
            }
        }
		
		public static ISession CurrentSession
        {
            get 
            {
                switch (Environment)
                {
                    case PlatformEnvironment.Desktop:
                        if (_session == null)
                        {
                            _session = CreateSession();
                        }

                        return _session;

                    case PlatformEnvironment.Browser:
                        ISession requestSession = HttpContext.Current.Items[NHIBERNATE_SESSION_KEY] as ISession;

                        if (requestSession == null)
                        {
                            requestSession = CreateSession();
                            HttpContext.Current.Items[NHIBERNATE_SESSION_KEY] = requestSession;
                        }

                        return requestSession;

                    default :
                        goto case PlatformEnvironment.Desktop;
                }
            }
        }

        public static void CommitSession()
        {
            if (CurrentSession != null && CurrentSession.IsOpen)
            {
                Reconnect();

                using (ITransaction tx = CurrentSession.BeginTransaction())
                {
                    try
                    {
                        if (!TransactionFailed)
                        {
                            CurrentSession.Flush();
                            tx.Commit();
                        }
                        else
                        {
                            tx.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    }
                    finally
                    {
                        Disconnect();
                    }
                }
            }
        }

        public static void ClearSession()
        {
            var session = CurrentSession;

            if (session != null && session.IsOpen)
            {
                session.Clear();
            }
        }

        public static void CloseSession()
        {
            var session = CurrentSession;

            if (session != null && session.IsOpen)
            {
                session.Clear();
                session.Close();

                session.Dispose();
            }

            if (Environment == PlatformEnvironment.Browser)
                HttpContext.Current.Items[NHIBERNATE_SESSION_KEY] = null;
            else
                _session = null;
        }

        public static bool TransactionFailed { get; set; }

		/// <summary>
		/// FOR TEST ONLY! Allows the session to act as if it is held in an HttpContext durring tests.
		/// </summary>
		public static void MockBrowserEnvironment()
		{
			_platformEnvironment = PlatformEnvironment.Browser;
		}

		/// <summary>
		/// For test ONLY! Clears the static Platform Environment variable after it has been mocked.
		/// </summary>
		public static void ClearPlatformEnvironment()
		{
			_platformEnvironment = PlatformEnvironment.None;
		}

		/// <summary>
		/// MADE FOR TESTING PURPOSES ONLY!!! Removes all foreign keys, then DELETES THE ENTIRE DATABASE!
		/// Requires updates for new NHibernate drivers
		/// </summary>
		public static void DropAllTables()
		{
			if (_nHibernateConfiguration == null)
			{
				throw new NullReferenceException("The current NHibernate Configuration has not been set.");
			}

			Reconnect();

			IDbCommand dropForeignKeys = CurrentSession.Connection.CreateCommand();
			dropForeignKeys.CommandText = @"
				/* Drop all Foreign Key constraints */
				DECLARE @name VARCHAR(128)
				DECLARE @constraint VARCHAR(254)
				DECLARE @SQL VARCHAR(254)

				SELECT @name = (SELECT TOP 1 TABLE_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'FOREIGN KEY' ORDER BY TABLE_NAME)

				WHILE @name is not null
				BEGIN
					SELECT @constraint = (SELECT TOP 1 CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'FOREIGN KEY' AND TABLE_NAME = @name ORDER BY CONSTRAINT_NAME)
					WHILE @constraint IS NOT NULL
					BEGIN
						SELECT @SQL = 'ALTER TABLE [dbo].[' + RTRIM(@name) +'] DROP CONSTRAINT ' + RTRIM(@constraint)
						EXEC (@SQL)
						PRINT 'Dropped FK Constraint: ' + @constraint + ' on ' + @name
						SELECT @constraint = (SELECT TOP 1 CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'FOREIGN KEY' AND CONSTRAINT_NAME <> @constraint AND TABLE_NAME = @name ORDER BY CONSTRAINT_NAME)
					END
				SELECT @name = (SELECT TOP 1 TABLE_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'FOREIGN KEY' ORDER BY TABLE_NAME)
				END";

			switch (_nHibernateConfiguration.Properties["connection.driver_class"])
			{
				case "NHibernate.Driver.SqlClientDriver":
					using (ITransaction tx = CurrentSession.BeginTransaction())
					{
						try
						{
							tx.Enlist(dropForeignKeys);
							dropForeignKeys.ExecuteNonQuery();
						}
						catch (Exception ex)
						{
							tx.Rollback();
							throw;
						}
					}
					break;
			}

            Disconnect();

			new SchemaExport(_nHibernateConfiguration).Drop(false, true);
		}

		/// <summary>
		/// DO NOT CALL UNLESS YOU INTEND TO RECREATE THE CURRENT DATABASE! Creates SQL Database based on current NHibernate Configuration.
		/// Extrapolates Business Model to Relational Database Tables on server declared in 'hibernate-configuration' element of config file.
		/// </summary>
		public static void ExportModelToDatabase()
		{
			if (_nHibernateConfiguration == null)
			{
				throw new NullReferenceException("The current NHibernate Configuration has not been set.");
			}

			new SchemaExport(_nHibernateConfiguration).Create(false, true);
		}

        private static ISession CreateSession()
        {
            ISession newSession = CurrentSessionFactory.OpenSession();

            if (newSession.IsConnected)
            {
                newSession.Disconnect();
            }

            return newSession;
        }

    	private static ISessionFactory CurrentSessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = CreateSessionFactory();
                }

                return _sessionFactory;
            }
        }

        private static ISessionFactory CreateSessionFactory()
        {
			if (_sessionFactoryConfiguration != null)
			{
				_nHibernateConfiguration = _sessionFactoryConfiguration
					.BuildConfiguration();
			}
			else
			{
				throw new NotImplementedException();
			}

			#region Sample Configurations
			//return Fluently.Configure()
			//    .Database(GetSqlConfiguration())
			//    .Mappings(m =>
			//    {
			//        m.FluentMappings.AddFromAssembly(AssemblyToMap);
			//        m.AutoMappings.Add(
			//            AutoMap.Assemblies(AssemblyToMap)
			//                .Where(t => t.IsSubclassOf(BaseClassOfTypeToMap) || OtherTypesToMap.Contains(t))
			//                .UseOverridesFromAssembly(AssemblyToMap)
			//                .IgnoreBase(BaseClassOfTypeToMap)
			//                .Conventions.Add(
			//                    DefaultCascade.SaveUpdate(),
			//                    DefaultLazy.Always()
			//                )
			//            )
			//            .ExportTo(Path.GetTempPath());
			//    })
			//    .BuildConfiguration();
			#endregion Sample Configurations

			ISessionFactory newfactory = _nHibernateConfiguration.BuildSessionFactory();

			return newfactory;
        }
        
        private static void Reconnect()
        {
            if (!CurrentSession.IsConnected)
            {
                CurrentSession.Reconnect();
            }
        }

        private static void Disconnect()
        {
            switch (Environment)
            {
                case PlatformEnvironment.Desktop:
                    if (_session != null && _session.IsConnected)
                    {
                        _session.Disconnect();
                    }
                    break;

                case PlatformEnvironment.Browser:
                    ISession requestSession = HttpContext.Current.Items[NHIBERNATE_SESSION_KEY] as ISession;

                    if (requestSession != null && requestSession.IsConnected)
                    {
                        requestSession.Disconnect();
                    }
                    break;

                default:
                    goto case PlatformEnvironment.Desktop;
            }
        }
	}
}
