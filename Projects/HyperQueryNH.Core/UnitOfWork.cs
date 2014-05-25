using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Criterion;

namespace HyperQueryNH.Core
{
    public class UnitOfWork<TKey> : IUnitOfWork<TKey>
		where TKey : struct
    {
        protected ISession Session;

        #region IUnitOfWork Members

        public UnitOfWork(ISession currentNHibernateSession)
        {
			Session = currentNHibernateSession;
        }

        public void Reconnect()
        {
            if (!Session.IsConnected)
            {
                Session.Reconnect();
            }
        }

        public void Disconnect()
        {
            if (Session.IsConnected)
            {
                Session.Disconnect();
            }
        }

        public void Initialize(Object mappedObject)
        {
            Reconnect();

            if (!NHibernateUtil.IsInitialized(mappedObject))
            {
                NHibernateUtil.Initialize(mappedObject);
            }

            //Disconnect();
        }

        public void AddToSession(object mappedObject)
        {
            Reconnect();

            Session.Save(mappedObject);
            
            //Disconnect();
        }

        public void Update(object mappedObject)
        {
            Reconnect();

            Session.SaveOrUpdate(mappedObject);
            
            //Disconnect();
        }

        public void Delete(object mappedObject)
        {
            Reconnect();

            Session.Delete(mappedObject);

            //Disconnect();
        }

        public void Evict(object mappedObject)
        {
            Reconnect();

            Session.Evict(mappedObject);

            //Disconnect();
        }

		public TDomainObject Get<TDomainObject>(TKey id)
        {
            Reconnect();

            TDomainObject obj = Session.Get<TDomainObject>(id);

            //Disconnect();

            return obj;
        }

        public TDomainObject Get<TDomainObject>(Expression<Func<TDomainObject, bool>> queryExpression)
        {
            Reconnect();

            TDomainObject obj = Session.Query<TDomainObject>().Where(queryExpression).SingleOrDefault();

            //Disconnect();

            return obj;
        }

        public TDomainObject GetFirst<TDomainObject>(Expression<Func<TDomainObject, bool>> queryExpression
            , Expression<Func<TDomainObject, string>> sortExpression
            , bool ascending)
        {
            Reconnect();

            TDomainObject obj = ascending == true
                ?  Session.Query<TDomainObject>()
                    .Where(queryExpression)
                    .OrderBy(sortExpression)
                    .FirstOrDefault()
                :  Session.Query<TDomainObject>()
                    .Where(queryExpression)
                    .OrderByDescending(sortExpression)
                    .FirstOrDefault();

            //Disconnect();

            return obj;
        }

        public int GetCount<TDomainObject>() where TDomainObject : class
        {
            Reconnect();

            return Session.CreateCriteria<TDomainObject>()
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();
        }

        public TDomainObject GetRandom<TDomainObject>() where TDomainObject : class
        {
            Reconnect();

            var rowcount = GetCount<TDomainObject>();
            var randomIndex = new Random().Next(rowcount - 1);

            return Session.Query<TDomainObject>()
                .Skip(randomIndex)
                .First();
        }

        public TDomainObject GetRandom<TDomainObject>(Expression<Func<TDomainObject, bool>> queryExpression) where TDomainObject : class
        {
            Reconnect();

            var rowcount = Session.Query<TDomainObject>()
                .Where(queryExpression)
                .Count() - 1;

            var randomIndex = new Random().Next(rowcount - 1);

            return Session.Query<TDomainObject>()
                .Where(queryExpression)
                .Skip(randomIndex)
                .First();
        }

        public IList<TDomainObject> GetAll<TDomainObject>()
        {
            Reconnect();

        	IList<TDomainObject> objList = Session.Query<TDomainObject>().ToList();

            //Disconnect();

            return objList;
        }

        public IList<TDomainObject> GetAll<TDomainObject>(int skip, int take)
        {
            Reconnect();

            IList<TDomainObject> objList = Session.Query<TDomainObject>()
                .Skip(skip)
                .Take(take)
                .ToList();

            //Disconnect();

            return objList;
        }

        public IList<TDomainObject> GetAll<TDomainObject>(Expression<Func<TDomainObject, bool>> queryExpression)
        {
            Reconnect();

            IList<TDomainObject> objList =  Session.Query<TDomainObject>().Where(queryExpression).ToList();

            //Disconnect();

            return objList;
        }

        public IList<TDomainObject> GetAll<TDomainObject>(Expression<Func<TDomainObject, bool>> queryExpression
            , int skip
            , int take)
        {
            Reconnect();

            IList<TDomainObject> objList = Session.Query<TDomainObject>()
                .Where(queryExpression)
                .Skip(skip)
                .Take(take)
                .ToList();

            //Disconnect();

            return objList;
        }

        public IList<TDomainObject> GetAll<TDomainObject>(Expression<Func<TDomainObject, string>> sortExpression
            , bool ascending)
        {
            return GetAll(sortExpression, ascending);
        }


        public IList<TDomainObject> GetAll<TDomainObject, TSortType>(Expression<Func<TDomainObject, TSortType>> sortExpression
            , bool ascending)
        {
            Reconnect();

            IList<TDomainObject> objList = ascending
                ? Session.Query<TDomainObject>()
                    .OrderBy(sortExpression)
                    .ToList()
                : Session.Query<TDomainObject>()
                    .OrderByDescending(sortExpression)
                    .ToList();

            //Disconnect();

            return objList;
        }

        public IList<TDomainObject> GetAll<TDomainObject, TSortType>(Expression<Func<TDomainObject, TSortType>> sortExpression
            , bool ascending
            , int skip
            , int take)
        {
            Reconnect();

            IList<TDomainObject> objList = ascending
                ? Session.Query<TDomainObject>()
                    .OrderBy(sortExpression)
                    .Skip(skip)
                    .Take(take)
                    .ToList()
                : Session.Query<TDomainObject>()
                    .OrderByDescending(sortExpression)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

            //Disconnect();

            return objList;
        }

        public IList<TDomainObject> GetAll<TDomainObject>(Expression<Func<TDomainObject, bool>> queryExpression
            , Expression<Func<TDomainObject, string>> sortExpression
            , bool ascending)
        {
            return GetAll(queryExpression, sortExpression, ascending);
        }

        public IList<TDomainObject> GetAll<TDomainObject, TSortType>(Expression<Func<TDomainObject, bool>> queryExpression
            , Expression<Func<TDomainObject, TSortType>> sortExpression
            , bool ascending)
        {
            Reconnect();

            IList<TDomainObject> objList = ascending == true
                ? Session.Query<TDomainObject>()
                    .Where(queryExpression)
                    .OrderBy(sortExpression)
                    .ToList()
                : Session.Query<TDomainObject>()
                    .Where(queryExpression)
                    .OrderByDescending(sortExpression)
                    .ToList();

            //Disconnect();

            return objList;
        }

        public IList<TDomainObject> GetAll<TDomainObject, TSortType>(Expression<Func<TDomainObject, bool>> queryExpression
            , Expression<Func<TDomainObject, TSortType>> sortExpression
            , bool ascending
            , int skip
            , int take)
        {
            Reconnect();

            IList<TDomainObject> objList = ascending == true
                ? Session.Query<TDomainObject>()
                    .Where(queryExpression)
                    .OrderBy(sortExpression)
                    .Skip(skip)
                    .Take(take)
                    .ToList()
                : Session.Query<TDomainObject>()
                    .Where(queryExpression)
                    .OrderByDescending(sortExpression)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

            //Disconnect();

            return objList;
        }

        //public IQuery CreateFilter(object collection, string queryString)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion
    }
}
