# Dapper.Plus
Dapper扩展插件

# 简介 

目前仅支持Mysql<br />
通过拼接sql的方式实现批量写入Mysql数据库，提高写入性能。<br />

# 使用方法

using (MySqlConnection connection = new MySqlConnection(connectionString))
{
    string sql = "insert into EntityTest(Name,Address,Phone,Gender) values (@Name,@Address,@Phone,@Gender)";
    connection.BulkInsertMysql(sql, list);
}
