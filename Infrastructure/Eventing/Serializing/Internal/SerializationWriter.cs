#define THROW_IF_NOT_OPTIMIZABLE

using System;
using System.Collections.Specialized;
using System.IO;

// ReSharper disable PossibleInvalidCastException
// ReSharper disable PossibleNullReferenceException
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode

namespace Eventing.Serializing.Internal
{
	/// <summary>
	/// Class which defines the writer for serialized data using the fast serialization optimization.
	/// A SerializationWriter instance is used to store values and objects in a byte array.
	/// <br/><br/>
	/// Once an instance is created, use the various methods to store the required data.
	/// ToArray() will return a byte[] containing all of the data required for deserialization.
	/// This can be stored in the SerializationInfo parameter in an ISerializable.GetObjectData() method.
	/// <para/>
	/// As an alternative to ToArray(), if you want to apply some post-processing to the serialized bytes, 
	/// such as compression, call UpdateHeader() first to ensure that the string and object token table 
	/// sizes are updated in the header, and then cast BaseStream to MemoryStream. You can then access the
	/// MemoryStream's internal buffer as follows:
	/// <para/>
	/// <example><code>
	/// writer.UpdateHeader();
	/// MemoryStream stream = (MemoryStream) writer.BaseStream;
	///	serializedData = MyCompressor.Compress(stream.GetBuffer(), (int) stream.Length);
	/// </code></example>
	/// </summary>
	public sealed class SerializationWriter: BinaryWriter
	{


        public SerializationWriter(Stream s)
            :base(s)
        {

        }

		/// <summary>
		/// Writes a BitVector32 into the stream.
		/// Stored Size: 4 bytes.
		/// </summary>
		/// <param name="value">The BitVector32 to store.</param>
		public void Write(BitVector32 value)
		{
			Write(value.Data);
		}

		/// <summary>
		/// Writes a DateTime value into the stream.
		/// Stored Size: 8 bytes
		/// </summary>
		/// <param name="value">The DateTime value to store.</param>
		public void Write(DateTime value)
		{
			Write(value.ToBinary());
		}

		/// <summary>
		/// Writes a Guid into the stream.
		/// Stored Size: 16 bytes.
		/// </summary>
		/// <param name="value"></param>
		public void Write(Guid value)
		{
			base.Write(value.ToByteArray());
		}


	}
}
