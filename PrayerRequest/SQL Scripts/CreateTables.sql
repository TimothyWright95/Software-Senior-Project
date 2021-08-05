Create TABLE Prayer
(
	PrayerID			int							constraint PKPrayer Primary Key Identity,
	Catagorie			int,
	Title				nvarchar(30),
	ShortDescription	nvarchar(150),
	Expiration			DateTime					DEFAULT (dateadd(day,7,getdate())),
	PostAnonymous		bit							Default 0, 
	UserID				nvarchar(450)				constraint Prayer_UserID References AspNetUsers(Id),
	LongDescription		text,
	USState				int

);
Create TABLE UserGroup
(
	GroupID				int							constraint PKUserGroup Primary Key Identity,
	GroupOpen			bit							Default 0, 
	GroupPassword		nvarchar(50),
	GroupName			nvarchar(50),
	GroupDescription	text,
	GroupLocation		text,
	GroupContact		text,
	OwnerID				nvarchar(450)				constraint UserGroup_UserID References AspNetUsers(Id),
	USState				int
);

Create TABLE Picture
(
	PictureID			int							constraint PKPicture Primary key Identity,
	PicturePath			nvarchar(100),
	GroupID				int							constraint Picture_GroupID References UserGroup(GroupID)
);

Create TABLE Post
(
	PostID				int							constraint PKPost Primary key Identity,
	OwnerID				nvarchar(450)				constraint Post_UserID References AspNetUsers(Id),
	Postmessage			text,
	GroupID				int							constraint Post_GroupID References UserGroup(GroupID),
	DatePosted			DATETIME					DEFAULT GETDATE()
);

Create TABLE Bulitin
(
	BulitinID			int							constraint PKBulitin Primary key Identity,
	GroupID				int							constraint Bulitin_GroupID References UserGroup(GroupID),
	DatePosted			datetime					DEFAULT GETDATE(),
	Bulitinmessage		text,
	EventTitle			nvarchar(100)
	
);


Create TABLE PrayerSubscribed
(
	PrayerID			int							constraint PrayerSubscribed_PrayerID References Prayer(PrayerID),
	UserID				nvarchar(450)				constraint PrayerSubscribed_UserID References AspNetUsers(Id),
	constraint	PKPrayerSubscribed primary Key(PrayerID,UserID)
);
Create TABLE GroupSubscribed
(
	GroupID			int							constraint GroupSubscribed_GroupID References UserGroup(GroupID),
	UserID			nvarchar(450)				constraint GroupSubscribed_UserID References AspNetUsers(Id),
	constraint	PKGroupSubscribed primary Key(GroupID,UserID)
);

Create TABLE GroupMod
(
	GroupID			int							constraint GroupMod_GroupID References UserGroup(GroupID),
	UserID			nvarchar(450)				constraint GroupMod_UserID References AspNetUsers(Id),
	constraint	PKGroupMod primary Key(GroupID,UserID)
);