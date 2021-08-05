
/*AddBulitin*/
CREATE OR ALTER PROC AddBulitin(
	@Bulitinmessage text,
	@EventTitle nvarchar(100),
	@GroupID int
)
AS
Insert Into Bulitin(Bulitinmessage, GroupID, EventTitle)
Values(@Bulitinmessage,@GroupID,@EventTitle)
GO
/*AddPost*/
CREATE OR ALTER PROC AddPost(
	@Postmessage text,
	@GroupID int,
	@OwnerID nvarchar(450)
)
AS
DECLARE @RETURNVAR int;
Insert Into Post(Postmessage, GroupID, OwnerID)
Values(@Postmessage,@GroupID, @OwnerID)
SELECT @RETURNVAR = SCOPE_IDENTITY();
SELECT @RETURNVAR
GO
/*CreateGroup*/
CREATE OR ALTER PROC CreateGroup(
	@GroupContact		text,
	@GroupDescription	text,
	@GroupLocation		text,
	@GroupName			nvarchar(50),
	@GroupOpen			bit,
	@GroupPassword		nvarchar(50) = NULL,
	@OwnerID			nvarchar(450),
	@State				int
)
AS
Insert Into UserGroup(GroupOpen, GroupPassword, GroupName, GroupDescription, GroupLocation, GroupContact, OwnerID, USState)
Values(@GroupOpen, @GroupPassword, @GroupName, @GroupDescription, @GroupLocation, @GroupContact, @OwnerID, @State)
Insert Into GroupMod
Values(SCOPE_IDENTITY(), @OwnerID)
GO
/*CreatePrayerRequest*/
CREATE OR ALTER PROC CreatePrayerRequest(
	@LongDescription	text,
	@ShortDescription	nvarchar(150),
	@PostAnonymous		bit,
	@Title				nvarchar(30),
	@UserID				nvarchar(450),
	@Expiration			datetime,
	@Catagorie			int,
	@State				int
	
)
AS
Insert Into Prayer(LongDescription, ShortDescription, PostAnonymous, Title, UserID, Expiration, Catagorie, USState)
Values(@LongDescription, @ShortDescription, @PostAnonymous, @Title, @UserID, @Expiration, @Catagorie, @State)				
GO
/*DeleteBulitin*/
CREATE OR ALTER PROC DeleteBulitin(
	@BulitinID int
)
AS
DELETE FROM Bulitin
WHERE BulitinID = @BulitinID
GO
/*DeletePost*/
CREATE OR ALTER PROC DeletePost(
	@PostId int
)
AS
DELETE FROM Post
WHERE PostId = @PostId
GO
/*DeletePrayerRequest*/
CREATE OR ALTER PROC DeletePrayerRequest(
	@PrayerID int
)
AS
DELETE From PrayerSubscribed
WHERE PrayerID = @PrayerID
DELETE FROM Prayer
WHERE PrayerID = @PrayerID
GO
/*DisbandGroup*/
CREATE OR ALTER PROC DisbandGroup(
	@GroupID int
)
AS

DELETE FROM Bulitin
WHERE GroupID = @GroupID
DELETE FROM Post
WHERE GroupID = @GroupID
DELETE FROM GroupSubscribed
WHERE GroupID = @GroupID
DELETE FROM GroupMod
WHERE GroupID = @GroupID
DELETE FROM Picture
WHERE GroupID = @GroupID
DELETE FROM UserGroup
WHERE GroupID = @GroupID
GO
/*GetAllGroups*/
CREATE OR ALTER PROC GetAllGroups(
	@TextFilter varchar(200),
	@State		int
)
AS
IF @State > 0
BEGIN
	SELECT *
	FROM UserGroup
	WHERE (CONTAINS(GroupName, @TextFilter) OR CONTAINS(GroupDescription, @TextFilter) OR CONTAINS(GroupContact, @TextFilter) OR CONTAINS(GroupLocation, @TextFilter)) AND USState = @State
END ELSE BEGIN
	SELECT *
	FROM UserGroup
	WHERE CONTAINS(GroupName, @TextFilter) OR CONTAINS(GroupDescription, @TextFilter) OR CONTAINS(GroupContact, @TextFilter) OR CONTAINS(GroupLocation, @TextFilter)
