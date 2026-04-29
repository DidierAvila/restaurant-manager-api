using System.Linq.Expressions;
using System.Reflection;

namespace RestaurantManager.Application.Features.Common.Pagination;

/// <summary>
/// Builder para construir expresiones de filtrado de forma fluida
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public class FilterExpressionBuilder<T>
{
    private Expression<Func<T, bool>> _expression = x => true;

    /// <summary>
    /// Añade una condición AND a la expresión
    /// </summary>
    /// <param name="condition">Condición a evaluar</param>
    /// <param name="expression">Expresión a añadir si la condición es verdadera</param>
    /// <returns>Builder para encadenamiento</returns>
    public FilterExpressionBuilder<T> AndIf(bool condition, Expression<Func<T, bool>> expression)
    {
        if (condition)
        {
            _expression = CombineAnd(_expression, expression);
        }
        return this;
    }

    /// <summary>
    /// Añade una condición AND para strings no vacíos
    /// </summary>
    /// <param name="value">Valor a evaluar</param>
    /// <param name="expression">Expresión a añadir si el valor no está vacío</param>
    /// <returns>Builder para encadenamiento</returns>
    public FilterExpressionBuilder<T> AndIfNotEmpty(string? value, Expression<Func<T, bool>> expression)
    {
        return AndIf(!string.IsNullOrWhiteSpace(value), expression);
    }

    /// <summary>
    /// Añade una condición AND para valores nullable que tienen valor
    /// </summary>
    /// <param name="value">Valor nullable a evaluar</param>
    /// <param name="expression">Expresión a añadir si el valor tiene un valor</param>
    /// <returns>Builder para encadenamiento</returns>
    public FilterExpressionBuilder<T> AndIfHasValue<TValue>(TValue? value, Expression<Func<T, bool>> expression)
        where TValue : struct
    {
        return AndIf(value.HasValue, expression);
    }

    /// <summary>
    /// Añade una condición AND para objetos no nulos
    /// </summary>
    /// <param name="value">Valor a evaluar</param>
    /// <param name="expression">Expresión a añadir si el valor no es nulo</param>
    /// <returns>Builder para encadenamiento</returns>
    public FilterExpressionBuilder<T> AndIfNotNull<TValue>(TValue? value, Expression<Func<T, bool>> expression)
        where TValue : class
    {
        return AndIf(value != null, expression);
    }

    /// <summary>
    /// Añade una condición de búsqueda de texto en múltiples campos
    /// </summary>
    /// <param name="searchTerm">Término de búsqueda</param>
    /// <param name="propertySelectors">Selectores de propiedades donde buscar</param>
    /// <returns>Builder para encadenamiento</returns>
    public FilterExpressionBuilder<T> AndSearchIn(string? searchTerm, params Expression<Func<T, string>>[] propertySelectors)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || propertySelectors.Length == 0)
            return this;

        var searchExpression = BuildSearchExpression(searchTerm, propertySelectors);
        return AndIf(true, searchExpression);
    }

    /// <summary>
    /// Añade una condición de rango de fechas
    /// </summary>
    /// <param name="dateFrom">Fecha desde</param>
    /// <param name="dateTo">Fecha hasta</param>
    /// <param name="propertySelector">Selector de la propiedad de fecha</param>
    /// <returns>Builder para encadenamiento</returns>
    public FilterExpressionBuilder<T> AndDateRange(DateTime? dateFrom, DateTime? dateTo, Expression<Func<T, DateTime>> propertySelector)
    {
        if (dateFrom.HasValue)
        {
            AndIf(true, CombinePropertyComparison(propertySelector, dateFrom.Value, ExpressionType.GreaterThanOrEqual));
        }

        if (dateTo.HasValue)
        {
            AndIf(true, CombinePropertyComparison(propertySelector, dateTo.Value, ExpressionType.LessThanOrEqual));
        }

        return this;
    }

    /// <summary>
    /// Añade una condición de rango numérico
    /// </summary>
    /// <param name="min">Valor mínimo</param>
    /// <param name="max">Valor máximo</param>
    /// <param name="propertySelector">Selector de la propiedad numérica</param>
    /// <returns>Builder para encadenamiento</returns>
    public FilterExpressionBuilder<T> AndNumericRange<TNumeric>(TNumeric? min, TNumeric? max, Expression<Func<T, TNumeric>> propertySelector)
        where TNumeric : struct, IComparable<TNumeric>
    {
        if (min.HasValue)
        {
            AndIf(true, CombinePropertyComparison(propertySelector, min.Value, ExpressionType.GreaterThanOrEqual));
        }

        if (max.HasValue)
        {
            AndIf(true, CombinePropertyComparison(propertySelector, max.Value, ExpressionType.LessThanOrEqual));
        }

        return this;
    }

    /// <summary>
    /// Construye la expresión final
    /// </summary>
    /// <returns>Expresión combinada</returns>
    public Expression<Func<T, bool>> Build()
    {
        return _expression;
    }

    /// <summary>
    /// Combina dos expresiones con AND
    /// </summary>
    private static Expression<Func<T, bool>> CombineAnd(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));
        var body = Expression.AndAlso(
            Expression.Invoke(expr1, parameter),
            Expression.Invoke(expr2, parameter));

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    /// <summary>
    /// Construye una expresión de búsqueda en múltiples propiedades
    /// </summary>
    private static Expression<Func<T, bool>> BuildSearchExpression(string searchTerm, Expression<Func<T, string>>[] propertySelectors)
    {
        var parameter = Expression.Parameter(typeof(T));
        var searchTermConstant = Expression.Constant(searchTerm.ToLower());

        Expression? combinedExpression = null;

        foreach (var propertySelector in propertySelectors)
        {
            var property = Expression.Invoke(propertySelector, parameter);
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            var propertyToLower = Expression.Call(property, toLowerMethod!);
            var containsExpression = Expression.Call(propertyToLower, containsMethod!, searchTermConstant);

            combinedExpression = combinedExpression == null 
                ? containsExpression 
                : Expression.OrElse(combinedExpression, containsExpression);
        }

        return Expression.Lambda<Func<T, bool>>(combinedExpression!, parameter);
    }

    /// <summary>
    /// Combina una propiedad con un valor usando el tipo de comparación especificado
    /// </summary>
    private static Expression<Func<T, bool>> CombinePropertyComparison<TProperty>(
        Expression<Func<T, TProperty>> propertySelector, 
        TProperty value, 
        ExpressionType comparisonType)
    {
        var parameter = Expression.Parameter(typeof(T));
        var property = Expression.Invoke(propertySelector, parameter);
        var constant = Expression.Constant(value);
        var comparison = Expression.MakeBinary(comparisonType, property, constant);

        return Expression.Lambda<Func<T, bool>>(comparison, parameter);
    }
}

/// <summary>
/// Métodos de extensión para facilitar el uso del FilterExpressionBuilder
/// </summary>
public static class FilterExpressionBuilderExtensions
{
    /// <summary>
    /// Crea un nuevo FilterExpressionBuilder para el tipo especificado
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <returns>Nuevo builder</returns>
    public static FilterExpressionBuilder<T> CreateFilter<T>()
    {
        return new FilterExpressionBuilder<T>();
    }
}