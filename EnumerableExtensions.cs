namespace BleBatteryRecorder;

static internal class EnumerableExtensions
{
	public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> enumerable)
	{
		return enumerable.Select((item, index) => (item, index));
	}
}
