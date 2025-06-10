using Unity.Collections.LowLevel.Unsafe;

namespace OC.Communication
{
    public class ClientVariable
    {
        public string Name { get; }
        public bool Reserved { get; set; }

        private readonly byte[] _buffer;
        private readonly int _length;
        private readonly int _offset;

        public ClientVariable(string name, byte[] buffer, int length, int offset)
        {
            Name = name;
            _buffer = buffer;
            _length = length;
            _offset = offset;
        }

        public bool Write(byte data)
        {
            if (_length != 1) return false;
            _buffer[_offset] = data;
            return true;
        }

        public bool Write(sbyte data)
        {
            if (_length != 1) return false;
            _buffer[_offset] = (byte)data;
            return true;
        }

        public bool Write(ushort data)
        {
            if (_length != 2) return false;
            for (var i = 0; i < _length; i++)
            {
                _buffer[_offset + i] = (byte)(data >> 8*i & 0xFF);
            }
            return true;
        }

        public bool Write(short data)
        {
            if (_length != 2) return false; 
            for (var i = 0; i < _length; i++)
            {
                _buffer[_offset + i] = (byte)(data >> 8*i & 0xFF);
            }
            return true;
        }

        public bool Write(uint data)
        {
            if (_length != 4) return false; 
            for (var i = 0; i < _length; i++)
            {
                _buffer[_offset + i] = (byte)(data >> 8*i & 0xFF);
            }
            return true;
        }

        public bool Write(int data)
        {
            if (_length != 4) return false;
            for (var i = 0; i < _length; i++)
            {
                _buffer[_offset + i] = (byte)(data >> 8*i & 0xFF);
            }
            return true;
        }

        public bool Write(ulong data)
        {
            if (_length != 8) return false;
            for (var i = 0; i < _length; i++)
            {
                _buffer[_offset + i] = (byte)(data >> 8*i & 0xFF);
            }
            return true;
        }

        public bool Write(long data)
        {
            if (_length != 8) return false;
            for (var i = 0; i < _length; i++)
            {
                _buffer[_offset + i] = (byte)(data >> 8*i & 0xFF);
            }
            return true;
        }

        public unsafe bool Write(ref float data)
        {
            if (_length != 4) return false;
            fixed (byte* b = &_buffer[_offset])
            fixed (float* value = &data)
            {
                *((int*)b) = *(int*)value;
            }
            return true;
        }
        
        public unsafe bool Write(ref float[] data)
        {
            if (data == null) return false;
            if (data.Length * 4 != _length) return false;
            fixed (byte* bytePointer = &_buffer[_offset])
            {
                fixed (float* dataPointer = &data[0])
                {
                    UnsafeUtility.MemCpy(bytePointer, dataPointer, data.Length * 4);
                }
            }
            return true;
        }

        public unsafe bool Write(ref double data)
        {
            if (_length != 8) return false;
            fixed (byte* b = &_buffer[_offset])
            fixed (double* v = &data)
            {
                *((long*)b) = *(long*)v;
            }
            return true;
        }

        public bool Read(ref byte data)
        {
            if (_length != 1) return false;
            data = _buffer[_offset];
            return true;
        }

        public bool Read(ref sbyte data)
        {
            if (_length != 1) return false;
            data = (sbyte)_buffer[_offset];
            return true;
        }

        public unsafe bool Read(ref ushort data)
        {
            if (_length != 2) return false;
            fixed(byte* bytePointer = &_buffer[_offset])
            {
                data = UnsafeUtility.As<byte, ushort>(ref *bytePointer);
            }
            return true;
        }

        public unsafe bool Read(ref short data)
        {
            if (_length != 2) return false;
            fixed (byte* bytePointer = &_buffer[_offset])
            {
                data = UnsafeUtility.As<byte, short>(ref *bytePointer);
            }
            return true;
        }

        public unsafe bool Read(ref uint data)
        {
            if (_length != 4) return false;
            fixed (byte* bytePointer = &_buffer[_offset])
            {
                data = UnsafeUtility.As<byte, uint>(ref *bytePointer);
            }
            return true;
        }

        public unsafe bool Read(ref int data)
        {
            if (_length != 4) return false;
            fixed (byte* bytePointer = &_buffer[_offset])
            {
                data = UnsafeUtility.As<byte, int>(ref *bytePointer);
            }
            return true;
        }

        public unsafe bool Read(ref ulong data)
        {
            if (_length != 8) return false;
            fixed (byte* bytePointer = &_buffer[_offset])
            {
                data = UnsafeUtility.As<byte, ulong>(ref *bytePointer);
            }
            return true;
        }

        public unsafe bool Read(ref long data)
        {
            if (_length != 8) return false;
            fixed (byte* bytePointer = &_buffer[_offset])
            {
                data = UnsafeUtility.As<byte, long>(ref *bytePointer);
            }
            return true;
        }

        public unsafe bool Read(ref float data)
        {
            if (_length != 4) return false;
            fixed (byte* bytePointer = &_buffer[_offset])
            {
                data = UnsafeUtility.As<byte, float>(ref *bytePointer);
            }
            return true;
        }
        
        public unsafe bool Read(ref float[] data)
        {
            if (data == null) return false;
            if (data.Length * 4 != _length) return false;

            fixed (byte* bytePointer = &_buffer[_offset])
            {
                fixed (float* dataPointer = &data[0])
                {
                    UnsafeUtility.MemCpy(dataPointer, bytePointer, data.Length * 4);
                }
            }
            return true;
        }

        public unsafe bool Read(ref double data)
        {
            if (_length != 8) return false;
            fixed (byte* bytePointer = &_buffer[_offset])
            {
                data = UnsafeUtility.As<byte, double>(ref *bytePointer);
            }
            return true;
        }
    }
}