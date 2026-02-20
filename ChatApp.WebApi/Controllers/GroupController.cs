using System.Diagnostics.CodeAnalysis;
using ChatApp.Application.commands;
using ChatApp.Application.commons;
using ChatApp.Application.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/groups")]
[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpPost("create-a-group")]
    [Produces(typeof(Result<string>))]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand createGroupCommand)
    {
        var result = await _groupService.CreateGroupAsync(User, createGroupCommand);

        if (!result.IsSuccessful)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("add-member-to-group")]
    [Produces(typeof(Result<string>))]
    public async Task<IActionResult> AddMemberToGroup([FromBody] AddMemberToGroupCommand addMemberToGroupCommand)
    {
        var result = await _groupService.AddMemberToGroupAsync(User, addMemberToGroupCommand);

        if (!result.IsSuccessful)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("remove-member-from-group")]
    [Produces(typeof(Result<string>))]
    public async Task<IActionResult> RemoveMemberFromGroup(
        [FromBody] RemoveMemberFromGroupCommand removeMemberFromGroupCommand)
    {
        var result = await _groupService.RemoveMemberFromGroupAsync(User, removeMemberFromGroupCommand);

        if (!result.IsSuccessful)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("exit-from-group/{groupId:required}")]
    [Produces(typeof(Result<string>))]
    public async Task<IActionResult> ExitFromGroup(string groupId)
    {
        var result = await _groupService.ExitGroupAsync(User, groupId);

        if (!result.IsSuccessful)
            return BadRequest(result);

        return Ok(result);
    }
}