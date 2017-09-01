namespace TestConsole.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public interface IConfigurationEntry<out T>
    {
        T Value { get; }

        event Action OnChanged;
    }

    public class ConfigurationEntry<T> : IConfigurationEntry<T>
    {
        private string _key;

        private Func<object, T> _converter;

        public event Action OnChanged;

        public T Value { get; private set; }

        private void Refresh(IReadOnlyDictionary<string, object> config, bool silent)
        {
            object value;
            if (config.TryGetValue(_key, out value))
            {
                try
                {
                    T typedValue = _converter != null ? _converter(value) : (T)value;
                    if (!object.Equals(Value, typedValue))
                    {
                        TraceEx.TraceInformation(string.Format("Configuration '{0}' is changed from '{1}' to '{2}'.", _key, Value, typedValue));
                        Value = typedValue;
                        if (OnChanged != null)
                        {
                            OnChanged();
                        }
                    }
                }
                catch (InvalidCastException)
                {
                    TraceEx.TraceWarning(string.Format("Cannot cast value '{0}' to type '{1}', original value is kept.", value, typeof(T)));
                    if (!silent)
                    {
                        throw;
                    }
                }
            }
            else
            {
                TraceEx.TraceWarning(string.Format("Cannot find configuration '{0}', original value is kept.", _key));
                if (!silent)
                {
                    throw new KeyNotFoundException(string.Format("Cannot find configuration '{0}'", _key));
                }
            }
        }

        public ConfigurationEntry(string key, IConfigurationEntry<IReadOnlyDictionary<string, object>> config, Func<object, T> converter = null)
        {
            _key = key;
            _converter = converter;
            Refresh(config.Value, false);
            config.OnChanged += () => Refresh(config.Value, true);
        }

        public ConfigurationEntry(string key, IConfigurationEntry<IReadOnlyDictionary<string, object>> config, T defaultValue, Func<object, T> converter = null)
        {
            _key = key;
            _converter = converter;
            Value = defaultValue;
            Refresh(config.Value, true);
            config.OnChanged += () => Refresh(config.Value, true);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator T(ConfigurationEntry<T> entry)
        {
            return entry.Value;
        }
    }

    public static class ConfigurationExtensions
    {
        public static ConfigurationEntry<T> CreateConfigEntry<T>(this IConfigurationEntry<IReadOnlyDictionary<string, object>> config, string name)
        {
            return new ConfigurationEntry<T>(name, config);
        }

        public static ConfigurationEntry<T> CreateConfigEntry<T>(this IConfigurationEntry<IReadOnlyDictionary<string, object>> config, string name, T defaultValue)
        {
            return new ConfigurationEntry<T>(name, config, defaultValue);
        }

        public static ConfigurationEntry<T[]> CreateArrayEntry<T>(this IConfigurationEntry<IReadOnlyDictionary<string, object>> config, string name)
        {
            return new ConfigurationEntry<T[]>(name, config, o => ((object[])o).Cast<T>().ToArray());
        }

        public static ConfigurationEntry<T[]> CreateArrayEntry<T>(this IConfigurationEntry<IReadOnlyDictionary<string, object>> config, string name, T[] defaultValue)
        {
            return new ConfigurationEntry<T[]>(name, config, defaultValue, o => ((object[])o).Cast<T>().ToArray());
        }

        public static ConfigurationEntry<IReadOnlyDictionary<string, T>> CreateDictionaryEntry<T>(this IConfigurationEntry<IReadOnlyDictionary<string, object>> config, string name)
        {
            return new ConfigurationEntry<IReadOnlyDictionary<string, T>>(name, config, o => ((Dictionary<string, object>)o).ToDictionary(p => p.Key, p => (T)p.Value));
        }

        public static ConfigurationEntry<IReadOnlyDictionary<string, T>> CreateDictionaryEntry<T>(this IConfigurationEntry<IReadOnlyDictionary<string, object>> config, string name, IReadOnlyDictionary<string, T> defaultValue)
        {
            return new ConfigurationEntry<IReadOnlyDictionary<string, T>>(name, config, defaultValue, o => ((Dictionary<string, object>)o).ToDictionary(p => p.Key, p => (T)p.Value));
        }

        public static ConfigurationEntry<IReadOnlyDictionary<string, T[]>> CreateDictionaryOfArrayEntry<T>(this IConfigurationEntry<IReadOnlyDictionary<string, object>> config, string name, IReadOnlyDictionary<string, T[]> defaultValue)
        {
            return new ConfigurationEntry<IReadOnlyDictionary<string, T[]>>(
                name,
                config,
                defaultValue,
                o => ((Dictionary<string, object>)o).ToDictionary(p => p.Key, p => ((object[])p.Value)?.Cast<T>().ToArray()));
        }
    }
}
