using System;

namespace CollectionGallery.Shared.Exceptions;

public class CollectionIdNotFoundException : Exception
{
    public CollectionIdNotFoundException() : base() { }

    public CollectionIdNotFoundException(string message) : base(message) { }
}

public class TagIdNotFoundException : Exception
{
    public TagIdNotFoundException() : base() { }

    public TagIdNotFoundException(string message) : base(message) { }
}