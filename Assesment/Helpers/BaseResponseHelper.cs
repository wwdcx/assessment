using AssesmentProject.Models;
using System.Security.Cryptography.X509Certificates;

namespace AssesmentProject.Helpers
{
    public class BaseResponseHelper
    {
        public BaseResponse GetResponse(int status_code, string status_desc)
        {
            return new BaseResponse()
            {
                ReturnCode = status_code,
                ReturnMessage = status_desc
            };
        }

        public BaseResponse<T> GetResponseWithReturnList<T>(List<T> result_ls, int status_code, string status_desc)
        {
            return new BaseResponse<T>()
            {
                Results = result_ls,
                ReturnCode = status_code,
                ReturnMessage = status_desc
            };
        }
    }
}
