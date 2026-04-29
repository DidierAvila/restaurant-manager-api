namespace RestaurantManager.Application.DTOs.Common;

/// <summary>
/// DTO genérico de respuesta para APIs
/// </summary>
/// <typeparam name="T">Tipo de datos de la respuesta</typeparam>
public class ResponseDto<T>
{
    /// <summary>
    /// Indica si la operación fue exitosa
    /// </summary>
    public bool IsSuccess { get; set; } = true;

    /// <summary>
    /// Mensaje descriptivo de la respuesta
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Datos de la respuesta
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Errores adicionales si los hay
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// Constructor para respuesta exitosa
    /// </summary>
    /// <param name="data">Datos de la respuesta</param>
    /// <param name="message">Mensaje opcional</param>
    public static ResponseDto<T> Success(T data, string message = "")
    {
        return new ResponseDto<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// Constructor para respuesta de error
    /// </summary>
    /// <param name="message">Mensaje de error</param>
    /// <param name="errors">Lista de errores adicionales</param>
    public static ResponseDto<T> Error(string message, List<string>? errors = null)
    {
        return new ResponseDto<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors
        };
    }
}
