using Infra.Crosscutting.Helpers.Json.Utilities;
using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;

namespace Newtonsoft.Json.Converters
{
    /// <summary>
    /// Converts an <see cref="Enum"/> to and from its name string value.
    /// </summary>
    public class StringSnakeCaseEnumConverter : JsonConverter
    {
        /// <summary>
        /// Gets or sets a value indicating whether integer values are allowed when deserializing.
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if integers are allowed when deserializing; otherwise, <c>false</c>.</value>
        public bool AllowIntegerValues { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringEnumConverter"/> class.
        /// </summary>
        public StringSnakeCaseEnumConverter()
        {
            AllowIntegerValues = true;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Enum e = (Enum)value;

            if (!EnumUtils.TryToString(e.GetType(), value, /*false, true, */out string enumName))
            {
                if (!AllowIntegerValues)
                {
                    throw new JsonSerializationException($"Integer value {e.ToString("D")} is not allowed.");
                }

                // enum value has no name so write number
                writer.WriteValue(value);
            }
            else
            {
                writer.WriteValue(enumName);
            }
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (!ReflectionUtils.IsNullableType(objectType))
                {
                    throw new JsonSerializationException($"Cannot convert null value to {objectType}.");
                }

                return null;
            }

            bool isNullable = ReflectionUtils.IsNullableType(objectType);
            Type t = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            try
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string enumText = reader.Value.ToString();

                    if (enumText == string.Empty && isNullable)
                    {
                        return null;
                    }

                    return EnumUtils.ParseEnum(t, enumText, !AllowIntegerValues);
                }

                if (reader.TokenType == JsonToken.Integer)
                {
                    if (!AllowIntegerValues)
                    {
                        throw new JsonSerializationException($"Integer value {reader.Value} is not allowed.");
                    }

                    return ConvertUtils.ConvertOrCast(reader.Value, CultureInfo.InvariantCulture, t);
                }
            }
            catch (Exception)
            {
                throw new JsonSerializationException($"Error converting value {MiscellaneousUtils.FormatValueForPrint(reader.Value)} to type '{objectType}'.");
            }

            // we don't actually expect to get here.
            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing enum.");
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            Type t = (ReflectionUtils.IsNullableType(objectType))
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            return t.IsEnum();
        }
    }
}