# TOSS ?
TOSS is an experimental project using AspNetCore and Blazor. Here is an overview of the project, many of the thing said here are not implemented yet but it gives you an overview 
of what we want to achieve here.

# Your Data
We'll gather personnal data as less as possible . All the data stored / processed are the following :
- Username : it's public
- Tosses (posts) : it's public,
- Password : it's stored but hashed by this code : [here](https://github.com/aspnet/Identity/blob/85f8a49aef68bf9763cd9854ce1dd4a26a7c5d3c/src/Core/PasswordHasher.cs){target=_blank}.
- Email : only here for security purpose (password recovery) it's stored as plain text but never showed.
- IP : for legal purpose we have to store the IP used for sending a toss
- Navigation : none is stored by the system itself, might be stored by the hosting provider (Microsoft Azure), but they don't use it if we don't ask.
- Hashtasg list : stored as plain-text, kept private

# Contact
If you have any question regarding this project or a suggestion please post an issue [here](https://github.com/RemiBou/Toss.Blazor/issues){target=_blank}, I'll try to answer as soon as possible.

# Security
Like every system on earth it fails at implementing absolute security. So be careful about every data you put in this system :
- Password : use a unique password or better, a password manager. This is stored as a hash, but Math can eventually lie one day.
- Email : make sure that if someone discovers your email nothing wrong could happen.
- Hashtag list : We'll try my best to keep it private.

# Business model
Because this project will make money with your content, we prefer to be transparent about how we do it, so the business model will be the following : 
- Any user can pay for displaying a Toss on the first slot. It will be displayed on the corresponding tag page
- Users will buy X views that will cost Y €

# Moderated content
We'll do our best to moderate the content posted here : every hateful content, fake news, illegal (in Europe at least), useless content will be removed. The posting chart will be created when people ill start to post here. 

# Social media pitfalls
All the social networks have the following pitfalls, this project will try to avoid them as much as possible :
- Troll : commenting on content has always been useless, from a quality of content point of view. if you want to answer to someone, post a new toss with the same hashtags.
- Harrassment : comment / PM are the first channel for harassmnent
- User engagement : online services are seeking user engagement at all cost : notification, email, app integration. This engagement causes a lot of problem in our society (very well described by Marc Gravell [here](https://blog.marcgravell.com/2018/12/a-thanksgiving-carol.html){target=_blank}).
- Self promoting : the central notion of Toss is the content, not the user. 
- Fake internet point : some user might be able to do a lot of thing just for earning fake point (likes, followers ...)
- Poor content : When people get money or fake point for posting content, they can post poor content to the system (fake news, repost, extremst point of view)

# How we stand
Here is my definition to how we stand compared to other social network / online service :
- Facebook : we don't ask for personnal data, and we are transparent about what we do with it.
- Twitter : we don't manipulate what is showed first, everything is ordered by date of post.
- Wikipedia : we are more oriented on one person point of view on something and the first thing shown is the latest
- Pinterest : we don't manipulate what is showed first, everything is ordered by date of post. it's more about textual content than pictures.
- Google+ : what ?
- Medium : more content is shown, no ability to comment, and oriented on medium sized content

# Transparency
Because the content is from your mind, we should not hide you what we are doing with it, that's why this project is and will stay open source.
The source code is available [here](https://github.com/RemiBou/Toss.Blazor){target=_blank}.
I'll also be transparent about cost of operation / breach detection etc ...