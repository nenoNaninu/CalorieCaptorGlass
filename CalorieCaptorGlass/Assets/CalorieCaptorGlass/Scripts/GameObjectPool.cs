using System.Collections;
using System.Collections.Generic;

namespace CalorieCaptorGlass
{
    public class GameObjectPool<T> : IReadOnlyList<T> where T : class
    {
        protected readonly List<T> _gameObjectList;
        protected readonly List<bool> _usedFlag;

        public IReadOnlyList<T> GameObjectList => _gameObjectList;
        public IReadOnlyList<bool> UsedFlag => _usedFlag;

        public int Count => _gameObjectList.Count;
        public int Capacity => _gameObjectList.Capacity;

        public GameObjectPool(int capacity = 20)
        {
            _gameObjectList = new List<T>(capacity);
            _usedFlag = new List<bool>(capacity);
        }

        public virtual void Add(T obj)
        {
            _gameObjectList.Add(obj);
            _usedFlag.Add(false);
        }

        public T Rent()
        {
            for (int i = 0; i < _usedFlag.Count; i++)
            {
                if (!_usedFlag[i])
                {
                    _usedFlag[i] = true;
                    return _gameObjectList[i];
                }
            }

            return null;
        }

        public void Return(T gameObject)
        {
            for (int i = 0; i < _gameObjectList.Count; i++)
            {
                if (ReferenceEquals(gameObject, _gameObjectList[i]))
                {
                    _usedFlag[i] = false;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _gameObjectList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int index] => this._gameObjectList[index];
    }
}