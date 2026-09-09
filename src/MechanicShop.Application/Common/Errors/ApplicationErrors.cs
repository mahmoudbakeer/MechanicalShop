using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Common.Errors;

public static class ApplicationErrors
{
    public static Error WorkOrderMustBeCompletedForInvoicing =>
        Error.Conflict(
            "WorkOrder.InvoiceIssuance.InvalidState",
            "WorkOrder must be in 'Completed' state to issue an invoice."
        );

    public static Error WorkOrderTimeOutsideWorkingHours(
        DateTimeOffset startAt,
        DateTimeOffset endAt
    ) =>
        Error.Conflict(
            "ApplicationErrors_WorkOrder_Time_Outside_OperatingHours.",
            $"WorkOrder with start time {startAt} and end time {endAt} is outside the shop operating hours."
        );

    public static Error LaborOccupied =>
        Error.Conflict(
            "ApplicationErrors_Employee_Occupied.",
            $"The labor is currently occupied and cannot be assigned to another task."
        );
    public static Error WorkOrderNotFound =>
        Error.NotFound("ApplicationErrors_WorkOrder_NotFound.", $"WorkOrder does not exist.");
    public static Error CustomerNotFound =>
        Error.NotFound("ApplicationErrors_Customer_NotFound.", $"Customer does not exist.");
    public static Error VehicleNotFound =>
        Error.NotFound("ApplicationErrors_Vehicle_NotFound.", $"Vehicle does not exist.");
    public static Error RepairTaskNotFound =>
        Error.NotFound("ApplicationErrors_RepairTask_NotFound.", $"RepairTask does not exist.");
    public static Error InvoiceNotFound =>
        Error.NotFound("ApplicationErrors_Invoice_NotFound.", $"Invoice does not exist.");

    public static Error VehicleSchedulingConflict =>
        Error.Conflict(
            "ApplicationErrors_Vehicle_Scheduling_Conflict.",
            "Vehicle already has Schedule at this time."
        );

    public static Error SpotNotAvailable(DateTimeOffset startAt, DateTimeOffset endAt) =>
        Error.Conflict(
            "ApplicationErrors_Spot_UnAvailable_Conflict.",
            $"Spot is not available at the specified time {startAt} - {endAt}."
        );

    public static Error InvalidRefreshToken =>
        Error.Validation("ApplicationErrors_RefreshToken_Invalid.", "Refresh token invalid.");
    public static Error ExpiredAccessTokenInvalid =>
        Error.Conflict(
            "ApplicationErrors_ExpiredAccessToken_Invalid.",
            "expired AccessToken invalid."
        );
    public static Error RefreshTokenExpired =>
        Error.Conflict(
            "ApplicationErrors_RefreshToken_Expired.",
            "RefreshToken expired and it's not valid."
        );

    public static Error LaborNotFound =>
        Error.Conflict("ApplicationErrors_Employee_NotFound", "Labor does not exist.");
    public static Error UserNotFound =>
        Error.Conflict("ApplicationErrors_Auth_User_NotFound.", "User does not exist.");
    public static Error FaildToGenerateToken =>
        Error.Failure(
            "ApplicationErrors_Auth_Token_Generation_Failed.",
            "Token generation failed."
        );
}
