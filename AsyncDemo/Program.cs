using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncDemo
{
    class Logger
    {
        public void log(string framework,string msg)
        {
            Console.WriteLine("component : {0},Message :{1} ", framework, msg);
        }
    }
    public  class OnlineFormatter:DatabaseLogFormatter
    {
        
     public OnlineFormatter(DbContext context, Action<string> writeAction) : base(context, writeAction) 
    { 
    } 
 
    public override void LogCommand<TResult>( DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext) 
    { 
        Write(string.Format( 
            "Context '{0}' is executing command '{1}'{2}", 
            Context.GetType().Name, 
            command.CommandText.Replace(Environment.NewLine, ""), Environment.NewLine)); 
    } 
 
    public override void LogResult<TResult>(DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext) 
    { 
    } 
}
    public class MyDbConfiguration : DbConfiguration
    {
        public MyDbConfiguration()
        {
            SetDatabaseLogFormatter(
                (context, writeAction) => new OnlineFormatter(context, writeAction));
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Func<int,int,string> funcEG=(x,y)=>((x+y).ToString());
            Console.WriteLine(funcEG.Invoke(2,3));
            Task ops=PerformDatabaseOperations();
            Console.WriteLine("Other task meanwhile....1");
            Console.WriteLine("Other task meanwhile....2"); 
            Console.ReadLine();
        }
         public static async Task PerformDatabaseOperations() 
        { 
            using (var db = new BloggingContext()) 
            {
               // db.Database.Log = Console.Write; 
                Logger log = new Logger();
                db.Database.Log = f => log.log("Ef", f);
                // Create a new blog and save it 
                db.Blogs.Add(new Blog 
                { 
                    Name = "Test Blog #" + (db.Blogs.Count() + 1) 
                });
                Console.WriteLine("Calling SaveChanges."); 
               await db.SaveChangesAsync();
               Console.WriteLine("SaveChanges completed."); 
                // Query for all blogs ordered by name 
               Console.WriteLine("Executing query."); 
                var blogs = (from b in db.Blogs 
                            orderby b.Name 
                            select b).ToList(); 
 
                // Write all blogs out to Console 
                Console.WriteLine();
                Console.WriteLine("Query completed with following results:"); 
                foreach (var blog in blogs) 
                { 
                    Console.WriteLine(" " + blog.Name); 
                } 
            } 
        } 
    
    }
}
