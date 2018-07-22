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
- Navigation : all the logs will be anonymized
- Hashtasg list : stored as plain-text, kept private
(I'll try to keep this) list up-to-date as I add features)

## It'll get hacked one day
Like every system (on computer or else) on earth I fail at implementing absolute security. So be careful about every data you put in this system :
- Password : use a unique password or better, a password manager. This is stored as a hash, but Math can (and will) lie?
- Email : make sure that if someone discovers your email nothing wrong could happen.
- Hashtag list : I'll try my best to keep it private but because it's stored as plain text, don't trust us.

## Business model
The business model will be around advertisement, so the user is the product : I sell user's eye time to the customer. But the following limitation will be here:
- Any user can buy an add (basically text+image+link) that will be displayed on all the pages for all the users
- Ads will be validated first
- Users will buy X views and clicks that will cost Y €
- The ad displaying is random. At first it will be the same ads for everybody but I might implement hashtag / language / country filter.

## Social media pitfalls
All the social networks have the following pitfalls :
- Troll : debate on social media are pointless, in Toss it's reduced at it's bare minimum : nothing. At best you'll be able to signal an offensive toss, but there is no comment mechanism.
- Harrassment : there is no way in toss to harass someone on it's publication. A policy against offensive content wil be created.
- Self promoting : the central notion of Toss is the content, not the user. So I'll do my best to avoid creation of a star system.
- Poor content : the content posting will be limited (like once an hour)
- Anonymity : we can't and shouldn't stop people from trying to be anonymous online, in some country it can save their lives. But a system of moderation will avoid people abusing it.

## Transparency

Because the content is from your mind, I should not hide you what I am doing with it, that's why this project is and will stay 100% Open Source. 

I'll also be transparent about cost of operation / breach detection ...