using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ClassLibrary1
{
    class BlockHeader
    {
        public const int BLOCK_HEADER_SIZE = 4;
        public bool IsFree { get; set; }

        public BlockHeader()
        {
            this.IsFree = true;
        }
        
        public BlockHeader(byte[] byteArray)
        {
            bool _isFree = BitConverter.ToBoolean(byteArray,0);
            this.IsFree = _isFree;
        }


        public byte[] ToByteArray()
        {
            byte[] byteArray = new byte[BLOCK_HEADER_SIZE];

            
            byteArray[0] = Convert.ToByte(IsFree);

            return byteArray;
        }

        
        public override String ToString()
        {
            return string.Format("Free: " +  IsFree); 
        }

       
    }
}
