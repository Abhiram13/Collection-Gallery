-- VERSION 1

SELECT parent.id, parent.name, parent.created_at, parent.updated_at, parent.collection_pic,
    COALESCE(
        (
            SELECT JSON_AGG(JSON_BUILD_OBJECT('id', item.id, 'name', item.name))
            FROM items item
            WHERE item.parent_collection_id = parent.id
        ), '[]'::json
    ) AS collectionItems,
    COALESCE(
        (
            SELECT JSON_AGG(JSON_BUILD_OBJECT('id', platform.id, 'name', platform.name))
            FROM platforms platform
            WHERE platform.id IN (
                SELECT ip.platform_id 
                FROM itemplatforms ip
                JOIN items i ON i.id = ip.item_id
                WHERE i.parent_collection_id = parent.id
            )
        ), '[]'::json
    ) AS collectionPlatforms,
    COALESCE(
        (
            SELECT JSON_AGG(JSON_BUILD_OBJECT('id', child.id, 'name', child.name, 'collectionPic', child.collection_pic))
            FROM collections child
            WHERE child.parent_collection_id = parent.id
        ), '[]'::json
    ) AS childCollection
FROM collections parent
WHERE parent.id = @ParentId
GROUP BY parent.id;

-- VERSION 2

SELECT parent.id, parent.name, parent.created_at, parent.updated_at, parent.collection_pic,    
    COALESCE(
        (
            SELECT JSON_AGG(JSON_BUILD_OBJECT('id', child.id, 'name', child.name, 'collectionPic', child.collection_pic))
            FROM collections child
            WHERE child.parent_collection_id = parent.id
        ), '[]'::json
    ) AS childCollection
FROM collections parent
WHERE parent.id = @ParentId
GROUP BY parent.id;