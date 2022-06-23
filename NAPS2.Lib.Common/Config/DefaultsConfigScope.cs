using System.Linq.Expressions;

namespace NAPS2.Config;

public class DefaultsConfigScope<TConfig> : ConfigScope<TConfig>
{
    private readonly ConfigStorage<TConfig> _storage;

    public DefaultsConfigScope(TConfig defaults) : base(ConfigScopeMode.ReadOnly)
    {
        _storage = new();
        // TODO
        //_storage = new(defaults);
    }

    protected override bool TryGetInternal<T>(Expression<Func<TConfig, T>> accessor, out T value)
    {
        return _storage.TryGet(accessor, out value);
    }

    protected override void SetInternal<T>(Expression<Func<TConfig, T>> accessor, T value) =>
        throw new InvalidOperationException();

    protected override void RemoveInternal<T>(Expression<Func<TConfig, T>> accessor) =>
        throw new InvalidOperationException();

    protected override void CopyFromInternal(ConfigStorage<TConfig> source) =>
        throw new InvalidOperationException();
}