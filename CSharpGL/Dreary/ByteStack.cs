using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpGL;

namespace CSharpGL.Dreary
{
    public class ByteStack
    {
        private byte[] _stack;
        private uint stackPointer;

        public ByteStack(int stackLen)
        {
            _stack = new byte[stackLen];
            stackPointer = 0;
        }

        public ByteStack(byte[] stack, int stackOffset, int stackLength)
        {
            _stack = new byte[stackLength];
            Buffer.BlockCopy(stack, stackOffset, _stack, 0, stackLength);
            stackPointer = 0;
        }

        #region Byte
        public void Push(byte i)
        {
            _stack[stackPointer++] = i;
        }

        public byte PopByte()
        {
            return _stack[stackPointer--];
        }
        #endregion

        #region Int32
        public void Push(int i)
        {
            byte[] inBytes = BitConverter.GetBytes(i);
            foreach (byte ix in inBytes)
            {
                Push(ix);
            }
        }

        public void PopInt(ref int output)
        {
            byte a = PopByte();
            byte b = PopByte();
            byte c = PopByte();
            byte d = PopByte();
            byte[] ar = new byte[] { a, b, c, d };

            output = BitConverter.ToInt32(ar, 0);
        }
        #endregion

        #region Int16
        public void Push(short i)
        {
            byte[] inBytes = BitConverter.GetBytes(i);
            foreach (byte ix in inBytes)
            {
                Push(ix);
            }
        }

        public void PopShort(ref ushort output)
        {
            byte a = PopByte();
            byte b = PopByte();
            byte[] ar = new byte[] { a, b };

            output = (ushort)BitConverter.ToInt16(ar, 0);
        }
        #endregion

        #region Int64

        public void Push(long i)
        {
            byte[] inBytes = BitConverter.GetBytes(i);
            foreach (byte ix in inBytes)
            {
                Push(ix);
            }
        }

        public void PopLong(ref long output)
        {
            byte a = PopByte();
            byte b = PopByte();
            byte c = PopByte();
            byte d = PopByte();
            byte e = PopByte();
            byte f = PopByte();
            byte g = PopByte();
            byte h = PopByte();
            byte[] ar = new byte[] { a, b, c, d, e, f, g, h };

            output = BitConverter.ToInt64(ar, 0);
        }
        #endregion

        public byte[] DumpStack()
        {
            return _stack;
        }
    }
}
