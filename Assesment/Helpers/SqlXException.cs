using System.Data.SqlClient;

namespace AssesmentProject.Helpers
{
    public class SqlXException : Exception
    {
        public string sql_query { get; private set; }
        public string param { get; private set; }
        public string err_msg { get; private set; }

        public SqlXException(string sql_query, string param, string err_msg)
        {
            this.sql_query = sql_query;
            this.param = param;
            this.err_msg = err_msg;
        }

        public override string ToString()
        {
            return "Query: " + sql_query + "Parameters: " + param + "Error: " + err_msg;
        }
    }
}
