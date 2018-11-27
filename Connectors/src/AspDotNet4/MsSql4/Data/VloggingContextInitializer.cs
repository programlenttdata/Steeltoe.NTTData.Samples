using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace MsSql4.Data
{
    public class VloggingContextInitializer : CreateDatabaseIfNotExists<VloggingContext>
    {
        protected override void Seed(VloggingContext context)
        {
            Console.Out.WriteLine("Seeding Database Vlogging");
            try
            {
                var blog = new Blog { Name = "Sample Vlog" };
                var posts = new List<Post> {
                    new Post { Title = "First Post Vlog", Content = "This is the first sample vlog post" },
                    new Post { Title = "Second Post Vlog", Content = "This is the second sample vlog post" }
                };
                blog.Posts = posts;
                context.Blogs.Add(blog);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }

            base.Seed(context);
        }
    }
}