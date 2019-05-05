using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Toss.Shared.Tosses
{
    public class ConnectedTagsQuery : IRequest<ConnectedTags>
    {
        public ConnectedTagsQuery()
        {
        }

        public ConnectedTagsQuery(string hashtag)
        {
            Hashtag = hashtag ?? throw new ArgumentNullException(nameof(hashtag));
        }

        [Required]
        public string Hashtag { get; set; }
    }
}
