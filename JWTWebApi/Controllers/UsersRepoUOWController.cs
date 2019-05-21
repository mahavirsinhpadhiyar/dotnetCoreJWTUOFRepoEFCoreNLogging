using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JWTWebApi.Contracts;
using JWTWebApi.Entities;
using JWTWebApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestCoreUow.UnitOfWork;

[Produces("application/json")]
[Route("api/v1/[Controller]")]
[ApiController]
public class UsersRepoUOWController: ControllerBase
{
    private readonly IUnitOfWork _userContext;
    private ILoggerManager _logger;

    public UsersRepoUOWController(IUnitOfWork context, ILoggerManager logger)
    {
        _userContext = context;
        _logger = logger;
    }

    /// <summary>
    ///Get users list
    /// </summary>
    /// <remarks>
    ///Get api/GetAll
    ///      response
    ///      {
    ///          Error: if any error occur will have the message
    ///          Response: Will give you status code: 200, 404 or 500
    ///          Result: Will return back the output result
    ///      }
    /// </remarks>
    /// <response code="200">Returns the newly created user</response>
    /// <response code="404">If the user details not found</response>
    /// <response code="500">If the user details not found or internal server error</response>
    /// <returns>return users list</returns>
    [HttpGet, Authorize]
    [Route("GetAll")]
    public async Task<ActionResult<APIResponse<IEnumerable<UserModel>>>> GetAll()
    {
        var userResponse = new APIResponse<IEnumerable<UserModel>>();
        try{
       
            var UserDetail = await _userContext.UserRepository.FindAllAsync();

            if(UserDetail.Count() == 0){
                userResponse.Error = "No users found";
                // userResponse.Response = HttpStatusCode.NotFound;
                userResponse.Result = null;
                return NotFound(userResponse);
            }

            _logger.LogInfo("Here is info message from our values controller.");
            _logger.LogDebug("Here is debug message from our values controller.");
            _logger.LogWarn("Here is warn message from our values controller.");
            _logger.LogError("Here is error message from our values controller.");

            // userResponse.Response = HttpStatusCode.OK;
            return Ok(userResponse.Result = UserDetail.AsEnumerable());

            }catch(Exception ex){
                userResponse.Error = "There is some internal problem. Sorry for the inconvenience";
                userResponse.Response = HttpStatusCode.InternalServerError;
                return StatusCode(500, userResponse);
                //ex will usefull in NLog
            }
    }

    /// <summary>
    ///Get Id wise user detail
    /// </summary>
    /// <remarks>
    ///GET /Get/1
    ///      response
    ///      {
    ///          Error: if any error occur will have the message
    ///          Response: Will give you status code: 200, 404 or 500
    ///          Result: Will return back the output result
    ///      }
    /// </remarks>
    /// <response code="200">Returns the newly created user</response>
    /// <response code="404">If the user details not found</response>
    /// <response code="500">If the user details not found</response>
    /// <returns>return user detail</returns>
    [HttpGet, Authorize]
    [Route("{Id}")]
    public async Task<ActionResult<APIResponse<UserModel>>> Get([Required]long Id)
    {
        var userResponse = new APIResponse<UserModel>();
        try {
            var UserDetail = await _userContext.UserRepository.FindByIdAsync(u => u.Id == Id);

            if(UserDetail == null){
                userResponse.Error = "No user details found for given Id";
                userResponse.Response = HttpStatusCode.NotFound;
                userResponse.Result = null;
                return NotFound(userResponse);
            }

            userResponse.Result = UserDetail;
            userResponse.Response = HttpStatusCode.OK;
            return Ok(userResponse);
        }catch(Exception ex){
            userResponse.Error = "There is some internal problem. Sorry for the inconvenience";
            userResponse.Response = HttpStatusCode.InternalServerError;
            return StatusCode(500, userResponse);
            //ex will usefull in NLog
        }
    }

    /// <summary>
    ///Create new user
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///     POST /create
    ///     {
    ///        "Username": "user1",
    ///        "Username": "username1",
    ///        "FirstName": "firstuser1",
    ///        "LastName": "lastuser1",
    ///        "Password": "lastuserpassword",
    ///     }
    ///      response
    ///      {
    ///          Error: if any error occur will have the message
    ///          Response: Will give you status code: 201, 400 or 500
    ///          Result: Will return back the output result
    ///      }
    /// </remarks>
    /// <returns>A newly created User</returns>
    /// <response code="201">Returns the newly created user</response>
    /// <response code="400">If the user detail is null</response>
    /// <response code="500">If any internal error occur</response>
    //Get api/Create
    [HttpPost, Authorize]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [Route("Create")]
     public async Task<ActionResult<APIResponse<UserModel>>> Create([Required]UserModel user){

        var userResponse = new APIResponse<UserModel>();

        try
        {
            if(!ModelState.IsValid)
            {
                userResponse.Error = "Invalid model object";
                userResponse.Response = HttpStatusCode.BadRequest;
                userResponse.Result = null;
                return userResponse;
            }

            _userContext.UserRepository.Create(user);
            var UpdatedRowValue = await _userContext.UserRepository.SaveAsync();

            if(UpdatedRowValue == 1)
            {
                userResponse.Error = "User created succefully";
                userResponse.Response = HttpStatusCode.Created;
                userResponse.Result = user;
                return StatusCode(201, userResponse);
            }
            else{
                userResponse.Error = "Invalid model object";
                userResponse.Response = HttpStatusCode.BadRequest;
                userResponse.Result = null;
                return BadRequest(userResponse);
            }
        }catch(Exception ex){
                userResponse.Error = "There is some internal problem. Sorry for the inconvenience";
                userResponse.Response = HttpStatusCode.InternalServerError;
                return StatusCode(500, userResponse);
                //ex will usefull in NLog
        }
     }

