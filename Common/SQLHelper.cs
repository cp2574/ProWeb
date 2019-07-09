using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Common
{
    public class SQLHelper
    {
        // 连接数据库
        private string connectString = string.Empty;

        public SQLHelper()
        {
            connectString = ConfigurationManager.ConnectionStrings["ERPEntities"].ToString();
        }

        /// <summary>
        /// 返回dataset数据集
        /// </summary>
        /// <param name="ct">SqlCommand对象的命令类型</param>
        /// <param name="strtxt">SqlCommand对象的文本</param>
        /// <param name="parm">SqlCommand对象的参数</param>
        /// <returns></returns>
        public  DataSet GetDataSet(CommandType ct, string strtxt, SqlParameter[] parm)
        {
            SqlConnection conn = new SqlConnection(connectString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                //指定cmd属性
                cmd.CommandText = strtxt;
                cmd.Connection = conn;
                cmd.CommandType = ct;
                if (parm != null)//是否执行存储过程
                {
                    foreach (SqlParameter i in parm)
                    {
                        cmd.Parameters.Add(i);
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter();//初始化SqlDataAdapter
                da.SelectCommand = cmd;//指定cmd命令
                DataSet ds = new DataSet();
                da.Fill(ds);

                return ds;

            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 返回DataTable数据集
        /// </summary>
        /// <param name="ct">SqlCommand对象的命令类型</param>
        /// <param name="strtxt">SqlCommand对象的文本</param>
        /// <param name="parm">SqlCommand对象的参数</param>
        /// <returns></returns>
        public  DataTable GetDataTable(CommandType ct, string strtxt, SqlParameter[] parm)
        {
            SqlConnection conn = new SqlConnection(connectString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                //指定cmd属性
                cmd.CommandText = strtxt;
                cmd.Connection = conn;
                cmd.CommandType = ct;
                if (parm != null)
                {
                    foreach (SqlParameter i in parm)
                    {
                        cmd.Parameters.Add(i);
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter();//初始化SqlDataAdapter
                da.SelectCommand = cmd;//指定cmd命令
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 返回受影响的行数
        /// </summary>
        /// <param name="ct">SqlCommand对象的命令类型</param>
        /// <param name="strtxt">SqlCommand对象的文本</param>
        /// <param name="tran">事务</param>
        /// <param name="parm">SqlCommand对象的参数</param>
        /// <returns></returns>
        public  bool ExcuteNonQuery(CommandType ct, string strtxt, SqlParameter[] parm)
        {
            SqlConnection conn = new SqlConnection(connectString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = ct;
                cmd.CommandText = strtxt;
                if (parm != null)
                {
                    foreach (SqlParameter i in parm)
                    {
                        cmd.Parameters.Add(i);
                    }
                }
                int falg = cmd.ExecuteNonQuery();//返回受影响行数
                if (falg > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex )
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        #region 事务处理
        /// <summary>
        /// 带参数事物
        /// </summary>
        /// <param name="sqlList"></param>
        /// <param name="paraList"></param>
        public  void ExecuteTrans(string  sqlText)
        {
            using (SqlConnection con = new SqlConnection(connectString))
            {
                SqlCommand cmd = new SqlCommand();
                SqlTransaction transaction = null;
                cmd.Connection = con;
                try
                {
                    con.Open();
                    transaction = con.BeginTransaction();
                    cmd.Transaction = transaction;
                    cmd.CommandText = sqlText;
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                finally {
                    con.Close();
                    con.Dispose();
                }
            }
        }
        #endregion


        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public  bool TabExists(string TableName)
        {
            string strsql = "select count(*) from sysobjects where id = object_id(N'[" + TableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
            object obj = GetDataSet(CommandType.Text, strsql, null);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 查询出最大id
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public  int GetMaxID(string FieldName, string TableName)
        {
            string strSql = "select max(" + FieldName + ")+1 from " + TableName;
            DataSet ds = GetDataSet(CommandType.Text, strSql, null);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                return int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 1;
            }
        }
        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public  string ExecuteReader(string strSQL)
        {

            using (SqlConnection con = new SqlConnection(connectString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(strSQL, con);
                    SqlDataReader myReader = cmd.ExecuteReader();//执行SQL语句的真正查询了
                    if (myReader.HasRows)
                    {
                        myReader.Read();
                        return myReader.GetValue(0).ToString();
                    }
                    else
                    {
                        return "";
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }


            }
        }

        /// <summary>
        /// 返回一行一列
        /// </summary>
        /// <param name="ct">SqlCommand对象的命令类型</param>
        /// <param name="strtxt">SqlCommand对象的文本</param>
        /// <param name="parm">SqlCommand对象的参数</param>
        /// <returns></returns>
        public  string MaxId(CommandType ct, string strtxt, SqlParameter[] parm)
        {
            SqlConnection conn = new SqlConnection(connectString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = ct;
                cmd.CommandText = strtxt;
                if (parm != null)
                {
                    foreach (SqlParameter i in parm)
                    {
                        cmd.Parameters.Add(i);
                    }
                }
                string flag = cmd.ExecuteScalar().ToString();//返回第一行第一列
                return flag;
            }
            catch
            {
                return "";
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public  int ExecuteSqlTran(List<String> SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    conn.Close();
                    throw ex;
                }
            }
        }
        /// <summary>
        /// AspNetPage分页，
        /// </summary>
        /// <param name="sqlstr">查询语句</param>
        /// <param name="pageindex">AspNetPager1.PageSize * (AspNetPager1.CurrentPageIndex - 1)</param>
        /// <param name="pagesize">AspNetPager1.PageSiz</param>
        /// <returns></returns>
        public  DataSet PagedataSet(string sqlstr, int pageindex, int pagesize)
        {
            SqlConnection cn = new SqlConnection(connectString);
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                cn.Open();
                cmd.Connection = cn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlstr;
                da.SelectCommand = cmd;
                da.Fill(ds, pageindex, pagesize, "tabone");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                cn.Close();
            }
            return ds;
        }

        /// <summary>
        /// 更新历史表数据
        /// </summary>
        /// <param name="dt"></param>
        public void UpdateTable(DataTable newDt,string tableName,string connsql)
        {

            string sql = string.Format("SELECT TOP 0 * FROM  {0}", tableName);
            DataSet ds = new DataSet();
            
            SqlConnection conn = new SqlConnection(connsql);
            SqlDataAdapter sda = new SqlDataAdapter();
            sda.SelectCommand = new SqlCommand(sql, conn);
            SqlCommandBuilder cb = new SqlCommandBuilder(sda);//自动生成相应的命令，这句很重要  

            conn.Open();

            try
            {

                sda.Fill(ds);
                DataTable dt = ds.Tables[0];

                foreach (DataRow dr in newDt.Rows)
                {
                    dt.Rows.Add(dr.ItemArray);
                }

                sda.Update(dt);
                //对表的更新提交到数据库  
                //DataRow[] drs = dt.Select(null, null, DataViewRowState.Added);//或者搜索之后再更新  
                //sda.Update(drs);  

                dt.AcceptChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }


        /// <summary>
                /// 执行存储过程
                /// </summary>
                /// <param name="storedProcName">存储过程名</param>
                /// <param name="parameters">存储过程参数</param>
                /// <param name="tableName">DataSet结果中的表名</param>
                /// <returns>DataSet</returns>

        public  int ExecStoredProcedure(string procName, params SqlParameter[] parameters)
        {
            int rtn = 0;
            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    SqlTransaction st = conn.BeginTransaction();
                    cmd.Transaction = st;
                    try
                    {
                        if (parameters != null) {
                            cmd.Parameters.AddRange(parameters);
                        }
                        cmd.CommandText = procName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        rtn = cmd.ExecuteNonQuery();
                        st.Commit();
                        return rtn;
                    }
                    catch (SqlException sqlex)
                    {
                        st.Rollback();
                        conn.Close();
                        throw sqlex;
                    }
                }
            }
        }
        public  int ExecuteStoredProcedure(string procName,params SqlParameter[] parameters)
        {
            int rtn = 0;
            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    try {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        cmd.CommandText = procName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddRange(parameters);
                        rtn= cmd.ExecuteNonQuery();
                        return rtn;
                    }
                    catch (SqlException sqlex)
                    {
                        conn.Close();
                        throw sqlex;
                    }
                }
            }

        }

        public  void ModifiTable(DataTable upDt, string tableName, string connsql) {
            DataSet ds = new DataSet();
            ds.Tables.Add(upDt);
            string _tableName = tableName;
            int result = 0;
            using (SqlConnection sqlconn = new SqlConnection(connsql))
            {
                sqlconn.Open();

                //使用加强读写锁事务     
                SqlTransaction tran = sqlconn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {

                   // ds.Tables[0].AcceptChanges();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        //所有行设为修改状态     
                        dr.SetModified();
                    }
                    //为Adapter定位目标表     

                    SqlCommand cmd = new SqlCommand(string.Format("select * from {0} where {1}", _tableName, " 1=2"), sqlconn, tran);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    SqlCommandBuilder sqlCmdBuilder = new SqlCommandBuilder(da);
                    sqlCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    da.AcceptChangesDuringUpdate = false;
                    string columnsUpdateSql = "";
                    SqlParameter[] paras = new SqlParameter[upDt.Columns.Count];
                    int parasIndex = 0;
                    //需要更新的列设置参数是,参数名为"@+列名"  
                    for (int i = 0; i < upDt.Columns.Count; i++)
                    {
                        //此处拼接要更新的列名及其参数值  
                        columnsUpdateSql += ("[" + upDt.Columns[i].ColumnName + "]" + "=@" + upDt.Columns[i].ColumnName + ",");
                        if (upDt.Columns[i].DataType.Name == "DateTime")
                        {
                            paras[i] = new SqlParameter("@" + upDt.Columns[i].ColumnName, SqlDbType.DateTime, 23, upDt.Columns[i].ColumnName);
                        }
                        else if (upDt.Columns[i].DataType.Name == "Int64")
                        {
                            paras[i] = new SqlParameter("@" + upDt.Columns[i].ColumnName, SqlDbType.NVarChar, 19, upDt.Columns[i].ColumnName);
                        }
                        else
                        {
                            paras[i] = new SqlParameter("@" + upDt.Columns[i].ColumnName, SqlDbType.NVarChar, 2000, upDt.Columns[i].ColumnName);
                        }
                    }
                    if (!string.IsNullOrEmpty(columnsUpdateSql))
                    {
                        //此处去掉拼接处最后一个","  
                        columnsUpdateSql = columnsUpdateSql.Remove(columnsUpdateSql.Length - 1);
                    }
                    //此处生成where条件语句  
                    string limitSql = ("[ID]=@ID");
                    SqlCommand updateCmd = new SqlCommand(string.Format(" UPDATE [{0}] SET {1} WHERE {2} ", _tableName, columnsUpdateSql, limitSql));
                    //不修改源DataTable     
                    updateCmd.UpdatedRowSource = UpdateRowSource.None;
                    da.UpdateCommand = updateCmd;
                    da.UpdateCommand.Parameters.AddRange(paras);
                    //da.UpdateCommand.Parameters.Add("@" + table.Columns[0].ColumnName, table.Columns[0].ColumnName);  
                    //每次往返处理的行数  
                    da.UpdateBatchSize = upDt.Rows.Count;
                    result = da.Update(ds, _tableName);
                    ds.AcceptChanges();
                    tran.Commit();

                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
                finally
                {
                    sqlconn.Dispose();
                    sqlconn.Close();
                }


            }
        }

    }
}
