using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace AssesmentProject.Helpers
{
    public class SqlExecution
    {
        public DataSet ExecuteQuery(string sql_query, SqlXParameter[] parameters, CommandType command_type = CommandType.Text)
        {
            DataSet dataset = new DataSet();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {

                    using (SqlCommand cmd = new SqlCommand(sql_query, conn))
                    {
                        conn.Open();

                        try
                        {
                            if (parameters != null)
                            {
                                foreach (var param in parameters)
                                {
                                    if (param.ParameterValue == null)
                                    {
                                        param.ParameterValue = DBNull.Value;
                                    }
                                    cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.ParameterValue));
                                }
                            }
                            cmd.CommandType = command_type;
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            adapter.Fill(dataset);
                        }
                        finally
                        {
                            cmd.Parameters.Clear();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw new SqlXException(sql_query, JsonConvert.SerializeObject(parameters), err.Message);
            }
            return dataset;
        }

        public bool ExecuteNonQuery(string sql_query, SqlXParameter[] parameters, CommandType command_type = CommandType.Text)
        {
            DataSet dataset = new DataSet();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {

                    using (SqlCommand cmd = new SqlCommand(sql_query, conn))
                    {
                        conn.Open();

                        try
                        {
                            if (parameters != null)
                            {
                                foreach (var param in parameters)
                                {
                                    if (param.ParameterValue == null)
                                    {
                                        param.ParameterValue = DBNull.Value;
                                    }
                                    cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.ParameterValue));
                                }
                            }
                            cmd.CommandType = command_type;
                            cmd.ExecuteNonQuery();
                        }
                        finally
                        {
                            cmd.Parameters.Clear();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw new SqlXException(sql_query, JsonConvert.SerializeObject(parameters), err.Message);
            }
            return true;
        }

        public object ExecuteScalar(string sql_query, SqlXParameter[] parameters, CommandType command_type = CommandType.Text)
        {
            object obj = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sql_query, conn))
                    {
                        conn.Open();

                        try
                        {
                            if (parameters != null)
                            {
                                foreach (var param in parameters)
                                {
                                    if (param.ParameterValue == null)
                                    {
                                        param.ParameterValue = DBNull.Value;
                                    }
                                    cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.ParameterValue));
                                }
                            }
                            obj = cmd.ExecuteScalar();
                        }
                        finally
                        {
                            cmd.Parameters.Clear();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw new SqlXException(sql_query, JsonConvert.SerializeObject(parameters), err.Message);
            }
            return obj;
        }
    }
}
