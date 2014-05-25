using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate;

namespace HyperQueryNH.Core
{
    public interface IUnitOfWork<TKey>
		where TKey : struct 
	{
        void Reconnect();
        void Disconnect();

        void Initialize(Object mappedObject);
        
        void AddToSession(object mappedObject);
        void Update(object mappedObject);
        void Delete(object mappedObject);

		TDomainObject Get<TDomainObject>(TKey id);
        TDomainObject Get<TDomainObject>(Expression<Func<TDomainObject, bool>> queryExpression);
        TDomainObject GetFirst<TDomainObject>(Expression<Func<TDomainObject, bool>> queryExpression
            , Expression<Func<TDomainObject, string>> sortExpression
            , bool ascending);

        int GetCount<TDomainObject>() where TDomainObject : class;
        TDomainObject GetRandom<TDomainObject>(Expression<Func<TDomainObject, bool>> queryExpression) where TDomainObject : class;
        TDomainObject GetRandom<TDomainObject>() where TDomainObject : class;

        IList<TDomainObject> GetAll<TDomainObject>();
        IList<TDomainObject> GetAll<TDomainObject>(int skip, int take);
        IList<TDomainObject> GetAll<TDomainObject>(Expression<Func<TDomainObject, bool>> queryExpression);
        IList<TDomainObject> GetAll<TDomainObject>(Expression<Func<TDomainObject, bool>> queryExpression
            , int skip
            , int take);
        IList<TDomainObject> GetAll<TDomainObject>(Expression<Func<TDomainObject, string>> sortExpression
            , bool ascending);
        IList<TDomainObject> GetAll<TDomainObject, TSortType>(Expression<Func<TDomainObject, TSortType>> sortExpression
            , bool ascending);
        IList<TDomainObject> GetAll<TDomainObject, TSortType>(Expression<Func<TDomainObject, TSortType>> sortExpression
            , bool ascending
            , int skip
            , int take);
        IList<TDomainObject> GetAll<TDomainObject>(Expression<Func<TDomainObject, bool>> queryExpression
            , Expression<Func<TDomainObject, string>> sortExpression
            , bool ascending);
        IList<TDomainObject> GetAll<TDomainObject, TSortType>(Expression<Func<TDomainObject, bool>> queryExpression
            , Expression<Func<TDomainObject, TSortType>> sortExpression
            , bool ascending);

        IList<TDomainObject> GetAll<TDomainObject, TSortType>(Expression<Func<TDomainObject, bool>> queryExpression
            , Expression<Func<TDomainObject, TSortType>> sortExpression
            , bool ascending
            , int skip
            , int take);
    }
}
