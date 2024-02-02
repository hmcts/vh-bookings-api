using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V2.Requests;
namespace BookingsApi.Extensions;

public static class ContractExtensions
{
    public static void SanitizeRequest(this BookNewHearingRequest request)
    {
        TrimAllStringsRecursively(request);
    }
    
    public static void SanitizeRequest(this BookNewHearingRequestV2 request)
    {
        TrimAllStringsRecursively(request);
    }

    /// <summary>
    /// Trim all strings in an object recursively
    /// </summary>
    /// <param name="obj"></param>
    public static void TrimAllStringsRecursively(this object obj)
    {
        if(obj == null)
            return;
        
        // if obj is a list
        if (obj is IEnumerable<object> list)
        {
            foreach (var item in list)
            {
                TrimAllStringsRecursively(item);
            }
            return;
        }
        
        var stringProperties = obj.GetType().GetProperties()
            .Where(p => p.PropertyType == typeof (string)).ToList();

        foreach (var stringProperty in stringProperties)
        {
            var currentValue = (string) stringProperty.GetValue(obj, null);
            stringProperty.SetValue(obj, currentValue?.Trim(), null) ;
        }

        var objectProperties = obj.GetType().GetProperties()
            .Where(p => !p.PropertyType.IsPrimitive &&
                        p.PropertyType != typeof(string) && p.PropertyType != typeof(DateTime)).ToList();

        foreach (var objectProperty in objectProperties)
        {
            var currentValue = objectProperty.GetValue(obj, null);
            TrimAllStringsRecursively(currentValue);
        }
    }
    
    public static JudiciaryParticipantHearingRoleCode MapToDomainEnum(this Contract.V1.Requests.Enums.JudiciaryParticipantHearingRoleCode hearingRoleCode)
    {
        return Enum.Parse<JudiciaryParticipantHearingRoleCode>(hearingRoleCode.ToString());
    }
}