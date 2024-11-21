# Moonlander Networking

## Setup
Before making any calls you need to make sure you have an authorized application on the Moonlander API.
Contact david@mnlndr.com if you do not have a valid ClientID and Client secret setup.
You should add the correct ClientID and ClientSecret on the APIManager.cs file.
## Examples
### Authenticating on the Moonlnader API with username and password or auth token (license key)
```csharp
using Moonlander.Networking

...
//With username and password
APIManager.Authenticate("Username", "password", OnUserAuthenticated);
//With auth token
APIManager.Authenticate("Token", OnUserAuthenticated);

...

//error will be the error message in case the authentication failed or empty string if suceeded, success will be true if the autentication was successful.
//If the user is already authenticated you will get error="Already authenticated." and success=false
private void OnUserAuthenticated(string error, bool success){
    if(success)
    {
        // User is authenticated, from this point, you can call any API endpoints that require authentication.
        Debug.Log("Authentication successful.");
    }   
    else
    {
        Debug.Log("Authentication failed.")
    }
}
```

### Calling OpenAI ChatGPT
```csharp
using Moonlander.Networking

...
//Provide a valid API key
OpenAIConnector openAI = new OpenAIConnector(apiKey);

//Prepare the messages (prompt) to send
List<KeyValuePair<string, string>> messages = new List<KeyValuePair<string, string>>();
//"system" message primes the model on how to respond to you. This is optional but should be the first message.
messages.Add(new KeyValuePair<string, string>("system", "You are a JSON API server. You only respond in valid JSON."));
//You can pass a whole conversation as sets of messages alternating "user" and "assistant" 
//to either have ChatGPT remember conversation or to give examples of how you would like to be ansewered.
//The last "user" message will be the prompt. 
messages.Add(new KeyValuePair<string, string>("user", "Give me a dreamy summer color pallette"));

//Make the call
openAI.Chat("gpt-3.5-turbo", messages, ReceiveChat);

...

private void ReceiveChat(OpenAICompletionResponse result)
{
    //see Packages/com.mnlndr.moonlander/Networking/Runtime/OpenAI/OpenAIData.cs => OpenAICompletionResponse to see all options with this object.
    //Chat completions will have a message object and other completions will have a text object
    Debug.Log("Recived from OpenAI: " + result.choices[0].message.content);
}
```

### Calling OpenAI text Completions

```csharp
using Moonlander.Networking

...
//Provide a valid API key
OpenAIConnector openAI = new OpenAIConnector(apiKey);

//There are a lot of optional arguments to Complete(). check them out in OpenAIConnector.cs and https://platform.openai.com/docs/api-reference/completions/create
openAI.Complete("text-davinci-003", "Once upon a time ", ReceiveCompletion)

private void ReceiveCompletion(OpenAICompletionResponse result)
{
    //see Packages/com.mnlndr.moonlander/Networking/Runtime/OpenAI/OpenAIData.cs => OpenAICompletionResponse to see all types.
    //Chat completions will have a message object and other completions will have a text object
    //There might be multiple choices depending on the request options.
    Debug.Log("Recived from OpenAI: " + result.choices[0].text);
}
```