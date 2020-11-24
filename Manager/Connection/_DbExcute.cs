using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using System;
using System.Data;


namespace Manager.Connection
{
    public class _DbExcute : Controller
    {
        protected IDbConnection _db;
        protected QueryFactory _queryBuilder;

        public _DbExcute()
        {
            _db = (new KataConnection()).GetConnection();
            _queryBuilder = KataConnection.QueryBuilder();
        }

    }
}
