using DriveInsight.Api.Dtos.Drives;
using DriveInsight.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DriveInsight.Api.Controllers;

[ApiController]
[Route("api/v1/drives")]
public class DrivesController : ControllerBase
{
    private readonly IDriveService driveService;

    public DrivesController(IDriveService driveService)
    {
        this.driveService = driveService;
    }

    [HttpPost]
    public async Task<ActionResult<CreateDriveResponse>> CreateDrive(
        CreateDriveRequest request,
        CancellationToken cancellationToken)
    {
        var response = await driveService.CreateDriveAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetDrive), new { id = response.DriveId }, response);
    }

    [HttpPost("{id:guid}/readings")]
    public async Task<ActionResult<AddReadingsResponse>> AddReadings(
        Guid id,
        AddReadingsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await driveService.AddReadingsAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (DriveServiceException exception)
        {
            return ToErrorResponse(exception);
        }
    }

    [HttpPost("{id:guid}/finish")]
    public async Task<ActionResult<FinishDriveResponse>> FinishDrive(
        Guid id,
        FinishDriveRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await driveService.FinishDriveAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (DriveServiceException exception)
        {
            return ToErrorResponse(exception);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<DriveListItemResponse>>> GetDrives(CancellationToken cancellationToken)
    {
        var response = await driveService.GetDrivesAsync(cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DriveDetailResponse>> GetDrive(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await driveService.GetDriveAsync(id, cancellationToken);
            return Ok(response);
        }
        catch (DriveServiceException exception)
        {
            return ToErrorResponse(exception);
        }
    }

    private ActionResult ToErrorResponse(DriveServiceException exception) => exception.Error switch
    {
        DriveServiceError.NotFound => NotFound(new { error = exception.Message }),
        DriveServiceError.Conflict => Conflict(new { error = exception.Message }),
        DriveServiceError.Validation => BadRequest(new { error = exception.Message }),
        _ => StatusCode(StatusCodes.Status500InternalServerError, new { error = "Unexpected drive service error." })
    };
}
