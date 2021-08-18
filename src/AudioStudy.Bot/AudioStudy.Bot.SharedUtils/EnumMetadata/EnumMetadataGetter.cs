using System;
using System.Collections.Generic;
using System.Linq;

namespace AudioStudy.Bot.SharedUtils.EnumMetadata {
    public class EnumMetadataGetter<TEnum, TMetadataAttribute, TMetadata> where TEnum : struct where TMetadataAttribute : EnumMetadataBaseAttribute {
        private readonly IEnumMetadataConverter<TMetadataAttribute, TMetadata> _defaultConverter;
        private readonly Func<TMetadataAttribute, EnumMetaDataValidationResult> _validator;
        private readonly TMetadataAttribute _defaultValue;
        private readonly Dictionary<TEnum, TMetadata> _defaultConverterCache = new Dictionary<TEnum, TMetadata>();
        private readonly Dictionary<Type, Dictionary<TEnum, TMetadata>> _convertersCache = new Dictionary<Type, Dictionary<TEnum, TMetadata>>();

        private readonly object _lock = new object();

        public EnumMetadataGetter(IEnumMetadataConverter<TMetadataAttribute, TMetadata> defaultConverter = null, Func<TMetadataAttribute, EnumMetaDataValidationResult> validator = null, TMetadataAttribute defaultValue = null) {
            _defaultConverter = defaultConverter;
            _validator = validator;
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// Получает метаданные для енума, кеширует полученные метаданные
        /// </summary>
        /// <param name="enumValue">значения, для которого нужно получить метаданные</param>
        /// <param name="converter">конвертер может быть использован если метаданные и атрибут с метаданными разные классы
        /// тогда в конвертер можно вставить дополнительную логику, напирмер по локализации, либо чтобы вставить значение по умолчанию
        /// </param>
        /// <returns></returns>
        public TMetadata GetMetadata(TEnum enumValue, IEnumMetadataConverter<TMetadataAttribute, TMetadata> converter = null) {
            if (_defaultConverter == null && converter == null) {
                throw new ArgumentNullException($"{nameof(_defaultConverter)} is not set, so you must specify {nameof(converter)}");
            }
            Dictionary<TEnum, TMetadata> cache = GetCache(converter);
            TMetadata metadata;
            if (cache.TryGetValue(enumValue, out metadata)) {
                return metadata;
            }
            lock (_lock) {
                if (cache.TryGetValue(enumValue, out metadata)) {
                    return metadata;
                }
                Type type = enumValue.GetType();
                if (!type.IsEnum) {
                    throw new ArgumentException("TEnum must be an enumerated type");
                }
                string name = Enum.GetName(type, enumValue);
                TMetadataAttribute attr = type.GetField(name)
                    .GetCustomAttributes(false)
                    .OfType<TMetadataAttribute>()
                    .SingleOrDefault() ?? _defaultValue;
                EnumMetaDataValidationResult validationResult = _validator?.Invoke(attr);
                if (validationResult?.Valid == false) {
                    throw new Exception(validationResult.Message ?? "Attribute is not in correct state");
                }
                if (converter != null) {
                    return converter.Convert(attr);
                }
                return _defaultConverter.Convert(attr);
            }
        }

        private Dictionary<TEnum, TMetadata> GetCache(IEnumMetadataConverter<TMetadataAttribute, TMetadata> converter) {
            if (converter == null) {
                return _defaultConverterCache;
            }
            Dictionary<TEnum, TMetadata> cache;
            Type converterType = converter.GetType();
            if (!_convertersCache.TryGetValue(converterType, out cache)) {
                lock (_lock) {
                    if (!_convertersCache.TryGetValue(converterType, out cache)) {
                        cache = new Dictionary<TEnum, TMetadata>();
                        _convertersCache[converterType] = cache;
                    }
                }
            }
            return cache;
        }
    }
}
