using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    class PageHeader
    {
        public const int PAGE_HEADER_SIZE = 12;
        public const int PAGE_SIZE = 240;
        public const int PAGE_TOTAL_SIZE = PAGE_HEADER_SIZE + PAGE_SIZE;

        public PageType pageType { get; set; }
        private int blockNumberOfPages;
        private int blockPageIndex;

        public PageHeader()
        {
            pageType = PageType.EMPTY;
        }

        public PageHeader(byte[] byteArray)
        {
            byte[] blockTypeByteArray = new byte[4];
            Array.Copy(byteArray, 0, blockTypeByteArray, 0, 4);
            byte[] blockNumberOfPagesByteArray = new byte[4];
            Array.Copy(byteArray, 4, blockNumberOfPagesByteArray, 0, 4);
            byte[] blockPageIndexByteArray = new byte[4];
            Array.Copy(byteArray, 8, blockPageIndexByteArray, 0, 4);

            this.pageType = PageType.ValueOf(BitConverter.ToInt32(blockTypeByteArray));
            this.blockNumberOfPages = BitConverter.ToInt32(blockNumberOfPagesByteArray);
            this.blockPageIndex = BitConverter.ToInt32(blockPageIndexByteArray);
        }

        public byte[] ToByteArray()
        {
            byte[] byteArray = new byte[PAGE_HEADER_SIZE];

            byte[] blockTypeArray = BitConverter.GetBytes(pageType.Size);
            byte[] blockNumberOfPagesArray = BitConverter.GetBytes(blockNumberOfPages);
            byte[] blockPageIndexArray = BitConverter.GetBytes(blockPageIndex);

            int index = 0;
            Array.Copy(blockTypeArray, 0, byteArray, index, blockTypeArray.Length);
            index = blockTypeArray.Length;
            Array.Copy(blockNumberOfPagesArray, 0, byteArray, index, blockNumberOfPagesArray.Length);
            index = blockTypeArray.Length + blockNumberOfPagesArray.Length;
            Array.Copy(blockPageIndexArray, 0, byteArray, index, blockPageIndexArray.Length);
            

            return byteArray;
        }

       
        public override String ToString()
        {
            return String.Format(
                "Page type:  " + pageType.Type + " Block number of pages:  " + blockNumberOfPages + "Block page index:  " + blockPageIndex

            );
        }

        public static PageType GetTypeBySize(int size)
        {
            int Size = size + BlockHeader.BLOCK_HEADER_SIZE;
            if (Size > 32)
            {
                return PageType.BLOCK_PAGE;
            }
            else if (Size > 16)
            {
                return PageType.BLOCK_32;
            }
            else if (Size > 4)
            {
                return PageType.BLOCK_16;
            }
            else
            {
                return PageType.BLOCK_4;
            }
        }

      
    }
}
