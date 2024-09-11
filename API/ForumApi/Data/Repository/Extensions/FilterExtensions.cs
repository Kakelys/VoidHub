using ForumApi.DTO.Filter;
using Newtonsoft.Json.Linq;

namespace ForumApi.Data.Repository.Extensions;

public static class FilterExtensions
{
    public static object ConvertValue(object value, Type targetType)
    {
        try
        {
            if (value is JArray jArray)
            {
                var elementType = targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    ? targetType.GetGenericArguments()[0]
                    : targetType;

                var convertedValues = jArray.ToObject(elementType.MakeArrayType());

                return convertedValues;
            }

            return Convert.ChangeType(value, targetType);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unable to convert value '{value}' to type '{targetType}'", ex);
        }
    }

    public static T ToType<T>(this Criteria criteria)
    {
        return (T)ConvertValue(criteria.Value, typeof(T));
    }

    public static List<T> ToList<T>(this Criteria criteria)
    {
        return criteria.Op == "in" || criteria.Op == "nin"
            ? ((IEnumerable<T>)ConvertValue(criteria.Value, typeof(IEnumerable<T>))).ToList()
            : new List<T> { (T)ConvertValue(criteria.Value, typeof(T)) };
    }
}
