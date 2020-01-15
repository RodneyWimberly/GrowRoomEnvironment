using Newtonsoft.Json;
using System.Linq;

namespace GrowRoomEnvironment.Core.Logging
{

    /// <summary>
    /// Represents a key value pair.
    /// </summary>
    public partial class Item
    {
        /// <summary>
        /// Initializes a new instance of the Item class.
        /// </summary>
        public Item()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Item class.
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <param name="value">The value of the item.</param>
        public Item(string key = default(string), string value = default(string))
        {
            Key = key;
            Value = value;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the key of the item.
        /// </summary>
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value of the item.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

    }
}
