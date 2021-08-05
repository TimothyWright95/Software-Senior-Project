using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using PrayerRequest.Models;



namespace PrayerRequest.Repository
{
    public class DBRepository : IDBRepository
    {
        public void AddBulitin(Bulitin model, int groupID)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("AddBulitin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@Bulitinmessage", model.Bulitinmessage);
                    command.Parameters.AddWithValue("@EventTitle", model.EventTitle);
                    command.Parameters.AddWithValue("@GroupID", groupID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public int AddPost(Post model)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("AddPost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@Postmessage", model.Postmessage);
                    command.Parameters.AddWithValue("@GroupID", model.GroupID);
                    command.Parameters.AddWithValue("@OwnerID", model.OwnerID);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public void CreateGroup(Group model)
        {
            //if (model.GroupPassword == null) model.GroupPassword = "nopassword";
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("CreateGroup", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@GroupContact", model.GroupContact);
                    command.Parameters.AddWithValue("@GroupDescription", model.GroupDescription);
                    command.Parameters.AddWithValue("@GroupLocation", model.GroupLocation);
                    command.Parameters.AddWithValue("@GroupName", model.GroupName);
                    command.Parameters.AddWithValue("@GroupOpen", model.GroupOpen);
                    if (model.GroupPassword != null)
                        command.Parameters.AddWithValue("@GroupPassword", model.GroupPassword);
                    command.Parameters.AddWithValue("@OwnerID", model.OwnerID);
                    command.Parameters.AddWithValue("@State", model.State);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void CreatePrayerRequest(Prayer model)
        {
            if (model.LongDescription == null) model.LongDescription = "";
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("CreatePrayerRequest", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@LongDescription", model.LongDescription);
                    command.Parameters.AddWithValue("@ShortDescription", model.ShortDescription);
                    command.Parameters.AddWithValue("@PostAnonymous", model.PostAnonymous);
                    command.Parameters.AddWithValue("@Title", model.Title);
                    command.Parameters.AddWithValue("@UserID", model.UserID);
                    command.Parameters.AddWithValue("@Expiration", model.Expiration);
                    command.Parameters.AddWithValue("@Catagorie", model.Catagorie);
                    command.Parameters.AddWithValue("@State", model.State);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteBulitin(int bulitinID, string userID, int groupID)
        {
            //check perms
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("DeleteBulitin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@BulitinID", bulitinID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeletePost(int postId, string userID, int groupID)
        {
            //check perms
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("DeletePost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@PostId", postId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeletePrayerRequest(int prayerID, string userID)
        {
            //check perms
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("DeletePrayerRequest", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@PrayerID", prayerID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DisbandGroup(int groupID, string userID)
        {
            //check perms
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("DisbandGroup", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@GroupID", groupID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Group> GetAllGroups(string textFilter, States state)
        {
            string procedure;
            if (textFilter == "")
            {
                procedure = "GetAllGroupsNoText";
            }
            else
            {
                procedure = "GetAllGroups";
            }
            List<Group> list = new List<Group>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(procedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    if (procedure == "GetAllGroups")
                        command.Parameters.AddWithValue("@TextFilter", textFilter.Length > 199 ? textFilter.Substring(0, 199) : textFilter);
                    command.Parameters.AddWithValue("@State", state);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Group temp = new Group
                            {
                                GroupContact = reader["GroupContact"].ToString(),
                                GroupDescription = reader["GroupDescription"].ToString(),
                                GroupLocation = reader["GroupLocation"].ToString(),
                                GroupName = reader["GroupName"].ToString(),
                                GroupOpen = (bool)reader["GroupOpen"],
                                GroupPassword = reader["GroupPassword"].ToString(),
                                OwnerID = reader["OwnerID"].ToString(),
                                State = (int)reader["USState"],
                                ID = (int)reader["GroupID"]
                            };
                            list.Add(temp);
                        }
                    }
                }
            }
            return list;
        }

        public List<Prayer> GetAllPrayerRequests(string textFilter, Catagorie catagorie, States state)
        {
            string procedure;
            if(textFilter == "")
            {
                procedure = "GetAllPrayerRequestsNoText";
            }
            else
            {
                procedure = "GetAllPrayerRequests";
            }
            List<Prayer> list = new List<Prayer>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(procedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@Catagorie", catagorie);
                    if(procedure == "GetAllPrayerRequests")
                        command.Parameters.AddWithValue("@TextFilter", textFilter.Length > 199 ? textFilter.Substring(0, 199) : textFilter);
                    command.Parameters.AddWithValue("@State", state);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Prayer temp = new Prayer
                            {
                                Catagorie = (PrayerRequest.Repository.Catagorie)((int)reader["Catagorie"]),
                                Expiration = DateTime.Parse(reader["Expiration"].ToString()),
                                LongDescription = reader["LongDescription"].ToString(),
                                PostAnonymous = (bool)reader["PostAnonymous"],
                                ShortDescription = reader["ShortDescription"].ToString(),
                                Title = reader["Title"].ToString(),
                                UserID = reader["UserID"].ToString(),
                                State = (PrayerRequest.Repository.States)((int)reader["USState"]),
                                ID = (int)reader["PrayerID"]
                            };
                            list.Add(temp);
                        }
                    }
                }
            }
            return list;
        }

        public List<Group> GetAllUserOwnedGroups(string userID)
        {
            List<Group> list = new List<Group>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetAllUserOwnedGroups", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@UserID", userID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Group temp = new Group
                            {
                                GroupContact = reader["GroupContact"].ToString(),
                                GroupDescription = reader["GroupDescription"].ToString(),
                                GroupLocation = reader["GroupLocation"].ToString(),
                                GroupName = reader["GroupName"].ToString(),
                                GroupOpen = (bool)reader["GroupOpen"],
                                GroupPassword = reader["GroupPassword"].ToString(),
                                OwnerID = reader["OwnerID"].ToString(),
                                State = (int)reader["USState"],
                                ID = (int)reader["GroupID"]
                            };
                            list.Add(temp);
                        }
                    }
                }
            }
            return list;
        }

        public List<Prayer> GetAllUserPrayer(string userID)
        {
            List<Prayer> list = new List<Prayer>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetAllUserPrayer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@UserID", userID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Prayer temp = new Prayer
                            {
                                Catagorie = (PrayerRequest.Repository.Catagorie)((int)reader["Catagorie"]),
                                Expiration = DateTime.Parse(reader["Expiration"].ToString()),
                                LongDescription = reader["LongDescription"].ToString(),
                                PostAnonymous = (bool)reader["PostAnonymous"],
                                ShortDescription = reader["ShortDescription"].ToString(),
                                Title = reader["Title"].ToString(),
                                UserID = reader["UserID"].ToString(),
                                State = (PrayerRequest.Repository.States)((int)reader["USState"]),
                                ID = (int)reader["PrayerID"]
                            };
                            list.Add(temp);
                        }
                    }
                }
            }
            return list;
        }

