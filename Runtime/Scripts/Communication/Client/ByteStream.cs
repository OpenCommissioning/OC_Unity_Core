namespace OC.Communication
{
    public class ByteStream
    {
        /// <summary>
        /// Gets the underlying byte buffer.
        /// </summary>
        internal byte[] Buffer => _buffer;
        
        /// <summary>
        /// Length of the underlying byte buffer.
        /// </summary>
        internal int Length => _buffer.Length;

        /// <summary>
        /// Position reached end of Detector.
        /// </summary>
        internal bool ReachedEnd => _position == _buffer.Length;
        
        private readonly byte[] _buffer;
        private int _position;

        /// <summary>
        /// Initializes a new ByteStream with the given size.
        /// </summary>
        internal ByteStream(int byteSize)
        {
            _buffer = new byte[byteSize];
        }

        /// <summary>
        /// Sets the position within the Detector.
        /// </summary>
        internal void Seek(int position)
        {
            if (_buffer.Length < _position) return;
            _position = position;
        }

        /// <summary>
        /// Reads bytes from the Detector and sets the position.
        /// </summary>
        internal byte[] Read(int count)
        {
            var numArray = new byte[count];
            if (_buffer.Length < _position + count) return numArray;

            for (var i = 0; i < count; i++)
            {
                numArray[i] = _buffer[_position + i];
            }

            _position += count;
            return numArray;
        }
        
        /// <summary>
        /// Writes bytes to the Detector and sets the position.
        /// </summary>
        internal void Write(byte[] values)
        {
            if (_buffer.Length < _position + values.Length) return;
            
            for (var i = 0; i < values.Length; i++)
            {
                _buffer[_position + i] = values[i];
            }

            _position += values.Length;
        }
        
        /// <summary>
        /// Writes the value to the Detector and sets the position.
        /// </summary>
        internal void Write(uint value)
        {
            if (_buffer.Length < _position + 4) return;
            
            _buffer[_position] = (byte) value;
            _buffer[_position + 1] = (byte) (value >> 8);
            _buffer[_position + 2] = (byte) (value >> 16);
            _buffer[_position + 3] = (byte) (value >> 24);
            
            _position += 4;
        }
        
        /// <summary>
        /// Writes the value to the Detector and sets the position.
        /// </summary>
        internal void Write(int value)
        {
            if (_buffer.Length < _position + 4) return;
            
            _buffer[_position] = (byte) value;
            _buffer[_position + 1] = (byte) (value >> 8);
            _buffer[_position + 2] = (byte) (value >> 16);
            _buffer[_position + 3] = (byte) (value >> 24);
            
            _position += 4;
        }
    }
}