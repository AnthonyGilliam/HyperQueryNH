using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

using HyperQueryNH.Core;

namespace HyperQuery.Repositories
{
    /// <summary>
    /// Repositories are the utilities that query specific objects from the database gateway.  This class provides essential methods to allow for advanced quering.
    /// </summary>
    /// <typeparam name="TQueryObject">The type of object to be queried from the database</typeparam>
    /// <typeparam name="TKey">The type used to represent the primary key of the objects being queried</typeparam>
	public abstract class RepositoryBase<TKey, TQueryObject>
		where TKey : struct
    {
    	protected readonly IUnitOfWork<TKey> UnitOfWork;

    	protected RepositoryBase(IUnitOfWork<TKey> unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public virtual void Save(TQueryObject obj)
        {
            UnitOfWork.AddToSession(obj);
        }

        public virtual void Update(TQueryObject obj)
        {
            UnitOfWork.Update(obj);
        }

        public virtual void Delete(TQueryObject obj)
        {
            UnitOfWork.Delete(obj);
        }

        public virtual TQueryObject GetByID(TKey objectID)
        {
            return UnitOfWork.Get<TQueryObject>(objectID);
        }

        public virtual TQueryObject GetFirst(Expression<Func<TQueryObject, string>> sortExpression
            , bool ascending)
        {
            return UnitOfWork.GetFirst<TQueryObject>(o => true
                , sortExpression
                , ascending);
        }

        public virtual TQueryObject GetFirst(Expression<Func<TQueryObject, bool>> queryExpression
            , Expression<Func<TQueryObject, string>> sortExpression
            , bool ascending)
        {
            return UnitOfWork.GetFirst<TQueryObject>(queryExpression
                , sortExpression
                , ascending);
        }

        public virtual IList<TQueryObject> GetAll()
        {
            IList<TQueryObject> objList = UnitOfWork.GetAll<TQueryObject>();
            return objList;
        }

        public virtual IList<TQueryObject> GetAll(Expression<Func<TQueryObject, bool>> queryExpression
            , Expression<Func<TQueryObject, string>> sortExpression
            , bool ascending)
        {
            return UnitOfWork.GetAll<TQueryObject>(queryExpression
                , sortExpression
                , ascending);
        }

        protected IList<TQueryObject> FindWhere(Expression<Func<TQueryObject, bool>> queryExpression)
        {
            return UnitOfWork.GetAll<TQueryObject>(queryExpression);
        }
    }
}
