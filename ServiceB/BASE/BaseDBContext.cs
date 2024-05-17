using Microsoft.EntityFrameworkCore;
using MSCore.EntityFramework.Model;
using MSCore.Util.ConfigurationManager;
using MSCore.Util.Object.Object_Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSCore.EntityFramework
{
    public class BaseDBContext : DbContext
    {
        /// <summary>
        /// Key
        /// </summary>
        public virtual string ConnectionKey { get; set; } = "App.Db.Project";

        public BaseDBContext(DbContextOptions options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConnectionInfoPlus connectionInfo = Appsettings.json.GetByPath<ConnectionInfoPlus>(ConnectionKey);
            if (connectionInfo.type == "dm")
            {
                string schema = connectionInfo.ConnectionString.Substring(connectionInfo.ConnectionString.ToLower().IndexOf("database")).Split(';')[0].Split('=')[1];
                modelBuilder.HasDefaultSchema(schema);
            }
        }



        /// <summary>
        /// 通过sql查询列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        public virtual List<T> ExecSQL<T>(string sql)
        {
            List<T> list = new List<T>();
            try
            {
                using (var command = Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;

                    Database.OpenConnection();
                    using (var reader = command.ExecuteReader())
                    {
                        var propertites = typeof(T).GetProperties();
                        while (reader.Read())
                        {
                            var obj = Activator.CreateInstance(typeof(T));
                            foreach (var property in propertites)
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i);
                                    if (columnName.Equals(property.Name, StringComparison.OrdinalIgnoreCase))
                                    {
                                        var item = reader[columnName];
                                        if (item != null)
                                        {
                                            if (property.PropertyType == typeof(bool))
                                            {
                                                obj.SetProperty(property.Name, Convert.ToBoolean(item));
                                            }
                                            else
                                            {
                                                if (Appsettings.DatabaseType == "sqlite")
                                                {
                                                    if (property.PropertyType == typeof(int))
                                                    {
                                                        item = Convert.ToInt32(item);
                                                    }
                                                    else if (property.PropertyType == typeof(DateTime))
                                                    {
                                                        item = Convert.ToDateTime(item);
                                                    }
                                                }
                                                obj.SetProperty(property.Name, item);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            list.Add((T)obj);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("sql执行失败：" + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 通过sql查询列表 异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> ExecSQLAsync<T>(string sql)
        {
            List<T> list = new List<T>();
            try
            {
                using (var command = Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;

                    Database.OpenConnection();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var propertites = typeof(T).GetProperties();
                        while (reader.Read())
                        {
                            var obj = Activator.CreateInstance(typeof(T));
                            foreach (var property in propertites)
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (reader.GetName(i).Equals(property.Name, StringComparison.OrdinalIgnoreCase))
                                    {
                                        var item = reader[property.Name];
                                        if (item != null)
                                        {
                                            if (property.PropertyType == typeof(bool))
                                            {
                                                obj.SetProperty(property.Name, Convert.ToBoolean(item));
                                            }
                                            else
                                            {
                                                if (Appsettings.DatabaseType == "sqlite")
                                                {
                                                    if (property.PropertyType == typeof(int))
                                                    {
                                                        item = Convert.ToInt32(item);
                                                    }
                                                    else if (property.PropertyType == typeof(DateTime))
                                                    {
                                                        item = Convert.ToDateTime(item);
                                                    }
                                                }
                                                obj.SetProperty(property.Name, item);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            list.Add((T)obj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("sql执行失败：" + ex.Message);
            }
            return list;
        }

    }
}
