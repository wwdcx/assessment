namespace AssesmentProject.Helpers
{
    public class SqlXParameter
    {
        public string ParameterName;
        public object ParameterValue;
        public SqlXParameter(string parameterName, object parameterValue)
        {
            ParameterName = parameterName;
            ParameterValue = parameterValue;
        }
    }
}
