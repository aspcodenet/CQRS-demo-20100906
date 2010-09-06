using System;
using System.IO;

namespace Eventing.Serializing.Internal
{
	/// <summary>
	/// A SerializationReader instance is used to read stored values and objects from a byte array.
	///
	/// Once an instance is created, use the various methods to read the required data.
	/// The data read MUST be exactly the same type and in the same order as it was written.
	/// </summary>
	public sealed class SerializationReader: BinaryReader
	{

        public SerializationReader(Stream s)
            :base(s)
        {

        }

	    /// <summary>
		/// Returns a DateTime value from the stream.
		/// </summary>
		/// <returns>A DateTime value.</returns>
		public DateTime ReadDateTime()
		{
			return DateTime.FromBinary(ReadInt64());
		}

		/// <summary>
		/// Returns a Guid value from the stream.
		/// </summary>
		/// <returns>A DateTime value.</returns>
		public Guid ReadGuid()
		{
			return new Guid(ReadBytes(16));
		}

	}
}
