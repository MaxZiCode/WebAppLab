namespace WebAppLab.Helpers
{
	public interface IChecker<T>
	{
		public bool Check(T value);
	}
}
