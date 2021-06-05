using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using MySqlConnector;

namespace Dapper.Plus
{
    public static class DapperPlus
    {
        public static void BulkInsertMysql<T>(this MySqlConnection connection,string sql, IEnumerable<T> data)
        {
            var type = typeof(T);
            var allProperties = PropertiesCache.TypePropertiesCache(type);
            var keyProperties = PropertiesCache.KeyPropertiesCache(type);
            var computedProperties = PropertiesCache.ComputedPropertiesCache(type);
            var columns = PropertiesCache.GetColumnNamesCache(type);

            var insertProperties = allProperties.Except(computedProperties).ToList();

            insertProperties = insertProperties.Except(keyProperties).ToList();

            //var insertPropertiesString = GetColumnsStringSqlServer(insertProperties, columns);
            var sqls = GetParmString(sql);
            var insertSqlmain = sqls.Item1;
            var parms = sqls.Item2;

            var typeCasts = new Type[insertProperties.Count];
            for (var i = 0; i < insertProperties.Count; i++)
            {
                if (insertProperties[i].PropertyType.IsEnum)
                {
                    typeCasts[i] = Enum.GetUnderlyingType(insertProperties[i].PropertyType);
                }
                else
                {
                    typeCasts[i] = null;
                }
            }
            StringBuilder strSql = new StringBuilder();

            int count = 0;
            foreach (var item in data)
            {
                var values = new object[insertProperties.Count];
                //for (var i = 0; i < insertProperties.Count; i++)
                //{
                //    var value = insertProperties[i].GetValue(item, null);
                //    values[i] = typeCasts[i] == null ? value : Convert.ChangeType(value, typeCasts[i]);
                //}
                strSql.Append(" (");
                foreach (var parm in parms)
                {
                    var value = insertProperties.Where(w => w.Name == parm).First().GetValue(item, null);
                    strSql.Append($"'{value}',");
                }
                strSql.Remove(strSql.Length - 1, 1);
                strSql.Append("),");

                count++;
            }
            strSql.Remove(strSql.Length - 1, 1);
            var inserSql = strSql.ToString();

            connection.Execute(insertSqlmain + inserSql);

            strSql.Clear();
        }

        private static (string,List<string>) GetParmString(string sql)
        {
            string strSql = sql.Substring(0,sql.IndexOf("values")+6);
            List<string> strarr = new List<string>();
            string sqltail = sql.Substring(sql.IndexOf("values"));
            var reg = new Regex(@"@([A-Za-z]+)");
            var result = reg.Matches(sqltail);
            foreach (Match item in result)
            {
                strarr.Add(item.Groups[1].Value);
            }

            return (strSql, strarr);
        }

        private static string GetColumnsStringSqlServer(IEnumerable<PropertyInfo> properties, IReadOnlyDictionary<string, string> columnNames, string tablePrefix = null)
        {
            if (tablePrefix == "target.")
            {
                return string.Join(", ", properties.Select(property => $"{tablePrefix}[{columnNames[property.Name]}] as [{property.Name}] "));
            }

            return string.Join(", ", properties.Select(property => $"{tablePrefix}[{columnNames[property.Name]}] "));
        }
    }
}
