using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using static System.Net.Mime.MediaTypeNames;



namespace EIAS
{
    /// <summary>
    /// Author: Timur Maistrenko
    /// <br></br>
    /// Security Class containing security and cryptography-related methods.
    /// </summary>
    public class User
    {
        public static readonly int MaxAboutLength = 1024;
        private string role, userName, email, status, about, accountCreated, lastLogin, options;
        private int uid;
        private byte[] pfp;

        private void ParseRow(DataRow row)
        {
            role = row.Field<string>("Role");
            userName = row.Field<string>("UserName");
            email = row.Field<string>("Email");
            status = row.Field<string>("Status");
            about = row.Field<string>("About");
            accountCreated = row.Field<string>("AccountCreated");
            lastLogin = row.Field<string>("LastLogin");
            options = row.Field<string>("Options");
            uid = row.Field<int>("UID");
            pfp = row.Field<byte[]>("PFP");
        }

        public User(int uid, string role, string userName, string email, string status, string about, string accountCreated, string lastLogin, string options, byte[] pfp)
        {
            this.uid = uid;
            this.role = role;  
            this.userName = userName;  
            this.email = email;
            this.status = status;
            this.about = about;
            this.accountCreated = accountCreated;
            this.lastLogin = lastLogin;
            this.options = options;
            this.pfp = pfp;
        }

        public User(DataRow row)
        {
            //var str = row.ItemArray.Select(x => x.ToString()).ToArray();
            ParseRow(row);
        }

        public User(int targetUid)
        {
            DBManager db = new();

            string query = "SELECT * FROM Users WHERE UID = @UID";
            var parameters = new[]
            {
                new SqlParameter("@UID", targetUid),
            };

            DataTable result = db.Query(query, parameters);
            if (result.Rows.Count == 0)
            {
                throw new ArgumentException("A user with the provided UID does not exist");
            }

            ParseRow(result.Rows[0]);
        }

        public User(string targetEmail)
        {
            DBManager db = new();

            string query = "SELECT * FROM Users WHERE Email = @Email";
            var parameters = new[]
            {
                new SqlParameter("@Email", targetEmail),
            };

            DataTable result = db.Query(query, parameters);
            if (result.Rows.Count == 0)
            {
                throw new ArgumentException("A user with the provided email does not exist");
            }

            ParseRow(result.Rows[0]);
        }

        public int UpdatePassword(string password)
        {
            DBManager db = new();

            string query = "SELECT * FROM Login WHERE UID = @UID";
            var parameters = new[]
            {
                new SqlParameter("@UID", uid),
            };

            DataTable result = db.Query(query, parameters);
            if (result.Rows.Count == 0)
            {
                throw new ArgumentException("A user with the provided UID does not exist");
            }

            string salt = result.Rows[0].Field<string>("Salt");
            if (salt == null)
            {
                throw new Exception("No salt is stored in the database for this user");
            }
            if (Security.PasswordHash(password, salt).Equals(result.Rows[0].Field<string>("Hash")))
            {
                throw new ArgumentException("Reused Password");
            }

            salt = Security.GenerateSalt();
            query = "UPDATE Login SET Salt = @Salt, Hash = @Hash WHERE UID = @UID";
            // Define parameters for the query
            parameters = new[]
            {   new SqlParameter("@Salt", salt),
                new SqlParameter("@Hash", Security.PasswordHash(password, salt)),
                new SqlParameter("@UID", uid),
            };

            return db.NonQuery(query, parameters);
        }

        public int UpdateEmail(string newEmail)
        {
            if (!Security.IsValidEmail(newEmail))
            {
                throw new ArgumentException("Invalid email address.");
            }

            DBManager db = new();

            string query = "SELECT * FROM Login WHERE UID = @UID";
            var parameters = new[]
            {
                new SqlParameter("@UID", uid),
            };

            DataTable result = db.Query(query, parameters);
            if (result.Rows.Count == 0)
            {
                throw new ArgumentException("A user with the provided UID does not exist");
            }

            if (result.Rows[0].Field<string>("Email").Equals(newEmail))
            {
                return 0;
            }


            query = "UPDATE Login SET Email = @Email WHERE UID = @UID";
            
            parameters = new[]
            {   new SqlParameter("@Email", newEmail),
                new SqlParameter("@UID", uid),
            };

            db.NonQuery(query, parameters);


            query = "UPDATE Users SET Email = @Email WHERE UID = @UID";
            
            parameters = new[]
            {   new SqlParameter("@Email", newEmail),
                new SqlParameter("@UID", uid),
            };

            email = newEmail;
            return db.NonQuery(query, parameters);
        }

        public int UpdateAbout(string newAbout)
        {
            if (!Exists())
            {
                throw new ArgumentException("A user with the provided UID does not exist");
            }

            if (newAbout.Length > MaxAboutLength)
            {
                newAbout = newAbout.Substring(0, MaxAboutLength);
            }
            newAbout = Security.Sanitize(newAbout);

            DBManager db = new();

            string query = "UPDATE Users SET About = @About WHERE UID = @UID";
            
            var parameters = new[]
            {   
                new SqlParameter("@About", newAbout),
                new SqlParameter("@UID", uid)
            };

            int rowsChanged = db.NonQuery(query, parameters);
            if (rowsChanged > 0)
            {
                about = newAbout;
            }
            return rowsChanged;
        }

        public string Role { get => role; set => role = value; }
        public string UserName { get => userName; set => userName = value; }
        public string Email { get => email; set => email = value; }
        public string Status { get => status; set => status = value; }
        public string About { get => about; set => about = value; }
        public string AccountCreated { get => accountCreated; set => accountCreated = value; }
        public string LastLogin { get => lastLogin; set => lastLogin = value; }
        public string Options { get => options; set => options = value; }
        public int Uid { get => uid; set => uid = value; }
        public byte[] Pfp { get => pfp; set => pfp = value; }

        public static string UserDateTimeNow() => DateTime.Now.ToString("MM/dd/yyyy-HH:mm:ss:fff");
        public bool Exists()
        {
            DBManager db = new();

            string query = "SELECT * FROM Users WHERE UID = @UID";
            var parameters = new[]
            {
                new SqlParameter("@UID", uid),
            };

            DataTable result = db.Query(query, parameters);
            return result.Rows.Count > 0;


        }
    }
}
