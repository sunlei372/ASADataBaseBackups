using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asa.Common
{
    /// <summary>
    /// 数据库通用类
    /// </summary>
    public static class DBHelper
    {
        // 哈希表用来存储缓存的参数信息，哈希表可以存储任意类型的参数。
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
        private static SqlConnection connection;
        private static string connectionString;
        public static void SetConnectionString(string ip,string name,string pwd) {
            connectionString = $"Data Source={ip}; User ID={name};Password={pwd};Initial Catalog=master;max pool size=3512;";
            connection = null;
        }
        #region DBHelper中固定对象以及属性
        //获得连接字符串
        public static string ConnectionString
        {
            get
            {
                connection = null;
                return DBHelper.connectionString;
            }
        }
        
        /// <summary>
        /// 创建数据库连接
        /// </summary>
        public static SqlConnection Connection
        {
            get
            {
                //如果连接为空就创建一个新的连接并打开
                if (connection == null)
                {
                    connection = new SqlConnection(ConnectionString);
                    connection.Open();
                }
                //如果连接处于关闭状态,将其打开连接
                else if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                //如果连接对象损坏将其关闭重新打开
                else if (connection.State == System.Data.ConnectionState.Broken)
                {
                    connection.Close();
                    connection.Open();
                }
                return connection;
            }
        }
        #endregion
        #region 经过简化后的方法

        /// <summary>
        /// 获取序列号
        /// </summary>
        /// <param name="ctype">类别</param>
        /// <param name="code">编号</param>
        /// <param name="cdate">日期</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="isnumber"></param>
        /// <returns></returns>
        public static string GeneraterSequence(string ctype, string code, string cdate, string defaultValue = "00", int isnumber = 0)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@ctype",ctype),
                new SqlParameter("@code",code),
                new SqlParameter("@cdate",cdate),
                new SqlParameter("@default",defaultValue),
                new SqlParameter("@isnumber",isnumber),
            };
            return DBHelper.ExecuteScalar("PROC_GENERATER_SEQUENCE", CommandType.StoredProcedure, param).ToString();
        }
        /// <summary>
        /// 执行sql语句或存储过程 (无参数)
        /// 返回影响行数
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程名称</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <returns>返回存储过程所影响的行数</returns>
        public static int ExecuteNonQuery(string cmdText)
        {
            return DBHelper.ExecuteNonQuery(cmdText, CommandType.Text);
        }
        /// <summary>
        /// 执行sql语句或存储过程 (无参数)
        /// 返回影响行数
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程名称</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <returns>返回存储过程所影响的行数</returns>
        public static int ExecuteNonQuery(string cmdText, CommandType comType)
        {
            return DBHelper.ExecuteNonQuery(Connection, comType, cmdText, null);
        }

        /// <summary>
        /// 执行sql语句或存储过程 (含参数)
        /// 返回影响行数
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程名称</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="paramValue">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回存储过程的影响行数</returns>
        public static int ExecuteNonQuery(string cmdText, CommandType comType, params SqlParameter[] paramValue)
        {
            return DBHelper.ExecuteNonQuery(Connection, comType, cmdText, paramValue);
        }
        /// <summary>
        /// 执行T-SQL语句 或 存储过程 (无参数)
        /// 返回结果的第一行第一列
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public static object ExecuteScalar(string cmdText)
        {
            return ExecuteScalar(cmdText, CommandType.Text);
        }
        /// <summary>
        /// 执行T-SQL语句 或 存储过程 (无参数)
        /// 返回结果的第一行第一列
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public static object ExecuteScalar(string cmdText, CommandType comType)
        {
            return ExecuteScalar(DBHelper.Connection, comType, cmdText, null);
        }
        /// <summary>
        /// 执行T-SQL语句 或 存储过程 (含参数)
        /// 返回结果的第一行第一列
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="paramValue">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public static object ExecuteScalar(string cmdText, CommandType comType, params SqlParameter[] paramValue)
        {
            return ExecuteScalar(DBHelper.Connection, comType, cmdText, paramValue);
        }
        /// <summary>
        /// 执行T-SQL语句 或 存储过程 (无参数)
        /// 返回SqlDataReader结果集
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <returns>返回SqlDataReader结果集</returns>
        public static SqlDataReader ExecuteReader(string cmdText, CommandType comType)
        {
            return ExecuteReader(DBHelper.ConnectionString, comType, cmdText, null);
        }
        /// <summary>
        /// 执行T-SQL语句 或 存储过程 (无参数)
        /// 返回SqlDataReader结果集
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <returns>返回SqlDataReader结果集</returns>
        public static SqlDataReader ExecuteReader(string cmdText)
        {
            return ExecuteReader(cmdText, CommandType.Text);
        }
        /// <summary>
        /// 执行T-SQL语句 或 存储过程 (含参数)
        /// 返回SqlDataReader结果集
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="paramValue">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回SqlDataReader结果集</returns>
        public static SqlDataReader ExecuteReader(string cmdText, CommandType comType, params SqlParameter[] paramValue)
        {
            return ExecuteReader(DBHelper.ConnectionString, comType, cmdText, paramValue);
        }

        /// <summary>
        /// 执行T-SQL语句 或 存储过程 (无参数)
        /// 返回DataSet结果集
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <returns>返回DataSet结果集</returns>
        public static DataSet ExecuteDataSet(string cmdText, CommandType comType)
        {
            return ExecuteDataSet(DBHelper.ConnectionString, comType, cmdText, null);
        }

        public static DataSet ExecutePageDataSet(string tablename, string field, string where, string orderby, int pageindex = 1, int pagesize = 20)
        {
            SqlParameter[] paramarr = new SqlParameter[]
            {
                new SqlParameter("@tablename",tablename),
                new SqlParameter("@field",field),
                new SqlParameter("@orderfield",orderby),
                new SqlParameter("@where",where),
                new SqlParameter("@pagesize",pagesize),
                new SqlParameter("@pageindex",pageindex)
            };
            return ExecuteDataSet("proc_datapager", CommandType.StoredProcedure, paramarr);
        }
        /// <summary>
        /// 执行T-SQL语句 或 存储过程 (无参数)
        /// 返回DataSet结果集
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <returns>返回DataSet结果集</returns>
        public static DataTable ExecuteDataTable(string cmdText)
        {
            return ExecuteDataSet(cmdText, CommandType.Text).Tables[0];
        }


        /// <summary>
        /// 执行T-SQL语句 或 存储过程 (无参数)
        /// 返回DataSet结果集
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <returns>返回DataSet结果集</returns>
        public static DataTable ExecuteDataTable(string cmdText, CommandType comType)
        {
            return ExecuteDataSet(DBHelper.ConnectionString, comType, cmdText, null).Tables[0];
        }
        /// <summary>
        /// 执行T-SQL语句 或 存储过程 (无参数)
        /// 返回DataSet结果集
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <returns>返回DataSet结果集</returns>
        public static DataSet ExecuteDataSet(string cmdText)
        {
            return ExecuteDataSet(cmdText, CommandType.Text);
        }

        /// <summary>
        /// 执行T-SQL语句 或 存储过程 (含参数)
        /// 返回DataSet结果集
        /// </summary>
        /// <param name="cmdText">T-SQL语句或存储过程</param>
        /// <param name="comType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="paramValue">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回DataSet结果集</returns>
        public static DataSet ExecuteDataSet(string cmdText, CommandType comType, params SqlParameter[] paramValue)
        {
            return ExecuteDataSet(DBHelper.ConnectionString, comType, cmdText, paramValue);
        }
        #endregion

        #region DBHelper中固定化方法
        /// <summary>
        ///执行一个不需要返回值的SqlCommand命令，通过指定专用的连接字符串。
        /// 使用参数数组形式提供参数列表 ，返回影响行数
        /// </summary>
        /// <remarks>
        /// 使用示例：
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();

                //清空SqlCommand中的参数列表
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        ///执行一条不返回结果的SqlCommand，通过一个已经存在的数据库连接 
        /// 使用参数数组提供参数，返回影响行数
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">一个现有的数据库连接</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行一条不返回结果的SqlCommand，通过一个已经存在的数据库事物处理 
        /// 使用参数数组提供参数 ，返回影响行数
        /// </summary>
        /// <remarks>
        /// 使用示例： 
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">一个存在的 sql 事物处理</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行一条返回结果集的SqlCommand命令，通过专用的连接字符串。
        /// 使用参数数组提供参数，返回一个SqlDataReader结果集
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个包含结果的SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);

            // 在这里使用try/catch处理是因为如果方法出现异常，则SqlDataReader就不存在，
            //CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
            //关闭数据库连接，并通过throw再次引发捕捉到的异常。
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }


        /// <summary>
        /// 执行一条返回结果集的SqlCommand命令，通过专用的连接字符串。
        /// 使用参数数组提供参数,返回一个DataSet 结果集
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个包含结果的SqlDataReader</returns>
        public static DataSet ExecuteDataSet(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                return ds;
            }
        }
        /// <summary>
        /// 执行一条返回结果集的SqlCommand命令，通过一个已经存在的数据库连接。
        /// 使用参数数组提供参数,返回一个DataSet 结果集
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个包含结果的SqlDataReader</returns>
        public static DataSet ExecuteDataSet(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            return ds;
        }

        /// <summary>
        /// 执行一条返回第一条记录第一列的SqlCommand命令，通过专用的连接字符串。 
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个object类型的数据，可以通过 Convert.To{Type}方法转换类型</returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 执行一条返回第一条记录第一列的SqlCommand命令，通过已经存在的数据库连接。
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例： 
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">一个已经存在的数据库连接</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个object类型的数据，可以通过 Convert.To{Type}方法转换类型</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }
        #endregion

        #region 缓存数据
        /// <summary>
        /// 缓存参数数组
        /// </summary>
        /// <param name="cacheKey">参数缓存的键值</param>
        /// <param name="cmdParms">被缓存的参数列表</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// 获取被缓存的参数
        /// </summary>
        /// <param name="cacheKey">用于查找参数的KEY值</param>
        /// <returns>返回缓存的参数数组</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            //新建一个参数的克隆列表
            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            //通过循环为克隆参数列表赋值
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                //使用clone方法复制参数列表中的参数
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }
        #endregion

        #region 为执行命令准备参数
        /// <summary>
        /// 为执行命令准备参数
        /// </summary>
        /// <param name="cmd">SqlCommand 命令</param>
        /// <param name="conn">已经存在的数据库连接</param>
        /// <param name="trans">数据库事物处理</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">Command text，T-SQL语句 例如 Select * from Products</param>
        /// <param name="cmdParms">返回带参数的命令</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            //判断数据库连接状态
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            //判断是否需要事物处理
            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        #endregion

        internal static SqlTransaction CreateTransaction()
        {
            return Connection.BeginTransaction();
        }
    }
}