    /// <summary>
    ///update user details
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///     POST /Update
    ///     {
    ///        "Username": "user1",
    ///        "FirstName": "firstuser1",
    ///        "LastName": "lastuser1",
    ///     }
    ///      response
    ///      {
    ///          Error: if any error occur will have the message
    ///          Response: Will give you status code: 200, 204, 400, 500 or 404
    ///          Result: Will return back the output result
    ///      }
    /// </remarks>
    /// <response code="200">Returns the user details updated successfully</response>
    /// <response code="204">Returns when any new user or invaild user</response>
    /// <response code="400">If the user detail is null</response>
    /// <response code="500">If any internal error occur</response>
    /// <response code="404">If any user not found</response>
    //Get api/Update
    [HttpPost, Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
     [Route("Update")]
     public async Task<ActionResult<APIResponse<UserModel>>> Update([Required]UserModel user, [Required]long Id)
     {
        var userResponse = new APIResponse<UserModel>();

        try
        {
            if(!ModelState.IsValid)
            {
                userResponse.Error = "Invalid model object";
                userResponse.Response = HttpStatusCode.BadRequest;
                userResponse.Result = null;
                return BadRequest(userResponse);
            }

            if(Id == 0){
                userResponse.Error = "Invalid model object";
                userResponse.Response = HttpStatusCode.NoContent;
                userResponse.Result = null;
                return StatusCode(204, userResponse);
            }

            var userDetail = await _userContext.UserRepository.FindByIdAsync(u => u.Id == Id);
            
            if(userDetail == null)
            {
                userResponse.Error = "No user detail found";
                userResponse.Response = HttpStatusCode.NotFound;
                userResponse.Result = null;
                return NotFound(userResponse);
            }
            else{

                userDetail.FirstName = user.FirstName;
                userDetail.LastName = user.LastName;
                _userContext.UserRepository.Update(userDetail);
                var UpdatedRowValue = await _userContext.UserRepository.SaveAsync();

                if(UpdatedRowValue != 1){
                    userResponse.Error = "There is some problem during updating the current details. Sorry for the inconvenience please try again";
                    userResponse.Response = HttpStatusCode.InternalServerError;
                    userResponse.Result = null;
                    return StatusCode(500, userResponse);
                }
                else{
                    userResponse.Response = HttpStatusCode.OK;
                    return Ok(userResponse);
                }
            }
        }catch(Exception ex){
                userResponse.Error = "There is some internal problem. Sorry for the inconvenience";
                userResponse.Response = HttpStatusCode.InternalServerError;
                return StatusCode(500, userResponse);
                //ex will usefull in NLog
        }
     }

    /// <summary>
    ///delete user details
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///     GET /Delete/{id}
    ///      response
    ///      {
    ///          Error: if any error occur will have the message
    ///          Response: Will give you status code: 200, 204, 400, 500 or 404
    ///          Result: Will return true(if deleted) or false(if any error occur)
    ///      }
    /// </remarks>
    /// <returns>A newly created User</returns>
    /// <response code="200">Returns successfully delete user</response>
    /// <response code="204">Returns when invaild user</response>
    /// <response code="500">If any internal error occur</response>
    /// <response code="404">If any user not found</response>
    //Get api/Update
    [HttpPost, Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(204)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
     [Route("Delete")]
     public async Task<ActionResult<APIResponse<bool>>> Delete([Required]long Id)
     {
        var userResponse = new APIResponse<bool>();

        try
        {
            if(Id == 0){
                userResponse.Error = "Invalid model object";
                userResponse.Response = HttpStatusCode.NoContent;
                userResponse.Result = false;
                return StatusCode(204, userResponse);
            }

            var userDetail = await _userContext.UserRepository.FindByIdAsync(u => u.Id == Id);
            
            if(userDetail == null)
            {
                userResponse.Error = "No user detail found";
                userResponse.Response = HttpStatusCode.NotFound;
                userResponse.Result = false;
                return NotFound(userResponse);
            }
            else{

                _userContext.UserRepository.Delete(userDetail);
                var deleteUserResult = await _userContext.UserRepository.SaveAsync();
                if(deleteUserResult != 1){
                    userResponse.Error = "There is some problem during deleting the current details. Sorry for the inconvenience please try again";
                    userResponse.Response = HttpStatusCode.InternalServerError;
                    userResponse.Result = false;
                    return StatusCode(500, userResponse);
                }
                else{
                    userResponse.Response = HttpStatusCode.OK;
                    userResponse.Result = true;
                    return Ok(userResponse);
                }
            }
        }catch(Exception ex){
                userResponse.Error = "There is some internal problem. Sorry for the inconvenience";
                userResponse.Response = HttpStatusCode.InternalServerError;
                return StatusCode(500, userResponse);
                //ex will usefull in NLog
        }
     }
}