// See https://aka.ms/new-console-template for more information
using System.Net.Http.Headers;
using System.Net;
using Octokit;
using System.Security.Principal;
using System;
using System.IO;
{
    Console.WriteLine("Enter owner");    
    string x = Console.ReadLine();

    Console.WriteLine("Enter repo");
    string y = Console.ReadLine();

    Console.WriteLine("Enter branch");
    string z = Console.ReadLine();

    var Client = new GitHubClient(new Octokit.ProductHeaderValue("Octokit-Test"));
    Client.Credentials = new Credentials("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");


    // github variables
    var owner = x;
    var repo = y;
    var branch = z;

    var targetFile = "https://github.com/" + owner + "/" + repo;

    try
    {
        //// try to get the file (and with the file the last commit sha)
        //var existingFile = await ghClient.Repository.Content.GetAllContentsByRef(owner, repo, targetFile, branch);

        //// update the file
        //var updateChangeSet = await ghClient.Repository.Content.UpdateFile(owner, repo, targetFile,
        //   new UpdateFileRequest("API File update", "Hello Universe! " + DateTime.UtcNow, existingFile.First().Sha, branch));

        var repository = Client.Repository.Get(owner, repo);
        Branch branch1 = await Client.Repository.Branch.Get(owner, repo, branch);


    }
    catch (Octokit.NotFoundException)
    {
        // if file is not found, create it
        var createChangeSet = await Client.Repository.Content.CreateFile(owner, repo, targetFile, new CreateFileRequest("API File creation", "Hello Universe! " + DateTime.UtcNow, branch));
    }
    Console.WriteLine("Enter new repo");


    string RepositoryName = Console.ReadLine();
    //string RepositoryName = "NewTestRepoOctoKit";



    var contextDelete = Client.Repository.Get(owner, RepositoryName);
    var reporResult = contextDelete.IsCompletedSuccessfully ? contextDelete.Result : null;
    long repositoryID = reporResult != null ? reporResult.Id : 0;

    try
    {
        var repository = new NewRepository(RepositoryName)
        {
            AutoInit = false,
            Description = "",
            LicenseTemplate = "mit",
            Private = false
        };


        if (!contextDelete.IsCompletedSuccessfully)
        {
            //var contextcreate = Client.Repository.Create(repository);

            var newRepository = Task.Run(async () => await Client.Repository.Create(repository)).GetAwaiter().GetResult();

            repositoryID = newRepository.Id;
            
            Console.WriteLine($"The respository {RepositoryName} was created.");
        }
        else
        {
            Console.WriteLine($"The respository already {RepositoryName} created.");
        }
    }
    catch (AggregateException e)
    {
        Console.WriteLine($"E: For some reason, the repository {RepositoryName}  can't be created. It may already exist. {e.Message}");
    }

    // Create  Repo
    //{


        //var contextdelete = Client.Repository.Delete(repositoryID);
        //OauthToken
        //var delRepository = Task.Run(async () => await Client.Repository.Delete(owner, RepositoryName)).GetAwaiter().GetResult();

        // Console.WriteLine($"The respository {RepositoryName} was deleted.");

        
    //}

    // Create File
    CreateFileRequest cfr = new CreateFileRequest($"First commit for {""}", "Empty", "main");
    var result = await Client.Repository.Content.CreateFile(owner, RepositoryName, "testfile1.cs", cfr);

    var sha = result.Commit.Sha;

    await Client.Repository.Content.UpdateFile(owner, RepositoryName, "testfile1.cs", new UpdateFileRequest("My updated file", "New file update", sha));
    // test



    // Git Clone
    
    var trees = Client.Git.Tree.GetRecursive(owner, RepositoryName, sha).Result;

    //Client.Repository.CreateRepository(newRepository, account.Login, account.IsUser)
    //    .Select(repository => cloneService.CloneRepository(repository.CloneUrl, repository.Name, directory))
    //    .SelectUnit();

    // Git Fetch
    // Git Pull
    // Git Push
}
