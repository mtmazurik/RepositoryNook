using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCA.Services.RepositoryNook.Exceptions;

namespace CCA.Services.RepositoryNook.HelperClasses
{
    public static class JsonExtensions
    {
        public static JProperty FindToken(this JToken container, string name)
        {
            foreach (JProperty token in container)
            {
                if (token.Name == name)                 // exits on first found
                {
                    return new JProperty(name, token.Value);
                }
            }
            throw new JsonTokenNotFoundError($"Error looking in Json container for Token: {name}.");
        }
        public static List<JToken> FindTokens(this JToken containerToken, string name)
        {
            List<JToken> matches = new List<JToken>();
            FindTokens(containerToken, name, matches);
            return matches;
        }

        private static void FindTokens(JToken containerToken, string name, List<JToken> matches)
        {
            if (containerToken.Type == JTokenType.Object)
            {
                foreach (JProperty child in containerToken.Children<JProperty>())
                {
                    if (child.Name == name)
                    {
                        matches.Add(child.Value);
                    }
                    FindTokens(child.Value, name, matches);
                }
            }
            else if (containerToken.Type == JTokenType.Array)
            {
                foreach (JToken child in containerToken.Children())
                {
                    FindTokens(child, name, matches);
                }
            }
        }
        
    }
}
