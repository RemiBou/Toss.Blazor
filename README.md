[![Build Status](https://dev.azure.com/remibou/toss/_apis/build/status/RemiBou.Toss.Blazor?branchName=master)](https://dev.azure.com/remibou/toss/_build/latest?definitionId=1?branchName=master)

# Toss.Blazor
Twitter-like web application using Blazor 0.7.0. You can login, post a new message (a "toss") with hashtag and select your favorite hashtags for finding messages.

# Tech stack
- Blazor (0.7.0)
- Bootstrap 4
- Toastr
- Asp.net Core 2.1.1
- MediatR
- RavenDB

# Feature list
- Security pages : auth/open id, edit account, reset password
- Push new messages to the home page
- Filter message by hashtag
- Add hashtag to your profile
- Pushing ads to the home page

# Running this sample
- checkout the project
- run ravendb and set the ravendbendpoint/database name on the secret file
- Edit Toss.Server secrets with your values using the empty-secrets.json provided
- Build Toss.Server
- Run Toss.Server
- This sample uses mailjet as mail provider, edit MailService if you want to change that

You can find detailed explanation on my blog here : https://remibou.github.io/
