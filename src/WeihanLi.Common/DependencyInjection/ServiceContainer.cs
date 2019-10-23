using System.Collections;
using System.Collections.Generic;

namespace WeihanLi.Common.DependencyInjection
{
    public interface IServiceContainer : IList<ServiceDefinition>
    {
    }

    public class ServiceContainer : IServiceContainer
    {
        private readonly List<ServiceDefinition> _services = new List<ServiceDefinition>();

        public ServiceDefinition this[int index]
        {
            get => _services[index];
            set => _services[index] = value;
        }

        public int Count => _services.Count;

        public bool IsReadOnly => false;

        public void Add(ServiceDefinition item)
        {
            _services.Add(item);
        }

        public void Clear()
        {
            _services.Clear();
        }

        public bool Contains(ServiceDefinition item)
        {
            return _services.Contains(item);
        }

        public void CopyTo(ServiceDefinition[] array, int arrayIndex)
        {
            _services.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ServiceDefinition> GetEnumerator()
        {
            return _services.GetEnumerator();
        }

        public int IndexOf(ServiceDefinition item)
        {
            return _services.IndexOf(item);
        }

        public void Insert(int index, ServiceDefinition item)
        {
            _services.Insert(index, item);
        }

        public bool Remove(ServiceDefinition item)
        {
            return _services.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _services.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
