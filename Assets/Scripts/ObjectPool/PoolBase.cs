
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolBase<T> : MonoBehaviour where T : MonoBehaviour {
	[SerializeField] private T _prefab;
	
	private readonly Stack<T> _pool = new();
	
	public T Get(Transform parent = null) {
		// 풀에 오브젝트가 없으면, 새로 만들어 주기
		T result = _pool.Count == 0 ? Instantiate(_prefab) : _pool.Pop();
		
		result.transform.SetParent(parent);
		result.transform.localScale = Vector3.one;
		result.gameObject.SetActive(true);
		
		return result;
	}
	
	public void Release(T obj) {
		obj.gameObject.SetActive(false);
		_pool.Push(obj);
	}
}