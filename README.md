# PageMemoryAllocator
Custom page memory allocator written on C#
## DESCRIPTION
## About allocator

A general purpose memory allocator must do at least **three tasks**: 
***allocating** a block of memory* of a given size, ***freeing** the allocated memory block* and 
***resizing** the allocated memory block*. These functions constitute a general purpose memory allocator interface.

Each block has a **header** and the **data**. Header always has a fixed size of **12 bytes**.
All blocks are **4-byte** aligned.
That means, to allocate **8 bytes** of memory, we need at least **24 bytes** of free memory. The structure of header:
> [memory size(4 bytes), previous block memory size(4 bytes), boolean is block free(1 byte), placeholder(3 bytes)]

### Algorithm description

#### Allocate function
```
void* mem_alloc(size_t size)
```

When the `mem_alloc` function has called, it searches for the page with a suitable block size type or an empty page.
If it found such page - it searches for the free block and occupies it. If free block wasn't found - page search process
continues. If empty page was found - it's assigned to a suitable block size type.

### Reallocate function
```
void* mem_realloc(void* addr, size_t size)
```

This function calls first of all decides, if it has to make the block smaller or bigger.
Block is marked as *'free'*, and new block is allocated (called alloc(size)).

### Free memory function
```
void mem_free(void* addr)
```

The function flags block as *'free'* block.

## DEMONSTRATION

>All memory state outputs are made with the `mem_dump` function, which, basically, 
>goes through 'our' heap and prints the states of all the existing blocks.

### The allocation of *24* and *32* bytes of memory
#### Memory will be successfully allocated
##### Code
```
Allocator allocator = new Allocator(1024);
int ind1 = allocator.alloc(24);
int ind2 = allocator.alloc(32);
System.out.println(allocator.dump());
```

##### Output
![allocating 24 and 32 bytes](img/1.png)
---

### The allocation of *24* and *32* bytes of memory and writing data
#### Memory will be successfully allocated and data will be written
##### Code
```
try {
    Allocator allocator = new Allocator(1024);
    int ind1 = allocator.alloc(24);
    int ind2 = allocator.alloc(32);
    allocator.write(ind1, new byte[]{1, 4, 8, 5});
    allocator.write(ind2, new byte[]{2, 2, 8, 9, 0, 0, 9});
    System.out.println(allocator.dump());
} catch (InvalidIndexException err) {
    System.err.println(err);
}
```

##### Output
![allocating 24 and 32 bytes and writing](img/2.png)
---

### The freeing of *24* bytes of memory
#### Memory will be successfully freed
##### Code
```
try {
    Allocator allocator = new Allocator(1024);
    int ind1 = allocator.alloc(24);
    allocator.write(ind1, new byte[]{1, 4, 8, 5});
    allocator.free(ind1);
    System.out.println(allocator.dump());
} catch (InvalidIndexException err) {
    System.err.println(err);
}
System.out.println(allocator.dump());
```

##### Output
![freeing 24 bytes block)](img/3.png)
---

### The allocating of *24* bytes of memory and then realloc it to 32 bytes of memory
#### Memory will be successfully reallocated to another page
##### Code
```
try {
    allocator.realloc(ind1, 48);
} catch (InvalidIndexException err) {
    System.err.println(err);
}
System.out.println(allocator.dump());
```

##### Output
![reallocating block](img/4.png)
