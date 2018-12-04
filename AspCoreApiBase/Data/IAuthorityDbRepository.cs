namespace AspCoreBase.Data
{
	public interface IAuthorityDbRepository
	{
		bool SaveAll();
		void AddEntity(object model);
	}
}
