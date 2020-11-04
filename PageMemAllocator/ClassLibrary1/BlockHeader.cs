using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ClassLibrary1
{
    class BlockHeader
    {
        public const int BLOCK_HEADER_SIZE = 4;
        private bool _isFree;

        public BlockHeader()
        {
            this._isFree = true;
        }

        public BlockHeader(byte[] byteArray)
        {
            bool _isFree = BitConverter.ToBoolean(byteArray,0);
            this._isFree = _isFree;
        }

        public bool isFree()
        {
            return _isFree;
        }

        public void setFree(bool _isFree)
        {
            this._isFree = _isFree;
        }

        public byte[] toByteArray()
        {
            byte[] byteArray = new byte[BLOCK_HEADER_SIZE];

            int index = 0;
            byteArray[index] = Convert.ToByte( _isFree);

            return byteArray;
        }

        
        public override String ToString()
        {
            return string.Format($"Free: {0}", _isFree); //////////////////
        }

       
    }
}
