using System.Collections.Generic;

namespace X_UniTMX
{
	public class PropertyContainer
	{
		/// <summary>
		/// Gets the list of properties.
		/// </summary>
		public PropertyCollection Properties { get; protected set; }

		/// <summary>
		/// Gets a string property
		/// </summary>
		/// <param name="property">Name of the property inside Tiled</param>
		/// <returns>The value of the property, String.Empty if property not found</returns>
		public string GetPropertyAsString(string property)
		{
			if (Properties == null)
				return string.Empty;
			return Properties.GetPropertyAsString(property);
		}
		/// <summary>
		/// Gets a boolean property
		/// </summary>
		/// <param name="property">Name of the property inside Tiled</param>
		/// <returns>The value of the property</returns>
		public bool GetPropertyAsBoolean(string property)
		{
			if (Properties == null)
				return false;
			return Properties.GetPropertyAsBoolean(property);
		}
		/// <summary>
		/// Gets an integer property
		/// </summary>
		/// <param name="property">Name of the property inside Tiled</param>
		/// <returns>The value of the property</returns>
		public int GetPropertyAsInt(string property)
		{
			if (Properties == null)
				return 0;
			return Properties.GetPropertyAsInt(property);
		}
		/// <summary>
		/// Gets a float property
		/// </summary>
		/// <param name="property">Name of the property inside Tiled</param>
		/// <returns>The value of the property</returns>
		public float GetPropertyAsFloat(string property)
		{
			if (Properties == null)
				return 0;
			return Properties.GetPropertyAsFloat(property);
		}

		/// <summary>
		/// Checks if a property exists
		/// </summary>
		/// <param name="property">Name of the property inside Tiled</param>
		/// <returns>true if property exists, false otherwise</returns>
		public bool HasProperty(string property)
		{
			if (Properties == null)
				return false;
			Property p;
			if (Properties.TryGetValue(property.ToLowerInvariant(), out p))
				return true;
			return false;
		}
	}
}
