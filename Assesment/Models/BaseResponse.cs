using System.Numerics;

namespace AssesmentProject.Models
{
    public class BaseResponse
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }

    public class BaseResponse<T> : BaseResponse
    {
        public List<T> Results { get; set; } = null;
    }

    public class StdModelPagingList<T> : BaseResponse
    {
        public int TotalCount { get; set; }
        public List<T> ReturnList { get; set; }
    }

}
