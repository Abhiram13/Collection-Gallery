namespace CollectionGallery.InfraStructure.Data.Constants;

public static class DbTableNames
{
    public static class Base
    {
        public const string CREATED_AT = "created_at";
        public const string UPDATED_AT = "updated_at";
        public const string DELETED_AT = "deleted_at";
        public const string ID = "id";
    }
    
    public static class File
    {
        public const string TABLE_NAME = "files";
        public const string NAME = "name";
        public const string EXTENSION = "extension";
        public const string SIZE = "size";
        public const string MIME = "mime_type";
        public const string BUCKET = "bucket";
        public const string STORAGE_KEY = "storage_key";
        public const string ITEM_ID = "item_id";
    }

    public static class Item
    {
        public const string TABLE_NAME = "items";
        public const string NAME = "name";
        public const string COLLECTION_ID = "collection_id";
    }

    public static class Tag
    {
        public const string TABLE_NAME = "tags";
        public const string NAME = "name";
    }

    public static class ItemTag
    {
        public const string TABLE_NAME = "item_tags";
        public const string ITEM_ID = "item_id";
        public const string TAG_ID = "tag_id";
    }
    
    public static class Collection
    {
        public const string TABLE_NAME = "collections";
        public const string NAME = "name";
        public const string PARENT_COLLECTION_ID = "parent_collection_id";
        public const string COVER_ITEM_ID = "cover_item_id";
    }
}