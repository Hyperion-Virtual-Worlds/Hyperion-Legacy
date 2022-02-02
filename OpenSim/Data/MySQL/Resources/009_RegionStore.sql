BEGIN;
 
ALTER TABLE primitems change itemID itemIDold varchar(255);
ALTER TABLE primitems change primID primIDold varchar(255);
ALTER TABLE primitems change assetID assetIDold varchar(255);
ALTER TABLE primitems change parentFolderID parentFolderIDold varchar(255);
ALTER TABLE primitems change creatorID creatorIDold varchar(255);
ALTER TABLE primitems change ownerID ownerIDold varchar(255);
ALTER TABLE primitems change groupID groupIDold varchar(255);
ALTER TABLE primitems change lastOwnerID lastOwnerIDold varchar(255);
ALTER TABLE primitems add itemID char(36);
ALTER TABLE primitems add primID char(36);
ALTER TABLE primitems add assetID char(36);
ALTER TABLE primitems add parentFolderID char(36);
ALTER TABLE primitems add creatorID char(36);
ALTER TABLE primitems add ownerID char(36);
ALTER TABLE primitems add groupID char(36);
ALTER TABLE primitems add lastOwnerID char(36);
UPDATE primitems set itemID = itemIDold, primID = primIDold, assetID = assetIDold, parentFolderID = parentFolderIDold, creatorID = creatorIDold, ownerID = ownerIDold, groupID = groupIDold, lastOwnerID = lastOwnerIDold;
ALTER TABLE primitems drop itemIDold;
ALTER TABLE primitems drop primIDold;
ALTER TABLE primitems drop assetIDold;
ALTER TABLE primitems drop parentFolderIDold;
ALTER TABLE primitems drop creatorIDold;
ALTER TABLE primitems drop ownerIDold;
ALTER TABLE primitems drop groupIDold;
ALTER TABLE primitems drop lastOwnerIDold;
ALTER TABLE primitems add constraint primary key(itemID);
ALTER TABLE primitems add index primitems_primid(primID);

COMMIT;