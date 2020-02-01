using System;
using System.Collections.Generic;

namespace Cocona
{
    public class CoconaAppFeatureCollection
    {
        private readonly Dictionary<Type, object?> _feature = new Dictionary<Type, object?>();

        /// <summary>
        /// Sets the feature in the collection.
        /// </summary>
        /// <typeparam name="TFeature"></typeparam>
        /// <param name="feature"></param>
        public void Set<TFeature>(TFeature feature)
            where TFeature : class
            => _feature[typeof(TFeature)] = feature;

        /// <summary>
        /// Gets the requested feature from the collection.
        /// </summary>
        /// <typeparam name="TFeature"></typeparam>
        /// <returns></returns>
        public TFeature Get<TFeature>()
            where TFeature: class
            => _feature.TryGetValue(typeof(TFeature), out var feature) ? (TFeature)feature! : default!;
    }
}
