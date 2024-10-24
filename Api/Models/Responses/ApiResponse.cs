namespace Api.Models.Responses;

public class ApiResponse
{
    /// <summary>
    /// Indicates whether the request was successful or not.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// A message, typically used to convey any error or success details.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Optional: Additional details about any errors that occurred.
    /// </summary>
    public List<string>? Errors { get; init; }

    /// <summary>
    /// Static method to create a success response without data.
    /// </summary>
    public static ApiResponse Succeeded(string message = "Operation Successful")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    /// <summary>
    /// Static method to create a failure response without data.
    /// </summary>
    public static ApiResponse Failed(string message, List<string>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

public class ApiResponse<T> : ApiResponse
{
    /// <summary>
    /// The data being returned in the response (if any).
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Static method to create a success response with optional data.
    /// </summary>
    public static ApiResponse<T?> Succeeded(string message = "Operation Successful", T? data = default)
    {
        return new ApiResponse<T?>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// Static method to create a failure response with optional data.
    /// </summary>
    public static ApiResponse<T?> Failure(string message, List<string>? errors = null, T? data = default)
    {
        return new ApiResponse<T?>
        {
            Success = false,
            Data = data,
            Message = message,
            Errors = errors
        };
    }
}