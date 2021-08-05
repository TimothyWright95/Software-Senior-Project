using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrayerRequest.Models;
using System.Drawing;
using System.IO;

namespace PrayerRequest.Repository
{
    public enum Catagorie
    {
        None = 1,
        Health,
        Finance,
        Emotional,
        Death,
        Pregnancy,
        Global,
        Community,
        Other
    }
    public enum States
    {
        [Description("Alabama")]
        AL = 1,
        [Description("Alaska")]
        AK,
        [Description("Arkansas")]
        AR,
        [Description("Arizona")]
        AZ,
        [Description("California")]
        CA,
        [Description("Colorado")]
        CO,
        [Description("Connecticut")]
        CT,
        [Description("D.C.")]
        DC,
        [Description("Delaware")]
        DE,
        [Description("Florida")]
        FL,
        [Description("Georgia")]
        GA,
        [Description("Hawaii")]
        HI,
        [Description("Iowa")]
        IA,
        [Description("Idaho")]
        ID,
        [Description("Illinois")]
        IL,
        [Description("Indiana")]
        IN,
        [Description("Kansas")]
        KS,
        [Description("Kentucky")]
        KY,
        [Description("Louisiana")]
        LA,
        [Description("Massachusetts")]
        MA,
        [Description("Maryland")]
        MD,
        [Description("Maine")]
        ME,
        [Description("Michigan")]
        MI,
        [Description("Minnesota")]
        MN,
        [Description("Missouri")]
        MO,
        [Description("Mississippi")]
        MS,
        [Description("Montana")]
        MT,
        [Description("North Carolina")]
        NC,
        [Description("North Dakota")]
        ND,
        [Description("Nebraska")]
        NE,
        [Description("New Hampshire")]
        NH,
        [Description("New Jersey")]
        NJ,
        [Description("New Mexico")]
        NM,
        [Description("Nevada")]
        NV,
        [Description("New York")]
        NY,
        [Description("Oklahoma")]
        OK,
        [Description("Ohio")]
        OH,
        [Description("Oregon")]
        OR,
        [Description("Pennsylvania")]
        PA,
        [Description("Rhode Island")]
        RI,
        [Description("South Carolina")]
        SC,
        [Description("South Dakota")]
        SD,
        [Description("Tennessee")]
        TN,
        [Description("Texas")]
        TX,
        [Description("Utah")]
        UT,
        [Description("Virginia")]
        VA,
        [Description("Vermont")]
        VT,
        [Description("Washington")]
        WA,
        [Description("Wisconsin")]
        WI,
        [Description("West Virginia")]
        WV,
        [Description("Wyoming")]
        WY
    }
    public interface IDBRepository
    {
        List<Group> GetAllGroups(string textFilter, States state);
        List<Group> GetAllUserOwnedGroups(string userID);
        List<Group> GetAllUserSubGroups(string userID);
        List<Prayer> GetAllUserPrayer(string userID);
        List<Prayer> GetAllUserSubPrayer(string userID);
        List<Prayer> GetAllPrayerRequests(string textFilter, Catagorie catagorie, States state );
        void CreatePrayerRequest(Prayer model);
        void SubscribeToGroup(int groupID, string usedID);
        void SubscribeToPrayer(int prayerID, string userID);
        void DeletePrayerRequest(int prayerID, string userID);
        void UnsubscribePrayerRequest(int prayerID, string userID);
        void UnsubscribeGroup(int groupID, string userId);
        void CreateGroup(Group model);
        void PromoteMod(int groupID, string userID, string newModID);
        void DisbandGroup(int groupID, string userID);
        Group GetGroup(int groupID);
        int AddPost(Post post);
        void DeletePost(int postId, string userID, int groupID);
        void AddBulitin(Bulitin model, int groupID);
        void DeleteBulitin(int bulitinID, string userID, int groupID);
        string GetGroupImage(int groupID);
        void SetGroupImage(string path, int groupID);
        string GetConnectionString();

        Prayer GetPrayer(int id);
        List<string> GetGroupMod(int ID);
        List<string> GetPrayerSubscribers(int ID);
        List<string> GetGroupSubscribers(int ID);
        List<int> GetUserSubscribedGroups(string id);
        List<int> GetUserSubscribedPrayers(string id);
        void DemoteMod(int groupID, string OwnerID, string newModID);

        void RemoveMember(int groupID, string OwnerID, string MemberID);
        List<Bulitin> GetAllBullitin(int groupID);
        List<Post> GetAllPosts(int GroupID);
        Post GetPost(int PostID);

    }


}
