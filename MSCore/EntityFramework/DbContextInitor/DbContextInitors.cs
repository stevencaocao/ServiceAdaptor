using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace MSCore.EntityFramework.DbContextInitor
{
    /// <summary>
    /// 存储多数据库项
    /// </summary>
    public class DbContextInitors
    {
        #region DbContextInitor
        public static ConcurrentDictionary<string, IDbContextInitor> DbContextInitorMap
            = new ConcurrentDictionary<string, IDbContextInitor>();


        static DbContextInitors()
        {
            DbContextInitorMap["mssql"] = new DbContextInitor_mssql();
            DbContextInitorMap["mysql"] = new DbContextInitor_mysql();
            DbContextInitorMap["sqlite"] = new DbContextInitor_sqlite();
            DbContextInitorMap["dm"] = new DbContextInitor_dm();
            DbContextInitorMap["pgsql"] = new DbContextInitor_pgsql();
        }
        #endregion
    }
}
