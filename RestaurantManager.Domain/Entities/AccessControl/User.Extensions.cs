using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace RestaurantManager.Domain.Entities.AccessControl;

public partial class User
{
    private Dictionary<string, object>? _additionalData;

    /// <summary>
    /// Obtiene los datos adicionales deserializados del campo Data
    /// </summary>
    [NotMapped]
    public Dictionary<string, object> AdditionalData
    {
        get
        {
            if (_additionalData == null)
            {
                try
                {
                    _additionalData = string.IsNullOrWhiteSpace(ExtraData)
                        ? new Dictionary<string, object>()
                        : JsonSerializer.Deserialize<Dictionary<string, object>>(ExtraData) ?? new Dictionary<string, object>();
                }
                catch (JsonException)
                {
                    _additionalData = new Dictionary<string, object>();
                }
            }
            return _additionalData;
        }
        set
        {
            _additionalData = value;
            ExtraData = JsonSerializer.Serialize(value ?? new Dictionary<string, object>());
        }
    }

    /// <summary>
    /// Obtiene un valor específico de los datos adicionales
    /// </summary>
    public T? GetAdditionalValue<T>(string key)
    {
        if (AdditionalData.TryGetValue(key, out var value))
        {
            if (value is JsonElement element)
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(element.GetRawText());
                }
                catch
                {
                    return default(T);
                }
            }

            if (value is T directValue)
            {
                return directValue;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }
        return default(T);
    }

    /// <summary>
    /// Establece un valor en los datos adicionales
    /// </summary>
    public void SetAdditionalValue<T>(string key, T value)
    {
        AdditionalData[key] = value ?? (object)string.Empty;
        ExtraData = JsonSerializer.Serialize(AdditionalData);
    }

    /// <summary>
    /// Elimina un valor de los datos adicionales
    /// </summary>
    public bool RemoveAdditionalValue(string key)
    {
        var removed = AdditionalData.Remove(key);
        if (removed)
        {
            ExtraData = JsonSerializer.Serialize(AdditionalData);
        }
        return removed;
    }

    /// <summary>
    /// Verifica si existe una clave en los datos adicionales
    /// </summary>
    public bool HasAdditionalValue(string key)
    {
        return AdditionalData.ContainsKey(key);
    }
}
