Create FULLTEXT CATALOG PrayerRequestFT AS DEFAULT

/*GroupName, GroupDescription, GroupContact, GroupLocation*/
Create FULLTEXT INDEX ON dbo.UserGroup(GroupName, GroupDescription, GroupContact, GroupLocation)
	KEY INDEX PKUserGroup ON PrayerRequestFT
	WITH CHANGE_TRACKING AUTO
GO
/*Title, ShortDescription, LongDescription*/
Create FULLTEXT INDEX ON dbo.Prayer(Title, ShortDescription, LongDescription)
	KEY INDEX PKPrayer ON PrayerRequestFT
	WITH CHANGE_TRACKING AUTO
GO