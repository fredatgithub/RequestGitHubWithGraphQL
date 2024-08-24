using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using System;
using System.Net.Http;


namespace RequestGitHubWithGraphQL
{
  internal class Program
  {
    static async void Main()
    {
      // Remplacez 'your_github_token' par votre propre token GitHub
      var token = "your_github_token";

      // Initialiser le client GraphQL
      var httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
      httpClient.DefaultRequestHeaders.Add("User-Agent", "C#-GraphQL-Example");

      //var graphQLClient = new GraphQLClient(new GraphQLHttpClientOptions
      //{
      //  EndPoint = new Uri("https://api.github.com/graphql"),
      //  HttpMessageHandler = new HttpClientHandler()
      //}, new NewtonsoftJsonSerializer(), httpClient);

      var graphQLClient = new GraphQLHttpClient(new GraphQLHttpClientOptions
      {
        EndPoint = new Uri("https://api.github.com/graphql")
      }, new NewtonsoftJsonSerializer(), httpClient);

      // Construire la requête GraphQL
      var query = new GraphQLRequest
      {
        Query = @"
                query($login: String!) {
                  user(login: $login) {
                    login
                    name
                    repositories(first: 5, orderBy: {field: CREATED_AT, direction: DESC}) {
                      totalCount
                      nodes {
                        name
                        description
                      }
                    }
                  }
                }",
        Variables = new { login = "octocat" }  // Remplacez 'octocat' par un autre nom d'utilisateur si besoin
      };

      try
      {
        // Exécuter la requête
        var response = await graphQLClient.PostAsync(query);

        // Vérifier s'il y a des erreurs
        if (response.Errors != null)
        {
          Console.WriteLine("Errors:");
          foreach (var error in response.Errors)
          {
            Console.WriteLine(error.Message);
          }
          return;
        }

        // Traiter la réponse
        var user = response.Data["user"];
        Console.WriteLine($"User: {user["name"]} (Login: {user["login"]})");
        Console.WriteLine($"Total Repositories: {user["repositories"]["totalCount"]}");

        foreach (var repo in user["repositories"]["nodes"])
        {
          Console.WriteLine($"- {repo["name"]}: {repo["description"]}");
        }
      }
      catch (HttpRequestException httpRequestException)
      {
        Console.WriteLine($"Request error: {httpRequestException.Message}");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error: {ex.Message}");
      }

      Console.WriteLine("Press any key to exit:");
      Console.ReadKey();
    }
  }
}