END
GO
CREATE OR ALTER PROC GetAllGroupsNoText(
	@State		int
)
AS
IF @State > 0
BEGIN
	SELECT *
	FROM UserGroup
	WHERE USState = @State
END ELSE BEGIN
	SELECT *
	FROM UserGroup
END
GO
/*GetAllPrayerRequests*/
CREATE OR ALTER PROC GetAllPrayerRequests(
	@TextFilter varchar(200),
	@State		int,
	@Catagorie	int
)
AS
IF @State > 0
BEGIN
	IF @Catagorie > 0
	BEGIN
		SELECT *
		FROM Prayer
		WHERE (CONTAINS(Title, @TextFilter) OR CONTAINS(ShortDescription, @TextFilter) OR CONTAINS(LongDescription, @TextFilter)) AND USState = @State AND Catagorie = @Catagorie
	END ELSE BEGIN
		SELECT *
		FROM Prayer
		WHERE (CONTAINS(Title, @TextFilter) OR CONTAINS(ShortDescription, @TextFilter) OR CONTAINS(LongDescription, @TextFilter)) AND USState = @State
	END
END ELSE BEGIN
	IF @Catagorie > 0
	BEGIN
		SELECT *
		FROM Prayer
		WHERE (CONTAINS(Title, @TextFilter) OR CONTAINS(ShortDescription, @TextFilter) OR CONTAINS(LongDescription, @TextFilter)) AND Catagorie = @Catagorie
	END ELSE BEGIN
		SELECT *
		FROM Prayer
		WHERE CONTAINS(Title, @TextFilter) OR CONTAINS(ShortDescription, @TextFilter) OR CONTAINS(LongDescription, @TextFilter)
	END
END
GO
CREATE OR ALTER PROC GetAllPrayerRequestsNoText(
	@State		int,
	@Catagorie	int
)
AS
IF @State > 0
BEGIN
	IF @Catagorie > 0
	BEGIN
		SELECT *
		FROM Prayer
		WHERE USState = @State AND Catagorie = @Catagorie
	END ELSE BEGIN
		SELECT *
		FROM Prayer
		WHERE USState = @State
	END
END ELSE BEGIN
	IF @Catagorie > 0
	BEGIN
		SELECT *
		FROM Prayer
		WHERE Catagorie = @Catagorie
	END ELSE BEGIN
		SELECT *
		FROM Prayer
	END
