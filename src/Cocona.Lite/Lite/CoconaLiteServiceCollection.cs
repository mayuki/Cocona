using System.Collections;

namespace Cocona.Lite
{
    public interface ICoconaLiteServiceCollection : IList<ServiceDescriptor>, IEnumerable<ServiceDescriptor>
    {
    }

    public class CoconaLiteServiceCollection : ICoconaLiteServiceCollection
    {
        private readonly List<ServiceDescriptor> _descriptors = new List<ServiceDescriptor>();

        public IEnumerator<ServiceDescriptor> GetEnumerator()
            => _descriptors.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void Add(ServiceDescriptor item)
            => _descriptors.Add(item);

        public void Clear()
            => _descriptors.Clear();

        public bool Contains(ServiceDescriptor item)
            => _descriptors.Contains(item);

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
            => _descriptors.CopyTo(array, arrayIndex);

        public bool Remove(ServiceDescriptor item)
            => _descriptors.Remove(item);

        public int Count => _descriptors.Count;

        public bool IsReadOnly => ((ICollection<ServiceDescriptor>)_descriptors).IsReadOnly;

        public int IndexOf(ServiceDescriptor item)
            => _descriptors.IndexOf(item);

        public void Insert(int index, ServiceDescriptor item)
            => _descriptors.Insert(index, item);

        public void RemoveAt(int index)
            => _descriptors.RemoveAt(index);

        public ServiceDescriptor this[int index]
        {
            get => _descriptors[index];
            set => _descriptors[index] = value;
        }
    }

    public class ServiceDescriptor
    {
        public Type ServiceType { get; }
        public Func<IServiceProvider, List<IDisposable>, object> Factory { get; private set; }

        public ServiceDescriptor(Type serviceType, Func<IServiceProvider, List<IDisposable>, object> factory, bool singleton)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            if (singleton)
            {
                Factory = (services, disposables) =>
                {
                    var instance = factory(services, disposables);
                    Factory = (_, __) => instance;
                    return instance;
                };
            }
            else
            {
                Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            }
        }
    }
}
