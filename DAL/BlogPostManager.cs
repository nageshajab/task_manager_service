using TaskManager.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace DAL
{
    public class BlogPostManager
    {
        public string paramUserid = "userid";
        public string paramTitle = "title";
        public string paramDescription = "description";
        public string paramRepositoryUrl = "repositoryUrls";
        public string paramBlogPostUrl = "blogpostUrl";
        public string paramTag = "tag";
        public string paramBlogid = "blogid";
        public string paramId = "id";
        public string ConnectionString = Environment.GetEnvironmentVariable("ApplicationDbContext");

        public List<BlogPost> List(int userid)
        {
            string querySelectBlogs = $"select * from blogs where userid={userid}";
            List<BlogPost> blogPosts = new List<BlogPost>();
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = querySelectBlogs;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    BlogPost blogPost = new BlogPost();
                    blogPost.BlogPostUrl = reader.GetString(paramBlogPostUrl);
                    blogPost.Title = reader.GetString(paramTitle);
                    blogPost.UserId = reader.GetInt16(paramUserid);
                    blogPost.Id = reader.GetInt32(paramId);
                    blogPost.Description = reader.GetString(paramDescription);
                    blogPost.RepositoryUrl = reader.GetString(paramRepositoryUrl);

                    //List<string> tags=new List<string>();
                    SqlCommand command2 = new SqlCommand();
                    command2.Connection = connection;
                    command2.CommandText = "select * from tags where blogid=" + blogPost.Id;
                    SqlDataReader reader2 = command2.ExecuteReader();
                    while (reader2.Read())
                    {
                        blogPost.Tags.Append(reader2.GetString(paramTag));
                    }

                    blogPosts.Add(blogPost);
                }

                connection.Close();
            }
            return blogPosts;
        }

        public int Insert(BlogPost blogPost)
        {
            int blogid;
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"insert into blogs(userid,title,description,repositoryurls,blogposturl) output INSERTED.ID values({blogPost.UserId},'{blogPost.Title}','{blogPost.Description}','{blogPost.RepositoryUrl}','{blogPost.BlogPostUrl}')";

                    command.Transaction = transaction;
                    blogid = (int)command.ExecuteScalar();

                    //add tags
                    AddTagsForBlogId(blogid, blogPost.Tags, connection, transaction);
                    transaction.Commit();
                }
            }
            return blogid;
        }

        public bool DeleteBlogPost(int id)
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
                    command2.CommandText = $"delete tags where blogid={id}";
                    command2.ExecuteScalar();

                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = $"delete blogs where id={id}";
                    command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public bool DeleteAllBlogPosts()
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
                    command2.CommandText = "delete tags where blogid is not null";
                    command2.ExecuteScalar();

                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = "delete blogs";
                    command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public void AddTagsForBlogId(int blogid, List<string> tags, SqlConnection connection, SqlTransaction transaction)
        {
            DeleteTagsByBlogId(blogid, connection, transaction);

            foreach (string tag in tags)
            {
                SqlCommand command2 = new SqlCommand();
                command2.Connection = connection;
                command2.CommandText = $"insert into tags(blogid,tag) values({blogid},'{tag}')";

                command2.Transaction = transaction;

                var affected2 = command2.ExecuteNonQuery();
            }
        }

        public void DeleteTagsByBlogId(int blogid, SqlConnection connection, SqlTransaction transaction)
        {
            SqlCommand command2 = new SqlCommand();
            command2.Connection = connection;
            command2.Transaction = transaction;
            command2.CommandText = $"delete tags where {paramBlogid}=@{paramBlogid}";

            command2.ExecuteScalar();
        }

        public string Update(BlogPost blogPost, int id)
        {
            BlogPost blogPostFromDb = Get(id);

            if (blogPostFromDb.Id == 0)
            {
                return "BlogPost with id not found";
            }

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"update blogs set userid=@{blogPost.UserId}, title='{blogPost.Title}', description='{blogPost.Description}', repositoryurls='{blogPost.RepositoryUrl}',blogposturl='{blogPost.BlogPostUrl}' where id =@{id}";

                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    AddTagsForBlogId(id, blogPost.Tags, connection, transaction);

                    transaction.Commit();
                }
            }
            return "updated";
        }

        public BlogPost Get(int blogid)
        {
            BlogPost blogPost = new BlogPost();
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();


                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select * from blogs where id={blogid}";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    blogPost.BlogPostUrl = reader.GetString(paramBlogPostUrl);
                    blogPost.Title = reader.GetString(paramTitle);
                    blogPost.UserId = reader.GetInt32(paramUserid);
                    blogPost.Id = reader.GetInt32(paramId);
                    blogPost.Description = reader.GetString(paramDescription);
                    blogPost.RepositoryUrl = reader.GetString(paramRepositoryUrl);
                }
                connection.Close();
            }

            blogPost = GetTagsForBlogs(blogid, blogPost);

            return blogPost;
        }

        public BlogPost GetTagsForBlogs(int blogid, BlogPost blogPost)
        {
            blogPost.Tags = new List<string>();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                SqlCommand command2 = new SqlCommand();
                command2.Connection = connection;

                command2.CommandText = $"select * from tags where {paramBlogid}=" + blogPost.Id;
                SqlDataReader reader2 = command2.ExecuteReader();

                while (reader2.Read())
                {
                    string tag = reader2.GetString(paramTag);
                    blogPost.Tags.Append(tag);
                }

                connection.Close();
            }
            return blogPost;
        }
    }
}
