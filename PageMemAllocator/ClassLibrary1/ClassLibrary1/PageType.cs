using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public enum Type
    {
        EMPTY ,
        BLOCK_4 ,
        BLOCK_16 ,
        BLOCK_32,
        BLOCK_PAGE
    }

    public class PageType
    {
        

        public static readonly PageType EMPTY = new PageType(0,  Type.EMPTY);
        public static readonly PageType BLOCK_4 = new PageType(4, Type.BLOCK_4);
        public static readonly PageType BLOCK_16 = new PageType(16,  Type.BLOCK_16);
        public static readonly PageType BLOCK_32 = new PageType(32,  Type.BLOCK_32);
        public static readonly PageType BLOCK_PAGE = new PageType(PageHeader.PAGE_SIZE - BlockHeader.BLOCK_HEADER_SIZE,  Type.BLOCK_PAGE);



        public int Size { get;private set; }
        public Type Type { get; private set; }

       public PageType(int size,  Type type)
        {
            Size = size;
            Type = type;
        }

       public static IEnumerable<PageType> Values
        {
            get
            {
                yield return EMPTY;
                yield return BLOCK_4;
                yield return BLOCK_16;
                yield return BLOCK_32;
                yield return BLOCK_PAGE;
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
