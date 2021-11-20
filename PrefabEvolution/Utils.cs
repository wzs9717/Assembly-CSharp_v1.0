namespace PrefabEvolution
{
	public static class Utils
	{
		public static bool IsBuildingPlayer => false;

		public static T Create<T>() where T : class, new()
		{
			return (T)null;
		}
	}
}
