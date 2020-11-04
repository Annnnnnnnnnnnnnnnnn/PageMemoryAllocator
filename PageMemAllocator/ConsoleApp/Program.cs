using System;
using ClassLibrary1;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            PageMemoryAllocator allocator = new PageMemoryAllocator(1024);
            int ind1 = allocator.MemAllocate(24);
            allocator.write(ind1, new byte[] { 1, 4, 8, 5 });
            allocator.realloc(ind1, 48);
            Console.WriteLine(allocator.dump());
        }
    }
}
