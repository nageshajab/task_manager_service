using TaskManager.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Reflection;
using File = TaskManager.Models.File;


namespace DAL
{
    public class FileManager
    {
        public string ConnectionString = Environment.GetEnvironmentVariable("ApplicationDbContext");

        public List<File> ListFilesByUserId(FileSearch fileSearch)
        {
            List<File> files = new();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = $"select * from (select b.userid,STRING_AGG(t.tag, ', ')as tags from files f, tags t where f.id=t.fileid group by f.userid) tags,(select * from files) filedetails where tags.UserId=filedetails.userid and tags.userid={fileSearch.UserId}";
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    File file = new File();
                    file.Name = reader.GetString("name");
                    file.UserId = reader.GetString("userid");
                    file.Description = reader.GetString("description");
                    file.ParentFolder = reader.GetString("parentfolder");
                    file.GoogleDrivePath = reader.GetString("googledrivepath");
                    file.AzurePath = reader.GetString("azurepath");
                    file.Tags = new List<string>(reader.GetString("tags").Split(",", StringSplitOptions.None));
                    files.Add(file);
                }

                connection.Close();
            }
            List<File> returnval = new List<File>();

            for (int i = 0; i < fileSearch.Tags.Length; i++)
            {
                string tag = fileSearch.Tags[i];
                foreach (File f in files)
                {
                    if (f.Tags.Contains(tag))
                        returnval.Add(f);
                }
            }
            return files;
        }

        public int Insert(File file)
        {
            int fileid;
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"INSERT INTO [dbo].[Files] ([userid],[name],[description],[parentFolder],[GoogledrivePath],[azurePath]) output INSERTED.ID VALUES ({file.UserId} ,'{file.Name}' ,'{file.Description}' ,'{file.ParentFolder}','{file.GoogleDrivePath}','{file.AzurePath}')";

                    command.Transaction = transaction;
                    fileid = (int)command.ExecuteScalar();

                    //add userroles
                    AddTagsForFile(fileid, file.Tags, connection, transaction);
                    transaction.Commit();
                }
            }
            return fileid;
        }

        public bool DeleteAllFiles()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command2 = new SqlCommand();
                    command2.Connection = connection;
                    command2.Transaction = transaction;
                    command2.CommandText = "delete tags where fileid is not null";
                    command2.ExecuteScalar();

                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = "delete files";
                    command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public void AddTagsForFile(int fileid, List<string> tags, SqlConnection connection, SqlTransaction transaction)
        {
            DeleteTagsByFileId(fileid, connection, transaction);

            foreach (string tag in tags)
            {
                SqlCommand command2 = new SqlCommand();
                command2.Connection = connection;
                command2.CommandText = $"insert into [tags](tag,fileid) values('{tag}',{fileid})";

                command2.Transaction = transaction;

                var affected2 = command2.ExecuteNonQuery();
            }
        }

        public void DeleteTagsByFileId(int fileid, SqlConnection connection, SqlTransaction transaction)
        {
            SqlCommand command2 = new SqlCommand();
            command2.Connection = connection;
            command2.Transaction = transaction;
            command2.CommandText = $"delete tags where fileid={fileid}";

            command2.ExecuteNonQuery();
        }

        public string Update(File file, int id)
        {
            File fileFromDb = Get(id);

            if (fileFromDb.Id == 0)
            {
                return "User with id not found";
            }

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"UPDATE [dbo].[Files] SET [userid] = {file.UserId},{file.Name},{file.Description},{file.ParentFolder},{file.GoogleDrivePath},{file.AzurePath} WHERE id={file.Id}";

                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    AddTagsForFile(id, file.Tags, connection, transaction);

                    transaction.Commit();
                }
            }
            return "updated";
        }

        public File Get(int fileid)
        {
            File file = new File();
            User User = new User();
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select * from (select b.userid,STRING_AGG(t.tag, ', ')as tags from files f, tags t where f.id=t.fileid group by b.userid) tags,(select * from files) filedetails where tags.fileid=filedetails.id and tags.fileid={fileid}";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    file.Name = reader.GetString("name");
                    file.UserId = reader.GetString("userid");
                    file.Description = reader.GetString("description");
                    file.ParentFolder = reader.GetString("parentfolder");
                    file.GoogleDrivePath = reader.GetString("googledrivepath");
                    file.AzurePath = reader.GetString("azurepath");
                    file.Tags = new List<string>(reader.GetString("tags").Split(",", StringSplitOptions.None));
                }
                connection.Close();
            }
            return file;
        }

        public List<string> GetTagsByUserId(int userid)
        {
            List<string> tags = new List<string>();
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select b.userid,STRING_AGG(t.tag, ', ') as tags  from blogs b, tags t where b.id = t.blogid and b.userid={userid} group by b.userid";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tags = reader.GetString("tags").Split(",", StringSplitOptions.None).ToList();
                }
                connection.Close();
            }
            return tags;
        }

        public bool DeleteFile(int fileid)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command2 = new SqlCommand();
                    command2.Connection = connection;
                    command2.Transaction = transaction;
                    command2.CommandText = "delete tags where fileid =" + fileid;
                    command2.ExecuteScalar();

                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = "delete files where id=" + fileid;
                    command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }
    }
}
