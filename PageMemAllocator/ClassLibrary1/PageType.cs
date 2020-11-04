using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public enum Type
    {
        EMPTY ,
        BLOCK_SIZE_4 ,
        BLOCK_SIZE_16 ,
        BLOCK_SIZE_32,
        BLOCK_PAGE_SIZE
    }

    public class PageType
    {
        

        public static readonly PageType EMPTY = new PageType(0, "EMPTY", Type.EMPTY);
        public static readonly PageType BLOCK_SIZE_4 = new PageType(4, "4B blocks",Type.BLOCK_SIZE_4);
        public static readonly PageType BLOCK_SIZE_16 = new PageType(16, "16B blocks", Type.BLOCK_SIZE_16);
        public static readonly PageType BLOCK_SIZE_32 = new PageType(32, "32B blocks", Type.BLOCK_SIZE_32);
        public static readonly PageType BLOCK_PAGE_SIZE = new PageType(PageHeader.PAGE_SIZE - BlockHeader.BLOCK_HEADER_SIZE, "Page size block", Type.BLOCK_PAGE_SIZE);



        public int Size { get;private set; }
        public String Alias { get;private set; }
        public Type Type { get; private set; }

       public PageType(int size, String alias, Type type)
        {
            this.Size = size;
            this.Alias = alias;
            this.Type = type;
        }

       public static IEnumerable<PageType> Values
        {
            get
            {
                yield return EMPTY;
                yield return BLOCK_SIZE_4;
                yield return BLOCK_SIZE_16;
                yield return BLOCK_SIZE_32;
                yield return BLOCK_PAGE_SIZE;
            }
        }

        public static PageType ValueOf(int size)
        {
            PageType pageType = null;
            foreach(var el in PageType.Values)
            {
                if(el.Size == size)
                {
                    pageType = el;
                    
                }
            }
            return pageType;
        }








    }

    
}
