namespace Forge.Repository
{
	/// <summary>
	/// A function signature useful for setting up events that
	/// are related to the repository object in question
	/// </summary>
	/// <typeparam name="T">The type of object this event is to deal with</typeparam>
	/// <param name="obj">The object matching the type in the repository</param>
	public delegate void RepositoryChanged<T>(T obj);

	/// <summary>
	/// A repository that manages an object given an identifyable key
	/// </summary>
	/// <typeparam name="TKey">The key type used to identify the object</typeparam>
	/// <typeparam name="TObject">The object type managed by this repository</typeparam>
	public interface IRepository<TKey, TObject>
	{
		/// <summary>
		/// Useful when following the observer pattern and needing to know
		/// when an object has been newly added to this repository
		/// </summary>
		event RepositoryChanged<TObject> onAdd;

		/// <summary>
		/// Useful when following the observer pattern and needing to know
		/// when an object has been removed from this repository
		/// </summary>
		event RepositoryChanged<TObject> onRemove;

		/// <summary>
		/// Used for adding an object that matches the key and value types to
		/// be contained within this repository
		/// </summary>
		/// <param name="key">The key that is uniquely related to the object</param>
		/// <param name="obj">The object that is to be stored in this repository</param>
		void Add(TKey key, TObject obj);

		/// <summary>
		/// Used for removing an object from the repository, when the object reference
		/// is unknown, it can be removed by supplying the key that uniquely identifies
		/// it through this method
		/// </summary>
		/// <param name="key">The key that is uniquely related to the object</param>
		void Remove(TKey key);

		/// <summary>
		/// Used to check and see if an object is found within this repository given it's key
		/// </summary>
		/// <param name="key">The key that is uniquely related to the object</param>
		/// <returns><c>true</c> if the object is in the repository, otherwise <c>false</c></returns>
		bool Exists(TKey key);
	}
}
