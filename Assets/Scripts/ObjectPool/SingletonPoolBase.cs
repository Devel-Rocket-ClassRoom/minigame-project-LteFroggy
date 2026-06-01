using UnityEngine;
using UnityEngine.Pool;

public abstract class SingletonPoolBase<TSelf, TItem> : Singleton<TSelf>
	where TSelf : SingletonPoolBase<TSelf, TItem>
	where TItem : MonoBehaviour
{
	[SerializeField] private TItem _prefab;
	private ObjectPool<TItem> _pool;

	protected override void Awake() {
		base.Awake();
		_pool = new ObjectPool<TItem>(
			createFunc: () => Instantiate(_prefab, transform),
			actionOnGet:     item => item.gameObject.SetActive(true),
			actionOnRelease: item => item.gameObject.SetActive(false),
			actionOnDestroy: item => Destroy(item.gameObject)
		);
	}

	protected TItem Get() => _pool.Get();
	
	public void Release(TItem item) => _pool.Release(item);
}