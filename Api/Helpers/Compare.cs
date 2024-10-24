using System.Collections;
using System.Reflection;

namespace Api.Helpers
{
    public static class ObjectComparer
    {
        /// <summary>
        /// Compares two objects of type T and recursively tracks changes in their properties.
        /// </summary>
        /// <typeparam name="T">The type of the objects to compare.</typeparam>
        /// <param name="original">The original object.</param>
        /// <param name="modified">The modified object.</param>
        /// <param name="changes">The list of changes detected between the objects.</param>
        /// <param name="path">The property path for tracking changes in nested objects.</param>
        public static void CompareObjectsRecursive<T>(T original, T modified, List<string> changes, string path = "")
        {
            // Check if either object is null and throw an appropriate exception
            if (original == null)
                throw new ArgumentNullException(nameof(original), "Original object is null");
        
            if (modified == null)
                throw new ArgumentNullException(nameof(modified), "Modified object is null");

            // Check if both objects are of the same exact type
            if (original.GetType() != modified.GetType())
                throw new InvalidOperationException("Objects are of different types and cannot be compared.");

            // Get properties of the type
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var originalValue = property.GetValue(original);
                var modifiedValue = property.GetValue(modified);

                var currentPath = string.IsNullOrEmpty(path) ? property.Name : $"{path}.{property.Name}";

                // Handle null values
                if (originalValue == null && modifiedValue == null)
                {
                    continue;
                }

                if (originalValue == null || modifiedValue == null)
                {
                    changes.Add($"Property '{currentPath}' changed from '{originalValue ?? "null"}' to '{modifiedValue ?? "null"}'");
                    continue;
                }

                // Check if the property is a collection (like List, Array, etc.)
                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && !(originalValue is string))
                {
                    CompareCollections((IEnumerable)originalValue, (IEnumerable)modifiedValue, currentPath, changes);
                }
                else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    // Recursively compare nested complex objects
                    CompareObjectsRecursive(originalValue, modifiedValue, changes, currentPath);
                }
                else
                {
                    // Compare primitive types and value types
                    if (!originalValue.Equals(modifiedValue))
                    {
                        changes.Add($"Property '{currentPath}' changed from '{originalValue}' to '{modifiedValue}'");
                    }
                }
            }
        }

        /// <summary>
        /// Compares two collections (e.g., lists, arrays) and tracks changes.
        /// </summary>
        /// <param name="original">The original collection.</param>
        /// <param name="modified">The modified collection.</param>
        /// <param name="path">The property path for tracking changes in nested objects.</param>
        /// <param name="changes">The list of changes detected between the objects.</param>
        private static void CompareCollections(IEnumerable original, IEnumerable modified, string path, List<string> changes)
        {
            var originalList = original.Cast<object>().ToList();
            var modifiedList = modified.Cast<object>().ToList();

            if (originalList.Count != modifiedList.Count)
            {
                changes.Add($"Collection '{path}' size changed from {originalList.Count} to {modifiedList.Count}");
                return;
            }

            for (int i = 0; i < originalList.Count; i++)
            {
                var originalItem = originalList[i];
                var modifiedItem = modifiedList[i];

                if (originalItem == null && modifiedItem == null)
                {
                    continue;
                }

                var currentPath = $"{path}[{i}]";

                if (originalItem == null || modifiedItem == null)
                {
                    changes.Add($"Item at '{currentPath}' changed from '{originalItem ?? "null"}' to '{modifiedItem ?? "null"}'");
                    continue;
                }

                // If items are complex objects, compare them recursively
                if (originalItem.GetType().IsClass && originalItem.GetType() != typeof(string))
                {
                    CompareObjectsRecursive(originalItem, modifiedItem, changes, currentPath);
                }
                else
                {
                    // Compare primitive items in the collection
                    if (!originalItem.Equals(modifiedItem))
                    {
                        changes.Add($"Item at '{currentPath}' changed from '{originalItem}' to '{modifiedItem}'");
                    }
                }
            }
        }
    }
}
