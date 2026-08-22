using Kyanite.Database.Default.Local;

namespace Kyanite.Database.Default.GoogleDrive;
/*
 * TODO: Finish implementation
 * 
 * Before this implemetation can be finished:
 * - make a way for DatabaseStack to know when to save data
 * - make a config panel that would let the user enter the data needed for stack to work
 */
public class GoogleDriveDatabaseStack() : LocalDatabaseStack(databasePath)
{
     const string databasePath = "./database.db";

     public override Task<bool> Prepare()
     {
          return base.Prepare();
     }
}