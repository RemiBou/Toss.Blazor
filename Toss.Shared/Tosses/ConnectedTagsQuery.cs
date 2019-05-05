using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Toss.Shared.Tosses
{
    public class ConnectedTagsQuery : IRequest<ConnectedTags>
    {
        public string Hashtag { get; set; }
    }
}
