using AssesmentProject.Helpers;
using AssesmentProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using AssesmentProject.Repositories;
using System.Net;
using NLog;
using System.Dynamic;
using Microsoft.Extensions.Caching.Memory;
using Assesment.Controllers;
using Microsoft.Extensions.Logging;
using LogLevel = NLog.LogLevel;
using Assesment.Helpers;

namespace AssesmentProject.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMemoryCache _cache;

        public UserController(ILogger<UserController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        BaseResponseHelper baseResponseHelper = new BaseResponseHelper();
        UserRepository userRepository = new UserRepository();

        [HttpPost("Registration")]
        public BaseResponse Registration([FromBody] User user)
        {
            try
            {
                if (!userRepository.Register(user))
                {
                    return baseResponseHelper.GetResponse(StatusCodes.Status400BadRequest, "Register user failed");
                }

                return baseResponseHelper.GetResponse(StatusCodes.Status200OK, "OK");
            }
            catch (SqlXException err)
            {
                _logger.LogError(err.ToString());
                return baseResponseHelper.GetResponse(StatusCodes.Status500InternalServerError, "Register user failed");
            }
            catch (Exception err)
            {
                //_logger.Error(err);
                _logger.LogError(err.ToString());
                return baseResponseHelper.GetResponse(StatusCodes.Status500InternalServerError, "Register user failed");
            }
        }

        [HttpGet("SelectUserList/{pageNum}/{pageSize}")]
        public BaseResponse<ExpandoObject> SelectUserList(int pageNum, int pageSize)
        {
            try
            {
                string cache_key = CacheX.CacheKey.cache_key_SelectUserList + pageNum + pageSize;
                if (_cache.TryGetValue(cache_key, out IEnumerable<ExpandoObject> ls))
                {
                    return baseResponseHelper.GetResponseWithReturnList<ExpandoObject>(ls.ToList(), StatusCodes.Status200OK, "OK");
                }

                List<ExpandoObject>user_ls = userRepository.SelectUserList(pageNum, pageSize);
                if (user_ls.Count > 0)
                {
                    ls = user_ls;
                    CacheX.AddCache(TimeSpan.FromSeconds(5), ls, cache_key, _cache);

                    return baseResponseHelper.GetResponseWithReturnList<ExpandoObject>(user_ls, StatusCodes.Status200OK, "OK");
                }
                return baseResponseHelper.GetResponseWithReturnList<ExpandoObject>(user_ls, StatusCodes.Status404NotFound, "No users found");
            }
            catch (SqlXException err)
            {
                _logger.LogError(err.ToString());
                return baseResponseHelper.GetResponseWithReturnList<ExpandoObject>(null, StatusCodes.Status500InternalServerError, "Select user list failed");
            }
            catch (Exception err)
            {
                _logger.LogError(err.ToString());
                return baseResponseHelper.GetResponseWithReturnList<ExpandoObject>(null, StatusCodes.Status500InternalServerError, "Select user list failed");
            }
        }

        [HttpPut("UpdateUserDetails/{id}")]
        public BaseResponse UpdateUserDetails([FromBody] User user_details, int id)
        {
            try
            {
                if (!userRepository.UpdateUserDetails(user_details, id))
                {
                    return baseResponseHelper.GetResponse(StatusCodes.Status400BadRequest, "Update user details failed");
                }
                return baseResponseHelper.GetResponse(StatusCodes.Status200OK, "OK");
            }
            catch (SqlXException err)
            {
                _logger.LogError(err.ToString());
                return baseResponseHelper.GetResponse(StatusCodes.Status500InternalServerError, "Update user details failed");
            }
            catch (Exception err)
            {
                _logger.LogError(err.ToString());
                return baseResponseHelper.GetResponse(StatusCodes.Status500InternalServerError, "Update user details failed");
            }
        }

        [HttpDelete("DeleteUser/{id}")]
        public BaseResponse DeleteUser ([FromRoute] int id)
        {
            try
            {
                if (!userRepository.DeleteUser(id))
                {
                    return baseResponseHelper.GetResponse(StatusCodes.Status400BadRequest, "Delete user failed");
                }
                return baseResponseHelper.GetResponse(StatusCodes.Status200OK, "OK");
            }
            catch (SqlXException err)
            {
                _logger.LogError(err.ToString());
                return baseResponseHelper.GetResponse(StatusCodes.Status500InternalServerError, "Delete user failed");
            }
            catch (Exception err)
            {
                _logger.LogError(err.ToString());
                return baseResponseHelper.GetResponse(StatusCodes.Status500InternalServerError, "Delete user failed");
            }
        }

        [HttpGet("SelectUserById/{id}")]
        public BaseResponse<User> SelectUserById([FromRoute] int id)
        {
            try
            {
                string cache_key = CacheX.CacheKey.cache_key_SelectUserById + id;
                if (_cache.TryGetValue(cache_key, out IEnumerable<User> ls))
                {
                    return baseResponseHelper.GetResponseWithReturnList<User>(ls.ToList(), StatusCodes.Status200OK, "OK");
                }

                List<User>user_ls = userRepository.SelectUserById(id);
                if (user_ls.Count > 0)
                {
                    ls = user_ls;
                    CacheX.AddCache(TimeSpan.FromSeconds(60), ls, cache_key, _cache);
                    return baseResponseHelper.GetResponseWithReturnList<User>(user_ls, StatusCodes.Status200OK, "OK");
                }
                return baseResponseHelper.GetResponseWithReturnList<User>(user_ls, StatusCodes.Status404NotFound, "User details not found");
            }
            catch (SqlXException err)
            {
                _logger.LogError(err.ToString());
                return baseResponseHelper.GetResponseWithReturnList<User>(null, StatusCodes.Status500InternalServerError, "Select user details failed");
            }
            catch (Exception err)
            {
                _logger.LogError(err.ToString());
                return baseResponseHelper.GetResponseWithReturnList<User>(null, StatusCodes.Status500InternalServerError, "Select user details failed");
            }
        }
    }
}
