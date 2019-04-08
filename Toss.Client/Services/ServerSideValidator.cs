using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toss.Client.Services
{
    public class ServerSideValidator : ComponentBase
    {
        private ValidationMessageStore _messageStore;

        [CascadingParameter] EditContext CurrentEditContext { get; set; }

        /// <inheritdoc />
        protected override void OnInit()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(ServerSideValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(ServerSideValidator)} " +
                    $"inside an {nameof(EditForm)}.");
            }

            _messageStore = new ValidationMessageStore(CurrentEditContext);
            CurrentEditContext.OnValidationRequested += (s, e) => _messageStore.Clear();
            CurrentEditContext.OnFieldChanged += (s, e) => _messageStore.Clear(e.FieldIdentifier);
        }

        public void DisplayErrors(Dictionary<string, List<string>> errors)
        {
            foreach (var err in errors)
            {
                _messageStore.AddRange(CurrentEditContext.Field(err.Key), err.Value);
            }        
            CurrentEditContext.NotifyValidationStateChanged();
        }
    }
}