        public List<Group> GetAllUserSubGroups(string userID)
        {
            List<Group> list = new List<Group>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetAllUserSubGroups", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@UserID", userID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Group temp = new Group
                            {
                                GroupContact = reader["GroupContact"].ToString(),
                                GroupDescription = reader["GroupDescription"].ToString(),
                                GroupLocation = reader["GroupLocation"].ToString(),
                                GroupName = reader["GroupName"].ToString(),
                                GroupOpen = (bool)reader["GroupOpen"],
                                GroupPassword = reader["GroupPassword"].ToString(),
                                OwnerID = reader["OwnerID"].ToString(),
                                State = (int)reader["USState"],
                                ID = (int)reader["GroupID"]
                            };
                            list.Add(temp);
                        }
                    }
                }
            }
            return list;
        }

        public List<Prayer> GetAllUserSubPrayer(string userID)
        {
            List<Prayer> list = new List<Prayer>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetAllUserSubPrayer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@UserID", userID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Prayer temp = new Prayer
                            {
                                Catagorie = (PrayerRequest.Repository.Catagorie)((int)reader["Catagorie"]),
                                Expiration = DateTime.Parse(reader["Expiration"].ToString()),
                                LongDescription = reader["LongDescription"].ToString(),
                                PostAnonymous = (bool)reader["PostAnonymous"],
                                ShortDescription = reader["ShortDescription"].ToString(),
                                Title = reader["Title"].ToString(),
                                UserID = reader["UserID"].ToString(),
                                State = (PrayerRequest.Repository.States)((int)reader["USState"]),
                                ID = (int)reader["PrayerID"]
                            };
                            list.Add(temp);
                        }
                    }
                }
            }
            return list;
        }

        public List<Post> GetAllPosts(int GroupID)
        {
            List<Post> list = new List<Post>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetAllPosts", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@GroupID", GroupID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Post temp = new Post
                            {
                                Postmessage = reader["Postmessage"].ToString(),
                                OwnerID = reader["OwnerID"].ToString(),
                                ID = (int)reader["PostID"],
                                GroupID = (int)reader["GroupID"],
                                Date = DateTime.Parse(reader["DatePosted"].ToString())
                            };
                            list.Add(temp);
                        }
                    }
                }
            }
            return list;
        }

        public string GetConnectionString()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddJsonFile($"appsettings.{environment}.json", optional: true)
           .AddEnvironmentVariables();

            if (environment == "Development")
            {
                builder.AddUserSecrets<Startup>();
            }

            var Configuration = builder.Build();
            return Configuration.GetConnectionString("PrayerRequestContextConnection");
        }

        public Group GetGroup(int groupID)
        {
            Group temp = new Group();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetGroup", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@GroupID", groupID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            temp.GroupContact = reader["GroupContact"].ToString();
                            temp.GroupDescription = reader["GroupDescription"].ToString();
                            temp.GroupLocation = reader["GroupLocation"].ToString();
                            temp.GroupName = reader["GroupName"].ToString();
                            temp.GroupOpen = (bool)reader["GroupOpen"];
                            temp.GroupPassword = reader["GroupPassword"].ToString();
                            temp.OwnerID = reader["OwnerID"].ToString();
                            temp.State = (int)reader["USState"];
                            temp.ID = (int)reader["GroupID"];

                        }
                    }
                }
            }
            return temp;
        }

        public Post GetPost(int PostID)
        {
            Post temp = new Post();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetPost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@PostID", PostID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            temp.Postmessage = reader["Postmessage"].ToString();
                            temp.OwnerID = reader["OwnerID"].ToString();
                            temp.ID = (int)reader["PostID"];
                            temp.GroupID = (int)reader["GroupID"];
                            temp.Date = DateTime.Parse(reader["DatePosted"].ToString());

                        }
                    }
                }
            }
            return temp;
        }

        public string GetGroupImage(int groupID)
        {
            string temp = string.Empty;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetGroupImage", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@GroupID", groupID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            temp = reader["PicturePath"].ToString();
                        }
                    }
                }
            }
            return temp;
        }

        public List<string> GetGroupMod(int ID)
        {
            List<string> toreturn = new List<string>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetGroupModerators", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@GroupID", ID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            toreturn.Add(reader["UserID"].ToString());
                        }
                    }
                }
            }
            return toreturn;
        }

        public List<string> GetGroupSubscribers(int ID)
        {
            List<string> toreturn = new List<string>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetGroupSubscribers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@GroupID", ID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            toreturn.Add(reader["UserID"].ToString());
                        }
                    }
                }
            }
            return toreturn;
        }

        public Prayer GetPrayer(int id)
        {
            Prayer temp = new Prayer();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetPrayer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@PrayerID", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            temp.Catagorie = (PrayerRequest.Repository.Catagorie)((int)reader["Catagorie"]);
                            temp.Expiration = DateTime.Parse(reader["Expiration"].ToString());
                            temp.LongDescription = reader["LongDescription"].ToString();
                            temp.PostAnonymous = (bool)reader["PostAnonymous"];
                            temp.ShortDescription = reader["ShortDescription"].ToString();
                            temp.Title = reader["Title"].ToString();
                            temp.UserID = reader["UserID"].ToString();
                            temp.State = (PrayerRequest.Repository.States)((int)reader["USState"]);
                            temp.ID = (int)reader["PrayerID"];
                        }
                    }
                }
            }
            return temp;
        }

        public List<string> GetPrayerSubscribers(int ID)
        {
            List<string> toreturn = new List<string>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetPrayerSubscribers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@PrayerID", ID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            toreturn.Add(reader["UserID"].ToString());
                        }
                    }
                }
            }
            return toreturn;
        }

        public List<int> GetUserSubscribedGroups(string id)
        {
            List<int> toreturn = new List<int>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetUserSubscribedGroups", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@UserID", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            toreturn.Add((int)reader["GroupID"]);
                        }
                    }
                }
            }
            return toreturn;
        }

        public List<int> GetUserSubscribedPrayers(string id)
        {
            List<int> toreturn = new List<int>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetUserSubscribedPrayers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@UserID", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            toreturn.Add((int)reader["PrayerID"]);
                        }
                    }
                }
            }
            return toreturn;
        }

        public void PromoteMod(int groupID, string OwnerID, string newModID)
        {
            //Check perms
            using(SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("PromoteMod", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@NewModID", newModID);
                    command.Parameters.AddWithValue("@GroupID", groupID);
                    command.ExecuteNonQuery();
                }
            }
        }
        public void DemoteMod(int groupID, string OwnerID, string newModID)
        {
            //Check perms
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("DemoteMod", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@ModID", newModID);
                    command.Parameters.AddWithValue("@GroupID", groupID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void RemoveMember(int groupID, string OwnerID, string MemberID)
        {
            //Check perms
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("RemoveMember", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@MemberID", MemberID);
                    command.Parameters.AddWithValue("@GroupID", groupID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SetGroupImage(string path, int groupID)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("SetGroupImage", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@Path", path);
                    command.Parameters.AddWithValue("@GroupID", groupID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SubscribeToGroup(int groupID, string userID)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("SubscribeToGroup", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@GroupID", groupID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SubscribeToPrayer(int prayerID, string userID)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("SubscribeToPrayer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@PrayerID", prayerID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UnsubscribeGroup(int groupID, string userID)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("UnsubscribeGroup", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@GroupID", groupID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UnsubscribePrayerRequest(int prayerID, string userID)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("UnsubscribePrayerRequest", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@PrayerID", prayerID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Bulitin> GetAllBullitin(int groupID)
        {
            List<Bulitin> toreturn = new List<Bulitin>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("GetAllBullitin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@GroupID", groupID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var temp = new Bulitin
                            {
                                Bulitinmessage = reader["Bulitinmessage"].ToString(),
                                DatePosted = DateTime.Parse(reader["DatePosted"].ToString()),
                                EventTitle = reader["EventTitle"].ToString(),
                                ID = (int)reader["BulitinID"]
                            };
                            toreturn.Add(temp);
                        }
                    }
                }
            }
            return toreturn;
        }
    }
}
