using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dapper.Plus.Demo
{
    class Program
    {
        public static string connectionString = "server=127.0.0.1;port=3306;user=root;password=123456;database=temp;CharSet=utf8;AllowLoadLocalInfile=true;";
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            List<EntityTest> list = new List<EntityTest>();
            for (int i = 0; i < 5000; i++)
            {
                EntityTest entity = new EntityTest()
                {
                    Name = "name" + i.ToString(),
                    Address = "abcdefg123456",
                    Phone = "13800138000",
                    Gender = 1
                    //CreatedTime = DateTime.Now.ToString()
                };
                list.Add(entity);
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            DapperBulkInsert(list);

            sw.Stop();
            Console.WriteLine($"耗时：{sw.ElapsedMilliseconds} ms");
            Console.ReadLine();


        }

        private static void DapperBulkInsert(List<EntityTest> list)
        {
            string sql = "insert into EntityTest(Name,Address,Phone,Gender) values  " +
                    "(@Name,@Address,@Phone,@Gender)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.BulkInsertMysql(sql, list);
            }
        }
    }
}
