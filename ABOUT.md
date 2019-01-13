# Toss.Blazor
Experimental project using AspNetCore Blazor. Here is an overview of the project, many of the thing said here are not implemented yet but it gives you an overview 
of what I want to achieve here.

## Overview
Toss is an experimental website for helping people worldwide to share informations.

## Respect for users
No private / personnal data will be gathered. All the data stored / processed are the following :
- Username : it's public, removed if not used after a year
- Tosses (posts) : it's public, kept only a year
- Password : it's stored but hashed by this code : https://github.com/aspnet/Identity/blob/85f8a49aef68bf9763cd9854ce1dd4a26a7c5d3c/src/Core/PasswordHasher.cs.
- Email : only here for security purpose (password recovery) it's stored as plain text but never showed.
- IP :for legal purpose we have to store the IP used for sending a toss
- Navigation : all the logs will be anonymized
- Hashtasg list : stored as plain-text, kept private
(I'll try to keep this) list up-to-date as I add features.

## It'll get hacked one day
Like every system (on computer or else) on earth I fail at implementing absolute security. So be careful about every data you put in this system :
- Password : use a unique password or better, a password manager. This is stored as a hash, but Math can (and will) lie?
- Email : make sure that if someone discovers your email nothing wrong could happen.
- Hashtag list : I'll try my best to keep it private but because it's stored as plain text, don't trust us.

## Business model
The business model will be around paid content, so the user is the product : I sell user's eye time to the customer. But the following limitation will be here:
- Any user can buy an ad (a Toss) that will be displayed on all the pages for all the users
- Users will buy X views and clicks that will cost Y €
- The ad displaying is only on the corresponding tag page.

## Social media pitfalls
All the social networks have the following pitfalls :
- Troll : debate on social media are pointless, in Toss it's reduced at it's bare minimum : nothing. At best you'll be able to signal an offensive toss, but there is no comment mechanism.
- Harrassment : By removing the ability to contact someone directly and by giving the ability to signal an offending Toss, I  hope there won't be any harassment.
- User engagement : online services are seeking user engagement at all cost : notification, email, app integration. This engagement causes a lot of problem in our society (very well described by Marc Gravell [here](https://blog.marcgravell.com/2018/12/a-thanksgiving-carol.html))
- Self promoting : the central notion of Toss is the content, not the user. So I'll do my best to avoid creation of a star system.
- Fake internet point : by removing all the notions of point / fan / followers / stars / likes ... I hope to get rid of all the bad things we can do for 
- Poor content : the content posting will be limited (like once a day or an hour).

## Transparency

Because the content is from your mind, I should not hide you what I am doing with it, that's why this project is and will stay 100% Open Source. 

I'll also be transparent about cost of operation / breach detection ...