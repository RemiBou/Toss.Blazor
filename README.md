[![Build Status](https://dev.azure.com/remibou/toss/_apis/build/status/RemiBou.Toss.Blazor?branchName=master)](https://dev.azure.com/remibou/toss/_build/latest?definitionId=1?branchName=master)

# Toss.Blazor
Twitter-like web application using Blazor 3.0.0-preview7. You can login, post a new message (a "toss") with hashtag and select your favorite hashtags for finding messages.

# Tech stack
- Blazor 3.0.0-preview7
- Pure CSS
- Asp.net Core 3.0.0-preview7
- MediatR
- RavenDB

# Feature list
- Security pages : auth/open id, edit account, reset password
- Push new messages
- Filter message by hashtag
- Add hashtag to your profile
- Create sponsored (payinng) toss
- See most used tags
- See user message
- start private conversation with message author (in progress)
- report message as not respecting code of conduct (in progress)
- user personnal home page with all favorite tags (in progress)

# Running this project
```cmd
// install sdk version from global.json here https://dotnet.microsoft.com/download
git clone https://github.com/RemiBou/Toss.Blazor
dotnet build
docker-compose up -d ravendb
//Edit Toss.Server secrets with your values using the empty-secrets.json provided
cd Toss.Server
dotnet run
```

You can find explanation of the different things done on this project on my blog here : https://remibou.github.io/

The current roadmap is here https://trello.com/b/czBBGKsw/toss
