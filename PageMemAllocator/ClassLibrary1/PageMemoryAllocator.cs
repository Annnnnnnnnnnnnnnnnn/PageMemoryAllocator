using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ClassLibrary1
{
    public class PageMemoryAllocator
    {

        private const int DEFAULT_SIZE = PageHeader.PAGE_TOTAL_SIZE * 10;

        private int Size;
        private byte[] buffer;


        public PageMemoryAllocator()
        {
            Size = DEFAULT_SIZE;
            buffer = new byte[Size];
            initPages();
        }

        public PageMemoryAllocator(int size)
        {
            this.Size = getValidSize(size);
            buffer = new byte[this.Size];
            initPages();
        }

        private bool CheckIndex(int index)
        {
            return index >= 0 && index % 4 == 0 && index < Size - PageHeader.PAGE_HEADER_SIZE - BlockHeader.BLOCK_HEADER_SIZE;
        }

        private int getPageHeaderIndex(int index)
        {
            int pagesNumber = index / PageHeader.PAGE_TOTAL_SIZE;
            return pagesNumber * PageHeader.PAGE_TOTAL_SIZE;
        }

        private PageHeader getPageHeader(int index)
        {
            int pageHeaderIndex = getPageHeaderIndex(index);
            int pageHeaderByteArrayLength = PageHeader.PAGE_HEADER_SIZE;
            byte[] pageHeaderByteArray = new byte[pageHeaderByteArrayLength];
                Array.Copy(buffer, pageHeaderIndex, pageHeaderByteArray, 0, pageHeaderByteArrayLength);
              
            return new PageHeader(pageHeaderByteArray);
        }

        private int getPageFreeBlockIndex(int index)
        {
            PageHeader pageHeader = getPageHeader(index);
            PageType pageType = pageHeader.pageType;

            int blockHeaderIndex = index + PageHeader.PAGE_HEADER_SIZE;
            while (blockHeaderIndex < index + PageHeader.PAGE_TOTAL_SIZE)
            {
                BlockHeader blockHeader = getBlockHeader(blockHeaderIndex);
                if (blockHeader.isFree())
                {
                    return blockHeaderIndex;
                }
                blockHeaderIndex += BlockHeader.BLOCK_HEADER_SIZE + pageType.Size;
            }
            return -1;
        }
        private BlockHeader getBlockHeader(int index)
        {
            int blockHeaderByteArrayLength = BlockHeader.BLOCK_HEADER_SIZE ;
            byte[] blockHeaderByteArray = new byte[blockHeaderByteArrayLength];
            Array.Copy(buffer, index, blockHeaderByteArray, 0, blockHeaderByteArrayLength);
            return new BlockHeader(blockHeaderByteArray);
        }

        private int getValidSize(int size)
        {
            return size % PageHeader.PAGE_TOTAL_SIZE == 0
                ? size
                : PageHeader.PAGE_TOTAL_SIZE * (size / PageHeader.PAGE_TOTAL_SIZE + 1);
        }

        private void writePageHeader(int index, PageHeader pageHeader)
        {
            byte[] pageHeaderByteArray = pageHeader.toByteArray();
            Array.Copy(pageHeaderByteArray, 0, buffer, index, pageHeaderByteArray.Length);
        }

        private void writeBlockHeader(int index, BlockHeader blockHeader)
        {
            byte[] blockHeaderByteArray = blockHeader.toByteArray();
            Array.Copy(blockHeaderByteArray, 0, buffer, index, blockHeaderByteArray.Length);
        }

        private void initPages()
        {
            PageHeader pageHeader = new PageHeader();
            byte[] pageHeaderByteArray = pageHeader.toByteArray();
            int index = 0;
            while (index < buffer.Length)
            {
                Array.Copy(pageHeaderByteArray, 0, buffer, index, pageHeaderByteArray.Length);
                index += PageHeader.PAGE_TOTAL_SIZE;
            }
        }

        private void initPage(int index, PageHeader pageHeader)
        {
            PageType pageType = pageHeader.pageType;
            writePageHeader(index, pageHeader);
            BlockHeader blockHeader = new BlockHeader();
            int blockHeaderIndex = index + PageHeader.PAGE_HEADER_SIZE;
            while (blockHeaderIndex < index + PageHeader.PAGE_TOTAL_SIZE)
            {
                writeBlockHeader(blockHeaderIndex, blockHeader);
                blockHeaderIndex += BlockHeader.BLOCK_HEADER_SIZE + pageType.Size;
            }
        }

        

        public int MemAllocate(int size)
        {
            PageType pageType = PageHeader.getPageTypeBySize(size);

            // Get first page header
            int pageIndex = 0;

            // Iterate through pages to find
            // free with sufficient blocks size type
            int index = -1;
            while (index == -1 && pageIndex < this.Size - PageHeader.PAGE_HEADER_SIZE)
            {
                PageHeader pageHeader = getPageHeader(pageIndex);
                PageType currentPageType = pageHeader.pageType;

                // Current page type matches needed type
                if (currentPageType == pageType)
                {
                    int freeBlockIndex = getPageFreeBlockIndex(pageIndex);
                    index = freeBlockIndex >= 0
                        ? freeBlockIndex
                        : index;
                }
                // Current page type is empty
                else if (currentPageType == PageType.EMPTY)
                {
                    pageHeader.pageType = pageType;
                    initPage(pageIndex, pageHeader);
                    index = pageIndex + PageHeader.PAGE_HEADER_SIZE;
                }

                pageIndex += PageHeader.PAGE_TOTAL_SIZE;
            }

            if (index >= 0)
            {
                BlockHeader blockHeader = getBlockHeader(index);
                blockHeader.setFree(false);
                writeBlockHeader(index, blockHeader);
            }

            return index;
        }


        public int MemRealloc(int index, int size) 
        {
          if (!CheckIndex(index)) throw new IndexOutOfRangeException();
          
          BlockHeader blockHeader = getBlockHeader(index);
          blockHeader.setFree(true);
          writeBlockHeader(index, blockHeader);

          byte[] data = read(index);
          int reallocIndex = MemAllocate(size);
          write(reallocIndex, data);

              return reallocIndex;
        }


        public void MemFree(int index)
        {
            if (!CheckIndex(index)) throw new IndexOutOfRangeException();
    
            BlockHeader blockHeader = getBlockHeader(index);
            blockHeader.setFree(true);
            writeBlockHeader(index, blockHeader);
        }

        public String MemDump()
        {
            String dump = "";
            int index = 0;

            // Iterate through blocks
            while (index < Size)
            {
                PageHeader pageHeader = getPageHeader(index);
                dump += pageHeader.ToString() + '\n';

                int pageDataLength = PageHeader.PAGE_TOTAL_SIZE - PageHeader.PAGE_HEADER_SIZE;
                byte[] pageData = new byte[pageDataLength]  ;
             Array.Copy(buffer, index + PageHeader.PAGE_HEADER_SIZE, pageData, 0, pageDataLength);
            dump += BitConverter.ToString(pageData);
                dump += '\n';
            
                index += PageHeader.PAGE_TOTAL_SIZE;
            }

            return dump;
        }

        public void write(int index, byte[] byteArray) 
        {
        if (!CheckIndex(index)) throw new IndexOutOfRangeException();
    
            Array.Copy(byteArray, 0, buffer, index + BlockHeader.BLOCK_HEADER_SIZE, byteArray.Length);
        }

        public byte[] read(int index)
        {
        if (!CheckIndex(index)) throw new IndexOutOfRangeException();

        int pageHeaderIndex = getPageHeaderIndex(index);
        PageHeader pageHeader = getPageHeader(pageHeaderIndex);
        PageType pageType = pageHeader.pageType;
            int readArrayLength = pageType.Size; 
            byte[] readArray = new byte[readArrayLength];
            Array.Copy(buffer, index + BlockHeader.BLOCK_HEADER_SIZE, readArray, 0, readArrayLength);
            return readArray;
        }
    }
}
