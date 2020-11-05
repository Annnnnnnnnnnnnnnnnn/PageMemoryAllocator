using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ClassLibrary1
{
    public class PageMemoryAllocator
    {

        private const int DEFAULT_SIZE = PageHeader.PAGE_TOTAL_SIZE * 5;

        private int _size;
        private byte[] _buffer;


        public PageMemoryAllocator()
        {
            _size = DEFAULT_SIZE;
            _buffer = new byte[_size];
            FillPages();
        }

        public PageMemoryAllocator(int size)
        {
            this._size = AlignSize(size);
            _buffer = new byte[this._size];
            FillPages();
        }

        private bool CheckIndex(int index)
        {
            return index >= 0 && index < _size - PageHeader.PAGE_HEADER_SIZE - BlockHeader.BLOCK_HEADER_SIZE;
        }

        private int GetPageHeaderIndex(int index)
        {
            int numberOfPage = index / PageHeader.PAGE_TOTAL_SIZE;
            return numberOfPage * PageHeader.PAGE_TOTAL_SIZE;
        }

        private PageHeader GetPageHeader(int index)
        {
            int pageHeaderIndex = GetPageHeaderIndex(index);
            int pageHeaderByteArrayLength = PageHeader.PAGE_HEADER_SIZE;
            byte[] pageHeaderByteArray = new byte[pageHeaderByteArrayLength];
                Array.Copy(_buffer, pageHeaderIndex, pageHeaderByteArray, 0, pageHeaderByteArrayLength);
              
            return new PageHeader(pageHeaderByteArray);
        }

        private int GetPageFreeBlockIndex(int index)
        {
            PageHeader pageHeader = GetPageHeader(index);
            PageType pageType = pageHeader.pageType;

            int blockHeaderIndex = index + PageHeader.PAGE_HEADER_SIZE;
            while (blockHeaderIndex < index + PageHeader.PAGE_TOTAL_SIZE)
            {
                BlockHeader blockHeader = GetBlockHeader(blockHeaderIndex);
                if (blockHeader.IsFree)
                {
                    return blockHeaderIndex;
                }
                blockHeaderIndex += BlockHeader.BLOCK_HEADER_SIZE + pageType.Size;
            }
            return -1;
        }
        private BlockHeader GetBlockHeader(int index)
        {
            int blockHeaderByteArrayLength = BlockHeader.BLOCK_HEADER_SIZE ;
            byte[] blockHeaderByteArray = new byte[blockHeaderByteArrayLength];
            Array.Copy(_buffer, index, blockHeaderByteArray, 0, blockHeaderByteArrayLength);
            return new BlockHeader(blockHeaderByteArray);
        }

        private int AlignSize(int size)
        {
            return size % PageHeader.PAGE_TOTAL_SIZE == 0
                ? size
                : PageHeader.PAGE_TOTAL_SIZE * (size / PageHeader.PAGE_TOTAL_SIZE + 1);
        }

        private void CreatePageHeader(int index, PageHeader pageHeader)
        {
            byte[] pageHeaderByteArray = pageHeader.ToByteArray();
            Array.Copy(pageHeaderByteArray, 0, _buffer, index, pageHeaderByteArray.Length);
        }

        private void CreateBlockHeader(int index, BlockHeader blockHeader)
        {
            byte[] blockHeaderByteArray = blockHeader.ToByteArray();
            Array.Copy(blockHeaderByteArray, 0, _buffer, index, blockHeaderByteArray.Length);
        }

        private void FillPages()
        {
            PageHeader pageHeader = new PageHeader();
            byte[] pageHeaderByteArray = pageHeader.ToByteArray();
            int index = 0;
            while (index < _buffer.Length)
            {
                Array.Copy(pageHeaderByteArray, 0, _buffer, index, pageHeaderByteArray.Length);
                index += PageHeader.PAGE_TOTAL_SIZE;
            }
        }

        private void FillPage(int index, PageHeader pageHeader)
        {
            PageType pageType = pageHeader.pageType;
            CreatePageHeader(index, pageHeader);
            BlockHeader blockHeader = new BlockHeader();
            int blockHeaderIndex = index + PageHeader.PAGE_HEADER_SIZE;
            while (blockHeaderIndex < index + PageHeader.PAGE_TOTAL_SIZE)
            {
                CreateBlockHeader(blockHeaderIndex, blockHeader);
                blockHeaderIndex += BlockHeader.BLOCK_HEADER_SIZE + pageType.Size;
            }
        }

        

        public int MemAllocate(int size)
        {
            PageType pageType = PageHeader.GetTypeBySize(size);

            
            int pageIndex = 0;

           
            int index = -1;
            while (index == -1 && pageIndex < _size - PageHeader.PAGE_HEADER_SIZE)
            {
                PageHeader pageHeader = GetPageHeader(pageIndex);
                PageType currentPageType = pageHeader.pageType;

                
                if (currentPageType == pageType)
                {
                    int freeBlockPosition = GetPageFreeBlockIndex(pageIndex);
                    index = freeBlockPosition >= 0
                        ? freeBlockPosition
                        : index;
                }
                
                else if (currentPageType == PageType.EMPTY)
                {
                    pageHeader.pageType = pageType;
                    FillPage(pageIndex, pageHeader);
                    index = pageIndex + PageHeader.PAGE_HEADER_SIZE;
                }

                pageIndex += PageHeader.PAGE_TOTAL_SIZE;
            }

            if (index >= 0)
            {
                BlockHeader blockHeader = GetBlockHeader(index);
                blockHeader.IsFree = false;
                CreateBlockHeader(index, blockHeader);
            }

            return index;
        }


        public int MemRealloc(int index, int size) 
        {
          if (!CheckIndex(index)) throw new IndexOutOfRangeException();
          
          BlockHeader blockHeader = GetBlockHeader(index);
          blockHeader.IsFree = true;
            CreateBlockHeader(index, blockHeader);

          byte[] data = ReadArray(index);
          int reallocIndex = MemAllocate(size);
            WriteArray(reallocIndex, data);

              return reallocIndex;
        }


        public void MemFree(int index)
        {
            if (!CheckIndex(index)) throw new IndexOutOfRangeException();
    
            BlockHeader blockHeader = GetBlockHeader(index);
            blockHeader.IsFree = true;
            CreateBlockHeader(index, blockHeader);
        }

        public String MemDump()
        {
            String dump = "";
            int index = 0;

            
            while (index < _size)
            {
                PageHeader pageHeader = GetPageHeader(index);
                dump += pageHeader.ToString() + '\n';

                int pageDataLength = PageHeader.PAGE_TOTAL_SIZE - PageHeader.PAGE_HEADER_SIZE;
                byte[] pageData = new byte[pageDataLength]  ;
             Array.Copy(_buffer, index + PageHeader.PAGE_HEADER_SIZE, pageData, 0, pageDataLength);
            dump += BitConverter.ToString(pageData);
                dump += '\n';
            
                index += PageHeader.PAGE_TOTAL_SIZE;
            }

            return dump;
        }

        public void WriteArray(int index, byte[] byteArray) 
        {
        if (!CheckIndex(index)) throw new IndexOutOfRangeException();
    
            Array.Copy(byteArray, 0, _buffer, index + BlockHeader.BLOCK_HEADER_SIZE, byteArray.Length);
        }

        public byte[] ReadArray(int index)
        {
        if (!CheckIndex(index)) throw new IndexOutOfRangeException();

        int pageHeaderIndex = GetPageHeaderIndex(index);
        PageHeader pageHeader = GetPageHeader(pageHeaderIndex);
        PageType pageType = pageHeader.pageType;
            int readArrayLength = pageType.Size; 
            byte[] readArray = new byte[readArrayLength];
            Array.Copy(_buffer, index + BlockHeader.BLOCK_HEADER_SIZE, readArray, 0, readArrayLength);
            return readArray;
        }
    }
}
