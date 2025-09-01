using AirportTicketBookingSystem.Common.Models;

namespace AirportTicketBookingSystem.Common.Helpers;

public static class FilterHelper
{
    public static Result<List<T>> FilterAsync<T>(
        List<T> items,
        params Func<T, bool>?[] predicates)
    {
        ArgumentNullException.ThrowIfNull(predicates);

        if (predicates.Any(p => p is null))
            throw new ArgumentException("Predicates array contains a null element.", nameof(predicates));
        
        Func<T, bool> combinedMatches = f => predicates.All(predicate => predicate!(f));
        return items.Where(combinedMatches).ToList();
    }
}