END
GO
/*GetAllUserOwnedGroups*/
CREATE OR ALTER PROC GetAllUserOwnedGroups(
	@UserID		nvarchar(450)
)
AS
SELECT *
FROM UserGroup
WHERE OwnerID = @UserID
GO
/*GetAllUserPrayer*/
CREATE OR ALTER PROC GetAllUserPrayer(
	@UserID		nvarchar(450)
)
AS
SELECT *
FROM Prayer
WHERE UserID = @UserID
GO
/*GetAllUserSubGroups*/
CREATE OR ALTER PROC GetAllUserSubGroups(
	@UserID		nvarchar(450)
)
AS
SELECT UserGroup.*
FROM UserGroup INNER JOIN GroupSubscribed ON UserGroup.GroupID = GroupSubscribed.GroupID
WHERE GroupSubscribed.UserID = @UserID
GO
/*GetAllUserSubPrayer*/
CREATE OR ALTER PROC GetAllUserSubPrayer(
	@UserID		nvarchar(450)
)
AS
SELECT Prayer.*/*Prayer.PrayerID, Prayer.Catagorie , Prayer.Title , Prayer.ShortDescription , Prayer.Expiration , Prayer.PostAnonymous , Prayer.UserID , Prayer.LongDescription , Prayer.USState*/
FROM Prayer INNER JOIN PrayerSubscribed ON Prayer.PrayerID = PrayerSubscribed.PrayerID
WHERE PrayerSubscribed.UserID = @UserID
GO
/*GetGroup*/
CREATE OR ALTER PROC GetGroup(
	@GroupID	int
)
AS
SELECT *
FROM UserGroup
WHERE GroupID = @GroupID
GO
/*GetPrayer*/
CREATE OR ALTER PROC GetPrayer(
	@PrayerID	int
)
AS
SELECT *
FROM Prayer
WHERE PrayerID = @PrayerID
GO
/*GetGroupImage*/
CREATE OR ALTER PROC GetGroupImage(
	@GroupID	int
)
AS
SELECT *
FROM Picture
WHERE GroupID = @GroupID
GO
/*PromoteMod*/
CREATE OR ALTER PROC PromoteMod(
	@GroupID	int,
	@NewModID	nvarchar(450)
)
AS
Insert Into GroupMod(GroupID, UserID)
Values(@GroupID,@NewModID)
GO
/*SetGroupImage*/
CREATE OR ALTER PROC SetGroupImage(
	@GroupID	int,
	@Path		nvarchar(100)
)
AS
Insert Into Picture(PicturePath, GroupID)
Values(@Path, @GroupID)
GO
/*SubscribeToGroup*/
CREATE OR ALTER PROC SubscribeToGroup(
	@GroupID	int,
	@UserID		nvarchar(450)
)
AS
Insert Into GroupSubscribed(GroupID, UserID)
Values(@GroupID,@UserID)
GO
/*SubscribeToPrayer*/
CREATE OR ALTER PROC SubscribeToPrayer(
	@PrayerID	int,
	@UserID		nvarchar(450)
)
AS
Insert Into PrayerSubscribed(PrayerID, UserID)
Values(@PrayerID,@UserID)
GO
/*UnsubscribeGroup*/
CREATE OR ALTER PROC UnsubscribeGroup(
	@GroupID	int,
	@UserID		nvarchar(450)
)
AS
DELETE FROM GroupSubscribed
WHERE GroupID = @GroupID AND UserID = @UserID
GO
/*UnsubscribePrayerRequest*/
CREATE OR ALTER PROC UnsubscribePrayerRequest(
	@PrayerID	int,
	@UserID		nvarchar(450)
)
AS
DELETE FROM PrayerSubscribed
WHERE PrayerID = @PrayerID AND UserID = @UserID
GO
CREATE OR ALTER PROC GetGroupModerators(
	@GroupID	int
)
AS
SELECT UserID
FROM GroupMod
WHERE GroupID = @GroupID
GO
CREATE OR ALTER PROC GetGroupSubscribers(
	@GroupID	int
)
AS
SELECT UserID
FROM GroupSubscribed
WHERE GroupID = @GroupID
GO
CREATE OR ALTER PROC GetPrayerSubscribers(
	@PrayerID	int
)
AS
SELECT UserID
FROM PrayerSubscribed
WHERE PrayerID = @PrayerID
GO

CREATE OR ALTER PROC GetUserSubscribedGroups(
	@UserID	nvarchar(450)
)
AS
SELECT GroupID
FROM GroupSubscribed
WHERE UserID = @UserID
GO
CREATE OR ALTER PROC GetUserSubscribedPrayers(
	@UserID	nvarchar(450)
)
AS
SELECT PrayerID
FROM PrayerSubscribed
WHERE UserID = @UserID
GO
CREATE OR ALTER PROC DemoteMod(
	@ModID		nvarchar(450),
	@GroupID	int
)
AS
DELETE FROM GroupMod
WHERE UserID = @ModID AND GroupID = @GroupID
GO
CREATE OR ALTER PROC RemoveMember(
	@MemberID		nvarchar(450),
	@GroupID	int
)
AS
DELETE FROM GroupSubscribed
WHERE UserID = @MemberID AND GroupID = @GroupID
GO
CREATE OR ALTER PROC GetAllBullitin(
	@GroupID		int
)
AS
SELECT *
FROM Bulitin
WHERE GroupID = @GroupID
ORDER BY DatePosted ASC 
GO
CREATE OR ALTER PROC GetAllPosts(
	@GroupID	int
)
AS
SELECT *
FROM Post
WHERE GroupID = @GroupID
ORDER BY DatePosted
GO
CREATE OR ALTER PROC GetPost(
	@PostID	int
)
AS
SELECT *
FROM Post
WHERE PostID = @PostID
GO